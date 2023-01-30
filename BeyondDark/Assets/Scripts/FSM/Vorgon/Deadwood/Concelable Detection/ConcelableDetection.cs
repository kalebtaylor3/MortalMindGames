using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ConcelableDetection : MonoBehaviour
{
    public ConcelableAreaInteractable concelableArea;
    public PlayerController player;
    public float hearingRange = 10f;

    public LayerMask obstructionMask;

    private static ConcelableDetection instance;

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
        if(player.isHiding)
        {
            if (concelableArea != null)
            {
                float distance = Vector3.Distance(concelableArea.transform.position, transform.position);
                if (distance <= hearingRange)
                {
                    if (concelableArea.rotator.transform.localRotation.y > 0.15f || concelableArea.rotator.transform.localRotation.x > 0.15f || concelableArea.rotator.transform.localRotation.z > 0.15f)
                    {
                        if (concelableArea.exposurePercentage > 0.5f)
                        {
                            if (concelableArea.doorCreak.isPlaying)
                            {
                                Debug.Log("Vorgons coming dumbass");
                            }
                        }
                    }
                }

                //fov check for seeing the concelable area. only react if exposed.


                Vector3 dir = (concelableArea.transform.position - transform.position).normalized;

                Debug.DrawRay(transform.position, dir, Color.yellow);

                Vector3 forwardV = transform.forward;
                float angle = Vector3.Angle(dir, forwardV);

                if (angle <= 45.0f)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, concelableArea.transform.position);

                    if (!Physics.Raycast(transform.position, dir, distanceToTarget, obstructionMask))
                    {
                        if (concelableArea.rotator.transform.localRotation.y > 0.25f || concelableArea.rotator.transform.localRotation.x > 0.25f || concelableArea.rotator.transform.localRotation.z > 0.25f)
                            Debug.Log("I can see ur bitch ass");
                    }
                }
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
