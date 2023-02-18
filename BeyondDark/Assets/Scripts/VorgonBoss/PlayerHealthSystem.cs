using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealthSystem : MonoBehaviour
{

    public float currentPlayerHealth = 100f;
    [SerializeField] private float maxPlaerHealth = 100f;

    [SerializeField] private Image bloodImage = null;

    [SerializeField] private Image hurtImage;
    [SerializeField] private float hurtTimer = 0.5f;



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
        bloodAlpha.a = 1 - (currentPlayerHealth / maxPlaerHealth);
        bloodImage.color = bloodAlpha;
    }

    public void TakeDamage()
    {
        if(currentPlayerHealth >= 0)
        {
            StartCoroutine(HurtFlash());
            UpdateHealth();
        }
    }

}
