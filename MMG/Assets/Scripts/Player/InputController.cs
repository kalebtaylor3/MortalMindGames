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
            interactionInputData.InteractedClicked = Input.GetKeyDown("joystick button 2");
            interactionInputData.InteractedReleased = Input.GetKeyUp("joystick button 2");
        }

        void GetCameraInput()
        {
            cameraInputData.InputVectorX = Gamepad.current.rightStick.x.ReadValue();
            cameraInputData.InputVectorY = Gamepad.current.rightStick.y.ReadValue();

            cameraInputData.ZoomClicked = Input.GetMouseButtonDown(1);
            cameraInputData.ZoomReleased = Input.GetMouseButtonUp(1);
        }

        void GetMovementInputData()
        {
            movementInputData.InputVectorX = Input.GetAxisRaw("Horizontal");
            movementInputData.InputVectorY = Input.GetAxisRaw("Vertical");

            movementInputData.RunClicked = Input.GetKeyDown("joystick button 8");
            movementInputData.RunReleased = Input.GetKeyUp("joystick button 8");

            if(movementInputData.RunClicked)
                movementInputData.IsRunning = true;

            if(movementInputData.RunReleased)
                movementInputData.IsRunning = false;

            movementInputData.JumpClicked = Gamepad.current.aButton.wasPressedThisFrame;
            movementInputData.CrouchClicked = Gamepad.current.bButton.wasPressedThisFrame;

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