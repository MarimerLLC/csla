Imports System.Linq.Expressions

Namespace Linq
  Friend Interface IIndexSearchable(Of T)
    Function SearchByExpression(ByVal expr As Expression(Of Func(Of T, Boolean))) As IEnumerable(Of T)
  End Interface
End Namespace