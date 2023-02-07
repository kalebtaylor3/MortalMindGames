using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MChaseState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;



    public MChaseState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Chase;
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

        if (minionController.onFire)
        {
            // If on Fire -> Burning
            minionFSM.PerformTransition(Transition.OnFlames);
        }

        if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
        {
            // Out of range -> Patrol
            minionFSM.PerformTransition(Transition.PlayerLost);
        }

        if(IsInCurrentRange(minionController.transform, playerT.position, 2))
        {
            // Reached player -> Attack
            minionFSM.PerformTransition(Transition.ReachedPlayer);
        }


    }

    public override void Act()
    {
        // Actions
        if (navAgent.isStopped == true)
        {
            navAgent.isStopped = false;
        }

        navAgent.destination = playerT.position;

    }
}
