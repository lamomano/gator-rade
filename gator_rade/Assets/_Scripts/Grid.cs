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


        //print(-(gridSpacing * gridSizeX) / 2);
        float startingX = -((gridSpacing * gridSizeX) / 2);
        float startingY = -((gridSpacing * gridSizeY) / 2);

        //print(startingX);
        //print(startingY);

        for (int col = 0; col < gridSizeY; col++)
        {
            for (int row = 0; row < gridSizeX; row++)
            {
                Vector3 targetPosition = new Vector3(
                    startingX + ((row + 1) * gridSpacing),
                    startingY + ((col + 1) * gridSpacing),
                    0
                );

                GameObject tileObject = Instantiate(tilePrefab, targetPosition, Quaternion.identity);

                Tile tile = tileObject.GetComponent<Tile>();
                tile.type = UnityEngine.Random.Range(1, 5);
                tile.UpdateAppearance();

                tiles.Add(tileObject);
            }
        }
    }
}
