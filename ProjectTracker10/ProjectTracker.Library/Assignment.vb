Imports System.Data.SqlClient

<Serializable()> _
Public MustInherit Class Assignment
  Inherits BusinessBase

  Protected mAssigned As New SmartDate(Now)
  Protected mRole As Integer = 0

#Region " Business Properties and Methods "

  Public ReadOnly Property Assigned() As String
    Get
      Return mAssigned.Text
    End Get
  End Property

  Public Property Role() As String
    Get
      Return Roles.Item(CStr(mRole))
    End Get
    Set(ByVal Value As String)
      If Role <> Value Then
        mRole = CInt(Roles.Key(Value))
        MarkDirty()
      End If
    End Set
  End Property

#End Region

#Region " Roles List "

  Private Shared mRoles As RoleList

  'Shared Sub New()
  '  mRoles = RoleList.GetList()
  'End Sub

  Public Shared ReadOnly Property Roles() As RoleList
    Get
      If mRoles Is Nothing Then
        mRoles = RoleList.GetList
      End If
      Return mRoles
    End Get
  End Property

  Protected Shared ReadOnly Property DefaultRole() As String
    Get
      ' return the first role in the list
      Return Roles.Item(0)
    End Get
  End Property

#End Region

#Region " Constructors "

  Protected Sub New()
    MarkAsChild()
  End Sub

#End Region

End Class
