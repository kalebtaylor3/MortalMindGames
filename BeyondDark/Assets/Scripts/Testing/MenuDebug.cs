using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDebug : MonoBehaviour
{
    public Toggle godModeTogg;
    public Toggle vorgonTogg;

    private void Awake()
    {
        if(!MenuManager.Instance.mainMenu)
        {
            //godModeTogg.isOn = PlayerHealthSystem.Instance.invincible;
            vorgonTogg.isOn = WorldData.Instance.vorgon.gameObject.activeSelf;
        }        
    }

    public void TurnVorgonOff()
    {
        WorldData.Instance.TurnVorgonOff(vorgonTogg.isOn);
    }

    public void GodMode()
    {
        WorldData.Instance.GodMode(godModeTogg.isOn);
    }
}
