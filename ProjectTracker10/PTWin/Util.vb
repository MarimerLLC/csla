Module Util

  Public Sub BindField(ByVal control As Control, _
      ByVal propertyName As String, ByVal dataSource As Object, _
      ByVal dataMember As String)

    Dim bd As Binding
    Dim index As Integer

    For index = control.DataBindings.Count - 1 To 0 Step -1
      bd = control.DataBindings.Item(index)
      If bd.PropertyName = propertyName Then
        control.DataBindings.Remove(bd)
      End If
    Next
    control.DataBindings.Add(propertyName, dataSource, dataMember)

  End Sub

End Module
