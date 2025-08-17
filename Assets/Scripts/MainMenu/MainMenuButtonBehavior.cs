using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonBehavior : MonoBehaviour
{
    public int currentScene;
    public Animator mainMenuAnimator;
    public Animator[] otherMenuAnimators;
    public GameObject mainMenuGO;
    public float timer;


    public GameObject pausedMenu;
    public GameObject Minigame;
    public bool paused;


    // IN CASE OF BUGS WITH THE MAIN MENU ANIMATIONS

    /*void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R) && mainMenuGO.transform.position.x != 6.217656)
        {
            mainMenuAnimator.SetTrigger("Open");

            for (int i = 0; i < otherMenuAnimators.Length; i++)
            {
                otherMenuAnimators[i].SetTrigger("Close");
            }
        }
        else if (mainMenuGO.transform.position.x == 6.217656)
        {

        }

    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && paused == false)
        {
            pausedMenu.SetActive(true);
            Minigame.SetActive(false);
            paused = true;
            PauseButton();
        }
        else if (paused == true && Input.GetKeyDown(KeyCode.Escape))
        {
            pausedMenu.SetActive(false);
            Minigame.SetActive(true);
            paused = false;
            ResumeButton();
        }
    }


    public void QuitProgram()
    {
        Application.Quit();
        Debug.Log("The program closed successfully!");
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(currentScene);
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
    public void StartCustomDeckQuiz()
    {
        SceneManager.LoadScene(8);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    IEnumerator cooldown()
    {
        yield return new WaitForSeconds(timer);
    }
}

