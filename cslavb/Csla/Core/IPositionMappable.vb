Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  Friend Interface IPositionMappable(Of T)
    Function PositionOf(ByVal item As T) As Integer
  End Interface
End Namespace