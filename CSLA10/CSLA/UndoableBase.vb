Option Strict On

Imports System.Reflection
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Core

  <Serializable()> _
  Public MustInherit Class UndoableBase
    Inherits CSLA.Core.BindableBase

    ' keep a stack of object state values
    <NotUndoable()> _
    Private mStateStack As New Stack

    ' variables containing type info for comparisons
    Private Shared UndoableType As Type = GetType(UndoableBase)
    Private Shared BusinessType As Type = GetType(BusinessBase)
    Private Shared CollectionType As Type = GetType(BusinessCollectionBase)

    Protected ReadOnly Property EditLevel() As Integer
      Get
        Return mStateStack.Count
      End Get
    End Property

    Protected Friend Sub CopyState()

      Dim currentType As Type = Me.GetType
      Dim state As New Hashtable
      Dim fields() As FieldInfo
      Dim field As FieldInfo
      Dim fieldName As String

      Do
        ' get the list of fields in this type
        fields = currentType.GetFields( _
                                BindingFlags.NonPublic Or _
                                BindingFlags.Instance Or _
                                BindingFlags.Public)

        For Each field In fields
          If field.DeclaringType Is currentType Then
            ' see if this field is marked as not undoable
            If Not NotUndoableField(field) Then
              ' the field is undoable, so it needs to be processed
              If TypeInheritsFrom(field.FieldType, CollectionType) Then
                ' this is a child collection, cascade the call
                CType(field.GetValue(Me), BusinessCollectionBase).CopyState()

              ElseIf TypeInheritsFrom(field.FieldType, BusinessType) Then
                ' this is a child object, cascade the call
                CType(field.GetValue(Me), BusinessBase).CopyState()

              Else
                ' this is a normal field, simply trap the value
                fieldName = field.DeclaringType.Name & "!" & field.Name
                state.Add(fieldName, field.GetValue(Me))

              End If

            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is UndoableType

      ' serialize the state and stack it
      Dim buffer As New MemoryStream
      Dim formatter As New BinaryFormatter
      formatter.Serialize(buffer, state)
      mStateStack.Push(buffer.ToArray)

    End Sub

    ' restore the state of the object to its
    ' previous value by taking the data out of 
    ' the stack and restoring it into the fields
    ' of the object
    Protected Friend Sub UndoChanges()
      ' if we are a child object we might be asked to
      ' undo below the level where we stacked states,
      ' so just do nothing in that case
      If EditLevel > 0 Then
        Dim buffer As New MemoryStream(CType(mStateStack.Pop(), Byte()))
        buffer.Position = 0
        Dim formatter As New BinaryFormatter
        Dim state As Hashtable = CType(formatter.Deserialize(buffer), Hashtable)

        Dim currentType As Type = Me.GetType
        Dim fields() As FieldInfo
        Dim field As FieldInfo
        Dim fieldName As String


        Do
          ' get the list of fields in this type
          fields = currentType.GetFields( _
                                  BindingFlags.NonPublic Or _
                                  BindingFlags.Instance Or _
                                  BindingFlags.Public)

          For Each field In fields
            If field.DeclaringType Is currentType Then
              ' see if the field is undoable or not
              If Not NotUndoableField(field) Then
                ' the field is undoable, so restore its value

                If TypeInheritsFrom(field.FieldType, CollectionType) Then
                  ' this is a child collection, cascade the call
                  CType(field.GetValue(Me), BusinessCollectionBase).UndoChanges()

                ElseIf TypeInheritsFrom(field.FieldType, BusinessType) Then
                  ' this is a child object, cascade the call
                  CType(field.GetValue(Me), BusinessBase).UndoChanges()

                Else
                  ' this is a regular field, restore its value
                  fieldName = field.DeclaringType.Name & "!" & field.Name
                  field.SetValue(Me, state.Item(fieldName))

                End If

              End If

            End If
          Next

          currentType = currentType.BaseType

        Loop Until currentType Is UndoableType

      End If

    End Sub

    Protected Friend Sub AcceptChanges()
      If EditLevel > 0 Then
        mStateStack.Pop()

        Dim currentType As Type = Me.GetType
        Dim fields() As FieldInfo
        Dim field As FieldInfo
        Dim fieldName As String

        Do
          ' get the list of fields in this type
          fields = currentType.GetFields( _
                                  BindingFlags.NonPublic Or _
                                  BindingFlags.Instance Or _
                                  BindingFlags.Public)

          For Each field In fields
            If field.DeclaringType Is currentType Then
              ' see if the field is undoable or not
              If Not NotUndoableField(field) Then
                ' the field is undoable so see if it is a collection
                If TypeInheritsFrom(field.FieldType, CollectionType) Then
                  ' it is a collection so cascade the call
                  CType(field.GetValue(Me), BusinessCollectionBase).AcceptChanges()

                ElseIf TypeInheritsFrom(field.FieldType, BusinessType) Then
                  ' it is a child object so cascade the call
                  CType(field.GetValue(Me), BusinessBase).AcceptChanges()

                End If

              End If

            End If
          Next

          currentType = currentType.BaseType

        Loop Until currentType Is UndoableType

      End If
    End Sub

