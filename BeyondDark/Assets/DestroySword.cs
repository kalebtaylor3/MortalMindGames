using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySword : MonoBehaviour
{

    public GameObject sword;

    private void OnEnable()
    {
        Destroy(sword);
    }
}
