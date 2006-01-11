Namespace Validation

  ''' <summary>
  ''' Implements common business rules.
  ''' </summary>
  Public Module CommonRules

#Region " StringRequired "

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
    Public Function StringRequired( _
      ByVal target As Object, ByVal e As RuleArgs) As Boolean

      Dim value As String = CStr(CallByName(target, e.PropertyName, CallType.Get))
      If Len(value) = 0 Then
        e.Description = e.PropertyName & " required"
        Return False

      Else
        Return True
      End If

    End Function

#End Region

#Region " StringMaxLength "

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
    Public Function StringMaxLength(ByVal target As Object, _
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

      ''' <summary>
      ''' Returns a string representation of the object.
      ''' </summary>
      Public Overrides Function ToString() As String
        Return MyBase.ToString & "!" & mMaxLength.ToString
      End Function

    End Class

#End Region

#Region " IntegerMaxValue "

    Public Function IntegerMaxValue(ByVal target As Object, ByVal e As RuleArgs) As Boolean
      Dim max As Integer = CType(e, IntegerMaxValueRuleArgs).MaxValue
      Dim value As Integer = CType(CallByName(target, e.PropertyName, CallType.Get), Integer)
      If value > max Then
        e.Description = String.Format("{0} can not exceed {1}", _
          e.PropertyName, max.ToString)
        Return False
      Else
        Return True
      End If
    End Function

    Public Class IntegerMaxValueRuleArgs
      Inherits RuleArgs

      Private mMaxValue As Integer

      Public ReadOnly Property MaxValue() As Integer
        Get
          Return mMaxValue
        End Get
      End Property

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

#Region " MaxValue "

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
        e.Description = String.Format("{0} can not exceed {1}", _
          e.PropertyName, max.ToString)
        Return False

      Else
        Return True
      End If

    End Function

    Public Class MaxValueRuleArgs(Of T)
      Inherits RuleArgs

      Private mMaxValue As T

      Public ReadOnly Property MaxValue() As T
        Get
          Return mMaxValue
        End Get
      End Property

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
        e.Description = String.Format("{0} can not exceed {1}", _
          e.PropertyName, min.ToString)
        Return False

      Else
        Return True
      End If

    End Function

    Public Class MinValueRuleArgs(Of T)
      Inherits RuleArgs

      Private mMinValue As T

      Public ReadOnly Property MinValue() As T
        Get
          Return mMinValue
        End Get
      End Property

      Public Sub New(ByVal propertyName As String, ByVal MinValue As T)
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

  End Module

End Namespace
