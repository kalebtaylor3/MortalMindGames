using MMG;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class LostState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
    PlayerController player;

    bool lostTimer;
    float lostTime;

    public LostState(VorgonController controller)
    {
        stateID = FSMStateID.Lost;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
        lostTimer = false;
        lostTime = 0;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();
        //MonoBehaviour.StartCoroutine(StartLostTimer());
        lostTimer = false;
        lostTime = 0;
        vorgonControl.navAgent.speed = vorgonControl.defaultSpeed;
        player = WorldData.Instance.player;
        vorgonControl.vorgonAnimator.SetBool("Lost", true);
    }

    public override void Reason()
    {
        // Transitions
        if(lostTimer)
        {

            if (vorgonControl.stunned)
            {
                // If stunned -> Stun
                vorgonFSM.PerformTransition(Transition.Stunned);
                vorgonControl.vorgonAnimator.SetBool("Lost", false);
            }
            else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight && !player.safeZone)
            {
                // If player Found -> Chase
                vorgonFSM.PerformTransition(Transition.PlayerFound);
                vorgonControl.vorgonAnimator.SetBool("Lost", false);
            }
            else if (vorgonControl.playerDetected && !player.safeZone)
            {
                // If player Detected (stealth system) -> Close Patrol
                vorgonFSM.PerformTransition(Transition.PlayerDetected);
                vorgonControl.vorgonAnimator.SetBool("Lost", false);
            }
            else if (WorldData.Instance.canSeek)
            {
                // If wrong section -> Seek
                vorgonFSM.PerformTransition(Transition.WrongSection);
                vorgonControl.vorgonAnimator.SetBool("Lost", false);
            } 
            else
            {
                // -> Patrol
                vorgonFSM.PerformTransition(Transition.PlayerLost);
                vorgonControl.vorgonAnimator.SetBool("Lost", false);
            }
        }
        else
        {
            lostTime += 3 * Time.deltaTime;

            if(lostTime >= 3f)
            {
                lostTimer = true;
            }
        }
    }

    public override void Act()
    {
        // Actions
        if (!vorgonControl.navAgent.isStopped)
        {
            vorgonControl.navAgent.isStopped = true;
        }
    }
}
