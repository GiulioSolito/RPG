using System;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using GameDevTV.Saving;
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
        private WeaponConfig currentWeaponConfig;
        private Weapon currentWeapon;
        
        private float nextAttack;
        
        private Health target;
        private Equipment equipment;
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
            equipment = GetComponent<Equipment>();
        }

        void OnEnable()
        {
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }
        
        void OnDisable()
        {
            if (equipment)
            {
                equipment.equipmentUpdated -= UpdateWeapon;
            }
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

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon = AttachWeapon(weapon);
        }

        void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            if (weapon == null)
            {
                EquipWeapon(defaultWeaponConfig);
            }
            else
            {
                EquipWeapon(weapon);
            }
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
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetAttackRange();
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
                nextAttack = Time.time + currentWeaponConfig.GetAttackRate();
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
                currentWeapon.OnHit();
            }
            
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
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
            EquipWeapon(weaponConfig);
        }
    }
}