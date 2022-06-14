using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health = 100f;

        public bool IsDead { get; private set; }

        private Animator anim;
        
        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        public void TakeDamage(float damage)
        {
            if (IsDead) return;
            
            health = Mathf.Max(health - damage, 0);

            if (health <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            anim.SetTrigger("Die");
            IsDead = true;
        }
    }
}