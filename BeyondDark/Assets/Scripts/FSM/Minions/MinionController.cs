using MMG;
using System;
//using System;
using System.Collections;
using System.Collections.Generic;
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

    public float rangedAttackDuration = 2.0f;
    public float MeleeAttackDuration = 2.0f;

    public bool foundWall = false;
    public Transform currentWall = null;

    public bool canTakeSwordDamage = true;

    public Transform bloodSpawn;
    public GameObject blood;

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

        // 
    }

    IEnumerator SpawnMinion()
    {
        // Change to wait for animation
        yield return new WaitForSeconds(3f);
        spawning = false;
    }

    void HandleHP()
    {
        // If hp 0 destroy minion
        if (healthPoints <= 0)
        {
            healthPoints = 0;
            minionDeath = true;
            OnDeath?.Invoke(this);
            //CHANGE TO COROUTINE FOR ANIMATION
            Destroy(gameObject, 1.5f);
        }
        //Set hp to UI
        healthUI.value = healthPoints;
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

                GameObject obj = Instantiate(blood);
                obj.transform.position = bloodSpawn.position;
                obj.transform.LookAt(Camera.main.transform);

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
        var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
        projectileObj.GetComponent<MProjectile>().minionControl = this;
    }

    public void RangedReposition(bool safeFlag)
    {
        safe = safeFlag;

        if(!safe)
        {
            var projectileObj = Instantiate(acidProjectile, shootPos.position, Quaternion.identity) as GameObject;
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
