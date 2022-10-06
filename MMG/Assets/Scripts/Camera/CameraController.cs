using NaughtyAttributes;
using UnityEngine;

namespace MMG
{    
    public class CameraController : MonoBehaviour
    {
        #region Variables
            #region Data
                [Space,Header("Data")]
                [SerializeField] private CameraInputData camInputData = null;

                [Space,Header("Custom Classes")]
                [SerializeField] private CameraZoom cameraZoom = null;
                [SerializeField] private CameraSwaying cameraSway = null;
                [SerializeField] private LayerMask interactableLayer = ~0;
                [SerializeField] private Transform peakLeft;
                [SerializeField] private Transform peakRight;
                [SerializeField] private Transform peakIdle;

            #endregion

            #region Settings
                [Space,Header("Look Settings")]
                [SerializeField] private Vector2 sensitivity = Vector2.zero;
                [SerializeField] private Vector2 smoothAmount = Vector2.zero;
                [SerializeField] [MinMaxSlider(-90f,90f)] private Vector2 lookAngleMinMax = Vector2.zero;
            #endregion

            #region Private
               private float m_yaw;
               private float m_pitch;

               private float m_desiredYaw;
               private float m_desiredPitch;

                #region Components                    
                    private Transform m_pitchTranform;
                    private Camera m_cam;
                #endregion
            #endregion
            
        #endregion

        #region BuiltIn Methods  
            void Awake()
            {
                GetComponents();
                InitValues();
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
        #endregion

        #region Custom Methods
            void GetComponents()
            {
                m_pitchTranform = transform.GetChild(0).transform;
                m_cam = GetComponentInChildren<Camera>();
            }

            void InitValues()
            {
               // m_yaw = transform.eulerAngles.y;
               // m_desiredYaw = m_yaw;
            }

            void InitComponents()
            {
                cameraZoom.Init(m_cam, camInputData);
                cameraSway.Init(m_cam.transform);
            }

            void HandlePeaking()
            {
                Ray _ray = new Ray(m_cam.transform.position, m_cam.transform.forward);
                RaycastHit _hitInfo;

                bool _hitSomething = Physics.SphereCast(_ray, 5f, out _hitInfo, 0.3f, interactableLayer);

                if (!_hitSomething)
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
                m_desiredYaw += camInputData.InputVector.x * sensitivity.x * Time.deltaTime;
                m_desiredPitch -= camInputData.InputVector.y * sensitivity.y * Time.deltaTime;

                m_desiredPitch = Mathf.Clamp(m_desiredPitch,lookAngleMinMax.x,lookAngleMinMax.y);
            }

            void SmoothRotation()
            {
                m_yaw = Mathf.Lerp(m_yaw,m_desiredYaw, smoothAmount.x * Time.deltaTime);
                m_pitch = Mathf.Lerp(m_pitch,m_desiredPitch, smoothAmount.y * Time.deltaTime);
            }

            void ApplyRotation()
            {
                transform.eulerAngles = new Vector3(0f,m_yaw,0f);
                m_pitchTranform.localEulerAngles = new Vector3(m_pitch,0f,0f);
            }

            public void HandleSway(Vector3 _inputVector,float _rawXInput)
            {
                cameraSway.SwayPlayer(_inputVector,_rawXInput);
            }

            void HandleZoom()
            {
                if(camInputData.ZoomClicked || camInputData.ZoomReleased)
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
