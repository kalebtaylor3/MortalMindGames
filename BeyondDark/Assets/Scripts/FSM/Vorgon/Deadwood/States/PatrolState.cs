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
        currentWP = -1;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        vorgonControl.navAgent.isStopped = false;
        currentWP = 0;
        destination = WorldData.Instance.FindActiveSection(WorldData.Instance.activeVorgonSection).SectionWaypoints[currentWP].position;
        vorgonControl.navAgent.destination = destination;
    }

    public override void Reason()
    {
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
            // If player Detected (stealth system) -> Alerted
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
        }
        else if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            // If wrong section -> Seek
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }

    }

    public override void Act()
    {
        // Actions
        if (vorgonControl.navAgent.remainingDistance <= vorgonControl.navAgent.stoppingDistance) //done with path
        {
            if (vorgonControl.SearchAnimCanPlay)
            {
                if(!vorgonControl.SearchAnimIsPlaying)
                {
                    vorgonControl.PlaySearchAnim();
                }                
            }
            else if (!vorgonControl.SearchAnimCanPlay) 
            {
                
                currentWP++;

                if (currentWP >= WorldData.Instance.FindActiveSection(WorldData.Instance.activeVorgonSection).SectionWaypoints.Count)
                {
                    currentWP = 0;
                    
                }
                
                destination = WorldData.Instance.FindActiveSection(WorldData.Instance.activeVorgonSection).SectionWaypoints[currentWP].position;
                vorgonControl.navAgent.destination = destination;
                vorgonControl.SearchAnimCanPlay = true;

            }
        }
    }
}
