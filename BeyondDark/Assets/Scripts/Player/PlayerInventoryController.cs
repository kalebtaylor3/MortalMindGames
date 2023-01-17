using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.PlayerLoop;

namespace MMG
{
    public class PlayerInventoryController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private InventoryInputData inventoryInputData = null;

        public List<GameObject> items = new List<GameObject>();

        public SpellBook spellBook;

        private GameObject activeItem;
        #endregion

        #region Events
        public static event Action<GameObject> OnItemSwitched;

        [HideInInspector] public bool inventoryOpen = false;
        #endregion

        public static PlayerInventoryController Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) { Instance = this; }
        }

        #region Functions
        // Update is called once per frame
        void Update()
        {
            if(inventoryInputData.OpenSpellBook)
            {
                this.GetComponent<PlayerController>().cameraController.ResetFOV();
                if (this.gameObject.tag == "Player" && !this.GetComponent<PlayerController>().isHiding)
                {
                    
                    if (inventoryOpen)
                    {                        
                        //close the book
                        Debug.Log("Spell Book Closed");
                        inventoryOpen = false;
                        spellBook.transform.position = spellBook.itemSlotPosition.position;
                        spellBook.transform.rotation = spellBook.itemSlotPosition.rotation;
                        spellBook.gameObject.SetActive(false);
                    }
                    else
                    {
                        //otherwise open the book
                        Debug.Log("Spell Book Opened");
                        inventoryOpen = true;
                        spellBook.gameObject.SetActive(true);
                        spellBook.OpenBook();
                    }
                }               
            }
        }

        public void UpdatePages()
        {
            spellBook.UpdatePages(items);
        }

        void DisableAllItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].SetActive(false);
            }
        }

        //public GameObject ReturnActiveItem()
        //{
        //    GameObject activeItem = null;
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        if (items[i].activeInHierarchy)
        //        {
        //            activeItem = items[i];
        //        }
        //    }
        //    return activeItem;
        //}
    }
    #endregion
}
