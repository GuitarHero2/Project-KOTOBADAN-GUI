using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DefinitionAnimatorManager : MonoBehaviour
{
    public int definitionSectionNumber;
    public Animator downwardsButtonAnimator;
    public Animator upwardsButtonAnimator;
    public Animator backButtonAnimator;
    void Update()
    {
        if (definitionSectionNumber == 1)
        {
            downwardsButtonAnimator.SetTrigger("Open");
        }
        
        if (definitionSectionNumber > 1 && definitionSectionNumber < 4)
        {
            upwardsButtonAnimator.SetTrigger("Open");
        }
        else
        {
            upwardsButtonAnimator.SetTrigger("Close");
        }
    }

    public void IncreaseSectionNumber()
    {
        definitionSectionNumber++;
    }
    
    public void DecreaseSectionNumber()
    {
        definitionSectionNumber--;
    }
}
