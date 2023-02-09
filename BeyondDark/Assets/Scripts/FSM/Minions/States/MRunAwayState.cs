using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MRunAwayState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;


    public MRunAwayState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.RunAway;
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
        if(minionController.safe)
        {
            // RANGED
            if (minionType == MinionController.MINION_TYPE.RANGED)
            {
                if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
                {
                    // If on chase range -> Aim
                    minionFSM.PerformTransition(Transition.PlayerDetected);
                }

                if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
                {
                    // Out of range -> Patrol
                    minionFSM.PerformTransition(Transition.PlayerLost);
                }
            }
            //MELEE
            else if (minionType == MinionController.MINION_TYPE.MELEE)
            {
                if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
                {
                    // If on chase range -> Chase
                    minionFSM.PerformTransition(Transition.PlayerFound);
                }

                if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
                {
                    // Out of range -> Patrol
                    minionFSM.PerformTransition(Transition.PlayerLost);
                }
            }
        }
    }

    public override void Act()
    {
        // Actions
        if (navAgent.isStopped == false)
        {
            //navAgent.isStopped = true;
        }

        Vector3 direction = minionController.transform.position - playerT.position;
        Vector3 newPos = minionController.transform.position + direction;

        Debug.DrawRay(newPos, Vector3.up, Color.magenta, 1.0f); //so you can see with gizmos

        navAgent.SetDestination(newPos);
    }
}