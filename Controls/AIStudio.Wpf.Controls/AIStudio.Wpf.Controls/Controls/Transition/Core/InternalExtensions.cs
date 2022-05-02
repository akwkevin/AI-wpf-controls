#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System;
using System.Collections;

namespace AIStudio.Wpf.Controls
{
    internal static class InternalExtensions
    {
        /// <summary>
        /// Determines if an item of the specified type exists within the collection.
        /// </summary>
        /// <param name="type">
        /// The type to search for.
        /// </param>
        /// <returns>
        /// <c>true</c> if an item of the specified type is found; otherwise <c>false</c>.
        /// </returns>
        static public bool ContainsType(this IList list, Type type)
        {
            // If no items, skip
            if (list == null) return false;

            // Search each item
            foreach (object compare in list)
            {
                if (compare.GetType() == type)
                {
                    return true;
                }
            }

            // Not found
            return false;
        }

        /// <summary>
        /// Indicates if the type can be created as a specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type to be created as.
        /// </typeparam>
        /// <param name="type">
        /// The type to try and create.
        /// </param>
        /// <returns>
        /// <c>true</c> if the type can be created as <typeparamref name="T"/>.
        /// </returns>
        static public bool IsCreatableAs<T>(this Type type)
        {
            // Validate parameters
            if (type == null) throw new ArgumentNullException("type");

            // Make sure type matches
            if (!typeof(T).IsAssignableFrom(type)) return false;

            // Make sure it's public
            if (type.IsNotPublic) return false;

            // Make sure it's not a generic type
            if (type.IsGenericType) return false;

            // Make sure it's not an interface
            if (type.IsInterface) return false;

            // Make sure it's not abstract
            if (type.IsAbstract) return false;

            // Valid
            return true;
        }

    }
}
