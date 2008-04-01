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
    Private _propertyList As List(Of IPropertyInfo)
    Private _fieldData() As IFieldData

    Friend Sub New(ByVal businessObjectType As Type)
      _propertyList = GetConsolidatedList(businessObjectType)
      _fieldData = New IFieldData(_propertyList.Count - 1) {}
    End Sub

    ''' <summary>
    ''' Called when parent object is deserialized to
    ''' restore property list.
    ''' </summary>
    Friend Sub SetPropertyList(ByVal businessObjectType As Type)
      _propertyList = GetConsolidatedList(businessObjectType)
    End Sub

    ''' <summary>
    ''' Returns a copy of the property list for
    ''' the business object. Returns
    ''' null if there are no properties registered
    ''' for this object.
    ''' </summary>
    Public Function GetRegisteredProperties() As List(Of IPropertyInfo)
      Return New List(Of IPropertyInfo)(_propertyList)
    End Function

    ''' <summary>
    ''' Gets a value indicating whether there
    ''' are any managed fields available.
    ''' </summary>
    Public ReadOnly Property HasFields() As Boolean
      Get
        Return _propertyList.Count > 0
      End Get
    End Property

#Region "ConsolidatedPropertyList"

    Private Shared _consolidatedLists As Dictionary(Of Type, List(Of IPropertyInfo)) = New Dictionary(Of Type, List(Of IPropertyInfo))()

    Private Shared Function GetConsolidatedList(ByVal type As Type) As List(Of IPropertyInfo)
      Dim result As List(Of IPropertyInfo) = Nothing
      If (Not _consolidatedLists.TryGetValue(type, result)) Then
        SyncLock _consolidatedLists
          If (Not _consolidatedLists.TryGetValue(type, result)) Then
            result = CreateConsolidatedList(type)
            _consolidatedLists.Add(type, result)
          End If
        End SyncLock
      End If
      Return result
    End Function

    Private Shared Function CreateConsolidatedList(ByVal type As Type) As List(Of IPropertyInfo)
      Dim result As List(Of IPropertyInfo) = New List(Of IPropertyInfo)()
      ' get inheritance hierarchy
      Dim current As Type = type
      Dim hierarchy As List(Of Type) = New List(Of Type)()
      Do
        hierarchy.Add(current)
        current = current.BaseType
      Loop While current IsNot Nothing AndAlso Not current.Equals(GetType(BusinessBase))
      ' walk from top to bottom to build consolidated list
      For index As Integer = hierarchy.Count - 1 To 0 Step -1
        result.AddRange(PropertyInfoManager.GetPropertyListCache(hierarchy(index)))
      Next index
      ' set Index properties on all unindexed PropertyInfo objects
      Dim max As Integer = -1
      For Each item In result
        If item.Index = -1 Then
          max += 1
          item.Index = max
        Else
          max = item.Index
        End If
      Next item
      ' return consolidated list
      Return result
    End Function

#End Region

#Region " Get/Set/Find fields"

    ''' <summary>
    ''' Gets the <see cref="IFieldData" /> object
    ''' for a specific field.
    ''' </summary>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    Friend Function GetFieldData(ByVal prop As IPropertyInfo) As IFieldData
      Try
        Return _fieldData(prop.Index)

      Catch ex As IndexOutOfRangeException
        Throw New InvalidOperationException(My.Resources.PropertyNotRegistered, ex)
      End Try

    End Function

    Private Function GetOrCreateFieldData(ByVal prop As IPropertyInfo) As IFieldData
      Try
        Dim field = _fieldData(prop.Index)
        If field Is Nothing Then
          field = prop.NewFieldData(prop.Name)
          _fieldData(prop.Index) = field
        End If
        Return field

      Catch ex As IndexOutOfRangeException
        Throw New InvalidOperationException(My.Resources.PropertyNotRegistered, ex)
      End Try

    End Function

    Friend Function FindProperty(ByVal value As Object) As IPropertyInfo
      Dim index = 0
      For Each item In _fieldData
        If item IsNot Nothing AndAlso item.Value IsNot Nothing AndAlso item.Value.Equals(value) Then
          Return _propertyList(index)
        End If
        index += 1
      Next item
      Return Nothing
    End Function

    ''' <summary>
    ''' Sets the value for a specific field.
    ''' </summary>
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    ''' <param name="value">
    ''' Value to store for field.
    ''' </param>
    Friend Sub SetFieldData(ByVal prop As IPropertyInfo, ByVal value As Object)
      Dim valueType As Type
      If value IsNot Nothing Then
        valueType = value.GetType
      Else
        valueType = prop.Type
      End If
      value = Utilities.CoerceValue(prop.Type, valueType, Nothing, value)
      Dim field = GetOrCreateFieldData(prop)
      field.Value = value
    End Sub

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
    Friend Sub SetFieldData(Of P)(ByVal prop As IPropertyInfo, ByVal value As P)
      Dim field = GetOrCreateFieldData(prop)
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
    ''' <param name="prop">
    ''' The property corresponding to the field.
    ''' </param>
    ''' <param name="value">
    ''' Value to store for field.
    ''' </param>
    Friend Function LoadFieldData(ByVal prop As IPropertyInfo, ByVal value As Object) As IFieldData
      Dim valueType As Type
      If value IsNot Nothing Then
        valueType = value.GetType
      Else
        valueType = prop.Type
      End If
      value = Utilities.CoerceValue(prop.Type, valueType, Nothing, value)
      Dim field = GetOrCreateFieldData(prop)
      field.Value = value
      field.MarkClean()
      Return field
    End Function

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
    Friend Function LoadFieldData(Of P)(ByVal prop As IPropertyInfo, ByVal value As P) As IFieldData
      Dim field = GetOrCreateFieldData(prop)
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
    Friend Sub RemoveField(ByVal prop As IPropertyInfo)
      Try
        Dim field = _fieldData(prop.Index)
        If field IsNot Nothing Then
          field.Value = Nothing
        End If

      Catch ex As IndexOutOfRangeException
        Throw New InvalidOperationException(My.Resources.PropertyNotRegistered, ex)
      End Try

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
      Try
        Return _fieldData(propertyInfo.Index) IsNot Nothing

      Catch ex As IndexOutOfRangeException
        Throw New InvalidOperationException(My.Resources.PropertyNotRegistered, ex)
      End Try

    End Function

