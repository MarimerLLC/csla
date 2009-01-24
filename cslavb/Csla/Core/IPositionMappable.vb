Imports System

Namespace Core

  Interface IPositionMappable(Of T)
    Function PositionOf(ByVal item As T) As Integer
  End Interface

End Namespace