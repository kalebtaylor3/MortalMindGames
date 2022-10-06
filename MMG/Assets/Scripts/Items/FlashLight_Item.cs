using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight_Item : Item
{

    public GameObject light;
    bool lightActive = false;

    private void Start()
    {
        light.SetActive(false);
    }

    public override void UseItem()
    {
        lightActive = !lightActive;
        base.UseItem();
        if (lightActive)
            light.SetActive(true);
        else
            light.SetActive(false);
    }
}
