using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Stamina
    [SerializeField] private Slider staminaSlider;


    //STAMINA
    public void UpdateStaminaSlider(float stamina)
    {
        staminaSlider.value = stamina;
    }
}
