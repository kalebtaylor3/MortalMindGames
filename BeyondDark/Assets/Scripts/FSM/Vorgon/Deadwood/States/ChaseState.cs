using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class ChaseState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;
    PlayerController player;

    public static event Action onStartChase;
    public static event Action onEndChase;

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
        vorgonControl.navAgent.speed = vorgonControl.chaseSpeed;
        player = WorldData.Instance.player;
        onStartChase?.Invoke();
    }

    public override void Reason()
    {
        // Transitions

        if(player.safeZone)
        {
            vorgonControl.inChase = false;
            vorgonFSM.PerformTransition(Transition.PlayerLost);
            onEndChase?.Invoke();
        }

        
        if (vorgonControl.stunned)
        {
            // If stunned -> Stun
            vorgonControl.inChase = false;
            vorgonFSM.PerformTransition(Transition.Stunned);
            onEndChase?.Invoke();
        }
        else if (vorgonControl.playerT.isHiding)
        {
            // If lost player -> Alerted
            vorgonControl.inChase = false;
            vorgonControl.sawConceal = true;
            vorgonControl.SetLastDetectedLocation(WorldData.Instance.lastConceal.searchPos.position, WorldData.Instance.lastConceal, VorgonController.EVENT_TYPE.SOUND);
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
            onEndChase?.Invoke();
        }
        else if (!vorgonControl.PlayerInSight)
        {
            // If lost player -> Alerted
            vorgonControl.inChase = false;
            vorgonControl.SetLastDetectedLocation(vorgonControl.playerT.transform.position, null, VorgonController.EVENT_TYPE.SOUND);
            vorgonFSM.PerformTransition(Transition.PlayerDetected);
            onEndChase?.Invoke();            
        }       
        else if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position,2))
        {
            // Reach Player -> Attack
            vorgonControl.inChase = false;
            vorgonFSM.PerformTransition(Transition.ReachedPlayer);            
        }        
    }

    public override void Act()
    {
        // Actions
        vorgonControl.navAgent.destination = vorgonControl.playerT.transform.position;
    }
}
