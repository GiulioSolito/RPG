using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float attackRange = 2f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float damage = 5f;
        private float nextAttack;
        
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

        void Update()
        {
            if (target == null) return;
            if (target.IsDead) return;
            
            if (IsInRange() == false)
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                mover.Cancel();
                AttackBehaviour();
            }
        }

        bool IsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= attackRange;
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
            target.TakeDamage(damage);
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
        }
    }
}