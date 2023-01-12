using MMG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public int lastCollectedRelic;
    public int collectedRelicsCount;

    public enum REALMS { MORTAL = 0, VORGON = 1 };

    public REALMS activeRealm = REALMS.MORTAL;

    #endregion

    #region Functions

    private void Start()
    {
        lastCollectedRelic = -1;
    }

    /*
     * Last Collected Relic = Pick Up Item ID
     *  0 = Map of Deadwood
     *  1 = Relic 1
     *  2 = Relic 2
     *  3 = Relic 3
     *  We can later add the names of the relics instead of just ids if we want/need to
    */

    public void ItemPickedUp(int id)
    {
        // Will show what relic was last collected, we can use this for checkpoints
        lastCollectedRelic = id;
        collectedRelicsCount++;        
    }



    #endregion
}
