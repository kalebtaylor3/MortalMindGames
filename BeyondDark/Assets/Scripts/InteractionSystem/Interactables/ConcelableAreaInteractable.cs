using Cinemachine;
using MMG;
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
        public enum clamp { X, Y };
        public clamp cameraClamp;

        [SerializeField] Animator enteranceAnimator;
        [SerializeField] GameObject rotator;
        public float rotationSpeed = 25;

        [SerializeField] InputController input;

        private void Update()
        {
            //set reveal % to what is returned from hiding spot camera 

            exposurePercentage = concelableAreaCam.GetRevealPercentage();

            if (isHidding)
                displayText = "Press X to Exit";

            if (cameraClamp == clamp.X && exposurePercentage > 0)
                rotator.transform.Rotate(new Vector3(0, exposurePercentage, 0) * rotationSpeed * Time.deltaTime);
            else if(cameraClamp == clamp.Y && exposurePercentage > 0) 
                rotator.transform.Rotate(new Vector3(exposurePercentage, 0, 0) * rotationSpeed * Time.deltaTime);

            if (exposurePercentage == 0)
            {
                rotator.transform.rotation = new Quaternion(0, 0, 0, 0);
                //ResetRotator();
            }


        }


        public override void OnInteract()
        {
            base.OnInteract();
            exposurePercentage = 0;
            isHidding = true;
            enteranceAnimator.SetTrigger("Enter");
            StartCoroutine(WaitForEnterAnimation());
            input.canMove = false;
        }

        IEnumerator WaitForEnterAnimation()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length);
            playerCamera.gameObject.SetActive(false);
            StartCoroutine(WaitForCloseAnimation());
        }

        IEnumerator WaitForCloseAnimation()
        {
            yield return new WaitForSeconds(2f);
            //enable the camera controls n stuff
            enteranceAnimator.SetTrigger("Inside");
            StartCoroutine(WaitForInside());
        }

        IEnumerator WaitForInside()
        {
            yield return new WaitForSeconds(enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length);
            //enable the camera controls n stuff
            enteranceAnimator.enabled = false;
        }

        void ResetRotator()
        {
           // rotator.transform.rotation = Quaternion.Lerp(rotator.transform.rotation, new Quaternion(0, 0, 0, 0), 100 * Time.deltaTime);
        }


        //setup function to handle being caught. if vorgon is in area and reveal % is higher than a certain amount then caught
    }
}