#Region " Helper Functions "

    Private Function NotUndoableField(ByVal Field As FieldInfo) As Boolean

      ' get a list of all NotUndoableAttributes for this field
      Dim attribs() As Object = _
        Field.GetCustomAttributes(GetType(NotUndoableAttribute), True)
      ' return True if any NotUndoableAttributes exist on this field
      Return (UBound(attribs) > -1)

    End Function

    Private Function TypeInheritsFrom( _
      ByVal TypeToCheck As Type, ByVal CheckAgainst As Type) As Boolean

      Dim base As Type = TypeToCheck

      ' scan up through the inheritance hierarchy, checking each
      ' class to see if it is the one we're looking for
      While Not base.BaseType Is Nothing
        ' if we find the target class return True
        If base Is CheckAgainst Then Return True
        base = base.BaseType
      End While

      ' the target class is not in the inheritance hierarchy so
      ' return False
      Return False

    End Function

#End Region

    Public Sub DumpState()

      Dim currentType As Type = Me.GetType
      Dim state As New Hashtable
      Dim field As FieldInfo
      Dim fieldName As String

      Dim fields() As FieldInfo

      Debug.IndentSize = 2
      Debug.WriteLine("OBJECT " & currentType.Name)
      Debug.WriteLine("UndoableBase!EditLevel: " & EditLevel)

      Do
        ' get the list of fields in this type
        fields = currentType.GetFields( _
                                BindingFlags.NonPublic Or _
                                BindingFlags.Instance Or _
                                BindingFlags.Public)

        For Each field In fields
          If field.DeclaringType Is currentType Then
            fieldName = field.DeclaringType.Name & "!" & field.Name
            ' see if this field is marked as not undoable
            If Not NotUndoableField(field) Then
              ' the field is undoable, so it needs to be processed
              If TypeInheritsFrom(field.FieldType, CollectionType) Then
                ' this is a child collection, cascade the call
                Debug.Indent()
                Debug.WriteLine("COLLECTION " & fieldName)
                CType(field.GetValue(Me), BusinessCollectionBase).DumpState()
                Debug.Unindent()

              ElseIf TypeInheritsFrom(field.FieldType, BusinessType) Then
                ' this is a child object, cascade the call
                Debug.Indent()
                Debug.WriteLine("CHILD OBJECT " & fieldName)
                CType(field.GetValue(Me), BusinessBase).DumpState()
                Debug.Unindent()

              Else
                ' this is a normal field, simply trap the value
                Debug.WriteLine(fieldName & ": " & field.GetValue(Me).ToString)

              End If

            Else
              ' field is not undoable
              Debug.WriteLine("<NotUndoable()> " & fieldName & ": " & field.GetValue(Me).ToString)
            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is UndoableType

    End Sub

  End Class

End Namespace
