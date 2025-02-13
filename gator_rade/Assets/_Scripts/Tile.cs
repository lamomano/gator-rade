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
     * blank = -1
     */


    public int type = 1;


    private GameObject tileObject;
    private GameGrid gameGrid;
    private MeshRenderer meshRenderer;


    public void Start()
    {
        if (tileObject == null)
        {
            tileObject = gameObject;
        }

        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        UpdateAppearance();

        gameObject.AddComponent<DragHandler>();
    }

    public override string ToString() 
    {
        return new string("("+x +", "+y+")");

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
            case -1:
                meshRenderer.enabled = false;
                break;
            default:
                print("not a valid type");
                break;
        }
    }


    /// <summary>
    /// swaps this tile's x and y coordinates given a targettile
    /// </summary>
    /// <param name="targetTile"></param>
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


    /// <summary>
    /// sets the tile to the given coordinates
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void SetCoordinates(int row, int col)
    {
        x = row;
        y = col;
        ResetPosition();
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
