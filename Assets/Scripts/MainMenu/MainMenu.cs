using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        // TODO: Load prev save data
    }

    public void StartNewGame()
    {
        // TODO: reset prev save data 
        SceneManager.LoadScene("Lobby");
    }
    
    public void LoadGame()
    {
        // TODO: load prev data
        // TODO: load scene was prev data
        SceneManager.LoadScene("Lobby");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
