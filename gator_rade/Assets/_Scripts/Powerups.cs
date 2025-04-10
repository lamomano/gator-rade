using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Powerups : MonoBehaviour
{
    // makes sure the powerup is at the front of the screen
    public int bombAmount = 5;

    public Dictionary<string, int> remainingUses = new Dictionary<string, int>()
    {
        {"Bomb", 0 },
    };
    private float zOffset = 5f;
    public GameObject bombPrefab;

    public bool isDragging = false;


    private InputHandler inputHandler;
    private GameGrid gameGrid;
    private InputAction press;

    private TMP_Text bombText;


    private GameObject currentDraggedObject;

    delegate void PowerupFunction(Tile targetTile);
    PowerupFunction powerupFunction;


    /// <summary>
    /// stores a function and a prefab for the given name of the powerup to use
    /// </summary>
    private struct Powerup
    {
        public Action function;
        public GameObject prefab;
        

        public Powerup(Action givenAction, GameObject givenPrefab)
        {
            function = givenAction;
            prefab = givenPrefab;
        }
    }

    private Dictionary<string, Powerup> powerups = new Dictionary<string, Powerup>();
    private void Awake()
    {
        inputHandler = (InputHandler)FindObjectOfType(typeof(InputHandler));
        gameGrid = (GameGrid)FindObjectOfType(typeof(GameGrid));



        bombText = transform.Find("Hotbar").Find("Powerup1").Find("Text").GetComponent<TMP_Text>();



        press = inputHandler.inputActions.FindAction("Press");

        press.Enable();
        press.canceled += _ => { isDragging = false; };

        powerups = new Dictionary<string, Powerup>
        {
            {"Bomb", new Powerup(Bomb, bombPrefab) },
        };

        ResetPowerupUses();
    }



    /// <summary>
    /// called whenever the game restarts
    /// </summary>
    public void ResetPowerupUses()
    {
        // reset bomb uses
        remainingUses["Bomb"] = bombAmount;

        UpdateButtonTexts();
        //print("reset uses");
    }



    private void UpdateButtonTexts()
    {
        bombText.text = "Bombs (" + remainingUses["Bomb"] + ")";
    }



    public void OnButtonPress(string actionName)
    {
        
        // roblox tech
        if (powerups.TryGetValue(actionName, out Powerup thisPowerup))
        {
            // check to see if the player has any bombs first
            if (remainingUses[actionName] > 0)
            {
                powerups[actionName].function.Invoke();
            }
            
        }
        else
        {
            print("Could not find powerup struct by the name of " +  actionName);
        }
    }






    private void Bomb()
    {
        //print("bomb clicked");

        StartDrag("Bomb");
        powerupFunction = BombEffect;
    }


    /// <summary>
    /// bomb breaks blocks in a T - shape
    /// </summary>
    /// <param name="targetTile"></param>
    private void BombEffect(Tile targetTile)
    {

        // middle section
        targetTile.type = (int)TileType.blank;
        targetTile.UpdateAppearance();

        // now break tiles in all 4 directions

        List<Tile> tileList = new List<Tile>();

        Tile upTile = gameGrid.GetTileFromCoordinates(targetTile.x, targetTile.y+1);
        if (upTile != null) 
            tileList.Add(upTile);

        // turns out you dont need to check to see if the item is null or not, it just wont add to the list :)
        Tile downTile = gameGrid.GetTileFromCoordinates(targetTile.x, targetTile.y - 1);
        tileList.Add(downTile);
        Tile leftTile = gameGrid.GetTileFromCoordinates(targetTile.x-1, targetTile.y);
        tileList.Add(leftTile);
        Tile rightTile = gameGrid.GetTileFromCoordinates(targetTile.x+1, targetTile.y);
        tileList.Add(rightTile);

        foreach (Tile tile in tileList)
        {
            if (tile == null) continue;
            tile.type = (int)TileType.blank;
            tile.UpdateAppearance();
        }

        remainingUses["Bomb"] -= 1;
        UpdateButtonTexts();
    }





    private void StartDrag(string givenName)
    {
        if (!isDragging)
        {

            currentDraggedObject = Instantiate(powerups[givenName].prefab, GetWorldPos(), Quaternion.identity);
            StartCoroutine(Drag());
        }
    }






    private Vector3 GetWorldPos()
    {
        Vector3 pointerPos = inputHandler.currentWorldPos;
        //print(pointerPos);
        return pointerPos;
    }





    /// <summary>
    /// tries to find a tile at the intended coordinate position, given the position.
    /// </summary>
    /// <returns></returns>
    private Tile FindTileAtPosition(Vector3 targetPosition)
    {
        // now get nearest grid tile
        List<int> coordinates = gameGrid.GetCoordinatesFromPosition(targetPosition);
        Tile targetTile = gameGrid.GetTileFromCoordinates(coordinates[0], coordinates[1]);
        if (targetTile != null && (targetTile.type != (int)TileType.blank || targetTile.type != (int)TileType.immovable))
        {
            return targetTile;
        }
        return null;
    }



    private IEnumerator Drag()
    {
        // input started / pressed
        // getting position

        //print("input started");
        isDragging = true;

        Transform thisTransform = currentDraggedObject.transform;
        //Vector3 offset = thisTransform.position - GetWorldPos();

        Vector3 targetPosition;

        do
        {
            // on drag logic

            //Vector3 targetPosition = GetWorldPos() + offset;
            targetPosition = GetWorldPos();
            targetPosition.z = -zOffset;
            //print(targetPosition);
            thisTransform.position = targetPosition;


            yield return null;
        }
        while (isDragging);

        //print("stopped dragging");
        // input ended
        Destroy(currentDraggedObject);
        currentDraggedObject = null;

        // check if within grid bounds
        //Bounds gridBoundary = new Bounds(new Vector3(-gameGrid.GetMaxPosX(), -gameGrid.GetMaxPosY(), -10), new Vector3(gameGrid.GetMaxPosX(), gameGrid.GetMaxPosY(), 10));
        //print(targetPosition);
        if ((targetPosition.x >= (-gameGrid.GetMaxPosX() - gameGrid.gridSpacing) && targetPosition.x <= (gameGrid.GetMaxPosX() + gameGrid.gridSpacing))
            &&
            (targetPosition.y >= (-gameGrid.GetMaxPosY() - gameGrid.gridSpacing) && targetPosition.y <= (gameGrid.GetMaxPosY() + gameGrid.gridSpacing)))
        {
            //print("powerup placed within grid");


            // now get nearest grid tile
            Tile targetTile = FindTileAtPosition(targetPosition);

            if (targetTile != null)
            {
                if (powerupFunction != null)
                {
                    powerupFunction(targetTile);
                }
            }
        }
    }
}
