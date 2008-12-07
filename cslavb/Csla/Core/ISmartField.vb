Imports System

Namespace Core

  ''' <summary>
  ''' Defines members required for smart
  ''' data types that understand the concept
  ''' of being empty, and representing an
  ''' empty value as an empty string.
  ''' </summary>
  Public Interface ISmartField
    ''' <summary>
    ''' Sets or gets the text representation
    ''' of the value.
    ''' </summary>
    ''' <remarks>
    ''' An empty string indicates an empty value.
    ''' </remarks>
    Property Text() As String
    ''' <summary>
    ''' Gets a value indicating whether the
    ''' field's value is empty.
    ''' </summary>
    ReadOnly Property IsEmpty() As Boolean
  End Interface

End Namespace
