using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Equipping;
using RPG.Combat;
using RPG.Stats;
using UnityEngine;

namespace RPG.Control
{
    public class CharacterEquipper : Equipper
    {
        private BaseStats baseStats;

        protected override void Awake()
        {
            base.Awake();
            baseStats = GetComponent<BaseStats>();

            Opsive.Shared.Events.EventHandler.RegisterEvent(this, EventNames.c_Equipper_OnChange,
                UpdateCharacter);
        }

        void UpdateCharacter()
        {
            baseStats.Damage = baseStats.GetStat(Stat.Damage) + GetEquipmentStatInt("BaseAttack");
            baseStats.AttackRate = GetEquipmentStatInt("AttackRate");
            baseStats.AttackRange = GetEquipmentStatInt("AttackRange");

            if (GetEquippedItem(0) != null)
            {
                SetWeapon(GetEquippedItem(0));
            }
        }

        public void SetWeapon(Item item)
        {
            var runtimeController = GetComponent<Animator>().runtimeAnimatorController as AnimatorOverrideController;
            var overrideController = item.GetAttribute<Attribute<AnimatorOverrideController>>("AnimatorOverrider");
                
            
            if (overrideController != null)
            {
                if (GetEquippedItem(0) == null)
                {
                    GetComponent<Animator>().runtimeAnimatorController = runtimeController;
                }
                else
                {
                    GetComponent<Animator>().runtimeAnimatorController = overrideController.OverrideValue;
                }
            }
        }
    }
}