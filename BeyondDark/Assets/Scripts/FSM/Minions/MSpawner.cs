using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MSpawner : MonoBehaviour
{
    enum SPAWNER_TYPE { REGULAR, CUSTOM };

    [SerializeField] SPAWNER_TYPE type = SPAWNER_TYPE.REGULAR;
    [SerializeField] List<Transform> MspawnPos = new List<Transform>();
    [SerializeField] List<Transform> RspawnPos = new List<Transform>();
    [SerializeField] MinionController.MINION_TYPE spawnType = MinionController.MINION_TYPE.MELEE;
    [SerializeField] float delay = 2.0f;
    [SerializeField] GameObject minionsGO;

    [SerializeField] GameObject Gate;


    // Default SpawnPos
    Transform spawnPos;

    private bool spawned = false;
    private void OnEnable()
    {        
        if(type== SPAWNER_TYPE.REGULAR)
        {
            spawnPos = this.transform;
        }

        spawned = false;
        StartCoroutine(ActivateSpawner());
    }

    // Check if there is spawned minions left (CUSTOM ONLY)
    public bool CheckMinions()
    {
        foreach (Transform Mpos in MspawnPos)
        {
            if(Mpos.childCount != 0)
            {
                return false;
            }
        }

        foreach (Transform Rpos in RspawnPos)
        {
            if (Rpos.childCount != 0)
            {
                return false;
            }
        }
           
        return true;
    }

    IEnumerator ActivateSpawner()
    {
        if (type == SPAWNER_TYPE.REGULAR)
        {
            while (true)
            {
                if (!spawned)
                {
                    spawned = true;
                    //Wait for animation


                    float rand1 = Random.Range(0, 2);
                    float rand2 = Random.Range(0, 2);

                    var spawnedMinion = Instantiate(minionsGO, spawnPos.position + new Vector3(rand1, spawnPos.position.y, rand2), Quaternion.identity) as GameObject;
                    spawnedMinion.GetComponent<MinionController>().type = spawnType;

                    yield return new WaitUntil(() => !spawnedMinion.GetComponent<MinionController>().spawning);
                    yield return new WaitForSeconds(delay);

                    spawned = false;

                }
            }
        }
        else if (type == SPAWNER_TYPE.CUSTOM) 
        {
            if(MspawnPos.Count !=0)
            {
                foreach (Transform Mpos in MspawnPos)
                {
                    var spawnedMinion = Instantiate(minionsGO, Mpos.position, Quaternion.identity, Mpos) as GameObject;
                    spawnedMinion.transform.localScale = new Vector3(1f, 1f, 1f);
                    spawnedMinion.GetComponent<MinionController>().type = MinionController.MINION_TYPE.MELEE;
                }
            }
            
            if(RspawnPos.Count != 0)
            {
                foreach (Transform Rpos in RspawnPos)
                {
                    var spawnedMinion = Instantiate(minionsGO, Rpos.position, Quaternion.identity, Rpos) as GameObject;
                    spawnedMinion.transform.localScale = new Vector3(1f, 1f, 1f);
                    spawnedMinion.GetComponent<MinionController>().type = MinionController.MINION_TYPE.RANGED;
                }
            }            
        }
    }
}
