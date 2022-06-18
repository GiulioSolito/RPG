using System;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        private float nextAttack;
        [SerializeField] private Transform rightHandTransform;
        [SerializeField] private Transform leftHandTransform;
        [SerializeField] private Weapon defaultWeapon;
        private Weapon currentWeapon;
        
        private Health target;

        private Mover mover;
        private ActionScheduler scheduler;
        private Animator anim;

        void Awake()
        {
            mover = GetComponent<Mover>();
            scheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
        }

        void Start()
        {
            EquipWeapon(defaultWeapon);
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

            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
            }
            else
            {
                target.TakeDamage(currentWeapon.GetDamage());
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
    }
}