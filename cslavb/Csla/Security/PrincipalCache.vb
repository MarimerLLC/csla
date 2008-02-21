Imports System.Configuration
Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Provides a cache for a limited number of
  ''' principal objects at the AppDomain level.
  ''' </summary>
  Public Class PrincipalCache

    Private Shared _cache As List(Of IPrincipal) = New List(Of IPrincipal)()

    Private Shared _maxCacheSize As Integer

    Private Sub New()
    End Sub

    Private Shared ReadOnly Property MaxCacheSize() As Integer
      Get
        If _maxCacheSize = 0 Then
          Dim tmp As String = System.Configuration.ConfigurationManager.AppSettings("CslaPrincipalCacheSize")
          If String.IsNullOrEmpty(tmp) Then
            _maxCacheSize = 10
          Else
            _maxCacheSize = Convert.ToInt32(tmp)
          End If
        End If
        Return _maxCacheSize
      End Get
    End Property

    ''' <summary>
    ''' Gets a principal from the cache based on
    ''' the identity name. If no match is found null
    ''' is returned.
    ''' </summary>
    ''' <param name="name">
    ''' The identity name associated with the principal.
    ''' </param>
    Public Shared Function GetPrincipal(ByVal name As String) As IPrincipal
      SyncLock _cache
        For Each item As IPrincipal In _cache
          If item.Identity.Name = name Then
            Return item
          End If
        Next item
        Return Nothing
      End SyncLock
    End Function

    ''' <summary>
    ''' Adds a principal to the cache.
    ''' </summary>
    ''' <param name="principal">
    ''' IPrincipal object to be added.
    ''' </param>
    Public Shared Sub AddPrincipal(ByVal principal As IPrincipal)
      SyncLock _cache
        If (Not _cache.Contains(principal)) Then
          _cache.Add(principal)
          If _cache.Count > MaxCacheSize Then
            _cache.RemoveAt(0)
          End If
        End If
      End SyncLock
    End Sub

    ''' <summary>
    ''' Clears the cache.
    ''' </summary>
    Public Shared Sub Clear()
      SyncLock _cache
        _cache.Clear()
      End SyncLock
    End Sub

  End Class

End Namespace
