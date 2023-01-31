using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UI;

public class ConcelableDetection : MonoBehaviour
{
    public ConcelableAreaInteractable concelableArea;
    public PlayerController player;
    public float hearingRange = 10f;

    public LayerMask obstructionMask;

    private static ConcelableDetection instance;

    public VorgonController vorgon;

    public float detectionSpeed = 0.8f;
    public float seeingDetectionSpeed = 0.4f;
    public Image hearingDetectionUI; // reference to the UI image on the canvas
    public CanvasGroup hearingCanvas;

    public Image seeingDetectionUI; // reference to the UI image on the canvas
    public CanvasGroup seeingCanvas;

    float hearingExposure;
    float exposure;


    private Color rayColor = Color.blue;

    public static ConcelableDetection Instance
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


    private void Update()
    {

        if (hearingExposure <= 0)
            hearingExposure = 0;

        if (hearingExposure > 1)
            hearingExposure = 1;

        if (exposure <= 0)
            exposure = 0;

        if (exposure > 1)
            exposure = 1;

        if (player.isHiding)
        {
            if (concelableArea != null)
            {
                float distance = Vector3.Distance(concelableArea.transform.position, transform.position);

                if (distance <= hearingRange)
                {
                    //exposure -= Time.deltaTime * detectionSpeed;

                    if (concelableArea.rotator.transform.localRotation.y > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.x > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.z > concelableArea.maxLocalRotationValue)
                        hearingExposure -= Time.deltaTime * detectionSpeed;
                    else if (concelableArea.exposurePercentage <= 0)
                    {
                        hearingExposure -= Time.deltaTime * detectionSpeed;
                    }
                    else if (concelableArea.rotator.transform.localRotation.y > 0 || concelableArea.rotator.transform.localRotation.x > 0 || concelableArea.rotator.transform.localRotation.z > 0)
                    {

                        if (concelableArea.doorCreak.isPlaying)
                        {
                            if (concelableArea.doorCreak.volume > 0.35f)
                            {
                                hearingExposure += Time.deltaTime * (detectionSpeed * concelableArea.exposurePercentage);
                                if (hearingCanvas.alpha == 1)
                                {
                                    Debug.Log("Vorgons coming dumbass");
                                    vorgon.SetLastDetectedLocation(concelableArea.searchPos.position, concelableArea.transform.position, VorgonController.EVENT_TYPE.SOUND);
                                }
                            }
                        }
                    }
                }
                else
                {
                    hearingExposure -= Time.deltaTime * detectionSpeed;
                }

                hearingCanvas.alpha = Mathf.Lerp(0, 1, hearingExposure);
                hearingDetectionUI.fillAmount = hearingExposure; // update the UI to match the detection level

                //fov check for seeing the concelable area. only react if exposed.


                Vector3 dir = (concelableArea.transform.position - transform.position).normalized;

                Debug.DrawRay(transform.position, dir, rayColor);

                Vector3 forwardV = transform.forward;
                float angle = Vector3.Angle(dir, forwardV);

                if (angle <= 45.0f)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, concelableArea.transform.position);

                    Debug.DrawRay(transform.position, dir, Color.yellow);

                    if (!Physics.Raycast(transform.position, dir, distanceToTarget, obstructionMask))
                    {
                        if (concelableArea.rotator.transform.localRotation.y > 0.25f || concelableArea.rotator.transform.localRotation.x > 0.25f || concelableArea.rotator.transform.localRotation.z > 0.25f)
                        {
                            //increase see reveal % and show ui depending on that. if exposure is over a certain amount the react
                            //need logic so this only happens once
                            //Debug.Log("I can see ur bitch ass");
                            exposure += Time.deltaTime * seeingDetectionSpeed;

                            //if (exposure == 1)
                            //    exposure = 1;

                            if (seeingCanvas.alpha == 1)
                            {
                                Debug.Log("I see you");
                            }

                            rayColor = Color.red;
                        }
                        else
                        {
                            exposure -= Time.deltaTime * seeingDetectionSpeed;
                            rayColor = Color.green;
                        }
                    }
                    else
                    {
                        exposure -= Time.deltaTime * seeingDetectionSpeed;
                        rayColor = Color.green;
                    }
                }
                else
                {
                    exposure -= Time.deltaTime * seeingDetectionSpeed;
                }

            }
            else
            {
                exposure -= Time.deltaTime * seeingDetectionSpeed;
                hearingExposure -= Time.deltaTime * detectionSpeed;
            }

            if (concelableArea.exposurePercentage <=0)
            {
                exposure -= Time.deltaTime * seeingDetectionSpeed;
            }

            seeingCanvas.alpha = Mathf.Lerp(0, 1, exposure);
            seeingDetectionUI.fillAmount = exposure; // update the UI to match the detection level

            //fov check
            //hearing check
        }
    }

    public void SetConcelableArea(ConcelableAreaInteractable newArea)
    {
        concelableArea = newArea;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }

}
