using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public class ItemInteractable : InteractableBase
    {
        [SerializeField] bool destroyOnPickUp = true;
        public Dialogue dialogue;
        public TutorialTrigger tutorial;
        public GameObject compass;

        private void Start()
        {
            if(compass != null)
                compass.SetActive(false);
        }

        public override void OnInteract()
        {
            if(dialogue != null)
                DialougeSystem.instance.PlayDialogue(dialogue);

            if (tutorial != null)
                TutorialController.instance.SetTutorial(tutorial.imageTut, tutorial.vidTut, 2);

            if (compass != null)
                compass.SetActive(false);


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
