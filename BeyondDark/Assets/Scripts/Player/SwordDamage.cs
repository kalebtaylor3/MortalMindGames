using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordDamage : MonoBehaviour
{
    public float currentAmountOfDamage = 0;

    public Camera playerCam;

    private static SwordDamage instance;

    bool canDamage = true;
    bool sparkOnce = true;

    public GameObject sparks;

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
        if(currentAmountOfDamage > 0)
        {
            Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1.5f))
            {
                if (hit.collider.gameObject.tag == "Wall" && sparkOnce)
                {
                    Debug.Log("Hitwall");
                    StartCoroutine(SparkDelay());
                    sparkOnce = false;
                    GameObject obj = Instantiate(sparks);
                    obj.transform.position = hit.point;
                }
            }
        }
    }

    IEnumerator SparkDelay()
    {
        yield return new WaitForSeconds(0.2f);
        sparkOnce = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Minion" && canDamage)
            {
                other.GetComponent<MinionController>().ReceiveDamage(currentAmountOfDamage);
                canDamage = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canDamage = true;
    }
}
