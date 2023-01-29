using MMG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
using static UnityEditor.FilePathAttribute;

public class VorgonController : MonoBehaviour
{
    #region Variables
    // For Sound Effect && FSM
    public enum EVENT_TYPE { LOST = 0, ANIM, SOUND };    
    public enum SOUND_TYPE { ALERT = 0, GROWL };

    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public PlayerController playerT;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;
    [SerializeField] private float stunDuration;
    [SerializeField] AudioSource alertAudioSource;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    private Color rayColor = Color.green;

    private List<AudioClip>[] audioClips = new List<AudioClip>[10];
    
    [SerializeField] private List<AudioClip> ALERTSounds;
    [SerializeField] private List<AudioClip> GROWLSounds;
    #endregion

    #region Debug
    [HideInInspector] public bool stunned = false;
    [HideInInspector] public bool isAttacking = false;


    [HideInInspector] public bool PlayerInSight = false;
    [HideInInspector] public bool canSeePlayer;
    [HideInInspector] public bool playerDetected = false;
    [SerializeField] public bool awareOfPlayer = false;
    [HideInInspector] public bool inChase = false;

    [HideInInspector] public Vector3 LastSeen = Vector3.zero;
    [HideInInspector] public bool SearchAnimCanPlay = true;
    [HideInInspector] public bool SearchAnimIsPlaying = false;

    #endregion

    private void Start()
    {
        audioClips[0] = ALERTSounds;
        audioClips[1] = GROWLSounds;
    }

    private void Update()
    {
        //navAgent.destination = playerT.position;        

        Debug.DrawRay(transform.position, transform.forward, rayColor);
        LineOfSight();

    }

    public void StunVorgon()
    {
        StartCoroutine(TriggerStun());
    }

    IEnumerator TriggerStun()
    {
        stunned = true;
        yield return new WaitForSeconds(stunDuration);
        stunned = false;
    }

    public void Attack()
    {
        StartCoroutine(TriggerAttack());
    }

    IEnumerator TriggerAttack()
    {
        isAttacking = true;
        WorldData.Instance.PlayerDeathMortalRealm();
        yield return new WaitForSeconds(2);
        isAttacking = false;
    }

    public void LineOfSight()
    {
        Vector3 dir = (playerT.transform.position - transform.position).normalized;

        Debug.DrawRay(transform.position, dir, Color.yellow);

        Vector3 forwardV = transform.forward;
        float angle = Vector3.Angle(dir, forwardV);

        if(angle <= 45.0f && !playerT.isHiding)
        {
            float distanceToTarget = Vector3.Distance(transform.position, playerT.transform.position);

            if (!Physics.Raycast(transform.position, dir, distanceToTarget, obstructionMask))
            {
                canSeePlayer = true;
                PlayerInSight = true;
                rayColor = Color.red;
            }                
            else
            {
                canSeePlayer = false;
                PlayerInSight = false;
                rayColor = Color.green;                
            }             
        }
        else
        {
            canSeePlayer = false;
            rayColor = Color.green;
            PlayerInSight = false;            
        }
    }

    public void PlaySearchAnim()
    {
        StartCoroutine(TrigerSearchAnim());
    }

    IEnumerator TrigerSearchAnim()
    {
        SearchAnimIsPlaying = true;
        yield return new WaitUntil(() => !alertAudioSource.isPlaying);
        SearchAnimCanPlay = false;
        SearchAnimIsPlaying = false;
    }

    public void SetLastDetectedLocation(Vector3 location, EVENT_TYPE type = EVENT_TYPE.LOST)
    {
        if (type == EVENT_TYPE.LOST)
        {
            awareOfPlayer = false;            
        }
        else
        {
            StartCoroutine(TriggerLastSeen(location, type));
        }        
    }

    IEnumerator TriggerLastSeen(Vector3 location, EVENT_TYPE type)
    {
        LastSeen = location;
        if(!playerDetected)
        {
            playerDetected = true;

            if (type == EVENT_TYPE.ANIM)
            {
                SearchAnimIsPlaying = true;
                //navAgent.isStopped = true;
                awareOfPlayer = true;

                PlaySoundEffect(SOUND_TYPE.ALERT);

                yield return new WaitUntil(() => !alertAudioSource.isPlaying); // Include animation

                //SearchAnimCanPlay = false;
                SearchAnimIsPlaying = false;
                navAgent.isStopped = false;

            }
            else if (type == EVENT_TYPE.SOUND)
            {
                //audioSource.PlayOneShot();
                if (!alertAudioSource.isPlaying && !awareOfPlayer)
                {
                    awareOfPlayer = true;
                    PlaySoundEffect(SOUND_TYPE.GROWL);
                }
                yield return null;
            }
        }
        else
        {
            yield return null;
        }
    }

    public void PlaySoundEffect(SOUND_TYPE type)
    {
        int rand = Random.Range(0, ALERTSounds.Count - 1);
        alertAudioSource.PlayOneShot(audioClips[(int)type][rand]);
    }

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
