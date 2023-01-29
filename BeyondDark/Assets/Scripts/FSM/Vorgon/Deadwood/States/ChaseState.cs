using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

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
        vorgonControl.navAgent.isStopped = false;
        vorgonControl.inChase = true;
    }

    public override void Reason()
    {
        // Transitions

        
        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonControl.inChase = false;
            vorgonFSM.PerformTransition(Transition.Stunned);
            
        }
        else if (!vorgonControl.PlayerInSight)
        {
            // If lost player -> Close Patrol
            vorgonControl.inChase = false;
            vorgonControl.SetLastDetectedLocation(vorgonControl.playerT.transform.position);
            vorgonFSM.PerformTransition(Transition.PlayerLost);
            
        }
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position,2))
        {
            // Reach Player -> Attack
            vorgonControl.inChase = false;
            vorgonFSM.PerformTransition(Transition.ReachedPlayer);
            
        }
        else if(vorgonControl.playerT.isHiding)
        {
            // If player is hiding -> Close Patrol
            vorgonControl.inChase = false;
            vorgonControl.SetLastDetectedLocation(vorgonControl.playerT.transform.position);
            vorgonFSM.PerformTransition(Transition.PlayerLost);
            
        }
    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = vorgonControl.playerT.transform.position;
    }
}
