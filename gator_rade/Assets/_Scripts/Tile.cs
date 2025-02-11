using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Tile : MonoBehaviour
{
    // positions
    public int x;
    public int y;


    /*
     * red = 1
     * blue = 2
     * yellow = 3
     * green = 4
     * 
     */


    public int type = 1;


    private GameObject tileObject;
    private Grid gameGrid;


    public void Start()
    {
        if (tileObject == null)
        {
            tileObject = gameObject;
        }

        gameGrid = (Grid)FindObjectOfType<Grid>();

        UpdateAppearance();

        gameObject.AddComponent<DragHandler>();
    }


    private void ChangeColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }


    public void UpdateAppearance()
    {
        switch (type)
        {
            case 1:
                ChangeColor(Color.red);
                break;
            case 2:
                ChangeColor(Color.blue);
                break;
            case 3:
                ChangeColor(Color.yellow);
                break;
            case 4:
                ChangeColor(Color.green);
                break;
            default:
                print("not a valid type");
                break;
        }
    }

    public void SwapPositions(Tile targetTile)
    {
        int initialX = targetTile.x;
        int initialY = targetTile.y;
        Vector3 initialPos = targetTile.transform.position;


        targetTile.x = x;
        targetTile.y = y;
        targetTile.ResetPosition();

        x = initialX;
        y = initialY;
        ResetPosition();

        //print("swapping tiles");
    }

    public Vector3 GetGridPosition()
    {
        return gameGrid.CalculateGridPosition(x, y);
    }

    public void ResetPosition()
    {
        transform.position = GetGridPosition();
    }
}
