using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public WorldData.SECTIONS sectionType = WorldData.SECTIONS.NONE;

    private void OnTriggerStay(Collider other)
    {
        
        if (other.tag == "Player" && WorldData.Instance.activePlayerSection != sectionType)
        {
            WorldData.Instance.activePlayerSection = sectionType;
        }
        else if (other.tag == "Vorgon" && WorldData.Instance.activeVorgonSection != sectionType)
        {
            WorldData.Instance.activeVorgonSection = sectionType;
        }
    }
}
