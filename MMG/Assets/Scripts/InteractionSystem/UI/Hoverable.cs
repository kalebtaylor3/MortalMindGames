using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace MMG
{
    public class Hoverable : MonoBehaviour, IHoverable
    {
        
        [BoxGroup("Settings")] public string displayText;
        [BoxGroup("Settings")] public Transform displayTextTransform;

        private Material myMat;
        public Material MyMaterial => myMat;

        private MeshRenderer meshRenderer;
        private MeshRenderer MeshRenderer => meshRenderer;

        public string DisplayText
        {
            get => displayText;
            set => displayText = value;
        }

        public Transform DisplayTextTransform => displayTextTransform;


        protected virtual void Awake()
        {
            GetComponents();
        }

        protected virtual void GetComponents()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            myMat = meshRenderer.material;
        }

        public void OnHoverStart(Material hoverMat)
        {
            meshRenderer.material = hoverMat;
        }

        public void OnHoverEnd()
        {
            meshRenderer.material = myMat;
        }

    }
}
