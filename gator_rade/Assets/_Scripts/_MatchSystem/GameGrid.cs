using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;





public class GameGrid : MonoBehaviour
{

    public int gridSizeX;
    public int gridSizeY;

    public int wallHeight = 3;
    public float tileSizePercentage = .9f;
    public float gridSpacing = 1.5f;
    private int minimumTilesForMatch = 3;


    public GameObject tilePrefab;
    public GameObject wallPrefab;

    public List<GameObject> tiles = new List<GameObject>();
    public List<Vector3> DESIGNATED_TILES = new List<Vector3>();


    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        GenerateWalls();
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


    public Tile GetTileFromCoordinates(int row, int col)
    {

        foreach (GameObject tileGameObject in tiles)
        {
            Tile thisTile = tileGameObject.GetComponent<Tile>();

            if (thisTile.x ==  row && thisTile.y == col)
            {
                return thisTile;
            }
        }

        return null;
    }





    public void GenerateWalls()
    {
        // generate sides first

        for (int i = 0; i < gridSizeY; i++)
        {
            Vector3 left = CalculateGridPosition(-1, i);
            Vector3 right = CalculateGridPosition(gridSizeX, i);

            GameObject block1 = Instantiate(wallPrefab, left, Quaternion.identity);
            GameObject block2 = Instantiate(wallPrefab, right, Quaternion.identity);

            block1.transform.localPosition += new Vector3(0.5f, -0.5f, 0);
            block2.transform.localPosition += new Vector3(0.5f, -0.5f, 0);

            //block1.transform.localScale = new Vector3(tileSizePercentage, tileSizePercentage, tileSizePercentage);
            //block2.transform.localScale = new Vector3(tileSizePercentage, tileSizePercentage, tileSizePercentage);

        }

        // make the walls a lil higher

        for (int i = 0; i < wallHeight; i++)
        {
            Vector3 left = CalculateGridPosition(-1, i + gridSizeY);
            Vector3 right = CalculateGridPosition(gridSizeX, i + gridSizeY);

            GameObject block1 = Instantiate(wallPrefab, left, Quaternion.identity);
            GameObject block2 = Instantiate(wallPrefab, right, Quaternion.identity);

            block1.transform.localPosition += new Vector3(0.5f, -0.5f, 0);
            block2.transform.localPosition += new Vector3(0.5f, -0.5f, 0);
        }

        // then bottom corners
        Vector3 leftpos = CalculateGridPosition(-1, -1);
        Vector3 rightpos = CalculateGridPosition(gridSizeX, -1);

        GameObject leftCorner = Instantiate(wallPrefab, leftpos, Quaternion.identity);
        GameObject rightCorner = Instantiate(wallPrefab, rightpos, Quaternion.identity);

        leftCorner.transform.localPosition += new Vector3(0.5f, -0.5f, 0);
        rightCorner.transform.localPosition += new Vector3(0.5f, -0.5f, 0);

        // then floor
        for (int i = 0; i < gridSizeX; i++)
        {
            Vector3 targetPos = CalculateGridPosition(i, -1);
            GameObject floorBlock = Instantiate(wallPrefab, targetPos, Quaternion.identity);
            floorBlock.transform.localPosition += new Vector3(0.5f, -0.5f, 0);
        }
    }


