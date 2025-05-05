using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Texture tutorialImage;
    private RawImage imageimageimageimage; // this is where the image will be placed

    public TMP_Text scoreText;
    public GameManager gameManager;
    public Canvas pauseCanvas;
    public Canvas winCanvas;
    public Canvas loseCanvas;
    private Canvas tutorialCanvas;
    private GameObject restartButton;
    private GameObject helpButton;

    
    

    private float restartCooldown = 1f;
    private bool debounce = false;

    void Awake()
    {
        scoreText = gameObject.transform.Find("Score").GetComponent<TMP_Text>();

        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));

        pauseCanvas = gameObject.transform.Find("PauseMenu").GetComponent <Canvas>();
        winCanvas = gameObject.transform.Find("WinScreen").GetComponent<Canvas>();
        loseCanvas = gameObject.transform.Find("LoseScreen").GetComponent<Canvas>();
        tutorialCanvas = gameObject.transform.Find("Tutorial").GetComponent<Canvas>();
        imageimageimageimage = tutorialCanvas.gameObject.transform.Find("Image").GetComponent<RawImage>();

        restartButton = gameObject.transform.Find("Restart").gameObject;
        helpButton = pauseCanvas.gameObject.transform.Find("Help").gameObject;

        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
        tutorialCanvas.enabled = false;
    }

    private void Start()
    {
        if (tutorialImage != null)
        {
            imageimageimageimage.texture = tutorialImage;
            ShowTutorial();
        }
        else
        {
            helpButton.gameObject.SetActive(false);
        }
    }


    public void UpdateUI()
    {
        scoreText.text = "Score: " + gameManager.gatoradeCollected + "/" + gameManager.gatoradeNeeded;
    }



    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(restartCooldown);
        debounce = false;
    }

    public void RestartLevel()
    {
        //if (pauseCanvas.enabled) return;
        if (debounce) return;
        if (tutorialCanvas.enabled) return;

        debounce = true;
        StartCoroutine(Cooldown());
        gameManager.Unpause();
        gameManager.NewRound();

        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
        tutorialCanvas.enabled = false;
        restartButton.SetActive(true);
    }

    public void ShowTutorial()
    {
        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
        restartButton.SetActive(false);
        tutorialCanvas.enabled = true;

        gameManager.Pause();
    }

    public void HideTutorial()
    {
        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        loseCanvas.enabled = false;
        restartButton.SetActive(true);
        tutorialCanvas.enabled = false;

        gameManager.Unpause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void TogglePause()
    {
        if (winCanvas.enabled) return;
        if (loseCanvas.enabled) return;
        if (tutorialCanvas.enabled) return;

        pauseCanvas.enabled = !pauseCanvas.enabled;
        restartButton.SetActive(!pauseCanvas.enabled);

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
        tutorialCanvas.enabled = false;

        winCanvas.enabled = true;
    }


    public void ShowLoseScreen()
    {
        if (winCanvas.enabled) return;
        pauseCanvas.enabled = false;
        winCanvas.enabled = false;
        tutorialCanvas.enabled = false;

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
