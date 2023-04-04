using Cinemachine;
using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class LoreInteractable : InteractableBase
{
    public LoreInputData LoreInputData;
    public static Action<bool> OnCollect;
    public static Action Collect;
    public static Action<bool> OnPutDown;
    public CinemachineVirtualCamera loreCam;
    public GameObject ui;
    public GameObject loreUI;
    public InteractionController controller;

    public Transform inspectLocation;
    Vector3 startPos;
    Quaternion targetRotation;
    Quaternion startRot;

    public GameObject volume;
    public GameObject cam;
    public GameObject loreLight;

    bool interacting = false;

    public float rotateSpeed = 1;

    public AudioSource lookAtSource;
    public AudioClip lookAtClip;

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        loreLight.SetActive(false);
        volume.SetActive(false);
        cam.SetActive(false);
        loreUI.SetActive(false);
    }

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        loreLight.SetActive(false);
        volume.SetActive(false);
        cam.SetActive(false);
        loreUI.SetActive(false);
    }

    private void OnEnable()
    {
        WorldData.OnDeath += PutBookAway;
    }


    public override void OnInteract()
    {
        LoreInputData.PutAway = false;
        base.OnInteract();

        transform.position = inspectLocation.position;
        transform.rotation = inspectLocation.rotation;
        targetRotation = loreCam.transform.rotation;
        OnCollect?.Invoke(true);
        interacting = true;
        loreCam.enabled = true;
        volume.SetActive(true);
        StartCoroutine(WaitForLoreCam());
        controller.enabled = false;
        ui.SetActive(false);
        loreUI.SetActive(true);
        Collect?.Invoke();
        lookAtSource.PlayOneShot(lookAtClip);
    }

    private void Update()
    {
        if (interacting)
        {

            //Quaternion rotation = inspectLocation.rotation;
            loreCam.transform.rotation = targetRotation;

            if (LoreInputData.RotateLeft)
                transform.Rotate(new Vector3(0, 0, 5), -rotateSpeed * Time.deltaTime);

            if (LoreInputData.RotateRight)
                transform.Rotate(new Vector3(0,0, 5), rotateSpeed * Time.deltaTime);

            if (LoreInputData.PutAway == true)
            {
                StopAllCoroutines();
                OnPutDown?.Invoke(false);
                transform.position = startPos;
                transform.rotation = startRot;
                interacting = false;
                loreCam.enabled = false;
                volume.SetActive(false);
                cam.SetActive(false);
                loreLight.SetActive(false);
                controller.enabled = true;
                ui.SetActive(true);
                loreUI.SetActive(false);
                lookAtSource.PlayOneShot(lookAtClip);
            }

            //transform.rotation = rotation;
        }
    }

    IEnumerator WaitForLoreCam()
    {
        yield return new WaitForSeconds(2);
        cam.SetActive(true);
        loreLight.SetActive(true);
    }

    void PutBookAway()
    {
        LoreInputData.PutAway = true;
    }    
}
