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
            for (int i = 0; i < bossController.activeMinions.Count; i++)
            {
                Destroy(bossController.activeMinions[i].gameObject);
                bossController.activeMinions.Remove(bossController.activeMinions[i]);
            }

            for(int i = 0; i < bossController.activeHellFire.Count; i++)
            {
                Destroy(bossController.activeHellFire[i]);
                bossController.activeHellFire.Remove(bossController.activeHellFire[i]);
            }
            bossController.music.Stop();
        }

        StartCoroutine(WaitForDisable());
    }

    IEnumerator WaitForDisable()
    {
        yield return new WaitForSeconds(2);
        vorgonBoss.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
