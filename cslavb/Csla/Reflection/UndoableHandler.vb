Imports System
Imports System.Collections.Generic
Imports System.Reflection

Namespace Reflection
  NotInheritable Class UndoableHandler
    Private Sub New()
    End Sub
    Private Shared ReadOnly _undoableFieldCache As New Dictionary(Of Type, List(Of DynamicMemberHandle))()

    Public Shared Function GetCachedFieldHandlers(ByVal type As Type) As List(Of DynamicMemberHandle)
      Dim handlers As List(Of DynamicMemberHandle)
      If Not _undoableFieldCache.TryGetValue(type, handlers) Then
        Dim newHandlers = BuildHandlers(type)
        SyncLock _undoableFieldCache
          'ready to add, lock 
          If Not _undoableFieldCache.TryGetValue(type, handlers) Then
            _undoableFieldCache.Add(type, newHandlers)
            handlers = newHandlers
          End If
        End SyncLock
      End If
      Return handlers
    End Function

    Private Shared Function BuildHandlers(ByVal type As Type) As List(Of DynamicMemberHandle)
      Dim handlers = New List(Of DynamicMemberHandle)()
      ' get the list of fields in this type 
      Dim fields = type.GetFields(BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.[Public])

      For Each field As FieldInfo In fields
        ' make sure we process only our variables 
        If field.DeclaringType Is type Then
          ' see if this field is marked as not undoable 
          If Not NotUndoableField(field) Then
            ' the field is undoable, so it needs to be processed. 
            handlers.Add(New DynamicMemberHandle(field))
          End If
        End If
      Next
      Return handlers
    End Function

    Private Shared Function NotUndoableField(ByVal field As FieldInfo) As Boolean
      Return Attribute.IsDefined(field, GetType(NotUndoableAttribute))
    End Function

  End Class
End Namespace