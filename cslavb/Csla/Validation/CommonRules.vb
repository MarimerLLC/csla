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
          String.Format(My.Resources.StringRequiredRule, RuleArgs.GetPropertyName(e))
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

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim max As Integer = CInt(args("MaxLength"))
      Dim value As String = _
        CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) > max Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = max.ToString

        Else
          outValue = max.ToString(format)
        End If
        e.Description = _
          String.Format(My.Resources.StringMaxLengthRule, RuleArgs.GetPropertyName(e), outValue)
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
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the max length for the string.
      ''' </summary>
      Public ReadOnly Property MaxLength() As Integer
        Get
          Return CType(Me("MaxLength"), Integer)
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxLength As Integer)
        MyBase.New(propertyName)
        Me("MaxLength") = maxLength
        Me("Format") = String.Empty
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxLength As Integer)
        MyBase.New(propertyInfo)
        Me("MaxLength") = maxLength
        Me("Format") = String.Empty
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxLength As Integer)
        MyBase.New(propertyName, friendlyName)
        Me("MaxLength") = maxLength
        Me("Format") = String.Empty
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      ''' <param name="format">Format string for the max length
      ''' value in the broken rule string.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxLength As Integer, ByVal format As String)
        MyBase.New(propertyName)
        Me("MaxLength") = maxLength
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      ''' <param name="format">Format string for the max length
      ''' value in the broken rule string.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxLength As Integer, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MaxLength") = maxLength
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxLength">Max length of characters allowed.</param>
      ''' <param name="format">Format string for the max length
      ''' value in the broken rule string.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxLength As Integer, ByVal format As String)
        MyBase.New(propertyName, friendlyName)
        Me("MaxLength") = maxLength
        Me("Format") = format
      End Sub

    End Class

#End Region

#Region " StringMinLength "

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
    Public Function StringMinLength(ByVal target As Object, _
      ByVal e As RuleArgs) As Boolean

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim min As Integer = CInt(args("MinLength"))
      Dim value As String = _
        CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) < min Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = min.ToString

        Else
          outValue = min.ToString(format)
        End If
        e.Description = _
          String.Format(My.Resources.StringMinLengthRule, RuleArgs.GetPropertyName(e), outValue)
        Return False

      Else
        Return True
      End If
    End Function

    ''' <summary>
    ''' Custom <see cref="RuleArgs" /> object required by the
    ''' <see cref="StringMinLength" /> rule method.
    ''' </summary>
    Public Class MinLengthRuleArgs
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the Min length for the string.
      ''' </summary>
      Public ReadOnly Property MinLength() As Integer
        Get
          Return CInt(Me("MinLength"))
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      Public Sub New(ByVal propertyName As String, ByVal minLength As Integer)
        MyBase.New(propertyName)
        Me("MinLength") = minLength
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minLength As Integer)
        MyBase.New(propertyInfo)
        Me("MinLength") = minLength
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal minLength As Integer)
        MyBase.New(propertyName, friendlyName)
        Me("MinLength") = minLength
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      ''' <param name="format">Format string for the min length value.</param>
      Public Sub New(ByVal propertyName As String, ByVal minLength As Integer, ByVal format As String)
        MyBase.New(propertyName)
        Me("MinLength") = minLength
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      ''' <param name="format">Format string for the min length value.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minLength As Integer, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MinLength") = minLength
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minLength">Min length of characters allowed.</param>
      ''' <param name="format">Format string for the min length value.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal minLength As Integer, ByVal format As String)
        MyBase.New(propertyName, friendlyName)
        Me("MinLength") = minLength
        Me("Format") = format
      End Sub

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

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim max As Integer = CInt(args("MaxValue"))
      Dim value As Integer = CType(CallByName(target, e.PropertyName, CallType.Get), Integer)
      If value > max Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = max.ToString

        Else
          outValue = max.ToString(format)
        End If
        e.Description = String.Format(My.Resources.MaxValueRule, _
          RuleArgs.GetPropertyName(e), outValue)
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
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the max value for the property.
      ''' </summary>
      Public ReadOnly Property MaxValue() As Integer
        Get
          Return CInt(Me("MaxValue"))
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As Integer)
        MyBase.New(propertyName)
        Me("MaxValue") = maxValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxValue As Integer)
        MyBase.New(propertyInfo)
        Me("MaxValue") = maxValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxValue As Integer)
        MyBase.New(propertyName, friendlyName)
        Me("MaxValue") = maxValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As Integer, ByVal format As String)
        MyBase.New(propertyName)
        Me("MaxValue") = maxValue
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxValue As Integer, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MaxValue") = maxValue
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxValue As Integer, ByVal format As String)
        MyBase.New(propertyName, friendlyName)
        Me("MaxValue") = maxValue
        Me("Format") = format
      End Sub

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

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim min As Integer = CInt(args("MinValue"))
      Dim value As Integer = CType(CallByName(target, e.PropertyName, CallType.Get), Integer)
      If value < min Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = min.ToString

        Else
          outValue = min.ToString(format)
        End If
        e.Description = String.Format(My.Resources.MinValueRule, _
          RuleArgs.GetPropertyName(e), outValue)
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
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the min value for the property.
      ''' </summary>
      Public ReadOnly Property MinValue() As Integer
        Get
          Return CInt(Me("MinValue"))
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As Integer)
        MyBase.New(propertyName)
        Me("MinValue") = minValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minValue As Integer)
        MyBase.New(propertyInfo)
        Me("MinValue") = minValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal minValue As Integer)
        MyBase.New(propertyName, friendlyName)
        Me("MinValue") = minValue
        Me("Format") = ""
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for the min value.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As Integer, ByVal format As String)
        MyBase.New(propertyName)
        Me("MinValue") = minValue
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for the min value.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minValue As Integer, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MinValue") = minValue
        Me("Format") = format
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for the min value.</param>
      Public Sub New(ByVal propertyName As String, friendlyName as string, ByVal minValue As Integer, ByVal format As String)
        MyBase.New(propertyName,friendlyname)
        Me("MinValue") = minValue
        Me("Format") = format
      End Sub

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

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim pi As PropertyInfo = target.GetType.GetProperty(e.PropertyName)
      Dim value As T = DirectCast(pi.GetValue(target, Nothing), T)
      Dim max As T = CType(args("MaxValue"), T)

      Dim result As Integer = value.CompareTo(max)
      If result >= 1 Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = max.ToString

        Else
          outValue = String.Format(String.Format("{{0:{0}}}", format), max)
        End If
        e.Description = String.Format(My.Resources.MaxValueRule, RuleArgs.GetPropertyName(e), outValue)
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
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the max value for the property.
      ''' </summary>
      Public ReadOnly Property MaxValue() As T
        Get
          Return CType(Me("MaxValue"), T)
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As T)
        MyBase.New(propertyName)
        Me("MaxValue") = maxValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxValue As T)
        MyBase.New(propertyInfo)
        Me("MaxValue") = maxValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxValue As T)
        MyBase.New(propertyName, friendlyName)
        Me("MaxValue") = maxValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyName As String, ByVal maxValue As T, ByVal format As String)
        MyBase.New(propertyName)
        Me("MaxValue") = maxValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal maxValue As T, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MaxValue") = maxValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="maxValue">Maximum allowed value for the property.</param>
      ''' <param name="format">Format string for the max value.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal maxValue As T, ByVal format As String)
        MyBase.New(propertyName, friendlyName)
        Me("MaxValue") = maxValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

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

      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim pi As PropertyInfo = target.GetType.GetProperty(e.PropertyName)
      Dim value As T = DirectCast(pi.GetValue(target, Nothing), T)
      Dim min As T = CType(args("MinValue"), T)

      Dim result As Integer = value.CompareTo(min)
      If result <= -1 Then
        Dim format As String = CStr(args("Format"))
        Dim outValue As String
        If String.IsNullOrEmpty(format) Then
          outValue = min.ToString

        Else
          outValue = String.Format(String.Format("{{0:{0}}}", format), min)
        End If
        e.Description = String.Format(My.Resources.MinValueRule, RuleArgs.GetPropertyName(e), min.ToString)

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
      Inherits DecoratedRuleArgs

      ''' <summary>
      ''' Get the min value for the property.
      ''' </summary>
      Public ReadOnly Property MinValue() As T
        Get
          Return CType(Me("MinValue"), T)
        End Get
      End Property

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As T)
        MyBase.New(propertyName)
        Me("MinValue") = minValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minValue As T)
        MyBase.New(propertyInfo)
        Me("MinValue") = minValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal minValue As T)
        MyBase.New(propertyName, friendlyName)
        Me("MinValue") = minValue
        Me("Format") = ""
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for min value.</param>
      Public Sub New(ByVal propertyName As String, ByVal minValue As T, ByVal format As String)
        MyBase.New(propertyName)
        Me("MinValue") = minValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for min value.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal minValue As T, ByVal format As String)
        MyBase.New(propertyInfo)
        Me("MinValue") = minValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

      ''' <summary>
      ''' Create a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="minValue">Minimum allowed value for the property.</param>
      ''' <param name="format">Format string for min value.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal minValue As T, ByVal format As String)
        MyBase.New(propertyName, friendlyName)
        Me("MinValue") = minValue
        Me("Format") = format
        Me("ValueType") = GetType(T).FullName
      End Sub

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

      Dim ruleSatisfied As Boolean
      Dim args As DecoratedRuleArgs = DirectCast(e, DecoratedRuleArgs)
      Dim expression As Regex = DirectCast(args("RegEx"), Regex)
      Dim nullOption As RegExRuleArgs.NullResultOptions = _
        DirectCast(args("NullOption"), RegExRuleArgs.NullResultOptions)

      Dim value As Object = _
        CallByName(target, e.PropertyName, CallType.Get)

      If value Is Nothing AndAlso nullOption = RegExRuleArgs.NullResultOptions.ConvertToEmptyString Then
        value = String.Empty
      End If

      If value Is Nothing Then
        ' if the value is null at this point
        ' then return the pre-defined result value
        ruleSatisfied = (nullOption = RegExRuleArgs.NullResultOptions.ReturnTrue)

      Else
        ' the value is not null, so run the 
        ' regular expression
        ruleSatisfied = expression.IsMatch(value.ToString())
      End If

      If (Not ruleSatisfied) Then
        e.Description = _
          String.Format(My.Resources.RegExMatchRule, RuleArgs.GetPropertyName(e))
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
      Inherits DecoratedRuleArgs

#Region " NullResultOptions "

      ''' <summary>
      ''' List of options for the NullResult
      ''' property.
      ''' </summary>
      Public Enum NullResultOptions
        ''' <summary>
        ''' Indicates that a null value
        ''' should always result in the 
        ''' rule returning false.
        ''' </summary>
        ReturnFalse
        ''' <summary>
        ''' Indicates that a null value
        ''' should always result in the 
        ''' rule returning true.
        ''' </summary>
        ReturnTrue
        ''' <summary>
        ''' Indicates that a null value
        ''' should be converted to an
        ''' empty string before the
        ''' regular expression is
        ''' evaluated.
        ''' </summary>
        ConvertToEmptyString
      End Enum

