using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private Weapon equippedPrefab;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float damage = 5f;
        [SerializeField] private float percentageBonus = 0;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile;

        private const string WeaponName = "Weapon";
        
        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator anim)
        {
            DestroyOldWeapon(rightHand, leftHand);
            
            if (equippedPrefab == null) return null;

            Transform handTransform = GetTransform(rightHand, leftHand);

            Weapon weapon = Instantiate(equippedPrefab, handTransform);
            weapon.gameObject.name = WeaponName;
            
            var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
                
            if (animatorOverride != null)
            {
                anim.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WeaponName);

            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WeaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            handTransform = isRightHanded ? rightHand : leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetOwner(instigator.transform);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }
        
        public float GetAttackRange()
        {
            return attackRange;
        }

        public float GetAttackRate()
        {
            return attackRate;
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetPercantageBonus()
        {
            return percentageBonus;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}