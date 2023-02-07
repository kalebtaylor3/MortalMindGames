using MMG;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public PlayerController player;

    [SerializeField] public float healthPoints;
    [SerializeField] public float maxHealthPoints;
    

    [SerializeField] public Slider healthUI;
    [SerializeField] public GameObject flamesParticle;
    [SerializeField] public GameObject projectile;
    [SerializeField] public Transform shootPos;
    [SerializeField] public float projectileSpeed;
    



    public bool minionDeath = false;
    public bool onFire = false;
    public float fireDamage = 1;
    public bool isAttacking = false;

    public float rangedAttackDuration = 2.0f;
    public float MeleeAttackDuration = 2.0f;


    private void OnEnable()
    {
        // Set up hp for ranged or melee

        StopAllCoroutines();

        healthUI.maxValue = healthPoints;
        healthUI.value = healthPoints;
        minionDeath = false;
        onFire = false;
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        //navAgent.destination = player.transform.position;

        if(onFire)
        {
            flamesParticle.SetActive(true);
            ReceiveDamage(fireDamage * Time.deltaTime);
        }
        else
        {
            flamesParticle.SetActive(false);
        }
    }

    void HandleHP()
    {
        // If hp 0 destroy minion
        if (healthPoints <= 0)
        {
            healthPoints = 0;
            minionDeath = true;
            

            //CHANGE TO COROUTINE FOR ANIMATION
            Destroy(gameObject, 1.5f);
        }
        //Set hp to UI
        healthUI.value = healthPoints;
    }

    public void ReceiveDamage(float amount)
    {
        

        // Lowe Health
        healthPoints = Mathf.Clamp(healthPoints - amount, 0, maxHealthPoints);

        HandleHP();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Bullet"))
        {
            if(other.GetComponent<Projectile>())
            {
                //Apply DAMAGE (TEMP default to 5 for the projectile dont want to touch a work in progress)
                SetOnFireRand();
                ReceiveDamage(5f);
            }
        }
    }

    public void RangedAttack()
    {
        StartCoroutine(IsAttacking(rangedAttackDuration));
        var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
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
        int rand = Random.Range(0, 2);

        if (rand == 1)
        {
            StartCoroutine(OnFire());
        }
    }

    IEnumerator OnFire()
    {
        onFire = true;
        float rand = Random.Range(5, 15);
        yield return new WaitForSeconds(rand);
        onFire = false;
    }


    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
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
