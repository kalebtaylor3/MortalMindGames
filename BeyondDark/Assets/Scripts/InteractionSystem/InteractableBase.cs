using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace MMG
{
    public class InteractableBase : MonoBehaviour, IInteractable
    {
        #region Events
        public static event Action<PickUp> OnPickUp;
        #endregion

        #region Variables    
        [Space,Header("Interactable Settings")]

        [SerializeField] private bool holdInteract = true;
        [ShowIf("holdInteract")][SerializeField] private float holdDuration = 1f;
            
        [Space] 
        [SerializeField] private bool multipleUse = false;
        [SerializeField] public bool isInteractable = true;

        [SerializeField] public string displayText = "interact";

        [SerializeField] private bool isPickup = false;
        [SerializeField] private bool isHidingSpot = false;

        public PickUp pickUp;

        #endregion

        #region Properties    
        public float HoldDuration => holdDuration; 

            public bool HoldInteract => holdInteract;
            public bool MultipleUse => multipleUse;
            public bool IsInteractable => isInteractable;

            public string TooltipMessage => displayText;

            public bool IsPickup => isPickup;
            public PickUp Pickup => pickUp;
        #endregion

        #region Functions

        private void Start()
        {
            if (!IsPickup)
                pickUp = null;
        }

        public virtual void OnInteract()
        {
                Debug.Log("INTERACTED: " + gameObject.name);

                if (IsPickup)
                    OnPickUp.Invoke(pickUp);
        }
        #endregion
    }
}
