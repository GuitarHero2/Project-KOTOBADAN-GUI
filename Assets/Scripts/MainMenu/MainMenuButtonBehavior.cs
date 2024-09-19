using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonBehavior : MonoBehaviour
{
    public void QuitProgram()
    {
        Application.Quit();
        Debug.Log("The program closed successfully!");
    }

    public void StartN5Quiz()
    {
        SceneManager.LoadScene(2);
    }
    public void StartN4Quiz()
    {
        SceneManager.LoadScene(3);
    }
    public void StartN3Quiz()
    {
        SceneManager.LoadScene(4);
    }
    public void StartN2Quiz()
    {
        SceneManager.LoadScene(5);
    }
    public void StartN1Quiz()
    {
        SceneManager.LoadScene(6);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
