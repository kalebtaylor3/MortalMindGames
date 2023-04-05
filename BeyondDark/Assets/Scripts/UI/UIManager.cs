using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Stamina
    [SerializeField] private Slider staminaSlider;

    [SerializeField] private Image fillImage;    

    public float fadeTimer = 0.5f;

    public bool fading = false;
    public bool fadedOut = true;

    private void OnEnable()
    {
        //staminaSlider = GetComponent<Slider>();
    }

    private void Update()
    {
        if(WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            if (!staminaSlider.gameObject.activeSelf && !WorldData.Instance.playerDeath)
            {
                staminaSlider.gameObject.SetActive(true);
            }
            else if(WorldData.Instance.playerDeath)
            {
                Color newC = fillImage.color;

                newC.a = 0f;

                fillImage.color = newC;
            }


            if (staminaSlider.value == 1 && !fading && !fadedOut)
            {
                fading = true;
                StopAllCoroutines();
                StartCoroutine(FadeSlider(false, fillImage.color.a));
            }
            else if (staminaSlider.value < 1 && !fading && fadedOut)
            {
                if (WorldData.Instance.player.movementInputData.IsRunning)
                {
                    fading = true;
                    StopAllCoroutines();
                    StartCoroutine(FadeSlider(true, fillImage.color.a));
                }
            }

            if (staminaSlider.value < 1 && fading && !fadedOut && !WorldData.Instance.player.movementInputData.IsRunning)
            {
                fading = true;
                StopAllCoroutines();
                StartCoroutine(FadeSlider(false, fillImage.color.a));
            }
        }
        else
        {
            if (staminaSlider.gameObject.activeSelf)
            {
                staminaSlider.gameObject.SetActive(false);
            }
        }        
    }


    public IEnumerator FadeSlider(bool flag, float f = 1)
    {
        float clockT = 0f;

        Color newC = fillImage.color;

        if (flag)
        {
            fadedOut = false;
            while (clockT < fadeTimer)
            {
                newC.a = Mathf.Lerp(0f, 1f, clockT / fadeTimer);

                clockT += Time.deltaTime;
                clockT = Mathf.Clamp(clockT, 0, fadeTimer);
                yield return null;
                fillImage.color = newC;
            }

            newC.a = 1f;

            fillImage.color = newC;
        }
        else
        {
            fadedOut = true;
            while (clockT < fadeTimer)
            {
                newC.a = Mathf.Lerp(f, 0f, clockT / fadeTimer);

                clockT += Time.deltaTime;
                clockT = Mathf.Clamp(clockT, 0, fadeTimer);
                yield return null;
                fillImage.color = newC;
            }

            newC.a = 0f;

            fillImage.color = newC;
        }

        fading = false;
    }

    //STAMINA
    public void UpdateStaminaSlider(float stamina)
    {
        staminaSlider.value = stamina;
    }
}
