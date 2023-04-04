using MMG;
using System;
//using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
//sing Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class MinionController : MonoBehaviour
{
    public enum MINION_TYPE { MELEE = 0, RANGED = 1 };


    [SerializeField] public MINION_TYPE type;
    [SerializeField] public NavMeshAgent navAgent;
    [SerializeField] public MinionFSM minionFSM;
    //[SerializeField] public PlayerController player;

    [SerializeField] public float healthPoints;
    [SerializeField] public float maxHealthPoints;

    [SerializeField] public PlayerCombatController playerController;
    [SerializeField] public Transform aimAt;
    

    [SerializeField] public Slider healthUI;
    [SerializeField] public GameObject flamesParticle;
    [SerializeField] public GameObject projectile;
    [SerializeField] public GameObject acidProjectile;    
    [SerializeField] public Transform shootPos;
    [SerializeField] public float projectileSpeed;

    public bool spawning = true;
    public bool minionDeath = false;
    public bool onFire = false;
    public float fireDamage = 1;
    public bool isAttacking = false;
    public bool safe = true;

    public float rangedAttackDuration = 0.5f;
    public float MeleeAttackDuration = 2.0f;

    public bool foundWall = false;
    public Transform currentWall = null;

    public bool canTakeSwordDamage = true;

    public Transform bloodSpawn;
    public GameObject blood;
    public GameObject deathBlood;

    [Header("Audio")]
    //Source
    public AudioSource movementSource;
    public AudioSource effectsSource;
    public AudioSource burningSource;

    //Sounds
    public List<AudioClip> clipsWalk;
    public List<AudioClip> clipsHurt;
    public List<AudioClip> clipsttack;
    public AudioClip clipShooting;
    public AudioClip clipAiming;
    public AudioClip clipbreathing; //AIM
    //public AudioClip clipOnfire; // SET UP ON SOURCE
    public AudioClip[] shots;


    public Animator animController;

    public static event Action<MinionController> OnDeath;

    private void OnEnable()
    {
        // Set up hp for ranged or melee

        StopAllCoroutines();

        healthUI.maxValue = healthPoints;
        healthUI.value = healthPoints;

        spawning = true;
        minionDeath = false;
        onFire = false;
        isAttacking = false;

        StartCoroutine(SpawnMinion());

        minionFSM.enabled = true;

        playerController = GameObject.FindGameObjectWithTag("VorgonRealmPlayer").GetComponent<PlayerCombatController>();

        aimAt = playerController.aimAt;

        //player = GameObject.FindGameObjectWithTag("VorgonRealmPlayer").GetComponent<player>;
        MinionDeath.Death += Death;
    }

    private void OnDisable()
    {
        MinionDeath.Death -= Death;
    }

    // Update is called once per frame
    void Update()
    {
        //navAgent.destination = player.transform.position;

        // BURNING
        if(onFire)
        {
            flamesParticle.SetActive(true);
            ReceiveDamage(fireDamage * Time.deltaTime);
        }
        else
        {
            flamesParticle.SetActive(false);
        }
        
        // Wall of Souls Detection
        if(currentWall == false && foundWall)
        {
            foundWall = false;
        }

        // Animations
        animController.SetFloat("Speed", navAgent.speed);

        if(navAgent.isStopped)
        {
            animController.SetBool("IsWalking", false);
        }
        else
        {
            animController.SetBool("IsWalking", true);
        }
    }

    IEnumerator SpawnMinion()
    {
        // Change to wait for animation
        animController.SetBool("Spawning", true);
        navAgent.isStopped = true;
        float rand = UnityEngine.Random.Range(0.5f, 3f);
        yield return new WaitForSeconds(rand);
        navAgent.isStopped = false;
        spawning = false;
        animController.SetBool("Spawning", false);
    }

    void HandleHP()
    {
        // If hp 0 destroy minion
        if (healthPoints <= 0 && !minionDeath)
        {
            healthPoints = 0;
            minionDeath = true;
            animController.SetBool("Dead", true);
            OnDeath?.Invoke(this);
        }

        //Set hp to UI
        healthUI.value = healthPoints;
    }

    public void Death(MinionController minion)
    {
        if(minionDeath)
        {
            // Kill minion
            Destroy(minion.gameObject);

            //Spawn Blood (Boom)
            GameObject obj = Instantiate(deathBlood);
            obj.transform.position = bloodSpawn.position;
            obj.transform.LookAt(Camera.main.transform);
            Destroy(obj, 3.0f);
        }        
    }

    public void ReceiveDamage(float amount, bool sword = false)
    {
        if (canTakeSwordDamage)
        {
            if (!spawning)
            {
                if (amount >= 15)
                {
                    safe = false;
                    StartCoroutine(OnFire());
                }

                if(sword)
                {
                    int rand = UnityEngine.Random.Range(0, clipsHurt.Count);
                    effectsSource.PlayOneShot(clipsHurt[rand]);
                }

                if(!onFire)
                {
                    GameObject obj = Instantiate(blood);
                    obj.transform.position = bloodSpawn.position;
                    obj.transform.LookAt(Camera.main.transform);
                    Destroy(obj, 3.0f);
                }                

                // Low Health
                healthPoints = Mathf.Clamp(healthPoints - amount, 0, maxHealthPoints);

                HandleHP();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            if(other.GetComponent<Projectile>())
            {
                //Apply DAMAGE (TEMP default to 5 for the projectile dont want to touch a work in progress)
                SetOnFireRand();
                ReceiveDamage(other.GetComponent<Projectile>().damage);
            }
        }
        else if(other.CompareTag("WallOfSouls"))
        {
            FoundWallOfSouls(other.gameObject.GetComponent<WallHealthTEMP>().ShotPos);
        }
        else if(other.CompareTag("MassDamage") && safe)
        {
            ReceiveDamage(other.GetComponent<MassDamage>().damage);
        }
    }

    public void RangedAttack()
    {
        // Sound (Move based on timing)
        effectsSource.PlayOneShot(shots[UnityEngine.Random.Range(0, shots.Length)]);

        StartCoroutine(IsAttacking(rangedAttackDuration));
        StartCoroutine(MAttack());
        

    }

    public void MeleeAttack()
    {
        StartCoroutine(MAttack());
        StartCoroutine(IsAttacking(MeleeAttackDuration));
    }

    IEnumerator MAttack()
    {
        animController.SetBool("Attacking", true);

        if (type == MINION_TYPE.MELEE)
        {
            animController.SetTrigger("Attack");
        }
        else
        {
            animController.SetTrigger("RangedAttack");
            yield return new WaitForSeconds(0.18f);
            var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
            projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
            projectileObj.GetComponent<MProjectile>().minionControl = this;
            //projectileObj.transform.rotation = Quaternion.LookRotation(projectileObj.GetComponent<Rigidbody>().velocity + new Vector3(0, 180, 0));
        }
        
        //animController.SetBool("Attacking", true);

        yield return new WaitUntil(() => !isAttacking);
        animController.SetBool("Attacking", false);

    }

    public void RangedReposition(bool safeFlag)
    {
        safe = safeFlag;

        if(!safe)
        {
            var projectileObj = Instantiate(acidProjectile, shootPos.position, Quaternion.identity,shootPos) as GameObject;
            projectileObj.GetComponent<Rigidbody>().AddForce(shootPos.up * 5, ForceMode.Impulse);
            projectileObj.GetComponent<MProjectile>().minionControl = this;
        }
    }


    IEnumerator IsAttacking(float duration)
    {
        isAttacking = true;
        yield return new WaitForSeconds(duration);
        isAttacking = false;
    }

    void SetOnFireRand()
    {
        // Burn (RANDOM)
        int rand = UnityEngine.Random.Range(0, 2);

        if (rand == 1)
        {
            StartCoroutine(OnFire());
        }
    }

    IEnumerator OnFire()
    {
        if(!safe)
        {
            onFire = true;
            //burningSource.Play();

        }
        safe = false;
        
        float rand = UnityEngine.Random.Range(5, 15);
        yield return new WaitForSeconds(rand);
        onFire = false;
        //burningSource.Stop();
        safe = true;
    }

    public void FoundWallOfSouls(Transform wall)
    {
        foundWall = true;
        currentWall = wall;
    }

    public void WallDeath()
    {
        currentWall = null;
        foundWall = false;
    }


    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
