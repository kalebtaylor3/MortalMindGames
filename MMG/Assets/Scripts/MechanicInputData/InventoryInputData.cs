using UnityEngine;
using MMG;

[CreateAssetMenu(fileName = "InventoryInputData", menuName = "MMG_Player/Data/InventoryInputData")]
public class InventoryInputData : ScriptableObject
{
    private bool item1ChangeClick;
    private bool item2ChangeClick;
    private bool item3ChangeClick;
    private bool item4ChangeClick;
    private bool putItemAway;

    public bool Change1Clicked
    {
        get => item1ChangeClick;
        set => item1ChangeClick = value;
    }

    public bool Change2Clicked
    {
        get => item2ChangeClick;
        set => item2ChangeClick = value;
    }

    public bool Change3Clicked
    {
        get => item3ChangeClick;
        set => item3ChangeClick = value;
    }

    public bool Change4Clicked
    {
        get => item4ChangeClick;
        set => item4ChangeClick = value;
    }

    public bool PutItemAway
    {
        get => putItemAway;
        set => putItemAway = value;
    }
}
