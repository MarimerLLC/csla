Imports System.Collections.ObjectModel
Imports System.Linq.Expressions

Namespace Linq
  ''' <summary>
  ''' Interface that defines a what a set of indexes should do
  ''' </summary>
  Public Interface IIndexSet(Of T)
    ''' <summary>
    ''' Insert an item into all indexes
    ''' </summary>
    Sub InsertItem(ByVal item As T)
    ''' <summary>
    ''' Insert an item into the index for the given property
    ''' </summary>
    Sub InsertItem(ByVal item As T, ByVal [property] As String)
    ''' <summary>
    ''' Remove an item from all indexes
    ''' </summary>
    Sub RemoveItem(ByVal item As T)
    ''' <summary>
    ''' Remove an item from the index for the given property
    ''' </summary>
    Sub RemoveItem(ByVal item As T, ByVal [property] As String)
    ''' <summary>
    ''' Reindex the item for all indexes
    ''' </summary>
    Sub ReIndexItem(ByVal item As T)
    ''' <summary>
    ''' Reindex the item in the index for a given property
    ''' </summary>
    Sub ReIndexItem(ByVal item As T, ByVal [property] As String)
    ''' <summary>
    ''' Clear all the indexes
    ''' </summary>
    Sub ClearIndexes()
    ''' <summary>
    ''' Clear the index for a given property
    ''' </summary>
    Sub ClearIndex(ByVal [property] As String)
    ''' <summary>
    ''' Search for items using a given index and a given expression
    ''' </summary>
    Function Search(ByVal expr As Expression(Of Func(Of T, Boolean)), ByVal [property] As String) As IEnumerable(Of T)
    ''' <summary>
    ''' Determine whether there is an index for a given property present
    ''' </summary>
    Function HasIndexFor(ByVal [property] As String) As Boolean
    ''' <summary>
    ''' Determine whether the index set has an index that enables search for a given expression
    ''' </summary>
    Function HasIndexFor(ByVal expr As Expression(Of Func(Of T, Boolean))) As String
    ''' <summary>
    ''' Return an index based on an indexer using a property name
    ''' </summary>
    Default ReadOnly Property Item(ByVal [property] As String) As IIndex(Of T)
    ''' <summary>
    ''' Tell the index set that it is time to allow for loading of an on demand index
    ''' </summary>
    Sub LoadIndex(ByVal [property] As String)
  End Interface
End Namespace