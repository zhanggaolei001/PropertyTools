﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataGridOperator.cs" company="PropertyTools">
//   Copyright (c) 2014 PropertyTools contributors
// </copyright>
// <summary>
//   Represents an abstract base class for DataGrid operators.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PropertyTools.Wpf
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Windows;

    using PropertyTools.DataAnnotations;

    using HorizontalAlignment = System.Windows.HorizontalAlignment;

    /// <summary>
    /// Represents an abstract base class for <see cref="DataGrid" /> operators.
    /// </summary>
    /// <remarks>An operator implements operations for a <see cref="DataGrid" /> based on the different data its 
    /// <see cref="DataGrid.ItemsSource" /> binds to.</remarks>
    public abstract class DataGridOperator : IDataGridOperator
    {
        /// <summary>
        /// The property descriptors.
        /// </summary>
        private readonly Dictionary<PropertyDefinition, PropertyDescriptor> descriptors = new Dictionary<PropertyDefinition, PropertyDescriptor>();

        /// <summary>
        /// Gets or sets the default horizontal alignment.
        /// </summary>
        /// <value>
        /// The default horizontal alignment.
        /// </value>
        public HorizontalAlignment DefaultHorizontalAlignment { get; set; } = HorizontalAlignment.Center;

        /// <summary>
        /// Gets or sets the default column width.
        /// </summary>
        /// <value>
        /// The default width of the columns.
        /// </value>
        public GridLength DefaultColumnWidth { get; set; } = new GridLength(1, GridUnitType.Star);

        /// <summary>
        /// Determines whether columns can be deleted.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        ///   <c>true</c> if columns can be deleted; otherwise <c>false</c>.
        /// </returns>
        public virtual bool CanDeleteColumns(DataGrid owner)
        {
            var list = owner.ItemsSource;
            return owner.CanDelete && owner.ItemsInColumns && list != null && !list.IsFixedSize;
        }

        /// <summary>
        /// Determines whether rows can be deleted.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        ///   <c>true</c> if rows can be deleted; otherwise <c>false</c>.
        /// </returns>
        public virtual bool CanDeleteRows(DataGrid owner)
        {
            var list = owner.ItemsSource;
            return owner.CanDelete && owner.ItemsInRows && list != null && !list.IsFixedSize;
        }

        /// <summary>
        /// Determines whether columns can be inserted.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        ///   <c>true</c> if columns can be inserted; otherwise <c>false</c>.
        /// </returns>
        public virtual bool CanInsertColumns(DataGrid owner)
        {
            var list = owner.ItemsSource;
            return owner.ItemsInColumns && owner.CanInsert && list != null && !list.IsFixedSize;
        }

        /// <summary>
        /// Determines whether rows can be inserted.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        ///   <c>true</c> if rows can be inserted; otherwise <c>false</c>.
        /// </returns>
        public virtual bool CanInsertRows(DataGrid owner)
        {
            var list = owner.ItemsSource;
            return owner.ItemsInRows && owner.CanInsert && list != null && !list.IsFixedSize;
        }

        /// <summary>
        /// Deletes the item at the specified index.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        ///   <c>true</c> if rows can be inserted; otherwise <c>false</c>.
        /// </returns>
        protected virtual bool DeleteItem(DataGrid owner, int index)
        {
            var list = owner.ItemsSource;
            index = this.GetItemsSourceIndex(owner, index);
            if (list == null)
            {
                return false;
            }

            if (index < 0 || index >= list.Count)
            {
                return false;
            }

            list.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Deletes the columns.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The index.</param>
        /// <param name="n">The number of columns to delete.</param>
        public virtual void DeleteColumns(DataGrid owner, int index, int n)
        {
            if (!owner.ItemsInColumns)
            {
                return;
            }

            for (var i = index + n - 1; i >= index; i--)
            {
                this.DeleteItem(owner, i);
            }
        }

        /// <summary>
        /// Deletes the rows.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The index.</param>
        /// <param name="n">The number of rows to delete.</param>
        public virtual void DeleteRows(DataGrid owner, int index, int n)
        {
            for (var i = index + n - 1; i >= index; i--)
            {
                this.DeleteItem(owner, i);
            }
        }

        /// <summary>
        /// Inserts columns at the specified index.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The index.</param>
        /// <param name="n">The number of columns to insert.</param>
        public virtual void InsertColumns(DataGrid owner, int index, int n)
        {
            if (!owner.ItemsInColumns) return;

            for (var i = 0; i < n; i++)
            {
                this.InsertItem(owner, index);
            }
        }

        /// <summary>
        /// Inserts rows at the specified index.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The index.</param>
        /// <param name="n">The number of rows to insert.</param>
        public virtual void InsertRows(DataGrid owner, int index, int n)
        {
            for (var i = 0; i < n; i++)
            {
                this.InsertItem(owner, index);
            }
        }

        /// <summary>
        /// Gets the number of items.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <returns>The number.</returns>
        public virtual int GetItemCount(DataGrid owner)
        {
            return owner.CollectionView.Cast<object>().Count();
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        /// The number.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual int GetRowCount(DataGrid owner)
        {
            return owner.ItemsInRows ? this.GetItemCount(owner) : owner.PropertyDefinitions.Count;
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <returns>
        /// The number.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual int GetColumnCount(DataGrid owner)
        {
            return owner.ItemsInRows ? owner.PropertyDefinitions.Count : this.GetItemCount(owner);
        }

        /// <summary>
        /// Determines whether items can be sorted by the specified column/row index.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="index">The column index if items are in rows, otherwise the row index.</param>
        /// <returns>
        ///   <c>true</c> if the items can be sorted; <c>false</c> otherwise.
        /// </returns>
        public virtual bool CanSort(DataGrid owner, int index)
        {
            return owner.PropertyDefinitions[index].CanSort;
        }

        /// <summary>
        /// Gets the value in the specified cell.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell.</param>
        /// <returns>The value</returns>
        public virtual object GetCellValue(DataGrid owner, CellRef cell)
        {
            if (cell.Column < 0 || cell.Column >= owner.Columns || cell.Row < 0 || cell.Row >= owner.Rows)
            {
                return null;
            }

            var item = this.GetItem(owner, cell);
            if (item != null)
            {
                var pd = owner.GetPropertyDefinition(cell);
                if (pd != null)
                {
                    var descriptor = this.GetPropertyDescriptor(pd);
                    if (descriptor != null)
                    {
                        return descriptor.GetValue(item);
                    }

                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the property in the specified cell.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="currentValue">The current value.</param>
        /// <returns>
        /// The type of the property.
        /// </returns>
        public virtual Type GetPropertyType(PropertyDefinition definition, CellRef cell, object currentValue)
        {
            var descriptor = this.GetPropertyDescriptor(definition);
            if (descriptor?.PropertyType == null)
            {
                return currentValue?.GetType() ?? typeof(object);
            }

            return descriptor.PropertyType;
        }

        /// <summary>
        /// Converts the collection view index to an items source index.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="index">The index in the collection view.</param>
        /// <returns>The index in the items source</returns>
        public virtual int GetItemsSourceIndex(DataGrid owner, int index)
        {
            if (owner.CollectionView == null)
            {
                return index;
            }

            // if not using custom sort, and not sorting
            if (owner.CustomSort == null && owner.CollectionView.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            // if using custom sort, and not sorting
            var sdc = owner.CustomSort as ISortDescriptionComparer;
            if (sdc != null && sdc.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            // get the item at the specified index in the collection view
            // TODO: find a better way to do this
            object item;
            if (!TryGetByIndex(owner.CollectionView, index, out item))
            {
                throw new InvalidOperationException("The collection view is probably out of sync. (GetItemsSourceIndex)");
            }

            // get the index of the item in the items source
            var i = owner.ItemsSource.IndexOf(item);
            
            // Debug.WriteLine(index + " -> " + i);
            return i;
        }

        /// <summary>
        /// Converts the items source index to a collection view index.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="index">The index in the items source.</param>
        /// <returns>The index in the collection view</returns>
        public virtual int GetCollectionViewIndex(DataGrid owner, int index)
        {
            if (owner.CollectionView == null)
            {
                return index;
            }

            // if not using custom sort, and not sorting
            if (owner.CustomSort == null && owner.CollectionView.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            // if using custom sort, and not sorting
            var sdc = owner.CustomSort as ISortDescriptionComparer;
            if (sdc != null && sdc.SortDescriptions.Count == 0)
            {
                // return the same index
                return index;
            }

            if (index < 0 || index >= owner.ItemsSource.Count)
            {
                throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
            }

            // get the item at the specified index in the items source
            var item = owner.ItemsSource[index];

            // get the index of the item in the collection view
            // TODO: find a better way to do this
            int index2;
            if (!TryGetIndex(owner.CollectionView, item, out index2))
            {
                throw new InvalidOperationException("The collection view is probably out of sync. (GetCollectionViewIndex)");
            }

            return index2;
        }

        /// <summary>
        /// Gets the item in the specified cell.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell reference.</param>
        /// <returns>
        /// The <see cref="object" />.
        /// </returns>au
        public abstract object GetItem(DataGrid owner, CellRef cell);

        /// <summary>
        /// Inserts an item to <see cref="DataGrid" /> at the specified index.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The index of the inserted item if insertion is successful, <c>-1</c> otherwise.
        /// </returns>
        public abstract int InsertItem(DataGrid owner, int index);

        /// <summary>
        /// Sets value of the specified cell to the specified value.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell to change.</param>
        /// <param name="value">The value.</param>
        public abstract void SetValue(DataGrid owner, CellRef cell, object value);

        /// <summary>
        /// Auto-generates the columns.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public virtual void AutoGenerateColumns(DataGrid owner)
        {
            foreach (var cd in this.GenerateColumnDefinitions(owner.ItemsSource))
            {
                owner.ColumnDefinitions.Add(cd);
            }
        }

        /// <summary>
        /// Updates the property definitions.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public virtual void UpdatePropertyDefinitions(DataGrid owner)
        {
            this.descriptors.Clear();

            // Set the property descriptors.
            var itemType = TypeHelper.GetItemType(owner.ItemsSource);
            var properties = TypeDescriptor.GetProperties(itemType);
            foreach (var pd in owner.PropertyDefinitions)
            {
                if (!string.IsNullOrEmpty(pd.PropertyName))
                {
                    var descriptor = properties[pd.PropertyName];
                    this.SetPropertiesFromDescriptor(pd, descriptor);
                    this.descriptors[pd] = descriptor;
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the specified type.
        /// </summary>
        /// <param name="owner">The data grid.</param>
        /// <param name="itemType">The type.</param>
        /// <returns>
        /// The new instance.
        /// </returns>
        protected virtual object CreateItem(DataGrid owner, Type itemType)
        {
            if (itemType == typeof(string))
            {
                return string.Empty;
            }

            if (itemType == typeof(double))
            {
                return 0.0;
            }

            if (itemType == typeof(int))
            {
                return 0;
            }

            if (owner.CreateItem != null)
            {
                return owner.CreateItem();
            }

            // TODO: the item type may not have a parameterless constructor!
            try
            {
                return Activator.CreateInstance(itemType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <param name="pd">The property definition.</param>
        /// <returns>The property descriptor.</returns>
        public virtual PropertyDescriptor GetPropertyDescriptor(PropertyDefinition pd)
        {
            PropertyDescriptor descriptor;
            return this.descriptors.TryGetValue(pd, out descriptor) ? descriptor : null;
        }

        /// <summary>
        /// Sets the properties from descriptor.
        /// </summary>
        /// <param name="pd">The property definition.</param>
        /// <param name="descriptor">The descriptor.</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected virtual void SetPropertiesFromDescriptor(PropertyDefinition pd, PropertyDescriptor descriptor)
        {
            if (descriptor == null)
            {
                return;
            }

            if (descriptor.GetAttributeValue<System.ComponentModel.ReadOnlyAttribute, bool>(a => a.IsReadOnly)
                || descriptor.GetAttributeValue<DataAnnotations.ReadOnlyAttribute, bool>(a => a.IsReadOnly)
                || descriptor.IsReadOnly)
            {
                pd.IsReadOnly = true;
            }

            if (descriptor.GetAttributeValue<System.ComponentModel.DataAnnotations.EditableAttribute, bool>(a => a.AllowEdit)
                || descriptor.GetAttributeValue<DataAnnotations.EditableAttribute, bool>(a => a.AllowEdit))
            {
                pd.IsEditable = true;
            }

            var ispa = descriptor.GetFirstAttributeOrDefault<ItemsSourcePropertyAttribute>();
            if (ispa != null)
            {
                pd.ItemsSourceProperty = ispa.PropertyName;
            }

            var svpa = descriptor.GetFirstAttributeOrDefault<SelectedValuePathAttribute>();
            if (svpa != null)
            {
                pd.SelectedValuePath = svpa.Path;
            }

            var dmpa = descriptor.GetFirstAttributeOrDefault<DisplayMemberPathAttribute>();
            if (dmpa != null)
            {
                pd.DisplayMemberPath = dmpa.Path;
            }

            var eba = descriptor.GetFirstAttributeOrDefault<EnableByAttribute>();
            if (eba != null)
            {
                pd.IsEnabledByProperty = eba.PropertyName;
                pd.IsEnabledByValue = eba.PropertyValue;
            }
        }

        /// <summary>
        /// Generates column definitions based on a list of items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A sequence of column definitions.</returns>
        protected abstract IEnumerable<ColumnDefinition> GenerateColumnDefinitions(IList list);

        /// <summary>
        /// Gets the binding path for the specified cell.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell.</param>
        /// <returns>
        /// The binding path
        /// </returns>
        public abstract string GetBindingPath(DataGrid owner, CellRef cell);

        /// <summary>
        /// Tries to convert an object to the specified type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="convertedValue">The converted value.</param>
        /// <returns>
        /// True if conversion was successful.
        /// </returns>
        private static bool TryConvert(object value, Type targetType, out object convertedValue)
        {
            try
            {
                if (value != null && targetType == value.GetType())
                {
                    convertedValue = value;
                    return true;
                }

                if (value == null)
                {
                    // reference types
                    if (!targetType.IsValueType)
                    {
                        convertedValue = null;
                        return true;
                    }

                    // nullable types
                    if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        convertedValue = null;
                        return true;
                    }
                }

                if (targetType == typeof(string))
                {
                    convertedValue = value?.ToString();
                    return true;
                }

                if (targetType == typeof(double))
                {
                    convertedValue = Convert.ToDouble(value);
                    return true;
                }

                if (targetType == typeof(int))
                {
                    convertedValue = Convert.ToInt32(value);
                    return true;
                }

                if (targetType == typeof(bool))
                {
                    var s = value as string;
                    if (s != null)
                    {
                        convertedValue = !string.IsNullOrEmpty(s) && s != "0";
                        return true;
                    }

                    convertedValue = Convert.ToBoolean(value);
                    return true;
                }

                var converter = TypeDescriptor.GetConverter(targetType);
                if (value != null && converter.CanConvertFrom(value.GetType()))
                {
                    convertedValue = converter.ConvertFrom(value);
                    return true;
                }

                if (value != null)
                {
                    var parseMethod = targetType.GetMethod("Parse", new[] { value.GetType(), typeof(IFormatProvider) });
                    if (parseMethod != null)
                    {
                        convertedValue = parseMethod.Invoke(null, new[] { value, CultureInfo.CurrentCulture });
                        return true;
                    }
                }

                convertedValue = null;
                return false;
            }
            catch
            {
                convertedValue = null;
                return false;
            }
        }

        /// <summary>
        /// Tries to set cell value in the specified cell.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the cell value was set.</returns>
        public virtual bool TrySetCellValue(DataGrid owner, CellRef cell, object value)
        {
            if (owner.ItemsSource == null)
            {
                return false;
            }

            var current = this.GetItem(owner, cell);

            var pd = owner.GetPropertyDefinition(cell);
            if (pd == null)
            {
                return false;
            }

            if (current == null || pd.IsReadOnly)
            {
                return false;
            }

            object convertedValue;
            var targetType = this.GetPropertyType(pd, cell, current);
            if (!TryConvert(value, targetType, out convertedValue))
            {
                return false;
            }

            var descriptor = this.GetPropertyDescriptor(pd);
            if (descriptor != null)
            {
                descriptor.SetValue(current, convertedValue);
            }
            else
            {
                this.SetValue(owner, cell, convertedValue);
            }

            return true;
        }

        /// <summary>
        /// Gets the data context for the specified cell.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="cell">The cell.</param>
        /// <returns>The context object.</returns>
        public object GetDataContext(DataGrid owner, CellRef cell)
        {
            var pd = owner.GetPropertyDefinition(cell);
            var item = this.GetItem(owner, cell);
            return pd.PropertyName != null ? item : owner.ItemsSource;
        }

        /// <summary>
        /// Tries to get the item of the specified index.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///   <c>false</c> if the index was not found in the sequence; otherwise <c>true</c>.
        /// </returns>
        private bool TryGetByIndex(IEnumerable sequence, int index, out object item)
        {
            var i = 0;
            foreach (var current in sequence)
            {
                if (i == index)
                {
                    item = current;
                    return true;
                }

                i++;
            }

            item = null;
            return false;
        }

        /// <summary>
        /// Tries to get the index of the specified item.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        /// <param name="item">The item.</param>
        /// <param name="index">The index.</param>
        /// <returns><c>false</c> if the item was not found in the sequence; otherwise <c>true</c>.</returns>
        private bool TryGetIndex(IEnumerable sequence, object item, out int index)
        {
            var i = 0;
            foreach (var current in sequence)
            {
                if (current == item || (current?.Equals(item) ?? false))
                {
                    index = i;
                    return true;
                }

                i++;
            }

            index = -1;
            return false;
        }
    }
}