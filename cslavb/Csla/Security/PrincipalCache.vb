Imports System.Configuration
Imports System.Security.Principal

Namespace Security

  ''' <summary>
  ''' Provides a cache for a limited number of
  ''' principal objects at the AppDomain level.
  ''' </summary>
  Public Class PrincipalCache

    Private Shared mCache As List(Of IPrincipal) = New List(Of IPrincipal)()

    Private Shared mMaxCacheSize As Integer

    Private Sub New()
    End Sub

    Private Shared ReadOnly Property MaxCacheSize() As Integer
      Get
        If mMaxCacheSize = 0 Then
          Dim tmp As String = System.Configuration.ConfigurationManager.AppSettings("CslaPrincipalCacheSize")
          If String.IsNullOrEmpty(tmp) Then
            mMaxCacheSize = 10
          Else
            mMaxCacheSize = Convert.ToInt32(tmp)
          End If
        End If
        Return mMaxCacheSize
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
      SyncLock mCache
        For Each item As IPrincipal In mCache
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
      SyncLock mCache
        If (Not mCache.Contains(principal)) Then
          mCache.Add(principal)
          If mCache.Count > MaxCacheSize Then
            mCache.RemoveAt(0)
          End If
        End If
      End SyncLock
    End Sub

    ''' <summary>
    ''' Clears the cache.
    ''' </summary>
    Public Shared Sub Clear()
      SyncLock mCache
        mCache.Clear()
      End SyncLock
    End Sub

  End Class

End Namespace
