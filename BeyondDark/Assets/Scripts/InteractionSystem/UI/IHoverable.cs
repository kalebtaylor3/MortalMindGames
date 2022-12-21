using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{    
    public interface IHoverable
    {
        string DisplayText { get; set;}
        Transform DisplayTextTransform { get; }

        void OnHoverStart(Material hoverMat);
        void OnHoverEnd();
    }
}
