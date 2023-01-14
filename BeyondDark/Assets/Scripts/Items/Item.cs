using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // RELIC TYPE, default to none
    public RelicSpawnManager.RELIC_TYPE relicType = RelicSpawnManager.RELIC_TYPE.NONE;

    public string itemName;
    public int maxUses;

    public virtual void UseItem()
    {
        Debug.Log("used" + itemName);
    }
}
