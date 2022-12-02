using UnityEngine;
using MMG;

[CreateAssetMenu(fileName = "InventoryInputData", menuName = "MMG_Player/Data/InventoryInputData")]
public class InventoryInputData : ScriptableObject
{
    private bool openSpellBook;

    public bool OpenSpellBook
    {
        get => openSpellBook;
        set => openSpellBook = value;
    }
}
