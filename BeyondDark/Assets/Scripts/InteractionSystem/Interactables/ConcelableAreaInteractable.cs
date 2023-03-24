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
        [SerializeField] public HiddingCameraController concelableAreaCam;
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        [SerializeField] private GameObject player;
        public PlayerController playerController;
        [SerializeField] public AudioSource doorCreak;
        [HideInInspector] public float exposurePercentage;
        private bool isHidding = false;
        public Transform cameraPosition;
        public Transform lookAtTransform;
        [HideInInspector] public Vector3 lookAtStartPosition;
        Quaternion startRotation;

        public GameObject deathCAA;

        public enum clamp { X, Y, Z };
        public clamp cameraClamp;
        [SerializeField] public float maxLocalRotationValue;
        public bool negativeRotation;

        [SerializeField] public Animator enteranceAnimator;
        [SerializeField] public GameObject rotator;
        public float rotationSpeed = 25;

        [SerializeField] InputController input;

        bool happenOnce = false;
        bool canInteract = false;
        bool canRotate = true;
        [HideInInspector] public bool canCreak = false;
        bool canExit = false;

        public Transform searchPos;

        Vector3 startcamPosition;

        public static event Action OnEnteredSpot;
        public static event Action OnLeaveSpot;


        public float maxRight;
        public float maxLeft;

        private void Start()
        {
            startcamPosition = concelableAreaCam.transform.position;
            lookAtStartPosition = lookAtTransform.position;

            concelableAreaCam.cam.LookAt = lookAtTransform;
            startRotation = rotator.transform.rotation;

            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

            if (cameraClamp == clamp.Z)
                concelableAreaCam.cam.Follow = lookAtTransform;

            doorCreak.enabled = false;
        }

        private void Awake()
        {
            startcamPosition = concelableAreaCam.transform.position;
            lookAtStartPosition = lookAtTransform.position;

            concelableAreaCam.cam.LookAt = lookAtTransform;
            startRotation = rotator.transform.rotation;

            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

            if (cameraClamp == clamp.Z)
                concelableAreaCam.cam.Follow = lookAtTransform;

            doorCreak.enabled = false;
        }

        private void Update()
        {
            //set reveal % to what is returned from hiding spot camera 

            if (canCreak)
                exposurePercentage = concelableAreaCam.GetRevealPercentage();
            else
                exposurePercentage = 0;

            if (canCreak)
            {
                //lookAtTransform.position += new Vector3(concelableAreaCam.leftRightPercentage, 0, 0) * 0.15f * Time.deltaTime;
                concelableAreaCam.comp.m_Pan += concelableAreaCam.leftRightPercentage;

                if (concelableAreaCam.comp.m_Pan > maxRight)
                    concelableAreaCam.comp.m_Pan = maxRight;

                if (concelableAreaCam.comp.m_Pan < maxLeft)
                    concelableAreaCam.comp.m_Pan = maxLeft;

            }

            if (isHidding)
                displayText = "Press X to Exit";
            else
                displayText = "Press X to Hide!";

            if (exposurePercentage > 0)
            {
                doorCreak.volume = Mathf.Clamp(doorCreak.volume, 0.1f, 0.5f);
                doorCreak.volume = exposurePercentage;
            }
            else
                doorCreak.volume = 0.5f;


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
            else if(cameraClamp == clamp.Y)
            {
                if (rotator.transform.localRotation.x > maxLocalRotationValue)
                {
                    canRotate = false;
                    doorCreak.Stop();
                }
                else
                    canRotate = true;
            }
            else
            {
                if (rotator.transform.localRotation.z > maxLocalRotationValue)
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
            else if(cameraClamp == clamp.Z && exposurePercentage > 0)
            {
                if (canRotate)
                {
                    if (!doorCreak.isPlaying)
                        doorCreak.Play();
                    rotator.transform.Rotate(new Vector3(0, 0, exposurePercentage) * rotationSpeed * Time.deltaTime);
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
                    else if (cameraClamp == clamp.Z)
                    {
                        if (rotator.transform.rotation.z < startRotation.z)
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
                    OnEnteredSpot?.Invoke();
                    input.canMove = false;
                    if(ConcelableDetection.Instance != null)
                        ConcelableDetection.Instance.SetConcelableArea(this);
                    //playerCameraHolder.enabled = false;
                    exposurePercentage = 0;
                    enteranceAnimator.SetTrigger("Enter");

                    // Turns off kill collision
                    playerController.ConcealEnterKillCollision(false);

                    StartCoroutine(WaitForEnterAnimation());
                    isHidding = true;
                    canExit = false;

                    WorldData.Instance.lastConceal = this;

                    this.GetComponent<BoxCollider>().enabled = false;
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
            yield return new WaitForSeconds(1.5f);
            //enable the camera controls n stuff
            enteranceAnimator.SetTrigger("Inside");
            StartCoroutine(WaitForInside());
            happenOnce = false;
        }

        IEnumerator WaitForInside()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length + 1);
            //enable the camera controls n stuff
            doorCreak.enabled = true;
            //isHidding = true;
            happenOnce=false;
            canCreak = true;
            concelableAreaCam.cam.LookAt = lookAtTransform;
            enteranceAnimator.enabled = false;
            //Move Player
            //player.transform.position = transform.position;
            //playerCamera.LookAt = lookAtTransform;

            canExit = true;
            this.GetComponent<BoxCollider>().enabled = true;
            

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

        }

        void ResetRotator()
        {
            if (cameraClamp == clamp.Y)
                rotator.transform.Rotate(new Vector3(-0.95f, 0), rotationSpeed * Time.deltaTime);
            else if (cameraClamp == clamp.X)
                rotator.transform.Rotate(new Vector3(0, -0.95f), rotationSpeed * Time.deltaTime);
            else
                rotator.transform.Rotate(new Vector3(0, 0, -0.95f), rotationSpeed * Time.deltaTime);
        }

        public void ExitArea()
        {
            enteranceAnimator.enabled = true;
            //Debug.Log("Exiting Area");
            StartCoroutine(WaitForExit());

            // Turns on kill collision
            playerController.ConcealEnterKillCollision(true);

            enteranceAnimator.SetTrigger("Enter");
            StartCoroutine(WaitForExitClose());
            canCreak = false;
        }

        public void ToggleConcealDeath()
        {
            StartCoroutine(ToggleDeath());
        }

        public void ToggleCamChange()
        {
            playerCamera.gameObject.SetActive(true);
            concelableAreaCam.gameObject.SetActive(false);
            rotator.transform.rotation = startRotation;
        }

        IEnumerator ToggleDeath()
        {
            deathCAA.SetActive(true);
            yield return new WaitForSeconds(1f);


            //playerCamera.gameObject.SetActive(true);
            //concelableAreaCam.gameObject.SetActive(false);

            //WorldData.Instance.fadeOut.SetActive(true);
            canCreak = false;
            isHidding = false;
            happenOnce = false;
            input.canMove = true;
            OnLeaveSpot?.Invoke();
            deathCAA.SetActive(false);
            //enteranceAnimator.SetTrigger("Inside");
            //yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length + 1);

            Rest();

            // Turns on kill collision
            playerController.ConcealEnterKillCollision(true);

        }

        IEnumerator WaitForExit()
        {
            yield return new WaitForSeconds(0.9f);
            //Move Player
            //player.transform.position = searchPos.position;
            //playerCamera.LookAt = lookAtTransform;

            playerCamera.gameObject.SetActive(true);
            concelableAreaCam.gameObject.SetActive(false);
            input.canMove = true;
            happenOnce = false;
            happenOnce = false;
            isHidding = false;
            OnLeaveSpot?.Invoke();
            isHidding = false;
        }

        IEnumerator WaitForExitClose()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length + 1);
            enteranceAnimator.SetTrigger("Inside");

        }

        public void Rest()
        {
            startcamPosition = concelableAreaCam.transform.position;
            lookAtStartPosition = lookAtTransform.position;

            concelableAreaCam.cam.LookAt = lookAtTransform;
            startRotation = rotator.transform.rotation;

            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();

            if (cameraClamp == clamp.Y)
                concelableAreaCam.cam.Follow = lookAtTransform;

            if (cameraClamp == clamp.Z)
                concelableAreaCam.cam.Follow = lookAtTransform;

            concelableAreaCam.revealPercentage = 0;

            rotator.transform.rotation = new Quaternion(0, 0, 0, 0);

            ResetRotator();

            input.canMove = true;
            happenOnce = false;
            happenOnce = false;
            isHidding = false;
            OnLeaveSpot?.Invoke();
            isHidding = false;
            canInteract = true;
            canExit = false;
            canRotate = true;
            exposurePercentage = 0;
            enteranceAnimator.enabled = true;
        }




        //setup function to handle being caught. if vorgon is in area and reveal % is higher than a certain amount then caught
    }
}
