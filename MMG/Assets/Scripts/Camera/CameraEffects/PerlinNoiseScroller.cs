using UnityEngine;

namespace MMG
{    
    public class PerlinNoiseScroller
    {
        #region Variables
            PerlinNoiseData noiseData;

            Vector3 noiseOffset;
            Vector3 noiseVector;
        #endregion

        #region Properties
            public Vector3 Noise => noiseVector;
        #endregion

        #region Functions
            public PerlinNoiseScroller (PerlinNoiseData data)
            {
                noiseData = data;

                float rand = 32f;

                noiseOffset.x = Random.Range(0f,rand);
                noiseOffset.y = Random.Range(0f,rand);
                noiseOffset.z = Random.Range(0f,rand);
            }

            public void UpdateNoise()
            {
                float scrollOffset = Time.deltaTime * noiseData.frequency;

                noiseOffset.x += scrollOffset;
                noiseOffset.y += scrollOffset;
                noiseOffset.z += scrollOffset;

                noiseVector.x = Mathf.PerlinNoise(noiseOffset.x,0f);
                noiseVector.y = Mathf.PerlinNoise(noiseOffset.x,1f);
                noiseVector.z = Mathf.PerlinNoise(noiseOffset.x,2f);

                noiseVector -= Vector3.one * 0.5f;
                noiseVector *= noiseData.amplitude;
            }
        #endregion
    }
}
