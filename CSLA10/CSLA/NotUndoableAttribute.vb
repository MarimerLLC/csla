' this attribute allows us to mark fields that
' should not be copied as part of the undo process
<AttributeUsage(AttributeTargets.Field)> _
Public Class NotUndoableAttribute
  Inherits Attribute

End Class
