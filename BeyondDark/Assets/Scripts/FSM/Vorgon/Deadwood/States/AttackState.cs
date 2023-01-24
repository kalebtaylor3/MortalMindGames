using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : FSMState
{
    VorgonController vorgonControl;
    VorgonDeadwoodFSM vorgonFSM;

    public AttackState(VorgonController controller)
    {
        stateID = FSMStateID.Attack;
        vorgonControl = controller;
        vorgonFSM = controller.vorgonFSM;
    }

    public override void EnterStateInit()
    {
        //base.EnterStateInit();        
    }

    public override void Reason()
    {
        // Transitions

        // If stunned -> Stun
        if (vorgonControl.stunned)
        {
            vorgonFSM.PerformTransition(Transition.Stunned);
        }

        if(!vorgonControl.isAttacking)
        {
            // If player Found -> Chase
            if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.position, VorgonDeadwoodFSM.CHASE_DIST))
            {
                vorgonFSM.PerformTransition(Transition.PlayerFound);
            }

            // If wrong section -> Seek
            if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
            {
                vorgonFSM.PerformTransition(Transition.WrongSection);
            }

            // -> Lost
            vorgonFSM.PerformTransition(Transition.PlayerLost);

        }
    }

    public override void Act()
    {
        // Actions

        if(!vorgonControl.isAttacking)
        {
            vorgonControl.Attack();
        }

        if (!vorgonControl.navAgent.isStopped)
        {
            vorgonControl.navAgent.isStopped = true;
        }

    }
}
