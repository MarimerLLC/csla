Namespace Validation

  ''' <summary>
  ''' Implements common business rules.
  ''' </summary>
  Public NotInheritable Class CommonRules

    Private Sub New()

    End Sub

    ''' <summary>
    ''' Rule ensuring a String value contains one or more
    ''' characters.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate</param>
    ''' <param name="e">Arguments parameter specifying the name of the String
    ''' property to validate</param>
    ''' <returns>False if the rule is broken</returns>
    ''' <remarks>
    ''' This implementation uses late binding, and will only work
    ''' against String property values.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Shared Function StringRequired( _
      ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim value As String = CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) = 0 Then
        e.Description = e.PropertyName & " required"
        Return False

      Else
        Return True
      End If

    End Function

    ''' <summary>
    ''' Rule ensuring a String value doesn't exceed
    ''' a specified length.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate</param>
    ''' <param name="e">Arguments parameter specifying the name of the String
    ''' property to validate</param>
    ''' <returns>False if the rule is broken</returns>
    ''' <remarks>
    ''' This implementation uses late binding, and will only work
    ''' against String property values.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Shared Function StringMaxLength(ByVal target As Object, _
      ByVal e As RuleArgs) As Boolean

      Dim max As Integer = DirectCast(e, MaxLengthRuleArgs).MaxLength
      If Len(CallByName(target, e.PropertyName, CallType.Get).ToString) > max Then
        e.Description = e.PropertyName & " can not exceed " & max & " characters"
        Return False
      Else
        Return True
      End If
    End Function

    Public Class MaxLengthRuleArgs
      Inherits RuleArgs

      Private mMaxLength As Integer

      Public ReadOnly Property MaxLength() As Integer
        Get
          Return mMaxLength
        End Get
      End Property

      Public Sub New(ByVal propertyName As String, ByVal maxLength As Integer)
        MyBase.New(propertyName)
        mMaxLength = maxLength
      End Sub

    End Class

  End Class

End Namespace
