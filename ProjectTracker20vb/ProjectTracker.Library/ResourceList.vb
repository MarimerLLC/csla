<Serializable()> _
Public Class ResourceList
  Inherits ReadOnlyListBase(Of ResourceList, ResourceInfo)

#Region " Factory Methods "

  Public Shared Function EmptyList() As ResourceList

    Return New ResourceList

  End Function

  Public Shared Function GetResourceList() As ResourceList

    Return DataPortal.Fetch(Of ResourceList)(New Criteria)

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria
    ' no criteria - we retrieve all resources
  End Class


  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    RaiseListChangedEvents = False
    Using cn As New SqlConnection(Database.PTrackerConnection)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        With cm
          .CommandType = CommandType.StoredProcedure
          .CommandText = "getResources"

          Using dr As New SafeDataReader(.ExecuteReader)
            IsReadOnly = False
            While dr.Read()
              Dim info As New ResourceInfo(dr)
              Me.Add(info)
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
