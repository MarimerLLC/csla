Imports System
Imports System.Collections.Generic
Imports System.Reflection

Namespace Linq
  ''' <summary>
  ''' Interface that determines functionality of an index
  ''' </summary>
  Public Interface IIndex(Of T)
    Inherits ICollection(Of T)
    ''' <summary>
    ''' Field this index is indexing on
    ''' </summary>
    ReadOnly Property IndexField() As PropertyInfo
    ''' <summary>
    ''' Iterator that returns objects where there is a match based on the passed item
    ''' </summary>
    Function WhereEqual(ByVal item As T) As IEnumerable(Of T)
    ''' <summary>
    ''' Iterator that returns objects based on the expression and the item hashcode passed in
    ''' </summary>
    Function WhereEqual(ByVal pivotVal As Object, ByVal expr As Func(Of T, Boolean)) As IEnumerable(Of T)
    ''' <summary>
    ''' Reindex an item in this index
    ''' </summary>
    Sub ReIndex(ByVal item As T)
    ''' <summary>
    ''' Determine whether the given index is loaded or not
    ''' </summary>
    ReadOnly Property Loaded() As Boolean
    ''' <summary>
    ''' Set the index as not loaded anymore
    ''' </summary>
    Sub InvalidateIndex()
    ''' <summary>
    ''' Set the index as as loaded
    ''' </summary>
    Sub LoadComplete()
    ''' <summary>
    ''' Determine the index mode (always, ondemand, never)
    ''' </summary>
    Property IndexMode() As IndexModeEnum
  End Interface
End Namespace