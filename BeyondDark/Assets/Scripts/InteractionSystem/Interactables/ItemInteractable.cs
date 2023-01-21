using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public class ItemInteractable : InteractableBase
    {
        [SerializeField] bool destroyOnPickUp = true;

        public override void OnInteract()
        {
            base.OnInteract();

            if(destroyOnPickUp)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
            
        }
    }
}
