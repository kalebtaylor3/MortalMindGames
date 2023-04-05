using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialContainment : MonoBehaviour
{
    public List<ParticleSystem> wallParticle;
    public List<GameObject> collisionsGO;

    public void TurnOffWalls()
    {
        foreach (ParticleSystem p in wallParticle)
        {
            if (p != null)
            {
                p.Stop();
            }
        }

        foreach (GameObject go in collisionsGO)
        {
            if (go != null)
            {
                Destroy(go, 8f); ;
            }
        }
    }

    //IEnumerator TurnWallsOff()
    //{
    //    foreach (ParticleSystem p in wallParticle)
    //    {
    //        if (p != null)
    //        {
    //            p.Stop();
    //        }
    //    }

    //    foreach (GameObject go in collisionsGO)
    //    {
    //        if (go != null)
    //        {
    //            Destroy(go,8f); ;
    //        }
    //    }
    //}
}
