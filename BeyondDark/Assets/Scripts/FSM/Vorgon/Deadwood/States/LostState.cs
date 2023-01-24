using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    public LostState(VorgonController controller)
    {
        stateID = FSMStateID.Lost;
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

        // If stunned -> Stun
        if(vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
        }

        // If wrong section -> Seek
        if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            vorgonFSM.PerformTransition(Transition.WrongSection);
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
        if (!vorgonControl.navAgent.isStopped)
        {
            vorgonControl.navAgent.isStopped = true;
        }
    }

}
