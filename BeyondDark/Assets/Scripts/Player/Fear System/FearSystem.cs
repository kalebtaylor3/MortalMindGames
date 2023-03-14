using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FearSystem : MonoBehaviour
{

    public CameraBreathing breathing;

    float initialAmplitude;
    float initialFrequency;

    float hearingRange = 0.5f;

    public Transform vorgon;

    float minValueA;
    float maxValueA;
    float minValueF;
    float maxValueF;
    public float minDistance = 0;
    public float maxDistance = 10;

    private float currentAmplitute;
    private float currentFrequency;

    bool jumpScare = false;

    private void OnEnable()
    {

        JumpScare.OnJumpScare += OnJumpScare;

        initialAmplitude = breathing.initialAmplitude;
        initialFrequency = breathing.initialFrequency;


        minValueA = initialAmplitude;
        maxValueA = 3.5f;
        minValueF = initialFrequency;
        maxValueF = 1.5f;
    }

    private void OnDisable()
    {
        minValueA = initialAmplitude;
        maxValueA = 3.5f;
        minValueF = initialFrequency;
        maxValueF = 1.5f;
        JumpScare.OnJumpScare -= OnJumpScare;
    }


    void OnJumpScare()
    {
        StopCoroutine(ResetBreathing());
        jumpScare = true;
        breathing.noiseData.frequency = 1.5f;
        breathing.noiseData.amplitude = 3.5f;
        StartCoroutine(ResetBreathing());
    }

    IEnumerator ResetBreathing()
    {
        yield return new WaitForSeconds(4);
        breathing.noiseData.frequency = currentFrequency;
        breathing.noiseData.amplitude = currentAmplitute;
        jumpScare = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        minValueA = initialAmplitude;
        maxValueA = 3.5f;
        minValueF = initialFrequency;
        maxValueF = 1.5f;
    }

    private void Awake()
    {
        minValueA = initialAmplitude;
        maxValueA = 3.5f;
        minValueF = initialFrequency;
        maxValueF = 1.5f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!jumpScare)
        {
            float distance = Vector3.Distance(transform.position, vorgon.position);
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
            currentAmplitute = Mathf.Lerp(minValueA, maxValueA, 1 - (distance - minDistance) / (maxDistance - minDistance));
            currentFrequency = Mathf.Lerp(minValueF, maxValueF, 1 - (distance - minDistance) / (maxDistance - minDistance));

            breathing.noiseData.frequency = currentFrequency;
            breathing.noiseData.amplitude = currentAmplitute;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

}
