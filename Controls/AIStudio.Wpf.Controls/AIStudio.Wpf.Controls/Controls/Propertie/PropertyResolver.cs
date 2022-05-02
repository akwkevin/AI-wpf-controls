using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AIStudio.Wpf.Controls
{
    public class PropertyResolver
    {
        private static readonly Dictionary<Type, EditorTypeCode> TypeCodeDic = new Dictionary<Type, EditorTypeCode>()
        {
            [typeof(string)] = EditorTypeCode.PlainText,
            [typeof(sbyte)] = EditorTypeCode.SByteNumber,
            [typeof(byte)] = EditorTypeCode.ByteNumber,
            [typeof(short)] = EditorTypeCode.Int16Number,
            [typeof(ushort)] = EditorTypeCode.UInt16Number,
            [typeof(int)] = EditorTypeCode.Int32Number,
            [typeof(uint)] = EditorTypeCode.UInt32Number,
            [typeof(long)] = EditorTypeCode.Int64Number,
            [typeof(ulong)] = EditorTypeCode.UInt64Number,
            [typeof(float)] = EditorTypeCode.SingleNumber,
            [typeof(double)] = EditorTypeCode.DoubleNumber,
            [typeof(bool)] = EditorTypeCode.Switch,
            [typeof(DateTime)] = EditorTypeCode.DateTime,
            [typeof(HorizontalAlignment)] = EditorTypeCode.HorizontalAlignment,
            [typeof(VerticalAlignment)] = EditorTypeCode.VerticalAlignment,
            [typeof(ImageSource)] = EditorTypeCode.ImageSource,
            [typeof(Color)] = EditorTypeCode.Color
        };

        public string ResolveCategory(PropertyDescriptor propertyDescriptor)
        {
            var categoryAttribute = propertyDescriptor.Attributes.OfType<CategoryAttribute>().FirstOrDefault();

            return categoryAttribute == null || string.IsNullOrEmpty(categoryAttribute.Category) ? "杂项" : categoryAttribute.Category;
        }

        public string ResolveDisplayName(PropertyDescriptor propertyDescriptor)
        {
            var displayName = propertyDescriptor.DisplayName;
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = propertyDescriptor.Name;
            }

            return displayName;
        }

        public string ResolveDescription(PropertyDescriptor propertyDescriptor) => propertyDescriptor.Description;

        public bool ResolveIsBrowsable(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsBrowsable;

        public bool ResolveIsDisplay(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsLocalizable;

        public bool ResolveIsReadOnly(PropertyDescriptor propertyDescriptor) => propertyDescriptor.IsReadOnly;

        public object ResolveDefaultValue(PropertyDescriptor propertyDescriptor)
        {
            var defaultValueAttribute = propertyDescriptor.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
            return defaultValueAttribute?.Value;
        }

        public PropertyEditorBase ResolveEditor(PropertyDescriptor propertyDescriptor)
        {
            var editorAttribute = propertyDescriptor.Attributes.OfType<EditorAttribute>().FirstOrDefault();
            var editor = editorAttribute == null || string.IsNullOrEmpty(editorAttribute.EditorTypeName)
                ? CreateDefaultEditor(propertyDescriptor.PropertyType)
                : CreateEditor(Type.GetType(editorAttribute.EditorTypeName));

            return editor;
        }

        public virtual PropertyEditorBase CreateDefaultEditor(Type type)
        {
            EditorTypeCode editorType;
            if (TypeCodeDic.TryGetValue(type, out editorType))
            {
                switch (editorType)
                {
                    case EditorTypeCode.PlainText: return new PlainTextPropertyEditor();
                    case EditorTypeCode.SByteNumber: return new NumberPropertyEditor(sbyte.MinValue, sbyte.MaxValue);
                    case EditorTypeCode.ByteNumber: return new NumberPropertyEditor(byte.MinValue, byte.MaxValue);
                    case EditorTypeCode.Int16Number: return new NumberPropertyEditor(short.MinValue, short.MaxValue);
                    case EditorTypeCode.UInt16Number: return new NumberPropertyEditor(ushort.MinValue, ushort.MaxValue);
                    case EditorTypeCode.Int32Number: return new NumberPropertyEditor(int.MinValue, int.MaxValue);
                    case EditorTypeCode.UInt32Number: return new NumberPropertyEditor(uint.MinValue, uint.MaxValue);
                    case EditorTypeCode.Int64Number: return new NumberPropertyEditor(long.MinValue, long.MaxValue);
                    case EditorTypeCode.UInt64Number: return new NumberPropertyEditor(ulong.MinValue, ulong.MaxValue);
                    case EditorTypeCode.SingleNumber: return new NumberPropertyEditor(float.MinValue, float.MaxValue);
                    case EditorTypeCode.DoubleNumber: return new NumberPropertyEditor(double.MinValue, double.MaxValue);
                    case EditorTypeCode.Switch: return new SwitchPropertyEditor();
                    case EditorTypeCode.DateTime: return new DateTimePropertyEditor();
                    case EditorTypeCode.HorizontalAlignment: return new HorizontalAlignmentPropertyEditor();
                    case EditorTypeCode.VerticalAlignment: return new VerticalAlignmentPropertyEditor();
                    case EditorTypeCode.ImageSource: return new ImagePropertyEditor();
                    case EditorTypeCode.Color: return new ColorPropertyEditor();
                    default: return new ReadOnlyTextPropertyEditor();
                }
            }
            else
            {
                if (type.IsSubclassOf(typeof(Enum)))
                {
                    return new EnumPropertyEditor();
                }
                else
                {
                    return new ReadOnlyTextPropertyEditor();
                }
            }
        }

        public virtual PropertyEditorBase CreateEditor(Type type) => Activator.CreateInstance(type) as PropertyEditorBase ?? new ReadOnlyTextPropertyEditor();

        private enum EditorTypeCode
        {
            PlainText,
            SByteNumber,
            ByteNumber,
            Int16Number,
            UInt16Number,
            Int32Number,
            UInt32Number,
            Int64Number,
            UInt64Number,
            SingleNumber,
            DoubleNumber,
            Switch,
            DateTime,
            HorizontalAlignment,
            VerticalAlignment,
            ImageSource,
            Color,
            Brush
        }
    }
}
