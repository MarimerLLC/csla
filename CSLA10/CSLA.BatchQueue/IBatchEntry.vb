''' <summary>
''' Defines the interface that must be implemented by
''' all worker classes.
''' </summary>
''' <remarks>
''' To create a worker that can be executed within the
''' batch queue, implement this interface. The interface
''' will be invoked by the batch queue processor on the
''' server.
''' </remarks>
Public Interface IBatchEntry
  ''' <summary>
  ''' This method should contain your worker code that
  ''' is to be run in the batch queue.
  ''' </summary>
  ''' <param name="State">An optional object containing extra state data from the client.</param>
  Sub Execute(ByVal State As Object)
End Interface
