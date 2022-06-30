// using System;
// using System.Collections;
// using RPG.Attributes;
// using RPG.Control;
// using RPG.Movement;
// using UnityEngine;
//
// namespace RPG.Combat
// {
//     public class WeaponPickup : MonoBehaviour, IRaycastable
//     {
//         [SerializeField] private WeaponConfig weaponConfig;
//         [SerializeField] private float healthToRestore = 0;
//         [SerializeField] private float respawnTime = 5f;
//
//         private GameObject callingFighter;
//         
//         // void OnTriggerEnter(Collider other)
//         // {
//         //     if (!other.CompareTag("Player")) return;
//         //
//         //     callingFighter = other.GetComponent<Fighter>();
//         //     
//         //     Pickup(callingFighter);
//         // }
//
//         public void StartPickup()
//         {
//             Pickup(callingFighter);
//         }
//         
//         void Pickup(GameObject pickup)
//         {
//             Fighter fighter = pickup.GetComponent<Fighter>();
//
//             if (weaponConfig != null)
//             {
//                 fighter.EquipWeapon(weaponConfig);
//             }
//
//             fighter.GetComponent<ItemCollector>().StopPickup();
//
//             if (healthToRestore > 0)
//             {
//                 pickup.GetComponent<Health>().Heal(healthToRestore);
//             }
//             
//             StartCoroutine(RespawnPickup(respawnTime));
//         }
//
//         IEnumerator RespawnPickup(float seconds)
//         {
//             ShowPickup(false);
//             yield return new WaitForSeconds(seconds);
//             ShowPickup(true);
//         }
//         
//         void ShowPickup(bool shouldShow)
//         {
//             GetComponent<Collider>().enabled = shouldShow;
//
//             foreach (Transform child in transform)
//             {
//                 child.gameObject.SetActive(shouldShow);
//             }
//         }
//         
//         public CursorType GetCursorType()
//         {
//             return CursorType.Pickup;
//         }
//
//         public bool HandleRaycast(PlayerController callingController)
//         {
//             ItemCollector collector = callingController.GetComponent<ItemCollector>();
//             
//             if (Input.GetMouseButtonDown(0))
//             {
//                 callingFighter = callingController.gameObject;
//                 // collector.StartPickupCollector(this);
//             }
//
//             return true;
//         }
//     }
// }