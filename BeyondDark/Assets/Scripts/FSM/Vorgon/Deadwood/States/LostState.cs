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
            // If stunned -> Stun
            if (vorgonControl.stunned)
            {
                vorgonFSM.PerformTransition(Transition.Stunned);
            }

            // If wrong section -> Seek
            if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
            {
                vorgonFSM.PerformTransition(Transition.WrongSection);
            }

            // If player Found -> Chase
            if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position, VorgonDeadwoodFSM.CHASE_DIST))
            {
                vorgonFSM.PerformTransition(Transition.PlayerFound);
            }

            // -> Patrol
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }
        else
        {
            lostTime  = lostTime + Time.deltaTime;

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
