using UnityEngine;

namespace MMG
{
    [CreateAssetMenu(fileName = "PickableData", menuName = "FirstPersonController/Data/PickableData", order = 0)]
    public class PickableData : ScriptableObject
    {
        private PickUp pickable;
        public PickUp PickableItem
        {
            get => pickable;
            set => pickable = value;
        }

        public bool IsEmpty()
        {
            if (pickable != null)
                return false;
            else
                return true;
        }

        public bool IsSamePickable(PickUp _pickable)
        {
            if(pickable == _pickable)
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
            pickable = null;
        }

    }
}