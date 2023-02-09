using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionFSM : AdvancedFSM
{
    [SerializeField] public  int CHASE_DIST = 30;
    [SerializeField] public  int RANGED_DIST = 50;
    //public static int WAYPOINT_DIST = 1;

    public NavMeshAgent navAgent;
    [SerializeField] string StringState;

    SlotManager playerSlot = null;
    private Transform playerTransform;
    private MinionController minionController;


    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public string GetStateString()
    {
        string state = "NONE";
        if (CurrentState != null)
        {
            state = CurrentState.ToString();
        }
        return state;
    }    

    //// Initialize the FSM for the NPC tank.
    protected override void Initialize()
    {
        minionController = GetComponent<MinionController>();
        //debugDraw = true;

        // Find the Player and init appropriate data.
        GameObject objPlayer = GameObject.FindGameObjectWithTag("VorgonRealmPlayer");
        playerTransform = objPlayer.transform;
        playerSlot = objPlayer.GetComponent<SlotManager>();

        navAgent = minionController.navAgent;

        ConstructFSM();
    }

    //// Update each frame.
    protected override void FSMUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.Reason();
            CurrentState.Act();
        }

        StringState = GetStateString();
    }

    private void ConstructFSM()
    {
        // STATES
        // Patrol BOTH
        MPatrolState patrol = new MPatrolState(minionController, playerTransform, navAgent);
        patrol.AddTransition(Transition.PlayerFound, FSMStateID.Chase);     // Melee
        patrol.AddTransition(Transition.PlayerDetected, FSMStateID.Aim);    // Ranged
        patrol.AddTransition(Transition.OnFlames, FSMStateID.Burning);      // Both
        patrol.AddTransition(Transition.FoundWall, FSMStateID.AttackWall);  // Both
        patrol.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        // Both

        // Chase MELEE
        MChaseState chase = new MChaseState(minionController, playerTransform, navAgent, playerSlot);
        chase.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     
        chase.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack); 
        chase.AddTransition(Transition.OnFlames, FSMStateID.Burning);
        chase.AddTransition(Transition.FoundWall, FSMStateID.AttackWall);
        chase.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        // Both

        // Attack BOTH
        MAttackState attack = new MAttackState(minionController, playerTransform, navAgent, playerSlot);
        attack.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     // Both
        attack.AddTransition(Transition.PlayerFound, FSMStateID.Chase);     // Melee
        attack.AddTransition(Transition.OnFlames, FSMStateID.Burning);      // Both
        attack.AddTransition(Transition.PlayerDetected, FSMStateID.Aim);    // Ranged
        attack.AddTransition(Transition.FoundWall, FSMStateID.AttackWall);  // Both
        attack.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        // Both

        // Aim RANGED
        MAimState aim = new MAimState(minionController, playerTransform, navAgent);
        aim.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        aim.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);
        aim.AddTransition(Transition.FoundWall, FSMStateID.AttackWall);
        aim.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        


        // Burning MELEE
        MBurningState burning = new MBurningState(minionController, playerTransform, navAgent);
        burning.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        burning.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        burning.AddTransition(Transition.FoundWall, FSMStateID.AttackWall);
        burning.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        

        // Attack Wall BOTH
        MAttackWallState attackWall = new MAttackWallState(minionController, playerTransform, navAgent);
        attackWall.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     // Both
        attackWall.AddTransition(Transition.PlayerFound, FSMStateID.Chase);     // Melee
        attackWall.AddTransition(Transition.OnFlames, FSMStateID.Burning);      // Both
        attackWall.AddTransition(Transition.PlayerDetected, FSMStateID.Aim);    // Ranged
        attackWall.AddTransition(Transition.Unsafe, FSMStateID.RunAway);        // Both

        // Run Away BOTH
        MRunAwayState runAway = new MRunAwayState(minionController, playerTransform, navAgent);
        runAway.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     // Both
        runAway.AddTransition(Transition.PlayerFound, FSMStateID.Chase);     // Melee
        runAway.AddTransition(Transition.OnFlames, FSMStateID.Burning);      // Both
        runAway.AddTransition(Transition.PlayerDetected, FSMStateID.Aim);    // Ranged




        // Add State to FSM
        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(aim);
        AddFSMState(burning);
        AddFSMState(attackWall);
        AddFSMState(runAway);
    }

    private void OnEnable()
    {
        if (navAgent)
            navAgent.isStopped = false;
        if (CurrentState != null)
        {

        }
        // Request the state machine to perform the "enable" transition.
        // The current state of the AI must handle the enable request. See Dead state initalization in ConstructFSM.
        //PerformTransition(Transition.Enable);
    }
    private void OnDisable()
    {
        if (navAgent && navAgent.isActiveAndEnabled)
            navAgent.isStopped = true;
    }
}
