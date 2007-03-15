Imports System.Windows.Forms
Imports System.Data.SqlClient

<Serializable()> _
Public Class DynamicRoot
  Inherits BusinessBase(Of DynamicRoot)

#Region " Business Methods "

  ' TODO: add your own fields, properties and methods
  Private mId As Integer

  Public Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return mId
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As Integer)
      CanWriteProperty(True)
      If mId <> value Then
        mId = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object
    Return mId
  End Function

#End Region

#Region " Validation Rules "

  Protected Overrides Sub AddBusinessRules()

    ' TODO: add validation rules
    'ValidationRules.AddRule(Nothing, "")

  End Sub

#End Region

#Region " Authorization Rules "

  Protected Overrides Sub AddAuthorizationRules()

    ' TODO: add authorization rules
    'AuthorizationRules.AllowWrite("", "")

  End Sub

  Public Shared Function CanAddObject() As Boolean
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanGetObject() As Boolean
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanEditObject() As Boolean
    Return ApplicationContext.User.IsInRole("")
  End Function

  Public Shared Function CanDeleteObject() As Boolean
    Return ApplicationContext.User.IsInRole("")
  End Function

#End Region

#Region " Factory Methods "

  Public Shared Function NewDynamicRoot() As DynamicRoot
    Return DataPortal.Create(Of DynamicRoot)()
  End Function

  Friend Shared Function GetDynamicRoot(ByVal dr As SqlDataReader) As DynamicRoot
    Return New DynamicRoot(dr)
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Private Sub New(ByVal dr As SqlDataReader)
    Fetch(dr)
  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Create()
    ' load default values
  End Sub

  Private Sub Fetch(ByVal dr As SqlDataReader)
    ' load values
  End Sub

  Protected Overrides Sub DataPortal_Insert()
    ' insert values
  End Sub

  Protected Overrides Sub DataPortal_Update()
    ' update values
  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()
    ' delete values
  End Sub

#End Region

End Class
