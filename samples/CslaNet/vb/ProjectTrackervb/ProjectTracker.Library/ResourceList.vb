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
    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim data = From r In ctx.DataContext.Resources _
                 Select New ResourceInfo(r.Id, r.LastName, r.FirstName)
      IsReadOnly = False
      Me.AddRange(data)
      IsReadOnly = True
    End Using
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
