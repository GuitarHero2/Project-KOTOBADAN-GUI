using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonBehavior : MonoBehaviour
{
    public int currentScene;

    public void QuitProgram()
    {
        Application.Quit();
        Debug.Log("The program closed successfully!");
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
    }

    public void ResumeButton()
    {
        Time.timeScale = 1;
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(currentScene);
        Time.timeScale = 1;
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
    public void StartAllInQuiz()
    {
        SceneManager.LoadScene(7);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
