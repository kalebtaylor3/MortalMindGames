using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;

public class VorgonDeadwoodFSM : AdvancedFSM
{    
    [SerializeField ] public static int CHASE_DIST = 30;
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
    private VorgonController vorgonController;

    //// Initialize the FSM for the NPC tank.
    protected override void Initialize()
    {
        vorgonController = GetComponent<VorgonController>();
        //debugDraw = true;

        // Find the Player and init appropriate data.
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;
        
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

        // Chase
        ChaseState chase = new ChaseState(vorgonController);
        chase.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        chase.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);
        chase.AddTransition(Transition.PlayerDetected, FSMStateID.Alerted);
        chase.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        chase.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);


        // Lost
        LostState lost = new LostState(vorgonController);
        lost.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        lost.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        lost.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        lost.AddTransition(Transition.PlayerLost, FSMStateID.Patrol);
        lost.AddTransition(Transition.PlayerDetected, FSMStateID.Alerted);

        // Patrol
        PatrolState patrol = new PatrolState(vorgonController);
        patrol.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        patrol.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        patrol.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        patrol.AddTransition(Transition.PlayerDetected, FSMStateID.Alerted);

        // Attack
        AttackState attack = new AttackState(vorgonController);
        attack.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        attack.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        attack.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        attack.AddTransition(Transition.PlayerLost, FSMStateID.Lost);

        // Alerted
        AlertedState alerted = new AlertedState(vorgonController);
        alerted.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        alerted.AddTransition(Transition.PlayerDetected, FSMStateID.ClosePatrol);
        alerted.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        alerted.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        alerted.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);
        alerted.AddTransition(Transition.PlayerLost, FSMStateID.Lost);

        // Close Patrol
        ClosePatrolState closePatrol = new ClosePatrolState(vorgonController);
        closePatrol.AddTransition(Transition.WrongSection, FSMStateID.Seek);
        closePatrol.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        closePatrol.AddTransition(Transition.ReachedPlayer, FSMStateID.Attack);
        closePatrol.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        closePatrol.AddTransition(Transition.PlayerLost, FSMStateID.Lost);
        closePatrol.AddTransition(Transition.PlayerDetected, FSMStateID.Alerted);

        // Seek
        SeekState seek = new SeekState(vorgonController);
        seek.AddTransition(Transition.ReachedSection, FSMStateID.Lost);
        seek.AddTransition(Transition.Stunned, FSMStateID.Stunned);
        seek.AddTransition(Transition.PlayerFound, FSMStateID.Chase);
        seek.AddTransition(Transition.PlayerDetected, FSMStateID.Alerted);

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
        AddFSMState(alerted);
        AddFSMState(closePatrol);
        AddFSMState(seek);
        AddFSMState(stunned);

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
