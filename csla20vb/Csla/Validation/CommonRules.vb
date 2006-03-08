Imports System.Text.RegularExpressions

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
      If Len(CallByName( _
          target, e.PropertyName, CallType.Get).ToString) > max Then
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
    Public Function MaxValue(Of T)(ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim max As Object = CType(e, MaxValueRuleArgs(Of T)).MaxValue
      Dim value As Object = CallByName(target, e.PropertyName, CallType.Get)
      Dim result As Boolean
      Dim pType As Type = GetType(T)
      If pType.IsPrimitive Then
        If pType.Equals(GetType(Integer)) Then
          result = (CInt(value) <= CInt(max))

        ElseIf pType.Equals(GetType(Boolean)) Then
          result = (CBool(value) <= CBool(max))

        ElseIf pType.Equals(GetType(Single)) Then
          result = (CSng(value) <= CSng(max))

        ElseIf pType.Equals(GetType(Double)) Then
          result = (CDbl(value) <= CDbl(max))

        ElseIf pType.Equals(GetType(Byte)) Then
          result = (CByte(value) <= CByte(max))

        ElseIf pType.Equals(GetType(Char)) Then
          result = (CChar(value) <= CChar(max))

        ElseIf pType.Equals(GetType(Short)) Then
          result = (CShort(value) <= CShort(max))

        ElseIf pType.Equals(GetType(Long)) Then
          result = (CLng(value) <= CLng(max))

        ElseIf pType.Equals(GetType(UShort)) Then
          result = (CUShort(value) <= CUShort(max))

        ElseIf pType.Equals(GetType(UInteger)) Then
          result = (CUInt(value) <= CUInt(max))

        ElseIf pType.Equals(GetType(ULong)) Then
          result = (CULng(value) <= CULng(max))

        ElseIf pType.Equals(GetType(SByte)) Then
          result = (CSByte(value) <= CSByte(max))

        Else
          Throw New ArgumentException(My.Resources.PrimitiveTypeRequired)
        End If

      Else  ' not primitive
        Throw New ArgumentException(My.Resources.PrimitiveTypeRequired)
      End If

      If Not result Then
        e.Description = String.Format(My.Resources.MaxValueRule, _
          e.PropertyName, max.ToString)
        Return False

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

      Private mMaxValue As T

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
    Public Function MinValue(Of T)(ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim min As Object = CType(e, MinValueRuleArgs(Of T)).MinValue
      Dim value As Object = CallByName(target, e.PropertyName, CallType.Get)
      Dim result As Boolean
      Dim pType As Type = GetType(T)
      If pType.IsPrimitive Then
        If pType.Equals(GetType(Integer)) Then
          result = (CInt(value) >= CInt(min))

        ElseIf pType.Equals(GetType(Boolean)) Then
          result = (CBool(value) >= CBool(min))

        ElseIf pType.Equals(GetType(Single)) Then
          result = (CSng(value) >= CSng(min))

        ElseIf pType.Equals(GetType(Double)) Then
          result = (CDbl(value) >= CDbl(min))

        ElseIf pType.Equals(GetType(Byte)) Then
          result = (CByte(value) >= CByte(min))

        ElseIf pType.Equals(GetType(Char)) Then
          result = (CChar(value) >= CChar(min))

        ElseIf pType.Equals(GetType(Short)) Then
          result = (CShort(value) >= CShort(min))

        ElseIf pType.Equals(GetType(Long)) Then
          result = (CLng(value) >= CLng(min))

        ElseIf pType.Equals(GetType(UShort)) Then
          result = (CUShort(value) >= CUShort(min))

        ElseIf pType.Equals(GetType(UInteger)) Then
          result = (CUInt(value) >= CUInt(min))

        ElseIf pType.Equals(GetType(ULong)) Then
          result = (CULng(value) >= CULng(min))

        ElseIf pType.Equals(GetType(SByte)) Then
          result = (CSByte(value) >= CSByte(min))

        Else
          Throw New ArgumentException(My.Resources.PrimitiveTypeRequired)
        End If

      Else  ' not primitive
        Throw New ArgumentException(My.Resources.PrimitiveTypeRequired)
      End If

      If Not result Then
        e.Description = String.Format(My.Resources.MinValueRule, _
          e.PropertyName, min.ToString)
        Return False

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

      Private mMinValue As T

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