#End Region

#Region " IsValid/IsDirty"

    ''' <summary>
    ''' Returns a value indicating whether all
    ''' fields are valid.
    ''' </summary>
    Public Function IsValid() As Boolean
      For Each item In _fieldData
        If item IsNot Nothing AndAlso (Not item.IsValid) Then
          Return False
        End If
      Next item
      Return True
    End Function

    ''' <summary>
    ''' Returns a value indicating whether any
    ''' fields are dirty.
    ''' </summary>
    Public Function IsDirty() As Boolean
      For Each item In _fieldData
        If item IsNot Nothing AndAlso item.IsDirty Then
          Return True
        End If
      Next item
      Return False
    End Function

    ''' <summary>
    ''' Marks all fields as clean
    ''' (not dirty).
    ''' </summary>
    Friend Sub MarkClean()
      For Each item In _fieldData
        If item IsNot Nothing AndAlso item.IsDirty Then
          item.MarkClean()
        End If
      Next item
    End Sub

#End Region

#Region " IUndoableObject"

    Private _stateStack As Stack(Of Byte()) = New Stack(Of Byte())()

    ''' <summary>
    ''' Gets the current edit level of the object.
    ''' </summary>
    Public ReadOnly Property EditLevel() As Integer Implements IUndoableObject.EditLevel
      Get
        Return _stateStack.Count
      End Get
    End Property

    Private Sub CopyState(ByVal parentEditLevel As Integer) Implements Core.IUndoableObject.CopyState
      If Me.EditLevel + 1 > parentEditLevel Then
        Throw New UndoException(String.Format(My.Resources.EditLevelMismatchException, "CopyState"))
      End If

      Dim state(_propertyList.Count - 1) As IFieldData

      For index = 0 To _fieldData.Length - 1
        Dim item = _fieldData(index)
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
      Next index

      ' serialize the state and stack it
      Using buffer As New MemoryStream()
        Dim formatter = SerializationFormatterFactory.GetFormatter()
        formatter.Serialize(buffer, state)
        _stateStack.Push(buffer.ToArray())
      End Using
    End Sub

    Private Sub UndoChanges(ByVal parentEditLevel As Integer) Implements Core.IUndoableObject.UndoChanges
      If EditLevel > 0 Then
        If Me.EditLevel - 1 < parentEditLevel Then
          Throw New UndoException(String.Format(My.Resources.EditLevelMismatchException, "UndoChanges"))
        End If

        Dim state() As IFieldData = Nothing
        Using buffer As New MemoryStream(_stateStack.Pop())
          buffer.Position = 0
          Dim formatter = SerializationFormatterFactory.GetFormatter()
          state = CType(formatter.Deserialize(buffer), IFieldData())
        End Using

        For index = 0 To _fieldData.Length - 1
          Dim oldItem = state(index)
          Dim item = _fieldData(index)
          If oldItem Is Nothing AndAlso item IsNot Nothing Then
            ' potential child object
            Dim child = TryCast(item.Value, IUndoableObject)
            If child IsNot Nothing Then
              child.UndoChanges(parentEditLevel)
            Else
              ' null value
              _fieldData(index) = Nothing
            End If
          Else
            ' restore IFieldData object into field collection
            _fieldData(index) = state(index)
          End If
        Next index
      End If
    End Sub

    Private Sub AcceptChanges(ByVal parentEditLevel As Integer) Implements Core.IUndoableObject.AcceptChanges
      If Me.EditLevel - 1 < parentEditLevel Then
        Throw New UndoException(String.Format(My.Resources.EditLevelMismatchException, "AcceptChanges"))
      End If

      If EditLevel > 0 Then
        ' discard latest recorded state
        _stateStack.Pop()

        For Each item In _fieldData
          If item IsNot Nothing Then
            Dim child = TryCast(item.Value, IUndoableObject)
            If child IsNot Nothing Then
              ' cascade call to child
              child.AcceptChanges(parentEditLevel)
            End If
          End If
        Next item
      End If
    End Sub

#End Region

#Region " Child Objects "


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
      Dim result As List(Of Object) = New List(Of Object)()
      For Each item In _fieldData
        If item IsNot Nothing AndAlso (TypeOf item.Value Is IEditableBusinessObject OrElse TypeOf item.Value Is IEditableCollection) Then
          result.Add(item.Value)
        End If
      Next item
      Return result
    End Function

    ''' <summary>
    ''' Invokes the data portal to update
    ''' all child objects contained in 
    ''' the list of fields.
    ''' </summary>
    Public Sub UpdateChildren(ByVal ParamArray parameters() As Object)
      For Each item In _fieldData
        If item IsNot Nothing Then
          Dim obj As Object = item.Value
          If TypeOf obj Is IEditableBusinessObject OrElse TypeOf obj Is IEditableCollection Then
            Csla.DataPortal.UpdateChild(obj, parameters)
          End If
        End If
      Next item
    End Sub

#End Region

  End Class

End Namespace
