using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialougeSystem : MonoBehaviour
{
    public static DialougeSystem instance;
    public AudioClip[] dialogueClips; // array to hold all of your recorded dialogue clips
    [HideInInspector] public AudioClip currentClip; // variable to hold the current dialogue clip being played
    public TMP_Text dialogueText; // reference to the UI Text element used to display the dialogue text
    public Image dialogueBox; // reference to the UI Image element used as a background for the dialogue text
    [HideInInspector] public bool isPlaying = false;
    public float fadeSpeed; // speed at which the dialogue box fades in and out
    public float textFadeSpeed;
    [HideInInspector] public Dialogue nextDialogue; // reference to the next dialogue
    [HideInInspector] public Queue<Dialogue> dialogueQueue = new Queue<Dialogue>();

    // function to play the corresponding dialogue clip based on the Dialogue instance passed in

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void PlayDialogue(Dialogue dialogue)
    {
        if (isPlaying)
        {
            dialogueQueue.Enqueue(dialogue);
            return;
        }
        StopAllCoroutines();
        currentClip = GetClip(dialogue.clip);
        GetComponent<AudioSource>().PlayOneShot(currentClip);
        dialogueText.text = dialogue.dialougeText;
        StartCoroutine(FadeTextIn());
        StartCoroutine(FadeIn());
    }

    public AudioClip GetClip(AudioClip clip)
    {
        for (int i = 0; i < dialogueClips.Length; i++)
        {
            if (dialogueClips[i] == clip)
            {
                return dialogueClips[i];
            }
        }
        return null;
    }


    IEnumerator FadeTextIn()
    {
        dialogueText.gameObject.SetActive(true);
        Color textColor = dialogueText.color;
        textColor.a = 0;
        dialogueText.color = textColor;
        while (textColor.a < 1)
        {
            textColor.a += textFadeSpeed * Time.deltaTime;
            dialogueText.color = textColor;
            yield return null;
        }
    }

    IEnumerator FadeTextOut()
    {
        Color textColor = dialogueText.color;
        while (textColor.a > 0)
        {
            textColor.a -= textFadeSpeed * Time.deltaTime;
            dialogueText.color = textColor;
            yield return null;
        }
        dialogueText.gameObject.SetActive(false);
    }

    IEnumerator FadeIn()
    {
        isPlaying = true;
        dialogueBox.gameObject.SetActive(true);
        while (dialogueBox.color.a < 1)
        {
            dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, dialogueBox.color.a + (fadeSpeed * Time.deltaTime));
            yield return null;
        }
        yield return new WaitForSeconds(currentClip.length);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        StartCoroutine(FadeTextOut());
        while (dialogueBox.color.a > 0)
        {
            dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, dialogueBox.color.a - (fadeSpeed * Time.deltaTime));
            yield return null;
        }
        isPlaying = false;
        dialogueBox.gameObject.SetActive(false);
        if (dialogueQueue.Count > 0)
        {
            PlayDialogue(dialogueQueue.Dequeue()); // play the next dialogue in the queue
        }
    }
}
