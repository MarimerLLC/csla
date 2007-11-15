Imports System.Data.SqlClient

Namespace Admin

  <Serializable()> _
  Public Class Role
    Inherits BusinessBase(Of Role)

#Region " Business Methods "

    Private Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer, Role)("Id")
    Private mId As Integer = IdProperty.DefaultValue
    Private mIdSet As Boolean
    Public Property Id() As Integer
      Get
        If Not mIdSet Then
          ' generate a default id value
          mIdSet = True
          Dim parent As Roles = CType(Me.Parent, Roles)
          Dim max As Integer = 0
          For Each item As Role In parent
            If item.Id > max Then
              max = item.Id
            End If
          Next
          mId = max + 1
        End If
        Return GetProperty(Of Integer)(IdProperty, mId)
      End Get
      Set(ByVal value As Integer)
        mIdSet = True
        SetProperty(IdProperty, mId, value)
      End Set
    End Property

    Private Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String, Role)("Name")
    Private mName As String = NameProperty.DefaultValue
    Public Property Name() As String
      Get
        Return GetProperty(NameProperty, mName)
      End Get
      Set(ByVal value As String)
        SetProperty(NameProperty, mName, value)
      End Set
    End Property

    Private mTimestamp(7) As Byte

#End Region

#Region " Validation Rules "

    Protected Overrides Sub AddBusinessRules()

      ValidationRules.AddRule(Of Role)(AddressOf NoDuplicates, "Id")
      ValidationRules.AddRule( _
        AddressOf Csla.Validation.CommonRules.StringRequired, "Name")

    End Sub

    Private Shared Function NoDuplicates(Of T As Role)(ByVal target As T, _
      ByVal e As Csla.Validation.RuleArgs) As Boolean

      Dim parent As Roles = CType(target.Parent, Roles)
      If parent IsNot Nothing Then
        For Each item As Role In parent
          If item.Id = target.mId AndAlso Not ReferenceEquals(item, target) Then
            e.Description = "Role Id must be unique"
            Return False
          End If
        Next
      End If
      Return True

    End Function

#End Region

#Region " Authorization Rules "

    Protected Overrides Sub AddAuthorizationRules()

      AuthorizationRules.AllowWrite("Id", "Administrator")
      AuthorizationRules.AllowWrite("Name", "Administrator")

    End Sub

#End Region

#Region " Factory Methods "

    Friend Shared Function NewRole() As Role

      Return New Role

    End Function

    Friend Shared Function GetRole(ByVal dr As Csla.Data.SafeDataReader) As Role

      Return New Role(dr)

    End Function

    Private Sub New()

      MarkAsChild()
      ValidationRules.CheckRules()

    End Sub

    Private Sub New(ByVal dr As Csla.Data.SafeDataReader)

      MarkAsChild()
      Fetch(dr)

    End Sub

#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dr As Csla.Data.SafeDataReader)

      With dr
        mId = .GetInt32("id")
        mIdSet = True
        mName = .GetString("name")
        .GetBytes("LastChanged", 0, mTimestamp, 0, 8)
      End With
      MarkOld()

    End Sub

    Friend Sub Insert(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandText = "addRole"
        DoInsertUpdate(cm)
      End Using

    End Sub

    Friend Sub Update(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandText = "updateRole"
        cm.Parameters.AddWithValue("@lastChanged", mTimestamp)
        DoInsertUpdate(cm)
      End Using

    End Sub

    Private Sub DoInsertUpdate(ByVal cm As SqlCommand)

      cm.CommandType = CommandType.StoredProcedure
      cm.Parameters.AddWithValue("@id", mId)
      cm.Parameters.AddWithValue("@name", mName)
      Dim param As New SqlParameter("@newLastChanged", SqlDbType.Timestamp)
      param.Direction = ParameterDirection.Output
      cm.Parameters.Add(param)

      cm.ExecuteNonQuery()

      mTimestamp = CType(cm.Parameters("@newLastChanged").Value, Byte())

      MarkOld()

    End Sub

    Friend Sub DeleteSelf(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      ' if we're new then don't update the database
      If Me.IsNew Then Exit Sub

      DeleteRole(cn, mId)
      MarkNew()

    End Sub

    Friend Shared Sub DeleteRole( _
      ByVal cn As SqlConnection, ByVal id As Integer)

      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "deleteRole"
        cm.Parameters.AddWithValue("@id", id)
        cm.ExecuteNonQuery()
      End Using

    End Sub

#End Region

  End Class

End Namespace
