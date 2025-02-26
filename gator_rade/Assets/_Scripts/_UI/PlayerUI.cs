using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    
    public TMP_Text scoreText;
    public GameManager gameManager;

    void Awake()
    {
        scoreText = gameObject.transform.Find("Score").GetComponent<TMP_Text>();

        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
    }



    public void UpdateUI()
    {
        scoreText.text = "Score: " + gameManager.gatoradeCollected + "/" + gameManager.gatoradeNeeded;
    }


    public void RestartLevel()
    {
        gameManager.NewRound();
    }
}
