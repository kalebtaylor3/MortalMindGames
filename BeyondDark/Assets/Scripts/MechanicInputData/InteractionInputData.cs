using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MMG
{
    [CreateAssetMenu(fileName = "InventoryInputData", menuName = "MMG_Player/Data/InventoryInputData", order = 1)]
    public class InteractionInputData : ScriptableObject
    {
        private bool interactedClicked;
        private bool interactedRelease;

        public bool InteractedClicked
        {
            get => interactedClicked;
            set => interactedClicked = value;
        }

        public bool InteractedReleased
        {
            get => interactedRelease;
            set => interactedRelease = value;
        }

        public void ResetInput()
        {
            interactedClicked = false;
            interactedRelease = false;
        }
    }
}
