using System;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] private Weapon weapon;
        [SerializeField] private float respawnTime = 5f;
        
        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            other.GetComponent<Fighter>().EquipWeapon(weapon);
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
    }
}