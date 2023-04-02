using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.Video;
using static RelicSpawnManager;

public class WorldData : MonoBehaviour
{

    #region Instance

    public static WorldData Instance { get; private set; }

    public static event Action OnDeath;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);

        lastCollectedRelic = RELIC_TYPE.NONE;
        activePlayerSection = SECTIONS.NONE;
        activeVorgonSection = SECTIONS.NONE;

        pickUpCP = player.transform.position;

        SetCheckpoint();

        StartCoroutine(ResetTime());
        happenOnce = false;

        startingRange1 = stealthDetection.hearingRange1;
        startingRange2 = stealthDetection.hearingRange2;
        startingRange3 = stealthDetection.hearingRange3;
        runDetectionSpeed = stealthDetection.runningDetectionSpeed;
        walkDetectionSpeed = stealthDetection.walkingDetectionSpeed;
        crouchDetectionSpeed = stealthDetection.crouchingDetectionSpeed;

        playerSectionChange = false;
        vorgonSectionChange = false;
        canSeek = false;
    }

    #endregion

    #region Variables

    public enum REALMS { MORTAL = 0, VORGON = 1 };
    public enum SECTIONS
    {
        VORGONREALM = -1, NONE = 0, ONE, TWO, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN,
        ELEVEN, TWELVE, THIRTEEN, FOURTEEN, FIFTEEN, SIXTEEN, SEVENTEEN, EIGHTEEN
    };

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
    public GameObject safeZoneUI;

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
    [SerializeField] List<GameObject> sectionManager = null;

    [SerializeField] public bool playerSectionChange = false;
    [SerializeField] public bool vorgonSectionChange = false;

    [SerializeField] public bool canSeek = false;

    public ConcelableAreaInteractable lastConceal;

    public PlayerCombatController combatInventory;

    public GameObject mortalRealmPPE;
    public GameObject vorgonRealmPPE;

    public GameObject realm1;
    public GameObject realm2;
    public GameObject realm3;
    public GameObject deadWood;

    public int currentTrial = 0;

    public Material vorgonSkybox;
    public Material mortalSkybox;

    public StealthDetection stealthDetection;
    public ConcelableDetection concealDetection;

    public GameObject initialContain;

    [HideInInspector] public bool happenOnce = false;

    float startingRange1;
    float startingRange2;
    float startingRange3;

    float runDetectionSpeed;
    float walkDetectionSpeed;
    float crouchDetectionSpeed;

    #endregion

    [Header("DEBUG")]
    [SerializeField] List<Transform> RelicPos = new List<Transform>();

    [SerializeField]
    public List<GameObject> VorgonCharacterSpots;

    bool concelableTutorial = false;
    [HideInInspector] public bool stealthTutorial = false;
    [HideInInspector] public bool trapTutorial = false;
    public TutorialTrigger concelableTut;
    public TutorialTrigger trapTut;
    public TutorialTrigger stealthTut;

    public GameObject heartBeat;

    public VideoClip deathClip;

    public CameraController cameraC;

    private void OnEnable()
    {
        LoreInteractable.OnCollect += CameraControl;
        LoreInteractable.OnPutDown += CameraControl;
    }

    private void OnDisable()
    {
        LoreInteractable.OnCollect -= CameraControl;
        LoreInteractable.OnPutDown -= CameraControl;
    }

    void CameraControl(bool value)
    {
        cameraC.enabled = !value;
        player.enabled = !value;
    }


    private void Update()
    {

        if(lastConceal != null && !concelableTutorial)
        {
            TutorialController.instance.SetTutorial(concelableTut.imageTut, concelableTut.vidTut, 4);
            concelableTutorial = true;
        }


        if (activeRealm == REALMS.MORTAL)
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

        if (lastCollectedRelic == RELIC_TYPE.NONE || lastCollectedRelic == RELIC_TYPE.MAP)
        {
            if (!happenOnce)
            {
                //1 sections to be enabled
                //default vorgon detection values
                for (int i = 0; i < sectionManager.Count; i++)
                {
                    sectionManager[i].SetActive(false);
                }
                sectionManager[0].SetActive(true);
                stealthDetection.hearingRange1 = startingRange1;
                stealthDetection.hearingRange2 = startingRange2;
                stealthDetection.hearingRange3 = startingRange3;
                stealthDetection.jumpScare = false;
                stealthDetection.flashing = false;
                happenOnce = true;
                //setup difficulty progressions (awarness values)
            }
        }
        if (lastCollectedRelic == RELIC_TYPE.FLAMES)
        {
            if (!happenOnce)
            {
                //2 sections to be enabled
                //default vorgon detection values
                for (int i = 0; i < sectionManager.Count; i++)
                {
                    sectionManager[i].SetActive(false);
                }
                sectionManager[1].SetActive(true);
                stealthDetection.hearingRange1 = 45;
                stealthDetection.hearingRange2 = 35;
                stealthDetection.jumpScare = false;
                stealthDetection.flashing = false;
                happenOnce = true;
            }
            //setup difficulty progressions (awarness values)
        }
        if (lastCollectedRelic == RELIC_TYPE.WALL)
        {
            if (!happenOnce)
            {
                for (int i = 0; i < sectionManager.Count; i++)
                {
                    sectionManager[i].SetActive(false);
                }
                sectionManager[2].SetActive(true);
                stealthDetection.hearingRange1 = 60;
                stealthDetection.hearingRange2 = 45;
                stealthDetection.jumpScare = false;
                happenOnce = true;
            }

            //setup difficulty progressions (awarness values)
        }

        //Section Change
        if (playerSectionChange)
        {
            SectionChange();
        }

        if (!canSeek)
        {
            if (activePlayerSection == activeVorgonSection)
            {
                playerSectionChange = false;
                vorgonSectionChange = false;
            }
            else
            {
                //activePlayerSection = activeVorgonSection;
            }
        }
    }

    public void PlayerSafeZone(bool flag)
    {
        player.safeZone = flag;

        safeZoneUI.SetActive(!flag);
        heartBeat.SetActive(!flag);
        stealthDetection.detection = 0;
        concealDetection.exposure = 0;
    }

    public void SectionChange()
    {
        if (activePlayerSection != activeVorgonSection)
        {
            float dist = Vector3.Distance(player.transform.position, vorgon.transform.position);

            if (dist > 20f) 
            {
                canSeek = true;
            }
        }
    }

    public void RealmTeleport(bool flag, REALMS realm)
    {
        //Change Realm && Turn Vorgon on/off
        activeRealm = realm;
        vorgon.gameObject.SetActive(flag);
        concealDetection.enabled = flag;
        stealthDetection.enabled = flag;


        //UI
        detectionUI.SetActive(flag);
    }

    public void SetPauseGame(bool state)
    {
        gamePaused = state;

        canvas.SetActive(!state);
    }

    IEnumerator ResetTime()
    {
        Time.timeScale = 0;

        yield return null;

        Time.timeScale = 1;
    }


    #region Mortal Realm Functions


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
        happenOnce = false;

        if (type == RELIC_TYPE.MAP)
        {
            RelicSpawnManager.Instance.RelicPickedUp(go);
            TurnOffContainment();
        }

        switch (type)
        {
            case RELIC_TYPE.FLAMES:
                realm1.SetActive(true);
                realm2.SetActive(false);
                realm3.SetActive(false);

                currentTrial = 1;

                combatInventory.items[0] = true;
                combatInventory.items[1] = false;
                combatInventory.items[2] = false;
                break;
            case RELIC_TYPE.WALL:
                realm1.SetActive(false);
                realm2.SetActive(true);
                realm3.SetActive(false);

                currentTrial = 2;

                combatInventory.items[0] = false;
                combatInventory.items[1] = true;
                combatInventory.items[2] = false;
                break;
            case RELIC_TYPE.SWORD:
                realm1.SetActive(false);
                realm2.SetActive(false);
                realm3.SetActive(true);

                currentTrial = 3;

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
        //TriggerCheckpoint();lol
        OnDeath?.Invoke();
        StartCoroutine(TriggerPlayerDeathMR());
        happenOnce = false;
        //lastCollectedRelic = lastCollectedRelicCP;        
    }

    IEnumerator TriggerPlayerDeathMR()
    {
        input.canMove = false;
        PlayerInventoryController.Instance.CloseBook();
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

            //yield return new WaitForSeconds(1);

            lastConceal.enteranceAnimator.SetTrigger("Enter");

            yield return new WaitForSeconds(1.5f);

            lastConceal.ToggleConcealDeath();
            
            vorgon.playerDead = true;
            vorgon.detection = 0;
            lastConceal.canCreak = false;
            ConcelableDetection.Instance.hearingExposure = 0;
            ConcelableDetection.Instance.exposure = 0;
            ConcelableDetection.Instance.playerDead = true;

            //yield return new WaitForSeconds(1.0f);
            ConcelableDetection.Instance.vorgonKnows = false;
            yield return new WaitForSeconds(((float)deathClip.length));
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
            vorgonModel.SetActive(false);

            //lastConceal.ExitArea();
            //yield return new WaitForSeconds(2.5f);

        }
        else
        {
            playerDeathMR.SetActive(true);            
            vorgon.playerDead = true;
            vorgon.detection = 0;
            player.enabled = false;
            cameraC.enabled = false;
            yield return new WaitForSeconds(((float)deathClip.length));
            vorgonModel.SetActive(false);
            fadeOut.SetActive(true);
            player.enabled = true;
            cameraC.enabled = true;
            TpTest.Instance.MortalRealmDeath(pickUpCP);
            playerDeathMR.SetActive(false);
            yield return new WaitForSeconds(1);
            fadeOut.SetActive(false);
            vorgon.playerDead = false;
        }

        //lastConceal.enteranceAnimator.SetTrigger("Inside");

        //yield return new WaitForSeconds(1);

        //playerDeathMR.SetActive(false);
        //vorgon.transform.position = WorldData.Instance.FindActiveSection(WorldData.Instance.activePlayerSection).vorgonTP.position;
        vorgonModel.SetActive(true);
        //yield return new WaitForSeconds(1);
        //fadeOut.SetActive(false);

        //playerCam.ResetCam();



        if (lastConceal != null)
        {
            lastConceal.ToggleCamChange();
            lastConceal = null;
        }

        if (player.isHiding)
        {
            player.isHiding = false;
        }

        input.canMove = true;
    }

    public void TurnOffContainment()
    {
        StartCoroutine(TurnOffContain());
    }

    public IEnumerator TurnOffContain()
    {
        yield return new WaitForSeconds(3.5f);
        initialContain.SetActive(false);
    }

    #endregion

    #region Vorgon's Realm Functions

    public void VorgonRealmPlayerDeath()
    {
        happenOnce = false;
        StartCoroutine(VRPlayerDeath());
    }

    IEnumerator VRPlayerDeath()
    {
        yield return new WaitForSeconds(3.1f);

        switch (currentTrial)
        {
            case 1:
                realm1.GetComponent<VorgonTrial>().RestartTrial();
                break;
            case 2:
                realm2.GetComponent<VorgonTrial>().RestartTrial();
                break;
            case 3:
                realm3.GetComponent<VorgonTrial>().RestartTrial();
                break;
            default:
                break;
        }
    }

    #endregion


    // DEBUG

    public void TurnVorgonOff(bool flag)
    {
        vorgon.gameObject.SetActive(flag);
    }

    public void GodMode(bool flag)
    {
        PlayerHealthSystem.Instance.invincible = flag;
    }

    public void RelicTpDEBUG(int relic)
    {
        //Time.timeScale = 1;
        TpTest.Instance.MortalRealmDeath(RelicPos[relic].position);
        //Time.timeScale = 0;
    }

    public void PathTpDEBUG(int relic)
    {
        //Time.timeScale = 1;
        switch (relic)
        {
            case 0:

                TpTest.Instance.tpPlayer(VorgonCharacterSpots[0].transform.position);
                ItemPickedUp(RELIC_TYPE.FLAMES, player.transform.position);
                break;
            case 1:

                TpTest.Instance.tpPlayer(VorgonCharacterSpots[1].transform.position);
                ItemPickedUp(RELIC_TYPE.WALL, player.transform.position);
                break;
            case 2:
                TpTest.Instance.tpPlayer(VorgonCharacterSpots[2].transform.position);
                ItemPickedUp(RELIC_TYPE.SWORD, player.transform.position);
                break;
        }
        //Time.timeScale = 0;
    }
}
