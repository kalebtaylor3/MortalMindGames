using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VorgonController : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public Transform playerT;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;
    [SerializeField] public bool stunned = false;
    [SerializeField] private float stunDuration;



    private void Start()
    {
        

    }

    private void Update()
    {
        //navAgent.destination = playerT.position;        
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
}
