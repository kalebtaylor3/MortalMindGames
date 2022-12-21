﻿using NaughtyAttributes;
using UnityEngine;

namespace MMG
{    
    public class CameraController : MonoBehaviour
    {
        #region Variables
        [Space,Header("Data")]
        [SerializeField] private CameraInputData camInputData = null;

        [Space,Header("Custom Classes")]
        [SerializeField] private CameraZoom cameraZoom = null;
        [SerializeField] private CameraSwaying cameraSway = null;
        [SerializeField] private LayerMask interactableLayer = ~0;
        [SerializeField] private Transform peakLeft;
        [SerializeField] private Transform peakRight;
        [SerializeField] private Transform peakIdle;

        [Space,Header("Look Settings")]
        [SerializeField] private Vector2 sensitivity = Vector2.zero;
        [SerializeField] private Vector2 smoothAmount = Vector2.zero;
        [SerializeField] [MinMaxSlider(-90f,90f)] private Vector2 lookAngleMinMax = Vector2.zero;

        private float camYaw;
        private float camPitch;

        private float desiredYaw;
        private float desiredPitch;
        
        private Transform cameraPivotTransform;
        private Camera playerCam;
            
        #endregion

        #region Functions
        void Awake()
        {
            GetComponents();
            InitComponents();
            ChangeCursorState();
        }

        void Update()
        {
            HandlePeaking();
        }

        void LateUpdate()
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
            HandleZoom();
        }

        void GetComponents()
        {
            cameraPivotTransform = transform.GetChild(0).transform;
            playerCam = GetComponentInChildren<Camera>();
        }

        void InitComponents()
        {
            cameraZoom.Init(playerCam, camInputData);
            cameraSway.Init(playerCam.transform);
        }

        void HandlePeaking()
        {
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            RaycastHit hitInfo;

            bool hitSomething = Physics.SphereCast(ray, 5f, out hitInfo, 0.3f, interactableLayer);

            if (!hitSomething)
            {
                if (camInputData.IsPeakingLeft)
                    PeakLeft();

                if (camInputData.IsPeakingRight)
                    PeakRight();

                if (!camInputData.IsPeakingLeft && !camInputData.IsPeakingRight)
                    PeakIdle();
            }
        }

        void PeakLeft()
        {
            Debug.Log("Peaking Left");
            transform.rotation = Quaternion.Lerp(transform.rotation, peakLeft.rotation, 0.15f);
        }

        void PeakRight()
        {
            Debug.Log("Peaking Right");
            transform.rotation = Quaternion.Lerp(transform.rotation, peakRight.rotation, 0.15f);
        }

        void PeakIdle()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, peakIdle.rotation, 0.15f);
        }

        void CalculateRotation()
            {
                desiredYaw += camInputData.InputVector.x * sensitivity.x * Time.deltaTime;
                desiredPitch -= camInputData.InputVector.y * sensitivity.y * Time.deltaTime;

                desiredPitch = Mathf.Clamp(desiredPitch,lookAngleMinMax.x,lookAngleMinMax.y);
            }

        void SmoothRotation()
        {
            camYaw = Mathf.Lerp(camYaw,desiredYaw, smoothAmount.x * Time.deltaTime);
            camPitch = Mathf.Lerp(camPitch,desiredPitch, smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation()
        {
            transform.eulerAngles = new Vector3(0f,camYaw,0f);
            cameraPivotTransform.localEulerAngles = new Vector3(camPitch,0f,0f);
        }

        public void HandleSway(Vector3 _inputVector,float _rawXInput)
        {
            cameraSway.SwayPlayer(_inputVector,_rawXInput);
        }

        void HandleZoom()
        {
            if (camInputData.ZoomClicked & PlayerInventoryController.Instance.inventoryOpen)
                return;

            if (camInputData.ZoomReleased & PlayerInventoryController.Instance.inventoryOpen)
                return;

            if (camInputData.ZoomClicked || camInputData.ZoomReleased)
                cameraZoom.ChangeFOV(this);

        }

        public void ChangeRunFOV(bool _returning)
        {
            cameraZoom.ChangeRunFOV(_returning,this);
        }

        void ChangeCursorState()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion
    }
}
