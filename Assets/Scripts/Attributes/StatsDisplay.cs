using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class StatsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI levelText;

        private Health health;
        private Experience experience;
        private BaseStats baseStats;

        void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            healthText.text = $"Health: {health.GetHealth():0} / {health.GetMaxHealth():0}";
            experienceText.text = $"XP: {experience.GetExperience():0}";
            levelText.text = $"Level: {baseStats.GetLevel():0}";
        }
    }
}