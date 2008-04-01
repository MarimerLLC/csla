Namespace Core.FieldManager

  ''' <summary>
  ''' Contains a field value and related metadata.
  ''' </summary>
  ''' <typeparam name="T">Type of field value contained.</typeparam>
  <Serializable()> _
  Public Class FieldData(Of T)
    Implements IFieldData(Of T)

    Private _name As String
    Private _data As T
    Private _isDirty As Boolean

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="name">
    ''' Name of the field.
    ''' </param>
    Public Sub New(ByVal name As String)
      _name = name
    End Sub

    ''' <summary>
    ''' Gets the name of the field.
    ''' </summary>
    Public ReadOnly Property Name() As String Implements IFieldData.Name
      Get
        Return _name
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets the value of the field.
    ''' </summary>
    Public Property Value() As T Implements IFieldData(Of T).Value
      Get
        Return _data
      End Get
      Set(ByVal value As T)
        _data = value
        _isDirty = True
      End Set
    End Property

    Private Property IFieldData_Value() As Object Implements IFieldData.Value
      Get
        Return Me.Value
      End Get
      Set(ByVal value As Object)
        If value Is Nothing Then
          Me.Value = Nothing
        Else
          Me.Value = CType(value, T)
        End If
      End Set
    End Property

    Private ReadOnly Property IsDeleted() As Boolean Implements ITrackStatus.IsDeleted
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDeleted

        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the field
    ''' has been changed.
    ''' </summary>
    Public ReadOnly Property IsDirty() As Boolean Implements ITrackStatus.IsDirty, ITrackStatus.IsSelfDirty
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDirty

        Else
          Return _isDirty
        End If
      End Get
    End Property

    ''' <summary>
    ''' Marks the field as unchanged.
    ''' </summary>
    Public Sub MarkClean() Implements IFieldData.MarkClean

      _isDirty = False

    End Sub

    Private ReadOnly Property IsNew() As Boolean Implements ITrackStatus.IsNew
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsNew

        Else
          Return False
        End If
      End Get
    End Property

    Private ReadOnly Property IsValid() As Boolean Implements ITrackStatus.IsValid, ITrackStatus.IsSelfValid
      Get
        Dim child As ITrackStatus = TryCast(_data, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsValid

        Else
          Return True
        End If
      End Get
    End Property

  End Class

End Namespace