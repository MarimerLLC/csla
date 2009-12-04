''' <summary>
''' Implement this interface in a Page
''' to be notified when the currently
''' logged in user changes.
''' </summary>
Public Interface IRefresh
  ''' <summary>
  ''' Called by MainForm when the currently
  ''' logged in user changes.
  ''' </summary>
  Sub Refresh()
End Interface
