Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.Configuration

''' <summary>
''' Provides easy access to the batch queue functionality.
''' </summary>
''' <remarks>
''' Client code should create an instance of this class to
''' interact with a batch queue. A BatchQueue object can
''' be used to interact with either the default batch queue
''' server or with a specific batch queue server.
''' </remarks>
Public Class BatchQueue
  Private mQueueURL As String
  Private mServer As Server.BatchQueue

#Region " Constructors and Initialization "

  ''' <summary>
  ''' Creates an instance of the object that allows interaction
  ''' with the default batch queue server as specified in the
  ''' application configuration file.
  ''' </summary>
  Public Sub New()

    mQueueURL = ConfigurationSettings.AppSettings("DefaultBatchQueueServer")

  End Sub

  ''' <summary>
  ''' Creates an instance of the object that allows interaction
  ''' with a specific batch queue server as specified by
  ''' the URL passed as a parameter.
  ''' </summary>
  ''' <param name="QueueServerURL">A URL referencing the batch queue server.</param>
  Public Sub New(ByVal QueueServerURL As String)

    mQueueURL = QueueServerURL

  End Sub

  Private Function QueueServer() As Server.BatchQueue

    If mServer Is Nothing Then
      mServer = _
        CType(Activator.GetObject(GetType(Server.BatchQueue), _
        mQueueURL), _
        Server.BatchQueue)

    End If

    Return mServer

  End Function

#End Region

#Region " Submitting jobs "

  ''' <summary>
  ''' Submits an entry to the batch queue.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry)

    QueueServer.Submit(New Server.BatchEntry(Entry))

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue with extra state data.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="State">A reference to a serializable object containing state data.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, ByVal State As Object)

    QueueServer.Submit(New Server.BatchEntry(Entry, State))

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue with a specific priority.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="Priority">The priority of the entry.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, _
      ByVal Priority As Messaging.MessagePriority)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry)
    job.Info.Priority = Priority
    QueueServer.Submit(job)

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue with extra state data and
  ''' a specific priority.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="State">A reference to a serializable object containing state data.</param>
  ''' <param name="Priority">The priority of the entry.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, _
      ByVal State As Object, _
      ByVal Priority As Messaging.MessagePriority)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry, State)
    job.Info.Priority = Priority
    QueueServer.Submit(job)

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue to be held until a specific date/time.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="HoldUntil">The date/time until which the entry should be held.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, ByVal HoldUntil As Date)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry)
    job.Info.HoldUntil = HoldUntil
    QueueServer.Submit(job)

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue with extra state data
  ''' and to be held until a specific date/time.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="State">A reference to a serializable object containing state data.</param>
  ''' <param name="HoldUntil">The date/time until which the entry should be held.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, _
      ByVal State As Object, _
      ByVal HoldUntil As Date)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry, State)
    job.Info.HoldUntil = HoldUntil
    QueueServer.Submit(job)

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue to be held until 
  ''' a specific date/time and at a specific priority.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="HoldUntil">The date/time until which the entry should be held.</param>
  ''' <param name="Priority">The priority of the entry.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, _
      ByVal HoldUntil As Date, ByVal Priority As Messaging.MessagePriority)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry)
    job.Info.HoldUntil = HoldUntil
    job.Info.Priority = Priority
    QueueServer.Submit(job)

  End Sub

  ''' <summary>
  ''' Submits an entry to the batch queue specifying all details.
  ''' </summary>
  ''' <param name="Entry">A reference to your worker object.</param>
  ''' <param name="State">A reference to a serializable object containing state data.</param>
  ''' <param name="HoldUntil">The date/time until which the entry should be held.</param>
  ''' <param name="Priority">The priority of the entry.</param>
  Public Sub Submit(ByVal Entry As IBatchEntry, _
      ByVal State As Object, _
      ByVal HoldUntil As Date, _
      ByVal Priority As Messaging.MessagePriority)

    Dim job As Server.BatchEntry = New Server.BatchEntry(Entry, State)
    job.Info.HoldUntil = HoldUntil
    job.Info.Priority = Priority
    QueueServer.Submit(job)

  End Sub

#End Region

#Region " Public methods "

  ''' <summary>
  ''' Removes a holding or pending entry from the
  ''' batch queue.
  ''' </summary>
  ''' <param name="Entry">A reference to the entry to be removed.</param>
  Public Sub Remove(ByVal Entry As BatchEntryInfo)

    QueueServer.Remove(Entry)

  End Sub

  ''' <summary>
  ''' Returns the URL which identifies the batch
  ''' queue server to which this object is attached.
  ''' </summary>
  Public ReadOnly Property BatchQueueURL() As String
    Get
      Return mQueueURL
    End Get
  End Property

  ''' <summary>
  ''' Returns a collection of the batch entries currently
  ''' in the batch queue.
  ''' </summary>
  Public ReadOnly Property Entries() As BatchEntries
    Get
      Return QueueServer.GetEntries(GetPrincipal)
    End Get
  End Property

#End Region

#Region " System.Object overrides "

  Public Overrides Function ToString() As String

    Return mQueueURL

  End Function

  Public Overloads Function Equals(ByVal Queue As BatchQueue) As Boolean

    Return mQueueURL = Queue.BatchQueueURL

  End Function

  Public Overrides Function GetHashCode() As Integer

    Return mQueueURL.GetHashCode

  End Function

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

#End Region

End Class
