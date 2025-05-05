using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
/*
 * Author: [Ihejirika, Chijioke; Nguyen, Kanyon]
 * Last Updated: [03/21/2025]
 * [Handles the changing and restarting of scenes in the game]
 */


public class MenuUI : MonoBehaviour
{

    private Canvas mainMenu;
    private Canvas levelSelect;
    private Canvas helpScreen;

    //public TMP_Text levelText;
    public int currentScene;

    private void Awake()
    {
        //currentScene = SceneManager.;

        mainMenu = transform.Find("MainMenu").GetComponent<Canvas>();
        levelSelect = transform.Find("LevelSelect").GetComponent<Canvas>();
        helpScreen = transform.Find("HelpScreen").GetComponent<Canvas>();

        
        mainMenu.enabled = true;
        levelSelect.enabled = true;
        helpScreen.enabled = true;
        
        EventSystem.current.SetSelectedGameObject(null);
        mainMenu.gameObject.SetActive(true);
        levelSelect.gameObject.SetActive(false);
        helpScreen.gameObject.SetActive(false);
    }



    /// <summary>
    /// shows the main menu
    /// </summary>
    public void ShowMainMenu()
    {
        //SceneManager.LoadScene("Main Menu");
        /*
        mainMenu.enabled = true;
        levelSelect.enabled = false;
        helpScreen.enabled = false;
        */
        EventSystem.current.SetSelectedGameObject(null);
        mainMenu.gameObject.SetActive(true);
        levelSelect.gameObject.SetActive(false);
        helpScreen.gameObject.SetActive(false);
    }


    /// <summary>
    /// shows the level select
    /// </summary>
    public void ShowLevelSelect()
    {
        /*
        mainMenu.enabled = false;
        levelSelect.enabled = true;
        helpScreen.enabled = false;
        */
        EventSystem.current.SetSelectedGameObject(null);
        mainMenu.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(true);
        helpScreen.gameObject.SetActive(false);
    }


    /// <summary>
    /// shows the help screen
    /// </summary>
    public void ShowHelpScreen()
    {
        /*
        mainMenu.enabled = false;
        levelSelect.enabled = false;
        helpScreen.enabled = true;
        */
        EventSystem.current.SetSelectedGameObject(null);
        mainMenu.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(false);
        helpScreen.gameObject.SetActive(true);
    }


    /// <summary>
    /// called when a level button is clicked in the level select menu
    /// </summary>
    public void LevelClicked()
    {
        SceneManager.LoadScene(EventSystem.current.currentSelectedGameObject.name);
    }


    /// <summary>
    /// when player presses the Quit button in teh pause menu, close the game down
    /// </summary>
    public void QuitGame()
    {
        //print("quitting game");
        Application.Quit();
    }



    /// <summary>
    /// when play button in main menu is clicked.
    /// automatically loads the first scene
    /// </summary>
    public void PlayButtonPressed()
    {
        SceneManager.LoadScene("Playtest1Tutorial");
    }




    public void Play()
    {
        SceneManager.LoadScene(2);
    }

    public void Level(int levelNumber)
    {
        SceneManager.LoadScene(1);
    }

    public void Level1Select()
    {
        SceneManager.LoadScene(2);
    }

    public void Level2Select()
    {
        SceneManager.LoadScene(3);
    }

    public void RestartLevel1()
    {
        SceneManager.LoadScene(2);
    }

    public void RestartLevel2()
    {
        SceneManager.LoadScene(3);
    }
}