#End Region

      ''' <summary>
      ''' The <see cref="RegEx"/> object used to validate
      ''' the property.
      ''' </summary>
      Public ReadOnly Property RegEx() As Regex
        Get
          Return DirectCast(Me("RegEx"), Regex)
        End Get
      End Property

      ''' <summary>
      ''' Gets a value indicating whether a null value
      ''' means the rule will return true or false.
      ''' </summary>
      Public ReadOnly Property NullResult() As NullResultOptions
        Get
          Return DirectCast(Me("NullOption"), NullResultOptions)
        End Get
      End Property

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As RegExPatterns)
        MyBase.New(propertyName)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal pattern As RegExPatterns)
        MyBase.New(propertyInfo)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal pattern As RegExPatterns)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As String)
        MyBase.New(propertyName)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal pattern As String)
        MyBase.New(propertyInfo)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal pattern As String)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal regEx As System.Text.RegularExpressions.Regex)
        MyBase.New(propertyName)
        Me("RegEx") = regEx
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal regEx As System.Text.RegularExpressions.Regex)
        MyBase.New(propertyInfo)
        Me("RegEx") = regEx
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal regEx As System.Text.RegularExpressions.Regex)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = regEx
        Me("NullOption") = NullResultOptions.ReturnFalse
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As RegExPatterns, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal pattern As RegExPatterns, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyInfo)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="pattern">Built-in regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal pattern As RegExPatterns, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = New Regex(GetPattern(pattern))
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal pattern As String, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal pattern As String, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyInfo)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="pattern">Custom regex pattern to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal pattern As String, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = New Regex(pattern)
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal regEx As System.Text.RegularExpressions.Regex, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName)
        Me("RegEx") = regEx
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyInfo">PropertyInfo for the property to validate.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal regEx As System.Text.RegularExpressions.Regex, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyInfo)
        Me("RegEx") = regEx
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Creates a new object.
      ''' </summary>
      ''' <param name="propertyName">Name of the property to validate.</param>
      ''' <param name="friendlyName">A friendly name for the property, which
      ''' will be used in place of the property name when
      ''' creating the broken rule description string.</param>
      ''' <param name="regEx"><see cref="RegEx"/> object to use.</param>
      ''' <param name="nullResult">
      ''' Value indicating how a null value should be
      ''' handled by the rule method.
      ''' </param>
      Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal regEx As System.Text.RegularExpressions.Regex, ByVal nullResult As NullResultOptions)
        MyBase.New(propertyName, friendlyName)
        Me("RegEx") = regEx
        Me("NullOption") = nullResult
      End Sub

      ''' <summary>
      ''' Returns the specified built-in regex pattern.
      ''' </summary>
      ''' <param name="pattern">Pattern to return.</param>
      Public Shared Function GetPattern(ByVal pattern As RegExPatterns) As String
        Select Case pattern
          Case RegExPatterns.SSN
            Return "^\d{3}-\d{2}-\d{4}$"

          Case RegExPatterns.Email
            Return "^[A-Za-z0-9._%-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$"

          Case Else
            Return ""
        End Select
      End Function

    End Class

#End Region

  End Module

End Namespace
