Imports System.IO
Imports System.Collections.Specialized
Imports Csla.Serialization

Namespace Core.FieldDataManager

  ''' <summary>
  ''' Manages properties and property data for
  ''' a business object.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public Class FieldDataManager
    Implements IUndoableObject

    Friend Sub New()
      ' prevent creation from outside this assembly
    End Sub

#Region " FieldData "

    Private mFields As Dictionary(Of String, IFieldData)
    Private ReadOnly Property FieldData() As Dictionary(Of String, IFieldData)
      Get
        If mFields Is Nothing Then
          mFields = New Dictionary(Of String, IFieldData)
        End If
        Return mFields
      End Get
    End Property

    Friend ReadOnly Property HasFieldData() As Boolean
      Get
        Return mFields IsNot Nothing
      End Get
    End Property

#End Region

#Region " Get/Set/Find fields "

    Protected Friend Function GetFieldData(ByVal prop As IPropertyInfo) As IFieldData

      If FieldData.ContainsKey(prop.Name) Then
        Return FieldData(prop.Name)

      Else
        Return Nothing
      End If

    End Function

    Protected Friend Function FindPropertyName(ByVal value As Object) As String

      For Each item In FieldData
        If ReferenceEquals(item.Value.Value, value) Then
          Return item.Key
        End If
      Next
      Return Nothing

    End Function

    Protected Friend Sub SetFieldData(ByVal prop As IPropertyInfo, ByVal value As Object)

      If Not FieldData.ContainsKey(prop.Name) Then
        FieldData.Add(prop.Name, prop.NewFieldData)
      End If
      FieldData(prop.Name).Value = value

    End Sub

    Protected Friend Sub LoadFieldData(ByVal prop As IPropertyInfo, ByVal value As Object)

      If Not FieldData.ContainsKey(prop.Name) Then
        FieldData.Add(prop.Name, prop.NewFieldData)
      End If
      FieldData(prop.Name).Value = value
      FieldData(prop.Name).MarkClean()

    End Sub

    Protected Friend Sub RemoveField(ByVal propertyName As String)

      If FieldData.ContainsKey(propertyName) Then
        FieldData(propertyName).Value = Nothing
      End If

    End Sub

    Public Function FieldExists(ByVal propertyInfo As IPropertyInfo) As Boolean

      Return FieldData.ContainsKey(propertyInfo.Name)

    End Function

    Public Function GetChildren() As List(Of Object)

      Dim result As New List(Of Object)
      For Each item In FieldData
        If TypeOf item.Value.Value Is IEditableBusinessObject OrElse TypeOf item.Value.Value Is IEditableCollection Then
          result.Add(item.Value.Value)
        End If
      Next
      Return result

    End Function

#End Region

#Region " IsValid/IsDirty "

    Public Function IsValid() As Boolean

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing AndAlso Not item.Value.IsValid Then
          Return False
        End If
      Next
      Return True

    End Function

    Public Function IsDirty() As Boolean

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing AndAlso item.Value.IsDirty Then
          Return True
        End If
      Next
      Return False

    End Function

    Public Sub MarkClean()

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing AndAlso item.Value.IsDirty Then
          item.Value.MarkClean()
        End If
      Next

    End Sub

#End Region

#Region " IUndoableObject "

    Private mStateStack As New Stack(Of Byte())

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

      Dim state As New HybridDictionary

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        Dim child As IUndoableObject = TryCast(item.Value.Value, IUndoableObject)
        If child IsNot Nothing Then
          ' cascade call to child
          child.CopyState(parentEditLevel)
          ' store fact that child exists
          state.Add(item.Key, True)

        Else
          ' add the IFieldData object
          state.Add(item.Key, item.Value)
        End If
      Next

      ' serialize the state and stack it
      Using buffer As New MemoryStream
        Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
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

        Dim state As HybridDictionary
        Using buffer As New MemoryStream(mStateStack.Pop())
          buffer.Position = 0
          Dim formatter As ISerializationFormatter = SerializationFormatterFactory.GetFormatter
          state = _
            CType(formatter.Deserialize(buffer), HybridDictionary)
        End Using

        Dim oldFields As Dictionary(Of String, IFieldData) = mFields
        mFields = New Dictionary(Of String, IFieldData)

        For Each item As DictionaryEntry In state
          Dim key As String = CStr(item.Key)
          If TypeOf item.Value Is Boolean Then
            ' get child object from old field collection
            Dim child As IFieldData = DirectCast(oldFields(key), IFieldData)
            ' cascade call to child
            DirectCast(child, IUndoableObject).UndoChanges(parentEditLevel)
            mFields.Add(key, child)

          Else
            ' restore IFieldData object into field collection
            mFields.Add(key, DirectCast(item.Value, IFieldData))
          End If
        Next
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

        For Each item As KeyValuePair(Of String, IFieldData) In FieldData
          Dim child As IUndoableObject = TryCast(item.Value.Value, IUndoableObject)
          If child IsNot Nothing Then
            ' cascade call to child
            child.AcceptChanges(parentEditLevel)
          End If
        Next

      End If

    End Sub

#End Region

#Region " Update Children "

    Protected Friend Sub UpdateChildren()

      For Each item As KeyValuePair(Of String, IFieldData) In FieldData
        If item.Value IsNot Nothing Then
          Dim obj As Object = item.Value.Value
          If TypeOf obj Is IEditableBusinessObject OrElse TypeOf obj Is IEditableCollection Then
            ' TODO: update child
          End If
        End If
      Next

    End Sub

#End Region

  End Class

End Namespace
