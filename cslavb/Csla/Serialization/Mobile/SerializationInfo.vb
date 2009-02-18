Option Infer On
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Xml.Linq
Imports System.Runtime.Serialization

Namespace Serialization.Mobile

  ''' <summary>
  ''' Object containing the serialization
  ''' data for a specific object.
  ''' </summary>
#If TESTING Then
  [System.Diagnostics.DebuggerNonUserCode]
#End If
  <DataContract()> _
  Public Class SerializationInfo

    ''' <summary>
    ''' Object that contains information about
    ''' a single field.
    ''' </summary>
    <DataContract()> _
    Public Class FieldData

      Private _Value As Object
      Private _IsDirty As Boolean

      ''' <summary>
      ''' Field value.
      ''' </summary>
      <DataMember()> _
      Public Property Value() As Object
        Get
          Return _Value
        End Get
        Set(ByVal value As Object)
          _Value = value
        End Set
      End Property

      ''' <summary>
      ''' Indicates whether the field is dirty.
      ''' </summary>
      <DataMember()> _
      Public Property IsDirty() As Boolean
        Get
          Return _IsDirty
        End Get
        Set(ByVal value As Boolean)
          _IsDirty = value
        End Set
      End Property

    End Class

    ''' <summary>
    ''' Object that contains information about
    ''' a single child reference.
    ''' </summary>
    <DataContract()> _
    Public Class ChildData

      Private _ReferenceId As Integer
      Private _IsDirty As Boolean

      ''' <summary>
      ''' Reference number for the child.
      ''' </summary>
      <DataMember()> _
      Public Property ReferenceId() As Integer
        Get
          Return _ReferenceId
        End Get
        Set(ByVal value As Integer)
          _ReferenceId = value
        End Set
      End Property

      ''' <summary>
      ''' Indicates whether the field is dirty.
      ''' </summary>
      <DataMember()> _
      Public Property IsDirty() As Boolean
        Get
          Return _IsDirty
        End Get
        Set(ByVal value As Boolean)
          _IsDirty = value
        End Set
      End Property

    End Class

    Private _children As Dictionary(Of String, ChildData) = New Dictionary(Of String, ChildData)

    ''' <summary>
    ''' Dictionary containing child reference data.
    ''' </summary>
    <DataMember()> _
    Public Property Children() As Dictionary(Of String, ChildData)
      Get
        Return _children
      End Get
      Set(ByVal value As Dictionary(Of String, ChildData))
        _children = value
      End Set
    End Property

    Private _values As Dictionary(Of String, FieldData) = New Dictionary(Of String, FieldData)

    ''' <summary>
    ''' Dictionary containg field data.
    ''' </summary>
    <DataMember()> _
    Public Property Values() As Dictionary(Of String, FieldData)
      Get
        Return _values
      End Get
      Set(ByVal value As Dictionary(Of String, FieldData))
        _values = value
      End Set
    End Property

    Friend Sub New(ByVal referenceId As Integer)
      Me.ReferenceId = referenceId
    End Sub

    Private _referenceId As Integer
    ''' <summary>
    ''' Reference number for this object.
    ''' </summary>
    <DataMember()> _
    Public Property ReferenceId() As Integer
      Get
        Return _referenceId
      End Get
      Set(ByVal value As Integer)
        _referenceId = value
      End Set
    End Property

    Private _TypeName As String
    ''' <summary>
    ''' Assembly-qualified type name of the
    ''' object being serialized.
    ''' </summary>
    <DataMember()> _
    Public Property TypeName() As String
      Get
        Return _TypeName
      End Get
      Set(ByVal value As String)
        _TypeName = value
      End Set
    End Property

    ''' <summary>
    ''' Adds a value to the serialization stream.
    ''' </summary>
    ''' <param name="name">Name of the field.</param>
    ''' <param name="value">Value of the field.</param>
    ''' <remarks></remarks>
    Public Sub AddValue(ByVal name As String, ByVal value As Object)
      AddValue(name, value, False)
    End Sub

    ''' <summary>
    ''' Adds a value to the list of fields.
    ''' </summary>
    ''' <param name="name">Name of the field.</param>
    ''' <param name="value">Value of the field.</param>
    ''' <param name="isDirty">Flag indicating whether the value is dirty.</param>
    ''' <remarks></remarks>
    Public Sub AddValue(ByVal name As String, ByVal value As Object, ByVal isDirty As Boolean)
      Dim oFieldData As New FieldData
      oFieldData.Value = value
      oFieldData.IsDirty = isDirty

      _values.Add(name, oFieldData)

    End Sub

    ''' <summary>
    ''' Gets a value from the list of fields.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type to which the value should be coerced.
    ''' </typeparam>
    ''' <param name="name">
    ''' Name of the field.
    ''' </param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValue(Of T)(ByVal name As String) As T
      Dim value = _values(name).Value
      
      If value IsNot Nothing Then
        Return Utilities.CoerceValue(Of T)(value.GetType(), Nothing, value)
      Else
        Return CType(value, T)
      End If
    End Function

    ''' <summary>
    ''' Adds a child to the list of child references.
    ''' </summary>
    ''' <param name="name">
    ''' Name of the field.
    ''' </param>
    ''' <param name="referenceId">
    ''' Reference id for the child object.
    ''' </param>
    Public Sub AddChild(ByVal name As String, ByVal referenceId As Integer)
      AddChild(name, referenceId, False)
    End Sub

    ''' <summary>
    ''' Adds a child to the list of child references.
    ''' </summary>
    ''' <param name="name">
    ''' Name of the field.
    ''' </param>
    ''' <param name="referenceId">
    ''' Reference id for the child object.
    ''' </param>
    ''' <param name="isDirty">
    ''' Flag indicating whether the child is dirty.
    ''' </param>
    Public Sub AddChild(ByVal name As String, ByVal referenceId As Integer, ByVal isDirty As Boolean)
      Dim oChildData As New ChildData
      oChildData.ReferenceId = referenceId
      oChildData.IsDirty = isDirty
      _children.Add(name, oChildData)
    End Sub

  End Class

End Namespace

