using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSShooter : MonoBehaviour
{
    public Camera cam;
    public GameObject projectile;
    public Transform LHFirePoint, RHFirepoint;
    public float projectileSpeed = 70;
    public float fireRate = 4;
    public float arcRange = 1;



    private Vector3 destination;
    private bool leftHand;
    private float timeToFire;

    void Start()
    {
        
    }
    void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= timeToFire)
        {
            timeToFire = Time.time + 1 / fireRate;
            ShootProjectile();
        }
    }
    void ShootProjectile()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            destination = hit.point;
        else
            destination = ray.GetPoint(100000);
        if(leftHand)
        {
            leftHand = false;
            InstantiateProjectile(LHFirePoint);
        }
        else
        {
            leftHand = true;
            InstantiateProjectile(RHFirepoint);
        }



    }
    void InstantiateProjectile(Transform firePoint)
    {
        var projectileObj = Instantiate(projectile,firePoint.position,Quaternion.identity) as GameObject;
        RotateToDestination(projectileObj, destination, true);
        projectileObj.GetComponent<Rigidbody>().velocity = (destination - firePoint.position).normalized *projectileSpeed;
        iTween.PunchPosition(projectileObj, new Vector3(Random.Range(-arcRange, arcRange), Random.Range(-arcRange, arcRange), 0), Random.Range(3.0f, 5.0f));
    }
    void RotateToDestination(GameObject obj, Vector3 destination, bool onlyY)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if(onlyY)
        {
            rotation.x = 0;
            rotation.z = 0;

        }
        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
    }



}
