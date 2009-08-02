Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports Csla.Serialization
Imports Csla.Reflection

Namespace Core

  ''' <summary>
  ''' Implements n-level undo capabilities as
  ''' described in Chapters 2 and 3.
  ''' </summary>
  <Serializable()> _
  Public MustInherit Class UndoableBase
    Inherits Core.BindableBase

    Implements IUndoableObject

    ' keep a stack of object state values
    <NotUndoable()> _
    Private _stateStack As New Stack(Of Byte())
    <NotUndoable()> _
    Private _bindingEdit As Boolean

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
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected Property BindingEdit() As Boolean
      Get
        Return _bindingEdit
      End Get
      Set(ByVal value As Boolean)
        _bindingEdit = value
      End Set
    End Property

    ''' <summary>
    ''' Returns the current edit level of the object.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Never)> _
    Protected ReadOnly Property EditLevel() As Integer Implements IUndoableObject.EditLevel
      Get
        Return _stateStack.Count
      End Get
    End Property

    Sub CopyState(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) Implements IUndoableObject.CopyState
      If Not parentBindingEdit Then
        CopyState(parentEditLevel)
      End If
    End Sub

    Sub UndoChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) Implements IUndoableObject.UndoChanges
      If Not parentBindingEdit Then
        UndoChanges(parentEditLevel)
      End If
    End Sub

    Sub AcceptChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) _
      Implements IUndoableObject.AcceptChanges
      If Not parentBindingEdit Then
        AcceptChanges(parentEditLevel)
      End If
    End Sub

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
    Protected Friend Sub CopyState(ByVal parentEditLevel As Integer)

      CopyingState()

      Dim currentType As Type = Me.GetType()
      Dim state As New HybridDictionary()

      If Me.EditLevel + 1 > parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "CopyState"))
      End If

      Do
        ' get the list of fields in this type
        Dim handlers As List(Of DynamicMemberHandle) = UndoableHandler.GetCachedFieldHandlers(currentType)

        For Each h As DynamicMemberHandle In handlers
          Dim value = h.DynamicMemberGet(Me)

          If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType) Then

            'make sure the variable has a value
            If value Is Nothing Then
              'variable has no value - store that fact
              state.Add(GetFieldName(currentType.FullName, h.MemberName), Nothing)
            Else
              'this is a child object, cascade the call
              DirectCast(value, IUndoableObject).CopyState(Me.EditLevel + 1, BindingEdit)
            End If
          Else
            ' this is a normal field, simply trap the value
            state.Add(GetFieldName(currentType.FullName, h.MemberName), value)
          End If
        Next

        currentType = currentType.BaseType
      Loop While currentType IsNot GetType(UndoableBase)

      ' serialize the state and stack it
      Using buffer As New MemoryStream
        Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
        formatter.Serialize(buffer, state)
        _stateStack.Push(buffer.ToArray)
      End Using
      CopyStateComplete()

    End Sub

    ''' <summary>
    ''' This method is invoked after the UndoChanges
    ''' operation begins.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub UndoChangesComplete()

    End Sub

    ''' <summary>
    ''' This method is invoked before the UndoChanges
    ''' operation is complete.
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Protected Overridable Sub UndoingChanges()

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
    Protected Friend Sub UndoChanges(ByVal parentEditLevel As Integer)

      UndoingChanges()

      ' if we are a child object we might be asked to
      ' undo below the level where we stacked states,
      ' so just do nothing in that case
      If EditLevel > 0 Then
        If Me.EditLevel - 1 <> parentEditLevel Then
          Throw New UndoException( _
            String.Format(My.Resources.EditLevelMismatchException, "UndoChanges"))
        End If

        Dim state As HybridDictionary
        Using buffer As New MemoryStream(_stateStack.Pop())
          buffer.Position = 0
          Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
          state = _
            CType(formatter.Deserialize(buffer), HybridDictionary)
        End Using

        Dim currentType As Type = Me.GetType

        Do
          ' get the list of fields in this type
          Dim handlers As List(Of DynamicMemberHandle) = UndoableHandler.GetCachedFieldHandlers(currentType)

          For Each h As DynamicMemberHandle In handlers

            'the field is undoable, so restore its value
            Dim value = h.DynamicMemberGet(Me)

            If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType) Then

              'this is a child object
              'see if the previous value was empty
              If state.Contains(GetFieldName(currentType.FullName, h.MemberName)) Then

                'previous value was empty - restore to empty
                h.DynamicMemberSet.Invoke(Me, Nothing)
              Else
                'make sure the variable has a value
                If value IsNot Nothing Then
                  'this is a child object, cascade the call.
                  DirectCast(value, IUndoableObject).UndoChanges(Me.EditLevel, BindingEdit)
                End If
              End If
            Else
              'this is a regular field, restore its value
              h.DynamicMemberSet.Invoke(Me, state(GetFieldName(currentType.FullName, h.MemberName)))
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
    Protected Friend Sub AcceptChanges(ByVal parentEditLevel As Integer)

      AcceptingChanges()

      If Me.EditLevel - 1 <> parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "AcceptChanges"))
      End If

      If EditLevel > 0 Then
        _stateStack.Pop()

        Dim currentType As Type = Me.GetType
        

        Do
          ' get the list of fields in this type
          Dim handlers As List(Of DynamicMemberHandle) = UndoableHandler.GetCachedFieldHandlers(currentType)

          For Each h As DynamicMemberHandle In handlers

            'the field is undoable so see if it is a child object
            If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(h.MemberType) Then

              Dim value As Object = h.DynamicMemberGet(Me)
              If value IsNot Nothing Then
                'it is a child object so cascade the call
                DirectCast(value, IUndoableObject).AcceptChanges(Me.EditLevel, BindingEdit)
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

    ''' <summary>
    ''' Returns the full name of a field, including
    ''' the containing type name.
    ''' </summary>
    ''' <param name="typeName">Name of the containing type.</param>
    ''' <param name="memberName">Name of the member (field).</param>
    Private Function GetFieldName(ByVal typeName As String, ByVal memberName As String) As String
      Return typeName + "." + memberName
    End Function

    Private Function GetFieldName(ByVal field As FieldInfo) As String

      Return field.DeclaringType.FullName & "!" & field.Name

    End Function

#End Region

#Region " Reset child edit level "

    Friend Shared Sub ResetChildEditLevel(ByVal child As IUndoableObject, ByVal parentEditLevel As Integer, ByVal bindingEdit As Boolean)

      Dim targetLevel As Integer = parentEditLevel
      If bindingEdit AndAlso targetLevel > 0 AndAlso Not (child Is GetType(FieldManager.FieldDataManager)) Then _
        targetLevel -= 1

      ' if item's edit level is too high,
      ' reduce it to match list
      While child.EditLevel > targetLevel
        child.AcceptChanges(targetLevel, False)
      End While
      ' if item's edit level is too low,
      ' increase it to match list
      While child.EditLevel < targetLevel
        child.CopyState(targetLevel, False)
      End While

    End Sub

#End Region

#Region " MobileObject overrides "

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
      info.AddValue("_bindingEdit", _bindingEdit)
      MyBase.OnGetState(info, mode)
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="mode">
    ''' The StateMode indicating why this method was invoked.
    ''' </param>
    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
      _stateStack.Clear()
      _bindingEdit = info.GetValue(Of Boolean)("_bindingEdit")
      MyBase.OnSetState(info, mode)
    End Sub

#End Region

  End Class

End Namespace
