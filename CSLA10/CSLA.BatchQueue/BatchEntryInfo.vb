Imports System.Environment

''' <summary>
''' Values indicating the status of a batch queue entry.
''' </summary>
Public Enum BatchEntryStatus
  Pending
  Holding
  Active
End Enum

''' <summary>
''' Contains information about a batch queue entry.
''' </summary>
''' <remarks>
''' This object contains information about batch entry including
''' when it was submitted, the user that submitted the job and
''' which machine the user was using at the time. It also contains
''' the job's priority, status and the optional date/time until
''' which the job should be held until it is run.
''' </remarks>
<Serializable()> _
Public Class BatchEntryInfo

  Private mID As Guid = Guid.NewGuid
  Private mSubmitted As Date = Now
  Private mUser As String = System.Environment.UserName
  Private mMachine As String = System.Environment.MachineName
  Private mPriority As Messaging.MessagePriority = Messaging.MessagePriority.Normal
  Private mMsgID As String
  Private mHoldUntil As Date = Date.MinValue
  Private mStatus As BatchEntryStatus = BatchEntryStatus.Pending

  ''' <summary>
  ''' Returns the unique ID value for this batch entry.
  ''' </summary>
  Public ReadOnly Property ID() As Guid
    Get
      Return mID
    End Get
  End Property

  ''' <summary>
  ''' Returns the date and time that the batch entry
  ''' was submitted.
  ''' </summary>
  Public ReadOnly Property Submitted() As Date
    Get
      Return mSubmitted
    End Get
  End Property

  ''' <summary>
  ''' Returns the Windows user id of the user that
  ''' was logged into the workstation when the job
  ''' was submitted.
  ''' </summary>
  Public ReadOnly Property User() As String
    Get
      Return mUser
    End Get
  End Property

  ''' <summary>
  ''' Returns the name of the workstation from
  ''' which this job was submitted.
  ''' </summary>
  Public ReadOnly Property Machine() As String
    Get
      Return mMachine
    End Get
  End Property

  ''' <summary>
  ''' Returns the priority of this batch entry.
  ''' </summary>
  ''' <remarks>
  ''' The priority values flow from System.Messaging and
  ''' the priority is used by MSMQ to order the entries
  ''' in the queue.
  ''' </remarks>
  Public Property Priority() As Messaging.MessagePriority
    Get
      Return mPriority
    End Get
    Set(ByVal Value As Messaging.MessagePriority)
      mPriority = Value
    End Set
  End Property

  ''' <summary>
  ''' Returns the MSMQ message ID of the batch entry.
  ''' </summary>
  ''' <remarks>
  ''' This value is only valid after the batch entry
  ''' has been submitted to the queue.
  ''' </remarks>
  Public ReadOnly Property MessageID() As String
    Get
      Return mMsgID
    End Get
  End Property

  Friend Sub SetMessageID(ByVal ID As String)
    mMsgID = ID
  End Sub

  ''' <summary>
  ''' Returns the date and time until which the
  ''' batch entry will be held before it can be run.
  ''' </summary>
  ''' <remarks>
  ''' This value is optional. If it was provided, the batch
  ''' entry will be held until this date/time. At this date/time,
  ''' the entry will switch from Holding status to Pending
  ''' status and will be queued based on its priority along
  ''' with all other Pending entries.
  ''' </remarks>
  Public Property HoldUntil() As Date
    Get
      Return mHoldUntil
    End Get
    Set(ByVal Value As Date)
      mHoldUntil = Value
    End Set
  End Property

  ''' <summary>
  ''' Returns the status of the batch entry.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If the job is Holding, it means that the job
  ''' won't run until the data/time specified by
  ''' <see cref="P:CSLA.BatchQueue.BatchEntryInfo.HoldUntil" />.
  ''' </para><para>
  ''' If the job is Pending, it means that the job
  ''' will run as soon as possible, but that the queue
  ''' is busy. Pending entries are run in priority order based
  ''' on <see cref="P:CSLA.BatchQueue.BatchEntryInfo.Priority" />.
  ''' </para><para>
  ''' If the job is Active, it means that the job is
  ''' currently being executed on the server.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property Status() As BatchEntryStatus
    Get
      If mHoldUntil > Now AndAlso mStatus = BatchEntryStatus.Pending Then
        Return BatchEntryStatus.Holding

      Else
        Return mStatus
      End If
    End Get
  End Property

  Friend Sub SetStatus(ByVal Status As BatchEntryStatus)

    mStatus = Status

  End Sub

#Region " System.Object overrides "

  Public Overrides Function ToString() As String

    Return mUser & "@" & mMachine & ":" & mID.ToString

  End Function

  Public Overloads Function Equals(ByVal Info As BatchEntryInfo) As Boolean

    Return mID.Equals(Info.ID)

  End Function

  Public Overrides Function GetHashCode() As Integer

    Return mID.GetHashCode

  End Function

#End Region

End Class
