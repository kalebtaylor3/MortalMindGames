using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static MinionController;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MAttackState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;

    bool attacked;

    int availableSlot = -1;
    SlotManager playerSM = null;

    public MAttackState(MinionController controller, Transform player, NavMeshAgent agent, SlotManager playerSlotManager)
    {
        stateID = FSMStateID.Attack;
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
        playerSM = playerSlotManager;
    }

    public override void EnterStateInit()
    {
        navAgent.isStopped = false;
        attacked = false;
    }

    public override void Reason()
    {
        // Transitions

        if (minionController.foundWall)
        {
            // If found wall -> Attack Wall
            minionFSM.PerformTransition(Transition.FoundWall);
        }

        if (!minionController.safe)
        {
            // If unsafe -> Run Away
            minionFSM.PerformTransition(Transition.Unsafe);
        }

        // RANGED
        if (minionType == MinionController.MINION_TYPE.RANGED)
        {
            if (attacked && !minionController.isAttacking)
            {
                // After attack - > Aim
                minionFSM.PerformTransition(Transition.PlayerDetected);
            }

            if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
            {
                // Out of range -> Patrol
                minionFSM.PerformTransition(Transition.PlayerLost);
            }

        }
        // MELEE
        else if (minionType == MinionController.MINION_TYPE.MELEE)
        {

            if (minionController.onFire)
            {
                // If on Fire -> Burning
                minionFSM.PerformTransition(Transition.OnFlames);
            }

            if (attacked && IsInCurrentRange(minionController.transform, playerT.position, 2))
            {
                // If in attack tange -> Attack Again
                attacked = false;
            }

            if (!IsInCurrentRange(minionController.transform, playerT.position, 2))
            {
                // If out of attack tange -> Chase
                minionFSM.PerformTransition(Transition.PlayerFound);
            }

            if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
            {
                // Out of range -> Patrol
                minionFSM.PerformTransition(Transition.PlayerLost);
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

        // RANGED
        if (minionType == MinionController.MINION_TYPE.RANGED && !attacked)
        {
            // Call shoot projectile
            if(!minionController.isAttacking)
            {
                minionController.RangedAttack();
                attacked = true;
            }            
        }  
        else if (minionType == MinionController.MINION_TYPE.MELEE && !attacked) // MELEE
        {
            // Call Attack (check range)

            attacked = true;
        }

    }
}
