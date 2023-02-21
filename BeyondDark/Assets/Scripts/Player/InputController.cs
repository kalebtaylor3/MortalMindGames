using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

namespace MMG
{    
    public class InputController : MonoBehaviour
    {
        #region Data

        [SerializeField] public bool isVorgonCharacter = false;
        
            [Space,Header("Input Data")]
            [SerializeField] public CameraInputData cameraInputData = null;
            [SerializeField] private MovementInputData movementInputData = null;
            [SerializeField] private InteractionInputData interactionInputData = null;
            [SerializeField] private InventoryInputData inventoryInputData = null;
            [SerializeField] private ItemInputData itemInputData = null;
            [SerializeField] private QuickTimeEventInputData quickTimeInputData = null;
            [SerializeField] private CombatInputData combatInputData = null;

            [HideInInspector] public bool canMove = true;
            [HideInInspector] public bool canInteract = true;

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
            //if(!WorldData.Instance.gamePaused)
            {
                GetCameraInput();

                if (canMove)
                    GetMovementInputData();
                else
                {
                    movementInputData.InputVectorY = 0;
                    movementInputData.InputVectorX = 0;
                }

                if (canInteract)
                    GetInteractionInputData();
                GetInventoryInputData();
                GetItemInputData();
                GetQuickTimeEventInputData();

                if (isVorgonCharacter)
                    GetCombatInput();
            }
        }

        void GetQuickTimeEventInputData()
        {
            quickTimeInputData.SuccessKeyPressed = Gamepad.current.buttonWest.wasPressedThisFrame;
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

            if (!inventoryInputData.OpenSpellBook && !isVorgonCharacter)
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
            

            if(!isVorgonCharacter)
            {
                movementInputData.CrouchClicked = Gamepad.current.buttonEast.wasPressedThisFrame;
                cameraInputData.IsPeakingLeft = Gamepad.current.leftShoulder.isPressed;
                cameraInputData.IsPeakingRight = Gamepad.current.rightShoulder.isPressed;
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


        void GetCombatInput()
        {
            combatInputData.DashFlag = Gamepad.current.buttonEast.wasPressedThisFrame;


            if (combatInputData.IsThirdRelic)
            {
                if (combatInputData.CanCastFire)
                    combatInputData.CastFire = Gamepad.current.leftTrigger.isPressed;

                combatInputData.SwitchAbility = Gamepad.current.leftShoulder.wasPressedThisFrame;
                combatInputData.SwingSword = Gamepad.current.rightTrigger.wasPressedThisFrame;
            }
            else
            {
                if (combatInputData.CanCastFire)
                    combatInputData.CastFire = Gamepad.current.rightTrigger.isPressed;
            }

            combatInputData.StartWallPlace = Gamepad.current.leftTrigger.wasPressedThisFrame;

            if (combatInputData.CanCreateWall)
                combatInputData.CreateWall = Gamepad.current.leftTrigger.wasPressedThisFrame;

            combatInputData.CancelWall = Gamepad.current.buttonNorth.wasPressedThisFrame;

        }

        #endregion
    }
}