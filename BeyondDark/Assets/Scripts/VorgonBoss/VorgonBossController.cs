using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject slamIndication;
    public GameObject fireWarning;

    public GameObject projectile;
    public Transform shootPos;

    public float projectileSpeed = 15f;

    public float minZ = -135;
    public float maxZ = -180;

    private float currentZ;
    private float targetZ;

    float distanceToMaintain = 50;

    bool canRotate = true;
    bool canAttack = false;
    bool canCloseSlam = true;
    bool canCast = false;
    bool isCloseSlamming = false;
    bool rainFire = false;
    bool isTaunting = false;
    bool isSummoning = false;

    bool canHellFire = false;
    bool secondPhase = false;

    public Animator vorgonAnimator;

    public LayerMask groundLayer;
    public int numberOfHitPoints;
    public float minDistanceBetweenObjects;

    [HideInInspector] public List<Vector3> objectPositions = new List<Vector3>();


    public float maxHealth;
    public float currentHealth;
    public Animator healthBar;
    public Slider healthBarSlider;
    bool lastPhase = false;
    bool isDeadForLastPhase = false;


    //minions spawning
    bool isSpawningMinions = false;
    bool canSpawnMinions = true;

    public List<Transform> minionSpawns = new List<Transform>();
    public GameObject minion;

    [HideInInspector] public List<MinionController> activeMinions = new List<MinionController>();

    public CinemachineVirtualCamera playerCam;

    public GameObject finalVorgon;
    public Transform finalSpawnPos;

    bool once = false;

    [HideInInspector] public List<GameObject> activeHellFire = new List<GameObject>();

    public AudioSource music;

    public GameObject sword;
    public GameObject canvas;
    public GameObject crossHair;
    public GameObject vorgonModel;



    //if not attacking & no active minions & in second phase. spawn minions

    private void Awake()
    {
        state = State.Idle;
        once = false;
        isSpawningMinions = false;
        canSpawnMinions = true;
        lastPhase = false;
        isDeadForLastPhase = false;
        canRotate = true;
        canAttack = false;
        canCloseSlam = true;
        canCast = false;
        isCloseSlamming = false;
        rainFire = false;
        isTaunting = false;
        isSummoning = false;
        currentHealth = maxHealth;
        canHellFire = false;
        secondPhase = false;
        StopAllCoroutines();
        slamIndication.SetActive(false);
    }

    private void OnEnable()
    {
        Projectile.DealDamage += TakeDamage;
        SwordDamage.DealDamage += TakeDamage;
        MinionController.OnDeath += OnMinionDeath;

        slamIndication.SetActive(false);
        state = State.Idle;
        once = false;
        isSpawningMinions = false;
        canSpawnMinions = true;
        lastPhase = false;
        isDeadForLastPhase = false;
        canRotate = true;
        canAttack = false;
        canCloseSlam = true;
        canCast = false;
        isCloseSlamming = false;
        rainFire = false;
        isTaunting = false;
        isSummoning = false;
        currentHealth = maxHealth;
        canHellFire = false;
        secondPhase = false;
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        Projectile.DealDamage -= TakeDamage;
        SwordDamage.DealDamage -= TakeDamage;
        MinionController.OnDeath -= OnMinionDeath;
        slamIndication.SetActive(false);

        foreach (HellFireController o in Object.FindObjectsOfType<HellFireController>())
        {
            Destroy(o.gameObject);
        }

        foreach (MinionController o in Object.FindObjectsOfType<MinionController>())
        {
            Destroy(o.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (activeMinions.Count <= 2)
            canSpawnMinions = true;

        if (currentHealth <= 800 && !secondPhase)
        {
            Taunt(false);
            secondPhase = true;
        }

        if (currentHealth <= 650)
        {
            //spawn minions
            if(vorgonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !isSpawningMinions && canSpawnMinions)
            {
                isSpawningMinions = true;
                canSpawnMinions = false;
                vorgonAnimator.SetTrigger("Summon");
                Debug.Log("I would have spawned some minions");
                SpawnMinions();
                StartCoroutine(MinionSpawnDelay());
            }
        }

        if(currentHealth <= 250 && !lastPhase)
        {
            vorgonAnimator.SetTrigger("Death");
            lastPhase = true;
            isDeadForLastPhase = true;
            //SpawnMinions();
            healthBarSlider.gameObject.SetActive(false);
            healthBar.SetTrigger("End");
        }

        if(lastPhase)
        {

            for(int i = 0; i < activeMinions.Count; i++)
            {
                activeMinions[i].ReceiveDamage(1000, false);
            }

            for (int i = 0; i < activeHellFire.Count; i++)
            {
                Destroy(activeHellFire[i]);
            }

            //finalVorgon.SetActive(true);
            //vorgonModel.gameObject.SetActive(false);
            StartCoroutine(WaitForEnd());
            //if (activeMinions.Count == 0 && !once)
            //{

            //    Debug.Log("Minions gone time to finish");
            //    finalVorgon.SetActive(true);
            //    StartCoroutine(WaitForEnd());
            //    //playerCam.m_LookAt = vorgon.transform;
            //    //healthBarSlider.gameObject.SetActive(true);
            //    //healthBar.SetTrigger("Start");
            //    //healthBarSlider.value = maxHealth;
            //    once = true;
            //}
        }

        if(isDeadForLastPhase)
        {
            canAttack = false;
            canCast = false;
            canRotate = false;
            canCloseSlam = false;
        }

        if(!lastPhase)
            healthBarSlider.value = currentHealth;

        shootPos.LookAt(player.position);

        if (isCloseSlamming)
        {
            currentZ = maxZ - 5;
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


            if (!isTaunting && !isSummoning)
            {
                GetState();
                Act();
            }
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

    IEnumerator WaitForEnd()
    {
        yield return new WaitForSeconds(4);
        finalVorgon.SetActive(true);
        vorgonModel.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        Destroy(sword);
        canvas.SetActive(false);
        crossHair.SetActive(false);
    }

    void SpawnMinions()
    {

        for (int i = 0; i < minionSpawns.Count; i++) 
        {
            Vector3 pos = new Vector3(minionSpawns[i].transform.position.x, 0.75f, minionSpawns[i].transform.position.z);
            GameObject spawnie = Instantiate(minion, pos, Quaternion.identity);
            
            //spawnie.transform.position = pos;
            activeMinions.Add(spawnie.GetComponent<MinionController>());

            float rand = UnityEngine.Random.Range(0, 2);
            if (rand == 1)
                spawnie.GetComponent<MinionController>().type = MinionController.MINION_TYPE.RANGED;
            else
                spawnie.GetComponent<MinionController>().type = MinionController.MINION_TYPE.MELEE;
        }
    }

    void OnMinionDeath(MinionController minion)
    {
        activeMinions.Remove(minion);
    }

    IEnumerator MinionSpawnDelay()
    {
        yield return new WaitForSeconds(3);
        isSpawningMinions = false;
        vorgonAnimator.ResetTrigger("Summon");
    }


    public void TakeDamage(float amount)
    {
        currentHealth = currentHealth - amount;
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
            //canRotate = true;
            //canAttack = true;
            //check if can do ranged attack. all happen so often. delay will varry on what type of attack it is.
            if (canCast && !isSpawningMinions)
                state = State.RangeAttack;
        }

        if (playerCloseSlam > closeSlamRadius)
        {
            if (playerRange < attackRadius && canAttack && !isCloseSlamming && !isSpawningMinions)
                state = State.Slam;
            else
                canRotate = true;
        }

        //if (playerCloseSlam < closeSlamRadius && canCloseSlam && !isSpawningMinions)
        //    state = State.CloseSlam;
    }

    void Act()
    {
        switch (state)
        {
            case State.RangeAttack:
                if (canCast)
                {
                    if (canHellFire)
                    {
                        int randomNumber = Random.Range(0, 100);
                        if (randomNumber >= 75)
                            RangeAttack(1);
                        else
                            RangeAttack(0);
                    }
                    else
                    {
                        RangeAttack(0);
                    }
                }
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
        //canCast = true;

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

        yield return new WaitForSeconds(8);
        canRotate = true;

        yield return new WaitForSeconds(6);
        canAttack = true;
    }

    IEnumerator WaitForCloseSlam()
    {
        yield return new WaitForSeconds(7);
        canRotate = true;
        isCloseSlamming = false;

        yield return new WaitForSeconds(6);
        canCloseSlam = true;
    }


    void RangeAttack(int attackNom)
    {
        //0 being normal 1 being hell fire
        if (canCast)
        {
            switch (attackNom)
            {
                case 0:
                    Debug.Log("Normal cast");
                    vorgonAnimator.SetTrigger("RangeAttack");
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
        }

        canCast = false;
    }

    IEnumerator RainFire()
    {
        //first create one at players position
        GameObject playerMarker1  = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        List<GameObject> points = new List<GameObject>();
        activeHellFire.Add(playerMarker1);
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
                    activeHellFire.Add(obj);
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
        activeHellFire.Add(playerMarker2);
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
                    activeHellFire.Add(obj);
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
        activeHellFire.Add(playerMarker3);
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
                    activeHellFire.Add(obj);
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

        GameObject playerMarker4 = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        //List<GameObject> points = new List<GameObject>();
        activeHellFire.Add(playerMarker4);
        points.Add(playerMarker4);
        objectPositions.Add(playerMarker4.transform.position);

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
                    activeHellFire.Add(obj);
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

        GameObject playerMarker5 = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        //List<GameObject> points = new List<GameObject>();
        activeHellFire.Add(playerMarker5);
        points.Add(playerMarker5);
        objectPositions.Add(playerMarker5.transform.position);

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
                    activeHellFire.Add(obj);
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

        GameObject playerMarker6 = Instantiate(fireWarning, new Vector3(player.position.x, 0.4f, player.position.z), Quaternion.identity);
        activeHellFire.Add(playerMarker6);
        //List<GameObject> points = new List<GameObject>();
        points.Add(playerMarker6);
        objectPositions.Add(playerMarker6.transform.position);

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
                    activeHellFire.Add(obj);
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
        activeHellFire.Clear();

        StartCoroutine(WaitForCast(4f));
    }

    IEnumerator WaitForCast(float delay)
    {
        yield return new WaitForSeconds(1);
        var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
        //projectileObj.transform.rotation = Quaternion.LookRotation(projectileObj.GetComponent<Rigidbody>().velocity);
        yield return new WaitForSeconds(delay - 1);
        canCast = true;
        rainFire = false;
    }

    public void Taunt(bool firstTaunt)
    {
        isTaunting = true;
        canAttack = false;
        canCast = false;
        vorgonAnimator.SetTrigger("Taunt");
        StartCoroutine(WaitForTaunt(firstTaunt));
    }

    IEnumerator WaitForTaunt(bool firstTaunt)
    {
        yield return new WaitForSeconds(4);
        canCast = true;
        canAttack = true;
        isTaunting = false;

        if (!firstTaunt)
        {
            canHellFire = true;
            RangeAttack(1);
        }

    }

    public void SlamShake()
    {
        CameraShake.Instance.ShakeCamera(5, 5, 1);
        Rumbler.Instance.RumbleConstant(0.5f, 2f, 1);
    }

    public void SummonMinions()
    {
        isSummoning = true;
        //do the same math as hell fire expect create minions at the locations of the markers
        StartCoroutine(WaitForSummon());
    }

    IEnumerator WaitForSummon()
    {
        vorgonAnimator.SetTrigger("Summon");
        yield return new WaitForSeconds(2);
        isSummoning = false;
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
