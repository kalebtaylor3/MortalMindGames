using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VorgonCinematic : MonoBehaviour
{
    public List<Transform> movePoints;

    public NavMeshAgent navAgent;

    public Animator animator;

    int currentPoint;

    Vector3 destination;

    

    private void OnEnable()
    {
        currentPoint = -1;
    }

    private void Update()
    {
        animator.SetFloat("speed", navAgent.speed);
        MoveToPoints();

        if(navAgent.isStopped)
        {
            this.gameObject.SetActive(false);
        }
    }

    public void MoveToPoints()
    {
        if (navAgent.remainingDistance <= navAgent.stoppingDistance && !navAgent.isStopped) //done with path
        {
            currentPoint++;

            if (currentPoint >= movePoints.Count)
            {
                navAgent.isStopped = true;
            }
            else
            {
                destination = movePoints[currentPoint].position;
                navAgent.destination = destination;
            }            
        }
    }
}
