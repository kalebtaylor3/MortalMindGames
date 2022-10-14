using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    [CreateAssetMenu(fileName = "Interaction Data", menuName = "MMG_Player/Data/InteractionData")]
    public class InteractionData : ScriptableObject
    {
        private InteractableBase interactable;

        public InteractableBase Interactable
        {
            get => interactable;
            set => interactable = value;
        }

        public void Interact()
        {
            interactable.OnInteract();
            ResetData();
        }

        public bool IsSameInteractable(InteractableBase newInteractable) => interactable == newInteractable;
        public bool IsEmpty() => interactable == null;
        public void ResetData() => interactable = null;

    }
}
