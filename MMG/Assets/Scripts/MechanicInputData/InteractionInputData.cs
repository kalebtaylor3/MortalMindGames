using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MMG
{
    [CreateAssetMenu(fileName = "InventoryInputData", menuName = "FirstPersonController/Data/InventoryInputData", order = 1)]
    public class InteractionInputData : ScriptableObject
    {
        private bool m_interactedClicked;
        private bool m_interactedRelease;

        public bool InteractedClicked
        {
            get => m_interactedClicked;
            set => m_interactedClicked = value;
        }

        public bool InteractedReleased
        {
            get => m_interactedRelease;
            set => m_interactedRelease = value;
        }

        public void ResetInput()
        {
            m_interactedClicked = false;
            m_interactedRelease = false;
        }
    }
}
