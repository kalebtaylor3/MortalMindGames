using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public int maxUses;

    public virtual void UseItem()
    {
        Debug.Log("used" + itemName);
    }
}
