Imports System.Security.Principal
Imports System.Configuration
Imports CSLA.Security

''' <summary>
''' 
''' </summary>
Namespace Server

  ''' <summary>
  ''' This is the entry point for all queue requests from
  ''' the client via Remoting.
  ''' </summary>
  Public Class BatchQueue
    Inherits MarshalByRefObject

#Region " Public methods "

    ''' <summary>
    ''' Submits a batch entry to the queue.
    ''' </summary>
    ''' <param name="Entry">A reference to the batch entry.</param>
    Public Sub Submit(ByVal Entry As BatchEntry)

      BatchQueueService.Enqueue(Entry)

    End Sub

    ''' <summary>
    ''' Removes a holding or pending entry from the queue.
    ''' </summary>
    ''' <param name="Entry">A reference to the info object for the batch entry.</param>
    Public Sub Remove(ByVal Entry As BatchEntryInfo)

      BatchQueueService.Dequeue(Entry)

    End Sub

    ''' <summary>
    ''' Gets a list of the entries currently in the
    ''' batch queue.
    ''' </summary>
    ''' <param name="Principal">The requesting user's security credentials.</param>
    Public Function GetEntries(ByVal Principal As IPrincipal) As BatchEntries

      SetPrincipal(Principal)

      Dim entries As New BatchEntries()
      BatchQueueService.LoadEntries(entries)

      Return entries

    End Function

#End Region

#Region " Security "

    Private Function AUTHENTICATION() As String

      Return ConfigurationSettings.AppSettings("Authentication")

    End Function

    Private Sub SetPrincipal(ByVal Principal As Object)
      If AUTHENTICATION() = "Windows" Then
        ' when using integrated security, Principal must be Nothing
        ' and we need to set our policy to use the Windows principal
        If Principal Is Nothing Then
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal)
          Exit Sub

        Else
          Throw New System.Security.SecurityException("No principal object should be passed to DataPortal when using Windows integrated security")
        End If
      End If

      ' we expect Principal to be of type BusinessPrincipal, but
      ' we can't enforce that since it causes a circular reference
      ' with the business library so instead we must use type Object
      ' for the parameter, so here we do a check on the type of the
      ' parameter
      If Principal.ToString = "CSLA.Security.BusinessPrincipal" Then
        ' see if our current principal is
        ' different from the caller's principal
        If Not ReferenceEquals(Principal, System.Threading.Thread.CurrentPrincipal) Then
          ' the caller had a different principal, so change ours to
          ' match the caller's so all our objects use the caller's
          ' security
          System.Threading.Thread.CurrentPrincipal = CType(Principal, IPrincipal)
        End If

      Else
        Throw New System.Security.SecurityException("Principal must be of type BusinessPrincipal, not " & Principal.ToString)
      End If

    End Sub

#End Region

  End Class

End Namespace
