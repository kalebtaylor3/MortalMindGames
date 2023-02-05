using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StealthDetection : MonoBehaviour
{
    public float hearingRange1 = 10f; // distance for first sphere detection
    public float hearingRange2 = 5f; // distance for second sphere detection
    public float hearingRange3 = 2f; // distance for second sphere detection
    public float runningDetectionSpeed = 1f; // detection speed when player is running
    public float walkingDetectionSpeed = 0.4f; // detection speed when player is walking
    public float crouchingDetectionSpeed = 0.1f;
    public Image hearingDetectionUI; // reference to the UI image on the canvas
    public CanvasGroup hearingCanvas;

    public VorgonController vorgon;

    public PlayerController player; // reference to the player's transform
    public float detection; // current detection level

    private static StealthDetection instance;

    public float flashSpeed = 0.2f;

    bool detected = false;

    bool flashing = false;

    public static StealthDetection Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Null");
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        detection = 0f; // start with no detection
        hearingDetectionUI.fillAmount = detection; // update the UI to match the detection level
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        hearingDetectionUI.color = Color.white;
        detection = 0f; // start with no detection
        hearingDetectionUI.fillAmount = detection;
    }

    private void Update()
    {
        
        float distance = Vector3.Distance(player.transform.position, transform.position); // calculate distance between AI and player

        if (detection <= 0)
        {
            detection = 0;
            vorgon.SetLastDetectedLocation(Vector3.zero, null, VorgonController.EVENT_TYPE.LOST);
        }

        if (detection >= 1)
        {
            detection = 1;
            //hearingDetectionUI.color = Color.red;
            if (!flashing)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            hearingDetectionUI.color = Color.white;
        }


        if (!vorgon.inChase && !vorgon.isAttacking)
        {
            if (distance <= hearingRange3)
            {
                //crouching range

                if (!player.movementInputData.IsCrouching && player.currentSpeed > 0)
                    detection += Time.deltaTime * runningDetectionSpeed;
                else
                {

                    if (player.currentSpeed == 1)
                        detection += Time.deltaTime * walkingDetectionSpeed;
                    else if (player.currentSpeed > 0.7f)
                        detection += Time.deltaTime * crouchingDetectionSpeed;
                    else if (player.currentSpeed < 0.7f && !detected)
                    {
                        detection -= Time.deltaTime * crouchingDetectionSpeed;
                        hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                    }
                }

                if (detection >= 1f) // if detection level is full
                {
                    // AI can hear the player
                    Debug.Log("vorgon can hear the player now");
                    Vector3 detectionPosition = player.transform.position;
                    vorgon.SetLastDetectedLocation(detectionPosition, null, VorgonController.EVENT_TYPE.SOUND);
                }
            }
            else if (distance <= hearingRange2) // if player is within the second sphere's detection range
            {
                if (!player.movementInputData.IsCrouching)
                {
                    if (player.currentSpeed >= 1.8f && player.currentSpeed < 5) // if the player is walking or running
                    {
                        detection += Time.deltaTime * walkingDetectionSpeed; // increase detection level
                    }
                    else if (player.currentSpeed == 5)
                    {
                        detection += Time.deltaTime * runningDetectionSpeed;
                    }
                    else if (player.currentSpeed < 1.8f && !detected)
                    {
                        detection -= Time.deltaTime * walkingDetectionSpeed;
                        hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                    }
                }
                else
                {
                    if (!detected)
                    {
                        detection -= Time.deltaTime * walkingDetectionSpeed;
                        hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                    }
                }


                if (detection >= 1f) // if detection level is full
                {
                    // AI can hear the player
                    Debug.Log("vorgon can hear the player now");
                    Vector3 detectionPosition = player.transform.position;
                    vorgon.SetLastDetectedLocation(detectionPosition, null, VorgonController.EVENT_TYPE.SOUND);
                    
                }
            }
            else if (distance <= hearingRange1) // if player is within the first sphere's detection range
            {
                if (player.movementInputData.IsRunning) // if the player is running
                {
                    detection += Time.deltaTime * walkingDetectionSpeed; // increase detection level
                }
                else if (!player.movementInputData.IsRunning && !detected) // if the player is walking
                {
                    detection -= Time.deltaTime * walkingDetectionSpeed; // decrease detection level
                    hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                }
                if (detection >= 1f) // if detection level is full
                {
                    // AI can hear the player
                    Debug.Log("vorgon can hear the player now");
                    Vector3 detectionPosition = player.transform.position;
                    vorgon.SetLastDetectedLocation(detectionPosition, null, VorgonController.EVENT_TYPE.SOUND);
                }
            }
            else
            {
                if (!detected)
                {
                    detection -= Time.deltaTime * runningDetectionSpeed; // reset detection level if player is out of range
                    hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                    vorgon.SetLastDetectedLocation(Vector3.zero, null, VorgonController.EVENT_TYPE.LOST);
                }
            }
        }
        else
        {
            if (!detected)
            {
                detection -= Time.deltaTime * runningDetectionSpeed; // decrease detection level
                hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
                vorgon.SetLastDetectedLocation(Vector3.zero, null, VorgonController.EVENT_TYPE.LOST);
            }
        }

        hearingCanvas.alpha = Mathf.Lerp(0, 1, detection);
        hearingDetectionUI.fillAmount = detection; // update the UI to match the detection level
    }

    public void SetDetection(float amount)
    {
        detected = true;
        detection += amount;
        hearingDetectionUI.fillAmount = Mathf.Lerp(0, amount, 0.95f * Time.deltaTime);
        hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.red, 0.95f * Time.deltaTime);
        //hearingDetectionUI.color = Color.red;
        StartCoroutine(WaitForDetection());
    }

    private IEnumerator Flash()
    {
        flashing = true;
        while (detection >= 1)
        {
            hearingDetectionUI.color = Color.red;
            yield return new WaitForSeconds(flashSpeed);
            hearingDetectionUI.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
        }
        flashing = false;
    }

    IEnumerator WaitForDetection()
    {
        yield return new WaitForSeconds(2);
        detected = false;
        hearingDetectionUI.color = Color.Lerp(hearingDetectionUI.color, Color.white, 0.95f * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hearingRange1);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange2);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRange3);
    }

}
