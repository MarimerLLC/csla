<Serializable()> _
Public Class EditableRoot
  Inherits BusinessBase(Of EditableRoot)

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

  Public Shared Function NewEditableRoot() As EditableRoot
    Return DataPortal.Create(Of EditableRoot)()
  End Function

  Public Shared Function GetEditableRoot(ByVal id As Integer) As EditableRoot
    Return DataPortal.Fetch(Of EditableRoot)(New Criteria(id))
  End Function

  Public Shared Sub DeleteEditableRoot(ByVal id As Integer)
    DataPortal.Delete(New Criteria(id))
  End Sub

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    Private mId As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return mId
      End Get
    End Property
    Public Sub New(ByVal id As Integer)
      mId = id
    End Sub
  End Class

  Protected Overrides Sub DataPortal_Create()
    ' load default values
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
    ' load values
  End Sub

  Protected Overrides Sub DataPortal_Insert()
    ' insert values
  End Sub

  Protected Overrides Sub DataPortal_Update()
    ' update values
  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()
    DataPortal_Delete(New Criteria(mId))
  End Sub

  Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
    ' delete values
  End Sub

#End Region

End Class
