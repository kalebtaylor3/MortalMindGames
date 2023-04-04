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
            StartCoroutine(LoadingProgress("Main Menu"));
        }
        else if(scene == SCENES.GAME)
        {
            StartCoroutine(LoadingProgress("KalebMilestone4"));
        }
    }

    IEnumerator LoadingProgress(string scene)
    {
        async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);

        while (!async.isDone)
        {
            float prog = Mathf.Clamp01(async.progress / 0.9f);
            progressSlider.value = prog;

            yield return null;
        }
    }

    private void Update()
    {
        if (async != null)
        {
            //progressSlider.value = Mathf.Clamp01(async.progress / 0.9f);
        }
    }
}
