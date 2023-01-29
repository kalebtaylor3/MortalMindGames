using MMG;
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
    private float detection; // current detection level

    private void Start()
    {
        detection = 0f; // start with no detection
        hearingDetectionUI.fillAmount = detection; // update the UI to match the detection level
    }

    private void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position); // calculate distance between AI and player

        if (detection <= 0)
            detection = 0;

        if (distance <= hearingRange3)
        {
            //crouching range

            if (!player.movementInputData.IsCrouching && player.currentSpeed > 0)
                detection += Time.deltaTime * runningDetectionSpeed;
            else
            {
                if(player.currentSpeed == 1)
                    detection += Time.deltaTime * walkingDetectionSpeed;
                else if(player.currentSpeed > 0.7f)
                    detection += Time.deltaTime * crouchingDetectionSpeed;
                else if(player.currentSpeed < 0.7f)
                    detection -= Time.deltaTime * crouchingDetectionSpeed;
            }

            if (detection >= 1f) // if detection level is full
            {
                // AI can hear the player
                Debug.Log("vorgon can hear the player now");
                Vector3 detectionPosition = player.transform.position;
                vorgon.SetLastDetectedLocation(detectionPosition);
            }


        }
        else if (distance <= hearingRange2) // if player is within the second sphere's detection range
        {
            if (!player.movementInputData.IsCrouching)
            {
                if (player.currentSpeed >= 1.2f && player.currentSpeed < 5) // if the player is walking or running
                {
                    detection += Time.deltaTime * walkingDetectionSpeed; // increase detection level
                }
                else if (player.currentSpeed == 5)
                {
                    detection += Time.deltaTime * runningDetectionSpeed;
                }
                else if (player.currentSpeed < 1.2f)
                {
                    detection -= Time.deltaTime * walkingDetectionSpeed;
                }
            }
            else
            {
                detection -= Time.deltaTime * walkingDetectionSpeed;
            }

            if (detection >= 1f) // if detection level is full
            {
                // AI can hear the player
                Debug.Log("vorgon can hear the player now");
                Vector3 detectionPosition = player.transform.position;
                vorgon.SetLastDetectedLocation(detectionPosition);
            }
        }
        else if (distance <= hearingRange1) // if player is within the first sphere's detection range
        {
            if (player.movementInputData.IsRunning) // if the player is running
            {
                detection += Time.deltaTime * walkingDetectionSpeed; // increase detection level
            }
            else if (!player.movementInputData.IsRunning) // if the player is walking
            {
                detection -= Time.deltaTime * walkingDetectionSpeed; // decrease detection level
            }
            if (detection >= 1f) // if detection level is full
            {
                // AI can hear the player
                Debug.Log("vorgon can hear the player now");
                Vector3 detectionPosition = player.transform.position;
                vorgon.SetLastDetectedLocation(detectionPosition);
            }
        }
        else
        {
            detection = 0f; // reset detection level if player is out of range
        }
        hearingCanvas.alpha = Mathf.Lerp(0, 1, detection);
        hearingDetectionUI.fillAmount = detection; // update the UI to match the detection level
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
