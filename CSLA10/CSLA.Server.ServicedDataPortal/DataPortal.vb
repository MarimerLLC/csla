Imports System.EnterpriseServices

<Transaction(TransactionOption.Required), EventTrackingEnabled(True)> _
Public Class DataPortal
  Inherits ServicedComponent

  <AutoComplete(True)> _
  Public Function Create(ByVal Criteria As Object, ByVal Principal As Object) As Object
    Dim portal As New CSLA.Server.DataPortal
    Return portal.Create(Criteria, Principal)
  End Function

  <AutoComplete(True)> _
  Public Function Fetch(ByVal Criteria As Object, ByVal Principal As Object) As Object
    Dim portal As New CSLA.Server.DataPortal
    Return portal.Fetch(Criteria, Principal)
  End Function

  <AutoComplete(True)> _
  Public Function Update(ByVal obj As Object, ByVal Principal As Object) As Object
    Dim portal As New CSLA.Server.DataPortal()
    Return portal.Update(obj, Principal)
  End Function

  <AutoComplete(True)> _
  Public Sub Delete(ByVal Criteria As Object, ByVal Principal As Object)
    Dim portal As New CSLA.Server.DataPortal
    portal.Delete(Criteria, Principal)
  End Sub

End Class
