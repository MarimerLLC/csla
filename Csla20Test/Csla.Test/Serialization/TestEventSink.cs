using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Serialization
{
    [Serializable( )]
    class TestEventSink
    {
        public void Reg(Core.BusinessBase obj)
        {
            obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler
                (PrivateOnIsDirtyChanged);
        }

        private void PrivateOnIsDirtyChanged(object sender, 
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["Test.PrivateOnIsDirtyChanged"] = 
                "Test.PrivateOnIsDirtyChanged";
        }

        public void OnIsDirtyChanged(object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            Csla.ApplicationContext.GlobalContext["Test.OnIsDirtyChanged"] = 
                "Test.OnIsDirtyChanged";
        }
    }
}