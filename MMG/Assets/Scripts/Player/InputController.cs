using UnityEngine;
using NaughtyAttributes;

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
            interactionInputData.InteractedClicked = Input.GetKeyDown(KeyCode.F);
            interactionInputData.InteractedReleased = Input.GetKeyUp(KeyCode.F);
        }

        void GetCameraInput()
        {
            cameraInputData.InputVectorX = Input.GetAxis("Mouse X");
            cameraInputData.InputVectorY = Input.GetAxis("Mouse Y");

            cameraInputData.ZoomClicked = Input.GetMouseButtonDown(1);
            cameraInputData.ZoomReleased = Input.GetMouseButtonUp(1);
        }

        void GetMovementInputData()
        {
            movementInputData.InputVectorX = Input.GetAxisRaw("Horizontal");
            movementInputData.InputVectorY = Input.GetAxisRaw("Vertical");

            movementInputData.RunClicked = Input.GetKeyDown(KeyCode.LeftShift);
            movementInputData.RunReleased = Input.GetKeyUp(KeyCode.LeftShift);

            if(movementInputData.RunClicked)
                movementInputData.IsRunning = true;

            if(movementInputData.RunReleased)
                movementInputData.IsRunning = false;

            movementInputData.JumpClicked = Input.GetKeyDown(KeyCode.Space);
            movementInputData.CrouchClicked = Input.GetKeyDown(KeyCode.C);

            cameraInputData.IsPeakingLeft = Input.GetKey(KeyCode.Q);
            cameraInputData.IsPeakingRight = Input.GetKey(KeyCode.E);
        }

        void GetInventoryInputData()
        {
            inventoryInputData.Change1Clicked = Input.GetKeyDown(KeyCode.Alpha1);
            inventoryInputData.Change2Clicked = Input.GetKeyDown(KeyCode.Alpha2);
            inventoryInputData.Change3Clicked = Input.GetKeyDown(KeyCode.Alpha3);
            inventoryInputData.Change4Clicked = Input.GetKeyDown(KeyCode.Alpha4);
            inventoryInputData.PutItemAway = Input.GetKeyDown(KeyCode.X);
        }

        void GetItemInputData()
        {
            if (movementInputData.HasActiveItem)
                itemInputData.UseItemClick = Input.GetMouseButtonDown(0);
        }

        #endregion
    }
}