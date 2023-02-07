using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MAimState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;

    bool angleReached = false;


    public MAimState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Aim;
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
    }

    public override void EnterStateInit()
    {
        navAgent.isStopped = false;
        angleReached = false;
    }

    public override void Reason()
    {
        // Transitions

        if (angleReached && !minionController.isAttacking)
        {
            // Anlge reached - > Attack
            minionFSM.PerformTransition(Transition.ReachedPlayer);
        }
        else if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
        {
            // Out of range -> Patrol
            minionFSM.PerformTransition(Transition.PlayerLost);
        }
    }

    public override void Act()
    {
        // Actions

        if(navAgent.isStopped == false)
        {
            //navAgent.isStopped = true;
        }
        

        if (angleReached == false)
        {
            navAgent.isStopped = false;
            Vector3 direction = (playerT.position - minionController.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            minionController.transform.rotation = Quaternion.Slerp(minionController.transform.rotation, lookRotation, Time.deltaTime);

            float angle = Quaternion.Angle(minionController.transform.rotation, lookRotation);

            if (angle <= 1)
            {
                angleReached = true;
            }
        }
    }
}
