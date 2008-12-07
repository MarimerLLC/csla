Imports System

''' <summary>
''' Defines the base requirements for a criteria
''' object supported by the data portal.
''' </summary>
Public Interface ICriteria
  ''' <summary>
  ''' Type of the business object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  ReadOnly Property ObjectType() As Type
End Interface
