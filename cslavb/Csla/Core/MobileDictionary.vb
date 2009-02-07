Imports System.Collections.Generic
Imports System.Linq
Imports Csla.Serialization
Imports Csla.Serialization.Mobile

Namespace Core

  ''' <summary>
  ''' Defines a dictionary that can be serialized through
  ''' the MobileFormatter.
  ''' </summary>
  ''' <typeparam name="K">Key value: any primitive or IMobileObject type.</typeparam>
  ''' <typeparam name="V">Value: any primitive or IMobileObject type.</typeparam>
  <Serializable()> _
  Public Class MobileDictionary(Of K, V)
    Inherits Dictionary(Of K, V)
    Implements IMobileObject
    Private _keyIsMobile As Boolean
    Private _valueIsMobile As Boolean

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()
      DetermineTypes()
    End Sub

    ''' <summary>
    ''' Creates an instance of the object based
    ''' on the supplied dictionary, whose elements
    ''' are copied to the new dictionary.
    ''' </summary>
    ''' <param name="capacity">The initial number of elements 
    ''' the dictionary can contain.</param>
    Public Sub New(ByVal capacity As Integer)
      MyBase.New(capacity)
      DetermineTypes()
    End Sub

    ''' <summary>
    ''' Creates an instance of the object based
    ''' on the supplied dictionary, whose elements
    ''' are copied to the new dictionary.
    ''' </summary>
    ''' <param name="comparer">The comparer to use when
    ''' comparing keys.</param>
    Public Sub New(ByVal comparer As IEqualityComparer(Of K))
      MyBase.New(comparer)
      DetermineTypes()
    End Sub

    ''' <summary>
    ''' Creates an instance of the object based
    ''' on the supplied dictionary, whose elements
    ''' are copied to the new dictionary.
    ''' </summary>
    ''' <param name="dict">Source dictionary.</param>
    Public Sub New(ByVal dict As Dictionary(Of K, V))
      MyBase.New(dict)
      DetermineTypes()
    End Sub

    Private Sub DetermineTypes()
      _keyIsMobile = GetType(Csla.Serialization.Mobile.IMobileObject).IsAssignableFrom(GetType(K))
      _valueIsMobile = GetType(Csla.Serialization.Mobile.IMobileObject).IsAssignableFrom(GetType(V))
    End Sub

#Region "IMobileObject Members"

    Private Const _keyPrefix As String = "k"
    Private Const _valuePrefix As String = "v"

    Private Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      info.AddValue("count", Me.Keys.Count)
    End Sub

    Private Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      Dim count As Integer = 0
      For Each key In Me.Keys
        If _keyIsMobile Then
          Dim si As SerializationInfo = formatter.SerializeObject(key)
          info.AddChild(_keyPrefix & count, si.ReferenceId)
        Else
          info.AddValue(_keyPrefix & count, key)
        End If

        If _valueIsMobile Then
          Dim si As SerializationInfo = formatter.SerializeObject(Me(key))
          info.AddChild(_valuePrefix & count, si.ReferenceId)
        Else
          Dim value As V = Me(key)
          info.AddValue(_valuePrefix & count, value)
        End If
        count += 1
      Next key
    End Sub

    Private Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
    End Sub

    Private Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      Dim count As Integer = info.GetValue(Of Integer)("count")

      For index As Integer = 0 To count - 1
        Dim key As K
        If _keyIsMobile Then
          key = CType(formatter.GetObject(info.Children(_keyPrefix & index).ReferenceId), K)
        Else
          key = info.GetValue(Of K)(_keyPrefix & index)
        End If

        Dim value As V
        If _valueIsMobile Then
          value = CType(formatter.GetObject(info.Children(_valuePrefix & index).ReferenceId), V)
        Else
          value = info.GetValue(Of V)(_valuePrefix & index)
        End If

        Add(key, value)
      Next index
    End Sub

#End Region
  End Class

End Namespace