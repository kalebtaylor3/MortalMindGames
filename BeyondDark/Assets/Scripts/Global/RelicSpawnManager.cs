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

    #region Variables

    public enum RELIC_TYPE : int { NONE = -1, MAP = 0, FLAMES = 1, WALL = 2, SWORD = 3 };

    private string spawnPointsTag = "RelicSpawnPoints";

    [SerializeField]
    private List<GameObject> SpawnSpots;

    [SerializeField]
    private List<GameObject> VorgonCharacterSpots;

    [SerializeField]
    private List<GameObject> Relics;
    #endregion

    #region Functions
    //Spawn the same relic in all possible spawnpoints
    public void SpawnRelics(int id)
    {
        if (SpawnSpots.Count > 1)
        {
            for (int i = 0; i < SpawnSpots.Count; i++)
            {
                if (Relics[id - 1] != null)
                {
                    GameObject relic = Instantiate(Relics.Find((GameObject) => (int)GameObject.GetComponent<PickUp>().relicType == id), SpawnSpots[i].transform);
                    relic.GetComponent<PickUp>().VorgonTpPosition = VorgonCharacterSpots.Find((gameObject) => 
                        (int)gameObject.GetComponent<CharacterTransportLocation>().relicType == id).transform.position;
                }
            }
        }

    }

    // Handles a relic being picked up
    public void RelicPickedUp(GameObject go)
    {
        // Clear the spawnpoints
        SpawnSpots.Clear();

        // Destroy the Spawn Point
        if (go.transform.parent != null)
        {
            Destroy(go.transform.parent.gameObject);
        }

        // Populate spawnpoints again
        SpawnSpots.AddRange(GameObject.FindGameObjectsWithTag(spawnPointsTag));


        // Destroy the previous relics
        for (int i = 0; i < SpawnSpots.Count; i++)
        {
            if (SpawnSpots[i].transform.childCount != 0)
            {
                Destroy(SpawnSpots[i].transform.GetChild(0).gameObject);
            }

        }

        // Spawn next relic
        SpawnRelics((int)WorldData.Instance.lastCollectedRelic + 1);
    }
    #endregion
}
