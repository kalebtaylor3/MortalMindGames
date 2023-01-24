using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    public StunnedState(VorgonController controller)
    {
        stateID = FSMStateID.Stunned;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
    }

    public override void Reason()
    {
        // Transitions


        // Stun time over -> Lost
        if (!vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.StunDone);
        }
    }

    public override void Act()
    {
        // Actions
        if (!vorgonControl.navAgent.isStopped)
        {
            vorgonControl.navAgent.isStopped = true;
        }
    }
}

