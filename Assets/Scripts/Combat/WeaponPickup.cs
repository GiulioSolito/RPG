using System;
using System.Collections;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private float respawnTime = 5f;

        private Fighter callingFighter;
        
        // void OnTriggerEnter(Collider other)
        // {
        //     if (!other.CompareTag("Player")) return;
        //
        //     callingFighter = other.GetComponent<Fighter>();
        //     
        //     Pickup(callingFighter);
        // }

        public void StartPickup()
        {
            Pickup(callingFighter);
        }
        
        void Pickup(Fighter fighter)
        {
            fighter.EquipWeapon(weapon);
            fighter.GetComponent<ItemCollector>().StopPickup();
            StartCoroutine(RespawnPickup(respawnTime));
        }

        IEnumerator RespawnPickup(float seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }
        
        void ShowPickup(bool shouldShow)
        {
            GetComponent<Collider>().enabled = shouldShow;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
        
        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            ItemCollector collector = callingController.GetComponent<ItemCollector>();
            
            if (Input.GetMouseButtonDown(0))
            {
                callingFighter = callingController.GetComponent<Fighter>();
                collector.StartPickupCollector(this);
                // Pickup(callingController.GetComponent<Fighter>());
            }

            return true;
        }
    }
}