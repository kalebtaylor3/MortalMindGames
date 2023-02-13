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

    public GameObject slamIndication;

    public float minZ = 0.0f;
    public float maxZ = 10.0f;

    private float currentZ;
    private float targetZ;

    float distanceToMaintain = 50;

    bool canRotate = true;
    bool canAttack = true;
    bool canCloseSlam = true;

    public Animator vorgonAnimator;

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {



        if (canRotate)
        {
            var lookPos = player.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);



            float playerDistance = Vector3.Distance(transform.position, player.position);
            float playerCloseSlam = Vector3.Distance(closeSlam.position, player.position);
            float playerRange = Vector3.Distance(transform.position, player.position);

            if (playerRange < rangeAttackDistance && playerCloseSlam > closeSlamRadius && playerDistance > attackRadius)
            {
                vorgonAnimator.SetTrigger("RangeAttack");
            }

            if (playerDistance < triggerRadius && playerCloseSlam > closeSlamRadius)
            {
                currentZ = Mathf.Lerp(minZ, maxZ, playerDistance / triggerRadius);

                if(playerDistance < attackRadius && canAttack)
                {
                    vorgonAnimator.ResetTrigger("RangeAttack");
                    vorgonAnimator.ResetTrigger("CloseSlam");
                    vorgonAnimator.SetTrigger("Slam1");
                    Attack(0);
                }

            }
            else if (playerCloseSlam < closeSlamRadius && canCloseSlam)
            {
                vorgonAnimator.ResetTrigger("RangeAttack");
                vorgonAnimator.ResetTrigger("Slam1");
                vorgonAnimator.SetTrigger("CloseSlam");
                Attack(1);
            }
            else
            {
                currentZ = maxZ;
                canRotate = true;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
        }
    }

    void Attack(int attackNom)
    {

        GameObject warning = Instantiate(slamIndication);
        warning.transform.position = player.position;

        switch (attackNom)
        {
            case 0:
                canRotate = false;
                canAttack = false;
                StartCoroutine(WaitForAttack(warning));
                break;
            case 1:
                canCloseSlam = false;
                StartCoroutine(WaitForCloseSlam(warning));
                break;
        }
    }

    IEnumerator WaitForAttack(GameObject warning)
    {

        yield return new WaitForSeconds(2);
        Destroy(warning);

        yield return new WaitForSeconds(6);
        canRotate = true;

        yield return new WaitForSeconds(4);
        canAttack = true;
    }    

    IEnumerator WaitForCloseSlam(GameObject warning)
    {
        yield return new WaitForSeconds(1);
        Destroy(warning);

        yield return new WaitForSeconds(6);
        canCloseSlam = true;
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
