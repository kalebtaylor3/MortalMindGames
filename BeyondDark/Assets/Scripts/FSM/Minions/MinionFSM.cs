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

    private Transform playerTransform;
    private MinionController minionController;

    //// Initialize the FSM for the NPC tank.
    protected override void Initialize()
    {
        minionController = GetComponent<MinionController>();
        //debugDraw = true;

        // Find the Player and init appropriate data.
        GameObject objPlayer = GameObject.FindGameObjectWithTag("VorgonRealmPlayer");
        playerTransform = objPlayer.transform;

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

        // Chase MELEE
        MChaseState chase = new MChaseState(minionController, playerTransform, navAgent);
        chase.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     
        chase.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack); 
        chase.AddTransition(Transition.OnFlames, FSMStateID.Burning);

        // Attack BOTH
        MAttackState attack = new MAttackState(minionController, playerTransform, navAgent);
        attack.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);     // Both
        attack.AddTransition(Transition.PlayerFound, FSMStateID.Chase);     // Melee
        attack.AddTransition(Transition.OnFlames, FSMStateID.Burning);      // Both
        attack.AddTransition(Transition.PlayerDetected, FSMStateID.Aim);    // Ranged
        
        // Aim RANGED
        MAimState aim = new MAimState(minionController, playerTransform, navAgent);
        aim.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        aim.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);        


        // Burning MELEE
        MBurningState burning = new MBurningState(minionController, playerTransform, navAgent);
        burning.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        burning.AddTransition(Transition.PlayerFound, FSMStateID.Chase);


        // Add State to FSM
        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(aim);
        AddFSMState(burning);
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
