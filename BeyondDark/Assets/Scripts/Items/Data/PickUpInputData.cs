using UnityEngine;

[CreateAssetMenu(fileName = "PickUpInputData", menuName = "MMG_Player/Data/PickUpInputData", order = 0)]
public class PickUpInputData : ScriptableObject
{
    private bool pickUpClicked;
    private bool pickUpHold;
    private bool pickUpReleased;

    public bool PickUpClicked
    {
        get => pickUpClicked;
        set => pickUpClicked = value;
    }
    public bool PickUpHold
    {
        get => pickUpHold;
        set => pickUpHold = value;
    }
    public bool PickUpReleased
    {
        get => pickUpReleased;
        set => pickUpReleased = value;
    }
}