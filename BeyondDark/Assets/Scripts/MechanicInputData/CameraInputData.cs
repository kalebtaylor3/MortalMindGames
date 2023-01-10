using UnityEngine;

namespace MMG
{
    
    [CreateAssetMenu(fileName = "CameraInputData", menuName = "MMG_Player/Data/CameraInputData", order = 0)]
    public class CameraInputData : ScriptableObject
    {
        #region Data
            Vector2 inputVector;
            bool isZooming;
            bool zoomClicked;
            bool zoomReleased;
            bool isPeakingRight;
            bool isPeakingLeft;
        #endregion

        #region Properties
        public Vector2 InputVector => inputVector;
            public float InputVectorX
            {
                set => inputVector.x = value;
                get => inputVector.x;   
            }

            public float InputVectorY
            {
                set => inputVector.y = value;
                get => InputVector.y;
            }

            public bool IsZooming
            {
                get => isZooming;
                set => isZooming = value;
            }

            public bool ZoomClicked
            {
                get => zoomClicked;
                set => zoomClicked = value;
            }

            public bool ZoomReleased
            {
                get => zoomReleased;
                set => zoomReleased = value;
            }

            public bool IsPeakingLeft
            {
                get => isPeakingLeft;
                set => isPeakingLeft = value;
            }

            public bool IsPeakingRight
            {
                get => isPeakingRight;
                set => isPeakingRight = value;
            }

        #endregion

        #region Functions
        public void ResetInput()
            {
                inputVector = Vector2.zero;
                isZooming = false;
                zoomClicked = false;
                zoomReleased = false;
            }
        #endregion
    }
}
