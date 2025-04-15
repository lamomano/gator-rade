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
    private PlayerUI playerUI;

    public bool tokenDestroyed;
    
    public TMP_Text currentMovesLeft;
    public int MOVES_LEFT = 10;
    public int StartMoves = 10;



    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();
        playerUI = (PlayerUI)FindObjectOfType<PlayerUI>();

        if (currentMovesLeft == null)
        {
            currentMovesLeft = playerUI.gameObject.transform.Find("MovesLeft").gameObject.GetComponent<TMP_Text>();
        }
        currentMovesLeft.text = "Moves Left: " + MOVES_LEFT.ToString();
        tokenDestroyed = true;
        
        
    }

   


    public void ResetMoves()
    {
        MOVES_LEFT = gameManager.amountOfMoves;
        currentMovesLeft.color = Color.black;
        currentMovesLeft.text = "Moves Left: " + MOVES_LEFT.ToString();
        
    }

    /// <summary>
    /// called when the input does a successfull move 
    /// </summary>
    public void OnMove()
    {

       

        if (tokenDestroyed == true)
        {
            MOVES_LEFT--;         
            currentMovesLeft.text = "Moves Left: " + MOVES_LEFT.ToString();

            if(MOVES_LEFT == 0)
            {
                //Debug.Log("You lose");
                //playerUI.ShowLoseScreen();
                currentMovesLeft.color = Color.red;
                currentMovesLeft.text = "Out of moves";
            }


        }



    }

  
   



}
