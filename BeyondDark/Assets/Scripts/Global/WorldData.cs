using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RelicSpawnManager;

public class WorldData : MonoBehaviour
{

    #region Instance

    public static WorldData Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);       
    }

    #endregion


    #region Variables


    public RelicSpawnManager.RELIC_TYPE lastCollectedRelic = RELIC_TYPE.NONE;
    public int collectedRelicsCount;
    public GameObject lastPickUpGO;

    public enum REALMS { MORTAL = 0, VORGON = 1 };

    public REALMS activeRealm = REALMS.MORTAL;

    #endregion

    #region Functions

    private void Start()
    {
        lastCollectedRelic = RELIC_TYPE.NONE;
    }

    /*
     * Last Collected Relic = Pick Up Item ID
     *  0 = Map of Deadwood
     *  1 = Relic 1
     *  2 = Relic 2
     *  3 = Relic 3
     *  We can later add the names of the relics instead of just ids if we want/need to
    */

    public void ItemPickedUp(RelicSpawnManager.RELIC_TYPE type, GameObject go = null)
    {
        // Will show what relic was last collected, we can use this for checkpoints
        lastCollectedRelic = type;
        lastPickUpGO = go;
        collectedRelicsCount++;       
        
        if(type == RELIC_TYPE.MAP)
        {
            RelicSpawnManager.Instance.RelicPickedUp(go);
        }
    }

    public void SetCheckpoint()
    {

    }

    #endregion
}
