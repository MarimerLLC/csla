Imports System.Text.RegularExpressions
Imports System.Reflection

Namespace Validation

  ''' <summary>
  ''' Implements common business rules.
  ''' </summary>
  Public Module CommonRules

#Region " StringRequired "

    ''' <summary>
    ''' Rule ensuring a string value contains one or more
    ''' characters.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate</param>
    ''' <param name="e">Arguments parameter specifying the name of the string
    ''' property to validate</param>
    ''' <returns><see langword="false" /> if the rule is broken</returns>
    ''' <remarks>
    ''' This implementation uses late binding, and will only work
    ''' against string property values.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Function StringRequired( _
      ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim value As String = _
        CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) = 0 Then
        e.Description = _
          String.Format(My.Resources.StringRequiredRule, e.PropertyName)
        Return False

      Else
        Return True
      End If

    End Function

#End Region

#Region " StringMaxLength "

    ''' <summary>
    ''' Rule ensuring a string value doesn't exceed
    ''' a specified length.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate</param>
    ''' <param name="e">Arguments parameter specifying the name of the string
    ''' property to validate</param>
    ''' <returns><see langword="false" /> if the rule is broken</returns>
    ''' <remarks>
    ''' This implementation uses late binding, and will only work
    ''' against string property values.
    ''' </remarks>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
    Public Function StringMaxLength(ByVal target As Object, _
      ByVal e As RuleArgs) As Boolean

      Dim max As Integer = DirectCast(e, MaxLengthRuleArgs).MaxLength
      Dim value As String = _
        CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) > max Then
        e.Description = _
          String.Format(My.Resources.StringMaxLengthRule, e.PropertyName, max)
        Return False
      Else
        Return True
      End If
    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="StringMaxLength" /> rule method.
    ''' </summary>
    Public Class MaxLengthRuleArgs
      Inherits RuleArgs

      Private mMaxLength As Integer

      ''' <summary>
      ''' Get the max length for the string.
      ''' </summary>
      Public ReadOnly Property MaxLength() As Integer
        Get
          Return mMaxLength
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxLength As Integer)
        MyBase.New(propertyName)
        mMaxLength = maxLength
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMaxLength.ToString
      End Function

    End Class

#End Region

#Region " IntegerMaxValue "

    ''' <summary>
    ''' Rule ensuring an integer value doesn't exceed
    ''' a specified value.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate.</param>
    ''' <param name="e">Arguments parameter specifying the name of the
    ''' property to validate.</param>
    ''' <returns><see langword="false"/> if the rule is broken.</returns>
    Public Function IntegerMaxValue(ByVal target As Object, ByVal e As RuleArgs) As Boolean
      Dim max As Integer = CType(e, IntegerMaxValueRuleArgs).MaxValue
      Dim value As Integer = CType(CallByName(target, e.PropertyName, CallType.Get), Integer)
      If value > max Then
        e.Description = String.Format(My.Resources.MaxValueRule, _
          e.PropertyName, max.ToString)
        Return False
      Else
        Return True
      End If
    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="IntegerMaxValue" /> rule method.
    ''' </summary>
    Public Class IntegerMaxValueRuleArgs
      Inherits RuleArgs

      Private mMaxValue As Integer

      ''' <summary>
      ''' Get the max value for the property.
      ''' </summary>
      Public ReadOnly Property MaxValue() As Integer
        Get
          Return mMaxValue
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As Integer)
        MyBase.New(propertyName)
        mMaxValue = maxValue
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMaxValue.ToString
      End Function

    End Class

#End Region

#Region " IntegerMinValue "

    ''' <summary>
    ''' Rule ensuring an integer value doesn't go below
    ''' a specified value.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate.</param>
    ''' <param name="e">Arguments parameter specifying the name of the
    ''' property to validate.</param>
    ''' <returns><see langword="false"/> if the rule is broken.</returns>
    Public Function IntegerMinValue(ByVal target As Object, ByVal e As RuleArgs) As Boolean
      Dim min As Integer = CType(e, IntegerMinValueRuleArgs).MinValue
      Dim value As Integer = CType(CallByName(target, e.PropertyName, CallType.Get), Integer)
      If value < min Then
        e.Description = String.Format(My.Resources.MinValueRule, _
          e.PropertyName, min.ToString)
        Return False
      Else
        Return True
      End If
    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="IntegerMinValue" /> rule method.
    ''' </summary>
    Public Class IntegerMinValueRuleArgs
      Inherits RuleArgs

      Private mMinValue As Integer

      ''' <summary>
      ''' Get the min value for the property.
      ''' </summary>
      Public ReadOnly Property MinValue() As Integer
        Get
          Return mMinValue
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As Integer)
        MyBase.New(propertyName)
        mMinValue = MinValue
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMinValue.ToString
      End Function

    End Class

#End Region

