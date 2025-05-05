using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.Controls.AxisControl;


/*
 * Author: [Nguyen, Kanyon]
 * Last Updated: [02/21/2025]
 * [handles input from both mouse and keyboard for dragging tiles]
 * [shoutout to https://youtu.be/zo1dkYfIJVg for the tutorial]
 */

public class InputHandler : MonoBehaviour
{
    private bool DebugMode = true;
    public InputActionAsset inputActions;
    private InputAction press, screenPos;
    
    private float zFighting = .5f;
    private float dragMultiplier = 1f;


    // for how far a tile has to be dragged before checking if it is on another tile or not
    private const float dragDistanceIncrement = 0.05f;


    Camera mainCamera;
    private GameManager gameManager;
    private GameGrid gameGrid;
    private MoveTracker moveTracker;
    private Powerups powerupManager;
    private LiquidTeleport liquidTeleport;
    private Vector3 currentScreenPos;
    private float maxX;
    private float maxY;
    private float gridSpacing;


    public Tile currentTile;
    public Transform currentTransform;

    public bool isDragging;
   


    // make sure only one drag is active at a time
    public Coroutine dragThread = null;


    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();
        moveTracker = (MoveTracker)FindObjectOfType<MoveTracker>();
        powerupManager = (Powerups)FindObjectOfType<Powerups>();
        liquidTeleport = (LiquidTeleport)FindObjectOfType<LiquidTeleport>();    
        //print(gameGrid.gridSizeX);

        gridSpacing = gameGrid.gridSpacing;
        maxX = gameGrid.GetMaxPosX();
        maxY = gameGrid.GetMaxPosY();

        mainCamera = Camera.main;

        press = inputActions.FindAction("Press");
        screenPos = inputActions.FindAction("ScreenPos");

        screenPos.Enable();
        press.Enable();

