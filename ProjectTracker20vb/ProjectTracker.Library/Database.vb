Public Module Database

  Public ReadOnly Property PTrackerConnection() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

  Public ReadOnly Property SecurityConnection() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("Security").ConnectionString
    End Get
  End Property

End Module
