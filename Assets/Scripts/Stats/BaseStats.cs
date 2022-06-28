using System;
using RPG.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [TabGroup("NPC Level")]
        [Range(1, 99)] [SerializeField] private int startingLevel = 1;
        [TabGroup("Class")]
        [SerializeField] private CharacterClass characterClass;
        [TabGroup("Progression")]
        [SerializeField] private Progression progression;
        [TabGroup("Level Up Particle")]
        [SerializeField] private GameObject levelUpParticleEffect;
        [TabGroup("Modifier")]
        [SerializeField] private bool shouldUseModifiers = false;
        
        public event Action onLevelUp;

        private int currentLevel = 0;

        private Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();
        }

        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        void Start()
        {
            currentLevel = CalculateLevel();
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel();

            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                LevelUp();
            }
        }

        void LevelUp()
        {
            Instantiate(levelUpParticleEffect, transform);

            onLevelUp?.Invoke();
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * (1 + GetPercentageModifiers(stat) / 100);
        }

        float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel()
        {
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();
            }

            return currentLevel;
        }

        float GetAdditiveModifiers(Stat stat)
        {
            if (shouldUseModifiers == false) return 0;
            
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        float GetPercentageModifiers(Stat stat)
        {
            if (shouldUseModifiers == false) return 0;
            
            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;            
        }

        int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null) return startingLevel;

            float currentXP = experience.GetExperience();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);

                if (xpToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
    }
}