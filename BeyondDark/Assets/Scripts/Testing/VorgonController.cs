using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VorgonController : MonoBehaviour
{
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public Transform playerT;
    [SerializeField] public VorgonDeadwoodFSM vorgonFSM;

    private void Start()
    {
        

    }

    private void Update()
    {
        //navAgent.destination = playerT.position;        
    }
}
