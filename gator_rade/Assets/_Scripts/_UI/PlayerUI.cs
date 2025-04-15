using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{

    
    public TMP_Text scoreText;
    public GameManager gameManager;
    public Canvas pauseCanvas;
    public Canvas winCanvas;
    public Canvas loseCanvas;

    void Awake()
    {
        scoreText = gameObject.transform.Find("Score").GetComponent<TMP_Text>();

        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));

        pauseCanvas = gameObject.transform.Find("PauseMenu").GetComponent <Canvas>();
        winCanvas = gameObject.transform.Find("WinScreen").GetComponent<Canvas>();
        loseCanvas = gameObject.transform.Find("LoseScreen").GetComponent<Canvas>();

        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
    }



    public void UpdateUI()
    {
        scoreText.text = "Score: " + gameManager.gatoradeCollected + "/" + gameManager.gatoradeNeeded;
    }


    public void RestartLevel()
    {
        gameManager.Unpause();
        gameManager.NewRound();

        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void TogglePause()
    {
        if (winCanvas.enabled) return;
        if (loseCanvas.enabled) return;

        pauseCanvas.enabled = !pauseCanvas.enabled;

        if (pauseCanvas.enabled)
        {
            gameManager.Pause();
        }
        else
        {
            gameManager.Unpause();
        }
    }

    public void ShowWinScreen()
    {
        pauseCanvas.enabled = false;
        loseCanvas.enabled = false;

        winCanvas.enabled = true;
    }


    public void ShowLoseScreen()
    {
        pauseCanvas.enabled = false;
        winCanvas.enabled = false;

        loseCanvas.enabled = true;
    }





    public void NextLevel()
    {
        // check if there is a next level

        string sceneName = SceneManager.GetActiveScene().name;
        string numberPart = sceneName.Substring(6);

        // in case we test in test scene
        if (int.TryParse(numberPart, out int currentLevel))
            currentLevel = int.Parse(numberPart);
        else
            MainMenu();

        int nextLevel = currentLevel + 1;
        string nextSceneName = "Level " + nextLevel;

        // if the scene exists
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // no next level found, go back to main menu
            MainMenu(); 
        }
    }
}
