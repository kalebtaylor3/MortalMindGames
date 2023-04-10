using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerMenu : MonoBehaviour
{

    public bool credits;

    [SerializeField] GameObject loadScreen;

    private void Awake()
    {
        if(credits)
        {
            loadScreen.SetActive(true);
            loadScreen.GetComponent<LoadingScreen>().LoadNewScene(LoadingScreen.SCENES.CREDITS);
            //MenuManager.Instance.PauseMenuGO.SetActive(false);
            //SceneManager.LoadScene("Credits");
        }            
        else
        {
            loadScreen.SetActive(true);
            loadScreen.GetComponent<LoadingScreen>().LoadNewScene(LoadingScreen.SCENES.MENU);
            //SceneManager.LoadScene("Main Menu");
        }
            
    }
}
