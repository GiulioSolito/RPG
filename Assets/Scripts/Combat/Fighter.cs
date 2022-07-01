using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Equipping;
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
        [SerializeField] private ItemDefinition defaultWeaponConfig;
        public ItemDefinition currentWeaponConfig;
        private Weapon currentWeapon;
        private float nextAttack;

        private Health target;
        private Mover mover;
        private BaseStats baseStats;
        private ActionScheduler scheduler;
        private Animator anim;
        private IEquipper equipper;

        void Awake()
        {
            mover = GetComponent<Mover>();
            baseStats = GetComponent<BaseStats>();
            scheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
            equipper = GetComponent<Equipper>();
        }

        void Start()
        {
            if (currentWeaponConfig == null)
            {
                EquipWeapon(defaultWeaponConfig);
            }
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

        public void EquipWeapon(ItemDefinition weapon)
        {
            currentWeaponConfig = weapon;
        }
        
        Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, anim);
        }

        public Health GetTarget()
        {
            return target;
        }

        bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= baseStats.AttackRange;
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
                nextAttack = Time.time + baseStats.AttackRate;
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

            if (currentWeapon != null)
            {
                currentWeapon.OnHit();
            }
            else
            {
                target.TakeDamage(gameObject, baseStats.Damage);
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
                yield return baseStats.Damage;
            }
        }

        // public IEnumerable<float> GetPercentageModifiers(Stat stat)
        // {
        //     if (stat == Stat.Damage)
        //     {
        //         // yield return currentWeaponConfig.GetPercantageBonus();
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