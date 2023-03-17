using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MDeadState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;


    public MDeadState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Dead;
        
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
    }

    public override void EnterStateInit()
    {
        navAgent.isStopped = false;
        minionType = minionController.type;
    }

    public override void Reason()
    {
        // Transitions
        
    }

    public override void Act()
    {
        // Actions
        if (navAgent.isStopped == false)
        {
            navAgent.isStopped = true;
            minionFSM.enabled = false;
        }
    }
}