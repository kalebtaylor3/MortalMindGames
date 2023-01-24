using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QTE_InputData", menuName = "MMG_Player/Data/QuickTimeEventInputData")]
public class QuickTimeEventInputData : ScriptableObject
{
    private bool successKeyPressed;

    public bool SuccessKeyPressed
    {
        get => successKeyPressed;
        set => successKeyPressed = value;
    }
}
