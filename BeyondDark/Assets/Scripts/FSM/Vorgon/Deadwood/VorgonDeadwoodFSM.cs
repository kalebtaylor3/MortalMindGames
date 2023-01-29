using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class VorgonDeadwoodFSM : AdvancedFSM
{
    //Adjust these as needed...
    //public static int SLOT_DIST = 1;
    //public static int ATTACK_DIST = 25;
    [SerializeField ] public static int CHASE_DIST = 10;
    public static int WAYPOINT_DIST = 1;
        
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
    //private GameObject[] pointList;
    //private SlotManager playerSlotManager;
    //private SlotManager coverSlotManager;
    //private bool debugDraw;
    private VorgonController vorgonController;

    //// Initialize the FSM for the NPC tank.
    protected override void Initialize()
    {
        vorgonController = GetComponent<VorgonController>();
        //debugDraw = true;

        // Find the Player and init appropriate data.
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        //playerSlotManager = objPlayer.GetComponent<SlotManager>();
        //coverSlotManager = GameObject.FindGameObjectWithTag("CoverSlotManager").GetComponent<SlotManager>();
        //rigBody = GetComponent<Rigidbody>();

        //receivedAttackCommand = false;

        // Listen to attack events
        //AIManager.instance.startAttackEvent.AddListener(HandleStartAttackEvent);
        //AIManager.instance.stopAttackEvent.AddListener(HandleStopAttackEvent);

        // Create the FSM for the tank.
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
        //if (debugDraw)
        //{
        //    UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.red);
        //}
    }

    private void ConstructFSM()
    {
        ////pointList = GameObject.FindGameObjectsWithTag("WayPoint");
        //// Creating a waypoint transform array for each state.
        //Transform[] waypoints = new Transform[pointList.Length];
        //int i = 0;
        //foreach (GameObject obj in pointList)
        //{
        //    waypoints[i] = obj.transform;
        //    i++;
        //}
        

        // Chase
        ChaseState chase = new ChaseState(vorgonController);
        chase.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        chase.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);
        chase.AddTransition(Transition.PlayerLost, FSMStateID.ClosePatrol);
        chase.AddTransition(Transition.Stunned, FSMStateID.Stunned);


        // Lost
        LostState lost = new LostState(vorgonController);
        lost.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        lost.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        lost.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        lost.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        lost.AddTransition(Transition.PlayerDetected, FSMStateID.ClosePatrol);

        // Patrol
        PatrolState patrol = new PatrolState(vorgonController);
        patrol.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        patrol.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        patrol.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        patrol.AddTransition(Transition.PlayerDetected, FSMStateID.ClosePatrol);

        // Attack
        AttackState attack = new AttackState(vorgonController);
        attack.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        attack.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        attack.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        attack.AddTransition(Transition.PlayerLost, FSMStateID.Lost);

        // Close Patrol
        ClosePatrolState closePatrol = new ClosePatrolState(vorgonController);
        closePatrol.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        closePatrol.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        closePatrol.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        closePatrol.AddTransition(Transition.PlayerLost, FSMStateID.Lost);

        // Seek
        SeekState seek = new SeekState(vorgonController);
        seek.AddTransition(Transition.ReachedSection, FSMStateID.Lost);
        seek.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        seek.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        seek.AddTransition(Transition.PlayerDetected, FSMStateID.ClosePatrol);

        // Stunned
        StunnedState stunned = new StunnedState(vorgonController);
        stunned.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        stunned.AddTransition(Transition.StunDone, FSMStateID.Lost);
        stunned.AddTransition(Transition.PlayerFound, FSMStateID.Chase);


        // Add State to FSM
        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(lost);
        
        AddFSMState(attack);
        AddFSMState(closePatrol);
        AddFSMState(seek);
        AddFSMState(stunned);

    }

    //void HandleStartAttackEvent(int number)
    //{
    //    if (number == enemyControl.charID && CurrentStateID == FSMStateID.Attacking)
    //    {
    //        // Shoot
    //        receivedAttackCommand = true;
    //    }
    //}

    //void HandleStopAttackEvent()
    //{
    //    receivedAttackCommand = false;
    //}

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
