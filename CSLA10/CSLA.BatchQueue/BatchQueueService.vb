Imports System.Configuration
Imports System.Messaging
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 
''' </summary>
Namespace Server

  ''' <summary>
  ''' Implements the batch queue service.
  ''' </summary>
  ''' <remarks>
  ''' This class implements the server-side batch queue functionality.
  ''' It must be hosted within some process, typically a Windows Service.
  ''' It may also be hosted within a console application, which is
  ''' useful for testing and debugging.
  ''' </remarks>
  Public Class BatchQueueService

    Private Shared mChannel As TcpServerChannel
    Private Shared mQueue As MessageQueue
    Private Shared mMonitor As Threading.Thread
    Private Shared WithEvents mTimer As New System.Timers.Timer
    Private Shared mRunning As Boolean
    Private Shared mActiveEntries As Hashtable = Hashtable.Synchronized(New Hashtable)

    Private Shared mSync As New Threading.AutoResetEvent(False)
    Private Shared mWaitToEnd As New Threading.ManualResetEvent(False)

    ''' <summary>
    ''' Returns the name of the batch queue server.
    ''' </summary>
    Public Shared ReadOnly Property Name() As String
      Get
        Return LISTENER_NAME()
      End Get
    End Property

#Region " Start/Stop "

    ''' <summary>
    ''' Starts the batch queue.
    ''' </summary>
    ''' <remarks>
    ''' Call this as your Windows Service starts up to
    ''' start the batch queue itself. This will cause
    ''' the queue to start listening for user requests
    ''' via remoting and to the MSMQ for queued jobs.
    ''' </remarks>
    Public Shared Sub Start()

      mTimer.AutoReset = False

      ' open and/or create queue
      If MessageQueue.Exists(QUEUE_NAME) Then
        mQueue = New MessageQueue(QUEUE_NAME)

      Else
        mQueue = MessageQueue.Create(QUEUE_NAME)
      End If
      mQueue.MessageReadPropertyFilter.Extension = True

      ' start reading from queue
      mRunning = True
      mMonitor = New Threading.Thread(AddressOf MonitorQueue)
      mMonitor.Name = "MonitorQueue"
      mMonitor.Start()

      ' start remoting for Server.BatchQueue
      If mChannel Is Nothing Then
        ' set application name (virtual root name)
        RemotingConfiguration.ApplicationName = LISTENER_NAME()

        ' set up channel
        Dim properties As New Hashtable()
        properties("name") = "TcpBinary"
        properties("priority") = "2"
        properties("port") = CStr(PORT())
        Dim svFormatter As New BinaryServerFormatterSinkProvider()

        'TODO: uncomment the following line for .NET 1.1
        'svFormatter.TypeFilterLevel = Runtime.Serialization.Formatters.TypeFilterLevel.Full

        mChannel = New TcpServerChannel(properties, svFormatter)
        Channels.ChannelServices.RegisterChannel(mChannel)

        ' register our class
        RemotingConfiguration.RegisterWellKnownServiceType( _
          GetType(Server.BatchQueue), _
          "BatchQueue.rem", _
          WellKnownObjectMode.SingleCall)

      Else
        mChannel.StartListening(Nothing)
      End If

      Dim sb As New Text.StringBuilder()

      sb.Append("Batch queue processor started")
      sb.Append(vbCrLf)
      sb.AppendFormat("Name: {0}", Name)
      sb.Append(vbCrLf)
      sb.AppendFormat("Port: {0}", PORT)
      sb.Append(vbCrLf)
      sb.AppendFormat("Queue: {0}", QUEUE_NAME)
      sb.Append(vbCrLf)
      sb.AppendFormat("Max jobs: {0}", MAX_ENTRIES)
      sb.Append(vbCrLf)
      System.Diagnostics.EventLog.WriteEntry( _
        Name, sb.ToString, EventLogEntryType.Information)

    End Sub

    ''' <summary>
    ''' Stops the batch queue.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' Call this as your Windows Service is stopping. It
    ''' stops the batch queue, causing it to stop listening
    ''' for user requests via remoting and to stop looking for
    ''' jobs in the MSMQ queue.
    ''' </para><para>
    ''' NOTE that this method will not return until any
    ''' currently active (executing) batch jobs have completed.
    ''' </para>
    ''' </remarks>
    Public Shared Sub [Stop]()

      ' stop remoting for Server.BatchQueue
      mChannel.StopListening(Nothing)

      ' signal to stop working 
      mRunning = False
      mSync.Set()
      mMonitor.Join()
      ' close the queue
      mQueue.Close()

      If mActiveEntries.Count > 0 Then
        ' wait for work to end
        mWaitToEnd.WaitOne()
      End If

    End Sub

#End Region

