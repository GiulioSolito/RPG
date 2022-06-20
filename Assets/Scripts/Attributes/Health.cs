﻿using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float regenerationPercentage = 70f;

        private float health = -1f;

        public bool IsDead { get; private set; }

        private Animator anim;
        private ActionScheduler scheduler;

        void Awake()
        {
            anim = GetComponent<Animator>();
            scheduler = GetComponent<ActionScheduler>();
        }

        void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        void Start()
        {
            if (health < 0)
            {
                health = GetComponent<BaseStats>().GetStat(Stat.Health);
            }
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (IsDead) return;

            Debug.Log($"{gameObject.name} took damage: {damage}");

            health = Mathf.Max(health - damage, 0);

            if (health <= 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * (health / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        void Die()
        {
            IsDead = true;
            anim.SetTrigger("Die");
            scheduler.CancelCurrentAction();
        }
        
        void AwardExperience(GameObject instigator)
        {
            BaseStats baseStats = instigator.GetComponent<BaseStats>();
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null) return;
            
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        void RegenerateHealth()
        {
            float regenHealth = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            health = Mathf.Max(health, regenHealth);
        }
        
        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float) state;
            
            if (health <= 0)
            {
                Die();
            }
        }
    }
}