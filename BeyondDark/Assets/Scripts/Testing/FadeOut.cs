using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public Animator animator;

    private void OnEnable()
    {
        animator.SetTrigger("FadeOut");
        animator.SetBool("Faded", true);

        animator.Play("Fadeout");
    }
}
