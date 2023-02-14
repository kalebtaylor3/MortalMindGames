using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonBossController : MonoBehaviour
{
    /*
     *************************************
     *huge wip. need to clean up
     ***************************************
     */

    public enum State { RangeAttack, Slam, CloseSlam, FireWall, RainFire, Idle }
    public State state;

    public Transform player;
    public Transform closeSlam;
    public float rotationSpeed = 5f; // The speed of the movement
    public float maxBackupDistance = 10f;
    public float backupSpeed = 2f;
    public float triggerRadius = 110f;
    public float attackRadius = 76;
    public float closeSlamRadius = 13f;
    public float rangeAttackDistance = 120;
    public float hellFireRange;
    public float hellFireSwitchTime;

    //public GameObject slamIndication;
    public GameObject fireWarning;

    public float minZ = -135;
    public float maxZ = -180;

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

    public LayerMask groundLayer;
    public int numberOfHitPoints;
    public float minDistanceBetweenObjects;

    List<Vector3> objectPositions = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        state = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {

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

        if (!canAttack || !canCloseSlam || !canCast)
            state = State.Idle;

        //if in shoot range distance & not in melee or close range

        if (playerRange < rangeAttackDistance && playerCloseSlam > closeSlamRadius && playerRange > attackRadius)
        {
            canRotate = true;
            canAttack = true;
            //check if can do ranged attack. all happen so often. delay will varry on what type of attack it is.
            if (canCast)
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
        switch (state)
        {
            case State.RangeAttack:
                int randomNumber = Random.Range(0, 100);
                if (randomNumber >= 60)
                    RangeAttack(1);
                else
                    RangeAttack(0);
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

        yield return new WaitForSeconds(3);
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
                Debug.Log("Normal cast");
                vorgonAnimator.SetTrigger("RangeAttack");
                //var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
                //projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
                //projectileObj.GetComponent<MProjectile>().minionControl = this;
                StartCoroutine(WaitForCast(4f));
                break;
            case 1:
                Debug.Log("Doing hellfire");
                canRotate = true;
                vorgonAnimator.SetTrigger("HellFire");
                vorgonAnimator.SetBool("notHellFire", true);
                rainFire = true;
                StartCoroutine(RainFire());
                //StartCoroutine(WaitForCast(15f));
                break;
        }

        canCast = false;
    }

    IEnumerator RainFire()
    {
        //first create one at players position
        GameObject playerMarker1  = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        List<GameObject> points = new List<GameObject>();
        points.Add(playerMarker1);
        objectPositions.Add(playerMarker1.transform.position);

        for (int i = 0; i < numberOfHitPoints; i++)
        {
            Vector3 randomPoint = player.position + Random.insideUnitSphere * hellFireRange;
            randomPoint.y = 100;
            Debug.DrawRay(randomPoint, Vector3.down, Color.red, 100);

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                bool isTooClose = false;

                foreach(Vector3 objPos in objectPositions)
                {
                    if(Vector3.Distance(objPos, hit.point) < minDistanceBetweenObjects)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if(!isTooClose)
                {
                    GameObject obj = Instantiate(fireWarning, hit.point, Quaternion.identity);
                    points.Add(obj);
                    objectPositions.Add(hit.point);
                }

                //Instantiate(fireWarning, hit.point, Quaternion.identity);
            }

        }

        yield return new WaitForSeconds(hellFireSwitchTime);
        for (int i = 0; i < points.Count; i++)
            Destroy(points[i]);


        objectPositions.Clear();
        points.Clear();

        GameObject playerMarker2 = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        //List<GameObject> points = new List<GameObject>();
        points.Add(playerMarker2);
        objectPositions.Add(playerMarker2.transform.position);


        for (int i = 0; i < numberOfHitPoints; i++)
        {
            Vector3 randomPoint = player.position + Random.insideUnitSphere * hellFireRange;
            randomPoint.y = 100;
            Debug.DrawRay(randomPoint, Vector3.down, Color.red, 100);

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                bool isTooClose = false;

                foreach (Vector3 objPos in objectPositions)
                {
                    if (Vector3.Distance(objPos, hit.point) < minDistanceBetweenObjects)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    GameObject obj = Instantiate(fireWarning, hit.point, Quaternion.identity);
                    points.Add(obj);
                    objectPositions.Add(hit.point);
                }

                //Instantiate(fireWarning, hit.point, Quaternion.identity);
            }

        }

        yield return new WaitForSeconds(hellFireSwitchTime);
        for (int i = 0; i < points.Count; i++)
            Destroy(points[i]);


        objectPositions.Clear();
        points.Clear();

        GameObject playerMarker3 = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        //List<GameObject> points = new List<GameObject>();
        points.Add(playerMarker3);
        objectPositions.Add(playerMarker3.transform.position);

        for (int i = 0; i < numberOfHitPoints; i++)
        {
            Vector3 randomPoint = player.position + Random.insideUnitSphere * hellFireRange;
            randomPoint.y = 100;
            Debug.DrawRay(randomPoint, Vector3.down, Color.red, 100);

            if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                bool isTooClose = false;

                foreach (Vector3 objPos in objectPositions)
                {
                    if (Vector3.Distance(objPos, hit.point) < minDistanceBetweenObjects)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    GameObject obj = Instantiate(fireWarning, hit.point, Quaternion.identity);
                    points.Add(obj);
                    objectPositions.Add(hit.point);
                }

                //Instantiate(fireWarning, hit.point, Quaternion.identity);
            }

        }

        yield return new WaitForSeconds(hellFireSwitchTime);
        for (int i = 0; i < points.Count; i++)
            Destroy(points[i]);


        objectPositions.Clear();
        points.Clear();

        vorgonAnimator.SetBool("notHellFire", false);


        StartCoroutine(WaitForCast(8f));
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
