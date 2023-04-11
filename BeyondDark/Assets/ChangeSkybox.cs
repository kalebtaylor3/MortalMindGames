using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public Material mortalRealmSkybox;
    public GameObject mortalRealmPPE;
    public GameObject vorgonRealmPPE;

    private void OnEnable()
    {
        RenderSettings.skybox = mortalRealmSkybox;
        mortalRealmPPE.SetActive(true);
    }

    private void Update()
    {
        RenderSettings.skybox = mortalRealmSkybox;
        mortalRealmPPE.SetActive(true);
        vorgonRealmPPE.SetActive(false);
    }
}
