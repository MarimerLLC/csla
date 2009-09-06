Imports System
Imports System.IO
Imports System.Collections.Generic
Imports Csla.Serialization
Imports System.Resources
Imports Csla.Serialization.Mobile

Namespace Core.FieldManager

  ''' <summary>
  ''' Manages properties and property data for
  ''' a business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class FieldDataManager
    Implements IUndoableObject
    Implements IMobileObject

    Private _businessObjectType As String

    <NonSerialized()> _
    Private _parent As BusinessBase

    <NonSerialized()> _
    Private _propertyList As List(Of IPropertyInfo)

    Private _fieldData() As IFieldData

    Private Sub New()
      'exists to support MobileFormatter
    End Sub

    Friend Sub New(ByVal businessObjectType As Type)
      SetPropertyList(businessObjectType)
      _fieldData = New IFieldData(_propertyList.Count - 1) {}
    End Sub

    ''' <summary>
    ''' Called when parent object is deserialized to
    ''' restore property list.
    ''' </summary>
    Friend Sub SetPropertyList(ByVal businessObjectType As Type)
      _businessObjectType = businessObjectType.AssemblyQualifiedName
      _propertyList = GetConsolidatedList(businessObjectType)
    End Sub

    ''' <summary>
    ''' Called by parent to set the back-reference.
    ''' </summary>
    Friend Sub SetParent(ByVal parent As BusinessBase)
      _parent = parent
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
      ForceStaticFieldInit(type)
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
        Dim source = PropertyInfoManager.GetPropertyListCache(hierarchy(index))
        source.IsLocked = True
        result.AddRange(source)
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
    <System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Advanced)> _
    Public Function GetFieldData(ByVal prop As IPropertyInfo) As IFieldData
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
        valueType = value.GetType()
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

    ''' <summary>
    ''' Gets a value indicating whether the specified field
    ''' has been changed.
    ''' </summary>
    ''' <param name="propertyInfo">
    ''' The property corresponding to the field.
    ''' </param>
    ''' <returns>True if the field has been changed.</returns>
    Public Function IsFieldDirty(ByVal propertyInfo As IPropertyInfo) As Boolean
      Try
        Dim result As Boolean
        Dim field = _fieldData(propertyInfo.Index)
        If field IsNot Nothing Then
          result = field.IsDirty

        Else
          result = False
        End If
        Return result

      Catch ex As IndexOutOfRangeException
        Throw New InvalidOperationException(My.Resources.PropertyNotRegistered, ex)
      End Try
    End Function

#End Region

#Region " IsValid/IsDirty/IsBusy"

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

    Friend Function IsBusy() As Boolean
      For Each item In _fieldData
        If item IsNot Nothing AndAlso item.IsBusy Then
          Return True
        End If
      Next item
    End Function

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

    Private Sub CopyState(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) Implements Core.IUndoableObject.CopyState

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
            child.CopyState(parentEditLevel, parentBindingEdit)
            ' indicate that there was a value here
            state(index) = New FieldData(Of Boolean)(item.Name)
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

    Private Sub UndoChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) Implements Core.IUndoableObject.UndoChanges

      If EditLevel > 0 Then
        If Me.EditLevel - 1 <> parentEditLevel Then
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
          If item IsNot Nothing Then
            Dim undoable = TryCast(item.Value, IUndoableObject)
            If undoable IsNot Nothing Then
              ' current value is undoable
              If oldItem IsNot Nothing Then
                undoable.UndoChanges(parentEditLevel, parentBindingEdit)
              Else
                _fieldData(index) = Nothing
              End If
              Continue For
            End If
          End If
          ' restore IFieldData object into field collection
          _fieldData(index) = oldItem
        Next index
      End If
    End Sub

    Private Sub AcceptChanges(ByVal parentEditLevel As Integer, ByVal parentBindingEdit As Boolean) Implements Core.IUndoableObject.AcceptChanges
      If Me.EditLevel - 1 <> parentEditLevel Then
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
              child.AcceptChanges(parentEditLevel, parentBindingEdit)
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

