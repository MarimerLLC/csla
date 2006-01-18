Imports System.Data.SqlClient

Namespace Admin

  <Serializable()> _
  Public Class Role
    Inherits BusinessBase(Of Role)

#Region " Business Methods "

    Private mId As Integer
    Private mIdSet As Boolean
    Private mName As String = ""
    Private mTimestamp(7) As Byte

    Public Property Id() As Integer
      Get
        CanReadProperty(True)
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
        Return mId
      End Get
      Set(ByVal value As Integer)
        CanWriteProperty(True)
        If Not mId.Equals(value) Then
          mIdSet = True
          mId = value
          PropertyHasChanged()
        End If
      End Set
    End Property

    Public Property Name() As String
      Get
        CanReadProperty(True)
        Return mName
      End Get
      Set(ByVal value As String)
        CanWriteProperty(True)
        If Not mName.Equals(value) Then
          mName = value
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

      ValidationRules.AddRule( _
        AddressOf Csla.Validation.CommonRules.StringRequired, "Name")
      ValidationRules.AddRule(AddressOf NoDuplicates, "Id")

    End Sub

    Private Function NoDuplicates(ByVal target As Object, _
      ByVal e As Csla.Validation.RuleArgs) As Boolean

      Dim parent As Roles = CType(Me.Parent, Roles)
      For Each item As Role In parent
        If item.Id = mId AndAlso Not ReferenceEquals(item, Me) Then
          e.Description = "Role Id must be unique"
          Return False
        End If
      Next
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

    End Sub

#End Region

#Region " Data Access "

    Private Sub New(ByVal dr As Csla.Data.SafeDataReader)

      MarkAsChild()
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
        cm.Parameters.AddWithValue("@id", Id)
        cm.ExecuteNonQuery()
      End Using

    End Sub

#End Region

  End Class

End Namespace
