using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class noFog : MonoBehaviour
{
    public Camera cameraWithoutFog;

    private void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void Awake()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }

    private void Start()
    {
        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    }
    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    }
    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == cameraWithoutFog)
            RenderSettings.fog = false;
        else
            RenderSettings.fog = true;
    }
}
