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

        print("input started");
        isDragging = true;
        Vector3 offset = currentTransform.position - currentWorldPos;

        Transform tokenTransform = currentTile.currentToken.transform;
        

        while (isDragging)
        {
            // on drag logic
            //currentTransform.position = currentWorldPos + offset;

            Vector3 targetPosition = currentWorldPos + offset;
            Vector3 gridPosition = currentTile.GetGridPosition();
            Vector3 localTargetPosition = currentTransform.InverseTransformPoint(targetPosition);

            // check to see which of the 4 directions the block has moved furthest in
            if (Mathf.Abs(targetPosition.x - gridPosition.x) > Mathf.Abs(targetPosition.y - gridPosition.y))
            {
                // horizontal movement only
                float xClamp = Mathf.Clamp(
                    targetPosition.x,
                    currentTile.GetGridPosition().x - dragMultiplier * gridSpacing,
                    currentTile.GetGridPosition().x + dragMultiplier * gridSpacing);

                float maxX = gameGrid.GetMaxPosX();
                //print(maxX);

                float boundedX = Mathf.Clamp(
                    xClamp,
                    -maxX,
                    maxX
                    );

                currentTransform.position = new Vector3(boundedX, currentTile.GetGridPosition().y, currentTile.GetGridPosition().z - zFighting); // z fighting HAH AHAHXD?D??
            }
            else
            {
                // vertical movement only
                float yClamp = Mathf.Clamp(
                    targetPosition.y,
                    currentTile.GetGridPosition().y - dragMultiplier * gridSpacing,
                    currentTile.GetGridPosition().y + dragMultiplier * gridSpacing);

                float maxY = gameGrid.GetMaxPosY();
                //print(maxY);

                float boundedY = Mathf.Clamp(
                    yClamp,
                    -maxY,
                    maxY
                    );

                currentTransform.position = new Vector3(currentTile.GetGridPosition().x, boundedY, currentTile.GetGridPosition().z - zFighting); // z fighting xddddd
            }



            yield return null;
        }

        // input ended
        dragThread = null;

        OnTileSwap();
    }



    private void OnTileSwap()
    {
        Vector3 gridPosition = currentTile.GetGridPosition();
        Vector3 currentPosition = currentTransform.position;
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

        if (targetTile != null && targetTile != currentTile)
        {
            currentTile.SwapPositions(targetTile);

            // check to see if there are matching tiles for both swapped tiles
            List<Tile> matchingTiles1 = gameGrid.CheckForMatch(currentTile);
            List<Tile> matchingTiles2 = gameGrid.CheckForMatch(targetTile);




            if (matchingTiles1 != null && matchingTiles2 != null)
            {
                // combine list of tiles that need to be deleted
                List<Tile> allMatchingTiles = new List<Tile>();
                allMatchingTiles = matchingTiles1.Union<Tile>(matchingTiles2).ToList<Tile>();

                if (allMatchingTiles.Count > 1)
                {
                    for (int i = 0; i < allMatchingTiles.Count; i++)
                    {
                        gameGrid.DeleteTile(allMatchingTiles[i]);
                    }
                }
            }
            else
            {
                if (matchingTiles1 != null && matchingTiles1.Count > 0)
                {
                    for (int i = 0; i < matchingTiles1.Count; i++)
                    {
                        gameGrid.DeleteTile(matchingTiles1[i]);
                    }
                }
                else if (matchingTiles2 != null && matchingTiles2.Count > 0)
                {
                    for (int i = 0; i < matchingTiles2.Count; i++)
                    {
                        gameGrid.DeleteTile(matchingTiles2[i]);
                    }
                }
                else
                {
                    // no matching tiles, dont delete anything breh
                    // swap back
                    currentTile.SwapPositions(targetTile);
                }
            }
        }
        else
        {
            //print("no match at all");
            currentTile.ResetPosition();
        }
        //print("let go");
    }


}
