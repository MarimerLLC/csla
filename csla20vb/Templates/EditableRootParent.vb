Imports System.Data.SqlClient

<Serializable()> _
Public Class EditableRootParent
  Inherits BusinessBase(Of EditableRootParent)

#Region " Business Methods "

  Private mId As Integer
  Private mChildren As EditableChildList = _
    EditableChildList.NewEditableChildList()

  Public Property Id() As Integer
    Get
      CanReadProperty(True)
      Return mId
    End Get
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

  Public ReadOnly Property Children() As EditableChildList
    Get
      Return mChildren
    End Get
  End Property

  Public Overrides ReadOnly Property IsValid() As Boolean
    Get
      Return MyBase.IsValid AndAlso mChildren.IsValid
    End Get
  End Property

  Public Overrides ReadOnly Property IsDirty() As Boolean
    Get
      Return MyBase.IsDirty OrElse mChildren.IsDirty
    End Get
  End Property

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

  Public Shared Function NewEditableRootParent() As EditableRootParent
    Return DataPortal.Create(Of EditableRootParent)()
  End Function

  Public Shared Function GetEditableRootParent(ByVal id As Integer) As EditableRootParent
    Return DataPortal.Create(Of EditableRootParent)(New Criteria(id))
  End Function

  Public Shared Sub DeleteEditableRootParent(ByVal id As Integer)
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

  Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
    ' load default values
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
    ' load values
    Using dr As SqlDataReader = Nothing
      mChildren = EditableChildList.GetEditableChildList(dr)
    End Using
  End Sub

  Protected Overrides Sub DataPortal_Insert()
    ' insert values
    mChildren.Update(Me)
  End Sub

  Protected Overrides Sub DataPortal_Update()
    ' update values
    mChildren.Update(Me)
  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()
    DataPortal_Delete(New Criteria(mId))
  End Sub

  Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
    ' delete values
  End Sub

#End Region

End Class
