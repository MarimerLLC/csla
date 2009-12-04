Public Class DataConnection
  Private Sub New()
  End Sub

  Public Shared ReadOnly Property ConnectionString() As String
    Get
      Return System.Configuration.ConfigurationManager.ConnectionStrings("SampleConnectionString").ConnectionString
    End Get
  End Property
End Class
