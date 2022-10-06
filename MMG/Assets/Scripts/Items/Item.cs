using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{

    public enum itemType
    {
        Flashlight,
        Sword,
        Pitchfork
    }

    public string itemName;
    public itemType type;
    public int maxUses;

    public virtual void UseItem()
    {
        Debug.Log("used" + itemName);
    }
}
