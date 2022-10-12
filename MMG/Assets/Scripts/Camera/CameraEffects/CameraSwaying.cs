using UnityEngine;
using NaughtyAttributes;

namespace MMG
{    
    [System.Serializable]
    public class CameraSwaying
    {
        #region Variables
        [Space,Header("Sway Settings")]
        [SerializeField] private float swayAmount = 0f;
        [SerializeField] private float swaySpeed = 0f;
        [SerializeField] private float returnSpeed = 0f;
        [SerializeField] private float changeDirectionMultiplier = 0f;

        [SerializeField] private AnimationCurve swayCurve = new AnimationCurve();

        private Transform camTransform;
        private float scrollSpeed;

        private float xAmountThisFrame;
        private float xAmountPreviousFrame;
 
        private bool diffrentDirection;
        #endregion

        #region Functions
        public void Init(Transform cam)
        {
            camTransform = cam;
        }

        public void SwayPlayer(Vector3 inputVector, float rawXInput)
        {
            float _xAmount = inputVector.x;

            xAmountThisFrame = rawXInput;

            if(rawXInput != 0f) // if we have some input
            {
                if(xAmountThisFrame != xAmountPreviousFrame && xAmountPreviousFrame != 0) // if our previous dir is not equal to current one and the previous one was not idle
                    diffrentDirection = true;

                // then we multiplier our scroll so when changing direction it will sway to the other direction faster
                float _speedMultiplier = diffrentDirection ? changeDirectionMultiplier : 1f;
                scrollSpeed += (_xAmount * swaySpeed * Time.deltaTime * _speedMultiplier);
            }
            else // if we are not moving so there is no input
            {
                if(xAmountThisFrame == xAmountPreviousFrame) // check if our previous dir equals current dir
                    diffrentDirection = false; // if yes we want to reset this bool so basically it can be used correctly once we move again

                scrollSpeed = Mathf.Lerp(scrollSpeed,0f,Time.deltaTime * returnSpeed);
            }

            scrollSpeed = Mathf.Clamp(scrollSpeed,-1f,1f);
            //Debug.Log(_scrollSpeed);

            float _swayFinalAmount;

            if(scrollSpeed < 0f)
                _swayFinalAmount = -swayCurve.Evaluate(scrollSpeed) * -swayAmount;
            else
                _swayFinalAmount = swayCurve.Evaluate(scrollSpeed) * -swayAmount;
                
            Vector3 _swayVector;
            _swayVector.z = _swayFinalAmount;

            camTransform.localEulerAngles = new Vector3(camTransform.localEulerAngles.x,camTransform.localEulerAngles.y,_swayVector.z);

            xAmountPreviousFrame = xAmountThisFrame;
        }
        #endregion
    }
}
