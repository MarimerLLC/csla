Imports System.IO
Imports System.Collections.Specialized
Imports Csla.Serialization

Namespace Core.FieldManager

  ''' <summary>
  ''' Manages properties and property data for
  ''' a business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class FieldDataManager
    Implements IUndoableObject

    <NonSerialized()> _
    Private mPropertyList As List(Of IPropertyInfo)
    Private mFieldData() As IFieldData

    Friend Sub New(ByVal propertyList As List(Of IPropertyInfo))
      mPropertyList = propertyList
      ReDim mFieldData(0 To mPropertyList.Count - 1)
    End Sub

    ''' <summary>
    ''' Called when parent object is deserialized to
    ''' restore property list.
    ''' </summary>
    Friend Sub SetPropertyList(ByVal propertyList As List(Of IPropertyInfo))
      mPropertyList = propertyList
    End Sub

    '#Region " FieldData "

    '    Private mFields As FieldDataList

    '    Private ReadOnly Property FieldData() As FieldDataList
    '      Get
    '        If mFields Is Nothing Then
    '          mFields = New FieldDataList
    '        End If
    '        Return mFields
    '      End Get
    '    End Property

    '    Private ReadOnly Property HasFieldData() As Boolean
    '      Get
    '        Return mFields IsNot Nothing
    '      End Get
    '    End Property

    '#End Region

#Region " Get/Set/Find fields "

    ''' <summary>
    ''' Gets the <see cref="IFieldData" /> object
    ''' for a specific field.
    ''' </summary>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    Public Function GetFieldData(ByVal prop As IPropertyInfo) As IFieldData

      Return mFieldData(prop.Index)
      'Dim result As IFieldData = Nothing
      'If FieldData.TryGetValue(prop.Name, result) Then
      '  Return result

      'Else
      '  Return Nothing
      'End If

    End Function

    '''' <summary>
    '''' Gets the <see cref="IFieldData" /> object
    '''' for a specific field.
    '''' </summary>
    '''' <param name="key">
    '''' The property name corresponding to the field.
    '''' </param>
    'Public Function GetFieldData(ByVal key As String) As IFieldData

    '  Dim result As IFieldData = Nothing
    '  If FieldData.TryGetValue(key, result) Then
    '    Return result

    '  Else
    '    Return Nothing
    '  End If

    'End Function

    Friend Function FindProperty(ByVal value As Object) As IPropertyInfo

      Dim index = 0
      For Each item In mFieldData
        If item IsNot Nothing AndAlso item.Value.Equals(value) Then
          Return mPropertyList(index)
        End If
        index += 1
      Next
      Return Nothing
      'Return FieldData.FindPropertyName(value)

    End Function

    ''' <summary>
    ''' Sets the value for a specific field.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of field value.
    ''' </typeparam>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    ''' <param name="value">
    ''' Value to store for field.
    ''' </param>
    Public Sub SetFieldData(Of P)(ByVal prop As IPropertyInfo, ByVal value As P)

      Dim field = mFieldData(prop.Index)
      If field Is Nothing Then
        field = prop.NewFieldData(prop.Name)
        mFieldData(prop.Index) = field
      End If
      'Dim field As IFieldData = Nothing
      'If Not FieldData.TryGetValue(prop.Name, field) Then
      '  field = prop.NewFieldData(prop.Name)
      '  FieldData.Add(prop.Name, field)
      'End If
      Dim fd = TryCast(field, IFieldData(Of P))
      If fd IsNot Nothing Then
        fd.Value = value

      Else
        field.Value = value
      End If

    End Sub

    ''' <summary>
    ''' Sets the value for a specific field without
    ''' marking the field as dirty.
    ''' </summary>
    ''' <typeparam name="P">
    ''' Type of field value.
    ''' </typeparam>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    ''' <param name="value">
    ''' Value to store for field.
    ''' </param>
    Public Function LoadFieldData(Of P)(ByVal prop As IPropertyInfo, ByVal value As P) As IFieldData

      Dim field = mFieldData(prop.Index)
      If field Is Nothing Then
        field = prop.NewFieldData(prop.Name)
        mFieldData(prop.Index) = field
      End If
      'Dim field As IFieldData = Nothing
      'If Not FieldData.TryGetValue(prop.Name, field) Then
      '  field = prop.NewFieldData(prop.Name)
      '  FieldData.Add(prop.Name, field)
      'End If
      Dim fd = TryCast(field, IFieldData(Of P))
      If fd IsNot Nothing Then
        fd.Value = value

      Else
        field.Value = value
      End If
      field.MarkClean()
      Return field

    End Function

    ''' <summary>
    ''' Removes the value for a specific field.
    ''' The <see cref="IFieldData" /> object is
    ''' not removed, only the contained field value.
    ''' </summary>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    Public Sub RemoveField(ByVal prop As IPropertyInfo)

      Dim field = mFieldData(prop.Index)
      If field IsNot Nothing Then
        field.Value = Nothing
      End If
      'Dim field As IFieldData = Nothing
      'If FieldData.TryGetValue(propertyName, field) Then
      '  field.Value = Nothing
      'End If

    End Sub

    ''' <summary>
    ''' Returns a value indicating whether an
    ''' <see cref="IFieldData" /> entry exists
    ''' for the specified property.
    ''' </summary>
    ''' <param name="propertyInfo">
    ''' The property corresponding to the field.
    ''' </param>
    Public Function FieldExists(ByVal propertyInfo As IPropertyInfo) As Boolean

      Return mFieldData(propertyInfo.Index) IsNot Nothing
      'Return FieldData.ContainsKey(propertyInfo.Name)

    End Function

    ''' <summary>
    ''' Returns a list of all child objects
    ''' contained in the list of fields.
    ''' </summary>
    ''' <remarks>
    ''' This method returns a list of actual child
    ''' objects, not a list of
    ''' <see cref="IFieldData" /> container objects.
    ''' </remarks>
    Public Function GetChildren() As List(Of Object)

      Dim result As New List(Of Object)
      For Each item In mFieldData
        If item IsNot Nothing AndAlso (TypeOf item.Value Is IEditableBusinessObject OrElse TypeOf item.Value Is IEditableCollection) Then
          result.Add(item.Value)
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    If TypeOf item.Value Is IEditableBusinessObject OrElse TypeOf item.Value Is IEditableCollection Then
      '      result.Add(item.Value)
      '    End If
      '  Next
      'End If
      Return result

    End Function

