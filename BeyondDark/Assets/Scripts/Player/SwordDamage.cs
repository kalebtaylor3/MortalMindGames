using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordDamage : MonoBehaviour
{
    public float currentAmountOfDamage = 0;

    public Camera playerCam;

    private static SwordDamage instance;

    bool sparkOnce = true;
    bool damageOnce = true;
    bool bloodOnce = false;

    public GameObject sparks;
    public GameObject blood;
    public GameObject vorgonBlood;

    public AudioClip wallDing;
    public AudioClip bloodHit;
    public AudioSource swordAudioSource;

    Animator swordAnimator;

    public static event Action<float> DealDamage;

    public static SwordDamage Instance
    {
        get
        {
            if (instance == null)
                UnityEngine.Debug.LogError("Null");
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void SetDamage(float value)
    {
        currentAmountOfDamage = value;

    }

    private void Update()
    {

        if (Gamepad.current.rightTrigger.wasPressedThisFrame)
            bloodOnce = false;

        if(currentAmountOfDamage > 0)
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1.8f))
            {
                if (hit.collider.gameObject.tag == "Wall" && sparkOnce && currentAmountOfDamage > 0)
                {
                    if (!swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sword004_Chainsaw"))
                    {
                        Debug.Log("Hitwall");
                        StartCoroutine(SparkDelay());
                        swordAudioSource.PlayOneShot(wallDing);
                        sparkOnce = false;
                        GameObject obj = Instantiate(sparks);
                        obj.transform.position = hit.point;
                        obj.transform.LookAt(playerCam.transform);
                        CameraShake.Instance.ShakeCamera(0.7f, 0.5f, 0.3f);
                    }
                }

                if (hit.collider.gameObject.tag == "Arm" && damageOnce && currentAmountOfDamage > 0)
                {
                    if (swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Sword004_Chainsaw"))
                    {
                        Debug.Log("Hitwall");
                        StartCoroutine(DamageDelay());
                        swordAudioSource.PlayOneShot(bloodHit);
                        damageOnce = false;
                        GameObject obj = Instantiate(vorgonBlood);
                        obj.transform.position = hit.point;
                        obj.transform.LookAt(playerCam.transform);
                        CameraShake.Instance.ShakeCamera(0.7f, 0.5f, 0.3f);
                        DealDamage?.Invoke(currentAmountOfDamage * 2);
                    }
                }
            }
        }
    }

    IEnumerator SparkDelay()
    {
        yield return new WaitForSeconds(0.7f);
        sparkOnce = true;
    }


    IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(0.5f);
        damageOnce = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Minion" && currentAmountOfDamage > 0)
            {
                other.GetComponent<MinionController>().ReceiveDamage(currentAmountOfDamage, true);
                other.GetComponent<MinionController>().canTakeSwordDamage = false;

                //StopCoroutine(WaitForDamageAgain());
                StartCoroutine(WaitForDamageAgain(other));
            }
        }
    }

    IEnumerator WaitForDamageAgain(Collider other)
    {
        yield return new WaitForSeconds(0.4f);

        if (other != null) 
        {            
            other.GetComponent<MinionController>().canTakeSwordDamage = true;
        }        
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    canDamage = true;
    //}
}
