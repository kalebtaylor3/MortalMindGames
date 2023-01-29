using MMG;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class VorgonController : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public PlayerController playerT;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;
    [SerializeField] public bool stunned = false;
    [SerializeField] private float stunDuration;
    [SerializeField] public bool isAttacking = false;

    [SerializeField] public bool PlayerInSight = false;
    [SerializeField] public bool canSeePlayer;
    public bool playerDetected = false;
    public bool inChase = false;

    public Vector3 LastSeen = Vector3.zero;
    [HideInInspector] public bool SearchAnimCanPlay = true;
    [HideInInspector] public bool SearchAnimIsPlaying = false;

    public LayerMask targetMask;
    public LayerMask obstructionMask;


    private Color rayColor = Color.green;

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
        yield return new WaitForSeconds(1.5f);
        SearchAnimCanPlay = false;
        SearchAnimIsPlaying = false;
    }

    public void SetLastDetectedLocation(Vector3 location)
    {
        LastSeen = location;
        playerDetected = true;
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
