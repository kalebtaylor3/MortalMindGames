using Cinemachine;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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

        public CinemachineRecomposer angle;
        public CinemachineCameraOffset offsett;

        [Space,Header("Look Settings")]
        [SerializeField] public Vector2 sensitivity = Vector2.zero;
        [SerializeField] private Vector2 smoothAmount = Vector2.zero;
        [SerializeField] [MinMaxSlider(-90f,90f)] private Vector2 lookAngleMinMax = Vector2.zero;

        private float camYaw;
        private float camPitch;

        private float desiredYaw;
        private float desiredPitch;
        
        private Transform cameraPivotTransform;
        public CinemachineVirtualCamera playerCam;

        public  MovementInputData input;
        public AudioSource peakAudio;
        public AudioSource peakResetAudio;
        bool playOnce = false;
        private bool canPeakRight;
        private bool canPeakLeft;


        public LayerMask layerMask; // The layer to check for collision
        public float raycastDistance = 1.0f; // The distance to cast the raycast
        bool canLook = true;

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

            canPeakLeft = CheckPeakLeft();
            canPeakRight = CheckPeakRight();
        }

        void LateUpdate()
        {
            CalculateRotation();
            SmoothRotation();

            if (canLook)
            {
                ApplyRotation();
                HandleZoom();
            }
        }

        public void CanLook(bool value)
        {
            canLook = value;
        }

        private void OnEnable()
        {
            peakAudio.enabled = false;
            ResetFOV();
            ConcelableAreaInteractable.OnEnteredSpot += ResetFOV;
        }

        private void OnDisable()
        {
            ConcelableAreaInteractable.OnEnteredSpot -= ResetFOV;
        }

        bool CheckPeakLeft()
        {

            // Cast a ray to the left of the object
            RaycastHit hitLeft;
            bool hitLeftSomething = Physics.Raycast(transform.position, -transform.right, out hitLeft, raycastDistance, layerMask);

            // Check if either ray hit something on the layerMask
            if (hitLeftSomething)
            {
                return false;
            }

            return true;
        }

        bool CheckPeakRight()
        {
            // Cast a ray to the right of the object
            RaycastHit hitRight;
            bool hitRightSomething = Physics.Raycast(transform.position, transform.right, out hitRight, raycastDistance, layerMask);

            // Check if either ray hit something on the layerMask
            if (hitRightSomething)
            {
                return false;
            }

            return true;
        }

        public void ResetCam()
        {
            WorldData.Instance.lastConceal.concelableAreaCam.enabled = false;
        }

        public void ResetFOV()
        {
            if(this.gameObject.activeInHierarchy)
            {
                cameraZoom.ChangeRunFOV(true, this);
            }
            //playerCam.m_Lens.FieldOfView = 60;
            //playerCam.m_Lens.FieldOfView = Mathf.Lerp(playerCam.m_Lens.FieldOfView, 60, 0.25f * Time.deltaTime);
            
            camInputData.IsZooming = false;
            camInputData.ZoomReleased = true;
            camInputData.ZoomClicked = false;
            camInputData.IsPeakingLeft = false;
            camInputData.IsPeakingRight = false;

            PeakIdle();
            
            input.RunReleased = true;
            input.RunClicked = false;
            input.IsRunning = false;
        }

        void GetComponents()
        {
            cameraPivotTransform = transform.GetChild(0).transform;
            playerCam = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        void InitComponents()
        {
            cameraZoom.Init(playerCam, camInputData);
            cameraSway.Init(playerCam.transform);

            desiredYaw = transform.eulerAngles.y;
            desiredPitch = transform.eulerAngles.x;
        }

        void HandlePeaking()
        {
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            RaycastHit hitInfo;

            bool hitSomething = Physics.SphereCast(ray, 5f, out hitInfo, 0.3f, interactableLayer);

            //if (!hitSomething)
            //{
                if (camInputData.IsPeakingLeft && canPeakLeft)
                    PeakLeft();
                else if (camInputData.IsPeakingRight && canPeakRight)
                    PeakRight();
                else
                    PeakIdle();

            //}
        }

        void PeakLeft()
        {
            //Debug.Log("Peaking Left");
            //transform.rotation = Quaternion.Lerp(transform.rotation, peakLeft.rotation, 0.65f);
            //this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, peakLeft.position, 0.55f);
            peakAudio.enabled = true;
            angle.m_Dutch = Mathf.Lerp(angle.m_Dutch, 25, 3 * Time.deltaTime);
            offsett.m_Offset.x = Mathf.Lerp(offsett.m_Offset.x, -0.60f, 3 * Time.deltaTime);
            playOnce = false;
        }

        void PeakRight()
        {
            //Debug.Log("Peaking Right");
            //this.gameObject.transform.position = Vector3.Lerp(this.gameObject.transform.position, peakRight.position, 0.55f);
            //transform.rotation = Quaternion.Lerp(transform.rotation, peakRight.rotation, 0.65f);
            //playerCam.ForceCameraPosition(peakRight.position, peakRight.rotation);
            //angle.m_Dutch += Mathf.Lerp(angle.m_Dutch, -25, 0.25f * Time.deltaTime);
            peakAudio.enabled = true;
            angle.m_Dutch = Mathf.Lerp(angle.m_Dutch, -25, 3 * Time.deltaTime);
            offsett.m_Offset.x = Mathf.Lerp(offsett.m_Offset.x, 0.60f, 3 * Time.deltaTime);
            playOnce = false;
        }

        void PeakIdle()
        {
            if (!playOnce)
            {
                peakResetAudio.Play();
                playOnce = true;
            }
            peakAudio.enabled = false;
            angle.m_Dutch = Mathf.Lerp(angle.m_Dutch, 0, 4.5f * Time.deltaTime);
            offsett.m_Offset.x = Mathf.Lerp(offsett.m_Offset.x, 0, 4.5f * Time.deltaTime);
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
            if (camInputData.ZoomClicked && PlayerInventoryController.Instance.inventoryOpen)
                return;

            if (camInputData.ZoomReleased && PlayerInventoryController.Instance.inventoryOpen)
                return;

            if (camInputData.ZoomClicked)
                cameraZoom.ChangeFOV(this);

            if(camInputData.ZoomReleased)
                if(camInputData.IsZooming)
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
