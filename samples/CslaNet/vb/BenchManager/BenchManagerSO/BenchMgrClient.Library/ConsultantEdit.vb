Imports System.Data.SqlClient

<Serializable()> _
Public Class ConsultantEdit
  Inherits BusinessBase(Of ConsultantEdit)

#Region " Business Methods "

  Private _id As Integer
  Public ReadOnly Property Id() As Integer
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _id
    End Get
  End Property

  Private _name As String = ""
  Public Property Name() As String
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _name
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As String)
      CanWriteProperty(True)
      If Not _name.Equals(value) Then
        _name = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Private _onBench As Boolean
  Public Property OnBench() As Boolean
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Get
      CanReadProperty(True)
      Return _onBench
    End Get
    <System.Runtime.CompilerServices.MethodImpl(Runtime.CompilerServices.MethodImplOptions.NoInlining)> _
    Set(ByVal value As Boolean)
      CanWriteProperty(True)
      If Not _onBench.Equals(value) Then
        _onBench = value
        PropertyHasChanged()
      End If
    End Set
  End Property

  Protected Overrides Function GetIdValue() As Object

    Return _id

  End Function

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()

    ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, "Name")

  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewConsultant() As ConsultantEdit

    Return DataPortal.Create(Of ConsultantEdit)()

  End Function

  Friend Shared Function GetConsultant(ByVal item As BenchService.ConsultantData) As ConsultantEdit

    Return New ConsultantEdit(item)

  End Function

  Private Sub New()

    ' require use of factory methods

  End Sub

  Private Sub New(ByVal item As BenchService.ConsultantData)
    Me.New()
    Fetch(item)
  End Sub

#End Region

#Region " Data Access "

  Private Shared _lastId As Integer

  <RunLocal()> _
  Private Overloads Sub DataPortal_Create()

    _id = System.Threading.Interlocked.Decrement(_lastId)
    _onBench = True
    ValidationRules.CheckRules()

  End Sub

  Private Sub Fetch(ByVal item As BenchService.ConsultantData)

    _id = item.Id
    _name = item.Name
    _onBench = item.OnBench
    MarkOld()

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Insert()

    DoUpdate()
    MarkOld()

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_Update()

    DoUpdate()
    MarkOld()

  End Sub

  <RunLocal()> _
  Protected Overrides Sub DataPortal_DeleteSelf()

    DoUpdate()
    MarkNew()

  End Sub

  Private Sub DoUpdate()

    Dim svc As New BenchService.BenchServiceClient
    Dim data As New BenchService.ConsultantUpdateData
    Csla.Data.DataMapper.Map(Me, data)
    data.IsNew = Me.IsNew
    data.Delete = Me.IsDeleted

    Dim newdata As BenchService.ConsultantData = svc.UpdateConsultant(data)

    _id = newdata.Id
    _name = newdata.Name
    _onBench = newdata.OnBench

  End Sub

  Public Overrides Function Save() As ConsultantEdit

    Dim result As ConsultantEdit = MyBase.Save()
    ConsultantNVList.FlushCache()
    Return result

  End Function

#End Region

End Class
