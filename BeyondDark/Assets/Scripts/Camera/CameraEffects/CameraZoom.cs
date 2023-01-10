using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

namespace MMG
{    
    [System.Serializable]
    public class CameraZoom
    {
        #region Variables

        [Space,Header("Zoom Settings")]
        [Range(20f,60f)] [SerializeField] private float zoomFOV = 20f;
        [SerializeField] private AnimationCurve zoomCurve = new AnimationCurve();
        [SerializeField] private float zoomTransitionDuration = 0f;

        [Space,Header("Run Settings")]
        [Range(60f,100f)] [SerializeField] private float runFOV = 60f;
        [SerializeField] private AnimationCurve runCurve = new AnimationCurve();
        [SerializeField] private float runTransitionDuration = 0f;
        [SerializeField] private float runReturnTransitionDuration = 0f;

        private float m_initFOV;
        private CameraInputData camInputData;

        private bool running;
        private bool zooming;

        private CinemachineVirtualCamera cam;

        private IEnumerator _ChangeFOVRoutine;
        private IEnumerator _ChangeRunFOVRoutine;

        #endregion

        #region Functions
        public void Init(CinemachineVirtualCamera cam, CameraInputData data)
        {
            camInputData = data;

            this.cam = cam;
            m_initFOV = this.cam.m_Lens.FieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if(running)
            {
                camInputData.IsZooming = !camInputData.IsZooming;
                zooming = camInputData.IsZooming;
                return;
            }

            if(_ChangeRunFOVRoutine != null)
                mono.StopCoroutine(_ChangeRunFOVRoutine);

            if(_ChangeFOVRoutine != null)
                mono.StopCoroutine(_ChangeFOVRoutine);

            _ChangeFOVRoutine = ChangeFOVRoutine();
            mono.StartCoroutine(_ChangeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            float _percent = 0f;
            float _smoothPercent = 0f;

            float _speed = 1f / zoomTransitionDuration;

            float _currentFOV = cam.m_Lens.FieldOfView;
            float  _targetFOV = camInputData.IsZooming ? m_initFOV : zoomFOV;

            camInputData.IsZooming = !camInputData.IsZooming;
            zooming = camInputData.IsZooming;

            while(_percent < 1f)
            {
                _percent += Time.deltaTime * _speed;
                _smoothPercent = zoomCurve.Evaluate(_percent);
                cam.m_Lens.FieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, _smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool returning,MonoBehaviour mono)
        {
            if(_ChangeFOVRoutine != null)
                mono.StopCoroutine(_ChangeFOVRoutine);

            if(_ChangeRunFOVRoutine != null)
                mono.StopCoroutine(_ChangeRunFOVRoutine);

            _ChangeRunFOVRoutine = ChangeRunFOVRoutine(returning);
            mono.StartCoroutine(_ChangeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            float _percent = 0f;
            float _smoothPercent = 0f;

            float _duration = returning ? runReturnTransitionDuration : runTransitionDuration;
            float _speed = 1f / _duration;

            float _currentFOV = cam.m_Lens.FieldOfView;
            float  _targetFOV = returning ? m_initFOV : runFOV;

            running = !returning;

            while(_percent < 1f)
            {
                _percent += Time.deltaTime * _speed;
                _smoothPercent = runCurve.Evaluate(_percent);
                cam.m_Lens.FieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, _smoothPercent);
                yield return null;
            }
        }
        #endregion
    }
}