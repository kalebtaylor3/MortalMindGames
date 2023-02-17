using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawner : MonoBehaviour
{
    enum SPAWNER_TYPE { REGULAR, CUSTOM };

    [SerializeField] SPAWNER_TYPE type = SPAWNER_TYPE.REGULAR;
    [SerializeField] Transform spawnPos = null;
    [SerializeField] MinionController.MINION_TYPE spawnType = MinionController.MINION_TYPE.MELEE;
    [SerializeField] float delay = 2.0f;
    [SerializeField] GameObject minionsGO;
    

    private bool spawned = false;
    private void OnEnable()
    {
        if(type == SPAWNER_TYPE.REGULAR)
        {
            spawnPos.position = this.transform.position;
        }
        
        spawned = false;
        StartCoroutine(ActivateSpawner());
    }

    IEnumerator ActivateSpawner()
    {
        while(true)
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
}
