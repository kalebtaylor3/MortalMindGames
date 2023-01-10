using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MMG
{        
    public class InteractionUIPanel : MonoBehaviour
    {
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI tooltipText;

        //Stamina
        [SerializeField] private Slider staminaSlider;


        public void SetDisplayText(string displayText)
        {
            tooltipText.SetText(displayText);
        }

        public void UpdateProgressBar(float fillAmount)
        {
            progressBar.fillAmount = fillAmount;
        }

        public void ResetUI()
        {
            progressBar.fillAmount = 0f;
            tooltipText.SetText("");
        }

        //STAMINA NAIM
        public void UpdateStaminaSlider(float stamina)
        {
            staminaSlider.value = stamina;
        }

    }
}
