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
        navAgent.isStopped = false;
    }

    public override void Reason()
    {
        // Transitions
        if(minionController.onFire)
        {
            // If on Fire -> Burning
            minionFSM.PerformTransition(Transition.OnFlames);
        }

        // RANGED
        if(minionType == MinionController.MINION_TYPE.RANGED)
        {
            if(IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
            {
                // If on chase range -> Aim
                minionFSM.PerformTransition(Transition.PlayerDetected);
            }
        }
        //MELEE
        else if(minionType == MinionController.MINION_TYPE.MELEE)
        {
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
        if (navAgent.isStopped == false)
        {
            navAgent.isStopped = true;
        }
    }
}
