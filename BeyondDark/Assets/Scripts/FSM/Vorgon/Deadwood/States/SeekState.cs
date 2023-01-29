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

        
        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonFSM.PerformTransition(Transition.Stunned);
        }
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight)
        {
            // If player Found -> Chase
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }
        else if (vorgonControl.playerDetected)
        {
            // If player Detected (stealth system) -> Close Patrol
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
        }
        else if (IsInCurrentRange(vorgonControl.transform, destination, 2)) 
        {
            // Reach section -> Lost
            vorgonFSM.PerformTransition(Transition.ReachedSection);
        }
        
    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = destination;
    }
}
