using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LoreInputData", menuName = "MMG_Player/Data/LoreInputData")]
public class LoreInputData : ScriptableObject
{
    private bool rotateRight;
    private bool rotateLeft;

    private bool putAway = true;

    public bool RotateRight
    {
        get => rotateRight;
        set => rotateRight = value;
    }

    public bool RotateLeft
    {
        get => rotateLeft;
        set => rotateLeft = value;
    }

    public bool PutAway
    {
        get => putAway;
        set => putAway = value;
    }
}
