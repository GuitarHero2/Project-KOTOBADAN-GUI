using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScreenSettings : MonoBehaviour
{
    //DEBUG
    public TMP_Text currentWidth;
    public TMP_Text currentHeight;

    public TMP_Text fullScreenMessage;

    void Update()
    {
        currentHeight.text = Screen.height.ToString();
        currentWidth.text = Screen.width.ToString();
        FullScreenChecker();
    }

    public void SetResolutionTo4K()
    {
        Screen.SetResolution(3840, 2160, Screen.fullScreen == false);
    }
    public void SetResolutionToQHD()
    {
        Screen.SetResolution(2560, 1440, Screen.fullScreen == false);
    }
    public void SetResolutionToFHD()
    {
        Screen.SetResolution(1920, 1080, Screen.fullScreen == false);
    }
    public void SetResolutionToHDPlus()
    {
        Screen.SetResolution(1600, 900, Screen.fullScreen == false);
    }
    public void SetResolutionToHDTV()
    {
        Screen.SetResolution(1366, 768, Screen.fullScreen == false);
    }
    public void SetResolutionToHD()
    {
        Screen.SetResolution(1280, 720, Screen.fullScreen == false);
    }

    public void FullScreenChecker()
    {
        if (Screen.fullScreen == true)
        {
            fullScreenMessage.text = "Fullscreen: ON";
        }
        else
        {
            fullScreenMessage.text = "Fullscreen: OFF";
        }
    }
}
