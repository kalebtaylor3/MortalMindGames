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
