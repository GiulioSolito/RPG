using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController animatorOverride;
        [SerializeField] private GameObject equippedPrefab;
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float damage = 5f;
        [SerializeField] private float percentageBonus = 0;
        [SerializeField] private bool isRightHanded = true;
        [SerializeField] private Projectile projectile;

        private const string WeaponName = "Weapon";
        
        public void Spawn(Transform rightHand, Transform leftHand, Animator anim)
        {
            DestroyOldWeapon(rightHand, leftHand);
            
            if (equippedPrefab == null) return;

            Transform handTransform = GetTransform(rightHand, leftHand);

            GameObject weapon = Instantiate(equippedPrefab, handTransform);
            weapon.name = WeaponName;
            
            var overrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
                
            if (animatorOverride != null)
            {
                anim.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                anim.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
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
    }
}