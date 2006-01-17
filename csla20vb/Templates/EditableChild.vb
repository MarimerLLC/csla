Imports System.Data.SqlClient

<Serializable()> _
Public Class EditableChild
  Inherits BusinessBase(Of EditableChild)

#Region " Business Methods "

  Private mId As Integer

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

#End Region

#Region " Factory Methods "

  Friend Shared Function NewEditableChild() As EditableChild

    Return New EditableChild

  End Function

  Friend Shared Function GetEditableChild(ByVal dr As SqlDataReader) As EditableChild

    Return New EditableChild(dr)

  End Function

  Private Sub New()
    MarkAsChild()
  End Sub

  Private Sub New(ByVal dr As SqlDataReader)

    MarkAsChild()
    Fetch(dr)

  End Sub

#End Region

#Region " Data Access "

  Private Sub Fetch(ByVal dr As SqlDataReader)
    ' TODO: load values
    MarkOld()
  End Sub

  Friend Sub Insert()
    ' TODO: insert values
    MarkOld()
  End Sub

  Friend Sub Update()
    ' TODO: update values
    MarkOld()
  End Sub

  Friend Sub DeleteSelf()
    ' TODO: delete values
    MarkNew()
  End Sub

#End Region

End Class
