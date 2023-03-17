using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class MAttackWallState : FSMState
{
    MinionController minionController;
    MinionFSM minionFSM;
    MinionController.MINION_TYPE minionType;
    Transform playerT;
    public NavMeshAgent navAgent;

    bool angleReached = false;

    public MAttackWallState(MinionController controller, Transform player, NavMeshAgent agent)
    {
        stateID = FSMStateID.AttackWall;
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
        minionType = minionController.type;
    }

    public override void Reason()
    {
        // Transitions

        if (minionController.minionDeath)
        {
            // Minion Death
            minionFSM.PerformTransition(Transition.MinionDeath);
        }
        else
        {
            if (!minionController.safe)
            {
                // If unsafe -> Run Away
                minionFSM.PerformTransition(Transition.Unsafe);
            }

            if (!minionController.foundWall)
            {
                if (!IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
                {
                    // Out of range -> Patrol
                    minionFSM.PerformTransition(Transition.PlayerLost);
                }

                // RANGED
                if (minionType == MinionController.MINION_TYPE.RANGED)
                {
                    if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.RANGED_DIST))
                    {
                        // If on chase range -> Aim
                        minionFSM.PerformTransition(Transition.PlayerDetected);
                    }


                }
                //MELEE
                else if (minionType == MinionController.MINION_TYPE.MELEE)
                {
                    if (minionController.onFire)
                    {
                        // If on Fire -> Burning
                        minionFSM.PerformTransition(Transition.OnFlames);
                    }

                    if (IsInCurrentRange(minionController.transform, playerT.position, minionFSM.CHASE_DIST))
                    {
                        // If on chase range -> Chase
                        minionFSM.PerformTransition(Transition.PlayerFound);
                    }
                }
            }
        }
    }

    public override void Act()
    {
        // RANGED
        if (minionType == MinionController.MINION_TYPE.RANGED)
        {
            // Call shoot projectile
            // Minion Body Rotation
            if(!angleReached)
            {
                Vector3 direction = (minionController.currentWall.position - minionController.shootPos.position).normalized;
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
            else if(angleReached)
            {
                if(!minionController.isAttacking)
                {
                    minionController.RangedAttack();
                }
            }

           
        }
        else if (minionType == MinionController.MINION_TYPE.MELEE) // MELEE
        {
            // Call Attack (check range)

            if(!navAgent.isStopped)
            {
                navAgent.isStopped = true;
            }
            
        }
    }
}