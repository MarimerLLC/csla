Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports Csla.Serialization

Namespace Core

  ''' <summary>
  ''' Implements n-level undo capabilities as
  ''' described in Chapters 2 and 3.
  ''' </summary>
  <Serializable()> _
  Public MustInherit Class UndoableBase
    Inherits Csla.Core.BindableBase

    Implements IUndoableObject

    ' keep a stack of object state values
    <NotUndoable()> _
    Private mStateStack As New Stack(Of Byte())
    <NotUndoable()> _
    Private mbindingEdit As Boolean

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub New()

    End Sub

    ''' <summary>
    ''' Gets or sets a value indicating whether n-level undo
    ''' was invoked through IEditableObject. FOR INTERNAL
    ''' CSLA .NET USE ONLY!
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Property BindingEdit() As Boolean
      Get
        Return mbindingEdit
      End Get
      Set(ByVal value As Boolean)
        mbindingEdit = value
      End Set
    End Property

    ''' <summary>
    ''' Returns the current edit level of the object.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected ReadOnly Property EditLevel() As Integer Implements IUndoableObject.EditLevel
      Get
        Return mStateStack.Count
      End Get
    End Property

    ''' <summary>
    ''' This method is invoked before the CopyState
    ''' operation begins.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub CopyingState()

    End Sub

    ''' <summary>
    ''' This method is invoked after the CopyState
    ''' operation is complete.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub CopyStateComplete()

    End Sub

    ''' <summary>
    ''' Copies the state of the object and places the copy
    ''' onto the state stack.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Friend Sub CopyState(ByVal parentEditLevel As Integer) Implements IUndoableObject.CopyState

      CopyingState()

      Dim currentType As Type = Me.GetType
      Dim state As New HybridDictionary()
      Dim fields() As FieldInfo

      If Me.EditLevel + 1 > parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "CopyState"))
      End If

      Do
        ' get the list of fields in this type
        fields = currentType.GetFields( _
                                BindingFlags.NonPublic Or _
                                BindingFlags.Instance Or _
                                BindingFlags.Public)

        For Each field As FieldInfo In fields
          ' make sure we process only our variables
          If field.DeclaringType Is currentType Then
            ' see if this field is marked as not undoable
            If Not NotUndoableField(field) Then
              ' the field is undoable, so it needs to be processed
              Dim value As Object = field.GetValue(Me)

              If GetType(Csla.Core.IUndoableObject). _
                  IsAssignableFrom(field.FieldType) Then
                ' make sure the variable has a value
                If value Is Nothing Then
                  ' variable has no value - store that fact
                  state.Add(GetFieldName(field), Nothing)

                Else
                  ' this is a child object, cascade the call
                  If Not mbindingEdit Then
                    DirectCast(value, IUndoableObject).CopyState(Me.EditLevel + 1)
                  End If
                End If

              Else
                ' this is a normal field, simply trap the value
                state.Add(GetFieldName(field), value)

              End If

            End If

          End If
        Next

        currentType = currentType.BaseType

      Loop Until currentType Is GetType(UndoableBase)

      ' serialize the state and stack it
      Using buffer As New MemoryStream
        Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
        formatter.Serialize(buffer, state)
        mStateStack.Push(buffer.ToArray)
      End Using
      CopyStateComplete()

    End Sub

    ''' <summary>
    ''' This method is invoked before the UndoChanges
    ''' operation begins.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub UndoingChanges()

    End Sub

    ''' <summary>
    ''' This method is invoked after the UndoChanges
    ''' operation is complete.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub UndoChangesComplete()

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
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Friend Sub UndoChanges(ByVal parentEditLevel As Integer) Implements IUndoableObject.UndoChanges

      UndoingChanges()

      ' if we are a child object we might be asked to
      ' undo below the level where we stacked states,
      ' so just do nothing in that case
      If EditLevel > 0 Then
        If Me.EditLevel - 1 < parentEditLevel Then
          Throw New UndoException( _
            String.Format(My.Resources.EditLevelMismatchException, "UndoChanges"))
        End If

        Dim state As HybridDictionary
        Using buffer As New MemoryStream(mStateStack.Pop())
          buffer.Position = 0
          Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
          state = _
            CType(formatter.Deserialize(buffer), HybridDictionary)
        End Using

        Dim currentType As Type = Me.GetType
        Dim fields() As FieldInfo
        Dim field As FieldInfo


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

                If GetType(Csla.Core.IUndoableObject). _
                  IsAssignableFrom(field.FieldType) Then
                  ' this is a child object
                  ' see if the previous value was empty
                  If state.Contains(GetFieldName(field)) Then
                    ' previous value was empty - restore to empty
                    field.SetValue(Me, Nothing)

                  Else
                    ' make sure the variable has a value
                    If Not value Is Nothing Then
                      ' this is a child object, cascade the call
                      If Not mbindingEdit Then
                        DirectCast(value, IUndoableObject).UndoChanges(Me.EditLevel)
                      End If
                    End If
                  End If

                Else
                    ' this is a regular field, restore its value
                    field.SetValue(Me, state.Item(GetFieldName(field)))
                End If
              End If
            End If
          Next

          currentType = currentType.BaseType
        Loop Until currentType Is GetType(UndoableBase)
      End If
      UndoChangesComplete()

    End Sub

    ''' <summary>
    ''' This method is invoked before the AcceptChanges
    ''' operation begins.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub AcceptingChanges()

    End Sub

    ''' <summary>
    ''' This method is invoked after the AcceptChanges
    ''' operation is complete.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub AcceptChangesComplete()

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
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Friend Sub AcceptChanges(ByVal parentEditLevel As Integer) _
      Implements IUndoableObject.AcceptChanges

      AcceptingChanges()

      If Me.EditLevel - 1 < parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "AcceptChanges"))
      End If

      If EditLevel > 0 Then
        mStateStack.Pop()

        Dim currentType As Type = Me.GetType
        Dim fields() As FieldInfo
        Dim field As FieldInfo

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
                ' the field is undoable so see if it is editable
                If GetType(Csla.Core.IUndoableObject). _
                  IsAssignableFrom(field.FieldType) Then
                  Dim value As Object = field.GetValue(Me)
                  ' make sure the variable has a value
                  If Not value Is Nothing Then
                    ' this is a child object, cascade the call
                    If Not mbindingEdit Then
                      DirectCast(value, IUndoableObject).AcceptChanges(Me.EditLevel)
                    End If
                  End If
                End If
              End If
            End If
          Next

          currentType = currentType.BaseType

        Loop Until currentType Is GetType(UndoableBase)

      End If
      AcceptChangesComplete()

    End Sub

#Region " Helper Functions "

    Private Shared Function NotUndoableField(ByVal field As FieldInfo) As Boolean

      Return Attribute.IsDefined(field, GetType(NotUndoableAttribute))

    End Function

    Private Function GetFieldName(ByVal field As FieldInfo) As String

      Return field.DeclaringType.Name & "!" & field.Name

    End Function

#End Region

#Region " Reset child edit level "

    Friend Shared Sub ResetChildEditLevel(ByVal child As IUndoableObject, ByVal parentEditLevel As Integer)

      ' if item's edit level is too high,
      ' reduce it to match list
      While child.EditLevel > parentEditLevel
        child.AcceptChanges(parentEditLevel)
      End While
      ' if item's edit level is too low,
      ' increase it to match list
      While child.EditLevel < parentEditLevel
        child.CopyState(parentEditLevel)
      End While

    End Sub

#End Region

  End Class

End Namespace
