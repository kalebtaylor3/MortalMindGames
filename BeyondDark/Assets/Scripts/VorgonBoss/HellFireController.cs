using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class HellFireController : MonoBehaviour
{

    public GameObject projectile;
    public Transform shootPos;
    public float projectileSpeed = 10;
    public float delay = 1f; // change this value to adjust the delay
    private bool canShoot = false;

    List<GameObject> shootList = new List<GameObject>();

    private void Awake()
    {
        canShoot = false;
        float random = Random.Range(0, 0.6f);
        StartCoroutine(WaitToShoot(random));
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(DisableShooting());
        canShoot = false;
        float random = Random.Range(0, 1f);
        StartCoroutine(WaitToShoot(random));
    }

    
    

    IEnumerator WaitToShoot(float delay)
    {
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    private void OnDestroy()
    {

        //Destroy(shootList[shootList.Count - 1]);
    }

    // Update is called once per frame
    void Update()
    {
        if (canShoot)
        {
            ShootProjectile();
            canShoot = false;
            Invoke("EnableShooting", delay);
        }
    }

    // Function to create the projectile
    void ShootProjectile()
    {
        var projectileObj = Instantiate(projectile, shootPos.position, Quaternion.identity) as GameObject;
        projectileObj.GetComponent<Rigidbody>().velocity = shootPos.forward * projectileSpeed;
        shootList.Add(projectileObj);
    }

    // Function to enable shooting
    void EnableShooting()
    {
        canShoot = true;
    }
}
