Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.Silverlight
Imports Csla.Validation


#If(Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
Imports Csla.Data
#End If

Namespace Rolodex.Business.BusinessClasses
  <Serializable> _
  Public Class CompanyContactPhoneList
	  Inherits BusinessListBase(Of CompanyContactPhoneList,CompanyContactPhone)

#If SILVERLIGHT Then
	Public Sub New()
		MarkAsChild()
	End Sub
	Protected Overrides Sub AddNewCore()
	  Dim newContactPhone As CompanyContactPhone = CompanyContactPhone.NewCompanyContactPhone()
	  Add(newContactPhone)
	End Sub

#Else
	Private Sub New()
		MarkAsChild()
	End Sub
	Protected Overrides Function AddNewCore() As Object
	  Dim newContactPhone As CompanyContactPhone = CompanyContactPhone.NewCompanyContactPhone()
	  Add(newContactPhone)
	  Return newContactPhone
	End Function
#End If

	Friend Shared Function NewCompanyContactPhoneList() As CompanyContactPhoneList
	  Return New CompanyContactPhoneList()
	End Function
  End Class
End Namespace
