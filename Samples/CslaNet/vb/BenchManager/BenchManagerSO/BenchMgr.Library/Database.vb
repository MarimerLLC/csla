Module Database

  Public ReadOnly Property BenchMgrConnectionString() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("BenchMgr").ConnectionString
    End Get
  End Property

End Module
