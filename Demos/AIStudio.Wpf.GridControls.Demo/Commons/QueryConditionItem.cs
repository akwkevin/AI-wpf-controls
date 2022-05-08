using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Windows;
using AIStudio.Wpf.Controls.Bindings;
using AIStudio.Wpf.GridControls.Demo.Attributes;

namespace AIStudio.Wpf.GridControls.Demo.Commons
{
    public class QueryConditionItem : BaseControlItem
    {
        public static QueryConditionItem GetQueryConditionItem(PropertyInfo property)
        {      
            var queryConditionItem = new QueryConditionItem();
            if (GetControlItem(property, queryConditionItem) == false)
                return null;

            var attribute = ColumnHeaderAttribute.GetPropertyAttribute(property);
            if (attribute != null)
            {
                queryConditionItem.Visibility = attribute.IsPin ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                queryConditionItem.Visibility =  Visibility.Collapsed;
            }
            return queryConditionItem;

        }       
    }
}
