using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    Vector3 destination;

    public SeekState(VorgonController controller)
    {
        stateID = FSMStateID.Seek;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        destination = WorldData.Instance.FindSectionCenter(WorldData.Instance.activePlayerSection);
    }

    public override void Reason()
    {
        // Check for section change
        if(WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            destination = WorldData.Instance.FindSectionCenter(WorldData.Instance.activePlayerSection);
        }

        // Transitions
        // Reach section -> Lost
        if (vorgonControl.transform.position == destination)
        {
            vorgonFSM.PerformTransition(Transition.ReachedSection);
        }
    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = destination;
    }
}