#End Region

#Region " IsValid/IsDirty "

    ''' <summary>
    ''' Returns a value indicating whether all
    ''' fields are valid.
    ''' </summary>
    Public Function IsValid() As Boolean

      For Each item In mFieldData
        If item IsNot Nothing AndAlso Not item.IsValid Then
          Return False
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    If item IsNot Nothing AndAlso Not item.IsValid Then
      '      Return False
      '    End If
      '  Next
      'End If
      Return True

    End Function

    ''' <summary>
    ''' Returns a value indicating whether any
    ''' fields are dirty.
    ''' </summary>
    Public Function IsDirty() As Boolean

      For Each item In mFieldData
        If item IsNot Nothing AndAlso item.IsDirty Then
          Return True
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    If item IsNot Nothing AndAlso item.IsDirty Then
      '      Return True
      '    End If
      '  Next
      'End If
      Return False

    End Function

    ''' <summary>
    ''' Marks all fields as clean
    ''' (not dirty).
    ''' </summary>
    Public Sub MarkClean()

      For Each item In mFieldData
        If item IsNot Nothing AndAlso item.IsDirty Then
          item.MarkClean()
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    If item IsNot Nothing AndAlso item.IsDirty Then
      '      item.MarkClean()
      '    End If
      '  Next
      'End If

    End Sub

#End Region

