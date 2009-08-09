Imports System
Imports System.Collections.Generic

Namespace Serialization.Mobile

  ''' <summary>
  ''' Implements an equality comparer for <see cref="IMobileObject" /> that compares
  ''' the objects only on the basis is the reference value.
  ''' </summary>
  Public NotInheritable Class ReferenceComparer(Of T)
    Implements IEqualityComparer(Of T)

#Region " IEqualityComparer<T> Members "

    ''' <summary>
    ''' Determines if the two objects are reference-equal.
    ''' </summary>
    Public Shadows Function Equals(ByVal x As T, ByVal y As T) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of T).Equals
      Return Object.ReferenceEquals(x, y)
    End Function

    Public Shadows Function GetHashCode(ByVal obj As T) As Integer Implements System.Collections.Generic.IEqualityComparer(Of T).GetHashCode
      Return obj.GetHashCode()
    End Function

#End Region

  End Class
End Namespace

