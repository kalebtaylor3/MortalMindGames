using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePatrolState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    public ClosePatrolState(VorgonController controller)
    {
        stateID = FSMStateID.ClosePatrol;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        vorgonControl.navAgent.isStopped = false;
    }

    public override void Reason()
    {
        // Transitions

        // If stunned -> Stun
        if (vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
        }

        // If player Found -> Chase
        if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position, VorgonDeadwoodFSM.CHASE_DIST))
        {
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }

    }

    public override void Act()
    {
        // Actions

    }
}
