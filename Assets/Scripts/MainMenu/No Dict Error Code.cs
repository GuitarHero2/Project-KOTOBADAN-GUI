using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoDictErrorCode : MonoBehaviour
{
    public void OpenFileDirectory()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    public void CloseTheGame()
    {
        Application.Quit();
    }

}
