//-----------------------------------------------------------------------
// <copyright file="TestEventSink.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Serialization
{
    [Serializable( )]
    public class TestEventSink
    {
        public void Reg(Core.BusinessBase obj)
        {
            obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
                (PrivateOnIsDirtyChanged);
            obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnIsDirtyChanged);
        }

        private void PrivateOnIsDirtyChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            TestResults.Add("Test.PrivateOnIsDirtyChanged", "Test.PrivateOnIsDirtyChanged");
            Console.WriteLine("PrivateOnIsDirtyChanged event handler output assigned with reg method");
        }

        public void OnIsDirtyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            TestResults.Add("Test.OnIsDirtyChanged", "Test.OnIsDirtyChanged");
        }
    }
}