#region License Revision: 0 Last Revised: 3/29/2006 8:21 AM
/******************************************************************************
Copyright (c) Microsoft Corporation.  All rights reserved.


This file is licensed under the Microsoft Public License (Ms-PL). A copy of the Ms-PL should accompany this file. 
If it does not, you can obtain a copy from: 

http://www.microsoft.com/resources/sharedsource/licensingbasics/publiclicense.mspx
******************************************************************************/
#endregion // License
using System.Diagnostics;

namespace AIStudio.Wpf.Controls
{
    /// <summary>
    /// Provides trace level switches for various components of the framework.
    /// </summary>
    static internal class TraceSwitches
    {
        private static TraceSwitch transitionsSw;

        /// <summary>
        /// Defines a trace switch for the transitions themselves.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        static public TraceSwitch Transitions
        {
            get
            {
                if (transitionsSw == null)
                {
                    transitionsSw = new TraceSwitch("Transitions", "Transition operations trace switch");
                }

                return transitionsSw;
            }
        }
    }
}
