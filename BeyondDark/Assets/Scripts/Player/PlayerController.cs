using NaughtyAttributes;
using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Windows;

namespace MMG
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public bool safeZone = false;

        #region Variables
        #region Data

        [Space, Header("Data")]
        [SerializeField] public MovementInputData movementInputData = null;
        [SerializeField] private HeadBobData headBobData = null;
        [SerializeField] ItemInputData itemInputData = null;
        [SerializeField] private int attackNum = 1;
        //cycles through all attacks
        #endregion

        #region Locomotion

        [Space, Header("Locomotion Settings")]
        [SerializeField] private float crouchSpeed = 1f;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runSpeed = 3f;
        [SerializeField] private float jumpSpeed = 5f;
        [SerializeField] private float dashSpeed = 6f;
        [Slider(0f, 1f)][SerializeField] private float moveBackwardsSpeedPercentStanding = 0.4f;
        [Slider(0f, 1f)][SerializeField] private float moveBackwardsSpeedPercentCrouching = 0.5f;
        [Slider(0f, 1f)][SerializeField] private float moveSideSpeedPercent = 0.75f;

        [HideInInspector] public float speedCalculation;

        #endregion

        #region Run Settings

        [Space, Header("Run Settings")]
        [Slider(-1f, 1f)][SerializeField] private float canRunThreshold = 0.8f;

        [Slider(-1f, 1f)][SerializeField] private float staminaTest = 0.8f;

        [SerializeField] private AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField] private bool outOfStamina = false;

        #endregion

        #region Crouch Settings

        [Space, Header("Crouch Settings")]
        [Slider(0.2f, 0.9f)][SerializeField] private float crouchPercent = 0.6f;
        [SerializeField] private float crouchTransitionDuration = 1f;
        [SerializeField] private AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        #endregion

        #region Landing Settings

        [Space, Header("Landing Settings")]
        [Slider(0.05f, 0.5f)][SerializeField] private float lowLandAmount = 0.1f;
        [Slider(0.2f, 0.9f)][SerializeField] private float highLandAmount = 0.6f;
        [SerializeField] private float landTimer = 0.5f;
        [SerializeField] private float landDuration = 1f;
        [SerializeField] private AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        #endregion

        #region Gravity

        [Space, Header("Gravity Settings")]
        [SerializeField] private float gravityMultiplier = 2.5f;
        [SerializeField] private float stickToGroundForce = 5f;

        [SerializeField] private LayerMask groundLayer = ~0;
        [Slider(0f, 1f)][SerializeField] private float rayLength = 0.1f;
        [Slider(0.01f, 1f)][SerializeField] private float raySphereRadius = 0.1f;

        #endregion

        #region Inventory

        [Space, Header("Inventory Settings")]
        PlayerInventoryController playerInventory;
        GameObject activeItem = null;
        public Transform[] bookSlots;

        #endregion

        #region Wall Settings

        [Space, Header("Check Wall Settings")]
        [SerializeField] private LayerMask obstacleLayers = ~0;
        [Slider(0f, 1f)][SerializeField] private float rayObstacleLength = 0.1f;
        [Slider(0.01f, 1f)][SerializeField] private float rayObstacleSphereRadius = 0.1f;

        #endregion

        #region Smooth Settings

        [Space, Header("Smooth Settings")]
        [Range(1f, 100f)][SerializeField] private float smoothRotateSpeed = 5f;
        [Range(1f, 100f)][SerializeField] private float smoothInputSpeed = 5f;
        [Range(1f, 100f)][SerializeField] private float smoothVelocitySpeed = 5f;
        [Range(1f, 100f)][SerializeField] private float smoothFinalDirectionSpeed = 5f;
        [Range(1f, 100f)][SerializeField] private float smoothHeadBobSpeed = 5f;

        [Space]
        private bool experimental = false;
        [ShowIf("experimental")][Range(1f, 100f)][SerializeField] private float smoothInputMagnitudeSpeed = 5f;

        #endregion

        #region Components / Custom Classes / Caches
        private CharacterController characterController;
        private Transform yawTransform;
        private Transform camTransform;
        private HeadBob headBob;
        public CameraController cameraController;

        private RaycastHit hitInfo;
        private IEnumerator _CrouchRoutine;
        private IEnumerator _LandRoutine;
        #endregion

        #region Debug
        private Vector2 inputVector;
        private Vector2 smoothInputVector;

        private Vector3 finalMoveDir;
        private Vector3 smoothFinalMoveDir;

        private Vector3 finalMoveVector;

        [SerializeField]
        public float currentSpeed;
        public bool isDashing = false;
        [SerializeField]
        private float smoothCurrentSpeed;
        private float finalSmoothCurrentSpeed;
        private float walkRunSpeedDifference;

        private float finalRayLength;
        private bool hitWall;
        private bool isGrounded;
        private bool previouslyGrounded;

        private float initHeight;
        private float crouchHeight;
        private Vector3 initCenter;
        private Vector3 crouchCenter;
        private float initCamHeight;
        private float crouchCamHeight;
        private float crouchStandHeightDifference;
        private bool duringCrouchAnimation;
        private bool duringRunAnimation;
        private float inAirTimer;

        private float inputVectorMagnitude;
        private float smoothInputVectorMagnitude;
        #endregion

        #region Sounds

        [Space, Header("Foot Step Paramaters")]
        [SerializeField] private float stepSpeed = 0.9f;
        int lastStepNumber;
        [SerializeField] private AudioSource footStepAudioSource;
        [SerializeField] private AudioClip[] grassWalkSounds;
        [SerializeField] private AudioClip[] gravelWalkSounds;
        [SerializeField] private AudioClip[] dirtWalkSounds;
        [SerializeField] private AudioClip[] woodWalkSounds;

        // EFFECTS / Audio TESTING
        [SerializeField] private AudioSource EffectAudioSource;
        [SerializeField] private AudioClip[] SoundEffects;

        private float footStepTimer = 0;
        [HideInInspector] public float GetCurrentOffset;

        public AudioSource pickUpSource;

        #endregion

        #region Vibration System

        [Range(0.1f, 1.5f)]
        public float PulseFrequency = 1.2f;

        private float rumbleTime = 1f;
        private RumblePattern rumblePattern = RumblePattern.Constant;
        [SerializeField] Rumbler rumbler;
        private int[] timeDropdown = new int[] { 3, 5, 10 };
        private RumblePattern[] rumbleMode = new RumblePattern[] { RumblePattern.Constant, RumblePattern.CollectRellic, RumblePattern.Linear };

        #endregion


        [SerializeField]
        UIManager UiManager;

        public static event Action OnEnmptyStamina;
        public static event Action OnTeleport;

        [SerializeField] public bool isHiding = false;

        [SerializeField] public GameObject PlayerKillCollision;

        public Animator hands;

        bool canMove = true;

        public GameObject compass;

        #endregion

        #region Functions

        private void Awake()
        {
            playerInventory = GetComponent<PlayerInventoryController>();
            GetComponents();
            InitVariables();
            canMove = true;
            //ItemInteractable.OnPickUp += HandlePickUp;
            //ConcelableAreaInteractable.OnEnteredSpot += SetHiding;
            //ConcelableAreaInteractable.OnLeaveSpot += NotHiding;
        }

        void Start()
        {
            //GetComponents();
            //InitVariables();

            playerInventory = GetComponent<PlayerInventoryController>();
            //ItemInteractable.OnPickUp += HandlePickUp;
            //ConcelableAreaInteractable.OnEnteredSpot += SetHiding;
            //ConcelableAreaInteractable.OnLeaveSpot += NotHiding;
            //TpTest.RealmTransportation += HandleRealmTransport;
        }

        void CanMove(bool value)
        {
            canMove = value;
        }

        void Update()
        {
            if(yawTransform != null)
                RotateTowardsCamera();

            if (isHiding)
            {
                characterController.enabled = false;
                compass.SetActive(false);   
            }
            else
            {
                characterController.enabled = true;
                compass.SetActive(true);
            }

            if (characterController && canMove)
            {
                // Check if Grounded,Wall etc
                CheckIfGrounded();
                CheckIfWall();

                // Apply Smoothing
                SmoothInput();
                SmoothSpeed();
                SmoothDir();

                // Calculate Movement
                CalculateMovementDirection();
                CalculateSpeed();
                CalculateFinalMovement();


                // Stamina 
                HandleStamina();

                // Handle Player Movement, Gravity, Jump, Crouch etc.
                HandleCrouch();
                HandleHeadBob();
                HandleRunFOV();
                HandleCameraSway();
                HandleLanding();
                    
                //apply Gravity & Movement
                ApplyGravity();
                ApplyMovement();

                HasActiveItem();
                //activeItem = playerInventory.ReturnActiveItem();
                CheckForItemUse(activeItem);

                HandleFootSteps();
                SetCurrentOffSet();

                // Stamina
                UiManager.UpdateStaminaSlider(staminaTest);
                
                previouslyGrounded = isGrounded;
                //combat
                UseDefend();
                UseAttack();
                SwitchAttack();

                if (hands != null)
                {
                    if (currentSpeed == runSpeed)
                    {
                        hands.SetBool("IsRunning", true);
                    }
                    else
                    {
                        hands.SetBool("IsRunning", false);
                    }

                    if(movementInputData.IsCrouching)
                    {
                        hands.SetBool("IsCrouched", true);

                        if(currentSpeed == crouchSpeed)
                            hands.SetBool("IsCrouchedWalking", true);
                        else
                            hands.SetBool("IsCrouchedWalking", false);

                    }
                    else
                    {
                        hands.SetBool("IsCrouched", false);
                        hands.SetBool("IsCrouchedWalking", false);
                    }

                }
            }
        }
        //combat
        void SetHiding()
        {
            isHiding = true;
        }

        void NotHiding()
        {
            isHiding = false;
            this.GetComponent<PlayerInventoryController>().inventoryOpen = false;
        }

        void SetCurrentOffSet()
        {
            if(currentSpeed >= runSpeed)
            {
                GetCurrentOffset = 0.35f;
            }
            else if(currentSpeed >= walkSpeed - 0.5f)
            {
                GetCurrentOffset = 0.55f;
            }
            else
            {
                GetCurrentOffset = 0.75f;
            }
        }


        #region Initialize Methods    

        private void OnEnable()
        {
            playerInventory = GetComponent<PlayerInventoryController>();
            // Sound Effect for realm tp
            HandleRealmTransport();

            QuickTimeEventSystem.QTETrigger += HandleQuickTimeEvent;
            QuickTimeEventSystem.OnSuccess += HandleSuccess;
            QuickTimeEventSystem.OnFailure += HandleFailure;
            ItemInteractable.OnPickUp += HandlePickUp;

            ConcelableAreaInteractable.OnEnteredSpot += SetHiding;
            ConcelableAreaInteractable.OnLeaveSpot += NotHiding;
            PlayerHealthSystem.OnDeath += CanMove;

            canMove = true;

            GetComponents();
            //playerInventory = GetComponent<PlayerInventoryController>();

        }



        private void OnDisable()
        {
            QuickTimeEventSystem.QTETrigger -= HandleQuickTimeEvent;
            QuickTimeEventSystem.OnSuccess -= HandleSuccess;
            QuickTimeEventSystem.OnFailure -= HandleFailure;
            ItemInteractable.OnPickUp -= HandlePickUp;

            //ItemInteractable.OnPickUp += HandlePickUp;
            ConcelableAreaInteractable.OnEnteredSpot -= SetHiding;
            ConcelableAreaInteractable.OnLeaveSpot -= NotHiding;
            PlayerHealthSystem.OnDeath -= CanMove;
        }

        void GetComponents()
            {
                playerInventory = GetComponent<PlayerInventoryController>();
                characterController = GetComponent<CharacterController>();
                cameraController = GetComponentInChildren<CameraController>();
                yawTransform = cameraController.transform;
                camTransform = GetComponentInChildren<Camera>().transform;
                headBob = new HeadBob(headBobData, moveBackwardsSpeedPercentStanding, moveSideSpeedPercent);
            }

            void InitVariables()
            {   
                // Calculate where our character center should be based on height and skin width
                characterController.center = new Vector3(0f,characterController.height / 2f + characterController.skinWidth,0f);

                initCenter = characterController.center;
                initHeight = characterController.height;

                crouchHeight = initHeight * crouchPercent;
                crouchCenter = (crouchHeight / 2f + characterController.skinWidth) * Vector3.up;

                crouchStandHeightDifference = initHeight - crouchHeight;

                initCamHeight = yawTransform.localPosition.y;
                crouchCamHeight = initCamHeight - crouchStandHeightDifference;

                // Sphere radius not included. If you want it to be included just decrease by sphere radius at the end of this equation
                finalRayLength = rayLength + characterController.center.y;

                isGrounded = true;
                previouslyGrounded = true;

                inAirTimer = 0f;
                headBob.CurrentStateHeight = initCamHeight;

                walkRunSpeedDifference = runSpeed - walkSpeed;
            }
        #endregion

        #region Smoothing Methods
            void SmoothInput()
            {
                inputVector = movementInputData.InputVector.normalized;
                smoothInputVector = Vector2.Lerp(smoothInputVector,inputVector,Time.deltaTime * smoothInputSpeed);
                //Debug.DrawRay(transform.position, new Vector3(m_smoothInputVector.x,0f,m_smoothInputVector.y), Color.green);
            }

            void SmoothSpeed()
            {
                smoothCurrentSpeed = Mathf.Lerp(smoothCurrentSpeed, currentSpeed, Time.deltaTime * smoothVelocitySpeed);

                if(movementInputData.IsRunning && CanRun())
                {
                    float walkRunPercent = Mathf.InverseLerp(walkSpeed,runSpeed, smoothCurrentSpeed);
                    finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(walkRunPercent) * walkRunSpeedDifference + walkSpeed;
                }
                else
                {
                    finalSmoothCurrentSpeed = smoothCurrentSpeed;
                }
            }

            void SmoothDir()
            {

                smoothFinalMoveDir = Vector3.Lerp(smoothFinalMoveDir, finalMoveDir, Time.deltaTime * smoothFinalDirectionSpeed);
                Debug.DrawRay(transform.position, smoothFinalMoveDir, Color.yellow);
            }
                
            void SmoothInputMagnitude()
            {
                inputVectorMagnitude = inputVector.magnitude;
                smoothInputVectorMagnitude = Mathf.Lerp(smoothInputVectorMagnitude, inputVectorMagnitude, Time.deltaTime * smoothInputMagnitudeSpeed);
            }
        #endregion

        #region Locomotion Calculation Methods
            
            void HandleFootSteps()
            {
                if (!isGrounded) return;
                if (currentSpeed == 0) return;

                footStepTimer -= Time.deltaTime;

                if(footStepTimer <= 0)
                {
                    if (Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Vector3.down, out RaycastHit hit, 6, obstacleLayers))
                    {
                        int rand = GenerateRandomNumber();
                    //change sound depending on terrain

                    Terrain terrain = hit.collider.GetComponent<Terrain>();
                    if (terrain != null)
                    {
                        TerrainData terrainData = terrain.terrainData;

                        // Convert the hit point to alphamap space
                        float mapX = hit.textureCoord.x * terrainData.alphamapWidth;
                        float mapZ = hit.textureCoord.y * terrainData.alphamapHeight;

                        // Get the alphamaps at the hit point
                        float[,,] alphamaps = terrainData.GetAlphamaps(
                            Mathf.FloorToInt(mapX),
                            Mathf.FloorToInt(mapZ),
                            1, 1
                        );

                        // Find the index of the highest alpha value
                        float maxAlpha = 0f;
                        int maxIndex = -1;
                        for (int i = 0; i < alphamaps.GetLength(2); i++)
                        {
                            if (alphamaps[0, 0, i] > maxAlpha)
                            {
                                maxAlpha = alphamaps[0, 0, i];
                                maxIndex = i;
                            }
                        }

                        // Get the name of the terrain layer at that index
                        string layerName = terrainData.terrainLayers[maxIndex].name;

                        switch(layerName)
                        {
                            case "Grass":
                                footStepAudioSource.clip = grassWalkSounds[rand];
                                break;
                            case "Gravel 1":
                                footStepAudioSource.clip = gravelWalkSounds[rand];
                                break;
                            case "Gravel 2":
                                footStepAudioSource.clip = gravelWalkSounds[rand];
                                break;
                            case "Gravel 3":
                                footStepAudioSource.clip = gravelWalkSounds[rand];
                                break;
                            case "Gravel 4":
                                footStepAudioSource.clip = gravelWalkSounds[rand];
                                break;
                            case "Gravel 5":
                                footStepAudioSource.clip = gravelWalkSounds[rand];
                                break;
                            case "Dirt1":
                                footStepAudioSource.clip = dirtWalkSounds[rand];
                                break;
                            case "Dirt2":
                                footStepAudioSource.clip = dirtWalkSounds[rand];
                                break;
                            case "Dirt3":
                                footStepAudioSource.clip = dirtWalkSounds[rand];
                                break;
                            case "Dirt4":
                                footStepAudioSource.clip = dirtWalkSounds[rand];
                                break;
                        }

                    }
                    else
                    {
                            switch (hit.collider.tag)
                            {
                                case "Grass":
                                    footStepAudioSource.clip = grassWalkSounds[rand];
                                    break;
                                case "Gravel":
                                    footStepAudioSource.clip = gravelWalkSounds[rand];
                                    break;
                                case "Wood":
                                    footStepAudioSource.clip = woodWalkSounds[rand];
                                    break;
                                case "Dirt":
                                    footStepAudioSource.clip = dirtWalkSounds[rand];
                                    break;
                            }
                        }
                    }

                    if (movementInputData.IsRunning && CanRun())
                        footStepAudioSource.pitch = 1.3f;
                    else
                        footStepAudioSource.pitch = 1;

                    footStepAudioSource.volume = currentSpeed / 3;
                    footStepAudioSource.Play();
                    footStepTimer = GetCurrentOffset;
                }

            }

            int GenerateRandomNumber()
            {
                int newNumber;
                do
                {
                    newNumber = UnityEngine.Random.Range(0, 3);
                }
                while (newNumber == lastStepNumber);

                lastStepNumber = newNumber;
                return newNumber;
            }

        void CheckIfGrounded()
            {
                Vector3 origin = transform.position + characterController.center;

                bool hitGround = Physics.SphereCast(origin,raySphereRadius,Vector3.down,out hitInfo,finalRayLength,groundLayer);
                Debug.DrawRay(origin,Vector3.down * (finalRayLength),Color.red);

                isGrounded = hitGround ? true : false;
            }

            void CheckIfWall()
            {
                    
                Vector3 origin = transform.position + characterController.center;
                RaycastHit wallInfo;

                bool hitWall = false;

                if(movementInputData.HasInput && finalMoveDir.sqrMagnitude > 0)
                    hitWall = Physics.SphereCast(origin,rayObstacleSphereRadius,finalMoveDir, out wallInfo,rayObstacleLength,obstacleLayers);
                Debug.DrawRay(origin,finalMoveDir * rayObstacleLength,Color.blue);

                this.hitWall = hitWall ? true : false;
            }

            bool CheckIfRoof() /// TO FIX
            {
                Vector3 origin = transform.position;
                RaycastHit _roofInfo;

                bool hitRoof = Physics.SphereCast(origin,raySphereRadius,Vector3.up,out _roofInfo,initHeight);

                return hitRoof;
            }

            bool CanRun()
            {
                if(staminaTest >= 0)
                {
                    Vector3 normalizedDir = Vector3.zero;

                    if (smoothFinalMoveDir != Vector3.zero)
                        normalizedDir = smoothFinalMoveDir.normalized;

                    float dot = Vector3.Dot(transform.forward, normalizedDir);
                    return dot >= canRunThreshold && !movementInputData.IsCrouching ? true : false;
                }
                else
                {
                    return false;
                }                
            }

            void CalculateMovementDirection()
            {

                Vector3 vDir = transform.forward * smoothInputVector.y;
                Vector3 hDir = transform.right * smoothInputVector.x;

                Vector3 desiredDir = vDir + hDir;
                Vector3 flattenDir = FlattenVectorOnSlopes(desiredDir);

                finalMoveDir = flattenDir;
            }

            Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
            {
                if(isGrounded)
                    vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat,hitInfo.normal);
                    
                return vectorToFlat;
            }

            void CalculateSpeed()
            {
                currentSpeed = movementInputData.IsRunning && CanRun() ? runSpeed : walkSpeed;
                currentSpeed = movementInputData.IsCrouching ? crouchSpeed : currentSpeed;
                currentSpeed = !movementInputData.HasInput ? 0f : currentSpeed;
                currentSpeed = isDashing ? dashSpeed : currentSpeed;
                if(!movementInputData.IsCrouching)
                    currentSpeed = movementInputData.InputVector.y <= -1 ? currentSpeed * moveBackwardsSpeedPercentStanding : currentSpeed;
                else
                    currentSpeed = movementInputData.InputVector.y <= -1 ? currentSpeed * moveBackwardsSpeedPercentCrouching : currentSpeed;
                currentSpeed = movementInputData.InputVector.x != 0 && movementInputData.InputVector.y <  0.8f ? currentSpeed * moveSideSpeedPercent :  currentSpeed;

                speedCalculation = Mathf.Abs(movementInputData.InputVector.x + movementInputData.InputVector.y);

                speedCalculation =  Mathf.Clamp(speedCalculation, 0, 1);

                //Debug.Log("X :" + movementInputData.InputVector.x + "Y : " + movementInputData.InputVector.y);

            //Debug.Log(speedCalculation);

            if (!outOfStamina)
            {

                if (!movementInputData.IsRunning && speedCalculation <= 0.65f && speedCalculation > 0)
                    currentSpeed = runSpeed;

                if (movementInputData.IsRunning && speedCalculation <= 0.25f && speedCalculation > 0)
                    currentSpeed = runSpeed;

                if (movementInputData.IsRunning && speedCalculation > 1.4f)
                    currentSpeed = runSpeed;
            }

            if (!movementInputData.IsCrouching)
            {

                if (!movementInputData.IsRunning && speedCalculation <= 0.9 && speedCalculation > 0)
                    currentSpeed = walkSpeed * speedCalculation;

                if (movementInputData.InputVector.x < 0 && movementInputData.InputVector.y < 1f && movementInputData.InputVector.y > 0.7f)
                    currentSpeed = walkSpeed;

                    if (movementInputData.InputVector.y < 0)
                        currentSpeed = walkSpeed * moveBackwardsSpeedPercentStanding;
            }
            else
            {

                if (!movementInputData.IsRunning && speedCalculation <= 0.9 && speedCalculation > 0)
                    currentSpeed = crouchSpeed * speedCalculation;

                if (movementInputData.InputVector.x < 0 && movementInputData.InputVector.y < 1f && movementInputData.InputVector.y > 0.7f)
                    currentSpeed = crouchSpeed;

                if (movementInputData.InputVector.y < 0)
                        currentSpeed = crouchSpeed * moveBackwardsSpeedPercentCrouching;
            }


            //Debug.Log(currentSpeed);




            //if (movementInputData.IsRunning && currentSpeed >= runSpeed && CanRun())
            //    currentSpeed = runSpeed;

            //if (!movementInputData.IsRunning && currentSpeed >= walkSpeed)
            //    currentSpeed = walkSpeed;

            //if (currentSpeed < 0.5f)
            //    currentSpeed = 0;
            }

            void CalculateFinalMovement()
            {
                float smoothInputVectorMagnitude = experimental ? this.smoothInputVectorMagnitude : 1f;
                Vector3 finalVector = smoothFinalMoveDir * finalSmoothCurrentSpeed * smoothInputVectorMagnitude;

                finalMoveVector.x = finalVector.x ;
                finalMoveVector.z = finalVector.z ;

                if(characterController.isGrounded) 
                    finalMoveVector.y += finalVector.y ;

            }

            void HandleStamina()
            {
                if(movementInputData.IsRunning && currentSpeed >= runSpeed)
                {
                    staminaTest -= 0.10f * Time.deltaTime;
                }
                else if (!movementInputData.IsRunning)
                {
                    staminaTest += 0.2f * Time.deltaTime;
                }

                if(staminaTest <= 0f && movementInputData.IsRunning)
                {
                    //cameraController.ResetFOV();
                    staminaTest = -0.1f;
                    outOfStamina = true;
                    OnEnmptyStamina?.Invoke();
                }
                
                if(staminaTest <= 0f)
                {
                    if(outOfStamina && !EffectAudioSource.isPlaying)
                    {
                        outOfStamina = false;
                        EffectAudioSource.PlayOneShot(SoundEffects[0]);                    
                        OnEnmptyStamina?.Invoke();
                        cameraController.ResetFOV();
                    }                   
                }
                else if( staminaTest > 1f)
                {
                    staminaTest = 1f;
                    outOfStamina = false;
                }
            }


        #endregion

        #region Crouching Methods
            void HandleCrouch()
            {
                if(movementInputData.CrouchClicked && isGrounded)
                    InvokeCrouchRoutine();

                if (movementInputData.RunClicked && movementInputData.IsCrouching)
                    InvokeCrouchRoutine();
            }

            void InvokeCrouchRoutine()
            {
                if(movementInputData.IsCrouching)
                    if(CheckIfRoof())
                        return;

                if(_LandRoutine != null)
                    StopCoroutine(_LandRoutine);

                if(_CrouchRoutine != null)
                    StopCoroutine(_CrouchRoutine);

                _CrouchRoutine = CrouchRoutine();
                StartCoroutine(_CrouchRoutine);
            }

            IEnumerator CrouchRoutine()
            {
                duringCrouchAnimation = true;

                float percent = 0f;
                float smoothPercent = 0f;
                float speed = 1f / crouchTransitionDuration;

                float currentHeight = characterController.height;
                Vector3 currentCenter = characterController.center;

                float desiredHeight = movementInputData.IsCrouching ? initHeight : crouchHeight;
                Vector3 desiredCenter = movementInputData.IsCrouching ? initCenter : crouchCenter;

                Vector3 camPos = yawTransform.localPosition;
                float camCurrentHeight = camPos.y;
                float camDesiredHeight = movementInputData.IsCrouching ? initCamHeight : crouchCamHeight;

                movementInputData.IsCrouching = !movementInputData.IsCrouching;
                headBob.CurrentStateHeight = movementInputData.IsCrouching ? crouchCamHeight : initCamHeight;

                while(percent < 1f)
                {
                    percent += Time.deltaTime * speed;
                    smoothPercent = crouchTransitionCurve.Evaluate(percent);

                    characterController.height = Mathf.Lerp(currentHeight,desiredHeight,smoothPercent);
                    characterController.center = Vector3.Lerp(currentCenter,desiredCenter,smoothPercent);

                    camPos.y = Mathf.Lerp(camCurrentHeight,camDesiredHeight, smoothPercent);
                    yawTransform.localPosition = camPos;

                    yield return null;
                }

                duringCrouchAnimation = false;
            }
                
        #endregion

        #region Landing Methods
            void HandleLanding()
            {
                if(!previouslyGrounded && isGrounded)
                {
                    InvokeLandingRoutine();
                }
            }

            void InvokeLandingRoutine()
            {
                if(_LandRoutine != null)
                    StopCoroutine(_LandRoutine);

                _LandRoutine = LandingRoutine();
                StartCoroutine(_LandRoutine);
            }

            IEnumerator LandingRoutine()
            {
                //float percent = 0f;
                //float landAmount = 0f;

                //float speed = 1f / landDuration;

                //Vector3 localPos = yawTransform.localPosition;
                //float initLandHeight = localPos.y;

                //landAmount = inAirTimer > landTimer ? highLandAmount : lowLandAmount;

                //while(percent < 1f)
                //{
                //    percent += Time.deltaTime * speed;
                //    float desiredY = landCurve.Evaluate(percent) * landAmount;

                //    localPos.y = initLandHeight + desiredY;
                //    yawTransform.localPosition = localPos;

                   yield return null;

                //}
            }
        #endregion

        #region Locomotion Apply Methods

            void HandleHeadBob()
            {
                if(movementInputData.HasInput && isGrounded  && !hitWall)
                {
                    if(!duringCrouchAnimation) // we want to make our head bob only if we are moving and not during crouch routine
                    {
                        if (currentSpeed == 0 || currentSpeed < 0.5f)
                            return;

                    if (!movementInputData.IsCrouching)
                    {
                        headBob.ScrollHeadBob(movementInputData.IsRunning && CanRun(), movementInputData.IsCrouching, movementInputData.InputVector);
                        yawTransform.localPosition = Vector3.Lerp(yawTransform.localPosition, (Vector3.up * headBob.CurrentStateHeight) + (headBob.FinalOffset * currentSpeed) / 3, Time.deltaTime * smoothHeadBobSpeed);
                    }
                    else
                    {
                        headBob.ScrollHeadBob(movementInputData.IsRunning && CanRun(), movementInputData.IsCrouching, movementInputData.InputVector);
                        yawTransform.localPosition = Vector3.Lerp(yawTransform.localPosition, (Vector3.up * headBob.CurrentStateHeight) + headBob.FinalOffset, Time.deltaTime * smoothHeadBobSpeed);
                    }
                }
                }
                else // if we are not moving or we are not grounded
                {
                    if(!headBob.Resetted)
                    {
                        headBob.ResetHeadBob();
                    }

                    if(!duringCrouchAnimation) // we want to reset our head bob only if we are standing still and not during crouch routine
                        yawTransform.localPosition = Vector3.Lerp(yawTransform.localPosition,new Vector3(0f,headBob.CurrentStateHeight,0f),Time.deltaTime * smoothHeadBobSpeed);
                }
            }

            void HandleCameraSway()
            {
                cameraController.HandleSway(smoothInputVector,movementInputData.InputVector.x);
            }

            void HandleRunFOV()
            {
                if(movementInputData.HasInput && isGrounded  && !hitWall)
                {
                    if(movementInputData.RunClicked && CanRun())
                    {
                        duringRunAnimation = true;
                        cameraController.ChangeRunFOV(false);
                    }

                    if(movementInputData.IsRunning && CanRun() && !duringRunAnimation )
                    {
                        duringRunAnimation = true;
                        cameraController.ChangeRunFOV(false);
                    }
                }

                if(movementInputData.RunReleased || !movementInputData.HasInput || hitWall)
                {
                    if(duringRunAnimation)
                    {
                        duringRunAnimation = false;
                        cameraController.ChangeRunFOV(true);
                    }
                }
            }
            void HandleJump()
            {
                if(movementInputData.JumpClicked && !movementInputData.IsCrouching)
                {
                    //m_finalMoveVector.y += jumpSpeed /* m_currentSpeed */; // we are adding because ex. when we are going on slope we want to keep Y value not overwriting it
                    finalMoveVector.y = jumpSpeed /* m_currentSpeed */; // turns out that when adding to Y it is too much and it doesn't feel correct because jumping on slope is much faster and higher;
                    
                    previouslyGrounded = true;
                    isGrounded = false;
                }
            }
            void ApplyGravity()
            {
                if(characterController.isGrounded) // if we would use our own m_isGrounded it would not work that good, this one is more precise
                {
                    inAirTimer = 0f;
                    finalMoveVector.y = -stickToGroundForce;

                    HandleJump();
                }
                else
                {
                    inAirTimer += Time.deltaTime;
                    finalMoveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
                }
            }

            public void PlayerDash(bool state)
            {
                isDashing = state;

                if (isDashing)
                {
                    cameraController.ChangeRunFOV(false);
                }
                else if (!isDashing)
                {
                    cameraController.ChangeRunFOV(true);
                }
            }

            void ApplyMovement()
            {
                characterController.Move(finalMoveVector * Time.deltaTime);
            }

            void RotateTowardsCamera()
            {
                Quaternion currentRot = transform.rotation;
                Quaternion desiredRot = yawTransform.rotation;

                transform.rotation = Quaternion.Slerp(currentRot,desiredRot,Time.deltaTime * smoothRotateSpeed);
            }



        #endregion

        #region ItemMethods / InventoryMethods
            void HandlePickUp(PickUp ItemPickUp)
            {
                // Only handle pick ups if the player is the Mortal Real Player
                if(playerInventory.tag == "Player" && playerInventory)
                {
                    // check if the item already exists in the inventory
                    bool containItem = false;

                    for (int i = 0; i < playerInventory.items.Count; i++)
                    {
                        if (playerInventory.items[i].GetComponent<Item>().relicType == ItemPickUp.relicType)
                        {
                            containItem = true; 
                            break;
                        }                        
                    }

                    // check if the item already exists in the inventory
                    if (!containItem)
                    {
                        //Sound For Pick Up
                        if(ItemPickUp.relicType == RelicSpawnManager.RELIC_TYPE.MAP)
                        {
                            AddItemToInventory(ItemPickUp);
                        }
                        pickUpSource.clip = ItemPickUp.pickUpClip;
                        pickUpSource.Play();


                        
                        //PickUp.PickUpItem.SetActive(false);                
                        SetRumbleMode(1);
                        StartRumble();

                        WorldData.Instance.SetCheckpoint();

                        WorldData.Instance.ItemPickedUp(ItemPickUp.relicType, this.transform.position, ItemPickUp.gameObject);

                        if(movementInputData.IsCrouching)
                            InvokeCrouchRoutine();

                        // Move this to after a trial is completed or failed
                        //RelicSpawnManager.Instance.RelicPickedUp(ItemPickUp.gameObject);

                        // Player Inventory to World Data and triger realm tp
                        if (ItemPickUp != null)
                        {
                            if (ItemPickUp.RealmTp)
                            {
                                TpTest.Instance.tpPlayer(ItemPickUp.VorgonTpPosition);
                            }
                        }
                    }
                }
            }

            public void AddItemToInventory(PickUp ItemPickUp)
            {
                ItemPickUp.PickUpItem = Instantiate(ItemPickUp.PickUpItem, bookSlots[ItemPickUp.pickUpID]);

                
                    playerInventory.items.Add(ItemPickUp.PickUpItem);
                    playerInventory.UpdatePages();
            }

            void HasActiveItem()
            {
                if(activeItem != null)
                {
                    movementInputData.HasActiveItem = true;
                }
            }

            void CheckForItemUse(GameObject activeItem)
            {
                if(itemInputData.UseItemClick)
                {
                    if (activeItem != null)
                    {
                        Item data = activeItem.GetComponent<Item>();

                        data.UseItem();
                    }
                }
            }

        public void ConcealEnterKillCollision(bool flag)
        {
            PlayerKillCollision.SetActive(flag);
        }

        public void HandleRealmTransport()
        {
            EffectAudioSource.PlayOneShot(SoundEffects[1]);
            staminaTest = 0.8f;

            OnTeleport?.Invoke();
           // bool defendReleased;
           // bool attackReleased;
           // bool aSwitchReleased;
        }
        void UseDefend()
        {
           if(movementInputData.UseDefend == true)
            {
                //Debug.Log("Wall of Souls");

            }
        }
        void UseAttack()
        {
            if (movementInputData.UseAttack == true)
            {
                if (attackNum == 1)
                {
                    //Debug.Log("Unstoppable Spear");
                } else
                {
                    //Debug.Log("Wild Ride");
                }

            }
        }
        void SwitchAttack()
        {
            if (movementInputData.SwitchAttack == true)
            {
                //Debug.Log("I N D E E D");
                if(attackNum == 1)
                {
                    attackNum = 2;
                    //Debug.Log("Vorgon's Sword" + attackNum);

                } else if(attackNum == 2)
                {
                    attackNum = 1;
                    //Debug.Log("The Flames Of Darkness" + attackNum);
                }
            }
        }
        #endregion

        #region Vibration Methods

        public void SetRumbleMode(int selectedValue)
        {
            rumblePattern = rumbleMode[selectedValue];
        }
        public void SetDurration(float value)
        {
            rumbleTime = value;
        }

        public void StartRumble()
        {
            switch (rumblePattern)
            {
                case RumblePattern.Constant:
                    rumbler.RumbleConstant(0, 1f, rumbleTime);
                    break;
                case RumblePattern.CollectRellic:
                    rumbler.RumblePulse(0f, 1, PulseFrequency, rumbleTime);
                    break;
                case RumblePattern.Linear:
                    rumbler.RumbleLinear(0, 1f, 0, 0.5f, rumbleTime);
                    break;
                default:
                    break;
            }

        }

        public void StopRumble()
        {
            rumbler.StopRumble();
        }

        #endregion

        #region QuickTimeEventMethods

        void HandleQuickTimeEvent()
        {
            //disable movement
        }

        void HandleSuccess()
        {
            SetRumbleMode(1);
            StartRumble();
        }

        void HandleFailure()
        {
            SetDurration(1);
            SetRumbleMode(2);
            StartRumble();
        }

        #endregion

        #endregion
    }
}