        screenPos.performed += context => { currentScreenPos = context.ReadValue<Vector2>(); };
        press.performed += _ => { if (isClickedOn)
            {
                if (dragThread == null)
                    dragThread = StartCoroutine(Drag());
            }
        };
        press.canceled += _ => { isDragging = false; };

    }


    /// <summary>
    /// for getting the position of the click and whatnot
    /// don't need z value cause we are working with a 2D plane
    /// </summary>
    public Vector3 currentWorldPos
    {
        get
        {
            //float thisZ = camera.WorldToScreenPoint(currentTransform.position).z;

            float clampedX = Mathf.Clamp(currentScreenPos.x, 0, Screen.width);
            float clampedY = Mathf.Clamp(currentScreenPos.y, 0, Screen.height);
            Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(clampedX, clampedY, mainCamera.nearClipPlane));
            return new Vector3(worldPoint.x, worldPoint.y, worldPoint.z);

            //return mainCamera.ScreenToWorldPoint(currentScreenPos + new Vector3(0, 0, 0));
        }
    }


    private Transform tokenTransform
    {
        get
        {
            return currentTile.currentToken.transform;
        }
    }



    /// <summary>
    /// searches for tile when click is pressed and returns whatever is pressed on
    /// </summary>
    private bool isClickedOn
    {
        get
        {
            liquidTeleport.ResetWaitDelay(); //resets gatorcam delay and turns it off when you press

            if (powerupManager.isDragging) return false;
            if (gameManager.isPaused) return false;
            if (isDragging) return false;

            if (moveTracker.MOVES_LEFT == 0) return false;
            

            Ray ray = mainCamera.ScreenPointToRay(currentScreenPos);
            RaycastHit hit;

            

            if (Physics.Raycast(ray, out hit))
            {
                Tile thisTile = hit.transform.gameObject.GetComponent<Tile>();

                if (DebugMode == true)
                {
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 0.5f);
                }

                if (thisTile != null)
                {
                    //print(hit.transform.name);
                    currentTile = thisTile;
                    currentTransform = hit.transform;
                    return true;
                }
                return false;
            }
            return false;
        }
    }
    


    /// <summary>
    /// returns a vector2 when the currentToken's position is currently dragged towards
    /// for x, -1 represents left and 1 represents right
    /// for y, -1 represents down and 1 represents up
    /// 0 means it hasn't moved on that axis
    /// im keeping this just in case i have to optimize the onTileSwap in the future
    /// </summary>
    /// <returns></returns>
    private Vector2 GetTokenDragDirection()
    {
        Transform tokenTransform = currentTile.currentToken.transform;

        Vector3 gridPosition = currentTile.GetGridPosition();
        Vector3 localPosition = tokenTransform.localPosition;

        // neat trick to get -1, 1, and 0 without having to do monkey coding
        float xDirection = Mathf.Sign(localPosition.x);
        float yDirection = Mathf.Sign(localPosition.y);

        if (Mathf.Abs(localPosition.x) > Mathf.Abs(localPosition.y))
        {
            return new Vector2(xDirection, 0);
        }
        else if (Mathf.Abs(localPosition.y) > Mathf.Abs(localPosition.x))
        {
            return new Vector2(0, yDirection);
        }

        return Vector2.zero;
    }




    // placeholder values
    private Vector3 lastLoggedPosition = Vector3.zero;
    private Tile lastHoveredTile = null;

    /// <summary>
    /// drag handler logic
    /// </summary>
    /// <returns></returns>
    private IEnumerator Drag()
    {
        // input started / pressed
        // getting position

        //print("input started");
        isDragging = true;
        Vector3 offset = currentTransform.position - currentWorldPos;

        if (currentTile.currentToken == null)
        {
            yield return null;
        }
        Transform tokenTransform = currentTile.currentToken.transform;
        gameManager.FreezeAllLiquids();

        Vector3 gridPosition = currentTile.GetGridPosition();

        while (isDragging)
        {
            // on drag logic
            //currentTransform.position = currentWorldPos + offset;

            Vector3 targetPosition = currentWorldPos + offset;
            
            // switch to location position cause the token is the child of the actual tile object
            Vector3 localTargetPosition = currentTransform.InverseTransformPoint(targetPosition);


            // check to see which of the 4 directions the block has moved furthest in
            if (Mathf.Abs(targetPosition.x - gridPosition.x) > Mathf.Abs(targetPosition.y - gridPosition.y))
            {
                // horizontal movement only
                float xClamp = Mathf.Clamp(localTargetPosition.x, 
                    -dragMultiplier * gridSpacing, 
                    dragMultiplier * gridSpacing);
                //wall clamp
                float finalClamp = Mathf.Clamp(xClamp, -maxX, maxX);
                

                tokenTransform.localPosition = new Vector3(finalClamp, 0, currentTile.GetGridPosition().z - zFighting);
            }
            else
            {
                // vertical movement only
                float yClamp = Mathf.Clamp(localTargetPosition.y, 
                    -dragMultiplier * gridSpacing, 
                    dragMultiplier * gridSpacing);
                float finalClamp = Mathf.Clamp(yClamp, -maxY, maxY);


                tokenTransform.localPosition = new Vector3(0, finalClamp, currentTile.GetGridPosition().z - zFighting);
            }

            //print(GetTokenDragDirection());

            // visual swap logic
            // check to see if dragged tile has moved a significant amount, but only if the tile is above it or towards it direciton
            float totalDistance = Vector3.Distance(tokenTransform.position, gridPosition);
            if (totalDistance >= gridSpacing)
            {
                //print(totalDistance);

                

                if (Vector3.Distance(tokenTransform.position, lastLoggedPosition) > dragDistanceIncrement)
                {
                    lastLoggedPosition = targetPosition;

                    List<int> targetCoordinates = gameGrid.GetCoordinatesFromPosition(tokenTransform.position);

                    Tile hoveredTile = gameGrid.GetTileFromCoordinates(targetCoordinates[0], targetCoordinates[1]);

                    if (hoveredTile != null && hoveredTile != currentTile && lastHoveredTile != hoveredTile)
                    {
                        if (lastHoveredTile != null)
                        {
                            lastHoveredTile.ResetPosition();
                        }

                        hoveredTile.SetTemporaryPosition(gridPosition);
                        lastHoveredTile = hoveredTile;
                    }
                }
            }
            else
            {
                if (lastHoveredTile != null)
                {
                    lastHoveredTile.ResetPosition();
                    lastHoveredTile = null;
                }
                lastLoggedPosition = gridPosition;
            }
            

            yield return null;
        }

        // input ended
        dragThread = null;

        gameManager.UnfreezeLiquids();

        OnTileSwap();
    }



    private void OnTileSwap()
    {
        if (lastHoveredTile != null)
        {
            lastHoveredTile.ResetPosition();
            lastHoveredTile = null;
        }
        lastLoggedPosition = Vector3.zero;

        Vector3 gridPosition = currentTile.GetGridPosition();
        Vector3 currentPosition = tokenTransform.position;
        float totalDistance = Vector3.Distance(currentPosition, gridPosition);

        // if it didn't move much at all, go back breh
        //print(totalDistance);

        currentTile.ResetPosition();

        if (totalDistance <= gridSpacing * 0.5)
        {
            return;
        }


        Vector3 direction = currentPosition - gridPosition;


        Tile targetTile;
        List<int> targetCoordinates = gameGrid.GetCoordinatesFromPosition(currentPosition);
        if (targetCoordinates != null && targetCoordinates.Count > 0)
        {

            //print("INITIAL: xpos " + targetCoordinates[0] + ", ypos " + targetCoordinates[1]);
            targetTile = gameGrid.GetTileFromCoordinates(targetCoordinates[0], targetCoordinates[1]);
            //print(targetTile);
        }
        else
        {
            currentTile.ResetPosition();
            return;
        }


        //print("swapped tiles");

        // check to see if there are matching tiles for either of the swapped tiles
        currentTile.SwapPositions(targetTile);
        List<Tile> matchingTiles1 = gameGrid.CheckForMatch(currentTile);
        List<Tile> matchingTiles2 = gameGrid.CheckForMatch(targetTile);

        HashSet<Tile> allMatchingTiles = new HashSet<Tile>();
        if (matchingTiles1 != null) allMatchingTiles.UnionWith(matchingTiles1);
        if (matchingTiles2 != null) allMatchingTiles.UnionWith(matchingTiles2);

        if (allMatchingTiles.Count > 1)
        {
            // if any matches, get rid of the ones that need to go
            foreach (Tile tile in allMatchingTiles)
            {
                gameGrid.DeleteTile(tile);
            }

            // successful tile swap
            gameManager.UpdateFans();
            moveTracker.OnMove();
        }
        else
        {
            // no matching tiles, dont delete anything breh
            // swap back
            currentTile.SwapPositions(targetTile);
        }


        
        //print("let go");
    }


}
