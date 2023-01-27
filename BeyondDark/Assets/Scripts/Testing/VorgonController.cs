using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class VorgonController : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public Transform playerT;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;
    [SerializeField] public bool stunned = false;
    [SerializeField] private float stunDuration;
    [SerializeField] public bool isAttacking = false;
    [SerializeField] public bool PlayerInSight = false;

    private Color rayColor = Color.green;

    private void Update()
    {
        //navAgent.destination = playerT.position;        

        Debug.DrawRay(transform.position, transform.forward, rayColor);
        LineOfSight();

        //if (LineOfSight())
        //{
        //    //Debug.Log("In Sight");
        //}
        //else
        //{
        //    //Debug.Log("Not in Sight");
        //}
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
        Vector3 dir = (playerT.position - transform.position).normalized;

        Debug.DrawRay(transform.position, dir, Color.yellow);

       Vector3 forwardV = transform.forward;
        float angle = Vector3.Angle(dir, forwardV);

        if(angle <= 45.0f)
        {
            rayColor = Color.red;
            PlayerInSight = true;                     
        }
        else
        {
            rayColor = Color.green;
            PlayerInSight = false;            
        }    
    }

}
