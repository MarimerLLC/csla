<Serializable()> _
Public Class ResourceList
  Inherits ReadOnlyListBase(Of ResourceList, ResourceInfo)

#Region " Factory Methods "

  Public Shared Function EmptyList() As ResourceList

    Return New ResourceList

  End Function

  Public Shared Function GetResourceList() As ResourceList

    Return DataPortal.Fetch(Of ResourceList)()

  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    RaiseListChangedEvents = False
    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(Database.PTrackerConnection)
      Dim data = From r In ctx.DataContext.Resources Select r
      IsReadOnly = False
      For Each resource In data
        Me.Add(New ResourceInfo(resource))
      Next
      IsReadOnly = True
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
