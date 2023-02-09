using Cinemachine;
using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombatController : MonoBehaviour
{
    #region Variables
    [SerializeField] private CombatInputData combatInputData = null;

    public List<bool> items = new List<bool>();


    public Camera cam;
    public GameObject regularProjectile;
    public GameObject specialProjectile;
    public Transform LHFirePoint, RHFirepoint;
    public float projectileSpeed = 70;
    public float fireRate = 4;
    public float wallRate = 8f;
    public float arcRange = 1;



    private Vector3 destination;
    private bool leftHand;
    private float timeToFire;

    private float timeToWall;

    private float placeDelay;
    bool delayOnce = false;


    public GameObject wallMarker;
    private bool isInRange = false;
    private Vector3 wallDestination;
    private Quaternion wallRotation;

    private bool activeWall = false;
    private bool buildingWall = false;
    public int wallCubeAmount = 3;
    public GameObject wallCube;
    public float wallCubeDistace = 0.5f;

    public LayerMask ground;

    private float holdTime = 0;
    public float chargeThreshold = 1;
    private bool attackButtonPressed = false;

    float rumbleTime = 0.009f;

    public Image chargeBarForeground;
    bool rumbleOnce = false;

    public Image wallProgressBar;

    public GameObject swordOfVorgon;
    public Animator swordAnimator;
    bool firstSpawn3rdPath = false;

    int currentMagicAbility = 0;
    public float swingCoolDown = 2f;
    private float nextSwingTime = 0f;
    public static int noOfPresses = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 1;
    public TrailRenderer swingTrail;
    private float trailTime = 0.7f;

    #endregion

    #region Events
    #endregion

    public static PlayerCombatController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    private void OnEnable()
    {
        wallMarker.SetActive(false);
        swordOfVorgon.SetActive(false);
        currentMagicAbility = 0;
        swingTrail.enabled = false;
    }

    #region Functions
    // Update is called once per frame
    void Update()
    {
        if (items[0] == true)
            PathOne();
        else if (items[1] == true)
            PathTwo();
        else if (items[2] == true)
            PathThree();

        Mathf.Clamp(rumbleTime, 0.009f, 0.4f);

        if (attackButtonPressed && holdTime >= 0.3f)
        {
            rumbleTime += Time.deltaTime * 0.01f;

            if (holdTime >= chargeThreshold)
                holdTime = chargeThreshold;

            if (rumbleTime >= chargeThreshold)
                rumbleTime = chargeThreshold;

            if(holdTime >= chargeThreshold)
                Rumbler.Instance.RumblePulse(0, 0.4f, 0.1f, 0.1f);

            if (holdTime <= chargeThreshold)
                CameraShake.Instance.ShakeCamera((holdTime / 10), (holdTime / 3), chargeThreshold);

            if (holdTime < chargeThreshold && !rumbleOnce)
            {
                Rumbler.Instance.RumblePulse(0, rumbleTime, 0.8f, chargeThreshold);
                rumbleOnce = true;
            }
            else
            {
                Rumbler.Instance.StopRumble();
                rumbleOnce = false;
                rumbleTime = 0.009f;
            }

            holdTime += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(holdTime / chargeThreshold);
            chargeBarForeground.fillAmount = fillAmount;

            if (fillAmount >= 1)
            {
                chargeBarForeground.color = Color.red;
            }
        }
        else
        {
            chargeBarForeground.color = Color.white;
            chargeBarForeground.fillAmount = 0;
            rumbleTime = 0.009f;
            rumbleOnce = false;
        }

    }

    void PathOne()
    {
        combatInputData.IsThirdRelic = false;

        if (Time.time >= timeToFire)
            combatInputData.CanCastFire = true;
        else
            combatInputData.CanCastFire = false;

        if (combatInputData.CastFire)
        {
            holdTime += Time.deltaTime;
            attackButtonPressed = true;
        }

        if (!combatInputData.CastFire && attackButtonPressed)
        {
            attackButtonPressed = false;

            if (CheckCharge())
            {
                timeToFire = Time.time + 1 / fireRate;
                ShootSpecialProjectile();
            }
            else
            {
                timeToFire = Time.time + 1 / fireRate;
                ShootFlamesTwoHanded();
            }
        }
    }

    bool CheckCharge()
    {
        if (holdTime >= chargeThreshold)
            return true;
        else
            return false;
    }

    void ShootSpecialProjectile()
    {
        // Code for spawning the special projectile
        Debug.Log("Shot Special");
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);
        if (leftHand)
        {
            leftHand = false;
            InstantiateProjectile(LHFirePoint, specialProjectile, true);
        }
        else
        {
            leftHand = true;
            InstantiateProjectile(RHFirepoint, specialProjectile, true);
        }

        combatInputData.CastFire = false;
        holdTime = 0;
    }

    void ShootFlamesTwoHanded()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);
        if (leftHand)
        {
            leftHand = false;
            InstantiateProjectile(LHFirePoint, regularProjectile, false);
        }
        else
        {
            leftHand = true;
            InstantiateProjectile(RHFirepoint, regularProjectile, false);
        }

        combatInputData.CastFire = false;
        holdTime = 0;
    }

    void InstantiateProjectile(Transform firePoint, GameObject projectile, bool special)
    {
        if(special)
            CameraShake.Instance.ShakeCamera(2f, 2.5f, 0.3f);
        else
            CameraShake.Instance.ShakeCamera(0.7f, 1.5f, 0.3f);
        var projectileObj = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized * projectileSpeed;
        iTween.PunchPosition(projectileObj, new Vector3(UnityEngine.Random.Range(-arcRange, arcRange), UnityEngine.Random.Range(-arcRange, arcRange), 0), UnityEngine.Random.Range(0.5f, 2.0f));
    }

    void PathTwo()
    {

        fireRate = 2;

        combatInputData.IsThirdRelic = false;

        if (Time.time >= timeToFire)
            combatInputData.CanCastFire = true;
        else
            combatInputData.CanCastFire = false;

        if (Time.time >= timeToWall)
            combatInputData.CanCreateWall = true;
        else
            combatInputData.CanCreateWall = false;

        if (combatInputData.CastFire)
        {
            holdTime += Time.deltaTime;
            attackButtonPressed = true;
        }

        if (!combatInputData.CastFire && attackButtonPressed)
        {
            attackButtonPressed = false;

            if (CheckCharge())
            {
                timeToFire = Time.time + 1 / fireRate;
                ShootSpecialProjectileRightHand();
            }
            else
            {
                timeToFire = Time.time + 1 / fireRate;
                ShootFlamesRightHand();
            }
        }

        if (activeWall)
        {
            UpdateWallHolo();

            if (combatInputData.CancelWall)
            {
                delayOnce = false;
                activeWall = false;
                wallMarker.SetActive(false);
                combatInputData.StartWallPlace = false;
            }

        }

        if (!delayOnce)
        {
            if (combatInputData.StartWallPlace)
            {
                placeDelay = Time.time + 1 / 2f;
                Debug.Log("wall Holo");
                activeWall = true;
                wallMarker.SetActive(true);
                delayOnce = true;
            }
        }

        //Debug.Log(wallRate);

        if (combatInputData.CreateWall && Time.time >= placeDelay && !buildingWall)
        {
            timeToWall = Time.time + wallRate;
            //display ui for delay of next wall place
            wallProgressBar.gameObject.SetActive(true);
            SpawnWallOfSouls();
        }

        if (wallProgressBar.gameObject.activeInHierarchy)
        {
            wallProgressBar.fillAmount = 1 - ((Time.time - timeToWall + wallRate) / wallRate);
            if (wallProgressBar.fillAmount <= 0)
            {
                wallProgressBar.gameObject.SetActive(false);
                wallProgressBar.fillAmount = 1;
            }
        }

        //if building wall, time is less than place delay or can create wall is false. then display ui so the play knows they cannot place a wall.

        //also if temp wall is active give the player the option to cancel the wall place

    }

    void ShootFlamesRightHand()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);


        InstantiateProjectile(RHFirepoint, regularProjectile, false);

        combatInputData.CastFire = false;
        holdTime = 0;
    }

    void ShootSpecialProjectileRightHand()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);


        InstantiateProjectile(RHFirepoint, specialProjectile, true);

        combatInputData.CastFire = false;
        holdTime = 0;
    }


    void UpdateWallHolo()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 8, ground))
        {
            // Set the position of the object to be relative to the player camera
            wallDestination = hit.point;
            // Set the rotation of the object to face the direction the player is aiming
            wallRotation = Quaternion.LookRotation(ray.direction);

            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        if(isInRange)
        {
            wallMarker.transform.position = wallDestination;
            wallRotation = new Quaternion(wallMarker.transform.rotation.x, wallRotation.y, wallMarker.transform.rotation.z, wallMarker.transform.rotation.w);
            wallMarker.transform.rotation = wallRotation;
            wallMarker.SetActive(true);
        }
        else
        {
            Debug.Log("Invalded Placement");
            wallMarker.SetActive(false);
        }
    }


    void SpawnWallOfSouls()
    {
        Debug.Log("Wall Of Souls Spawned");
        delayOnce = false;
        activeWall = false;
        wallMarker.SetActive(false);
        combatInputData.StartWallPlace = false;
        buildingWall = true;
        GameObject wall = new GameObject();
        wall.transform.position = wallDestination;
        wall.name = "Wall of Souls";

        for(int i = 0; i < wallCubeAmount; i++)
        {
            var cube = Instantiate(wallCube, wallDestination + new Vector3(i * wallCubeDistace, 0, 0), Quaternion.identity) as GameObject;
            cube.transform.SetParent(wall.transform);
        }

        Rumbler.Instance.RumbleConstant(0, 0.01f, 1.8f);
        CameraShake.Instance.ShakeCamera(1.5f, 2, 1.8f);

        wall.transform.rotation = wallRotation;
        wall.transform.Translate(new Vector3(-(int)(wallCubeAmount / 2) * wallCubeDistace, -1.2f, 0), Space.Self);

        buildingWall = false;
    }

    void PathThree()
    {

        //ui needs to reset properly.

        combatInputData.IsThirdRelic = true;

        //0 being fire, 1 being wall;

        HandleSwordCombat();

        Debug.Log(currentMagicAbility);

        if(combatInputData.SwitchAbility)
        {
            if (currentMagicAbility == 0)
                currentMagicAbility = 1;
            else if (currentMagicAbility == 1)
                currentMagicAbility = 0;
        }

        if(currentMagicAbility == 0)
        {
            if (Time.time >= timeToFire)
                combatInputData.CanCastFire = true;
            else
                combatInputData.CanCastFire = false;

            if (combatInputData.CastFire)
            {
                holdTime += Time.deltaTime;
                attackButtonPressed = true;
            }

            if (!combatInputData.CastFire && attackButtonPressed)
            {
                attackButtonPressed = false;

                if (CheckCharge())
                {
                    timeToFire = Time.time + 1 / fireRate;
                    ShootSpecialProjectileLeftHand();
                }
                else
                {
                    timeToFire = Time.time + 1 / fireRate;
                    ShootFlamesLeftHand();
                }
            }
        }
        else if(currentMagicAbility == 1)
        {
            if (Time.time >= timeToWall)
                combatInputData.CanCreateWall = true;
            else
                combatInputData.CanCreateWall = false;

            if (activeWall)
            {
                UpdateWallHolo();

                if (combatInputData.CancelWall)
                {
                    delayOnce = false;
                    activeWall = false;
                    wallMarker.SetActive(false);
                    combatInputData.StartWallPlace = false;
                }

            }

            if (!delayOnce)
            {
                if (combatInputData.StartWallPlace)
                {
                    placeDelay = Time.time + 1 / 2f;
                    Debug.Log("wall Holo");
                    activeWall = true;
                    wallMarker.SetActive(true);
                    delayOnce = true;
                }
            }

            if (combatInputData.CreateWall && Time.time >= placeDelay && !buildingWall)
            {
                timeToWall = Time.time + wallRate;
                //display ui for delay of next wall place
                wallProgressBar.gameObject.SetActive(true);
                SpawnWallOfSouls();
            }

            if (wallProgressBar.gameObject.activeInHierarchy)
            {
                wallProgressBar.fillAmount = 1 - ((Time.time - timeToWall + wallRate) / wallRate);
                if (wallProgressBar.fillAmount <= 0)
                {
                    wallProgressBar.gameObject.SetActive(false);
                    wallProgressBar.fillAmount = 1;
                }
            }
        }

        //current left hand ability

        //logic for sword combat

        if(!firstSpawn3rdPath)
        {
            StartCoroutine(WaitForSpawn());
            firstSpawn3rdPath = true;
        }
        else
        {
            swordOfVorgon.SetActive(true);
        }

        //need to manage switching between left hand abilities
    }

    void HandleSwordCombat()
    {

        if (swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            swordAnimator.SetBool("hit1", false);
        }

        if (swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            swordAnimator.SetBool("hit2", false);
        }

        if (swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            swordAnimator.SetBool("hit3", false);
            noOfPresses = 0;
        }

        if (Time.time - lastClickedTime > trailTime)
        {
            swingTrail.enabled = false;
        }


        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfPresses = 0;
            swordAnimator.SetBool("hit1", false);
            swordAnimator.SetBool("hit2", false);
            swordAnimator.SetBool("hit3", false);
        }
        if(Time.time > nextSwingTime)
        {
            if (combatInputData.SwingSword)
            {
                OnSwing();
            }
        }
    }

    void OnSwing()
    {
        lastClickedTime = Time.time;
        noOfPresses++;
        swingTrail.enabled = true;
        if (noOfPresses == 1)
        {
            swordAnimator.SetBool("hit1", true);
            trailTime = 0.7f;
            Rumbler.Instance.RumblePulse(0, 0.1f, 0.1f, 0.2f);
        }
        noOfPresses = Mathf.Clamp(noOfPresses, 0, 3);

        if (noOfPresses >= 2 && swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            swordAnimator.SetBool("hit1", false);
            swordAnimator.SetBool("hit2", true);
            trailTime = 0.7f;
            Rumbler.Instance.RumblePulse(0, 0.2f, 0.1f, 0.2f);
        }

        if(noOfPresses >= 3 && swordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            swordAnimator.SetBool("hit2", false);
            swordAnimator.SetBool("hit3", true);
            trailTime = 1.5f;
            Rumbler.Instance.RumbleConstant(0, 0.3f, 1.5f);
        }

    }

    IEnumerator WaitForSpawn()
    {
        yield return new WaitForSeconds(1);
        swordOfVorgon.SetActive(true);
        swordAnimator.SetTrigger("Unsheathe");
    }

    void ShootFlamesLeftHand()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);


        InstantiateProjectile(LHFirePoint, regularProjectile, false);

        combatInputData.CastFire = false;
        holdTime = 0;
    }

    void ShootSpecialProjectileLeftHand()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);


        InstantiateProjectile(LHFirePoint, specialProjectile, true);

        combatInputData.CastFire = false;
        holdTime = 0;
    }


}
    #endregion
