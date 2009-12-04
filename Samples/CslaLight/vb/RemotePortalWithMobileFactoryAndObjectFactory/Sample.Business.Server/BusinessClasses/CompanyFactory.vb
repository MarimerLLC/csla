Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.DataPortalClient
Imports System.ComponentModel
Imports Csla.Validation
Imports Csla.Server

#If Not SILVERLIGHT = 1 Then
Imports System.Data.SqlClient
#End If

Public Class CompanyFactory
  Inherits ObjectFactory

  Private Function GetCompany(ByVal criteria As SingleCriteria(Of Company, Integer)) As Company

    Dim target As CompanyObjectFactoryTarget = Csla.DataPortal.Fetch(Of CompanyObjectFactoryTarget)(New SingleCriteria(Of CompanyObjectFactoryTarget, Integer)(criteria.Value))
    Dim returnValue As Company = Company.LoadCompany(target.CompanyId, target.CompanyName, New SmartDate(target.DateAdded))
    MarkOld(returnValue)
    Return returnValue
  End Function

  Private Function CreateCompany() As Company
    Dim target As CompanyObjectFactoryTarget = Csla.DataPortal.Create(Of CompanyObjectFactoryTarget)()
    Dim newCompany As Company = Company.LoadCompany(target.CompanyId, target.CompanyName, New SmartDate(target.DateAdded))
    newCompany.CheckRules()
    Return newCompany
  End Function

  Private Function SaveCompany(ByVal target As Company) As Company
    target.SetID(Csla.DataPortal.Update(Of CompanyObjectFactoryTarget)(CompanyObjectFactoryTarget.CloneCompany(target)).CompanyId)
    MarkOld(target)
    Return target
  End Function

End Class
