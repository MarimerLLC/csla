Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Linq
  Friend Interface IBalancedSearch(Of T)
    Function ItemsLessThan(ByVal pivot As Object) As IEnumerable(Of T)
    Function ItemsGreaterThan(ByVal pivot As Object) As IEnumerable(Of T)
    Function ItemsLessThanOrEqualTo(ByVal pivot As Object) As IEnumerable(Of T)
    Function ItemsGreaterThanOrEqualTo(ByVal pivot As Object) As IEnumerable(Of T)
    Function ItemsEqualTo(ByVal pivot As Object) As IEnumerable(Of T)
  End Interface

End Namespace

