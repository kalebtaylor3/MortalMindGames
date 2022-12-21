using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace MMG
{    
    public class InputController : MonoBehaviour
    {
        #region Data
            [Space,Header("Input Data")]
            [SerializeField] private CameraInputData cameraInputData = null;
            [SerializeField] private MovementInputData movementInputData = null;
            [SerializeField] private InteractionInputData interactionInputData = null;
            [SerializeField] private InventoryInputData inventoryInputData = null;
            [SerializeField] private ItemInputData itemInputData = null;
        #endregion

        #region Functions
        void Start()
            {
                cameraInputData.ResetInput();
                movementInputData.ResetInput();
                interactionInputData.ResetInput();
            }

        void Update()
        {
            GetCameraInput();
            GetMovementInputData();
            GetInteractionInputData();
            GetInventoryInputData();
            GetItemInputData();
        }
        void GetInteractionInputData()
        {
            interactionInputData.InteractedClicked = Gamepad.current.xButton.wasPressedThisFrame;
            interactionInputData.InteractedReleased = Gamepad.current.xButton.wasReleasedThisFrame;
        }

        void GetCameraInput()
        {
            cameraInputData.InputVectorX = Gamepad.current.rightStick.x.ReadValue();
            cameraInputData.InputVectorY = Gamepad.current.rightStick.y.ReadValue();

            cameraInputData.ZoomClicked = Gamepad.current.rightTrigger.isPressed;
            cameraInputData.ZoomReleased = Gamepad.current.rightTrigger.isPressed;
        }

        void GetMovementInputData()
        {
            movementInputData.InputVectorX = Gamepad.current.leftStick.x.ReadValue();
            movementInputData.InputVectorY = Gamepad.current.leftStick.y.ReadValue();

            movementInputData.RunClicked = Gamepad.current.leftStickButton.wasPressedThisFrame;
            movementInputData.RunReleased = Gamepad.current.leftStickButton.wasReleasedThisFrame;

            if(movementInputData.RunClicked)
                movementInputData.IsRunning = true;

            if(movementInputData.RunReleased)
                movementInputData.IsRunning = false;

            movementInputData.JumpClicked = Gamepad.current.aButton.wasPressedThisFrame;
            movementInputData.CrouchClicked = Gamepad.current.bButton.wasReleasedThisFrame;

            cameraInputData.IsPeakingLeft = Gamepad.current.leftShoulder.isPressed;
            cameraInputData.IsPeakingRight = Gamepad.current.rightShoulder.isPressed;
        }

        void GetInventoryInputData()
        {
            inventoryInputData.OpenSpellBook = Input.GetKeyDown("joystick button 6");
        }

        void GetItemInputData()
        {
            if (movementInputData.HasActiveItem)
                itemInputData.UseItemClick = Input.GetMouseButtonDown(0);
        }

        #endregion
    }
}