using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MAimState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;

    bool angleReached = false;


    public MAimState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.Aim;
        minionController = controller;
        minionFSM = controller.minionFSM;
        minionType = controller.type;
        playerT = player;
        navAgent = agent;
    }

    public override void EnterStateInit()
    {
        navAgent.isStopped = false;
        angleReached = false;
    }

    public override void Reason()
    {
        // Transitions

        if (angleReached && !minionController.isAttacking)
        {
            // Anlge reached - > Attack
            minionFSM.PerformTransition(Transition.ReachedPlayer);
        }
        else if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
        {
            // Out of range -> Patrol
            minionFSM.PerformTransition(Transition.PlayerLost);
        }
    }

    public override void Act()
    {
        // Actions
        

        if (angleReached == false)
        {
            // Minion Body Rotation
            Vector3 direction = (playerT.position - minionController.shootPos.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            minionController.transform.rotation = Quaternion.Slerp(minionController.transform.rotation, lookRotation, Time.deltaTime);


            // Minion Head Rotation
            minionController.shootPos.rotation = Quaternion.Slerp(minionController.shootPos.rotation, lookRotation, Time.deltaTime);


            //Vector3 target = playerT.position - minionController.shootPos.position;
            //Vector3 Hdirection = Vector3.RotateTowards(minionController.shootPos.forward, target, 1 * Time.deltaTime, 0);
            //minionController.shootPos.rotation = Quaternion.LookRotation(Hdirection);

            Debug.DrawRay(minionController.shootPos.position, minionController.shootPos.forward, Color.magenta);

            float angle = Quaternion.Angle(minionController.shootPos.rotation, lookRotation);

            if (angle <= 1)
            {
                angleReached = true;
            }

        }
    }
}
