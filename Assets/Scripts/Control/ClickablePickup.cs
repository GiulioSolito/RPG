using System;
using System.Collections;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Control
{
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        private Pickup pickup;

        void Awake()
        {
            pickup = GetComponent<Pickup>();
        }

        public void Pickup()
        {
            if (pickup != null)
            {
                pickup.PickupItem();
            }
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
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