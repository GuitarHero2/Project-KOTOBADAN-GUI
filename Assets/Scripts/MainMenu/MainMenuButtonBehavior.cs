using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButtonBehavior : MonoBehaviour
{
    public void QuitProgram()
    {
        Application.Quit();
        Debug.Log("The program closed successfully!");
    }
}
