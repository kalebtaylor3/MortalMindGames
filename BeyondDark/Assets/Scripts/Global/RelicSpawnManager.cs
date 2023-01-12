using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicSpawnManager : MonoBehaviour
{

    #region Instance

    public static RelicSpawnManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }

    #endregion

    private string spawnPointsTag = "RelicSpawnPoints";

    [SerializeField]
    private List<GameObject> SpawnSpots;

    [SerializeField]
    private List<GameObject> Relics;

    

    private void Start()
    {
        //SpawnSpots.AddRange(GameObject.FindGameObjectsWithTag(spawnPointsTag));
        //SpawnRelics(WorldData.Instance.collectedRelicsCount);
    }


    public void SpawnRelics(int id)
    {
        if (SpawnSpots.Count != 0)
        {
            for (int i = 0; i < SpawnSpots.Count; i++)
            {
                if(Relics[id - 1] != null)
                {
                    GameObject relic = Instantiate(Relics[id - 1], SpawnSpots[i].transform);                    
                }
                else
                {
                    Debug.Log("NullSpawnPoint");
                }
            }
        }
        
    }

    public void RelicPickedUp(GameObject go)
    {
        SpawnSpots.Clear();

        //Destroy(go.transform.parent.gameObject);
        if (go.transform.parent != null)
        {
            Destroy(go.transform.parent.gameObject);
        }


        SpawnSpots.AddRange(GameObject.FindGameObjectsWithTag(spawnPointsTag));

        
        for (int i = 0; i < SpawnSpots.Count; i++)
        {
            if(SpawnSpots[i].transform.childCount != 0)
            {
                Destroy(SpawnSpots[i].transform.GetChild(0).gameObject);
            }
            
        }

        SpawnRelics(WorldData.Instance.collectedRelicsCount);

    }

}
