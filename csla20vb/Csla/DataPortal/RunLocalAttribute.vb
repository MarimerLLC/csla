''' <summary>
''' Marks a DataPortal_XYZ method to
''' be run on the client even if the server-side
''' DataPortal is configured for remote use.
''' </summary>
<AttributeUsage(AttributeTargets.Method)> _
Public NotInheritable Class RunLocalAttribute
  Inherits Attribute

End Class
