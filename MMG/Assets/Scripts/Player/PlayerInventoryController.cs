using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MMG
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [SerializeField] private InventoryInputData inventoryInputData = null;

        public List<GameObject> items = new List<GameObject>();

        private GameObject activeItem;

        public static event Action<GameObject> OnItemSwitched;

        // Update is called once per frame
        void Update()
        {
            if (inventoryInputData.Change1Clicked)
            {
                if (items.Count >= 1)
                {
                    DisableAllItems();

                    if (items[0] != null)
                        items[0].gameObject.SetActive(true);
                }
            }
            if (inventoryInputData.Change2Clicked)
            {
                if (items.Count >= 2)
                {
                    DisableAllItems();
                    if (items[1] != null)
                        items[1].gameObject.SetActive(true);
                }
            }
            if (inventoryInputData.Change3Clicked)
            {
                if (items.Count >= 3)
                {
                    DisableAllItems();
                    if (items[2] != null)
                        items[2].gameObject.SetActive(true);
                }
            }
            if (inventoryInputData.Change4Clicked)
            {
                if (items.Count >= 4)
                {
                    DisableAllItems();
                    if (items[3] != null)
                        items[3].gameObject.SetActive(true);
                }
            }

            if(inventoryInputData.PutItemAway)
                DisableAllItems();
        }

        void DisableAllItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(false);
            }
        }

        public GameObject ReturnActiveItem()
        {
            GameObject activeItem = null;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].activeInHierarchy)
                {
                    activeItem = items[i];
                }
            }
            return activeItem;
        }
    }
}
