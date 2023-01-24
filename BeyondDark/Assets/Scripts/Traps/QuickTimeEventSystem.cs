using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuickTimeEventSystem : MonoBehaviour
{
    public float maxTime = 4f;
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

    bool inEvent = false;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    void Start()
    {
        successMarker.rectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
    }

    void Update()
    {
        if (eventTriggered)
        {
            if(inEvent)
            {
                timer += Time.deltaTime * 4;
                fillRect.fillAmount = timer / maxTime;
            }

            if (timer >= successTime - successTimeRange * maxTime && timer <= successTime + successTimeRange * maxTime)
            {
                if (qteInput.SuccessKeyPressed)
                {
                    Debug.Log("Quick time event successful!");
                    //eventTriggered = false;
                    inEvent = false;
                    Success();
                }
            }
            else if (timer < successTime - successTimeRange * maxTime || timer > successTime + successTimeRange * maxTime)
            {
                if (qteInput.SuccessKeyPressed)
                {
                    Debug.Log("Quick time event failed!");
                    //eventTriggered = false;
                    inEvent = false;
                    Fail();
                }
            }
            if (timer >= maxTime)
            {
                Debug.Log("Quick time event failed!");
                eventTriggered = false;
                inEvent = false;
                Fail();
            }
        }
        else
        {
            successMarker.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            TriggerEvent();
    }

    public void TriggerEvent()
    {
        if (!inEvent)
        {
            StartCoroutine(WaitToTrigger());
        }
    }

    IEnumerator WaitToTrigger()
    {
        yield return new WaitForSeconds(0.1f);
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
        StartCoroutine(WaitToGoAway());
        OnSuccess?.Invoke();
    }

    void Fail()
    {
        //alert vorgon by playing a really loud fail sound.
        OnFailure?.Invoke();
        StartCoroutine(WaitToGoAway());
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
