using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

using Random = UnityEngine.Random;


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
    private float BREAK_TIME = 0.25f;


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
    private Vector3 originalSize = Vector3.zero;


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
        // remove any tokens beforehand if it is not null
        if (currentToken != null)
        {
            Destroy(currentToken);
        }
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
        originalSize = currentToken.transform.localScale;
    }



    public void UpdateAppearance(bool KEEP_COLLIDER_ON = true)
    {
        boxCollider.enabled = KEEP_COLLIDER_ON;
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
                //gameObject.GetComponent<Renderer>().enabled = false;
                if (currentToken != null)
                {
                    StartCoroutine(BreakAnimation());
                }
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



    private IEnumerator BreakAnimation(GameObject targetBlock = null)
    {
        if (targetBlock == null)
            targetBlock = currentToken;
        float startTime = Time.time;


        //Vector3 targetSize = originalSize * 1.25f;
        Vector3 targetSize = targetBlock.transform.localScale * 1.25f;

        float growDuration = BREAK_TIME * 0.6f;
        float shrinkDuration = BREAK_TIME * 0.4f;

        float maxRotation = Random.Range(20f, 30f);
        float rotationSpeed = 4f;
        float lastRotationAngle = 0f;


        while (true)
        {
            yield return new();

            if (targetBlock == null)
                break;
            float elapsed = Time.time - startTime;
            float percentage = elapsed / BREAK_TIME;

            float newRotationAngle = Mathf.Sin(elapsed * rotationSpeed * Mathf.PI * 2f) * maxRotation;
            float deltaRotation = newRotationAngle - lastRotationAngle;
            targetBlock.transform.Rotate(0f, deltaRotation, 0f);
            lastRotationAngle = newRotationAngle;



            //targetBlock.transform.localScale = Vector3.Lerp(originalSize, originalSize * targetSize, percentage);

            if (elapsed < growDuration)
            {
                float growPercent = elapsed / growDuration;
                targetBlock.transform.localScale = Vector3.Lerp(originalSize, targetSize, growPercent);
            }
            else
            {
                float shrinkElapsed = elapsed - growDuration;
                float shrinkPercent = shrinkElapsed / shrinkDuration;
                targetBlock.transform.localScale = Vector3.Lerp(targetSize, Vector3.zero, shrinkPercent);
            }


            if (percentage > 1)
            {
                boxCollider.enabled = false;
                break;
            }
        }
        

        Destroy(targetBlock);
        currentToken = null;

        boxCollider.enabled = false;
        gameObject.GetComponent<Renderer>().enabled = false;
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


    /// <summary>
    /// for the key
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        Liquid thisLiquid = other.gameObject.GetComponent<Liquid>();
        if (thisLiquid != null)
        {
            thisLiquid.DeleteSelf();
            type = (int)TileType.blank;
            GameObject[] lockedWalls = GameObject.FindGameObjectsWithTag("Lock");

            // break locked walls
            foreach (GameObject obj in lockedWalls)
            {
                //Destroy(obj);
                StartCoroutine(BreakAnimation(obj));
            }
            UpdateAppearance(false);
             
        }
    }
}
