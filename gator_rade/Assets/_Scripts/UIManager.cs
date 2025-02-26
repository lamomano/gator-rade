using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
/*
 * Author: [Ihejirika, Chijioke]
 * Last Updated: [02/26/2025]
 * [Handles the changing and restarting of scenes in the game]
 */


public class UIManager : MonoBehaviour
{

    //public TMP_Text levelText;
    public int currentScene;

    private void Start()
    {
        //currentScene = SceneManager.;
    }
    public void Play()
    {
        SceneManager.LoadScene(2);
      
    }

    public void Level()
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
