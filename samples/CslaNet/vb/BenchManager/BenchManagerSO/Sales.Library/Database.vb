Module Database

  Public ReadOnly Property SalesConnectionString() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("Sales").ConnectionString
    End Get
  End Property

End Module
