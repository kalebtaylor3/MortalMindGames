using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class VorgonBossController : MonoBehaviour
{

    public Transform player;
    public float rotationSpeed = 5f; // The speed of the movement
    public float maxBackupDistance = 10f;
    public float backupSpeed = 2f;
    public float triggerRadius = 100f;
    public float attackRadius = 80f;

    public float minZ = 0.0f;
    public float maxZ = 10.0f;

    private float currentZ;
    private float targetZ;

    float distanceToMaintain = 50;

    bool canRotate = true;
    bool canAttack = true;

    public Animator vorgonAnimator;

    // Start is called before the first frame update
    void Start()
    {

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

            if (playerDistance < triggerRadius)
            {
                currentZ = Mathf.Lerp(minZ, maxZ, playerDistance / triggerRadius);

                if(playerDistance < attackRadius && canAttack)
                {
                    Attack();
                }

            }
            else
            {
                currentZ = maxZ;
                canRotate = true;
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
        }
    }

    void Attack()
    {
        StartCoroutine(WaitForAttack());
        vorgonAnimator.SetTrigger("Slam1");
        canRotate = false;
        canAttack = false;
    }

    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(3);
        canRotate = true;

        yield return new WaitForSeconds(10);
        canAttack = true;
    }    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }

}
