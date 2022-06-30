using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Equipping;
using RPG.Combat;
using UnityEngine;

namespace RPG.Control
{
    public class CharacterEquipper : Equipper
    {
        private Fighter fighter;

        protected override void Awake()
        {
            base.Awake();
            fighter = GetComponent<Fighter>();
        }

        public override bool Equip(Item item)
        {
            var result = base.Equip(item);

            fighter.EquipWeapon(item);
            
            return result;
        }
    }
}