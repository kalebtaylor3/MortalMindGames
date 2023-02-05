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
    }

    public override void Reason()
    {
        // Transitions

    }

    public override void Act()
    {
        // Actions

    }
}
