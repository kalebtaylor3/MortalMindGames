using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertedState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    bool alertReached = false;
    bool detectedAgain = false;
    bool killConceal = false;

    public AlertedState(VorgonController controller)
    {
        stateID = FSMStateID.Alerted;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
    }

    public override void EnterStateInit()
    {
        alertReached = false;
        detectedAgain = false;
        killConceal = false;

        vorgonControl.playerDetected = false;
    }

    public override void Reason()
    {
        // Transitions
        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.Stunned);
        }
        else if ((killConceal && vorgonControl.sawConceal && vorgonControl.playerT.isHiding) || (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, 30) && vorgonControl.playerT.isHiding && vorgonControl.PlayerInSight))
        {
            // If saw enter conceal and reached the conceal pos -> Attack
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.ReachedPlayer);
        }
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, 30) && vorgonControl.PlayerInSight)
        {
            // If player Found -> Chase
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }        
        else if (alertReached && !detectedAgain)
        {
            // If point reached -> Close Patrol
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
        }
        else if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
        {
            // If wrong section -> Seek
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }        
    }

    public override void Act()
    {
        // Actions

        if (vorgonControl.playerDetected)
        {
            alertReached = false;
            //vorgonControl.playerDetected = false;
            detectedAgain = true;
        }

        if (!alertReached && !vorgonControl.SearchAnimIsPlaying)
        {
            vorgonControl.navAgent.destination = vorgonControl.LastSeen;

            if (vorgonControl.navAgent.remainingDistance <= vorgonControl.navAgent.stoppingDistance) //done with path
            {

                if (vorgonControl.SearchAnimCanPlay)
                {
                    if (!vorgonControl.SearchAnimIsPlaying)
                    {
                        vorgonControl.PlaySearchAnim();
                    }
                }

                alertReached = true;
                vorgonControl.PlaySoundEffect(VorgonController.SOUND_TYPE.ALERT);
                vorgonControl.playerDetected = false;
            }
        }     
        else if(alertReached && vorgonControl.sawConceal && vorgonControl.SearchAnimIsPlaying)
        {
            killConceal = true;
        }
        else if (!alertReached && vorgonControl.SearchAnimIsPlaying && !detectedAgain)
        {
            vorgonControl.navAgent.isStopped = true;
        }

        if (vorgonControl.concealPos != Vector3.zero && alertReached && vorgonControl.SearchAnimIsPlaying)
        {
            Vector3 direction = (vorgonControl.concealPos - vorgonControl.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            vorgonControl.transform.rotation = Quaternion.Slerp(vorgonControl.transform.rotation, lookRotation, Time.deltaTime);
        }


    }
}