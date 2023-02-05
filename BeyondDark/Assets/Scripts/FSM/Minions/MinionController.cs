using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    public bool minionDeath = false;
    public bool onFire = false;



    private void OnEnable()
    {
        // Set up hp for ranged or melee

        healthUI.maxValue = healthPoints;
        healthUI.value = healthPoints;
    }

    // Update is called once per frame
    void Update()
    {
        //navAgent.destination = player.transform.position;
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
                ReceiveDamage(5f);
            }
        }
    }
}
