using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
    int currentWP;
    Vector3 destination;


    public PatrolState(VorgonController controller)
    {
        stateID = FSMStateID.Patrol;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
        currentWP = 0;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        vorgonControl.navAgent.isStopped = false;
        currentWP = 0;
        destination = WorldData.Instance.FindActiveSection(WorldData.Instance.activeVorgonSection).SectionWaypoints[currentWP].position;
    }

    public override void Reason()
    {
        // Transitions

        // If stunned -> Stun
        if (vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
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
        if (IsInCurrentRange(vorgonControl.transform, destination, 2)) 
        {            
            currentWP++;
            destination = WorldData.Instance.FindActiveSection(WorldData.Instance.activeVorgonSection).SectionWaypoints[currentWP].position;
        }

        vorgonControl.navAgent.destination = destination;

    }
}
