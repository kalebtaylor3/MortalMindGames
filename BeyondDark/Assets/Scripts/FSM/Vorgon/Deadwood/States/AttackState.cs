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

            if (IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, VorgonDeadwoodFSM.CHASE_DIST) && vorgonControl.PlayerInSight && !vorgonControl.playerT.isHiding) 
            {
                // If player Found -> Chase
                vorgonFSM.PerformTransition(Transition.PlayerFound);
            }
            else if (WorldData.Instance.activeVorgonSection != WorldData.Instance.activePlayerSection)
            {
                // If wrong section -> Seek
                vorgonFSM.PerformTransition(Transition.WrongSection);
            }
            else
            {
                // -> Lost
                vorgonFSM.PerformTransition(Transition.PlayerLost);
            }
        }
    }

    public override void Act()
    {
        // Actions

        if(!vorgonControl.isAttacking && IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, 2) && !vorgonControl.playerT.isHiding)
        {
            vorgonControl.Attack();
            //vorgonControl.gameObject.SetActive(false);
            vorgonControl.transform.position = WorldData.Instance.FindActiveSection(WorldData.Instance.activePlayerSection).vorgonTP.position;
            //vorgonControl.gameObject.SetActive(true);
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }
        else if(!vorgonControl.isAttacking && IsInCurrentRange(vorgonControl.transform, vorgonControl.playerT.transform.position, 5) && vorgonControl.playerT.isHiding)
        {
            vorgonControl.Attack(true);
            //vorgonControl.gameObject.SetActive(false);
            vorgonControl.transform.position = WorldData.Instance.FindActiveSection(WorldData.Instance.activePlayerSection).vorgonTP.position;
            //vorgonControl.gameObject.SetActive(true);
            vorgonFSM.PerformTransition(Transition.PlayerLost);
        }

        if (!vorgonControl.navAgent.isStopped)
        {
            vorgonControl.navAgent.isStopped = true;
        }

    }
}