#Region " IUndoableObject "

    Private mStateStack As New Stack(Of Byte())

    ''' <summary>
    ''' Gets the current edit level of the object.
    ''' </summary>
    Public ReadOnly Property EditLevel() As Integer Implements IUndoableObject.EditLevel
      Get
        Return mStateStack.Count
      End Get
    End Property

    Private Sub CopyState(ByVal parentEditLevel As Integer) Implements IUndoableObject.CopyState

      If Me.EditLevel + 1 > parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "CopyState"))
      End If

      Dim state(mPropertyList.Count - 1) As IFieldData

      For index = 0 To mFieldData.Length - 1
        Dim item = mFieldData(index)
        If item IsNot Nothing Then
          Dim child = TryCast(item.Value, IUndoableObject)
          If child IsNot Nothing Then
            ' cascade call to child
            child.CopyState(parentEditLevel)

          Else
            ' add the IFieldData object
            state(index) = item
          End If
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    Dim child = TryCast(item.Value, IUndoableObject)
      '    If child IsNot Nothing Then
      '      ' cascade call to child
      '      child.CopyState(parentEditLevel)
      '      ' store fact that child exists
      '      state.Add(item.Name, True)

      '    Else
      '      ' add the IFieldData object
      '      state.Add(item.Name, item)
      '    End If
      '  Next
      'End If

      ' serialize the state and stack it
      Using buffer As New MemoryStream
        Dim formatter = SerializationFormatterFactory.GetFormatter
        formatter.Serialize(buffer, state)
        mStateStack.Push(buffer.ToArray)
      End Using

    End Sub

    Private Sub UndoChanges(ByVal parentEditLevel As Integer) Implements IUndoableObject.UndoChanges

      If EditLevel > 0 Then
        If Me.EditLevel - 1 < parentEditLevel Then
          Throw New UndoException( _
            String.Format(My.Resources.EditLevelMismatchException, "UndoChanges"))
        End If

        Dim state() As IFieldData
        Using buffer As New MemoryStream(mStateStack.Pop())
          buffer.Position = 0
          Dim formatter = SerializationFormatterFactory.GetFormatter
          state = _
            CType(formatter.Deserialize(buffer), IFieldData())
        End Using

        For index = 0 To mFieldData.Length - 1
          Dim oldItem = state(index)
          Dim item = mFieldData(index)
          If oldItem Is Nothing AndAlso item IsNot Nothing Then
            ' potential child object
            Dim child = TryCast(item.Value, IUndoableObject)
            If child IsNot Nothing Then
              child.UndoChanges(parentEditLevel)

            Else
              ' null value
              mFieldData(index) = Nothing
            End If

          Else
            ' restore IFieldData object into field collection
            mFieldData(index) = state(index)
          End If
        Next
        'If HasFieldData Then
        '  Dim state As HybridDictionary
        '  Using buffer As New MemoryStream(mStateStack.Pop())
        '    buffer.Position = 0
        '    Dim formatter = SerializationFormatterFactory.GetFormatter
        '    state = _
        '      CType(formatter.Deserialize(buffer), HybridDictionary)
        '  End Using

        '  Dim oldFields = FieldData
        '  mFields = New FieldDataList

        '  For Each item As DictionaryEntry In state
        '    Dim key = CStr(item.Key)
        '    If TypeOf item.Value Is Boolean Then
        '      ' get child object from old field collection
        '      Dim child = DirectCast(oldFields.GetValue(key), IFieldData)
        '      ' add to new list
        '      FieldData.Add(key, child)
        '      ' cascade call to child
        '      DirectCast(child.Value, IUndoableObject).UndoChanges(parentEditLevel)

        '    Else
        '      ' restore IFieldData object into field collection
        '      FieldData.Add(key, DirectCast(item.Value, IFieldData))
        '    End If
        '  Next

        'Else
        '  mStateStack.Pop()
        'End If
      End If

    End Sub

    Private Sub AcceptChanges(ByVal parentEditLevel As Integer) Implements IUndoableObject.AcceptChanges

      If Me.EditLevel - 1 < parentEditLevel Then
        Throw New UndoException( _
          String.Format(My.Resources.EditLevelMismatchException, "AcceptChanges"))
      End If

      If EditLevel > 0 Then
        ' discard latest recorded state
        mStateStack.Pop()

        For Each item In mFieldData
          If item IsNot Nothing Then
            Dim child = TryCast(item.Value, IUndoableObject)
            If child IsNot Nothing Then
              ' cascade call to child
              child.AcceptChanges(parentEditLevel)
            End If
          End If
        Next
        'If HasFieldData Then
        '  For Each item In FieldData.GetFieldDataList
        '    Dim child = TryCast(item.Value, IUndoableObject)
        '    If child IsNot Nothing Then
        '      ' cascade call to child
        '      child.AcceptChanges(parentEditLevel)
        '    End If
        '  Next
        'End If
      End If

    End Sub

#End Region

#Region " Update Children "

    ''' <summary>
    ''' Invokes the data portal to update
    ''' all child objects contained in 
    ''' the list of fields.
    ''' </summary>
    Public Sub UpdateChildren(ByVal ParamArray parameters() As Object)

      For Each item In mFieldData
        If item IsNot Nothing Then
          Dim obj As Object = item.Value
          If TypeOf obj Is IEditableBusinessObject OrElse TypeOf obj Is IEditableCollection Then
            Csla.DataPortal.UpdateChild(obj, parameters)
          End If
        End If
      Next
      'If HasFieldData Then
      '  For Each item In FieldData.GetFieldDataList
      '    If item IsNot Nothing Then
      '      Dim obj As Object = item.Value
      '      If TypeOf obj Is IEditableBusinessObject OrElse TypeOf obj Is IEditableCollection Then
      '        Csla.DataPortal.UpdateChild(obj, parameters)
      '      End If
      '    End If
      '  Next
      'End If

    End Sub

#End Region

  End Class

End Namespace
