using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerMenu : MonoBehaviour
{
    private void Awake()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
