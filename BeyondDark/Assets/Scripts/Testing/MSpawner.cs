using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSpawner : MonoBehaviour
{
    [SerializeField] MinionController.MINION_TYPE spawnType = MinionController.MINION_TYPE.MELEE;
    [SerializeField] float delay = 2.0f;
    [SerializeField] GameObject minionsGO;

    private bool spawned = false;
    private void OnEnable()
    {
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

                var spawnedMinion = Instantiate(minionsGO, this.transform.position + new Vector3(rand1, this.transform.position.y, rand2), Quaternion.identity) as GameObject;
                spawnedMinion.GetComponent<MinionController>().type = spawnType;

                yield return new WaitUntil(() => !spawnedMinion.GetComponent<MinionController>().spawning);
                yield return new WaitForSeconds(delay);

                spawned = false;
            }
        } 
    }
}
