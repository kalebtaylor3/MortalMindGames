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


        // Stun time over 
        if (!vorgonControl.stunned)
        {
            // If player Found -> Chase
            if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position, VorgonDeadwoodFSM.CHASE_DIST))
            {
                vorgonFSM.PerformTransition(Transition.PlayerFound);
            }

            //-> Lost
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

