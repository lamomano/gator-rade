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

    public TextMeshProUGUI currentMovesLeft;
    private void Awake()
    {
        gameGrid = (GameGrid)FindObjectOfType<GameGrid>();
        gameManager = (GameManager)FindObjectOfType<GameManager>();
        currentMovesLeft.text = gameManager.MOVES_LEFT.ToString();
    }


    /// <summary>
    /// called when the input does a successfull move
    /// </summary>
    public void OnMove()
    {
        if(tokenDestroyed == true)
        {
            gameManager.MOVES_LEFT--;
            gameManager.amountOfMoves = gameManager.MOVES_LEFT;
            currentMovesLeft.text = gameManager.MOVES_LEFT.ToString();

            if(gameManager.MOVES_LEFT == 0)
            {
                Debug.Log("You lose");
            }
        }
    }
}
