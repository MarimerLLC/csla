//INSTANT C# NOTE: Formerly VB.NET project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.ComponentModel;
using Csla.Validation;
using Csla.Server;

#if !SILVERLIGHT
using System.Data.SqlClient;
#endif

namespace Sample.Business
{
  public class CompanyFactory : ObjectFactory
  {

    private Company GetCompany(SingleCriteria<Company, int> criteria)
    {

      CompanyObjectFactoryTarget target = Csla.DataPortal.Fetch<CompanyObjectFactoryTarget>(new SingleCriteria<CompanyObjectFactoryTarget, int>(criteria.Value));
      Company returnValue = Company.LoadCompany(target.CompanyId, target.CompanyName, new SmartDate(target.DateAdded));
      MarkOld(returnValue);
      return returnValue;
    }

    private Company CreateCompany()
    {
      CompanyObjectFactoryTarget target = Csla.DataPortal.Create<CompanyObjectFactoryTarget>();
      Company newCompany = Company.LoadCompany(target.CompanyId, target.CompanyName, new SmartDate(target.DateAdded));
      newCompany.CheckRules();
      return newCompany;
    }

    private Company SaveCompany(Company target)
    {
      target.SetID(Csla.DataPortal.Update<CompanyObjectFactoryTarget>(CompanyObjectFactoryTarget.CloneCompany(target)).CompanyId);
      MarkOld(target);
      return target;
    }

  }

} //end of root namespace