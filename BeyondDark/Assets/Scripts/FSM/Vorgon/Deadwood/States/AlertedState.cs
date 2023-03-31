using MMG;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AlertedState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
    PlayerController player;

    bool alertReached = false;
    bool detectedAgain = false;
    bool killConceal = false;
    bool closePatrolFlag = false;
    bool rotConceal = false;
    bool canConcealKill = false;

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
        closePatrolFlag = false;
        rotConceal = false;
        canConcealKill = false;

        vorgonControl.navAgent.isStopped = false;

        vorgonControl.playerDetected = false;
        player = WorldData.Instance.player;
    }

    public override void Reason()
    {
        //Logic

        if (vorgonControl.SearchAnimIsPlaying)
        {
            vorgonControl.navAgent.isStopped = true;
        }
        else
        {
            vorgonControl.navAgent.isStopped = false;
        }


        if (alertReached)
        {
            if (!vorgonControl.SearchAnimIsPlaying)
            {
                if (vorgonControl.sawConceal || canConcealKill)
                {
                    //killConceal = true;

                    if (killConceal)
                    {
                        vorgonControl.playerDetected = false;
                        vorgonFSM.PerformTransition(Transition.ReachedPlayer);
                    }
                    // If saw enter conceal and reached the conceal pos -> Attack

                    // closePatrolFlag = false;
                }
                else if (closePatrolFlag) 
                {
                    // If point reached -> Close Patrol
                    vorgonControl.SearchAnimCanPlay = false;
                    vorgonFSM.PerformTransition(Transition.PlayerDetected);
                    //closePatrolFlag = true;
                }
            }

        }

        // Transitions
        if(player.safeZone)
        {
            // If player Found -> Chase
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }

        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.Stunned);
        }
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, 30) && vorgonControl.PlayerInSight && !vorgonControl.playerT.isHiding && !player.safeZone)
        {
            // If player Found -> Chase
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.PlayerFound);
        }
        else if (WorldData.Instance.canSeek)
        {
            // If wrong section -> Seek
            vorgonControl.playerDetected = false;
            vorgonFSM.PerformTransition(Transition.WrongSection);
        }        
    }

    public override void Act()
    {
        // Actions

        if(WorldData.Instance.GetComponent<ConcelableDetection>().exposure >= 1)
        {
            canConcealKill = true;
        }
        

        if (vorgonControl.playerDetected)
        {
            alertReached = false;
            //vorgonControl.playerDetected = false;
            detectedAgain = true;
            //closePatrolFlag = false;
        }

        if (!alertReached && !vorgonControl.SearchAnimIsPlaying)
        {
            vorgonControl.navAgent.destination = vorgonControl.LastSeen;

            if (vorgonControl.navAgent.remainingDistance <= vorgonControl.navAgent.stoppingDistance) //done with path
            {

                //if(!player.isHiding)
                //{
                    if (vorgonControl.SearchAnimCanPlay)
                    {
                        if (!vorgonControl.SearchAnimIsPlaying)
                        {
                            vorgonControl.PlaySearchAnim();
                        }
                    }
                //}
                

                alertReached = true;
                detectedAgain = false;
                vorgonControl.PlaySoundEffect(VorgonController.SOUND_TYPE.ALERT);
                vorgonControl.playerDetected = false;
            }
        }

        if (alertReached)
        {
            if (vorgonControl.concealPos != null)
            {
                if (!rotConceal)
                {
                    //if(vorgonControl.sawConceal)
                    //{
                        vorgonControl.navAgent.isStopped = false;
                        Vector3 direction = (vorgonControl.concealPos - vorgonControl.transform.position).normalized;
                        Quaternion lookRotation = Quaternion.LookRotation(direction);
                        vorgonControl.transform.rotation = Quaternion.Slerp(vorgonControl.transform.rotation, lookRotation, Time.deltaTime);

                        if (Quaternion.Angle(vorgonControl.transform.rotation, lookRotation) <= 10)
                        {
                            if (!vorgonControl.SearchAnimIsPlaying)
                            {
                                vorgonControl.PlaySearchAnim();
                                rotConceal = true;
                            }                                                   
                        }
                    //}
                    //else
                    //{
                    //    rotConceal = true;
                    //}
                    
                }
                else
                {
                    killConceal = true;
                    closePatrolFlag = true;
                }
            }
            else
            {
                closePatrolFlag = true;
            }
        }


        //else if (alertReached && (vorgonControl.sawConceal || vorgonControl.PlayerInSight) && !vorgonControl.SearchAnimIsPlaying)
        //{
        //    killConceal = true;
        //    closePatrolFlag = false;
        //}
        //else if (!alertReached && vorgonControl.SearchAnimIsPlaying && !detectedAgain && !vorgonControl.playerT.isHiding)
        //{
        //    vorgonControl.navAgent.isStopped = true;
        //    closePatrolFlag = true;
        //}
        //else if (alertReached && !vorgonControl.SearchAnimIsPlaying && vorgonControl.playerT.isHiding)
        //{
        //    closePatrolFlag = true;
        //}

    }
}