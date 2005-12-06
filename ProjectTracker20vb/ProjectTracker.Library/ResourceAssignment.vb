Imports System.Data.SqlClient

<Serializable()> _
Public Class ResourceAssignment
  Inherits BusinessBase(Of ResourceAssignment)

#Region " Business Methods "

  Private mProjectId As Guid = Guid.Empty
  Private mProjectName As String = ""
  Private mAssigned As New SmartDate(Today)
  Private mRole As Integer
  Private mTimestamp(7) As Byte

  Public ReadOnly Property ProjectId() As Guid
    Get
      If CanReadProperty() Then
        Return mProjectId
      Else
        Throw New System.Security.SecurityException("Property get not allowed")
      End If
    End Get
  End Property

  Public ReadOnly Property ProjectName() As String
    Get
      If CanReadProperty() Then
        Return mProjectName
      Else
        Throw New System.Security.SecurityException("Property get not allowed")
      End If
    End Get
  End Property

  Public ReadOnly Property Assigned() As String
    Get
      CanReadProperty(True)
      Return mAssigned.Text
    End Get
  End Property

  Public Property Role() As Integer
    Get
      CanReadProperty(True)
      Return mRole
    End Get
    Set(ByVal value As Integer)
      CanWriteProperty(True)
      If Not mRole.Equals(value) Then
        mRole = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Public Function GetProject() As Project

    Return Project.GetProject(mProjectId)

  End Function

  Protected Overrides Function GetIdValue() As Object

    Return mProjectId

  End Function

#End Region

#Region " Constructors "

  Private Sub New()

    MarkAsChild()

  End Sub

#End Region

#Region " Factory Methods "

  Friend Shared Function NewResourceAssignment( _
    ByVal project As Project, ByVal role As Integer) As ResourceAssignment

    Return New ResourceAssignment(project, role)

  End Function

  Friend Shared Function NewResourceAssignment( _
    ByVal projectId As Guid, ByVal role As Integer) As ResourceAssignment

    Return New ResourceAssignment(Project.GetProject(projectId), role)

  End Function

  Friend Shared Function NewResourceAssignment( _
    ByVal projectId As Guid) As ResourceAssignment

    Return New ResourceAssignment(Project.GetProject(projectId), RoleList.DefaultRole)

  End Function

  Friend Shared Function GetResourceAssignment( _
    ByVal dr As SafeDataReader) As ResourceAssignment

    Return New ResourceAssignment(dr)

  End Function

#End Region

#Region " Data Access "

  Private Sub New(ByVal project As Project, ByVal role As Integer)

    MarkAsChild()
    mProjectId = project.Id
    mProjectName = project.Name
    mAssigned.Date = Now
    mRole = role

  End Sub

  Private Sub New(ByVal dr As SafeDataReader)

    MarkAsChild()
    With dr
      mProjectId = .GetGuid("ProjectId")
      mProjectName = .GetString("Name")
      mAssigned = .GetSmartDate("Assigned")
      mRole = .GetInt32("Role")
      .GetBytes("LastChanged", 0, mTimestamp, 0, 8)
    End With
    MarkOld()

  End Sub

  Friend Sub Insert(ByVal tr As SqlClient.SqlTransaction, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cm As New SqlClient.SqlCommand
      With cm
        .Connection = tr.Connection
        .Transaction = tr
        .CommandType = CommandType.StoredProcedure
        .CommandText = "addAssignment"
        LoadParameters(cm, resource)

        Using dr As SqlDataReader = cm.ExecuteReader
          dr.Read()
          dr.GetBytes(0, 0, mTimestamp, 0, 8)
        End Using

        MarkOld()
      End With
    End Using

  End Sub

  Friend Sub Update(ByVal tr As SqlClient.SqlTransaction, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    Using cm As New SqlClient.SqlCommand
      With cm
        .Connection = tr.Connection
        .Transaction = tr
        .CommandType = CommandType.StoredProcedure
        .CommandText = "updateAssignment"
        LoadParameters(cm, resource)
        cm.Parameters.AddWithValue("@lastChanged", mTimestamp)

        .ExecuteNonQuery()

        MarkOld()
      End With
    End Using

  End Sub

  Private Sub LoadParameters(ByVal cm As SqlCommand, ByVal resource As Resource)

    With cm
      .Parameters.AddWithValue("@projectId", mProjectId)
      .Parameters.AddWithValue("@resourceId", resource.Id)
      .Parameters.AddWithValue("@assigned", mAssigned.DBValue)
      .Parameters.AddWithValue("@role", mRole)
    End With

  End Sub

  Friend Sub DeleteSelf(ByVal tr As SqlClient.SqlTransaction, ByVal resource As Resource)

    ' if we're not dirty then don't update the database
    If Not Me.IsDirty Then Exit Sub

    ' if we're new then don't update the database
    If Me.IsNew Then Exit Sub

    Using cm As New SqlClient.SqlCommand
      With cm
        .Connection = tr.Connection
        .Transaction = tr
        .CommandType = CommandType.StoredProcedure
        .CommandText = "deleteAssignment"
        .Parameters.AddWithValue("@projectId", mProjectId)
        .Parameters.AddWithValue("@resourceId", resource.Id)

        .ExecuteNonQuery()

        MarkNew()
      End With
    End Using

  End Sub

#End Region

End Class
