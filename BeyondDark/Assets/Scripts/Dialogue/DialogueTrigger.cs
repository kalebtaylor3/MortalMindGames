using System;
using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // an instance of the Dialogue class, containing the clipName and message
    private DialougeSystem dialogueSystem; // reference to the DialogueManager script

    public bool oilTrap;
    public bool barrelConcelable;
    public bool springTrap;
    public bool chestConcelable;

    public static event Action OnOIlTrap;
    public static event Action OnBarrelConcelable;

    public static event Action OnSpringTrap;
    public static event Action OnChestConcelable;

    private void OnTriggerEnter(Collider other)
    {
        dialogueSystem = DialougeSystem.instance;

        if (other.tag == "Player")
        {
            dialogueSystem.PlayDialogue(dialogue); // call the PlayDialogue function in the DialogueManager script and pass in the Dialogue instance
            Destroy(this);

            if(oilTrap)
            {
                OnOIlTrap?.Invoke();
            }

            if(barrelConcelable)
            {
                OnBarrelConcelable?.Invoke();
            }

            if(springTrap)
            {
                OnSpringTrap?.Invoke();
            }

            if(chestConcelable)
            {
                OnChestConcelable?.Invoke();
            }

        }
        else if(other.tag == "VorgonRealmPlayer")
        {
            StartCoroutine(PlayVoiceLinewithDelay());
        }
    }

    IEnumerator PlayVoiceLinewithDelay()
    {
        // Wait until after the out of breath spawn audio
        yield return new WaitForSeconds(1.5f);
        dialogueSystem.PlayDialogue(dialogue); // call the PlayDialogue function in the DialogueManager script and pass in the Dialogue instance
        Destroy(this);
    }

}