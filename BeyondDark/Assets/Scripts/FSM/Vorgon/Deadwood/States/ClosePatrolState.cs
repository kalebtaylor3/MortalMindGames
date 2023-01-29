using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePatrolState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    private bool reachedLastSeen;
    private int searchCount = 0;
    float range = 10;

    public ClosePatrolState(VorgonController controller)
    {
        stateID = FSMStateID.ClosePatrol;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
        reachedLastSeen = false;
        searchCount = 0;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        vorgonControl.navAgent.isStopped = false;
        reachedLastSeen = false;
        searchCount = 0;
    }

    public override void Reason()
    {
        // Transitions

        // If stunned -> Stun
        if (vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
        }
        else if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            // If wrong section -> Seek
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight)
        {
            // If player Found -> Chase
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }        
        else if(searchCount >= 5)
        {
            //If searched 5 times -> Lost
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }
    }

    public override void Act()
    {
        // Actions
       
        {
            if (vorgonControl.navAgent.remainingDistance <= vorgonControl.navAgent.stoppingDistance) //done with path
            {
                if(vorgonControl.SearchAnimCanPlay)
                {
                    if (!vorgonControl.SearchAnimIsPlaying)
                    {
                        vorgonControl.PlaySearchAnim();
                    }
                }
                else
                {
                    Vector3 randSearchPoint;
                    if (vorgonControl.RandomPoint(vorgonControl.LastSeen, range, out randSearchPoint)) //pass in our centre point and radius of area
                    {
                        Debug.DrawRay(randSearchPoint, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                        vorgonControl.navAgent.destination = randSearchPoint;
                        vorgonControl.SearchAnimCanPlay = true;
                        searchCount++;
                    }
                }
                
            }
        }

    }
}
