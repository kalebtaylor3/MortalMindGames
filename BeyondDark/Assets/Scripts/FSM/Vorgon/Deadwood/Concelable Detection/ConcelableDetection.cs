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

    public float detectionSpeed = 0.4f;
    public Image hearingDetectionUI; // reference to the UI image on the canvas
    public CanvasGroup hearingCanvas;

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

        if (exposure <= 0)
            exposure = 0;

        if (exposure > 1)
            exposure = 1;

        if(player.isHiding)
        {
            if (concelableArea != null)
            {
                float distance = Vector3.Distance(concelableArea.transform.position, transform.position);

                if (distance <= hearingRange)
                {
                    //exposure -= Time.deltaTime * detectionSpeed;

                    if (concelableArea.rotator.transform.localRotation.y > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.x > concelableArea.maxLocalRotationValue || concelableArea.rotator.transform.localRotation.z > concelableArea.maxLocalRotationValue)
                        exposure -= Time.deltaTime * detectionSpeed;
                    else if (concelableArea.exposurePercentage <= 0)
                    {
                        exposure -= Time.deltaTime * detectionSpeed;
                    }
                    else if (concelableArea.rotator.transform.localRotation.y > 0 || concelableArea.rotator.transform.localRotation.x > 0 || concelableArea.rotator.transform.localRotation.z > 0)
                    {

                        if (concelableArea.doorCreak.isPlaying)
                        {
                            if (concelableArea.doorCreak.volume > 0.3f)
                            {
                                exposure += Time.deltaTime * (detectionSpeed * concelableArea.exposurePercentage);
                                if (hearingCanvas.alpha == 1)
                                {
                                    Debug.Log("Vorgons coming dumbass");
                                    vorgon.SetLastDetectedLocation(concelableArea.transform.position, VorgonController.EVENT_TYPE.SOUND);
                                }
                            }
                        }
                    }
                }
                else
                {
                    exposure -= Time.deltaTime * detectionSpeed;
                }

                hearingCanvas.alpha = Mathf.Lerp(0, 1, exposure);
                hearingDetectionUI.fillAmount = exposure; // update the UI to match the detection level

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
                            //need logic so this only happens once
                            Debug.Log("I can see ur bitch ass");
                            rayColor = Color.red;
                        }
                        else
                            rayColor = Color.green;
                    }
                    else
                    {
                        rayColor = Color.green;
                    }
                }
            }
            else
            {
                exposure -= Time.deltaTime * detectionSpeed;
            }

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
