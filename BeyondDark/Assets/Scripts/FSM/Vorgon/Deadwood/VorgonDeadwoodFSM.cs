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
    public static int CHASE_DIST = 40;
    public static int WAYPOINT_DIST = 1;
        
    public NavMeshAgent navAgent;


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

    // Update each frame.
    protected override void FSMUpdate()
    {
        if (CurrentState != null)
        {
            CurrentState.Reason();
            CurrentState.Act();
        }
        //if (debugDraw)
        //{
        //    UsefullFunctions.DebugRay(transform.position, transform.forward * 5.0f, Color.red);
        //}
    }

    private void ConstructFSM()
    {

        // //pointList = GameObject.FindGameObjectsWithTag("WayPoint");
        // // Creating a waypoint transform array for each state.
        // Transform[] waypoints = new Transform[pointList.Length];
        // int i = 0;
        // foreach (GameObject obj in pointList)
        // {
        //     waypoints[i] = obj.transform;
        //     i++;
        // }

        // Create States

        //Patrol
        PatrolState patrol = new PatrolState(vorgonController);
        patrol.AddTransition(Transition.ReachedPlayer, FSMStateID.Lost);



       // Add states to the state list.
       AddFSMState(patrol);
        
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
