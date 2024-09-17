using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugging : MonoBehaviour
{
    public GameObject saveIntoDictButton;
    public bool activateDebugging;

    void Start()
    {
        activateDebugging = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.D))
        {
            activateDebugging = true;
        }

        if (activateDebugging == true)
        {
            saveIntoDictButton.SetActive(true);
        }
    }
}
