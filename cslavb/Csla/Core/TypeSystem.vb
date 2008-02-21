Namespace Core

  Friend NotInheritable Class TypeSystem

    Private Sub New()
    End Sub

    Friend Shared Function GetElementType(ByVal seqType As Type) As Type
      Dim ienum As Type = FindIEnumerable(seqType)
      If ienum Is Nothing Then
        Return seqType
      End If
      Return ienum.GetGenericArguments()(0)
    End Function

    Private Shared Function FindIEnumerable(ByVal seqType As Type) As Type
      If seqType Is Nothing OrElse seqType Is GetType(String) Then
        Return Nothing
      End If

      If seqType.IsArray Then
        Return GetType(IEnumerable(Of )).MakeGenericType(seqType.GetElementType())
      End If

      If seqType.IsGenericType Then
        For Each arg As Type In seqType.GetGenericArguments()
          Dim ienum As Type = GetType(IEnumerable(Of )).MakeGenericType(arg)
          If ienum.IsAssignableFrom(seqType) Then
            Return ienum
          End If
        Next arg
      End If

      Dim ifaces() As Type = seqType.GetInterfaces()
      If ifaces IsNot Nothing AndAlso ifaces.Length > 0 Then
        For Each iface As Type In ifaces
          Dim ienum As Type = FindIEnumerable(iface)
          If ienum IsNot Nothing Then
            Return ienum
          End If
        Next iface
      End If

      If seqType.BaseType IsNot Nothing AndAlso seqType.BaseType IsNot GetType(Object) Then
        Return FindIEnumerable(seqType.BaseType)
      End If

      Return Nothing
    End Function

  End Class

End Namespace