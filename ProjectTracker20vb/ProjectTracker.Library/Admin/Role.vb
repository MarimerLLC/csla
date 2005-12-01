Imports System.Data.SqlClient

Namespace Admin

  <Serializable()> _
  Public Class Role
    Inherits BusinessBase(Of Role)

#Region " Business Methods "

    Private mId As Integer
    Private mName As String = ""

    Public Property Id() As Integer
      Get
        CanReadProperty(True)
        Return mId
      End Get
      Set(ByVal value As Integer)
        CanWriteProperty(True)
        If Not mId.Equals(value) Then
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

#Region " Constructors "

    Private Sub New()

      MarkAsChild()

    End Sub

#End Region

#Region " Factory Methods "

    Friend Shared Function GetRole(ByVal dr As Csla.Data.SafeDataReader) As Role

      Return New Role(dr)

    End Function

#End Region

#Region " Data Access "

    Private Sub New(ByVal dr As Csla.Data.SafeDataReader)

      MarkAsChild()
      mId = dr.GetInt32("id")
      mName = dr.GetString("name")
      MarkOld()

    End Sub

    Friend Sub Insert(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "addRole"
        cm.Parameters.AddWithValue("@id", mId)
        cm.Parameters.AddWithValue("@name", mName)
        cm.ExecuteNonQuery()
      End Using

    End Sub

    Friend Sub Update(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = CommandType.StoredProcedure
        cm.CommandText = "updateRole"
        cm.Parameters.AddWithValue("@id", mId)
        cm.Parameters.AddWithValue("@name", mName)
        cm.ExecuteNonQuery()
      End Using

    End Sub

    Friend Sub DeleteSelf(ByVal cn As SqlConnection)

      ' if we're not dirty then don't update the database
      If Not Me.IsDirty Then Exit Sub

      ' if we're new then don't update the database
      If Me.IsNew Then Exit Sub

      DeleteRole(cn, mId)

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

#Region " Web Support "

    ''' <summary>
    ''' For use from Web Forms to atomically
    ''' insert/update a Role.
    ''' </summary>
    ''' <param name="forceUpdate">True to update data, False to insert data.</param>
    Friend Sub WebSave(ByVal forceUpdate As Boolean)

      If forceUpdate Then
        MarkOld()
      End If
      MarkDirty()
      DataPortal.Update(Me)

    End Sub

    Protected Overrides Sub DataPortal_Insert()

      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        Insert(cn)
        cn.Close()
      End Using

    End Sub

    Protected Overrides Sub DataPortal_Update()

      Using cn As New SqlConnection(DataBase.DbConn)
        cn.Open()
        Update(cn)
        cn.Close()
      End Using

    End Sub

#End Region

  End Class

End Namespace
