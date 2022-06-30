using System;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class Pickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private GameObject pickupUI;
        
        private Inventory inventory;
        private ItemObject itemObject;

        void Awake()
        {
            inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            itemObject = GetComponent<ItemObject>();
        }

        void Start()
        {
            pickupUI.SetActive(false);
        }

        public void PickupItem()
        {
            if (itemObject == null) return;
            inventory.AddItem(itemObject.ItemInfo);
            Destroy(gameObject);
        }
        
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
        
        void OnMouseEnter()
        {
            pickupUI.SetActive(true);
        }

         void OnMouseExit()
        {
            pickupUI.SetActive(false);
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            ItemCollector collector = callingController.GetComponent<ItemCollector>();
            
            if (Input.GetMouseButtonDown(0))
            {
                collector.StartPickupCollector(this);
            }

            return true;
        }
    }
}