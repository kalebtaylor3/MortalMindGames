using UnityEngine;

namespace  MMG
{    
    public class HeadBob
    {
        #region Variables
            HeadBobData headBobData;

            float xScrollAmount;
            float yScrollAmount;

            bool reseated;
            Vector3 finalOffset;
            float currentStaeHeight = 0f;
        #endregion

        #region Properties
            public Vector3 FinalOffset => finalOffset;
            public bool Resetted => reseated;
            public float CurrentStateHeight
            {
                get => currentStaeHeight;
                set => currentStaeHeight = value;
            }
        #endregion

        #region Functions
        public HeadBob(HeadBobData data,float moveBackwardsMultiplier,float moveSideMultiplier)
        {
            headBobData = data;

            headBobData.MoveBackwardsFrequencyMultiplier = moveBackwardsMultiplier;
            headBobData.MoveSideFrequencyMultiplier = moveSideMultiplier;

            xScrollAmount = 0f;
            yScrollAmount = 0f;

            reseated = false;
            finalOffset = Vector3.zero;
        }

        public void ScrollHeadBob(bool running, bool crouching, Vector2 input)
        {
            reseated = false;

            float amplitudeMultiplier;
            float frequencyMultiplier;
            float additionalMultiplier; // when moving backwards or to sides

            amplitudeMultiplier = running ? headBobData.runAmplitudeMultiplier : 1f;
            amplitudeMultiplier = crouching ? headBobData.crouchAmplitudeMultiplier : amplitudeMultiplier;

            frequencyMultiplier = running ? headBobData.runFrequencyMultiplier : 1f;
            frequencyMultiplier = crouching ? headBobData.crouchFrequencyMultiplier : frequencyMultiplier;

            additionalMultiplier = input.y == -1 ? headBobData.MoveBackwardsFrequencyMultiplier : 1f;
            additionalMultiplier = input.x != 0 & input.y == 0 ? headBobData.MoveSideFrequencyMultiplier : additionalMultiplier;


            xScrollAmount += Time.deltaTime * headBobData.xFrequency * frequencyMultiplier ; // you can also multiply this by _additionalMultiplier but it looks unnatural a bit;
            yScrollAmount += Time.deltaTime * headBobData.yFrequency * frequencyMultiplier ;

            float xValue;
            float yValue;

            xValue = headBobData.xCurve.Evaluate(xScrollAmount);
            yValue = headBobData.yCurve.Evaluate(yScrollAmount);

            finalOffset.x = xValue * headBobData.xAmplitude * amplitudeMultiplier * additionalMultiplier;
            finalOffset.y = yValue * headBobData.yAmplitude * amplitudeMultiplier * additionalMultiplier;
        }

        public void ResetHeadBob()
        {
            reseated = true;

            xScrollAmount = 0f;
            yScrollAmount = 0f;

            finalOffset = Vector3.zero;
        }
        #endregion
    }
}
