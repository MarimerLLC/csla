Imports System.Security.Principal
Imports System.Configuration

''' <summary>
''' 
''' </summary>
Namespace Server

  ''' <summary>
  ''' A batch queue entry.
  ''' </summary>
  ''' <remarks>
  ''' Each batch queue entry consists of basic information about
  ''' the entry, a Principal object (if you are using CSLA .NET
  ''' security), the actual worker object containing the code
  ''' to be run and an optional state object containing arbitrary
  ''' state data.
  ''' </remarks>
  <Serializable()> _
  Public NotInheritable Class BatchEntry
    Private mInfo As New BatchEntryInfo
    Private mPrincipal As IPrincipal = GetPrincipal()
    Private mWorker As IBatchEntry
    Private mState As Object

    ''' <summary>
    ''' Returns a reference to the object containing information
    ''' about this batch entry.
    ''' </summary>
    Public ReadOnly Property Info() As BatchEntryInfo
      Get
        Return mInfo
      End Get
    End Property

    ''' <summary>
    ''' Returns a reference to the 
    ''' <see cref="T:CSLA.Security.BusinessPrincipal" />
    ''' object for the user that submitted this entry.
    ''' </summary>
    Public ReadOnly Property Principal() As IPrincipal
      Get
        Return mPrincipal
      End Get
    End Property

    ''' <summary>
    ''' Returns a reference to the worker object that
    ''' contains the code which is to be executed as
    ''' a batch process.
    ''' </summary>
    Public Property Entry() As IBatchEntry
      Get
        Return mWorker
      End Get
      Set(ByVal Value As IBatchEntry)
        mWorker = Value
      End Set
    End Property

    ''' <summary>
    ''' Returns a reference to the optional state object.
    ''' </summary>
    ''' <returns></returns>
    Public Property State() As Object
      Get
        Return mState
      End Get
      Set(ByVal Value As Object)
        mState = Value
      End Set
    End Property

#Region " Batch execution "

    ' this will run in a background thread in the
    ' thread pool
    Friend Sub Execute(ByVal State As Object)


      Dim oldPrincipal As IPrincipal

      Try
        ' set this thread's principal to our user
        oldPrincipal = Threading.Thread.CurrentPrincipal
        SetPrincipal(mPrincipal)

        Try
          ' now run the user's code
          mWorker.Execute(mState)

          Dim sb As New Text.StringBuilder()
          With sb
            .Append("Batch job completed")
            .Append(vbCrLf)
            .AppendFormat("Batch job: {0}", Me.ToString)
            .Append(vbCrLf)
            .AppendFormat("Job object: {0}", CType(mWorker, Object).ToString)
            .Append(vbCrLf)
          End With

          System.Diagnostics.EventLog.WriteEntry( _
            BatchQueueService.Name, sb.ToString, EventLogEntryType.Information)

        Catch ex As Exception
          Dim sb As New Text.StringBuilder()
          With sb
            .Append("Batch job failed due to execution error")
            .Append(vbCrLf)
            .AppendFormat("Batch job: {0}", Me.ToString)
            .Append(vbCrLf)
            .AppendFormat("Job object: {0}", CType(mWorker, Object).ToString)
            .Append(vbCrLf)
            .Append(ex.ToString)
          End With

          System.Diagnostics.EventLog.WriteEntry( _
            BatchQueueService.Name, sb.ToString, EventLogEntryType.Warning)
        End Try

      Finally
        Server.BatchQueueService.Deactivate(Me)
        ' reset the thread's principal object
        Threading.Thread.CurrentPrincipal = oldPrincipal
      End Try

    End Sub

#End Region

#Region " System.Object overrides "

    Public Overrides Function ToString() As String

      Return mInfo.ToString

    End Function

    Public Overloads Function Equals(ByVal Entry As BatchEntry) As Boolean

      Return mInfo.Equals(Entry.Info)

    End Function

    Public Overrides Function GetHashCode() As Integer

      Return mInfo.GetHashCode

    End Function

#End Region

#Region " Constructors "

    Friend Sub New(ByVal Entry As IBatchEntry)

      mWorker = Entry

    End Sub

    Friend Sub New(ByVal Entry As IBatchEntry, ByVal State As Object)

      mWorker = Entry
      mState = State

    End Sub

#End Region

#Region " Security "

    Private Function AUTHENTICATION() As String
      Return ConfigurationSettings.AppSettings("Authentication")
    End Function

    Private Function GetPrincipal() As System.Security.Principal.IPrincipal
      If AUTHENTICATION() = "Windows" Then
        ' Windows integrated security 
        Return Nothing

      Else
        ' we assume using the CSLA framework security
        Return System.Threading.Thread.CurrentPrincipal
      End If
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
      If CType(Principal, IPrincipal).Identity.AuthenticationType = "CSLA" Then
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
