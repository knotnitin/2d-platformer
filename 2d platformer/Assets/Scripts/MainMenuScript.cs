using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{

    
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    public void ControlsMenu()
    {
        SceneManager.LoadScene("Controls Scene");
    }

    public void MainMenuReturn()
    {
        Debug.Log("Returning to Main Menu");
        SceneManager.LoadScene("Main Menu");
    }
}
