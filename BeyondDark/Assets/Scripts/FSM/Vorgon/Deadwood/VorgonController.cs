using MMG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;
//using static UnityEditor.FilePathAttribute;
using UnityEngine.UI;
using UnityEngine.ProBuilder.Shapes;

public class VorgonController : MonoBehaviour
{
    #region Variables
    // For Sound Effect && FSM
    public enum EVENT_TYPE { LOST = 0, ANIM, SOUND };    
    public enum SOUND_TYPE { ALERT = 0, GROWL };

    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public PlayerController playerT;
    [SerializeField] public GameObject PlayerKillCollision;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;
    [SerializeField] AudioSource alertAudioSource;
    [SerializeField] public float defaultSpeed;
    [SerializeField] public float chaseSpeed;

    public LayerMask targetMask;
    public LayerMask obstructionMask;
    private Color rayColor = Color.green;

    // For FSM
    public Vector3 concealPos = new Vector3(-1, -1, -1);
    public ConcelableAreaInteractable concealArea;

    // ANIMATIONS
    //public Animation vorgonAnimation;
    //public List<AnimationClip> animationsMR;

    [SerializeField] Animator vorgonAnimator;

    public Image detectionUI; // reference to the UI image on the canvas
    public CanvasGroup sightCanvas;
    public float detection;
    public float detectionSpeed = 0.4f;

    private List<AudioClip>[] audioClips = new List<AudioClip>[10];
    
    [SerializeField] private List<AudioClip> ALERTSounds;
    [SerializeField] private List<AudioClip> GROWLSounds;

    public float flashSpeed = 0.1f;

    bool flashing = false;
    bool happenOnce = false;
    public bool sawConceal = false;

    public float noticeRate;
    public float visionCone = 90.0f;

    #endregion

    #region Debug
    [HideInInspector] public bool stunned = false;
    [HideInInspector] public bool isAttacking = false;


    [SerializeField] public bool PlayerInSight = false;
    [HideInInspector] public bool canSeePlayer;
    [HideInInspector] public bool playerDetected = false;
    [SerializeField] public bool awareOfPlayer = false;
    [HideInInspector] public bool inChase = false;

    [HideInInspector] public Vector3 LastSeen = Vector3.zero;
    [HideInInspector] public bool SearchAnimCanPlay = true;
    [HideInInspector] public bool SearchAnimIsPlaying = false;

    [HideInInspector] public bool playerDead = false;

    public bool detectionOut = false;

    #endregion

    private void Awake()
    {
        audioClips[0] = ALERTSounds;
        audioClips[1] = GROWLSounds;
        happenOnce = false;
        canSeePlayer = false;
        PlayerInSight = false;
        playerDetected = false;
    }

    private void Start()
    {
        audioClips[0] = ALERTSounds;
        audioClips[1] = GROWLSounds;
    }


    private void OnEnable()
    {
        detectionOut = true;
        sawConceal = false;
        SearchAnimIsPlaying = false;
        SearchAnimCanPlay = true;

        if (WorldData.Instance != null && WorldData.Instance.activePlayerSection != WorldData.SECTIONS.NONE) 
        {
            transform.position = WorldData.Instance.FindActiveSection(WorldData.Instance.activePlayerSection).vorgonTP.position;
            //navAgent.isStopped = false;
        }
        
        //StealthDetection.Instance.detection = 0;
    }

    private void Update()
    {
        //navAgent.destination = playerT.position;        

        Debug.DrawRay(transform.position, transform.forward, rayColor);
        LineOfSight();

        if(detectionOut)
        {
            detection -= Time.deltaTime * detectionSpeed;
            if(detection <= 0)
            {
                detection = 0;
                detectionOut = false;
            }
        }

        if(playerT.isHiding && sawConceal)
        {
            ConcelableDetection.Instance.exposure = 1;
            ConcelableDetection.Instance.vorgonKnows = true;
        }

        vorgonAnimator.SetFloat("speed", navAgent.speed);
        
    }

    public void StunVorgon(float stunTime)
    {
        StartCoroutine(TriggerStun(stunTime));
    }

