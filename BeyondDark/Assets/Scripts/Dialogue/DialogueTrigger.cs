using System;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // an instance of the Dialogue class, containing the clipName and message
    public DialougeSystem dialogueSystem; // reference to the DialogueManager script

    public bool trap;
    public bool concelable;

    public static event Action OnTrap;
    public static event Action OnConcelable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogueSystem.PlayDialogue(dialogue); // call the PlayDialogue function in the DialogueManager script and pass in the Dialogue instance
            Destroy(this);

            if(trap)
            {
                OnTrap?.Invoke();
            }

            if(concelable)
            {
                OnConcelable?.Invoke();
            }

        }
}
}