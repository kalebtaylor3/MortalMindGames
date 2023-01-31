using MMG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpTest : MonoBehaviour
{
    // TESTING

    #region Instance

    public static TpTest Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }

    #endregion

    [SerializeField]
    private GameObject mortalRealmPlayer;
    [SerializeField]
    private GameObject VorgonRealmPlayer;

    [SerializeField]
    //private Transform tpPosition;

    //public static event Action RealmTransportation;


    public void tpPlayer(Vector3 tpPosition)
    {
        StartCoroutine(RealmTransport(tpPosition));

        // Debug.Log("tp");

        //CopySpecialComponents(mortalRealmPlayer, VorgonRealmPlayer);

        //mortalRealmPlayer.transform.position = Vector3.zero;
    }

    IEnumerator RealmTransport(Vector3 tpPosition)
    {
        //Debug.Log("before");        
        //RealmTransportation?.Invoke();

        yield return new WaitForSeconds(3.0f);

        //Debug.Log("after");

        if (WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            //CopySpecialComponents(mortalRealmPlayer, VorgonRealmPlayer);

            VorgonRealmPlayer.transform.position = tpPosition;

            VorgonRealmPlayer.SetActive(true);
            VorgonRealmPlayer.GetComponent<PlayerController>().HandleRealmTransport();
            //VorgonRealmPlayer.GetComponent<PlayerController>().cameraController.ResetFOV();

            mortalRealmPlayer.SetActive(false);
            InteractionUIPanel.Instance.ResetUI();
            
            WorldData.Instance.activeRealm = WorldData.REALMS.VORGON;
        }
        else if (WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
        {
            mortalRealmPlayer.transform.position = tpPosition;

            //CopySpecialComponents(VorgonRealmPlayer, mortalRealmPlayer);
            mortalRealmPlayer.SetActive(true);
            mortalRealmPlayer.GetComponent<PlayerController>().HandleRealmTransport();

            //Move vorgon character to next location
            

            VorgonRealmPlayer.SetActive(false);
            InteractionUIPanel.Instance.ResetUI();
            //RealmTransportation?.Invoke();
            WorldData.Instance.activeRealm = WorldData.REALMS.MORTAL;
        }


    }

    public void MortalRealmDeath(Vector3 tpPosition)
    {
        mortalRealmPlayer.transform.position = tpPosition;
    }

    
}

