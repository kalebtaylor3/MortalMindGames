using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
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

    public float flashSpeed = 0.2f;

    [HideInInspector] public float hearingExposure;
    [HideInInspector] public float exposure;

    bool detected = false;
    bool flashingHearing = false;
    bool flashingSight = false;
    bool happenOnce = false;

    public bool vorgonKnows = false;

    [HideInInspector] public bool playerDead = false;


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

    private void OnEnable()
    {
        happenOnce = false;
        flashingHearing = false;
        flashingSight = false;
        detected = false;
        vorgonKnows = false;
        exposure = 0;
        hearingExposure = 0;
    }


    private void Update()
    {

        if(!player.isHiding)
        {
            hearingExposure = 0;
            hearingCanvas.alpha = 0;
            seeingCanvas.alpha = 0;
            exposure = 0;
        }

        if (hearingExposure <= 0)
            hearingExposure = 0;

        if (hearingExposure >= 1)
        {
            hearingExposure = 1;
            //hearingDetectionUI.color = Color.red;
            if (!flashingHearing)
            {
                StartCoroutine(FlashHearing());
            }
        }
        else
        {
            hearingDetectionUI.color = Color.white;
        }

        if (exposure <= 0)
            exposure = 0;

        if (exposure >= 1)
        {
            exposure = 1;
            if (!happenOnce)
            {
                if (!flashingHearing)
                {
                    StartCoroutine(FlashSight(1));
                    happenOnce = true;
                }
            }
            //seeingDetectionUI.color = Color.red;
        }
        else
        {
            seeingDetectionUI.color = Color.white;
        }

        if (player.isHiding)
        {
            if (concelableArea != null)
            {
                if (!playerDead)
                {
                    float distance = Vector3.Distance(concelableArea.transform.position, vorgon.transform.position);

                    if (distance <= hearingRange)
                    {
                        //exposure -= Time.deltaTime * detectionSpeed;

                        if (concelableArea.rotator.transform.localRotation.y > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.x > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.z > concelableArea.maxLocalRotationValue)
                        {
                            if (!detected && !vorgonKnows)
                                hearingExposure -= Time.deltaTime * detectionSpeed;
                        }
                        else if (concelableArea.exposurePercentage <= 0)
                        {
                            if (!detected && !vorgonKnows)
                                hearingExposure -= Time.deltaTime * detectionSpeed;
                        }
                        else if (concelableArea.rotator.transform.localRotation.y > 0 || concelableArea.rotator.transform.localRotation.x > 0 || concelableArea.rotator.transform.localRotation.z > 0)
                        {

                            if (concelableArea.doorCreak.isPlaying)
                            {
                                if (concelableArea.doorCreak.volume > 0.35f)
                                {
                                    hearingExposure += Time.deltaTime * (detectionSpeed * concelableArea.exposurePercentage);
                                    if (hearingCanvas.alpha >= 1)
                                    {
                                        detected = true;

                                        StartCoroutine(WaitForDetected());
                                        Debug.Log("Vorgons coming dumbass");
                                        vorgon.SetLastDetectedLocation(concelableArea.searchPos.position, concelableArea, VorgonController.EVENT_TYPE.SOUND);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!detected && !vorgonKnows)
                            hearingExposure -= Time.deltaTime * detectionSpeed;
                    }
                }

                hearingCanvas.alpha = Mathf.Lerp(0, 1, hearingExposure);
                hearingDetectionUI.fillAmount = hearingExposure; // update the UI to match the detection level

                //fov check for seeing the concelable area. only react if exposed.

                if (!playerDead)
                {
                    Vector3 dir = (concelableArea.transform.position - vorgon.transform.position).normalized;

                    Debug.DrawRay(vorgon.transform.position, dir, rayColor);

                    Vector3 forwardV = vorgon.transform.forward;
                    float angle = Vector3.Angle(dir, forwardV);

                    if (angle <= 45.0f)
                    {
                        float distanceToTarget = Vector3.Distance(vorgon.transform.position, concelableArea.transform.position);

                        Debug.DrawRay(vorgon.transform.position, dir, Color.yellow);

                        if (!Physics.Raycast(vorgon.transform.position, dir, distanceToTarget, obstructionMask))
                        {
                            if (concelableArea.rotator.transform.localRotation.y > 0.25f || concelableArea.rotator.transform.localRotation.x > 0.25f || concelableArea.rotator.transform.localRotation.z > 0.25f)
                            {
                                //increase see reveal % and show ui depending on that. if exposure is over a certain amount the react
                                //need logic so this only happens once
                                //Debug.Log("I can see ur bitch ass");
                                exposure += Time.deltaTime / (distanceToTarget * 0.18f);

                                //if (exposure == 1)
                                //    exposure = 1;

                                if (seeingCanvas.alpha == 1)
                                {
                                    Debug.Log("I see you");
                                    vorgon.PlayerInSight = true;
                                }

                                rayColor = Color.red;
                            }
                            else
                            {
                                exposure -= Time.deltaTime * seeingDetectionSpeed;
                                vorgon.PlayerInSight = false;
                                rayColor = Color.green;
                                happenOnce = false;
                            }
                        }
                        else
                        {
                            exposure -= Time.deltaTime * seeingDetectionSpeed;
                            rayColor = Color.green;
                            happenOnce = false;
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

                if (concelableArea.exposurePercentage <= 0)
                {
                    exposure -= Time.deltaTime * seeingDetectionSpeed;
                }
            }

            seeingCanvas.alpha = Mathf.Lerp(0, 1, exposure);
            seeingDetectionUI.fillAmount = exposure; // update the UI to match the detection level

            //fov check
            //hearing check
        }
    }

    IEnumerator WaitForDetected()
    {
        yield return new WaitForSeconds(2);
        detected = false;
    }

    private IEnumerator FlashHearing()
    {
        flashingHearing = true;
        while (hearingExposure >= 1)
        {
            hearingDetectionUI.color = Color.red;
            yield return new WaitForSeconds(flashSpeed);
            hearingDetectionUI.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
        }
        flashingHearing = false;
    }

    private IEnumerator FlashSight(float duration)
    {
        flashingSight = true;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            seeingDetectionUI.color = Color.red;
            yield return new WaitForSeconds(flashSpeed);
            seeingDetectionUI.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
            elapsedTime += flashSpeed * 2;
        }
        seeingDetectionUI.color = Color.red;
        flashingSight = false;
    }


    public void SetConcelableArea(ConcelableAreaInteractable newArea)
    {
        concelableArea = newArea;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(vorgon.transform.position, hearingRange);
    }

}
