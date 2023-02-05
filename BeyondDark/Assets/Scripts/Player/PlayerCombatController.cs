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



    void InstantiateProjectile(Transform firePoint)
    {
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

        if (combatInputData.CreateWall && Time.time >= placeDelay)
        {
            timeToWall = Time.time + wallRate;
            SpawnWallOfSouls();
        }

    }

    void UpdateWallHolo()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 20))
        {
            Vector3 aimDirection = cam.transform.forward;

            // Set the position of the object to be relative to the player camera
            wallDestination = cam.transform.position + aimDirection;

            // Set the rotation of the object to face the direction the player is aiming
            transform.rotation = Quaternion.LookRotation(aimDirection);
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
        }
    }


    void SpawnWallOfSouls()
    {
        Debug.Log("Wall Of Souls Spawned");
        delayOnce = false;
        activeWall = false;
        wallMarker.SetActive(false);
        combatInputData.StartWallPlace = false;
    }

    void PathThree()
    {
        combatInputData.IsThirdRelic = true;

        //need to manage switching between left hand abilities
    }

}
    #endregion
