#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// TypeConverter to convert Transition to/from other types.
    /// Currently only <see cref="string"/> is supported.
    /// </summary>
    public class TransitionConverter : TypeConverter
    {
        /// <summary>
        /// Cached value for GetStandardValues
        /// </summary>
        //private static TypeConverter.StandardValuesCollection _standardValues;

        /// <summary>
        /// TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="sourceType">Type to convert from</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == null)
            {
                throw new ArgumentNullException("sourceType");
            }
            // We can only handle strings
            return sourceType == typeof(string);
        }

        /// <summary>
        /// TypeConverter method override.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="destinationType">Type to convert to</param>
        /// <returns>true if conversion is possible</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            // We can convert to an InstanceDescriptor or to a string.
            return destinationType == typeof(InstanceDescriptor) ||
                   destinationType == typeof(string);
        }

        /// <summary>
        /// TypeConverter method implementation.
        /// </summary>
        /// <param name="context">ITypeDescriptorContext</param>
        /// <param name="culture">Current culture (see CLR specs)</param>
        /// <param name="value">value to convert from</param>
        /// <returns>value that is result of conversion</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // Try to get the value as a string
            string strValue = value as string;

            // Only continue if valid string
            if (!string.IsNullOrEmpty(strValue))
            {
                // Try to get the type
                Type transType = Type.GetType(strValue);

                // Only continue if we got a valid type
                if ((transType != null) && (typeof(Transition).IsAssignableFrom(transType)))
                {
                    // Create and return the transition instance
                    return Activator.CreateInstance(transType);
                }
            }

            // Not found. Try default base conversion.
            return base.ConvertFrom(context, culture, value);
        }
    }
}
