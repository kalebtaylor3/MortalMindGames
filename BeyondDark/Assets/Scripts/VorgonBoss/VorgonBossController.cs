using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class VorgonBossController : MonoBehaviour
{

    /*
     *************************************
     *huge wip. need to clean up
     ***************************************
     */

    public enum  State { RangeAttack, Slam, CloseSlam, FireWall, RainFire, Idle}
    public State state;

    public Transform player;
    public Transform closeSlam;
    public float rotationSpeed = 5f; // The speed of the movement
    public float maxBackupDistance = 10f;
    public float backupSpeed = 2f;
    public float triggerRadius = 100f;
    public float attackRadius = 80f;
    public float closeSlamRadius = 10f;
    public float rangeAttackDistance = 120;

    //public GameObject slamIndication;
    public GameObject fireWarning;

    public float minZ = 0.0f;
    public float maxZ = 10.0f;

    private float currentZ;
    private float targetZ;

    float distanceToMaintain = 50;

    bool canRotate = true;
    bool canAttack = true;
    bool canCloseSlam = true;
    bool canCast = true;
    bool isCloseSlamming = false;
    bool rainFire = false;

    public Animator vorgonAnimator;

    public GameObject ground;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        if(rainFire)
            RainFire();

        if (isCloseSlamming)
        {
            currentZ = maxZ;
            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }


        if (canRotate)
        {
            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            float playerDistance = Vector3.Distance(transform.position, player.position);

            if (playerDistance < triggerRadius)
            {
                currentZ = Mathf.Lerp(minZ, maxZ, playerDistance / triggerRadius);
            }
            else
            {
                currentZ = maxZ;
            }



            GetState();
            Act();
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);

        //float playerDistance = Vector3.Distance(transform.position, player.position);
        //if (playerDistance < triggerRadius)
        //{
        //    currentZ = Mathf.Lerp(minZ, maxZ, playerDistance / triggerRadius);
        //}
        //else
        //{
        //    currentZ = maxZ;
        //}

        //transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
    }

    void GetState()
    {
        float playerCloseSlam = Vector3.Distance(closeSlam.position, player.position);
        float playerRange = Vector3.Distance(transform.position, player.position);

        if(!canAttack || !canCloseSlam || !canCast)
            state = State.Idle;

        //if in shoot range distance & not in melee or close range

        if (playerRange < rangeAttackDistance && playerCloseSlam > closeSlamRadius && playerRange > attackRadius)
        {
            //check if can do ranged attack. all happen so often. delay will varry on what type of attack it is.
            if(canCast)
                state = State.RangeAttack;
        }

        if (playerCloseSlam > closeSlamRadius)
        {
            if (playerRange < attackRadius && canAttack && !isCloseSlamming)
                state = State.Slam;
            else
                canRotate = true;
        }

        if (playerCloseSlam < closeSlamRadius && canCloseSlam)
            state = State.CloseSlam;
    }

    void Act()
    {
        switch(state)
        {
            case State.RangeAttack:
                vorgonAnimator.SetTrigger("RangeAttack");
                RangeAttack(1);
                break;
            case State.Slam:
                vorgonAnimator.ResetTrigger("RangeAttack");
                vorgonAnimator.ResetTrigger("CloseSlam");
                vorgonAnimator.SetTrigger("Slam1");
                Attack(0);
                break;
            case State.CloseSlam:
                vorgonAnimator.ResetTrigger("RangeAttack");
                vorgonAnimator.ResetTrigger("Slam1");
                vorgonAnimator.SetTrigger("CloseSlam");
                Attack(1);
                break;
        }
    }

    void Attack(int attackNom)
    {

        //GameObject warning = Instantiate(slamIndication);
        //warning.transform.position = player.position;
        canCast = true;

        switch (attackNom)
        {
            case 0:
                canRotate = false;
                canAttack = false;
                StartCoroutine(WaitForAttack());
                break;
            case 1:
                canRotate = false;
                canCloseSlam = false;
                isCloseSlamming = true;
                StartCoroutine(WaitForCloseSlam());
                break;
        }
    }

    IEnumerator WaitForAttack()
    {

        yield return new WaitForSeconds(12);
        canRotate = true;

        yield return new WaitForSeconds(4);
        canAttack = true;
    }    

    IEnumerator WaitForCloseSlam()
    {
        yield return new WaitForSeconds(8f);
        canRotate = true;
        isCloseSlamming = false;

        yield return new WaitForSeconds(6);
        canCloseSlam = true;
    }


    void RangeAttack(int attackNom)
    {
        //0 being normal 1 being hell fire
        switch (attackNom)
        {
            case 0:
                StartCoroutine(WaitForCast(8f));
                break;
            case 1:
                rainFire = true;
                StartCoroutine(WaitForCast(15f));
                break;
        }

        canCast = false;
    }

    void RainFire()
    {
        
    }

    IEnumerator WaitForCast(float delay)
    {
        yield return new WaitForSeconds(delay);
        canCast = true;
        rainFire = false;
    }

    public void SlamShake()
    {
        CameraShake.Instance.ShakeCamera(10, 10, 1);
        Rumbler.Instance.RumbleConstant(0.5f, 2f, 1);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(closeSlam.position, closeSlamRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rangeAttackDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

}
