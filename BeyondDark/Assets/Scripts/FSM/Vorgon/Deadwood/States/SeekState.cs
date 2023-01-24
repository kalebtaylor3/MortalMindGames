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
        vorgonControl.navAgent.isStopped = false;
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

        // If stunned -> Stun
        if (vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
        }

        // Reach section -> Lost
        if (vorgonControl.transform.position == destination)
        {
            vorgonFSM.PerformTransition(Transition.ReachedSection);
        }

        // If player Found -> Chase
        if(IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position, VorgonDeadwoodFSM.CHASE_DIST))
        {
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }
    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = destination;
    }
}
