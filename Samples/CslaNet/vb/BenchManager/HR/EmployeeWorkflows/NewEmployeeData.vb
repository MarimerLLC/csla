Public Class NewEmployeeData

  Private _id As Integer
  Public Property Id() As Integer
    Get
      Return _id
    End Get
    Set(ByVal value As Integer)
      _id = value
    End Set
  End Property

  Private _name As String
  Public Property Name() As String
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property

  Private _hireDate As Date = Today
  Public Property HireDate() As Date
    Get
      Return _hireDate
    End Get
    Set(ByVal value As Date)
      _hireDate = value
    End Set
  End Property

  Private _birthDate As Date
  Public Property BirthDate() As Date
    Get
      Return _birthDate
    End Get
    Set(ByVal value As Date)
      _birthDate = value
    End Set
  End Property

  Private _taxId As String
  Public Property TaxId() As String
    Get
      Return _taxId
    End Get
    Set(ByVal value As String)
      _taxId = value
    End Set
  End Property


End Class
