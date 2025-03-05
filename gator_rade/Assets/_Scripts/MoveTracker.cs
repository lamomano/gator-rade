using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTracker : MonoBehaviour
{

    private GameGrid gameGrid;
    private GameManager gameManager;

    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();
    }


    /// <summary>
    /// called when the input does a successfull move
    /// </summary>
    public void OnMove()
    {
        
    }
}
