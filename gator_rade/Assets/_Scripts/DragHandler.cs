using System.Collections;
using System.Collections.Generic;
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
        }
        else
        {
            tile.ResetPosition();
        }

        
    }
}
