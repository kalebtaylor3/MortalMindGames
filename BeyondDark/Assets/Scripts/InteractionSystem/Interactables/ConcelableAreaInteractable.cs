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
        private Quaternion clampRotation;

        [SerializeField] InputController input;

        private void Update()
        {
            //set reveal % to what is returned from hiding spot camera 

            exposurePercentage = concelableAreaCam.GetRevealPercentage();

            if (isHidding)
                displayText = "Press X to Exit";

            rotator.transform.Rotate(new Vector3(0, exposurePercentage, 0) * 5.0f * Time.deltaTime);


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
        }


        //setup function to handle being caught. if vorgon is in area and reveal % is higher than a certain amount then caught
    }
}
