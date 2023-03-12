using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonTrial : MonoBehaviour
{
    [SerializeField]
    GameObject PathGO;

    [SerializeField]
    public List<MSpawner> MSpawners = new List<MSpawner>();

    public void RestartTrial()
    {
        if (MSpawners.Count != 0)
        {
            foreach (var spawn in MSpawners)
            {
                spawn.RestartSpawner();
            }
        }

        this.gameObject.SetActive(false);
    }
}
