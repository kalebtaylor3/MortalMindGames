using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerStartTutorial : MonoBehaviour
{
    public TutorialTrigger tutorial;

    private void Awake()
    {
        TutorialController.instance.SetTutorial(tutorial.imageTut, tutorial.vidTut, 1);
    }
}
