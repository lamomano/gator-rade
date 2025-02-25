using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestCanvas : MonoBehaviour
{

    
    public TMP_Text scoreText;
    public GameManager gameManager;

    void Start()
    {
        scoreText = gameObject.transform.Find("Score").GetComponent<TMP_Text>();

        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + gameManager.gatoradeCollected + "/" + gameManager.gatoradeAmount;
    }
}
