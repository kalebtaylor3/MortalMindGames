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

        public Vector3 tpPosition = new Vector3(10,1,-50);

        public bool RealmTp = false;
    }
}
