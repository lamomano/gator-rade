using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.Controls.AxisControl;

public class DragHandler : MonoBehaviour
{
    Vector3 mousePosition;


    private Tile tile;
    private Grid gameGrid;


    private float gridSpacing;

    public void Start()
    {
        tile = GetComponent<Tile>();
        gameGrid = (Grid)FindObjectOfType<Grid>();

        //print(gameGrid.gridSizeX);

        gridSpacing = gameGrid.gridSpacing;
    }

    private Vector3 GetMousePos()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }


    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag()
    {

        Vector3 gridPosition = tile.GetGridPosition();
        float multiplier = .9f; // testing to see what feels good

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);

        Vector3 direction = targetPosition - gridPosition;



        // check to see which of the 4 directions the block has moved furthest in
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {

            

            // horizontal movement only
            float xClamp = Mathf.Clamp(
                targetPosition.x,
                gridPosition.x - multiplier * gridSpacing,
                gridPosition.x + multiplier * gridSpacing);

            float maxX = gameGrid.GetMaxPosX();
            //print(maxX);

            float boundedX = Mathf.Clamp(
                xClamp,
                -maxX,
                maxX
                );

            transform.position = new Vector3(boundedX, gridPosition.y, gridPosition.z - 0.1f); // z fighting HAH AHAHXD?D??
        }
        else
        {
            

            // vertical movement only
            float yClamp = Mathf.Clamp(
                targetPosition.y,
                gridPosition.y - multiplier * gridSpacing,
                gridPosition.y + multiplier * gridSpacing);

            float maxY = gameGrid.GetMaxPosY();
            //print(maxY);

            float boundedY = Mathf.Clamp(
                yClamp,
                -maxY,
                maxY
                );

            transform.position = new Vector3(gridPosition.x, boundedY, gridPosition.z - 0.1f); // z fighting xddddd
        }
    }


    private void OnMouseUp()
    {
        //print("mouse up");


        Vector3 gridPosition = tile.GetGridPosition();

        Vector3 currentPosition = transform.position;
        float totalDistance = (currentPosition - gridPosition).magnitude;

        // if it didn't move much at all, go back breh
        //print(totalDistance);

        tile.ResetPosition();

        if (totalDistance <= gridSpacing/1.5)
        {
            return;
        }
       

        Vector3 direction = currentPosition - gridPosition;


        Tile targetTile = gameGrid.ReturnNearestTileAt(currentPosition);
        
        //print("swapped tiles");

        if (targetTile != null && targetTile != tile)
        {
            tile.SwapPositions(targetTile);

            // check to see if there are matching tiles for both swapped tiles
            List<Tile> matchingTiles1 = gameGrid.CheckForMatch(tile);
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
                    tile.SwapPositions(targetTile);
                }
            }
           

            
        }
        else
        {
            // check if tile is empty
            if (targetTile == null && tile != null)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    //horizontal
                    //check for left and right
                    if (direction.x <= 0)
                    {
                        //left
                    }
                    else
                    {
                        //right
                    }
                }
                else
                {
                    //vertical
                    if (direction.y <= 0)
                    {
                        //down

                    }
                    else
                    {
                        //up

                    }
                }
            }
            tile.ResetPosition();
        }

        
    }
}
