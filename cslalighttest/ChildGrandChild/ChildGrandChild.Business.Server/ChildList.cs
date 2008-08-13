using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;



namespace ChildGrandChild.Business
{
    [Serializable]

    public partial class ChildList : BusinessListBase<ChildList, Child>
    {
      //  protected override object AddNewCore()
      //  {
      //      Child item = new Child();
      //      Add(item);
      //      return item;
      //  }

        public ChildList()
        {
            AllowNew = true;
        }

        public void DumpEditLevels(StringBuilder sb)
        {
            sb.AppendFormat("  {0} {1}: {2}\r", this.GetType().Name, "n/a", this.EditLevel);
            foreach (Child item in DeletedList)
                item.DumpEditLevels(sb);
            foreach (Child item in this)
                item.DumpEditLevels(sb);
        }

        public static void FetchByName(string name, EventHandler<DataPortalResult<ChildList>> completed)
        {
            DataPortal<ChildList> dp = new DataPortal<ChildList>();
            dp.FetchCompleted += completed;
            dp.BeginFetch(new SingleCriteria<ChildList, string>(name));
        }

        
       
    }
}
