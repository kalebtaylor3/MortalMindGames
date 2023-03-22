using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTutorial : MonoBehaviour
{

    public TutorialTrigger tutorial;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            TutorialController.instance.SetTutorial(tutorial.imageTut, tutorial.vidTut, 0);
            Destroy(this);
        }
    }
}
