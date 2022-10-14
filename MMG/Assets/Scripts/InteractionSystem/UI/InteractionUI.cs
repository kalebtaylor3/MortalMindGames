using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace MMG
{    
    public class InteractionUI : MonoBehaviour
    {
        public Image holdProgressIMG;
        public Image tooltipBG;
        private RectTransform canvasTransform;
        private TextMeshProUGUI interactableDisplayText;

        public void Init()
        {
            GetComponents();
        }

        void GetComponents()
        {
            canvasTransform = GetComponent<RectTransform>();
            interactableDisplayText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetDisplayText(Transform parent , string displayText, float holdProgress)
        {
            if(parent)
            {
                canvasTransform.position = parent.position;
                canvasTransform.SetParent(parent);
            }

            interactableDisplayText.SetText(displayText);
            holdProgressIMG.fillAmount = holdProgress;
        }

        public void SetDisplayTextActiveState(bool state)
        {
            interactableDisplayText.gameObject.SetActive(state);
            holdProgressIMG.gameObject.SetActive(state);
            tooltipBG.gameObject.SetActive(state);
        }

        public void UpdateChargeProgress(float _progress)
        {
            holdProgressIMG.fillAmount = _progress;
        }

        public void LookAtPlayer(Transform _player)
        {
            canvasTransform.LookAt(_player,Vector3.up);
        }

        public void UnparentDisplayText()
        {
            canvasTransform.SetParent(null);
        }

        public bool IsDisplayTextActive()
        {
            return interactableDisplayText.gameObject.activeSelf;
        }

    }
}