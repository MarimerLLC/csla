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
        Dim child As ITrackStatus = TryCast(mData, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDeleted

        Else
          Return False
        End If
      End Get
    End Property

    Public ReadOnly Property IsDirty() As Boolean Implements ITrackStatus.IsDirty
      Get
        Dim child As ITrackStatus = TryCast(mData, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsDirty

        Else
          Return mIsDirty
        End If
      End Get
    End Property

    Public Sub MarkClean() Implements IFieldData.MarkClean

      mIsDirty = False

    End Sub

    Private ReadOnly Property IsNew() As Boolean Implements ITrackStatus.IsNew
      Get
        Dim child As ITrackStatus = TryCast(mData, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsNew

        Else
          Return False
        End If
      End Get
    End Property

    Private ReadOnly Property IsValid() As Boolean Implements ITrackStatus.IsValid
      Get
        Dim child As ITrackStatus = TryCast(mData, ITrackStatus)
        If child IsNot Nothing Then
          Return child.IsValid

        Else
          Return True
        End If
      End Get
    End Property

  End Class

End Namespace