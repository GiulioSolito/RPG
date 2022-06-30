using System;
using Opsive.UltimateInventorySystem.UI.Panels;
using UnityEngine;

namespace RPG.Control
{
    public class InventoryInputs : MonoBehaviour
    {
        [SerializeField] private DisplayPanel inventoryPanel;
        [SerializeField] private DisplayPanel equipmentPanel;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                inventoryPanel.ToggleOpenClose(inventoryPanel);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                equipmentPanel.ToggleOpenClose(equipmentPanel);
            }
        }
    }
}