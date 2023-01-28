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

        if(angle <= 45.0f)
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
}
