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

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(1);
    }
}
