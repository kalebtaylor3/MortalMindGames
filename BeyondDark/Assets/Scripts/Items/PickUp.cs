using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MMG
{
    public class PickUp: MonoBehaviour
    {
        public RelicSpawnManager.RELIC_TYPE relicType = RelicSpawnManager.RELIC_TYPE.NONE;

        public GameObject PickUpItem;
        public int pickUpID;

        public bool RealmTp = false;
    }
}
