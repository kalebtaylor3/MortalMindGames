using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.3f;
    public float shakeAmplitude = 1.2f;
    public float shakeFrequency = 2.0f;

    private CinemachineVirtualCamera vcam;
    private float shakeElapsedTime = 0f;

    private static CameraShake instance;

    public static CameraShake Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Null");
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCamera(float amplitude, float frequency)
    {
        shakeAmplitude = amplitude;
        shakeFrequency = frequency;
        shakeElapsedTime = shakeDuration;
    }

    private void Update()
    {
        if (shakeElapsedTime > 0)
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmplitude;
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = shakeFrequency;

            shakeElapsedTime -= Time.deltaTime;
        }
        else
        {
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Mathf.Lerp(vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain, 0, shakeDuration / shakeElapsedTime * Time.deltaTime);
        }
    }
}