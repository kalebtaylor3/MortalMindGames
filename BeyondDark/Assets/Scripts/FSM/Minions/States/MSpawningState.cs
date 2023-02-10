using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MSpawningState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;


    public MSpawningState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Spawning;
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
        if (!minionController.spawning)
        {
            minionFSM.PerformTransition(Transition.Spawned);
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
