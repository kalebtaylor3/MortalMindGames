using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSkybox : MonoBehaviour
{
    public Material mortalRealmSkybox;

    private void OnEnable()
    {
        RenderSettings.skybox = mortalRealmSkybox;
    }

    private void Update()
    {
        RenderSettings.skybox = mortalRealmSkybox;
    }
}
