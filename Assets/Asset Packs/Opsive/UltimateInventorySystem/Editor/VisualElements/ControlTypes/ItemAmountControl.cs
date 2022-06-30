﻿/// ---------------------------------------------
/// Ultimate Inventory System
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateInventorySystem.Editor.VisualElements.ControlTypes
{
    using Opsive.Shared.Editor.UIElements;
    using Opsive.Shared.Editor.UIElements.Controls;
    using Opsive.Shared.Editor.UIElements.Controls.Types;
    using Opsive.UltimateInventorySystem.Core;
    using Opsive.UltimateInventorySystem.Core.DataStructures;
    using Opsive.UltimateInventorySystem.Storage;
    using System;
    using System.Reflection;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using Object = UnityEngine.Object;

    /// <summary>
    /// The control type for an item amount.
    /// </summary>
    [ControlType(typeof(ItemAmount))]
    public class ItemAmountControl : TypeControlBase
    {
        protected InventorySystemDatabase m_Database;
        protected VisualElement m_ItemAmountFieldContainer;
        protected IntegerField m_IntegerField;
        protected ItemField m_ItemField;

        public override bool UseLabel => true;

        /// <summary>
        /// Returns the control that should be used for the specified ControlType.
        /// </summary>
        /// <param name="unityObject">A reference to the owning Unity Object.</param>
        /// <param name="target">The object that should have its fields displayed.</param>
        /// <param name="field">The field responsible for the control (can be null).</param>
        /// <param name="arrayIndex">The index of the object within the array (-1 indicates no array).</param>
        /// <param name="type">The type of control being retrieved.</param>
        /// <param name="value">The value of the control.</param>
        /// <param name="onChangeEvent">An event that is sent when the value changes. Returns false if the control cannot be changed.</param>
        /// <param name="userData">Optional data which can be used by the controls.</param>
        /// <returns>The created control.</returns>
        public override VisualElement GetControl(Object unityObject, object target, FieldInfo field, int arrayIndex, Type type, object value,
            Func<object, bool> onChangeEvent, object userData)
        {
            if (userData is object[] objArray) {
                for (int i = 0; i < objArray.Length; i++) {
                    if (objArray[i] is bool b) {
                        if (b == false) { return null; }
                    }
                    if (objArray[i] is InventorySystemDatabase database) { m_Database = database; }
                }
            } else if (userData is InventorySystemDatabase database) {
                m_Database = database;
            }

            var itemAmount = (ItemAmount)value;

            m_ItemAmountFieldContainer = new VisualElement();

            m_IntegerField = new IntegerField("Amount");
            m_IntegerField.SetValueWithoutNotify(itemAmount.Amount);
            m_IntegerField.RegisterValueChangedCallback(evt =>
            {
                if (!(onChangeEvent?.Invoke(new ItemAmount(m_ItemField.Value, m_IntegerField.value)) ?? false)) {
                    m_IntegerField.SetValueWithoutNotify(evt.newValue);
                }
            });
            m_ItemAmountFieldContainer.Add(m_IntegerField);

            m_ItemField = new ItemField(m_Database);
            m_ItemField.Refresh(itemAmount.Item);
            // Ensure the control is kept up to date as the value changes.
            if (field != null) {
                Action<object> onBindingUpdateEvent = (object newValue) =>
                {
                    var newItemAmount = (ItemAmount)newValue;
                    m_IntegerField.SetValueWithoutNotify(newItemAmount.Amount);
                    m_ItemField.Refresh(newItemAmount.Item);
                };
                m_ItemField.RegisterCallback<AttachToPanelEvent>(c =>
                {
                    BindingUpdater.AddBinding(field, arrayIndex, target, onBindingUpdateEvent);
                });
                m_ItemField.RegisterCallback<DetachFromPanelEvent>(c =>
                {
                    BindingUpdater.RemoveBinding(onBindingUpdateEvent);
                });
            }
            m_ItemField.OnValueChanged += () =>
            {
                if (!(onChangeEvent?.Invoke(new ItemAmount(m_ItemField.Value, m_IntegerField.value)) ?? false)) {
                    m_ItemField.Refresh(value as Item);
                }
            };
            m_ItemAmountFieldContainer.Add(m_ItemField);

            return m_ItemAmountFieldContainer;
        }
    }
}