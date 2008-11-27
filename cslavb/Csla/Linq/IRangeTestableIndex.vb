Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Linq

  ''' <summary>
  ''' Implemented by objects that support range
  ''' comparisons.
  ''' </summary>
  ''' <typeparam name="T">Type of object.</typeparam>
  Public Interface IRangeTestableIndex(Of T)
    Inherits IIndex(Of T)

    ''' <summary>
    ''' Implements a less than clause.
    ''' </summary>
    ''' <param name="pivotVal">Pivot value.</param>
    Function WhereLessThan(ByVal pivotVal As Object) As IEnumerable(Of T)
    ''' <summary>
    ''' Implements a greater than clause.
    ''' </summary>
    ''' <param name="pivotVal">Pivot value.</param>
    Function WhereGreaterThan(ByVal pivotVal As Object) As IEnumerable(Of T)
    ''' <summary>
    ''' Implements a less than or equal clause.
    ''' </summary>
    ''' <param name="pivotVal">Pivot value.</param>
    Function WhereLessThanOrEqualTo(ByVal pivotVal As Object) As IEnumerable(Of T)
    ''' <summary>
    ''' Implements a greater than or equal clause.
    ''' </summary>
    ''' <param name="pivotVal">Pivot value.</param>
    Function WhereGreaterThanOrEqualTo(ByVal pivotVal As Object) As IEnumerable(Of T)

  End Interface
End Namespace

