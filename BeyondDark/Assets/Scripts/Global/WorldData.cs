using MMG;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

    public enum REALMS { MORTAL = 0, VORGON = 1 };
    public enum SECTIONS { VORGONREALM = -1, NONE = 0, FIRST = 1, SECOND, THIRD, FOURTH };

    // DATA
    public RelicSpawnManager.RELIC_TYPE lastCollectedRelic = RELIC_TYPE.NONE;
    public REALMS activeRealm = REALMS.MORTAL;
    public int collectedRelicsCount;
    public GameObject lastPickUpGO = null;
    public Vector3 pickUpCP;
    public PlayerController MortalRealmController;
    public SECTIONS activePlayerSection;
    public SECTIONS activeVorgonSection;
    public Section ActiveVorgonSection;

    // FOR CHECKPOINT
    private RelicSpawnManager.RELIC_TYPE lastCollectedRelicCP = RELIC_TYPE.NONE;    
    private int collectedRelicsCountCP;
    private GameObject lastPickUpGOCP;

    // FOR AI
    [SerializeField] List<Section> sections = null;
    [SerializeField] List<Transform> Section01Waypoints = null;
    [SerializeField] List<Transform> Section02Waypoints = null;
    [SerializeField] List<Transform> Section03Waypoints = null;
    [SerializeField] List<Transform> Section04Waypoints = null;


    #endregion

    #region Functions

    private void Start()
    {
        lastCollectedRelic = RELIC_TYPE.NONE;
        activePlayerSection = SECTIONS.NONE;
        activeVorgonSection = SECTIONS.NONE;
    }

    public Vector3 FindSectionCenter(SECTIONS section)
    {
        Vector3 sectionCenter = (sections.Find((s) => s.sectionType == section)).transform.position;
        return sectionCenter;
    }

    /*
     * Last Collected Relic = Pick Up Item ID
     *  0 = Map of Deadwood
     *  1 = Relic 1
     *  2 = Relic 2
     *  3 = Relic 3
     *  We can later add the names of the relics instead of just ids if we want/need to
    */

    public void ItemPickedUp(RelicSpawnManager.RELIC_TYPE type, Vector3 playerPos, GameObject go = null)
    {
        // Will show what relic was last collected, we can use this for checkpoints
        lastCollectedRelic = type;
        lastPickUpGO = go;
        collectedRelicsCount++;
        pickUpCP = playerPos;
        
        if(type == RELIC_TYPE.MAP)
        {
            RelicSpawnManager.Instance.RelicPickedUp(go);
        }
    }

    public void SetCheckpoint()
    {
        lastCollectedRelicCP = lastCollectedRelic;
        collectedRelicsCountCP = collectedRelicsCount;
        lastPickUpGOCP = lastPickUpGO;
    }

    public void TriggerCheckpoint()
    {
        lastCollectedRelic = lastCollectedRelicCP;
        collectedRelicsCount = collectedRelicsCountCP;
        lastPickUpGO = lastPickUpGOCP;
    }

    #endregion
}
