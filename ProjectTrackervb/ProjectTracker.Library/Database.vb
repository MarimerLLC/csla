Imports System.Configuration.ConfigurationManager

Public Module Database

  Public ReadOnly Property PTrackerConnection() As String
    Get
      Return ConnectionStrings("PTracker").ConnectionString
    End Get
  End Property

  Public ReadOnly Property SecurityConnection() As String
    Get
      Return ConnectionStrings("Security").ConnectionString
    End Get
  End Property

End Module
