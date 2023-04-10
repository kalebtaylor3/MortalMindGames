using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public enum SCENES { MENU = 0 , GAME = 1, CREDITS = 2}

    
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
        else if(scene == SCENES.CREDITS)
        {
            StartCoroutine(LoadingProgress("Credits"));
        }
    }

    IEnumerator LoadingProgress(string scene)
    {
        progressSlider.value = 0;

        AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        float prog = 0;
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            prog = Mathf.MoveTowards(prog, async.progress, Time.unscaledDeltaTime);
            //prog = Mathf.Clamp01(async.progress / 0.9f);
            progressSlider.value = prog;

            if(prog >= 0.9f)
            {
                progressSlider.value = 1;
                async.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
