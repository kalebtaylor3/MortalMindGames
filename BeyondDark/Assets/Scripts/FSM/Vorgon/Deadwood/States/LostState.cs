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

        // If wrong section -> Seek
        if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }

    }

    public override void Act()
    {
        // Actions
        
    }

}
