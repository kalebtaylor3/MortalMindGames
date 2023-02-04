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
    public float wallRate = 1f;
    public float arcRange = 1;



    private Vector3 destination;
    private bool leftHand;
    private float timeToFire;

    private float timeToWall;



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

        if (combatInputData.StartWallPlace)
        {
            //timeToWall = Time.time + 1 / wallRate;
            Debug.Log("wall Holo");
            //SpawnWallOfSouls();
        }

        if(combatInputData.CreateWall)
        {
            timeToWall = Time.time + 1 / wallRate;
            SpawnWallOfSouls();
        }

    }

    void SpawnWallOfSouls()
    {
        Debug.Log("Wall Of Souls Spawned");
        combatInputData.StartWallPlace = false;
    }

    void PathThree()
    {
        combatInputData.IsThirdRelic = true;

        //need to manage switching between left hand abilities
    }

}
    #endregion
