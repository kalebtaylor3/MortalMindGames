using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{

    public float currentPlayerHealth = 100f;
    [SerializeField] private float maxPlayerHealth = 100f;
    [SerializeField] private int regenRate = 1;
    private bool canRegen = false;

    [SerializeField] private Image bloodImage = null;

    [SerializeField] private Image hurtImage;
    [SerializeField] private float hurtTimer = 0.5f;


    [SerializeField] private float healCooldown = 3.0f;
    [SerializeField] private float maxHealCooldown = 3.0f;
    [SerializeField] private bool startCooldown = false;

    private static PlayerHealthSystem instance;

    public bool invincible;
    public bool alive = true;

    public Animator deathAnimator;
    public GameObject deathCamera;

    public PlayerController charController;

    public AudioSource damageSource;
    public AudioClip damageClip;

    public static event Action<bool> OnDeath;

    public static PlayerHealthSystem Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Null");
            return instance;
        }
    }

    private void OnEnable()
    {
        CollisionActivation.endingPath += ResetHealth;
        ResetHealth();
        alive = true;
        OnDeath?.Invoke(true);
    }

    private void OnDisable()
    {
        CollisionActivation.endingPath -= ResetHealth;
        deathCamera.SetActive(false);
    }

    private void Awake()
    {
        instance = this;
        OnDeath?.Invoke(true);
    }


    void ResetHealth()
    {
        currentPlayerHealth = maxPlayerHealth;
        UpdateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        if(alive)
        {
            if (currentPlayerHealth <= 0)
            {
                if (!invincible)
                {
                    alive = false;
                    currentPlayerHealth = 0;

                    OnDeath?.Invoke(false);

                    deathAnimator.enabled = true;
                    deathCamera.SetActive(true);

                    deathAnimator.SetTrigger("Death");
                    deathAnimator.ResetTrigger("Alive");


                    GameObject lastRelic = WorldData.Instance.lastPickUpGO;
                    WorldData.Instance.TriggerCheckpoint();
                    WorldData.Instance.happenOnce = false;

                    if (WorldData.Instance.lastPickUpGO != null)
                    {
                        RelicSpawnManager.Instance.RelicPickedUp(WorldData.Instance.lastPickUpGO);
                    }
                    else
                    {
                        lastRelic.SetActive(true);
                    }

                    TpTest.Instance.tpPlayer(WorldData.Instance.pickUpCP);
                    WorldData.Instance.VorgonRealmPlayerDeath();
                }
            }

//            if (!invincible)
//            {
//                if (currentPlayerHealth <= 0)
//                {
//#if UNITY_EDITOR
//                    //UnityEditor.EditorApplication.isPlaying = false;
//#endif
//                }
//            }

            if (startCooldown)
            {
                healCooldown -= Time.deltaTime;
                if (healCooldown <= 0)
                {
                    canRegen = true;
                    startCooldown = false;
                }
            }

            if (canRegen)
            {
                if (currentPlayerHealth <= maxPlayerHealth - 0.01)
                {
                    currentPlayerHealth += Time.deltaTime * regenRate;
                    UpdateHealth();
                }
                else
                {
                    currentPlayerHealth = maxPlayerHealth;
                    healCooldown = maxHealCooldown;
                    canRegen = false;
                }
            }
        }


        if (alive)
            charController.enabled = true;
        else
            charController.enabled = false;

        
    }

    IEnumerator HurtFlash()
    {
        hurtImage.enabled = true;
        yield return new WaitForSeconds(hurtTimer);
        hurtImage.enabled = false;
    }

    void UpdateHealth()
    {
        Color bloodAlpha = bloodImage.color;
        bloodAlpha.a = 1- (currentPlayerHealth / maxPlayerHealth);
        bloodImage.color = bloodAlpha;
    }

    public void TakeDamage()
    {
        if(currentPlayerHealth >= 0)
        {
            canRegen = false;
            StartCoroutine(HurtFlash());
            UpdateHealth();
            healCooldown = maxHealCooldown;
            startCooldown = true;
            damageSource.PlayOneShot(damageClip);
        }
    }

}
