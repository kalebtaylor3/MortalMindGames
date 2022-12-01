using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellBookInputData", menuName = "MMG_Player/Data/SpellBookInputData", order = 0)]
public class SpellBookInputData : ScriptableObject
{
    private float pageTurn;
    private bool zoomBook;

    public float PageTurn
    {
        get => pageTurn;
        set => pageTurn = value;
    }
    public bool ZoomBook
    {
        get => zoomBook;
        set => zoomBook = value;
    }
}
