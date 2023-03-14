using UnityEngine;
using NaughtyAttributes;
using System.Collections;

namespace MMG
{    
    public class CameraBreathing : MonoBehaviour
    {
        #region Variables
        [Space,Header("Data")]
        [SerializeField] public PerlinNoiseData noiseData = null;



        [Space,Header("Axis")]
        [SerializeField] private bool x = true;
        [SerializeField] private bool y = true;
        [SerializeField] private bool z = true;
        [SerializeField] GameObject vorgonFear;
        [SerializeField] GameObject playersFear;
        private PerlinNoiseScroller perlinNoiseScroller;
        [HideInInspector] public Vector3 finalRot;
        private Vector3 finalPos;

        public float initialFrequency;
        public float initialAmplitude;


        private float fearFrequency;
        private float fearAmplitude;
        private Vector3 vorgonPos;
        private Vector3 fearPos;
        #endregion



        #region Functions

        void Start()
        {
            //if(playersFear == null)
            //{
            //    playersFear = GameObject.FindGameObjectWithTag("Player");

            //}
            //if(vorgonFear == null)
            //{
            //    vorgonFear = GameObject.FindGameObjectWithTag("Vorgon");
            //}
            perlinNoiseScroller = new PerlinNoiseScroller(noiseData);

            initialFrequency = noiseData.frequency;
            initialAmplitude = noiseData.amplitude;

            SetBreathingDefault();
            
        }

        private void OnEnable()
        {
            SetBreathingDefault();

            //perlinNoiseScroller = new PerlinNoiseScroller(noiseData);  // ahhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh

            PlayerController.OnEnmptyStamina += HandleEmptyStamina;
            PlayerController.OnTeleport += SetBreathingDefault;
        }

        private void OnDisable()
        {
            PlayerController.OnEnmptyStamina -= HandleEmptyStamina;
            PlayerController.OnTeleport -= SetBreathingDefault;
            StopAllCoroutines();
        }

        void SetBreathingDefault()
        {
            StopCoroutine(EmptyStaminaBreathing());

            noiseData.frequency = initialFrequency;
            noiseData.amplitude = initialAmplitude;
        }

        void HandleEmptyStamina()
        {
            //Debug.Log("No stamina");
            StartCoroutine(EmptyStaminaBreathing());
        }

        IEnumerator EmptyStaminaBreathing()
        {

            noiseData.amplitude = 3.5f;
            noiseData.frequency = 1;

            yield return new WaitForSeconds(7.0f);

            noiseData.frequency = initialFrequency;
            noiseData.amplitude = initialAmplitude;

            yield return null;
        }
        IEnumerator VorgonBreathing()
        {

            yield return null;
            
            
            
        }
        private void Update()
        {
            
            //StartCoroutine(VorgonBreathing());
            //vorgonPos = vorgonFear.transform.position;
            //fearPos = playersFear.transform.position;
            //fearAmplitude = Vector3.Distance(vorgonPos, fearPos);
            //if (fearAmplitude < 15)
            //{
            //    fearAmplitude = 25 - fearAmplitude;
            //    //fearAmplitude = fearAmplitude / 10;
            //}
            //else
            //{
            //    fearAmplitude = 0;
            //}
            
            //Debug.Log("Wild Ride" + fearAmplitude);
            //yield return new WaitForSeconds(5.0f);
        }
        void LateUpdate()
        {
            if (noiseData != null)
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
                            //posOffset.x += fearAmplitude;

                        if(y)
                            posOffset.y += perlinNoiseScroller.Noise.y;
                            //posOffset.y += fearAmplitude;

                            if (z)
                            posOffset.z += perlinNoiseScroller.Noise.z;
                            //posOffset.z += fearAmplitude;

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
                            //rotOffset.y += fearAmplitude;

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

                            transform.localPosition = finalPos;// + new Vector3(fearAmplitude, fearAmplitude, fearAmplitude);
                        transform.localEulerAngles = finalRot;

                        break;
                    }
                }

            }   
        }   
        #endregion
    }
}
