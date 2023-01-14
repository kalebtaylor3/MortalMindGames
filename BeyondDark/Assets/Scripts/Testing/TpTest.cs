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
    private Vector3 tpPosition;

    //public static event Action RealmTransportation;


    public void tpPlayer()
    {
        StartCoroutine(RealmTransport());

        // Debug.Log("tp");

        //CopySpecialComponents(mortalRealmPlayer, VorgonRealmPlayer);

        //mortalRealmPlayer.transform.position = Vector3.zero;
    }

    IEnumerator RealmTransport()
    {
        Debug.Log("before");        
        //RealmTransportation?.Invoke();

        yield return new WaitForSeconds(3.0f);

        Debug.Log("after");

        if (WorldData.Instance.activeRealm == WorldData.REALMS.MORTAL)
        {
            //CopySpecialComponents(mortalRealmPlayer, VorgonRealmPlayer);
            VorgonRealmPlayer.SetActive(true);
            mortalRealmPlayer.SetActive(false);
            
            WorldData.Instance.activeRealm = WorldData.REALMS.VORGON;
        }
        else if (WorldData.Instance.activeRealm == WorldData.REALMS.VORGON)
        {
            //CopySpecialComponents(VorgonRealmPlayer, mortalRealmPlayer);
            mortalRealmPlayer.SetActive(true);
            VorgonRealmPlayer.SetActive(false);
            //RealmTransportation?.Invoke();
            WorldData.Instance.activeRealm = WorldData.REALMS.MORTAL;
        }


    }

    // To copy components NOT NEEDED

    private void CopySpecialComponents(GameObject _sourceGO, GameObject _targetGO)
    {

        //_targetGO.GetComponent<PlayerInventoryController>().items = _sourceGO.GetComponent<PlayerInventoryController>().items;
        //_targetGO.GetComponent<PlayerController>().bookSlots = _sourceGO.GetComponent<PlayerController>().bookSlots;
        //_targetGO.GetComponentInChildren<SpellBook>().transform.gameObject = _sourceGO.GetComponentInChildren<SpellBook>().transform.gameObject;

        //foreach (var component in _sourceGO.GetComponents<Component>())
        //{
        //    var componentType = component.GetType();
        //    if (componentType != typeof(Transform) &&
        //        componentType != typeof(MeshFilter) &&
        //        componentType != typeof(MeshRenderer)
        //        )
        //    {
        //        Debug.Log("Found a component of type " + component.GetType());
        //        UnityEditorInternal.ComponentUtility.CopyComponent(component);
        //        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_targetGO);
        //        Debug.Log("Copied " + component.GetType() + " from " + _sourceGO.name + " to " + _targetGO.name);
        //    }
        //}
    }
}

