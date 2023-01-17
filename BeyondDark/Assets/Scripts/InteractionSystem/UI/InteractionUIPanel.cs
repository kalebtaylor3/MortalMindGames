using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MMG
{    
    public class InteractionUIPanel : MonoBehaviour
    {
        #region Instance

        public static InteractionUIPanel Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) { Instance = this; }
            else if (Instance != this) Destroy(gameObject);
        }

        #endregion


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

        //STAMINA
        public void UpdateStaminaSlider(float stamina)
        {
            staminaSlider.value = stamina;
        }

    }
}
