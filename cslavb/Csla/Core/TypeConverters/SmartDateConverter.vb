Imports System.ComponentModel

Namespace Core.TypeConverters

  ''' <summary>
  ''' Converts values to and from a SmartDate.
  ''' </summary>
  Public Class SmartDateConverter
    Inherits TypeConverter

    ''' <summary>
    ''' Determines if a value can be converted
    ''' to a SmartDate.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="sourceType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function CanConvertFrom(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal sourceType As System.Type) As Boolean
      If sourceType Is GetType(String) Then
        Return True
      ElseIf sourceType Is GetType(DateTime) Then
        Return True
      ElseIf sourceType Is GetType(DateTimeOffset) Then
        Return True
      ElseIf sourceType Is GetType(Date?) Then
        Return True
      End If
      Return MyBase.CanConvertFrom(context, sourceType)
    End Function

    ''' <summary>
    ''' Converts values to a SmartDate.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="culture"></param>
    ''' <param name="value"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ConvertFrom( _
      ByVal context As System.ComponentModel.ITypeDescriptorContext, _
      ByVal culture As System.Globalization.CultureInfo, ByVal value As Object) As Object

      If TypeOf value Is String Then
        Return New SmartDate(CStr(value))
      ElseIf TypeOf value Is DateTime Then
        Return New SmartDate(CDate(value))
      ElseIf value Is Nothing Then
        Return New SmartDate
      ElseIf TypeOf value Is Date? Then
        Return New SmartDate(DirectCast(value, Date?))
      ElseIf TypeOf value Is DateTimeOffset Then
        Return New SmartDate(DirectCast(value, DateTimeOffset).DateTime)
      End If
      Return MyBase.ConvertFrom(context, culture, value)

    End Function

    ''' <summary>
    ''' Determines if a SmartDate can be
    ''' convert to a value.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="destinationType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function CanConvertTo(ByVal context As System.ComponentModel.ITypeDescriptorContext, ByVal destinationType As System.Type) As Boolean
      If destinationType Is GetType(String) Then
        Return True
      ElseIf destinationType Is GetType(DateTime) Then
        Return True
      ElseIf destinationType Is GetType(DateTimeOffset) Then
        Return True
      ElseIf destinationType Is GetType(Date?) Then
        Return True
      End If
      Return MyBase.CanConvertTo(context, destinationType)
    End Function

    ''' <summary>
    ''' Converts a SmartDate to a value.
    ''' </summary>
    ''' <param name="context"></param>
    ''' <param name="culture"></param>
    ''' <param name="value"></param>
    ''' <param name="destinationType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ConvertTo( _
      ByVal context As System.ComponentModel.ITypeDescriptorContext, _
      ByVal culture As System.Globalization.CultureInfo, _
      ByVal value As Object, ByVal destinationType As System.Type) As Object

      Dim sd As SmartDate = DirectCast(value, SmartDate)
      If destinationType Is GetType(String) Then
        Return sd.Text
      ElseIf destinationType Is GetType(DateTime) Then
        Return sd.Date
      ElseIf destinationType Is GetType(DateTimeOffset) Then
        Return New DateTimeOffset(sd.Date)
      ElseIf destinationType Is GetType(Date?) Then
        Return New Date?(sd.Date)
      End If
      Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

  End Class

End Namespace
