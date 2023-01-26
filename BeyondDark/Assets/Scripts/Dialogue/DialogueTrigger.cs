using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue; // an instance of the Dialogue class, containing the clipName and message
    public DialougeSystem dialogueSystem; // reference to the DialogueManager script

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogueSystem.PlayDialogue(dialogue); // call the PlayDialogue function in the DialogueManager script and pass in the Dialogue instance
            Destroy(this);
        }
}
}