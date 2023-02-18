using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{

    public float currentPlayerHealth = 100f;
    [SerializeField] private float maxPlaerHealth = 100f;
    [SerializeField] private int regenRate = 1;
    private bool canRegen = false;

    [SerializeField] private Image bloodImage = null;

    [SerializeField] private Image hurtImage;
    [SerializeField] private float hurtTimer = 0.5f;


    [SerializeField] private float healCooldown = 3.0f;
    [SerializeField] private float maxHealCooldown = 3.0f;
    [SerializeField] private bool startCooldown = false;

    private static PlayerHealthSystem instance;

    public static PlayerHealthSystem Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Null");
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(currentPlayerHealth <= 0)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }

        if(startCooldown)
        {
            healCooldown -= Time.deltaTime;
            if(healCooldown <= 0)
            {
                canRegen = true;
                startCooldown = false;
            }
        }

        if(canRegen)
        {
            if(currentPlayerHealth <= maxPlaerHealth - 0.01)
            {
                currentPlayerHealth += Time.deltaTime * regenRate;
                UpdateHealth();
            }
            else
            {
                currentPlayerHealth = maxPlaerHealth;
                healCooldown = maxHealCooldown;
                canRegen = false;
            }
        }
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
        float value = currentPlayerHealth / 100;
        Debug.Log(value);
        bloodAlpha.a = 1- value;
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
        }
    }

}
