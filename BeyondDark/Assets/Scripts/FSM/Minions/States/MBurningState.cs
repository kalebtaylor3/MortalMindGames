using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MBurningState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;


    public MBurningState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Burning;
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
    }

    public override void EnterStateInit()
    {
        navAgent.isStopped = false;
    }

    public override void Reason()
    {
        // Transitions
        if(!minionController.onFire)
        {
            if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
            {
                // Out of range -> Patrol
                minionFSM.PerformTransition(Transition.PlayerLost);
            }

            if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
            {
                // If on chase range -> Chase
                minionFSM.PerformTransition(Transition.PlayerFound);
            }
        }
    }

    public override void Act()
    {
        // Actions

        if (navAgent.remainingDistance <= navAgent.stoppingDistance) //done with path
        {
            Vector3 randSearchPoint;
            if (minionController.RandomPoint(minionController.transform.position, 2, out randSearchPoint)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(randSearchPoint, Vector3.up, Color.magenta, 1.0f); //so you can see with gizmos
                navAgent.destination = randSearchPoint;
            }
        }

    }
}
