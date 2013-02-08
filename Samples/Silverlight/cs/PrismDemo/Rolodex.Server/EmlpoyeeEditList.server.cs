using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rolodex.Data;
using Csla;

namespace Rolodex
{
    public partial class EmlpoyeeEditList
    {
        private EmlpoyeeEditList() { }

        internal static EmlpoyeeEditList GetEmlpoyeeEditList(Company company)
        {
            return DataPortal.FetchChild<EmlpoyeeEditList>(company);
        }

        private void Child_Fetch(Company company)
        {
            company.Emlpoyees.ForEach(one => Add(EmlpoyeeEdit.GetEmlpoyeeEdit(one)));
        }
    }
}
