Friend Interface IFilterable
  Function MatchesFilter(ByVal filter As String) As Boolean
  Function MatchesFilter(ByVal filter As FilterProvider, ByVal state As Object) As Boolean
End Interface
