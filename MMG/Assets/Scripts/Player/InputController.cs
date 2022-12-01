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

            movementInputData.JumpClicked = Input.GetKeyDown(KeyCode.Space);
            movementInputData.CrouchClicked = Input.GetKeyDown("joystick button 1");

            cameraInputData.IsPeakingLeft = Input.GetKey("joystick button 4");
            cameraInputData.IsPeakingRight = Input.GetKey("joystick button 5");
        }

        void GetInventoryInputData()
        {
            inventoryInputData.Change1Clicked = Input.GetKeyDown(KeyCode.Alpha1);
            inventoryInputData.Change2Clicked = Input.GetKeyDown(KeyCode.Alpha2);
            inventoryInputData.Change3Clicked = Input.GetKeyDown(KeyCode.Alpha3);
            inventoryInputData.Change4Clicked = Input.GetKeyDown(KeyCode.Alpha4);
            inventoryInputData.PutItemAway = Input.GetKeyDown(KeyCode.X);

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