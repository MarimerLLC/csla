Imports System.EnterpriseServices

''' <summary>
''' Implements the transactional server-side DataPortal object as
''' discussed in Chapter 5.
''' </summary>
<Transaction(TransactionOption.Required), EventTrackingEnabled(True)> _
Public Class DataPortal
  Inherits ServicedComponent

  ''' <summary>
  ''' Invokes the server-side DataPortal Create method within
  ''' a COM+ transaction.
  ''' </summary>
  <AutoComplete(True)> _
  Public Function Create(ByVal Criteria As Object, ByVal context As DataPortalContext) As Object
    Dim portal As New CSLA.Server.DataPortal
    Return portal.Create(Criteria, context)
  End Function

  ''' <summary>
  ''' Invokes the server-side DataPortal Fetch method within
  ''' a COM+ transaction.
  ''' </summary>
  <AutoComplete(True)> _
  Public Function Fetch(ByVal Criteria As Object, ByVal context As DataPortalContext) As Object
    Dim portal As New CSLA.Server.DataPortal
    Return portal.Fetch(Criteria, context)
  End Function

  ''' <summary>
  ''' Invokes the server-side DataPortal Update method within
  ''' a COM+ transaction.
  ''' </summary>
  <AutoComplete(True)> _
  Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As Object
    Dim portal As New CSLA.Server.DataPortal
    Return portal.Update(obj, context)
  End Function

  ''' <summary>
  ''' Invokes the server-side DataPortal Delete method within
  ''' a COM+ transaction.
  ''' </summary>
  <AutoComplete(True)> _
  Public Sub Delete(ByVal Criteria As Object, ByVal context As DataPortalContext)
    Dim portal As New CSLA.Server.DataPortal
    portal.Delete(Criteria, context)
  End Sub

End Class
