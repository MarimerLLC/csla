Imports System.Resources

Public Class Strings

  Public Shared rm As New ResourceManager("CSLA.Resources.Strings", _
    System.Reflection.Assembly.GetExecutingAssembly())

  Public Shared Function GetResourceString(ByVal name As String) As String

    Return rm.GetString(name)

  End Function

End Class
