using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestUIController : MonoBehaviour
{
    [Range(0.1f, 1.5f)]
    public float PulseFrequency = 0.25f;

    private float rumbleTime = 3f;
    private RumblePattern rumblePattern = RumblePattern.Constant;
    [SerializeField] Rumbler rumbler;
    private int[] timeDropdown = new int[] { 3, 5, 10 };
    private RumblePattern[] rumbleMode = new RumblePattern[] { RumblePattern.Constant, RumblePattern.CollectRellic, RumblePattern.Linear };

    private void Start()
    {
        StartPressed();
    }

    public void SetDurration(int selectedValue)
    {
        rumbleTime = timeDropdown[selectedValue];
    }

    public void SetRumbleMode(int selectedValue)
    {
        rumblePattern = rumbleMode[selectedValue];
    }

    public void StartPressed()
    {
        switch (rumblePattern)
        {
            case RumblePattern.Constant:
                rumbler.RumbleConstant(1, 2, rumbleTime);
                break;
            case RumblePattern.CollectRellic:
                rumbler.RumblePulse(1, 2, PulseFrequency, rumbleTime);
                break;
            case RumblePattern.Linear:
                rumbler.RumbleLinear(1, 1.5f, 2, 2.5f, rumbleTime);
                break;
            default:
                break;
        }
        
    }
    public void StopPressed()
    {
        rumbler.StopRumble();
    }
}
