Public Class DataBase

  Public Shared ReadOnly Property DbConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

  Public Shared ReadOnly Property SecurityConn() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("Security").ConnectionString
    End Get
  End Property

End Class
