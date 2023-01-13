using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellBookInputData", menuName = "MMG_Player/Data/SpellBookInputData", order = 0)]
public class SpellBookInputData : ScriptableObject
{
    private bool pageTurnRight;
    private bool pageTurnLeft;
    private bool zoomBook;

    public bool PageTurnRight
    {
        get => pageTurnRight;
        set => pageTurnRight = value;
    }

    public bool PageTurnLeft
    {
        get => pageTurnLeft;
        set => pageTurnLeft = value;
    }

    public bool ZoomBook
    {
        get => zoomBook;
        set => zoomBook = value;
    }
}
