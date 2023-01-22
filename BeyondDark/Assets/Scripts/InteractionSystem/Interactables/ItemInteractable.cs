using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public class ItemInteractable : InteractableBase
    {
        public override void OnInteract()
        {
            base.OnInteract();

            Destroy(gameObject);
        }
    }
}
