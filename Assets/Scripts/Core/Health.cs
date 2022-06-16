﻿using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health = 100f;

        public bool IsDead { get; private set; }

        private Animator anim;
        private ActionScheduler scheduler;
        
        void Awake()
        {
            anim = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
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
            IsDead = true;
            anim.SetTrigger("Die");
            scheduler.CancelCurrentAction();
        }
    }
}