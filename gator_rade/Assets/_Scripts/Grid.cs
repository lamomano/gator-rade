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



    /// <summary>
    /// given a position, return the tile closest to that position
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Tile ReturnNearestTileAt(Vector3 givenPosition)
    {
        // roblox flashbacks
        GameObject closestTile = null;
        float closestDistance = Mathf.Infinity;
        for (int i = 0; i < tiles.Count; i++)
        {
            float distance = (givenPosition - tiles[i].GetComponent<Tile>().GetGridPosition()).magnitude;
            
            if (distance < closestDistance)
            {
                closestTile = tiles[i];
                closestDistance = distance;
            }
        }

        return closestTile.GetComponent<Tile>(); ;
    }





    /// <summary>
    /// goes through entire list to see if the coordinates of the grid are either 1 away on the x or y axis
    /// then checks to see if they are on the same axis on the other axis
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public List<Tile> GetAdjacentTiles(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        foreach (GameObject obj in tiles)
        {
            Tile otherTile = obj.GetComponent<Tile>();

            if (otherTile != null && otherTile != tile)
            {
                // check if they are in same row / col + grid
                if ((Mathf.Abs(otherTile.x - tile.x) == 1 && otherTile.y == tile.y) || (Mathf.Abs(otherTile.y - tile.y) == 1 && otherTile.x == tile.x))
                {
                    neighbors.Add(otherTile);
                }
            }
        }
        //print(neighbors.Count);
        return neighbors;
    }




    /// <summary>
    /// call it on one tile, and the tile will keep checking for matching neighbors until it cant anymore
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="matchingTiles"></param>
    /// <returns></returns>
    public List<Tile> MatchRecursive(Tile tile, List<Tile> matchingTiles)
    {
        // first of its kind woohoo
        if (matchingTiles == null)
        {
            matchingTiles = new List<Tile>();
        }
            

        if (matchingTiles.Contains(tile))
        {
            return matchingTiles;
        }
            

        matchingTiles.Add(tile);

        //check neighbors and see if they match
        List<Tile> neighbors = GetAdjacentTiles(tile);

        foreach (Tile neighbor in neighbors)
        {
            if (neighbor.type == tile.type)
            {
                MatchRecursive(neighbor, matchingTiles);
            }
        }

        return matchingTiles;
    }


    /// <summary>
    /// i love overloaded functions xddddd
    /// this is called when a tile's position is swapped and updated, so the system checks to see if a match was made or not
    /// </summary>
    /// <param name="givenPosition"></param>
    public List<Tile> CheckForMatch(Vector3 givenPosition)
    {
        List<Tile> matchingTiles = MatchRecursive(ReturnNearestTileAt(givenPosition), null);
        if (matchingTiles.Count > 2)
        {
            return matchingTiles;
        }
        return null; 
    }
    public List<Tile> CheckForMatch(Tile tile)
    {
        List<Tile> matchingTiles = MatchRecursive(tile, null);
        if (matchingTiles.Count > 2)
        {
            return matchingTiles;
        }
        return null;
    }



    /// <summary>
    /// deletes tile at given location while also clearing it from tiles list
    /// </summary>
    /// <param name="givenTile"></param>
    public void DeleteTile(Tile givenTile)
    {
        if (tiles.Contains(givenTile.gameObject))
        {
            tiles.Remove(givenTile.gameObject);

            Destroy(givenTile.gameObject);
        }
    }
}
