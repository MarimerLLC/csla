Public Interface IDataPortalResult

  ''' <summary>
  ''' Gets the resulting object.
  ''' </summary>
  ReadOnly Property [Object]() As Object

  ''' <summary>
  ''' Gets any resulting error information.
  ''' </summary>
  ReadOnly Property [Error]() As Exception

End Interface
