Imports Csla
Imports Csla.Serialization
Imports System.ComponentModel.DataAnnotations

<Serializable()> _
Public Class CustomerEdit
  Inherits BusinessBase(Of CustomerEdit)

#Region " Business Methods "

  Public Shared IdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(Function(c) c.Id)
  ''' <Summary>
  ''' Gets and sets the Id value.
  ''' </Summary>
  Public Property Id() As Integer
    Get
      Return GetProperty(IdProperty)
    End Get
    Set(ByVal value As Integer)
      SetProperty(IdProperty, value)
    End Set
  End Property

  Public Shared NameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Name)
  ''' <Summary>
  ''' Gets and sets the Name value.
  ''' </Summary>
  <Required(ErrorMessage:="Name required")> _
  Public Property Name() As String
    Get
      Return GetProperty(NameProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(NameProperty, value)
    End Set
  End Property

  Public Shared StatusProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(Function(c) c.Status)
  ''' <Summary>
  ''' Gets and sets the Status value.
  ''' </Summary>
  Public Property Status() As String
    Get
      Return GetProperty(StatusProperty)
    End Get
    Set(ByVal value As String)
      SetProperty(StatusProperty, value)
    End Set
  End Property

#End Region

#Region " Business Rules "

  Protected Overrides Sub AddBusinessRules()
    MyBase.AddBusinessRules()
  End Sub

#End Region

#Region " Silverlight Data Access "
#If SILVERLIGHT Then

  Public Shared Sub BeginNewCustomer( _
    ByVal proxyMode As DataPortal.ProxyModes, _
    ByVal callback As EventHandler(Of DataPortalResult(Of CustomerEdit)))

    Dim dp As New DataPortal(Of CustomerEdit)(proxyMode)
    AddHandler dp.CreateCompleted, callback
    dp.BeginCreate()
  End Sub

  Public Shared Sub BeginGetCustomer( _
    ByVal id As Integer, _
    ByVal callback As EventHandler(Of DataPortalResult(Of CustomerEdit)))

    Dim dp As New DataPortal(Of CustomerEdit)
    AddHandler dp.FetchCompleted, callback
    dp.BeginFetch(New SingleCriteria(Of CustomerEdit, Integer)(id))
  End Sub

  Public Sub New()
    ' required by MobileFormatter
  End Sub

  Public Overrides Sub DataPortal_Create( _
    ByVal handler As Csla.DataPortalClient.LocalProxy(Of CustomerEdit).CompletedHandler)

    Dim bw As New System.ComponentModel.BackgroundWorker
    AddHandler bw.DoWork, AddressOf DoCreate
    AddHandler bw.RunWorkerCompleted, AddressOf DoCreateComplete
    bw.RunWorkerAsync(handler)
  End Sub

  Private Sub DoCreate(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs)
    e.Result = e.Argument
  End Sub

  Private Sub DoCreateComplete(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs)
    Dim handler As Csla.DataPortalClient.LocalProxy(Of CustomerEdit).CompletedHandler
    handler = CType(e.Result, Csla.DataPortalClient.LocalProxy(Of CustomerEdit).CompletedHandler)
    Try
      Using BypassPropertyChecks
        Status = "Created " & ApplicationContext.ExecutionLocation.ToString
      End Using
      MyBase.DataPortal_Create(handler)
    Catch ex As Exception
      handler(Me, ex)
    End Try
  End Sub

#End Region

#Region " .NET Data Access "
#Else
#End Region
#Region " .NET Data Access "

  Public Shared Function GetCustomer(ByVal id As Integer) As CustomerEdit
    Return DataPortal.Fetch(Of CustomerEdit)(New SingleCriteria(Of CustomerEdit, Integer)(id))
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

  Protected Overrides Sub DataPortal_Create()
    LoadProperty(StatusProperty, "Created " & ApplicationContext.ExecutionLocation.ToString)
    MyBase.DataPortal_Create()
  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of CustomerEdit, Integer))
    Using BypassPropertyChecks
      Id = criteria.Value
      Name = "Test " & criteria.Value
      Status = "Retrieved " & ApplicationContext.ExecutionLocation.ToString
    End Using
  End Sub

  Protected Overrides Sub DataPortal_Insert()
    Using BypassPropertyChecks
      Id = 987
      Status = "Inserted " & ApplicationContext.ExecutionLocation.ToString
    End Using
  End Sub

  Protected Overrides Sub DataPortal_Update()
    Using BypassPropertyChecks
      Status = "Updated " & ApplicationContext.ExecutionLocation.ToString
    End Using
  End Sub

  Protected Overrides Sub DataPortal_DeleteSelf()
    Using BypassPropertyChecks
      Status = "Deleted " & ApplicationContext.ExecutionLocation.ToString
    End Using
  End Sub

#End If
#End Region

End Class
