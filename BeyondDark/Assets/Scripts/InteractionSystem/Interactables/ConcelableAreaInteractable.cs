using Cinemachine;
using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace MMG
{
    public class ConcelableAreaInteractable : InteractableBase
    {
        [SerializeField] private HiddingCameraController concelableAreaCam;
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private AudioSource doorCreak;
        private float exposurePercentage;
        private bool isHidding = false;
        public Transform cameraPosition;
        public Transform lookAtTransform;
        Vector3 lookAtStartPosition;
        Quaternion startRotation;

        public enum clamp { X, Y };
        public clamp cameraClamp;
        [SerializeField] private float maxLocalRotationValue;
        public bool negativeRotation;

        [SerializeField] Animator enteranceAnimator;
        [SerializeField] GameObject rotator;
        public float rotationSpeed = 25;

        [SerializeField] InputController input;

        bool happenOnce = false;
        bool canInteract = false;
        bool canRotate = true;
        bool canCreak = false;
        bool canExit = false;

        Vector3 startcamPosition;

        public static event Action OnEnteredSpot;
        public static event Action OnLeaveSpot;

        private void Start()
        {
            startcamPosition = concelableAreaCam.transform.position;
            lookAtStartPosition = lookAtTransform.position;

            concelableAreaCam.cam.LookAt = lookAtTransform;
            startRotation = rotator.transform.rotation;

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;
        }

        private void Update()
        {
            //set reveal % to what is returned from hiding spot camera 

            exposurePercentage = concelableAreaCam.GetRevealPercentage();

            if (isHidding)
                displayText = "Press X to Exit";
            else
                displayText = "Press X to Hide!";

            if (!canCreak)
                doorCreak.Stop();

            if (cameraClamp == clamp.X)
            {
                if (rotator.transform.localRotation.y > maxLocalRotationValue)
                {
                    doorCreak.Stop();
                    canRotate = false;
                }
                else
                    canRotate = true;
            }
            else
            {
                if (rotator.transform.localRotation.x > maxLocalRotationValue)
                {
                    canRotate = false;
                    doorCreak.Stop();
                }
                else
                    canRotate = true;
            }

            if (cameraClamp == clamp.X && exposurePercentage > 0)
            {
                //Mathf.Clamp(rotator.transform.rotation.y, 0, 40);
                if (canRotate)
                {
                    if (!doorCreak.isPlaying)
                        doorCreak.Play();

                    rotator.transform.Rotate(new Vector3(0, exposurePercentage, 0) * rotationSpeed * Time.deltaTime);
                    if(negativeRotation)
                        lookAtTransform.position += new Vector3(exposurePercentage * -1, 0, 0) * 0.15f * Time.deltaTime;
                    else
                        lookAtTransform.position += new Vector3(exposurePercentage, 0, 0) * 0.15f * Time.deltaTime;
                }
            }
            else if (cameraClamp == clamp.Y && exposurePercentage > 0)
            {
                if (canRotate)
                {
                    if(!doorCreak.isPlaying)
                        doorCreak.Play();
                    rotator.transform.Rotate(new Vector3(exposurePercentage, 0, 0) * rotationSpeed * Time.deltaTime);
                    lookAtTransform.localPosition += new Vector3(0, exposurePercentage, exposurePercentage * -1 * 0.5f) * 0.15f * Time.deltaTime;
                }
            }

            if (isHidding)
            {
                if (exposurePercentage == 0)
                {
                    //rotator.transform.rotation = new Quaternion(0, 0, 0, 0);
                    //lookAtTransform.position = lookAtStartPosition;
                    lookAtTransform.position = Vector3.Lerp(lookAtTransform.position, lookAtStartPosition, 0.95f * Time.deltaTime);
                    if (!doorCreak.isPlaying)
                        doorCreak.Play();
                    ResetRotator();

                    if (cameraClamp == clamp.Y)
                    {
                        if (rotator.transform.rotation.x > 0)
                        {
                            rotator.transform.rotation = new Quaternion(0, 0, 0, 0);
                            doorCreak.Stop();
                        }
                    }
                    else if (cameraClamp == clamp.X)
                    {
                        if (rotator.transform.rotation.y > startRotation.y)
                        {
                            rotator.transform.rotation = new Quaternion(0, 0, 0, 0);
                            doorCreak.Stop();
                        }
                    }

                }
            }

            if (isHidding)
            {
                if (concelableAreaCam.transform.position.x < -0.3f)
                {
                    //concelableAreaCam.transform.position = new Vector3(-0.3f, concelableAreaCam.transform.position.y, concelableAreaCam.transform.position.z);
                    concelableAreaCam.cam.ForceCameraPosition(new Vector3(-0.3f, concelableAreaCam.transform.position.y, concelableAreaCam.transform.position.z), cameraPosition.rotation);
                }
            }

            concelableAreaCam.cam.ForceCameraPosition(cameraPosition.position, lookAtTransform.rotation);

        }

        private void OnTriggerEnter(Collider other)
        {
            canInteract = true;
        }

        private void OnTriggerExit(Collider other)
        {
            canInteract = false;
        }

        public override void OnInteract()
        {
            if (!happenOnce)
            {
                if (!isHidding)
                {
                    base.OnInteract();
                    input.canMove = false;
                    //playerCameraHolder.enabled = false;
                    exposurePercentage = 0;
                    enteranceAnimator.SetTrigger("Enter");
                    StartCoroutine(WaitForEnterAnimation());
                    isHidding = true;
                    canExit = false;
                    this.GetComponent<BoxCollider>().enabled = false;
                    OnEnteredSpot?.Invoke();
                }
                else if(canExit)
                {
                    ExitArea();
                }
                happenOnce = true;
            }
        }
        IEnumerator WaitForEnterAnimation()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length);
            playerCamera.gameObject.SetActive(false);
            concelableAreaCam.gameObject.SetActive(true);
            StartCoroutine(WaitForCloseAnimation());
        }

        IEnumerator WaitForCloseAnimation()
        {
            yield return new WaitForSeconds(2f);
            //enable the camera controls n stuff
            enteranceAnimator.SetTrigger("Inside");
            StartCoroutine(WaitForInside());
            happenOnce = false;
        }

        IEnumerator WaitForInside()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length);
            //enable the camera controls n stuff
            enteranceAnimator.enabled = false;
            isHidding = true;
            happenOnce=false;
            canCreak = true;
            concelableAreaCam.cam.LookAt = lookAtTransform;
            canExit = true;
            this.GetComponent<BoxCollider>().enabled = true;

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

        }

        void ResetRotator()
        {
            if (cameraClamp == clamp.Y)
                rotator.transform.Rotate(new Vector3(-0.95f, 0), rotationSpeed * Time.deltaTime);
            else
                rotator.transform.Rotate(new Vector3(0, -0.95f), rotationSpeed * Time.deltaTime);
        }

        void ExitArea()
        {
            isHidding = false;
            Debug.Log("Exiting Area");
            playerCamera.gameObject.SetActive(true);
            concelableAreaCam.gameObject.SetActive(false);
            input.canMove = true;
            happenOnce = false;
            isHidding = false;
            enteranceAnimator.enabled = true;
            StartCoroutine(WaitForExit());
            enteranceAnimator.SetTrigger("Enter");
            StartCoroutine(WaitForExitClose());
            canCreak = false;
            OnLeaveSpot?.Invoke();
        }

        IEnumerator WaitForExit()
        {
            yield return new WaitForSeconds(1.8f);
            happenOnce = false;

        }

        IEnumerator WaitForExitClose()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length + 1);
            enteranceAnimator.SetTrigger("Inside");
        }


        //setup function to handle being caught. if vorgon is in area and reveal % is higher than a certain amount then caught
    }
}
