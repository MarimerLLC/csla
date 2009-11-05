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
  Public Class CompanyContactList
	  Inherits BusinessListBase(Of CompanyContactList, CompanyContact)

#If SILVERLIGHT Then
	Public Sub New()
		MarkAsChild()
	End Sub
	Protected Overrides Sub AddNewCore()
	  Dim newContact As CompanyContact = CompanyContact.NewCompanyContact()
	  Add(newContact)
	End Sub
#Else
	Private Sub New()
		MarkAsChild()
	End Sub
	Protected Overrides Function AddNewCore() As Object
	  Dim newContact As CompanyContact = CompanyContact.NewCompanyContact()
	  Add(newContact)
	  Return newContact
	End Function
#End If



	Friend Shared Function NewCompanyContactList() As CompanyContactList
	  Return New CompanyContactList()
	End Function

#If (Not SILVERLIGHT) Then
	Friend Shared Function GetCompanyContactList(ByVal reader As SafeDataReader) As CompanyContactList
	  Return DataPortal.FetchChild(Of CompanyContactList)(reader)
	End Function

	Private Sub Child_Fetch(ByVal reader As SafeDataReader)
	  Me.RaiseListChangedEvents = False
	  Do While reader.Read()
		Add(CompanyContact.GetCompanyContact(reader))
	  Loop
	  Me.RaiseListChangedEvents = True
	End Sub

#End If


  End Class
End Namespace