#Region " Process messages "

    ' this will be running on a background thread
    Private Shared Sub MonitorQueue()

      While mRunning
        ScanQueue()
        mSync.WaitOne()
      End While

    End Sub

    ' this runs on a threadpool thread
    Private Shared Sub mTimer_Elapsed(ByVal sender As Object, _
        ByVal e As System.Timers.ElapsedEventArgs) Handles mTimer.Elapsed

      mTimer.Stop()
      mSync.Set()

    End Sub

    ' this is called by MonitorQueue
    Private Shared Sub ScanQueue()

      Dim msg As Message
      Dim holdUntil As Date
      Dim nextWake As Date = Date.MaxValue

      Dim en As MessageEnumerator = mQueue.GetMessageEnumerator
      While en.MoveNext()
        msg = en.Current
        holdUntil = CDate(Text.Encoding.ASCII.GetString(msg.Extension))
        If holdUntil <= Now Then
          If mActiveEntries.Count < MAX_ENTRIES() Then
            ProcessEntry(mQueue.ReceiveById(msg.Id))

          Else
            ' the queue is busy, go to sleep
            Exit Sub
          End If

        ElseIf holdUntil < nextWake Then
          ' find the minimum holduntil value
          nextWake = holdUntil
        End If
      End While

      If nextWake < Date.MaxValue AndAlso nextWake > Now Then
        ' we have at least one entry holding, so set the
        ' timer to wake us when it should be run
        mTimer.Interval = nextWake.Subtract(Now).TotalMilliseconds
        mTimer.Start()
      End If

    End Sub

    Private Shared Sub ProcessEntry(ByVal msg As Message)

      ' get entry from queue
      Dim entry As BatchEntry
      Dim formatter As New BinaryFormatter()
      entry = CType(formatter.Deserialize(msg.BodyStream), BatchEntry)

      ' make active
      entry.Info.SetStatus(BatchEntryStatus.Active)
      mActiveEntries.Add(entry.Info.ID, entry.Info)

      ' start processing entry on background thread
      Threading.ThreadPool.QueueUserWorkItem(AddressOf entry.Execute)

    End Sub

    ' called by BatchEntry when it is done processing so
    ' we know that it is complete and we can start another
    ' job if needed
    Friend Shared Sub Deactivate(ByVal entry As BatchEntry)

      mActiveEntries.Remove(entry.Info.ID)
      mSync.Set()
      If Not mRunning AndAlso mActiveEntries.Count = 0 Then
        ' indicate that there are no active workers
        mWaitToEnd.Set()
      End If

    End Sub

#End Region

#Region " Enqueue/Dequeue/LoadEntries "

    Friend Shared Sub Enqueue(ByVal Entry As BatchEntry)

      Dim msg As New Message()
      Dim f As New BinaryFormatter()

      With msg
        .Label = Entry.ToString
        .Priority = Entry.Info.Priority
        .Extension = Text.Encoding.ASCII.GetBytes(CStr(Entry.Info.HoldUntil))
        Entry.Info.SetMessageID(.Id)
        f.Serialize(.BodyStream, Entry)
      End With

      mQueue.Send(msg)

      mSync.Set()

    End Sub

    Friend Shared Sub Dequeue(ByVal Entry As BatchEntryInfo)

      Dim label As String
      Dim msg As Message
      Dim msgID As String

      label = Entry.ToString
      Dim en As MessageEnumerator = mQueue.GetMessageEnumerator
      mQueue.MessageReadPropertyFilter.Label = True

      While en.MoveNext()
        If en.Current.Label = label Then
          ' we found a match
          msgID = en.Current.Id
          Exit While
        End If
      End While

      If Len(msgID) > 0 Then
        mQueue.ReceiveById(msgID)
      End If

    End Sub

    Friend Shared Sub LoadEntries(ByVal List As BatchEntries)

      ' load our list of BatchEntry objects
      Dim msgs() As Message
      Dim msg As Message
      Dim formatter As New BinaryFormatter()
      Dim de As DictionaryEntry
      Dim entry As Server.BatchEntry

      ' get all active entries
      SyncLock mActiveEntries.SyncRoot
        For Each de In Server.BatchQueueService.mActiveEntries
          List.Add(CType(de.Value, BatchEntryInfo))
        Next
      End SyncLock

      ' get all queued entries
      msgs = mQueue.GetAllMessages
      For Each msg In msgs
        entry = CType(formatter.Deserialize(msg.BodyStream), Server.BatchEntry)
        entry.Info.SetMessageID(msg.Id)
        List.Add(entry.Info)
      Next

    End Sub

#End Region

#Region " Utility functions "

    Private Shared Function QUEUE_NAME() As String
      Return ".\private$\" & ConfigurationSettings.AppSettings("QueueName")
    End Function

    Private Shared Function LISTENER_NAME() As String
      Return ConfigurationSettings.AppSettings("ListenerName")
    End Function

    Private Shared Function PORT() As Integer
      Return CInt(ConfigurationSettings.AppSettings("ListenerPort"))
    End Function

    Private Shared Function MAX_ENTRIES() As Integer
      Return CInt(ConfigurationSettings.AppSettings("MaxActiveEntries"))
    End Function

#End Region

  End Class

End Namespace
