using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private CombatInputData combatInputData = null;

    public CharacterController vorgonPlayer;
    public Transform orientation;
    public PlayerController playerMovement;

    public float dashForce;
    public float dashUpForce;
    public float dashDuration;

    public float dashCooldown;
    private float cooldownTimer;

    Vector3 newDashForce = Vector3.zero;
    private bool isDashing = false;


    private void OnEnable()
    {
        isDashing = false;
    }

    private void Update()
    {
        if(combatInputData.DashFlag)
        {
            calculateDash();
        }

        if(isDashing)
        {
            PerformDash();
        }     
        
        if(cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void calculateDash() 
    {
        if (cooldownTimer > 0)
        {
            return;
        }
        else
        {
            cooldownTimer = dashCooldown;
        }

        float InputH = Gamepad.current.leftStick.x.ReadValue();
        float InputV = Gamepad.current.leftStick.y.ReadValue();

        Vector3 dir = orientation.forward * InputV + orientation.right * InputH;

        if(InputH == 0 && InputV == 0)
        {
            dir = orientation.forward;
        }

        newDashForce = dir * dashForce + orientation.up * dashUpForce;
        isDashing = true;
    }

    private void PerformDash()
    {
        playerMovement.PlayerDash(true);
        vorgonPlayer.Move(newDashForce * Time.deltaTime);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private void ResetDash()
    {
        isDashing = false;
        newDashForce = Vector3.zero;
        playerMovement.PlayerDash(false);
    }

    private void AddImpact(Vector3 dir, float force, ref Vector3 forceToApply)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        forceToApply += dir.normalized * force / 3;
    }
}
