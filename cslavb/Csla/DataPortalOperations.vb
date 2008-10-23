Imports System.ComponentModel

''' <summary>
''' List of data portal operations.
''' </summary>
<EditorBrowsable(EditorBrowsableState.Advanced)> _
Public Enum DataPortalOperations
  ''' <summary>
  ''' Create operation.
  ''' </summary>
  Create
  ''' <summary>
  ''' Fetch operation.
  ''' </summary>
  Fetch
  ''' <summary>
  ''' Update operation (includes
  ''' insert, update and delete self).
  ''' </summary>
  Update
  ''' <summary>
  ''' Delete operation.
  ''' </summary>
  Delete
  ''' <summary>
  ''' Execute operation.
  ''' </summary>
  Execute
End Enum
