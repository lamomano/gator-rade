using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


/*
 * Author: [Nguyen, Kanyon]
 * Last Updated: [02/21/2025]
 * [handles input from both mouse and keyboard for dragging tiles]
 * [shoutout to https://youtu.be/zo1dkYfIJVg for the tutorial]
 */

public class InputHandler : MonoBehaviour
{
    public InputActionAsset inputActions;
    private InputAction press, screenPos;
    
    private float zFighting = .5f;
    private float dragMultiplier = 1f;


    Camera mainCamera;
    private GameManager gameManager;
    private GameGrid gameGrid;

    private Vector3 currentScreenPos;
    private float gridSpacing;


    public Tile currentTile;
    public Transform currentTransform;

    private bool isDragging;
   


    // make sure only one drag is active at a time
    public Coroutine dragThread = null;


    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();

        //print(gameGrid.gridSizeX);

        gridSpacing = gameGrid.gridSpacing;

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
    private Vector3 currentWorldPos
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
            Ray ray = mainCamera.ScreenPointToRay(currentScreenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Tile thisTile = hit.transform.gameObject.GetComponent<Tile>();
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
    

    private IEnumerator Drag()
    {
        // input started / pressed
        // getting position

        //print("input started");
        isDragging = true;
        Vector3 offset = currentTransform.position - currentWorldPos;

        Transform tokenTransform = currentTile.currentToken.transform;
        gameManager.FreezeAllLiquids();

        while (isDragging)
        {
            // on drag logic
            //currentTransform.position = currentWorldPos + offset;

            Vector3 targetPosition = currentWorldPos + offset;
            Vector3 gridPosition = currentTile.GetGridPosition();
            // switch to location position cause the token is the child of the actual tile object
            Vector3 localTargetPosition = currentTransform.InverseTransformPoint(targetPosition);


            // check to see which of the 4 directions the block has moved furthest in
            if (Mathf.Abs(targetPosition.x - gridPosition.x) > Mathf.Abs(targetPosition.y - gridPosition.y))
            {
                // horizontal movement only
                float xClamp = Mathf.Clamp(localTargetPosition.x, 
                    -dragMultiplier * gridSpacing, 
                    dragMultiplier * gridSpacing);

                tokenTransform.localPosition = new Vector3(xClamp, 0, currentTile.GetGridPosition().z - zFighting);
            }
            else
            {
                // vertical movement only
                float yClamp = Mathf.Clamp(localTargetPosition.y, 
                    -dragMultiplier * gridSpacing, 
                    dragMultiplier * gridSpacing);

                tokenTransform.localPosition = new Vector3(0, yClamp, currentTile.GetGridPosition().z - zFighting);
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
        Vector3 gridPosition = currentTile.GetGridPosition();
        Vector3 currentPosition = tokenTransform.position;
        float totalDistance = Vector3.Distance(currentPosition, gridPosition);

        // if it didn't move much at all, go back breh
        //print(totalDistance);

        currentTile.ResetPosition();

        if (totalDistance <= gridSpacing / 1.5)
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
