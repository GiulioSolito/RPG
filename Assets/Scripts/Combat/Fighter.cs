using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        private float nextAttack;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Weapon defaultWeapon;
        private Weapon currentWeapon;
        
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
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
        }

        void Update()
        {
            if (target == null) return;
            if (target.IsDead) return;
            
            if (IsInRange() == false)
            {
                mover.MoveTo(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }
        
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, anim);
        }

        public Health GetTarget()
        {
            return target;
        }

        bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= currentWeapon.GetAttackRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            
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
                nextAttack = Time.time + currentWeapon.GetAttackRate();
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
            
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
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
        
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.GetPercantageBonus();
            }
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        public object CaptureState()
        {
            return currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string) state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}