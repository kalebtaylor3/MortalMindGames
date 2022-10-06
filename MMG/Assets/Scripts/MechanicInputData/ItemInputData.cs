using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInputData", menuName = "FirstPersonController/Data/ItemInputData")]
public class ItemInputData : ScriptableObject
{
    private bool useItemClick;
    private string itemType;

    public bool UseItemClick
    {
        get => useItemClick;
        set => useItemClick = value;
    }

}
