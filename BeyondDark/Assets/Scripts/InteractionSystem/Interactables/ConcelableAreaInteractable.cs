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
        private float exposurePercentage;
        private bool isHidding = false;
        public Transform cameraPosition;
        public Transform lookAtTransform;
        Vector3 lookAtStartPosition;

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

        Vector3 startcamPosition;

        public static event Action OnEnteredSpot;

        private void Start()
        {
            startcamPosition = concelableAreaCam.transform.position;
            lookAtStartPosition = lookAtTransform.position;

            concelableAreaCam.cam.LookAt = lookAtTransform;

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

            if (cameraClamp == clamp.X)
            {
                if (rotator.transform.localRotation.y > maxLocalRotationValue)
                    canRotate = false;
                else
                    canRotate = true;
            }
            else
            {
                if (rotator.transform.localRotation.x > maxLocalRotationValue)
                    canRotate = false;
                else
                    canRotate = true;
            }

            Debug.Log(rotator.transform.localRotation);

            if (cameraClamp == clamp.X && exposurePercentage > 0)
            {
                //Mathf.Clamp(rotator.transform.rotation.y, 0, 40);
                if (canRotate)
                {
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
                    rotator.transform.Rotate(new Vector3(exposurePercentage, 0, 0) * rotationSpeed * Time.deltaTime);
                    lookAtTransform.position += new Vector3(exposurePercentage * -1 * 0.5f, exposurePercentage, 0) * 0.15f * Time.deltaTime;
                }
            }

            if (exposurePercentage == 0)
            {
                rotator.transform.rotation = new Quaternion(0, 0, 0, 0);
                lookAtTransform.position = lookAtStartPosition;
                //ResetRotator();
            }

            if (isHidding)
            {
                if (concelableAreaCam.transform.position.x < -0.3f)
                {
                    //concelableAreaCam.transform.position = new Vector3(-0.3f, concelableAreaCam.transform.position.y, concelableAreaCam.transform.position.z);
                    concelableAreaCam.cam.ForceCameraPosition(new Vector3(-0.3f, concelableAreaCam.transform.position.y, concelableAreaCam.transform.position.z), cameraPosition.rotation);
                }
            }

            concelableAreaCam.cam.ForceCameraPosition(cameraPosition.position, cameraPosition.rotation);

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
            if(!happenOnce)
            {
                if (!isHidding && canInteract)
                {
                    base.OnInteract();
                    input.canMove = false;
                    exposurePercentage = 0;
                    enteranceAnimator.SetTrigger("Enter");
                    StartCoroutine(WaitForEnterAnimation());
                    isHidding = true;
                    OnEnteredSpot?.Invoke();
                }
                else
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
            concelableAreaCam.cam.LookAt = lookAtTransform;

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

        }

        void ResetRotator()
        {
           rotator.transform.rotation = Quaternion.Lerp(rotator.transform.rotation, new Quaternion(0, 0, 0, 0), 100 * Time.deltaTime);
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
        }

        IEnumerator WaitForExit()
        {
            yield return new WaitForSeconds(1.5f);
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
