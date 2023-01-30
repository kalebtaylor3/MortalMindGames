using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;

public class LostState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
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
            else if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
            {
                // If wrong section -> Seek
                vorgonFSM.PerformTransition(Transition.WrongSection);
            } 
            else
            {
                // -> Patrol
                vorgonFSM.PerformTransition(Transition.PlayerLost);
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
