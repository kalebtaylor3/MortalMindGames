using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VorgonController : MonoBehaviour
{
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform playerT;

    private void Start()
    {
        

    }

    private void Update()
    {
        navAgent.destination = playerT.position;
    }
}
