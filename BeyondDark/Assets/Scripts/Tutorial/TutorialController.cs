using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialController : MonoBehaviour
{
    public static TutorialController instance;
    public Image textImage;
    public VideoPlayer tutVid;
    public Image fillImage;
    bool inTutorial = false;
    public GameObject canvasTut;
    public Animator tutAnimator;

    [HideInInspector] public bool firstPath = false;
    [HideInInspector] public bool secondPath = false;
    [HideInInspector] public bool thirdPath = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetTutorial(Sprite imageVal, VideoClip vidVal, float delay)
    {
        StartCoroutine(WaitForTut(imageVal, vidVal, delay));
    }

    private void Update()
    {
        if(inTutorial)
        {
            if(Gamepad.current.buttonSouth.isPressed)
            {
                fillImage.fillAmount += 1f * Time.unscaledDeltaTime;
            }
            else
            {
                if(fillImage.fillAmount != 1f)
                    fillImage.fillAmount -= 1f * Time.unscaledDeltaTime;
            }

            if (fillImage.fillAmount <= 0)
                fillImage.fillAmount = 0;

            if (fillImage.fillAmount == 1)
            {
                if(Gamepad.current.buttonSouth.wasReleasedThisFrame)
                {
                    Time.timeScale = 1;
                    Debug.Log("closing tutorial");
                    tutAnimator.SetBool("FadeOut", true);
                    StartCoroutine(WaitForFade());
                    inTutorial = false;
                }
            }

        }
    }

    IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(tutAnimator.GetCurrentAnimatorStateInfo(0).length);
        canvasTut.SetActive(false);
        MenuManager.Instance.canPause = true;
        //WorldData.Instance.gamePaused = false;
    }

    IEnumerator WaitForTut(Sprite imageVal, VideoClip vidVal, float delay)
    {
        MenuManager.Instance.canPause = false;
        //WorldData.Instance.gamePaused = true;
        fillImage.fillAmount = 0;
        yield return new WaitForSeconds(delay);
        tutAnimator.SetBool("FadeOut", false);
        inTutorial = true;
        textImage.sprite = imageVal;
        tutVid.clip = vidVal;
        canvasTut.SetActive(true);
        Time.timeScale = 0;
    }

}
