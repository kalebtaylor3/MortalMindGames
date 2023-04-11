using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEditor.Rendering;

public class QuickTimeEventSystem : MonoBehaviour
{
    public float maxTime = 3f;
    public float minTime = 2f;
    public float timer;
    public Image fillRect;
    public Image successMarker;

    private bool eventTriggered = false;
    private float successTime;
    public float successTimeRange = 0.05f; // 10% of maxTime

    public CanvasGroup uiCanvas;

    public float lerpTime = 2f; // time it takes to lerp from current alpha to 0
    private float currentLerpTime;

    [SerializeField] private GameObject UIPanel;
    [SerializeField] private QuickTimeEventInputData qteInput;

    public static event Action QTETrigger;
    public static event Action OnSuccess;
    public static event Action OnFailure;

    public AudioSource audioSource;
    public AudioClip failClip;
    public AudioClip passClip;

    public TMP_Text alertMessage;

    //public AudioClip completeClip;

    bool inEvent = false;
    bool canTrigger = true;

    private void OnEnable()
    {
        TrapInteractable.FailQTE += Fail;
        WorldData.OnDeath += OnDeath;
        inEvent = false;
        canTrigger = true;
        UIPanel.SetActive(false);
        alertMessage.alpha = 0;
        uiCanvas.alpha = 0;
    }

    private void OnDisable()
    {
        TrapInteractable.FailQTE -= Fail;
        WorldData.OnDeath -= OnDeath;
        //WorldData.OnDeath -= OnDeath;
    }

    private void Awake()
    {
        successMarker.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        inEvent = false;
        canTrigger = true;
        UIPanel.SetActive(false);
        alertMessage.alpha = 0;
        uiCanvas.alpha = 0;
    }

    void Start()
    {
        successMarker.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        inEvent = false;
        canTrigger = true;
        UIPanel.SetActive(false);
        alertMessage.alpha = 0;
        uiCanvas.alpha = 0;
    }

    void Update()
    {

        //Mathf.Clamp(successTime, 0.2f, 0.65f);

        if (canTrigger)
        {
            if (eventTriggered)
            {
                if (inEvent)
                {
                    timer += Time.deltaTime * 4;
                    fillRect.fillAmount = timer / maxTime;
                }

                if (timer >= successTime - successTimeRange * maxTime && timer <= successTime + successTimeRange * maxTime)
                {
                    if (qteInput.SuccessKeyPressed && inEvent && eventTriggered)
                    {
                        Debug.Log("Quick time event successful!");
                        //eventTriggered = false;
                        inEvent = false;
                        Success();
                    }
                }
                else if (timer < successTime - successTimeRange * maxTime || timer > successTime + successTimeRange * maxTime && inEvent)
                {
                    if (qteInput.SuccessKeyPressed && inEvent && eventTriggered)
                    {
                        Debug.Log("Quick time event failed!");
                        eventTriggered = false;
                        inEvent = false;
                        Fail(false);
                    }
                }
                if (timer >= maxTime)
                {
                    Debug.Log("Quick time event failed!");
                    eventTriggered = false;
                    inEvent = false;
                    Fail(false);
                }
            }
            else
            {
                successMarker.enabled = false;
            }
        }
    }

    public void TriggerEvent(float delay)
    {
        if (!inEvent)
        {
            StartCoroutine(WaitToTrigger(delay));
        }
    }

    IEnumerator WaitToTrigger(float delay)
    {
        yield return new WaitForSeconds(delay);
        fillRect.fillAmount = 0;
        successTime = UnityEngine.Random.Range(minTime, maxTime);
        timer = 0f;
        eventTriggered = true;
        successMarker.enabled = true;
        float successFillAmount = successTime / maxTime;
        float angle = -360 * successFillAmount;
        successMarker.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        uiCanvas.alpha = 1;
        UIPanel.SetActive(true);
        inEvent = true;
        StopAllCoroutines();
        QTETrigger?.Invoke();
    }

    void Success()
    {
        //arm the trap
        //audioSource.clip = passClip;
        audioSource.PlayOneShot(passClip);
        StartCoroutine(WaitToGoAway());
        OnSuccess?.Invoke();
        StartCoroutine(FadeOutAlert());
    }

    void Fail(bool death)
    {
        //alert vorgon by playing a really loud fail sound.
        inEvent = false;
        OnFailure?.Invoke();
        if (!death)
        {
            audioSource.PlayOneShot(failClip);
            StartCoroutine(WaitToGoAway());
            StartCoroutine(FadeAlert());
        }
        else
        {
            UIPanel.SetActive(false);
            alertMessage.alpha = 0;
            uiCanvas.alpha = 0;
        }
    }

    void OnDeath()
    {
        inEvent = false;
        eventTriggered = false;
        Fail(true);
        canTrigger = false;
        StopAllCoroutines();
        StartCoroutine(WaitForCanTrigger());
    }

    IEnumerator WaitForCanTrigger()
    {
        yield return new WaitForSeconds(2);
        canTrigger = true;
    }

    IEnumerator FadeAlert()
    {
        Color textColor = alertMessage.color;
        textColor.a = 0;
        alertMessage.color = textColor;
        while (textColor.a < 1)
        {
            textColor.a += 3 * Time.deltaTime;
            alertMessage.color = textColor;
            yield return null;
        }


        yield return new WaitForSeconds(2);

        StartCoroutine(FadeOutAlert());
    }

    IEnumerator FadeOutAlert()
    {

        Color textColor = alertMessage.color;
        while (textColor.a > 0)
        {
            textColor.a -= 1 * Time.deltaTime;
            alertMessage.color = textColor;
            yield return null;
        }
    }

    IEnumerator WaitToGoAway()
    {
        yield return new WaitForSeconds(1);
        eventTriggered = false;
        while (uiCanvas.alpha > 0)
        {
            uiCanvas.alpha = Mathf.LerpUnclamped(uiCanvas.alpha, 0, 4 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}
