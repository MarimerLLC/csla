' this attribute allows us to mark dataportal methods
' as transactional to trigger use of EnterpriseServices
<AttributeUsage(AttributeTargets.Method)> _
Public Class TransactionalAttribute
  Inherits Attribute
End Class
