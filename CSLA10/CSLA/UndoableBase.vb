Option Strict On

Imports System.Reflection
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary

''' <summary>
''' 
''' </summary>
Namespace Core

  ''' <summary>
  ''' Implements n-level undo capabilities.
  ''' </summary>
  ''' <remarks>
  ''' You should not directly derive from this class. Your
  ''' business classes should derive from
  ''' <see cref="CSLA.BusinessBase" />.
  ''' </remarks>
  <Serializable()> _
  Public MustInherit Class UndoableBase
    Inherits CSLA.Core.BindableBase

    ' keep a stack of object state values
    <NotUndoable()> _
    Private mStateStack As New Stack

    ''' <summary>
    ''' Returns the current edit level of the object.
    ''' </summary>
    Protected ReadOnly Property EditLevel() As Integer
      Get
        Return mStateStack.Count
      End Get
    End Property

    ''' <summary>
    ''' Copies the state of the object and places the copy
    ''' onto the state stack.
    ''' </summary>
    Protected Friend Sub CopyState()

      Dim currentType As Type = Me.GetType
      Dim state As New Hashtable()
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
          ' make sure we process only our variables
          If field.DeclaringType Is currentType Then
            ' see if this field is marked as not undoable
            If Not NotUndoableField(field) Then
              ' the field is undoable, so it needs to be processed
              Dim value As Object = field.GetValue(Me)

              If field.FieldType.IsSubclassOf(GetType(BusinessCollectionBase)) Then
                ' make sure the variable has a value
                If Not value Is Nothing Then
                  ' this is a child collection, cascade the call
                  CType(value, BusinessCollectionBase).CopyState()
                  Serialization.SerializationNotification.OnSerializing(value)
                End If

              ElseIf field.FieldType.IsSubclassOf(GetType(BusinessBase)) Then
                ' make sure the variable has a value
                If Not value Is Nothing Then
                  ' this is a child object, cascade the call
                  CType(value, BusinessBase).CopyState()
                  Serialization.SerializationNotification.OnSerializing(value)
                End If

              Else
                ' this is a normal field, simply trap the value
                fieldName = field.DeclaringType.Name & "!" & field.Name
                state.Add(fieldName, value)

              End If

            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is GetType(UndoableBase)

      ' tell child objects they're about to be serialized
      For Each child As Object In state
        Serialization.SerializationNotification.OnSerializing(child)
      Next

      ' serialize the state and stack it
      Dim buffer As New MemoryStream
      Dim formatter As New BinaryFormatter
      formatter.Serialize(buffer, state)
      mStateStack.Push(buffer.ToArray)

      ' tell child objects they've been serialized
      For Each child As Object In state
        Serialization.SerializationNotification.OnSerialized(child)
      Next

      currentType = Me.GetType
      Do
        ' get the list of fields in this type
        fields = currentType.GetFields( _
                                BindingFlags.NonPublic Or _
                                BindingFlags.Instance Or _
                                BindingFlags.Public)

        For Each field In fields
          ' make sure we process only our variables
          If field.DeclaringType Is currentType Then
            ' see if this field is marked as not undoable
            If Not NotUndoableField(field) Then
              ' the field is undoable, so it needs to be processed
              Dim value As Object = field.GetValue(Me)

              If field.FieldType.IsSubclassOf(GetType(BusinessCollectionBase)) Then
                ' make sure the variable has a value
                If Not value Is Nothing Then
                  ' this is a child collection, cascade the call
                  Serialization.SerializationNotification.OnSerializing(value)
                End If

              ElseIf field.FieldType.IsSubclassOf(GetType(BusinessBase)) Then
                ' make sure the variable has a value
                If Not value Is Nothing Then
                  ' this is a child object, cascade the call
                  Serialization.SerializationNotification.OnSerializing(value)
                End If

              End If

            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is GetType(UndoableBase)

    End Sub

    ''' <summary>
    ''' Restores the object's state to the most recently
    ''' copied values from the state stack.
    ''' </summary>
    ''' <remarks>
    ''' Restores the state of the object to its
    ''' previous value by taking the data out of 
    ''' the stack and restoring it into the fields
    ''' of the object.
    ''' </remarks>
    Protected Friend Sub UndoChanges()
      ' if we are a child object we might be asked to
      ' undo below the level where we stacked states,
      ' so just do nothing in that case
      If EditLevel > 0 Then
        Dim buffer As New MemoryStream(CType(mStateStack.Pop(), Byte()))
        buffer.Position = 0
        Dim formatter As New BinaryFormatter()
        Dim state As Hashtable = CType(formatter.Deserialize(buffer), Hashtable)

        ' tell child objects they've been deserialized
        For Each child As Object In state
          Serialization.SerializationNotification.OnDeserialized(child)
        Next

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
                Dim value As Object = field.GetValue(Me)

                If field.FieldType.IsSubclassOf(GetType(BusinessCollectionBase)) Then
                  ' make sure the variable has a value
                  If Not value Is Nothing Then
                    ' this is a child collection, cascade the call
                    CType(value, BusinessCollectionBase).UndoChanges()
                    Serialization.SerializationNotification.OnDeserialized(value)
                  End If

                ElseIf field.FieldType.IsSubclassOf(GetType(BusinessBase)) Then
                  ' make sure the variable has a value
                  If Not value Is Nothing Then
                    ' this is a child object, cascade the call
                    CType(value, BusinessBase).UndoChanges()
                    Serialization.SerializationNotification.OnDeserialized(value)
                  End If

                Else
                  ' this is a regular field, restore its value
                  fieldName = field.DeclaringType.Name & "!" & field.Name
                  field.SetValue(Me, state.Item(fieldName))

                End If

              End If

            End If
          Next

          currentType = currentType.BaseType

        Loop Until currentType Is GetType(UndoableBase)

      End If

    End Sub

    ''' <summary>
    ''' Accepts any changes made to the object since the last
    ''' state copy was made.
    ''' </summary>
    ''' <remarks>
    ''' The most recent state copy is removed from the state
    ''' stack and discarded, thus committing any changes made
    ''' to the object's state.
    ''' </remarks>
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
                Dim value As Object = field.GetValue(Me)

                If field.FieldType.IsSubclassOf(GetType(BusinessCollectionBase)) Then
                  ' make sure the variable has a value
                  If Not value Is Nothing Then
                    ' it is a collection so cascade the call
                    CType(value, BusinessCollectionBase).AcceptChanges()
                  End If

                ElseIf field.FieldType.IsSubclassOf(GetType(BusinessBase)) Then
                  ' make sure the variable has a value
                  If Not value Is Nothing Then
                    ' it is a child object so cascade the call
                    CType(value, BusinessBase).AcceptChanges()
                  End If
                End If

              End If

            End If
          Next

          currentType = currentType.BaseType

        Loop Until currentType Is GetType(UndoableBase)

      End If
    End Sub

#Region " Helper Functions "

    Private Function NotUndoableField(ByVal Field As FieldInfo) As Boolean

      Return Attribute.IsDefined(Field, GetType(NotUndoableAttribute))

      '' get a list of all NotUndoableAttributes for this field
      'Dim attribs() As Object = _
      '  Field.GetCustomAttributes(GetType(NotUndoableAttribute), True)
      '' return True if any NotUndoableAttributes exist on this field
      'Return (UBound(attribs) > -1)

    End Function

#End Region

#Region " DumpState "

    ''' <summary>
    ''' Writes the object's field data into the debugger
    ''' output window in VS.NET.
    ''' </summary>
    Public Sub DumpState()

      Dim currentType As Type = Me.GetType
      Dim state As New Hashtable()
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
              If field.FieldType.IsSubclassOf(GetType(BusinessCollectionBase)) Then
                ' this is a child collection, cascade the call
                Debug.Indent()
                Debug.WriteLine("COLLECTION " & fieldName)
                If Not field.GetValue(Me) Is Nothing Then
                  CType(field.GetValue(Me), BusinessCollectionBase).DumpState()
                Else
                  Debug.WriteLine("<Nothing>")
                End If
                Debug.Unindent()

              ElseIf field.FieldType.IsSubclassOf(GetType(BusinessBase)) Then
                ' this is a child object, cascade the call
                Debug.Indent()
                Debug.WriteLine("CHILD OBJECT " & fieldName)
                If Not field.GetValue(Me) Is Nothing Then
                  CType(field.GetValue(Me), BusinessBase).DumpState()
                Else
                  Debug.WriteLine("<Nothing>")
                End If
                Debug.Unindent()

              Else
                ' this is a normal field, simply trap the value
                If Not field.GetValue(Me) Is Nothing Then
                  Debug.WriteLine(fieldName & ": " & field.GetValue(Me).ToString)
                Else
                  Debug.WriteLine(fieldName & ": <Nothing>")
                End If
              End If

            Else
              ' field is not undoable
              If Not field.GetValue(Me) Is Nothing Then
                Debug.WriteLine("<NotUndoable()> " & fieldName & ": " & field.GetValue(Me).ToString)
              Else
                Debug.WriteLine("<NotUndoable()> " & fieldName & ": <Nothing>")
              End If
            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is GetType(UndoableBase)

    End Sub

#End Region

  End Class

End Namespace