    IEnumerator TriggerStun(float stunTime)
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }

    public void Attack(bool hiding = false)
    {        
        StartCoroutine(TriggerAttack(hiding));
    }

    IEnumerator TriggerAttack(bool hiding)
    {
        isAttacking = true;
        WorldData.Instance.PlayerDeathMortalRealm();
        yield return new WaitForSeconds(2);
        isAttacking = false;
    }

    public void LineOfSight()
    {

        if (detection <= 0)
        {
            detection = 0;
            SetLastDetectedLocation(Vector3.zero, null, VorgonController.EVENT_TYPE.LOST);
        }

        if (detection >= 1)
        {

            if (playerT.isHiding)
            {
                SetLastDetectedLocation(WorldData.Instance.lastConceal.searchPos.position, WorldData.Instance.lastConceal, VorgonController.EVENT_TYPE.SOUND); /// HERE
            }

            detection = 1;
            if (!happenOnce)
            {
                if (!flashing)
                {
                    

                    StartCoroutine(Flash(1));
                    happenOnce = true;
                }
            }
            //detectionUI.color = Color.red;
        }
        else
        {
            detectionUI.color = Color.white;
        }

        if (!playerDead)
        {
            Vector3 dir = (playerT.transform.position - transform.position).normalized;

            Debug.DrawRay(transform.position, dir, Color.yellow);

            Vector3 forwardV = transform.forward;
            float angle = Vector3.Angle(dir, forwardV);

            if (angle <= visionCone && !playerT.isHiding)
            {
                float distanceToTarget = Vector3.Distance(transform.position, playerT.transform.position);

                if (!Physics.Raycast(transform.position, dir, distanceToTarget, obstructionMask))
                {
                    detection += Time.deltaTime / (distanceToTarget * noticeRate);
                    rayColor = Color.red;
                }
                else
                {
                    detection -= Time.deltaTime * detectionSpeed;
                    rayColor = Color.green;
                }
            }
            else
            {
                detection -= Time.deltaTime * detectionSpeed;
                rayColor = Color.green;
            }

            if (detection >= 1)
            {
                PlayerInSight = true;
                canSeePlayer = true;
            }
            else
            {
                happenOnce = false;
                canSeePlayer = false;
                PlayerInSight = false;
            }
        }

        sightCanvas.alpha = Mathf.Lerp(0, 1, detection);
        detectionUI.fillAmount = detection; // update the UI to match the detection level

    }

    public void PlaySearchAnim()
    {
        StartCoroutine(TrigerSearchAnim());
    }

    IEnumerator TrigerSearchAnim()
    {
        SearchAnimIsPlaying = true;
        if(!alertAudioSource.isPlaying)
        {
            yield return new WaitUntil(() => !alertAudioSource.isPlaying);
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
        }
        
        SearchAnimCanPlay = false;
        SearchAnimIsPlaying = false;
    }

    public void SetLastDetectedLocation(Vector3 location, ConcelableAreaInteractable conceal, EVENT_TYPE type = EVENT_TYPE.LOST)
    {
        if (type == EVENT_TYPE.LOST)
        {
            awareOfPlayer = false;
            //concealArea = null;
        }
        else
        {
            //concealArea = null;
            StartCoroutine(TriggerLastSeen(location, type));
        }

        if (conceal != null && conceal.searchPos.position != Vector3.zero)
        {
            concealPos = conceal.transform.position;
            concealArea = conceal;
        }

    }

    IEnumerator TriggerLastSeen(Vector3 location, EVENT_TYPE type)
    {
        LastSeen = location;
       
        if (!playerDetected)
        {
            playerDetected = true;

            if (type == EVENT_TYPE.ANIM)
            {
                SearchAnimIsPlaying = true;
                awareOfPlayer = true;
                //navAgent.isStopped = true;
                //awareOfPlayer = true;

                PlaySoundEffect(SOUND_TYPE.ALERT);

                yield return new WaitUntil(() => !alertAudioSource.isPlaying); // Include animation

                //SearchAnimCanPlay = false;
                SearchAnimIsPlaying = false;
                //navAgent.isStopped = false;                
            }
            else if (type == EVENT_TYPE.SOUND)
            {
                //awareOfPlayer = true;
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
        if(!alertAudioSource.isPlaying)
        {
            alertAudioSource.PlayOneShot(audioClips[(int)type][rand]);
        }        
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

    private IEnumerator Flash(float duration)
    {
        flashing = true;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            detectionUI.color = Color.red;
            yield return new WaitForSeconds(flashSpeed);
            detectionUI.color = Color.white;
            yield return new WaitForSeconds(flashSpeed);
            elapsedTime += flashSpeed * 2;
        }
        detectionUI.color = Color.red;
        flashing = false;
    }

}
