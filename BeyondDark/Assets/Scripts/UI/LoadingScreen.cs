using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public enum SCENES { MENU = 0 , GAME = 1}

    AsyncOperation async = null;
    public Slider progressSlider;

    public void LoadNewScene(SCENES scene)
    {
        if(scene == SCENES.MENU)
        {
            async = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        }
        else if(scene == SCENES.GAME)
        {
            async = SceneManager.LoadSceneAsync("KalebMilestone4", LoadSceneMode.Single);
        }
    }

    private void Update()
    {
        if (async != null)
        {
            progressSlider.value = Mathf.Clamp01(async.progress / 0.9f);
        }
    }
}
