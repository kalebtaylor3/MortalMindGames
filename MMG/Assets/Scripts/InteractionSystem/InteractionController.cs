using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public class InteractionController : MonoBehaviour
    {
        #region Variables    

            [Space, Header("Data")]
            [SerializeField] private InteractionInputData interactionInputData = null;
            [SerializeField] private InteractionData interactionData = null;

            [Space, Header("UI")]
            [SerializeField] private InteractionUIPanel uiPanel;

            [Space, Header("Ray Settings")]
            [SerializeField] private float rayDistance = 0f;
            [SerializeField] private float raySphereRadius = 0f;
            [SerializeField] private LayerMask interactableLayer = ~0;

            private Camera cam;

            private bool isInteracting;
            private float holdTimer = 0f;
                

        #endregion

        #region Functions    
        void Awake()
        {
            cam = FindObjectOfType<Camera>();
        }

        void Update()
        {
            CheckForInteractable();
            CheckForInteractableInput();
        }        
        void CheckForInteractable()
        {
            Ray ray = new Ray(cam.transform.position,cam.transform.forward);
            RaycastHit hitInfo;

            bool hitSomething = Physics.SphereCast(ray,raySphereRadius, out hitInfo, rayDistance, interactableLayer);

            if(hitSomething)
            {
                InteractableBase interactable = hitInfo.transform.GetComponent<InteractableBase>();

                if(interactable != null)
                {
                    if(interactionData.IsEmpty())
                    {
                        interactionData.Interactable = interactable;
                        uiPanel.SetTooltip(interactable.TooltipMessage);
                    }
                    else
                    {
                        if(!interactionData.IsSameInteractable(interactable))
                        {
                            interactionData.Interactable = interactable;
                            uiPanel.SetTooltip(interactable.TooltipMessage);
                        }  
                    }
                }
            }
            else
            {
                uiPanel.ResetUI();
                interactionData.ResetData();
            }

            Debug.DrawRay(ray.origin,ray.direction * rayDistance,hitSomething ? Color.green : Color.red);
        }

        void CheckForInteractableInput()
        {
            if(interactionData.IsEmpty())
                return;

            if(interactionInputData.InteractedClicked)
            {
                isInteracting = true;
                holdTimer = 0f;
            }

            if(interactionInputData.InteractedReleased)
            {
                isInteracting = false;
                holdTimer = 0f;
                uiPanel.UpdateProgressBar(0f);
            }

            if(isInteracting)
            {
                if(!interactionData.Interactable.IsInteractable)
                    return;

                if(interactionData.Interactable.HoldInteract)
                {
                    holdTimer += Time.deltaTime;

                    float heldPercent = holdTimer / interactionData.Interactable.HoldDuration;
                    uiPanel.UpdateProgressBar(heldPercent);

                    if(heldPercent > 1f)
                    {
                        interactionData.Interact();
                        isInteracting = false;
                    }
                }
                else
                {
                    interactionData.Interact();
                    isInteracting = false;
                }
            }
        }
        #endregion
    }
}
