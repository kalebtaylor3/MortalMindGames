using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerMenu : MonoBehaviour
{

    public bool credits;

    private void Awake()
    {
        if(credits)
            SceneManager.LoadScene("Credits");
        else
            SceneManager.LoadScene("Main Menu");
    }
}
