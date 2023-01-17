using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
//mostly the same just replaces peeking and zooming with combat
namespace MMG
{    
    public class InputController2 : MonoBehaviour
    {
        
#region Data
            [Space,Header("Input Data")]
            [SerializeField] private CameraInputData cameraInputData = null;
            [SerializeField] private MovementInputData movementInputData = null;
            [SerializeField] private InteractionInputData interactionInputData = null;
            [SerializeField] private InventoryInputData inventoryInputData = null;
            [SerializeField] private ItemInputData itemInputData = null;

            [HideInInspector] public bool canMove = true;
            [HideInInspector] public bool canInteract = true;
        [HideInInspector] public bool isInVorgonRealm = true;
        #endregion
        #region Functions
        void Start()
            {
                cameraInputData.ResetInput();
                movementInputData.ResetInput();
                interactionInputData.ResetInput();
                canMove = true;
            }

        private void OnEnable()
        {
            ConcelableAreaInteractable.OnEnteredSpot += UnCrouch;
        }

        void UnCrouch()
        {
            if(movementInputData.IsCrouching)
                movementInputData.CrouchClicked = true;
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

        }
        void GetInteractionInputData()
        {
            interactionInputData.InteractedClicked = Gamepad.current.buttonWest.wasPressedThisFrame;
            interactionInputData.InteractedReleased = Gamepad.current.buttonWest.wasReleasedThisFrame;
        }

        void GetCameraInput()
        {
            if (canMove)
            {
                cameraInputData.InputVectorX = Gamepad.current.rightStick.x.ReadValue();
                cameraInputData.InputVectorY = Gamepad.current.rightStick.y.ReadValue();
            }
            if (!isInVorgonRealm)
            {
                cameraInputData.ZoomClicked = Gamepad.current.rightTrigger.wasPressedThisFrame;
                cameraInputData.ZoomReleased = Gamepad.current.rightTrigger.wasReleasedThisFrame;
            }
        }

        void GetMovementInputData()
        {
            movementInputData.InputVectorX = Gamepad.current.leftStick.x.ReadValue();
            movementInputData.InputVectorY = Gamepad.current.leftStick.y.ReadValue();

            movementInputData.RunClicked = Gamepad.current.leftStickButton.IsPressed();
            movementInputData.RunReleased = Gamepad.current.leftStickButton.wasReleasedThisFrame;

            if(movementInputData.RunClicked && movementInputData.IsCrouching)
                movementInputData.CrouchClicked = true;

            if (movementInputData.RunClicked)
                movementInputData.IsRunning = true;

            if(movementInputData.RunReleased)
                movementInputData.IsRunning = false;

            movementInputData.JumpClicked = Gamepad.current.buttonSouth.wasPressedThisFrame;
            movementInputData.CrouchClicked = Gamepad.current.buttonEast.wasPressedThisFrame;
            if (!isInVorgonRealm)
            {
                cameraInputData.IsPeakingLeft = Gamepad.current.leftShoulder.isPressed;
                cameraInputData.IsPeakingRight = Gamepad.current.rightShoulder.isPressed;
            } else
            {
                movementInputData.UseDefend = Gamepad.current.leftShoulder.wasPressedThisFrame;
                movementInputData.UseAttack = Gamepad.current.rightShoulder.wasPressedThisFrame;
                movementInputData.SwitchAttack = Gamepad.current.rightTrigger.wasPressedThisFrame;
            }

        }

        void GetInventoryInputData()
        {
            inventoryInputData.OpenSpellBook = Gamepad.current.selectButton.wasPressedThisFrame;
        }

        void GetItemInputData()
        {
            if (movementInputData.HasActiveItem)
                itemInputData.UseItemClick = Input.GetMouseButtonDown(0);
        }

        #endregion
    }
}