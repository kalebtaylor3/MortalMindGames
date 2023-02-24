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
        vorgonControl.navAgent.speed = vorgonControl.defaultSpeed;
    }

    public override void Reason()
    {
        // Transitions


        // Stun time over 
        if (!vorgonControl.stunned)
        {
            
            if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight)
            {
                // If player Found -> Chase
                vorgonFSM.PerformTransition(Transition.PlayerFound);
            }
            else
            {
                //-> Lost
                vorgonFSM.PerformTransition(Transition.StunDone);
            }
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

