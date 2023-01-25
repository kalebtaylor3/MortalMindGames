using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public class ItemInteractable : InteractableBase
    {
        [SerializeField] bool destroyOnPickUp = true;
        public Dialogue dialogue;

        public override void OnInteract()
        {
            if(dialogue != null)
                DialougeSystem.instance.PlayDialogue(dialogue);
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
