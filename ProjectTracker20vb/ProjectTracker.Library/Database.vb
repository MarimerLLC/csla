Public Module DataBase

  Public ReadOnly Property DbConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

  Public ReadOnly Property SecurityConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("Security").ConnectionString
    End Get
  End Property

End Module
