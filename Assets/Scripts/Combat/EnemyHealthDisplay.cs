using RPG.Attributes;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText;

        private Fighter fighter;

        void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            if (fighter.GetTarget() == null)
            {
                healthText.text = $"Enemy: N/A";
                return;
            }

            Health health = fighter.GetTarget();
            healthText.text = $"Enemy: {health.GetHealth():0} / {health.GetMaxHealth():0}";
        }
    }
}