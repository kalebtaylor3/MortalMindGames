using UnityEngine;

namespace MMG
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }

    [CreateAssetMenu(fileName = "PerlinNoiseData", menuName = "MMG_Player/Data/PerlinNoiseData", order = 2)]
    public class PerlinNoiseData : ScriptableObject
    {
        #region Variables
            public TransformTarget transformTarget;

            [Space]
            public float amplitude;
            public float frequency;

            public float initialAmplitude;
            public float initialFreequency;
        #endregion    


        private void OnEnable()
        {
            amplitude = initialAmplitude;
            frequency = initialFreequency;
        }

        private void Awake()
        {
            amplitude = initialAmplitude;
            frequency = initialFreequency;  
        }
    }
}