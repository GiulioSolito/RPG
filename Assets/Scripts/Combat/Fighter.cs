using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [BoxGroup("Hand Transforms")]
        [SerializeField] private Transform rightHandTransform;
        [BoxGroup("Hand Transforms")]
        [SerializeField] private Transform leftHandTransform;
        
        [BoxGroup("Weapon")]
        [SerializeField] private WeaponConfig defaultWeaponConfig;
        private Item currentWeaponConfig;
        private Item currentWeapon;

        private ItemDefinition itemDefinition;
        
        private Attribute<int> damageAttribute;
        private Attribute<float> attackRateAttribute;
        private Attribute<float> rangeAttribute;
        private int damage;
        private float attackRate;
        private float range;
        
        private float nextAttack;
        
        private Health target;
        private Mover mover;
        private BaseStats baseStats;
        private ActionScheduler scheduler;
        private Animator anim;

        void Awake()
        {
            mover = GetComponent<Mover>();
            baseStats = GetComponent<BaseStats>();
            scheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
        }

        void Start()
        {
        }

        void Update()
        {
            if (target == null) return;
            if (target.IsDead) return;
            
            if (IsInRange(target.transform) == false)
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(Item weapon)
        {
            currentWeapon = weapon;

            itemDefinition = InventorySystemManager.GetItemDefinition(weapon.name);
            
            damageAttribute = itemDefinition.GetAttribute<Attribute<int>>("BaseAttack");
            attackRateAttribute = itemDefinition.GetAttribute<Attribute<float>>("AttackRate");
            rangeAttribute = itemDefinition.GetAttribute<Attribute<float>>("Range");
            
            range = rangeAttribute.GetValue();
            range = rangeAttribute.GetValue();
            range = rangeAttribute.GetValue();
        }

        public Health GetTarget()
        {
            return target;
        }

        bool IsInRange(Transform targetTransform)
        {
            
            
            return Vector3.Distance(transform.position, targetTransform.position) <= range;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (mover.CanMoveTo(combatTarget.transform.position) == false && IsInRange(combatTarget.transform) == false)
            {
                return false;
            }
            
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead;
        }

        public void Attack(GameObject combatTarget)
        {
            scheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }
        
        void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            
            if (Time.time > nextAttack)
            {
                //This will trigger the Hit() event
                TriggerAttack();
                nextAttack = Time.time + attackRate;
            }
        }

        void TriggerAttack()
        {
            anim.ResetTrigger("StopAttack");
            anim.SetTrigger("Attack");
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;

            float damage = baseStats.GetStat(Stat.Damage);

            if (currentWeapon != null)
            {
                // currentWeapon.OnHit();
            }
            
            // if (currentWeaponConfig.HasProjectile())
            // {
            //     currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            // }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        //Animation Event
        void Shoot()
        {
            Hit();
        }

        void StopAttack()
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("StopAttack");
        }
        
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        // public IEnumerable<float> GetPercentageModifiers(Stat stat)
        // {
        //     if (stat == Stat.Damage)
        //     {
        //         yield return currentWeaponConfig.GetPercantageBonus();
        //     }
        // }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            WeaponConfig weaponConfig = Resources.Load<WeaponConfig>(weaponName);
            // EquipWeapon(weaponConfig);
        }
    }
}