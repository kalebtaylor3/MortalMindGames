using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ChaseState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    public ChaseState(VorgonController controller)
    {
        stateID = FSMStateID.Chase;
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

        if(IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position,2))
        {
            vorgonFSM.PerformTransition(Transition.ReachedPlayer);
        }

    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = vorgonControl.playerT.position;
    }
}
