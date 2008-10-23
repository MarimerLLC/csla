Imports System.Reflection

Namespace Server

  Friend Class DataPortalMethodInfo

    Private _runLocal As Boolean
    Public Property RunLocal() As Boolean
      Get
        Return _runLocal
      End Get
      Private Set(ByVal value As Boolean)
        _runLocal = value
      End Set
    End Property

    Private _transactionalType As TransactionalTypes
    Public Property TransactionalType() As TransactionalTypes
      Get
        Return _transactionalType
      End Get
      Private Set(ByVal value As TransactionalTypes)
        _transactionalType = value
      End Set
    End Property

    Public Sub New(ByVal info As MethodInfo)
      Me.RunLocal = IsRunLocal(info)
      Me.TransactionalType = GetTransactionalType(info)
    End Sub

    Private Shared Function IsRunLocal(ByVal method As MethodInfo) As Boolean
      Return Attribute.IsDefined(method, GetType(RunLocalAttribute), False)
    End Function

    Private Shared Function IsTransactionalMethod(ByVal method As MethodInfo) As Boolean
      Return Attribute.IsDefined(method, GetType(TransactionalAttribute))
    End Function

    Private Shared Function GetTransactionalType(ByVal method As MethodInfo) As TransactionalTypes
      Dim result As TransactionalTypes
      If IsTransactionalMethod(method) Then
        Dim attrib As TransactionalAttribute = CType(Attribute.GetCustomAttribute(method, GetType(TransactionalAttribute)), TransactionalAttribute)
        result = attrib.TransactionType

      Else
        result = TransactionalTypes.Manual
      End If
      Return result
    End Function

  End Class

End Namespace