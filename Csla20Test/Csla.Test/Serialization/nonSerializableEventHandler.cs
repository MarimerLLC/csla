using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Serialization
{
    public class nonSerializableEventHandler
    {
        private int _count = 0;

        public void Reg(Csla.Core.BusinessBase obj)
        {
            obj.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(obj_PropertyChanged);
        }

        public void obj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _count += 1;
            Console.WriteLine(_count.ToString());
            Csla.ApplicationContext.GlobalContext["PropertyChangedFiredCount"] = _count;
        }
    }
}
