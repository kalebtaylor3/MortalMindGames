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

    }

    public override void Act()
    {
        // Actions

    }
}
