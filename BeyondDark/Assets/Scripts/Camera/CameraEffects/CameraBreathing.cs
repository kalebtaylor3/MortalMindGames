using UnityEngine;
using NaughtyAttributes;
using System.Collections;

namespace MMG
{    
    public class CameraBreathing : MonoBehaviour
    {
        #region Variables
        [Space,Header("Data")]
        [SerializeField] private PerlinNoiseData noiseData = null;

        [Space,Header("Axis")]
        [SerializeField] private bool x = true;
        [SerializeField] private bool y = true;
        [SerializeField] private bool z = true;

        private PerlinNoiseScroller perlinNoiseScroller;
        [HideInInspector] public Vector3 finalRot;
        private Vector3 finalPos;

        private float initialFrequency;
        private float initialAmplitude;
        #endregion

        #region Functions

        void Start()
        {
            perlinNoiseScroller = new PerlinNoiseScroller(noiseData);
        }

        private void OnEnable()
        {
            PlayerController.OnEnmptyStamina += HandleEmptyStamina;
        }

        private void OnDisable()
        {
            PlayerController.OnEnmptyStamina -= HandleEmptyStamina;
            StopAllCoroutines();
        }

        void HandleEmptyStamina()
        {
            Debug.Log("No stamina");
            StartCoroutine(EmptyStaminaBreathing());
        }

        IEnumerator EmptyStaminaBreathing()
        {
            //initialFrequency = noiseData.frequency;
            //initialAmplitude = noiseData.amplitude;

            //noiseData.frequency = 50;
            //noiseData.amplitude = 50;

            //yield return new WaitForSeconds(2.0f);

            //noiseData.frequency = initialFrequency;
            //noiseData.amplitude = initialAmplitude;

            yield return null;
        }

        void LateUpdate()
        {
            if(noiseData != null)
            {
                perlinNoiseScroller.UpdateNoise();

                Vector3 posOffset = Vector3.zero;
                Vector3 rotOffset = Vector3.zero;

                switch (noiseData.transformTarget)
                {
                    case TransformTarget.Position:
                    {
                        if(x)
                            posOffset.x += perlinNoiseScroller.Noise.x;

                        if(y)
                            posOffset.y += perlinNoiseScroller.Noise.y;

                        if(z)
                            posOffset.z += perlinNoiseScroller.Noise.z;

                        finalPos.x = x ? posOffset.x : transform.localPosition.x;
                        finalPos.y = y ? posOffset.y : transform.localPosition.y;
                        finalPos.z = z ? posOffset.z : transform.localPosition.z;

                        transform.localPosition = finalPos;
                        break;
                    }
                    case TransformTarget.Rotation:
                    {
                        if(x)
                            rotOffset.x += perlinNoiseScroller.Noise.x;

                        if(y)
                            rotOffset.y += perlinNoiseScroller.Noise.y;

                        if(z)
                            rotOffset.z += perlinNoiseScroller.Noise.z;

                        finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                        finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                        finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;
                        
                        transform.localEulerAngles = finalRot;

                        break;
                    }
                    case TransformTarget.Both:
                    {
                        if(x)
                        {
                            posOffset.x += perlinNoiseScroller.Noise.x;
                            rotOffset.x += perlinNoiseScroller.Noise.x;
                        }

                        if(y)
                        {
                            posOffset.y += perlinNoiseScroller.Noise.y;
                            rotOffset.y += perlinNoiseScroller.Noise.y;
                        }

                        if(z)
                        {
                            posOffset.z += perlinNoiseScroller.Noise.z;
                            rotOffset.z += perlinNoiseScroller.Noise.z;
                        }

                        finalPos.x = x ? posOffset.x : transform.localPosition.x;
                        finalPos.y = y ? posOffset.y : transform.localPosition.y;
                        finalPos.z = z ? posOffset.z : transform.localPosition.z;

                        finalRot.x = x ? rotOffset.x : transform.localEulerAngles.x;
                        finalRot.y = y ? rotOffset.y : transform.localEulerAngles.y;
                        finalRot.z = z ? rotOffset.z : transform.localEulerAngles.z;

                        transform.localPosition = finalPos;
                        transform.localEulerAngles = finalRot;

                        break;
                    }
                }

            }   
        }   
        #endregion
    }
}
