using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MPatrolState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;


    public MPatrolState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Patrol;
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
    }

    public override void EnterStateInit()
    {

    }

    public override void Reason()
    {
        // Transitions
        if(minionController.onFire)
        {
            minionFSM.PerformTransition(Transition.OnFlames);
        }

        // RANGED
        if(minionController.type == MinionController.MINION_TYPE.RANGED)
        {
            if(IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
            {
                minionFSM.PerformTransition(Transition.PlayerDetected);
            }
        }
        else if(minionController.type == MinionController.MINION_TYPE.MELEE)
        {
            if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
            {
                minionFSM.PerformTransition(Transition.PlayerFound);
            }
        }
    }

    public override void Act()
    {
        // Actions
        navAgent.isStopped = true;
    }
}