#Region "IMobileObject Members"

    Public Sub GetState(ByVal info As Serialization.Mobile.SerializationInfo) Implements Serialization.Mobile.IMobileObject.GetState
      info.AddValue("_businessObjectType", _businessObjectType)
      For Each data As IFieldData In _fieldData
        If data IsNot Nothing Then
          Dim mobile As IMobileObject = TryCast(data.Value, IMobileObject)
          If mobile Is Nothing Then
            info.AddValue(data.Name, data.Value, data.IsDirty)
          End If
        End If
      Next
      OnGetState(info)
    End Sub

    Public Sub IMobileObject_GetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter) Implements Serialization.Mobile.IMobileObject.GetChildren
      For Each data As IFieldData In _fieldData
        If data IsNot Nothing Then
          Dim mobile As IMobileObject = TryCast(data.Value , IMobileObject)
          If mobile IsNot Nothing Then
            Dim childInfo As SerializationInfo = formatter.SerializeObject(mobile)
            info.AddChild(data.Name, childInfo.ReferenceId, data.IsDirty)
          End If
        End If
      Next
      OnGetChildren(info, formatter)
    End Sub

    Private Sub SetState(ByVal info As Serialization.Mobile.SerializationInfo) Implements Serialization.Mobile.IMobileObject.SetState
      Dim type As String = CType(info.Values("_businessObjectType").Value, String)
      Dim businessObjecType As Type = System.Type.GetType(type)
      SetPropertyList(businessObjecType)
      _fieldData = New IFieldData(_propertyList.Count) {}
      For Each [property] As IPropertyInfo In _propertyList
        If info.Values.ContainsKey([property].Name) Then
          Dim value As SerializationInfo.FieldData = info.Values([property].Name)
          Dim data As IFieldData = GetOrCreateFieldData([property])
          data.Value = value.Value

          If Not value.IsDirty Then
            data.MarkClean()
          End If
        End If
      Next
      OnSetState(info)
    End Sub

    Private Sub SetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter) Implements Serialization.Mobile.IMobileObject.SetChildren
      For Each [property] As IPropertyInfo In _propertyList
        If info.Children.ContainsKey([property].Name) Then
          Dim childData As SerializationInfo.ChildData = info.Children([property].Name)

          Dim data As IFieldData = GetOrCreateFieldData([property])
          data.Value = formatter.GetObject(childData.ReferenceId)
          If Not childData.IsDirty Then
            data.MarkClean()
          End If
        End If
      Next
      OnSetChildren(info, formatter)
    End Sub

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overridable Sub OnGetState(ByVal info As SerializationInfo)

    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overridable Sub OnSetState(ByVal info As SerializationInfo)

    End Sub

    ''' <summary>
    ''' Override this method to insert your child object
    ''' references into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="formatter">
    ''' Reference to MobileFormatter instance. Use this to
    ''' convert child references to/from reference id values.
    ''' </param>
    Protected Overridable Sub OnGetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)

    End Sub

    ''' <summary>
    ''' Override this method to retrieve your child object
    ''' references from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="formatter">
    ''' Reference to MobileFormatter instance. Use this to
    ''' convert child references to/from reference id values.
    ''' </param>
    Protected Overridable Sub OnSetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)

    End Sub

#End Region

#Region " Force Static Field Init "

    ''' <summary>
    ''' Forces initialization of the static fields declared
    ''' by a type, and any of its base class types.
    ''' </summary>
    ''' <param name="type">Object to initialize.</param>
    Public Shared Sub ForceStaticFieldInit(ByVal type As Type)
      Dim attr = System.Reflection.BindingFlags.Static Or _
                 System.Reflection.BindingFlags.Public Or _
                 System.Reflection.BindingFlags.DeclaredOnly Or _
                 System.Reflection.BindingFlags.NonPublic

      Dim t As Type = type
      While t IsNot Nothing
        Dim fields() = t.GetFields(attr)
        If fields.Length > 0 Then
          fields(0).GetValue(Nothing)
        End If
        t = t.BaseType
      End While

    End Sub

#End Region
    
  End Class

End Namespace
