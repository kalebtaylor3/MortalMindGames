using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace MMG
{    
    public class InputController : MonoBehaviour
    {
        #region Data
            [Space,Header("Input Data")]
            [SerializeField] public CameraInputData cameraInputData = null;
            [SerializeField] private MovementInputData movementInputData = null;
            [SerializeField] private InteractionInputData interactionInputData = null;
            [SerializeField] private InventoryInputData inventoryInputData = null;
            [SerializeField] private ItemInputData itemInputData = null;
            [SerializeField] private QuickTimeEventInputData quickTimeInputData = null;
            [SerializeField] private bool controllerConnected = false;

            [HideInInspector] public bool canMove = true;
            [HideInInspector] public bool canInteract = true;

        #endregion

        #region Functions
        void Start()
        {
            if (Joystick.current != null) 
            {
                controllerConnected = true;
            }

            WorldData.Instance.controllerActive = controllerConnected;

            cameraInputData.ResetInput();
            movementInputData.ResetInput();
            interactionInputData.ResetInput();
            canMove = true;
        }

        private void OnEnable()
        {
            //ConcelableAreaInteractable.OnEnteredSpot += UnCrouch;
            //uickTimeEventSystem.QTETrigger += OnCantMove;
        }

        void UnCrouch()
        {
            if(movementInputData.IsCrouching)
                movementInputData.CrouchClicked = true;
        }

        void OnCanMove()
        {
            canMove = true;
        }

        void OnCantMove()
        {
            canMove = false;
        }

        void Update()
        {
            GetCameraInput();

            if(canMove)
                GetMovementInputData();
            else
            {
                movementInputData.InputVectorY = 0;
                movementInputData.InputVectorX = 0;
            }

            if(canInteract)
                GetInteractionInputData();
            GetInventoryInputData();
            GetItemInputData();
            GetQuickTimeEventInputData();

        }

        void GetQuickTimeEventInputData()
        {            
            if(controllerConnected)
            {
                //Controller
                quickTimeInputData.SuccessKeyPressed = Gamepad.current.buttonWest.wasPressedThisFrame;
            }
            else
            {
                //Keyboard
                quickTimeInputData.SuccessKeyPressed = Input.GetKeyDown(KeyCode.Return);
            }
            

            
        }

        void GetInteractionInputData()
        {

            if (controllerConnected)
            {
                // Controller
                interactionInputData.InteractedClicked = Gamepad.current.buttonWest.wasPressedThisFrame;
                interactionInputData.InteractedReleased = Gamepad.current.buttonWest.wasReleasedThisFrame;
            }
            else
            {
                //Keyboard
                interactionInputData.InteractedClicked = Input.GetKeyDown(KeyCode.F);
                interactionInputData.InteractedReleased = Input.GetKeyUp(KeyCode.F);
            } 
        }

        void GetCameraInput()
        {
            if (canMove)
            {

                if (controllerConnected)
                {
                    // Controller
                    cameraInputData.InputVectorX = Gamepad.current.rightStick.x.ReadValue();
                    cameraInputData.InputVectorY = Gamepad.current.rightStick.y.ReadValue();
                }
                else
                {
                    // Mouse
                    cameraInputData.InputVectorX = Input.GetAxis("Mouse X");
                    cameraInputData.InputVectorY = Input.GetAxis("Mouse Y");
                    cameraInputData.InputVectorY = Input.GetAxis("Mouse Y");
                }
            }

            if (!inventoryInputData.OpenSpellBook)
            {
                if (controllerConnected)
                {
                    // Controller
                    cameraInputData.ZoomClicked = Gamepad.current.rightTrigger.wasPressedThisFrame;
                    cameraInputData.ZoomReleased = Gamepad.current.rightTrigger.wasReleasedThisFrame;
                }
                else
                {
                    //Keyboard
                    cameraInputData.ZoomClicked = Input.GetKeyDown(KeyCode.Mouse1);
                    cameraInputData.ZoomReleased = Input.GetKeyUp(KeyCode.Mouse1);
                } 
            }
        }

        void GetMovementInputData()
        {
            if (controllerConnected)
            {
                //Controller
                movementInputData.InputVectorX = Gamepad.current.leftStick.x.ReadValue();
                movementInputData.InputVectorY = Gamepad.current.leftStick.y.ReadValue();

                movementInputData.RunClicked = Gamepad.current.leftStickButton.IsPressed();
                movementInputData.RunReleased = Gamepad.current.leftStickButton.wasReleasedThisFrame;
            }
            else
            {
                //Keyboard
                movementInputData.InputVectorX = Input.GetAxis("Horizontal");
                movementInputData.InputVectorY = Input.GetAxis("Vertical");

                movementInputData.RunClicked = Input.GetKeyDown(KeyCode.LeftShift);
                movementInputData.RunReleased = Input.GetKeyUp(KeyCode.LeftShift);
            }

            if (movementInputData.RunClicked && movementInputData.IsCrouching)
                movementInputData.CrouchClicked = true;

            if (movementInputData.RunClicked)
                movementInputData.IsRunning = true;

            if (movementInputData.RunReleased)
                movementInputData.IsRunning = false;


            if (controllerConnected)
            {
                //Controller
                movementInputData.JumpClicked = Gamepad.current.buttonSouth.wasPressedThisFrame;
                movementInputData.CrouchClicked = Gamepad.current.buttonEast.wasPressedThisFrame;

                
                cameraInputData.IsPeakingLeft = Gamepad.current.leftShoulder.isPressed;
                cameraInputData.IsPeakingRight = Gamepad.current.rightShoulder.isPressed;
            }
            else
            {
                //Keyboard
                movementInputData.JumpClicked = Input.GetKeyDown(KeyCode.Space);
                movementInputData.CrouchClicked = Input.GetKeyDown(KeyCode.LeftControl);

                
                cameraInputData.IsPeakingLeft = Input.GetKey(KeyCode.Q);
                cameraInputData.IsPeakingRight = Input.GetKey(KeyCode.E);
            }
        }

        void GetInventoryInputData()
        {
            //inventoryInputData.OpenSpellBook = Gamepad.current.selectButton.wasPressedThisFrame;
            inventoryInputData.OpenSpellBook = Input.GetKeyDown(KeyCode.Tab);
        }

        void GetItemInputData()
        {
            if (movementInputData.HasActiveItem)
                itemInputData.UseItemClick = Input.GetMouseButtonDown(0);
        }

        #endregion
    }
}