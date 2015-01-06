﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace PropertyTools.Wpf.ExtendedToolkit
{
    public class ExtendedToolkitDataGridControlFactory : DataGridControlFactory
    {
        public override FrameworkElement CreateDisplayControl(PropertyDefinition propertyDefinition, string bindingPath)
        {
            var ctl = CreateExtendedToolkitControl(propertyDefinition, bindingPath);
            if (ctl != null)
                return ctl;

            return base.CreateDisplayControl(propertyDefinition, bindingPath);
        }

        public override FrameworkElement CreateEditControl(PropertyDefinition propertyDefinition, string bindingPath)
        {
            var ctl = CreateExtendedToolkitControl(propertyDefinition, bindingPath);
            if (ctl != null)
                return ctl;

            return base.CreateEditControl(propertyDefinition, bindingPath);
        }

        public FrameworkElement CreateExtendedToolkitControl(PropertyDefinition propertyDefinition, string bindingPath)
        {
            var propertyType = propertyDefinition.PropertyType;
            if (propertyType.Is(typeof(DateTime)))
            {
                var c = new DateTimePicker()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(DateTimePicker.ValueProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(TimeSpan)))
            {
                var c = new TimeSpanUpDown()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(TimeSpanUpDown.ValueProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(int)) || propertyType.Is(typeof(int?)))
            {
                var c = new IntegerUpDown()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Minimum = int.MinValue,
                    Maximum = int.MaxValue,
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(IntegerUpDown.ValueProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(uint)) || propertyType.Is(typeof(uint?)))
            {
                var c = new LongUpDown()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Minimum = 0,
                    Maximum = uint.MaxValue,
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(LongUpDown.ValueProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(Brush)))
            {
                var c = new ColorBox.ColorBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };
                c.SetBinding(ColorBox.ColorBox.BrushProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(Guid)) || propertyType.Is(typeof(Guid?)))
            {
                var c = new MaskedTextBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA",
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(MaskedTextBox.TextProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            if (propertyType.Is(typeof(char)) || propertyType.Is(typeof(char?)))
            {
                var c = new MaskedTextBox()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Mask = "&",
                    IsReadOnly = propertyDefinition.IsReadOnly
                };
                c.SetBinding(MaskedTextBox.TextProperty, propertyDefinition.CreateBinding(bindingPath));
                return c;
            }

            return null;
        }
    }
}
