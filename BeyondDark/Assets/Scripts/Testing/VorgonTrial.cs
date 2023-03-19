using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VorgonTrial : MonoBehaviour
{
    [SerializeField]
    GameObject PathGO;

    [SerializeField]
    public List<MSpawner> MSpawners = new List<MSpawner>();

    public bool bossTrial;
    public VorgonBossController bossController;

    public GameObject vorgonBoss;

    public void RestartTrial()
    {
        if (MSpawners.Count != 0)
        {
            foreach (var spawn in MSpawners)
            {
                spawn.RestartSpawner();
            }
        }

        if(bossTrial)
        {
            vorgonBoss.SetActive(false);
        }

        this.gameObject.SetActive(false);
    }
}