#Region " MaxValue "

    ''' <summary>
    ''' Rule ensuring that a numeric value
    ''' doesn't exceed a specified maximum.
    ''' </summary>
    ''' <typeparam name="T">Type of the property to validate.</typeparam>
    ''' <param name="target">Object containing value to validate.</param>
    ''' <param name="e">Arguments variable specifying the
    ''' name of the property to validate, along with the max
    ''' allowed value.</param>
    Public Function MaxValue(Of T As IComparable)(ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim pi As PropertyInfo = target.GetType.GetProperty(e.PropertyName)
      Dim value As T = DirectCast(pi.GetValue(target, Nothing), T)
      Dim max As T = DirectCast(e, MaxValueRuleArgs(Of T)).MaxValue

      Dim result As Integer = value.CompareTo(max)
      If result = 1 Then
        e.Description = String.Format(My.Resources.MaxValueRule, e.PropertyName, max.ToString)

      Else
        Return True
      End If

    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="MaxValue" /> rule method.
    ''' </summary>
    ''' <typeparam name="T">Type of the property to validate.</typeparam>
    Public Class MaxValueRuleArgs(Of T)
      Inherits RuleArgs

      Private mMaxValue As T = Nothing

      ''' <summary>
      ''' Get the max value for the property.
      ''' </summary>
      Public ReadOnly Property MaxValue() As T
        Get
          Return mMaxValue
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As T)
        MyBase.New(propertyName)
        mMaxValue = maxValue
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMaxValue.ToString
      End Function

    End Class

#End Region

#Region " MinValue "

    ''' <summary>
    ''' Rule ensuring that a numeric value
    ''' doesn't exceed a specified minimum.
    ''' </summary>
    ''' <typeparam name="T">Type of the property to validate.</typeparam>
    ''' <param name="target">Object containing value to validate.</param>
    ''' <param name="e">Arguments variable specifying the
    ''' name of the property to validate, along with the min
    ''' allowed value.</param>
    Public Function MinValue(Of T As IComparable)(ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim pi As PropertyInfo = target.GetType.GetProperty(e.PropertyName)
      Dim value As T = DirectCast(pi.GetValue(target, Nothing), T)
      Dim min As T = DirectCast(e, MinValueRuleArgs(Of T)).MinValue

      Dim result As Integer = value.CompareTo(min)
      If result = -1 Then
        e.Description = String.Format(My.Resources.MinValueRule, e.PropertyName, min.ToString)

      Else
        Return True
      End If

    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="MinValue" /> rule method.
    ''' </summary>
    ''' <typeparam name="T">Type of the property to validate.</typeparam>
    Public Class MinValueRuleArgs(Of T)
      Inherits RuleArgs

      Private mMinValue As T = Nothing

      ''' <summary>
      ''' Get the min value for the property.
      ''' </summary>
      Public ReadOnly Property MinValue() As T
        Get
          Return mMinValue
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As T)
        MyBase.New(propertyName)
        mMinValue = minValue
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMinValue.ToString
      End Function

    End Class

#End Region

#Region " RegEx "

    ''' <summary>
    ''' Rule that checks to make sure a value
    ''' matches a given regex pattern.
    ''' </summary>
    ''' <param name="target">Object containing the data to validate</param>
    ''' <param name="e">RegExRuleArgs parameter specifying the name of the 
    ''' property to validate and the regex pattern.</param>
    ''' <returns>False if the rule is broken</returns>
    ''' <remarks>
    ''' This implementation uses late binding.
    ''' </remarks>
    Public Function RegExMatch(ByVal target As Object, _
      ByVal e As RuleArgs) As Boolean

      Dim rx As Regex = DirectCast(e, RegExRuleArgs).RegEx
      If Not rx.IsMatch(CallByName( _
          target, e.PropertyName, CallType.Get).ToString) Then
        e.Description = _
          String.Format(My.Resources.RegExMatchRule, e.PropertyName)
        Return False
      Else
        Return True
      End If
    End Function

    ''' <summary>
    ''' List of built-in regex patterns.
    ''' </summary>
    Public Enum RegExPatterns
      ''' <summary>
      ''' US Social Security number pattern.
      ''' </summary>
      SSN
      ''' <summary>
      ''' Email address pattern.
      ''' </summary>
      Email
    End Enum

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="RegExMatch" /> rule method.
    ''' </summary>
    Public Class RegExRuleArgs
      Inherits RuleArgs

      Private mRegEx As Regex

      ''' <summary>
      ''' The <see cref="RegEx"/> object used to validate
      ''' the property.
      ''' </summary>
      Public ReadOnly Property RegEx() As Regex
        Get
          Return mRegEx
        End Get
      End Property

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As RegExPatterns)
        MyBase.New(propertyName)
        mRegEx = New Regex(GetPattern(pattern))
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As String)
        MyBase.New(propertyName)
        mRegEx = New Regex(pattern)
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal regEx As Regex)
        MyBase.New(propertyName)
        mRegEx = regEx
      End Sub

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mRegEx.ToString
      End Function

      ''' <summary>
      ''' Returns the specified built-in regex pattern.
      ''' </summary>
      Public Shared Function GetPattern(ByVal pattern As RegExPatterns) As String
        Select Case pattern
          Case RegExPatterns.SSN
            Return "^\d{3}-\d{2}-\d{4}$"

          Case RegExPatterns.Email
            Return "\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b"

          Case Else
            Return ""
        End Select
      End Function

    End Class

#End Region

  End Module

End Namespace
