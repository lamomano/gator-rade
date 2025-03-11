using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MoveTracker : MonoBehaviour
{

    private GameGrid gameGrid;
    private GameManager gameManager;

    public bool tokenDestroyed;

    public TMP_Text currentMovesLeft;
    public int MOVES_LEFT = 0;



    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();
        currentMovesLeft.text = MOVES_LEFT.ToString();
        tokenDestroyed = true;
    }

  


    /// <summary>
    /// called when the input does a successfull move
    /// </summary>
    public void OnMove()
    {
        if(tokenDestroyed == true)
        {
            MOVES_LEFT--;         
            currentMovesLeft.text = "Moves Left: " + MOVES_LEFT.ToString();

            if(MOVES_LEFT == 0)
            {
                Debug.Log("You lose");
            } 
        }
    }
  

    
}
