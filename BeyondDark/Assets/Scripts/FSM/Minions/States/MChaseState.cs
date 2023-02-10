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

    int availableSlot = -1;
    SlotManager playerSM = null;

    public MChaseState(MinionController controller, Transform player, NavMeshAgent agent, SlotManager playerSlotManager)
    {
        stateID = FSMStateID.Chase;
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
    }

    public override void Reason()
    {
        // Transitions
        playerSM.ClearSlots(minionController.gameObject);
        availableSlot = playerSM.ReserveSlotAroundObject(minionController.gameObject);

        if (availableSlot != -1)
        {
            destPos = playerSM.GetSlotPosition(availableSlot);
        }
        else
        {
            navAgent.isStopped = true;
            //destPos = Vector3.zero;
        }

        if (minionController.minionDeath)
        {
            // Minion Death
            minionFSM.PerformTransition(Transition.MinionDeath);
        }
        else
        {
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

            if (IsInCurrentRange(minionController.transform, destPos, 1))
            {
                // Reached player -> Attack
                minionFSM.PerformTransition(Transition.ReachedPlayer);
            }
        }
    }

    public override void Act()
    {
        // Actions
        if (navAgent.isStopped == true && availableSlot != -1)
        {
            navAgent.isStopped = false;
        }

        navAgent.destination = destPos;

    }
}
