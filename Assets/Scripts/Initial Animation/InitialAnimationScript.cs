using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;

public class InitialAnimationScript : MonoBehaviour
{
    public TMP_Text mainText;
    public float floatChecker;
    public int timePassedAfterTextAnimation;
    public int timeBeforeSceneSkip;

    public int sceneIndex;



    void Start()
    {
        mainText.alpha = 0;

    }

    void Update()
    {
        floatChecker = mainText.alpha;

    }
    void FixedUpdate()
    {
        if (mainText.alpha >= 0 && mainText.alpha < 1)
        {
            mainText.alpha += 0.01f;
        }

        if (mainText.alpha >= 1)
        {
            timePassedAfterTextAnimation += 1;
        }

        if (timePassedAfterTextAnimation >= timeBeforeSceneSkip || Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}