    public void GenerateGrid()
    {
        // wipe arraylist if any tiles remain
        if (tiles.Count != 0)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                Destroy(tiles[i]);
            }
            tiles.Clear();
        }

        Vector3 center = Vector3.zero;

        // list for blocks taht shouldn't be generated on
        List<Vector2> nonoList = new List<Vector2>();


        // generate immovable tiles
        // or designated tiles
        foreach (Vector3 thisCoords in DESIGNATED_TILES)
        {
            int thisX = thisCoords.x.ConvertTo<int>();
            int thisY = thisCoords.y.ConvertTo<int>();

            Vector2 nonoPosition = new Vector2(thisX, thisY);
            if (nonoList.Contains(nonoPosition))
            {
                continue;
            }

            // make sure its not bigger than grid

            if (thisX > gridSizeX || thisY > gridSizeY)
            {
                print("("+thisX + ", "+thisY+") does not fit within the grid of "+gridSizeX+"x"+gridSizeY);
                continue;
            }
            int thisTileType = thisCoords.z.ConvertTo<int>();

            Vector3 targetPosition = CalculateGridPosition(thisX, thisY);

            GameObject tileObject = Instantiate(tilePrefab, targetPosition, Quaternion.identity);

            nonoList.Add(new Vector2(thisX, thisY));

            Tile tile = tileObject.GetComponent<Tile>();
            tile.type = thisTileType;
            tile.x = thisX;
            tile.y = thisY;
            tile.UpdateAppearance();
            
            if (tile.type != (int)TileType.immovable)
            {
                tiles.Add(tileObject);
            }
        }





        for (int col = 0; col < gridSizeY; col++)
        {
            for (int row = 0; row < gridSizeX; row++)
            {

                Vector2 nonoPosition = new Vector2(row, col);
                if (nonoList.Contains(nonoPosition))
                {
                    continue;
                }

                Vector3 targetPosition = CalculateGridPosition(row, col);

                GameObject tileObject = Instantiate(tilePrefab, targetPosition, Quaternion.identity);
                

                Tile tile = tileObject.GetComponent<Tile>();
                tile.x = row;
                tile.y = col;
                // make sure there are no 3 matches


                int maxAttempts = 25;
                int currentAttempts = 0;

                while (true)
                {
                    if (currentAttempts >= maxAttempts)
                    {
                        print("maxed out rip");
                        break;
                    }
                        
                    tile.type = UnityEngine.Random.Range(1, 5);

                    //print("new loop");

                    List<Tile> leftMatch = GetMatchesInDirection(tile, -1, 0);
                    List<Tile> rightMatch = GetMatchesInDirection(tile, 1, 0);
                    List<Tile> upMatch = GetMatchesInDirection(tile, 0, 1);
                    List<Tile> downMatch = GetMatchesInDirection(tile, 0, -1);

                    
                    int totalMatches = leftMatch.Count + rightMatch.Count + upMatch.Count + downMatch.Count;
                    //print(totalMatches);
                    if (totalMatches >= minimumTilesForMatch - 1)
                    {
                        currentAttempts += 1;
                        continue;
                    }
                        

                    break;
                }

                
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
    /// returns the coordinates of the nearest coordinate given the position
    /// </summary>
    /// <param name="givenPosition"></param>
    public List<int> GetCoordinatesFromPosition(Vector3 givenPosition)
    {
        // roblox flashbacks
        int closestX = -1;
        int closestY = -1;
        float closestDistance = Mathf.Infinity;


        for (int col = 0; col < gridSizeY; col++)
        {
            for (int row = 0; row < gridSizeX; row++)
            {
                Vector3 targetPosition = CalculateGridPosition(row, col);
                float distance = (givenPosition - targetPosition).magnitude;

                if (distance < closestDistance)
                {
                    closestX = row;
                    closestY = col;
                    closestDistance = distance;
                }
            }
        }


        if (closestX != -1 && closestY != -1)
        {
            List<int> coords = new List<int>();
            coords.Add(closestX);
            coords.Add(closestY);

            //checking order of placement
            //print(closestX);
            //print(coords[0]);
            //print(closestY);
            //print(coords[1]);

            return coords;
        }
        return null;
    }

    



    /// <summary>
    /// basically spams one direction to see how many matches are in the chain, given the x and y direction
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="xDirection"></param>
    /// <param name="yDirection"></param>
    /// <returns></returns>
    private List<Tile> GetMatchesInDirection(Tile tile, int xDirection, int yDirection)
    {
        List<Tile> matchingTiles = new List<Tile>();
        int currentX = tile.x;
        int currentY = tile.y;

        // keep going in the same direction until no more matches
        while (true)
        {
            currentX += xDirection;
            currentY += yDirection;
            Tile targetTile = GetTileFromCoordinates(currentX, currentY);

            if (targetTile == null || targetTile.type != tile.type)
                break;

            matchingTiles.Add(targetTile);
        }
        //print(matchingTiles.Count);
        return matchingTiles;
    }


    /// <summary>
    /// this is called when a tile's position is swapped and updated, so the system checks to see if a match was made or not
    /// only accepts vertically and horizontally matching tiles
    /// does not work if the tile is a blank tile though
    /// </summary>
    /// <param name="givenPosition"></param>
    public List<Tile> CheckForMatch(Tile tile)
    {
        if (tile.type == -1)
            return null;

        //List<Tile> matchingTiles = MatchRecursive(tile, null);

        // the ones that need to walk the plank
        List<Tile> targetTiles = new List<Tile>();

        // horizontal checks
        // have to do both left and right cause of T shape matches

        List<Tile> leftMatches = GetMatchesInDirection(tile, -1, 0);
        List<Tile> rightMatches = GetMatchesInDirection(tile, 1, 0);
        if (leftMatches.Count + rightMatches.Count >= minimumTilesForMatch - 1)
        {
            // add left and right matches + the included tile

            targetTiles.AddRange(leftMatches);
            targetTiles.AddRange(rightMatches);
            targetTiles.Add(tile);
        }

        // vertical matches XDDDDDDDDDDDD?

        List<Tile> upMatches = GetMatchesInDirection(tile, 0, 1);
        List<Tile> downMatches = GetMatchesInDirection(tile, 0, -1);
        if (upMatches.Count + downMatches.Count >= minimumTilesForMatch - 1)
        {
            // add left and right matches + the included tile

            targetTiles.AddRange(upMatches);
            targetTiles.AddRange(downMatches);
            targetTiles.Add(tile);
        }

        // remove any possible duplicates and send all the target tiles that need to be slaughtered
        // fancy technology :D
        targetTiles = targetTiles.Distinct().ToList();

        if (targetTiles.Count != 0)
        {
            //print("found matches");
            return targetTiles;
        }

        return null;
    }



    /// <summary>
    /// sets type to -1, indicating it is a blank tile and updating its appearance
    /// </summary>
    /// <param name="givenTile"></param>
    public void DeleteTile(Tile givenTile)
    {
        if (tiles.Contains(givenTile.gameObject))
        {
            givenTile.type = -1;
            givenTile.UpdateAppearance();
        }
    }

    public void DeleteTiles(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            DeleteTile(tile);
        }
    }
}
