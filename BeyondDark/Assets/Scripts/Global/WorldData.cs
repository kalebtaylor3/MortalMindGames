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
    public PlayerController player;
    public bool gamePaused = false;

    //UI
    public GameObject detectionUI;
    public GameObject canvas;

    // FOR CHECKPOINT
    private RelicSpawnManager.RELIC_TYPE lastCollectedRelicCP = RELIC_TYPE.NONE;    
    private int collectedRelicsCountCP;
    private GameObject lastPickUpGOCP;

    [SerializeField] InputController input;
    [SerializeField] GameObject playerDeathMR;
    [SerializeField] GameObject vorgonModel;
    [SerializeField] public GameObject fadeOut;
    [SerializeField] public CameraController playerCam;

    public VorgonController vorgon;

    // FOR AI
    [SerializeField] List<Section> sections = null;
    public ConcelableAreaInteractable lastConceal;

    public PlayerCombatController combatInventory;

    public GameObject mortalRealmPPE;
    public GameObject vorgonRealmPPE;

    public GameObject realm1;
    public GameObject realm2;
    public GameObject realm3;
    public GameObject deadWood;

    public Material vorgonSkybox;
    public Material mortalSkybox;

    #endregion

    private void Update()
    {

        if(activeRealm == REALMS.MORTAL)
        {
            deadWood.SetActive(true);
            RenderSettings.skybox = mortalSkybox;
            mortalRealmPPE.SetActive(true);
            vorgonRealmPPE.SetActive(false);
        }
        else
        {
            RenderSettings.skybox = vorgonSkybox;
            deadWood.SetActive(false);
            vorgonRealmPPE.SetActive(true);
            mortalRealmPPE.SetActive(false);
        }
    }

    public void RealmTeleport(bool flag, REALMS realm)
    {
        //Change Realm && Turn Vorgon on/off
        activeRealm = realm;
        vorgon.gameObject.SetActive(flag);

        //UI
        detectionUI.SetActive(flag);
    }

    public void SetPauseGame(bool state)
    {
        gamePaused = state;

        canvas.SetActive(!state);
    }


    #region Mortal Realm Functions

    private void Start()
    {
        lastCollectedRelic = RELIC_TYPE.NONE;
        activePlayerSection = SECTIONS.NONE;
        activeVorgonSection = SECTIONS.NONE;

        pickUpCP = player.transform.position;

        SetCheckpoint();
    }

    public Section FindActiveSection(SECTIONS section)
    {
        Section currentSection = sections.Find((s) => s.sectionType == section);
        return currentSection;
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

        switch(type)
        {
            case RELIC_TYPE.FLAMES:
                realm1.SetActive(true);
                combatInventory.items[0] = true;
                combatInventory.items[1] = false;
                combatInventory.items[2] = false;
                break;
            case RELIC_TYPE.WALL:
                realm2.SetActive(true);
                combatInventory.items[0] = false;
                combatInventory.items[1] = true;
                combatInventory.items[2] = false;
                break;
            case RELIC_TYPE.SWORD:
                realm3.SetActive(true);
                combatInventory.items[0] = false;
                combatInventory.items[1] = false;
                combatInventory.items[2] = true;
                break;
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

    public void PlayerDeathMortalRealm()
    {
        TriggerCheckpoint();
        StartCoroutine(TriggerPlayerDeathMR());
    }

    IEnumerator TriggerPlayerDeathMR()
    {
        input.canMove = false;
        //lastConceal.enteranceAnimator.SetTrigger("Enter");

        if (lastConceal != null && player.isHiding)
        {
            //dont call exit. first check if door is open. if door is open use the animator to play the open animation. wait for lenght of animation. then set jumpscare active then wait for lenght wait for for how ever many seconds are before fade to black. Before fade to black starts switch the cameras
            //lastConceal.enteranceAnimator.SetTrigger("Enter");
            //yield return new WaitForSeconds(lastConceal.enteranceAnimator.GetCurrentAnimatorClipInfo(0).Length);
            //playerDeathMR.SetActive(true);
            lastConceal.enteranceAnimator.enabled = true;
            lastConceal.canCreak = false;
            lastConceal.lookAtTransform.position = lastConceal.lookAtStartPosition;
            lastConceal.doorCreak.enabled = false;
            ConcelableDetection.Instance.seeingCanvas.gameObject.SetActive(false);
            vorgon.sightCanvas.gameObject.SetActive(false);

            yield return new WaitForSeconds(1);

            lastConceal.enteranceAnimator.SetTrigger("Enter");

            yield return new WaitForSeconds(2);

            lastConceal.ToggleConcealDeath();
            vorgonModel.SetActive(false);
            vorgon.playerDead = true;
            vorgon.detection = 0;
            lastConceal.canCreak = false;
            ConcelableDetection.Instance.hearingExposure = 0;
            ConcelableDetection.Instance.exposure = 0;
            ConcelableDetection.Instance.playerDead = true;

            yield return new WaitForSeconds(1.0f);
            ConcelableDetection.Instance.vorgonKnows = false;
            fadeOut.SetActive(true);
            lastConceal.ToggleCamChange();
            TpTest.Instance.MortalRealmDeath(pickUpCP);            
            yield return new WaitForSeconds(4);
            fadeOut.SetActive(false);
            vorgon.playerDead = false;
            ConcelableDetection.Instance.playerDead = false;

            lastConceal.enteranceAnimator.SetTrigger("Inside");

            lastConceal.Rest();
            lastConceal.doorCreak.enabled = true;
            ConcelableDetection.Instance.seeingCanvas.gameObject.SetActive(true);
            vorgon.sightCanvas.gameObject.SetActive(true);

            //lastConceal.ExitArea();
            //yield return new WaitForSeconds(2.5f);

        }
        else
        {
            playerDeathMR.SetActive(true);
            vorgonModel.SetActive(false);
            vorgon.playerDead = true;
            vorgon.detection = 0;
            yield return new WaitForSeconds(1);
            fadeOut.SetActive(true);
            TpTest.Instance.MortalRealmDeath(pickUpCP);            
            playerDeathMR.SetActive(false);
            yield return new WaitForSeconds(1);
            fadeOut.SetActive(false);
            vorgon.playerDead = false;
        }

        //lastConceal.enteranceAnimator.SetTrigger("Inside");

        //yield return new WaitForSeconds(1);

        //playerDeathMR.SetActive(false);
        vorgon.transform.position = WorldData.Instance.FindActiveSection(WorldData.Instance.activePlayerSection).vorgonTP.position;
        vorgonModel.SetActive(true);
        //yield return new WaitForSeconds(1);
        //fadeOut.SetActive(false);

        //playerCam.ResetCam();

        if(lastConceal != null)
        {
            lastConceal.ToggleCamChange();
            lastConceal = null;
        }
        
        
        input.canMove = true;
    }

    #endregion

    #region Vorgon's Realm Functions



    #endregion
}
