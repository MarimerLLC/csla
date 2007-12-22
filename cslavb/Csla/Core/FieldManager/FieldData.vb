Namespace Core.FieldDataManager

  <Serializable()> _
  Public Class FieldData(Of T)
    Implements IFieldData

    Private mData As T
    Private mIsDirty As Boolean

    Public Property Value() As T
      Get
        Return mData
      End Get
      Set(ByVal value As T)
        mData = value
        mIsDirty = True
      End Set
    End Property

    Private Property IFieldData_Value() As Object Implements IFieldData.Value
      Get
        Return Me.Value
      End Get
      Set(ByVal value As Object)
        Me.Value = CType(value, T)
      End Set
    End Property

    Private ReadOnly Property IsDeleted() As Boolean Implements ITrackStatus.IsDeleted
      Get
        Return False
      End Get
    End Property

    Public ReadOnly Property IsDirty() As Boolean Implements ITrackStatus.IsDirty
      Get

      End Get
    End Property

    Private ReadOnly Property IsNew() As Boolean Implements ITrackStatus.IsNew
      Get
        Return False
      End Get
    End Property

    Private ReadOnly Property IsValid() As Boolean Implements ITrackStatus.IsValid
      Get
        Return True
      End Get
    End Property

  End Class

End Namespace