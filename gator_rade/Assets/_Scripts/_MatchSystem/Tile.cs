using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;




public enum TileType
{
    red = 1,
    blue = 2,
    yellow = 3,
    green = 4,
    blank = -1,
    immovable = 10,
    key = 20,
}

public class Tile : MonoBehaviour
{
    // positions
    public int x;
    public int y;

    public float tokenZOffset = 0.1f;


    /*
     * red = 1
     * blue = 2
     * yellow = 3
     * green = 4
     * 
     * blank = -1
     * immovable = 10
     * 
     */

    public GameObject blueToken;
    public GameObject greenToken;
    public GameObject yellowToken;
    public GameObject redToken;
    public GameObject deadToken;
    public GameObject keyPrefab;

    


    public int type = 1;


    private GameObject tileObject;
    private GameGrid gameGrid;
    private BoxCollider boxCollider;

    public GameObject currentToken = null;


    public void Awake()
    {

        if (tileObject == null)
        {
            tileObject = gameObject;
        }

        boxCollider = gameObject.GetComponent<BoxCollider>();
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        
        if (type != 10)
        {
            //dragHandler = gameObject.AddComponent<DragHandler>();
            //print("setup draghandler");
        }  


        UpdateAppearance();
    }

    public override string ToString() 
    {
        return new string("("+x +", "+y+")");
    }

    private void ChangeColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
        gameObject.GetComponent<Renderer>().enabled = true;

    }


    /// <summary>
    /// sets a token prefab to show for the given tile
    /// </summary>
    private void SetToken(GameObject givenToken)
    {
        currentToken = Instantiate(givenToken);

        currentToken.transform.position = transform.position - new Vector3(0,0, tokenZOffset);
        currentToken.transform.parent = transform;

        if (type != (int)TileType.immovable && type != (int)TileType.key)
        {
            currentToken.transform.localScale = new Vector3(gameGrid.tileSizePercentage, currentToken.transform.localScale.y, gameGrid.tileSizePercentage);
        }
        else if (type == (int)TileType.key)
        {
            currentToken.transform.Rotate(0, 180, 0);
        }
    }



    public void UpdateAppearance()
    {
        // remove any tokens beforehand if it is not null
        if (currentToken != null)
        {
            Destroy(currentToken);
            currentToken = null;
        }

        boxCollider.enabled = true;
        boxCollider.isTrigger = false;
        switch (type)
        {
            case (int)TileType.red:

                ChangeColor(Color.red);
                SetToken(redToken);

                break;
            case (int)TileType.blue:

                ChangeColor(Color.blue);
                SetToken(blueToken);

                break;
            case (int)TileType.yellow:

                ChangeColor(Color.yellow);
                SetToken(yellowToken);


                break;
            case (int)TileType.green:

                ChangeColor(Color.green);
                SetToken(greenToken);

                break;
            case (int)TileType.blank:

                boxCollider.enabled = false;
                gameObject.GetComponent<Renderer>().enabled = false;
                break;

            case (int)TileType.immovable:

                ChangeColor(Color.grey);
                SetToken(deadToken);

                break;

            case (int)TileType.key:
                SetToken(keyPrefab);
                boxCollider.isTrigger = true;
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



    /// <summary>
    /// swaps the tiles over to the given position without resetting the values on the gamegrid
    /// </summary>
    public void SetTemporaryPosition(Vector3 givenPosition)
    {
        if (currentToken != null)
        {
            // have to transform it to a local position value
            //currentToken.transform.position = transform.InverseTransformPoint(givenPosition);
            currentToken.transform.position = givenPosition;
        }
    }




    public Vector3 GetGridPosition()
    {
        return gameGrid.CalculateGridPosition(x, y);
    }

    public void ResetPosition()
    {
        Vector3 targetPosition = GetGridPosition();
        transform.position = targetPosition;
        if (currentToken != null) 
        {
            currentToken.transform.position = targetPosition;
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        Liquid thisLiquid = other.gameObject.GetComponent<Liquid>();
        if (thisLiquid != null)
        {
            thisLiquid.DeleteSelf();
            type = (int)TileType.blank;
            GameObject[] lockedWalls = GameObject.FindGameObjectsWithTag("Lock");

            foreach (GameObject obj in lockedWalls)
            {
                Destroy(obj);
            }
            UpdateAppearance();
        }
    }
}
