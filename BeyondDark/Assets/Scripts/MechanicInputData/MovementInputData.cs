using UnityEngine;

namespace MMG
{    
    [CreateAssetMenu(fileName = "MovementInputData", menuName = "MMG_Player/Data/MovementInputData", order = 1)]
    public class MovementInputData : ScriptableObject
    {
        #region Data
            Vector2 inputVector;

            bool isRunning;
            bool isCrouching;

            bool crouchClicked;
            bool jumpClicked;

            bool runClicked;
            bool runReleased;
        //combat
        bool defendClicked;
        bool attackClicked;
        bool sAttackClicked;
        //not combat LR
        bool hasActiveItem;
        #endregion

        #region Properties
            public Vector2 InputVector => inputVector;
            public bool HasInput => inputVector != Vector2.zero;
            public float InputVectorX
            {
                set => inputVector.x = value;
            }

            public float InputVectorY
            {
                set => inputVector.y = value;
            }

            public bool IsRunning
            {
                get => isRunning;
                set => isRunning = value;
            }

            public bool IsCrouching
            {
                get => isCrouching;
                set => isCrouching = value;
            }

            public bool CrouchClicked
            {
                get => crouchClicked;
                set => crouchClicked = value;
            }

            public bool JumpClicked
            {
                get => jumpClicked;
                set => jumpClicked = value;
            }

            public bool RunClicked
            {
                get => runClicked;
                set => runClicked = value;
            }

            public bool RunReleased
            {
                get => runReleased;
                set => runReleased = value;
            }

            public bool HasActiveItem
            {
                get => hasActiveItem;
                set => hasActiveItem = value;
            }
        //Combat
        public bool UseDefend
        {
            get => defendClicked;
            set => defendClicked = value;
        }
        public bool UseAttack
        {
            get => attackClicked;
            set => attackClicked = value;
        }
        public bool SwitchAttack
        {
            get => sAttackClicked;
            set => sAttackClicked = value;
        }
        #endregion

        #region Functions
        public void ResetInput()
            {
                inputVector = Vector2.zero;
                
                isRunning = false;
                isCrouching = false;

                crouchClicked = false;
                jumpClicked = false;
                runClicked = false;
                runReleased =false;
            }
        #endregion
    }
}