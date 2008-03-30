<Serializable()> _
Public Class RoleList
  Inherits NameValueListBase(Of Integer, String)

#Region " Business Methods "

  Public Shared Function DefaultRole() As Integer

    Dim list As RoleList = GetList()
    If list.Count > 0 Then
      Return list.Items(0).Key

    Else
      Throw New NullReferenceException( _
        "No roles available; default role can not be returned")
    End If

  End Function

#End Region

#Region " Factory Methods "

  Private Shared mList As RoleList

  Public Shared Function GetList() As RoleList

    If mList Is Nothing Then
      mList = DataPortal.Fetch(Of RoleList)()
    End If
    Return mList

  End Function

  ''' <summary>
  ''' Clears the in-memory RoleList cache
  ''' so the list of roles is reloaded on
  ''' next request.
  ''' </summary>
  Public Shared Sub InvalidateCache()

    mList = Nothing

  End Sub

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Private Overloads Sub DataPortal_Fetch()

    Me.RaiseListChangedEvents = False
    Using ctx = ContextManager(Of ProjectTracker.DalLinq.PTrackerDataContext).GetManager(ProjectTracker.DalLinq.Database.PTracker)
      Dim data = From role In ctx.DataContext.Roles _
                 Select New NameValuePair(role.Id, role.Name)
      IsReadOnly = False
      Me.AddRange(data)
      IsReadOnly = True
    End Using
    Me.RaiseListChangedEvents = True

  End Sub

#End Region

End Class
