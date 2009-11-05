Imports System.ServiceModel
Imports System.Runtime.Serialization

<ServiceContract()> _
Public Interface ICompanyService

  <OperationContract()> _
  Function GetUser(ByVal userName As String, ByVal password As String) As UserInfo

  <OperationContract()> _
  Function GetCompany(ByVal companyId As Integer) As CompanyInfo

  <OperationContract()> _
  Sub UpdateCompany(ByVal company As CompanyInfo)

  <OperationContract()> _
  Function InsertCompany(ByVal company As CompanyInfo) As Integer

  <OperationContract()> _
  Sub DeleteCompany(ByVal companyId As Integer)

End Interface


<DataContract()> _
Public Class UserInfo
  Private _userName As String = String.Empty
  Private _role As String = String.Empty
  Private _isAuthenticated As Boolean = False

  Private Sub New()
  End Sub

  Public Sub New(ByVal userName As String, ByVal role As String, ByVal isAuthenticated As Boolean)
    _userName = userName
    _role = role
    _isAuthenticated = isAuthenticated
  End Sub

  <DataMember()> _
  Public Property IsAuthenticated() As Boolean
    Get
      Return _isAuthenticated
    End Get
    Set(ByVal value As Boolean)
      _isAuthenticated = value
    End Set
  End Property


  <DataMember()> _
  Public Property UserName() As String
    Get
      Return _userName
    End Get
    Set(ByVal value As String)
      _userName = value
    End Set
  End Property

  <DataMember()> _
  Public Property Role() As String
    Get
      Return _role
    End Get
    Set(ByVal value As String)
      _role = value
    End Set
  End Property
End Class

<DataContract()> _
Public Class CompanyInfo
  Private _companyId As Integer
  Private _companyName As String = String.Empty
  Private _dateAdded As String = String.Empty


  Private Sub New()
  End Sub

  Public Sub New(ByVal companyId As Integer, ByVal companyName As String, ByVal dateAdded As String)
    _companyId = companyId
    _companyName = companyName
    _dateAdded = dateAdded
  End Sub

  <DataMember()> _
  Public Property CompanyId() As Integer
    Get
      Return _companyId
    End Get
    Set(ByVal value As Integer)
      _companyId = value
    End Set
  End Property

  <DataMember()> _
  Public Property CompanyName() As String
    Get
      Return _companyName
    End Get
    Set(ByVal value As String)
      _companyName = value
    End Set
  End Property

  <DataMember()> _
  Public Property DateAdded() As String
    Get
      Return _dateAdded
    End Get
    Set(ByVal value As String)
      _dateAdded = value
    End Set
  End Property

  Public ReadOnly Property DateAddedValue() As Object
    Get
      If String.IsNullOrEmpty(_dateAdded) Then
        Return DBNull.Value
      Else
        Return _dateAdded
      End If
    End Get
  End Property

End Class

