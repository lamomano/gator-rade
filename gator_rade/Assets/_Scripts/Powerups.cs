using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Powerups : MonoBehaviour
{
    // makes sure the powerup is at the front of the screen
    public float zOffset = -5f;
    public GameObject bombPrefab;

    public bool isDragging = false;


    private InputHandler inputHandler;
    private GameGrid gameGrid;
    private InputAction press;


    private GameObject currentDraggedObject;



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

        press = inputHandler.inputActions.FindAction("Press");

        press.Enable();
        press.canceled += _ => { isDragging = false; };

        powerups = new Dictionary<string, Powerup>
        {
            {"Bomb", new Powerup(Bomb, bombPrefab) },
        };
    }





    public void OnButtonPress(string actionName)
    {
        
        // roblox tech
        if (powerups.TryGetValue(actionName, out Powerup thisPowerup))
        {
            powerups[actionName].function.Invoke();
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
            print(targetPosition);
            thisTransform.position = targetPosition;


            yield return null;
        }
        while (isDragging);

        print("stopped dragging");
        // input ended
        Destroy(currentDraggedObject);
        currentDraggedObject = null;

        // check if within grid bounds
        //Bounds gridBoundary = new Bounds(new Vector3(-gameGrid.GetMaxPosX(), -gameGrid.GetMaxPosY(), -10), new Vector3(gameGrid.GetMaxPosX(), gameGrid.GetMaxPosY(), 10));
        print(targetPosition);
        if ((targetPosition.x >= (-gameGrid.GetMaxPosX() - gameGrid.gridSpacing) && targetPosition.x <= (gameGrid.GetMaxPosX() + gameGrid.gridSpacing))
            &&
            (targetPosition.y >= (-gameGrid.GetMaxPosY() - gameGrid.gridSpacing) && targetPosition.y <= (gameGrid.GetMaxPosY() + gameGrid.gridSpacing)))
        {
            print("powerup placed within grid");

            // now get nearest grid tile
            Tile targetTile = gameGrid.ReturnNearestTileAt(targetPosition);
            if (targetTile != null)
            {
                targetTile.type = -1;
                targetTile.UpdateAppearance();
            }
        }
    }
}
