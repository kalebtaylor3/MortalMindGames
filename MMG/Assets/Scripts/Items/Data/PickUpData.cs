using UnityEngine;

namespace MMG
{
    [CreateAssetMenu(fileName = "PickUpData", menuName = "MMG_Player/Data/PickUpData", order = 0)]
    public class PickUpData : ScriptableObject
    {
        private PickUp pickUp;
        public PickUp PickUpItem
        {
            get => pickUp;
            set => pickUp = value;
        }

        public bool IsEmpty()
        {
            if (pickUp != null)
                return false;
            else
                return true;
        }

        public bool IsSamePickUp(PickUp pickUp)
        {
            if(this.pickUp == pickUp)
                return true;
            else
                return false;
        }

        public void Pick()
        {
            //pickable.OnPickUp();
        }

        public void Hold()
        {
           //pickable.OnHold();
        }

        public void Release()
        {
           //pickable.OnRelease();
        }

        public void ResetData()
        {
            pickUp = null;
        }

    }
}