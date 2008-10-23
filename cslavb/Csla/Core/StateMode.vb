Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  ''' <summary> 
  ''' Indicates the reason the MobileFormatter 
  ''' functionality has been invoked. 
  ''' </summary> 
  Public Enum StateMode
    ''' <summary> 
    ''' The object is being serialized for 
    ''' a clone or data portal operation. 
    ''' </summary> 
    Serialization
    ''' <summary> 
    ''' The object is being serialized for 
    ''' an n-level undo operation. 
    ''' </summary> 
    Undo
  End Enum
End Namespace