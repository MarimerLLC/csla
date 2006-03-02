<Serializable()> _
Public Class ProjectList
  Inherits ReadOnlyListBase(Of ProjectList, ProjectInfo)

#Region " Factory Methods "

  Public Shared Function GetProjectList() As ProjectList

    Return DataPortal.Fetch(Of ProjectList)(New Criteria)

  End Function

  Public Shared Function GetProjectList(ByVal name As String) As ProjectList

    Return DataPortal.Fetch(Of ProjectList)(New FilteredCriteria(name))

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
Private Class Criteria
    ' no criteria - retrieve all projects
  End Class

  <Serializable()> _
  Private Class FilteredCriteria
    Private mName As String
    Public ReadOnly Property Name() As String
      Get
        Return mName
      End Get
    End Property

    Public Sub New(ByVal name As String)
      mName = name
    End Sub
  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    ' fetch with no filter
    Fetch("")

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilteredCriteria)

    Fetch(criteria.Name)

  End Sub

  Private Sub Fetch(ByVal nameFilter As String)

    RaiseListChangedEvents = False
    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getProjects"
          Using dr As New SafeDataReader(.ExecuteReader)
            IsReadOnly = False
            While dr.Read()
              Dim info As New ProjectInfo(dr.GetGuid(0), dr.GetString(1))
              ' apply filter if necessary
              If Len(nameFilter) = 0 OrElse info.Name.IndexOf(nameFilter) = 0 Then
                Me.Add(info)
              End If
            End While
            IsReadOnly = True
          End Using
        End With
      End Using
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
