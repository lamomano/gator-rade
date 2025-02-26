using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


/*
 * Author: [Nguyen, Kanyon]
 * Last Updated: [11/13/2024]
 * [Handles the functionality of the main menu that the player sees when they first enter the game. Also handles functionality for level select]
 */


public class MainMenu : MonoBehaviour
{

    private Canvas mainMenu;
    private Canvas levelSelect;


    // Start is called before the first frame update
    void Start()
    {
        mainMenu = transform.Find("Main").GetComponent<Canvas>();
        levelSelect = transform.Find("LevelSelect").GetComponent<Canvas>();

        showMainMenu();
    }


    /// <summary>
    /// shows the main meun and hides the level select
    /// </summary>
    public void showMainMenu()
    {
        mainMenu.enabled = true;
        levelSelect.enabled = false;
    }


    /// <summary>
    /// hides the main menu and shows the level select
    /// </summary>
    public void showLevelSelect()
    {
        mainMenu.enabled = false;
        levelSelect.enabled = true;
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
    /// when player presses the play button, send them straight into the game
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Level01");
    }


    /// <summary>
    /// called whenever a button containing a level is called. transports player to that specified scene using the name of the button
    /// </summary>
    public void levelClicked()
    {
        SceneManager.LoadScene(EventSystem.current.currentSelectedGameObject.name);
    }

}
