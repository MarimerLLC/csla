''' <summary>
''' A helper object used to execute a specified class
''' from a specified DLL on the server.
''' </summary>
''' <remarks>
''' <para>
''' A worker object can be provided directly by the client
''' workstation. In that case, the worker object is passed
''' by value to the server where it is executed. The drawback
''' to such an approach is that the worker assembly must be
''' installed on both client and server.
''' </para><para>
''' BatchJobRequest is a worker object that specifies the
''' type and assembly name of a class on the server. When
''' this job is run, it dynamically creates an instance of
''' the specified class and executes it on the server. This
''' means that the actual worker assembly needs to be installed
''' only on the server, not on the client.
''' </para>
''' </remarks>
<Serializable()> _
Public Class BatchJobRequest

  Implements IBatchEntry

  Private mAssembly As String = ""
  Private mType As String = ""

  ''' <summary>
  ''' Creates a new object, specifying the type and assembly
  ''' of the actual worker object.
  ''' </summary>
  ''' <param name="Type">The class name of the actual worker object.</param>
  ''' <param name="Assembly">The name of the assembly containing the actual worker class.</param>
  Public Sub New(ByVal Type As String, ByVal [Assembly] As String)
    mAssembly = [Assembly]
    mType = Type
  End Sub

  ''' <summary>
  ''' The class name of the worker object.
  ''' </summary>
  Public Property Type() As String
    Get
      Return mType
    End Get
    Set(ByVal Value As String)
      mType = Value
    End Set
  End Property

  ''' <summary>
  ''' The name of the assembly containing the actual worker class.
  ''' </summary>
  Public Property [Assembly]() As String
    Get
      Return mAssembly
    End Get
    Set(ByVal Value As String)
      mAssembly = Value
    End Set
  End Property


  ''' <summary>
  ''' Executes the batch job on the server.
  ''' </summary>
  ''' <remarks>
  ''' This method runs on the server - it is called
  ''' by <see cref="T:CSLA.BatchQueue.Server.BatchEntry" />,
  ''' which is called by 
  ''' <see cref="T:CSLA.BatchQueue.Server.BatchQueueService" />.
  ''' </remarks>
  ''' <param name="State"></param>
  Public Sub Execute(ByVal State As Object) _
      Implements IBatchEntry.Execute

    ' create an instance of the specified object
    Dim job As IBatchEntry
    job = CType(AppDomain.CurrentDomain.CreateInstanceAndUnwrap(mAssembly, mType), IBatchEntry)

    ' execute the job
    job.Execute(State)

  End Sub

#Region " System.Object overrides "

  Public Overrides Function ToString() As String

    Return "BatchJobRequest: " & mType & "," & mAssembly

  End Function

#End Region

End Class
