using Opsive.Shared.Game;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.ItemActions;
using Opsive.UltimateInventorySystem.ItemObjectBehaviours;
using System.Collections;
using Opsive.UltimateInventorySystem.Equipping;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    /// <summary>
    /// A range attack action.
    /// </summary>
    public class RangeAttack : ItemObjectBehaviour
    {
        [Tooltip("The cooldown between each attack.")]
        [SerializeField] protected float coolDown;
        [Tooltip("The delay between the attack is triggered and when the bullets come out (used to sync fire with animation).")]
        [SerializeField] protected float startFireDelay;
        [Tooltip("The number of projectiles that should come out when firing.")]
        [SerializeField] protected int projectilesPerFire;
        [Tooltip("The Angle between each projectile when firing creating a cone of projectiles.")]
        [SerializeField] protected float projectileAngle;
        [Tooltip("The projectile prefab.")]
        [SerializeField] protected GameObject projectilePrefab;
        [Tooltip("The Audio source to play when attacking.")]
        [SerializeField] protected AudioSource audioSource;

        protected CharacterEquipper player;
        protected Item item;

        public int BulletsPerFire {
            get => projectilesPerFire;
            set => projectilesPerFire = value;
        }

        public GameObject BulletPrefab {
            get => projectilePrefab;
            set => projectilePrefab = value;
        }

        /// <summary>
        /// Use the item.
        /// </summary>
        /// <param name="itemObject">The item object.</param>
        /// <param name="itemUser">The item user.</param>
        public override void Use(ItemObject itemObject, ItemUser itemUser)
        {
            player = itemUser.GetComponent<CharacterEquipper>();
            if (player == null) { return; }

            item = itemObject.Item;

            m_NextUseTime = Time.time + coolDown;

            //Character animation
            player.SetWeapon(itemObject.Item);
            if (audioSource != null) { audioSource.Play(); }

            StartCoroutine(FireIE());
        }

        /// <summary>
        /// Fire coroutine.
        /// </summary>
        /// <returns>The IEnumerator.</returns>
        protected IEnumerator FireIE()
        {
            yield return new WaitForSeconds(startFireDelay);
            var charTransform = player.transform;


            for (int i = 0; i < projectilesPerFire; i++) {
                var angleDiff = 0f;
                var sign = ((-2 * (i % 2)) + 1);
                if (projectilesPerFire % 2 == 0) {
                    angleDiff = sign * Mathf.CeilToInt((i + 1) / 2f) * projectileAngle;
                } else {
                    angleDiff = sign * Mathf.CeilToInt(i / 2f) * projectileAngle;
                }
                var rot = charTransform.rotation.eulerAngles;
                rot = new Vector3(rot.x, rot.y + angleDiff, rot.z);
                var bulletGO = ObjectPool.Instantiate(
                    projectilePrefab,
                    charTransform.position + charTransform.forward + Vector3.up,
                    Quaternion.Euler(rot));

                // bulletGO.GetComponent<RangeAttackBullet>().Character = player;
            }
        }
    }
}