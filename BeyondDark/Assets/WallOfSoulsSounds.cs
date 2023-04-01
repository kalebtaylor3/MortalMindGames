using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOfSoulsSounds : MonoBehaviour
{
    public AudioSource wallsource;
    public AudioClip wallRaise;
    public AudioClip wallHit;
    public AudioSource activeWall;


    private void OnEnable()
    {
        activeWall.enabled = false;
    }

    public void RaiseWall()
    {
        wallsource.PlayOneShot(wallRaise);
    }

    public void CreatedWall()
    {
        wallsource.Stop();
        activeWall.enabled = true;
    }

    public void WallDamaged()
    {
        wallsource.PlayOneShot(wallHit);
    }

}
