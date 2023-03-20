using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePatrolState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
    PlayerController player; 

    private bool reachedLastpoint;
    private int searchCount = 0;
    float range = 10;
    //private bool detectedAgain;

    public ClosePatrolState(VorgonController controller)
    {
        stateID = FSMStateID.ClosePatrol;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
        reachedLastpoint = false;
        //detectedAgain = false;
        searchCount = 0;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        vorgonControl.navAgent.isStopped = false;
        //vorgonControl.playerDetected = false;
        //detectedAgain = false;
        reachedLastpoint = false;
        searchCount = 0;
        vorgonControl.navAgent.speed = vorgonControl.defaultSpeed;
        player = WorldData.Instance.player;
    }

    public override void Reason()
    {
        // Transitions
        if(player.safeZone)
        {
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }

        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonFSM.PerformTransition(Transition.Stunned);
        }
        else if (WorldData.Instance.canSeek)
        {
            // If wrong section -> Seek            
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }
        else if (vorgonControl.playerDetected && !player.safeZone)
        {
            // If player Detected (stealth system) -> Alerted
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
        }        
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight && !player.safeZone)
        {
            // If player Found -> Chase            
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }
        else if (searchCount >= 5)
        {
            //If searched 5 times -> Lost            
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }
    }

    public override void Act()
    {
        // Actions

        if (vorgonControl.SearchAnimCanPlay)
        {
            if (!vorgonControl.SearchAnimIsPlaying)
            {
                vorgonControl.PlaySearchAnim();
            }
        }
        else
        {
            if (vorgonControl.navAgent.remainingDistance <= vorgonControl.navAgent.stoppingDistance) //done with path
            {
                Vector3 randSearchPoint;
                if (vorgonControl.RandomPoint(vorgonControl.LastSeen, range, out randSearchPoint)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(randSearchPoint, Vector3.up, Color.magenta, 1.0f); //so you can see with gizmos
                    vorgonControl.navAgent.destination = randSearchPoint;
                    vorgonControl.SearchAnimCanPlay = true;
                    searchCount++;
                }
            }

        }

        




    }
}