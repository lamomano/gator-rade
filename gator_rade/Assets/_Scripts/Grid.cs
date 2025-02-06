using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public enum NodeColor
{
    Red,
    Blue,
    Green,
    Yellow,
}




public class Grid : MonoBehaviour
{

    public int gridSizeX;
    public int gridSizeY;

    public float gridSpacing = 1.5f;


    public GameObject tilePrefab;

    public List<GameObject> tiles = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }
    }


    /// <summary>
    /// returns the bounding box for the x position
    /// </summary>
    /// <returns></returns>
    public float GetMaxPosX()
    {
        return ((gridSizeX - 1) * gridSpacing) / 2;
    }

    
    /// <summary>
    /// returns the bounding box for the y position
    /// </summary>
    /// <returns></returns>
    public float GetMaxPosY()
    {
        return ((gridSizeY - 1) * gridSpacing) / 2;
    }



    /// <summary>
    /// returns the position of where the tile would be given the coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 CalculateGridPosition(int row, int col)
    {
        float startingX = -GetMaxPosX();
        float startingY = -GetMaxPosY();

        return new Vector3(
             startingX + (row * gridSpacing),
             startingY + (col * gridSpacing),
             0
        );
    }


    public void GenerateGrid()
    {
        // wipe arraylist if any tiles remain
        if (tiles.Count != 0)
        {
            for (int i = 0;i < tiles.Count; i++)
            {

                Destroy(tiles[i]);
            }
            tiles.Clear();
        }

        Vector3 center = Vector3.zero;


        //print(startingX);
        //print(startingY);

        for (int col = 0; col < gridSizeY; col++)
        {
            for (int row = 0; row < gridSizeX; row++)
            {
                Vector3 targetPosition = CalculateGridPosition(row, col);

                GameObject tileObject = Instantiate(tilePrefab, targetPosition, Quaternion.identity);

                Tile tile = tileObject.GetComponent<Tile>();
                tile.type = UnityEngine.Random.Range(1, 5);
                tile.x = row;
                tile.y = col;
                tile.UpdateAppearance();

                tiles.Add(tileObject);
            }
        }
    }


    public Tile ReturnNearestTileAt(Vector3 givenPosition)
    {
        // roblox flashbacks
        GameObject closestTile = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < tiles.Count; i++)
        {
            float distance = (givenPosition - tiles[i].transform.position).magnitude;
            
            if (distance < closestDistance)
            {
                closestTile = tiles[i];
                closestDistance = distance;
            }
        }
        return closestTile.GetComponent<Tile>(); ;
    }
}
