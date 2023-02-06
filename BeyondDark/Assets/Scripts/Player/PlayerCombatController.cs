using Cinemachine;
using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    #region Variables
    [SerializeField] private CombatInputData combatInputData = null;

    public List<bool> items = new List<bool>();


    public Camera cam;
    public GameObject projectile;
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
    }

    #region Functions
    // Update is called once per frame
    void Update()
    {
        if (items[0] == true)
            PathOne();
        else if (items[1] == true)
            PathTwo();
            
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
            timeToFire = Time.time + 1 / fireRate;
            ShootFlamesTwoHanded();
        }
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
            InstantiateProjectile(LHFirePoint);
        }
        else
        {
            leftHand = true;
            InstantiateProjectile(RHFirepoint);
        }

        combatInputData.CastFire = false;
    }

    void InstantiateProjectile(Transform firePoint)
    {
        CameraShake.Instance.ShakeCamera();
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
            timeToFire = Time.time + 1 / fireRate;
            ShootFlamesRightHand();
        }

        if (activeWall)
            UpdateWallHolo();

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

        Debug.Log(wallRate);

        if (combatInputData.CreateWall && Time.time >= placeDelay && !buildingWall)
        {
            timeToWall = Time.time + wallRate;
            //display ui for delay of next wall place
            SpawnWallOfSouls();
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


        InstantiateProjectile(RHFirepoint);

        combatInputData.CastFire = false;
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

        wall.transform.rotation = wallRotation;
        wall.transform.Translate(new Vector3(-(int)(wallCubeAmount / 2) * wallCubeDistace, 1.5f, 0), Space.Self);

        buildingWall = false;
    }

    void PathThree()
    {
        combatInputData.IsThirdRelic = true;

        //need to manage switching between left hand abilities
    }

    void ShootFlamesLeftHand()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);


        InstantiateProjectile(LHFirePoint);

        combatInputData.CastFire = false;
    }


}
    #endregion
