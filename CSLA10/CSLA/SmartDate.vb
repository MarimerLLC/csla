''' <summary>
''' Provides a date data type that understands the concept
''' of an empty date value.
''' </summary>
''' <remarks>
''' See Chapter 5 for a full discussion of the need for this
''' data type and the design choices behind it.
''' </remarks>
<Serializable()> _
Public NotInheritable Class SmartDate

  Implements IComparable

  Private mDate As Date
  Private mEmptyIsMin As Boolean

#Region " Constructors "

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  Public Sub New()
    mEmptyIsMin = True
    mDate = Date.MinValue
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal EmptyIsMin As Boolean)
    mEmptyIsMin = EmptyIsMin
    If mEmptyIsMin Then
      mDate = Date.MinValue
    Else
      mDate = Date.MaxValue
    End If
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="Value">The initial value of the object.</param>
  Public Sub New(ByVal Value As Date)
    mEmptyIsMin = True
    mDate = Value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="Value">The initial value of the object.</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal Value As Date, ByVal EmptyIsMin As Boolean)
    mEmptyIsMin = EmptyIsMin
    mDate = Value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="Value">The initial value of the object (as text).</param>
  Public Sub New(ByVal Value As String)
    mEmptyIsMin = True
    Me.Text = Value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="Value">The initial value of the object (as text).</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal Value As String, ByVal EmptyIsMin As Boolean)
    mEmptyIsMin = EmptyIsMin
    Me.Text = Value
  End Sub

#End Region

#Region " Text Support "

  Private mFormat As String = "Short date"

  ''' <summary>
  ''' Gets or sets the format string used to format a date
  ''' value when it is returned as text.
  ''' </summary>
  ''' <remarks>
  ''' The format string should follow the requirements for the
  ''' Visual Basic .NET Format() statement.
  ''' </remarks>
  ''' <value>A format string.</value>
  Public Property FormatString() As String
    Get
      Return mFormat
    End Get
    Set(ByVal Value As String)
      mFormat = Value
    End Set
  End Property

  ''' <summary>
  ''' Gets or sets the date value.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' This property can be used to set the date value by passing a
  ''' text representation of the date. Any text date representation
  ''' that can be parsed by the .NET runtime is valid.
  ''' </para><para>
  ''' When the date value is retrieved via this property, the text
  ''' is formatted by using the format specified by the 
  ''' <see cref="P:CSLA.SmartDate.FormatString" /> property. The default is the
  ''' "Short Date" format.
  ''' </para>
  ''' </remarks>
  ''' <returns></returns>
  Public Property Text() As String
    Get
      Return DateToString(mDate, mFormat, mEmptyIsMin)
    End Get
    Set(ByVal Value As String)
      mDate = StringToDate(Value, mEmptyIsMin)
    End Set
  End Property

#End Region

#Region " Date Support "

  ''' <summary>
  ''' Gets or sets the date value.
  ''' </summary>
  Public Property [Date]() As Date
    Get
      Return mDate
    End Get
    Set(ByVal Value As Date)
      mDate = Value
    End Set
  End Property

#End Region

#Region " System.Object overrides "

  ''' <summary>
  ''' Returns a text representation of the date value.
  ''' </summary>
  Public Overrides Function ToString() As String
    Return Me.Text
  End Function

  ''' <summary>
  ''' Returns True if the object is equal to this SmartDate.
  ''' </summary>
  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is SmartDate Then
      Dim tmp As SmartDate = DirectCast(obj, SmartDate)
      If Me.IsEmpty AndAlso tmp.IsEmpty Then
        Return True
      Else
        Return mDate.Equals(tmp.Date)
      End If

    ElseIf TypeOf obj Is Date Then
      Return mDate.Equals(DirectCast(obj, Date))

    ElseIf TypeOf obj Is String Then
      Return Me.CompareTo(CStr(obj)) = 0

    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Returns a hash code for this object.
  ''' </summary>
  Public Overrides Function GetHashCode() As Integer
    Return mDate.GetHashCode
  End Function

#End Region

#Region " DBValue "

  ''' <summary>
  ''' Gets a database-friendly version of the date value.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If the SmartDate contains an empty date, this returns DBNull. Otherwise
  ''' the actual date value is returned as type Date.
  ''' </para><para>
  ''' This property is very useful when setting parameter values for
  ''' a Command object, since it automatically stores null values into
  ''' the database for empty date values.
  ''' </para><para>
  ''' When you also use the SafeDataReader and its GetSmartDate method,
  ''' you can easily read a null value from the database back into a
  ''' SmartDate object so it remains considered as an empty date value.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property DBValue() As Object
    Get
      If Me.IsEmpty Then
        Return DBNull.Value

      Else
        Return mDate
      End If
    End Get
  End Property

#End Region

#Region " Empty Dates "

  ''' <summary>
  ''' Indicates whether this object contains an empty date.
  ''' </summary>
  ''' <returns>True if the date is empty.</returns>
  Public ReadOnly Property IsEmpty() As Boolean
    Get
      If mEmptyIsMin Then
        Return mDate.Equals(Date.MinValue)
      Else
        Return mDate.Equals(Date.MaxValue)
      End If
    End Get
  End Property

  ''' <summary>
  ''' Indicates whether an empty date is the min or max possible date value.
  ''' </summary>
  ''' <remarks>
  ''' Whether an empty date is considered to be the smallest or largest possible
  ''' date is only important for comparison operations. This allows you to
  ''' compare an empty date with a real date and get a meaningful result.
  ''' </remarks>
  ''' <returns>True if an empty date is the smallest date, False if it is the largest.</returns>
  Public ReadOnly Property EmptyIsMin() As Boolean
    Get
      Return mEmptyIsMin
    End Get
  End Property

#End Region

#Region " Conversion Functions "

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue of the Date datatype.
  ''' </remarks>
  ''' <param name="Value">The text representation of the date.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate(ByVal Value As String) As Date
    Return StringToDate(Value, True)
  End Function

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue or MaxValue of the Date datatype depending
  ''' on the EmptyIsMin parameter.
  ''' </remarks>
  ''' <param name="Value">The text representation of the date.</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate(ByVal Value As String, ByVal EmptyIsMin As Boolean) As Date
    If Len(Value) = 0 Then
      If EmptyIsMin Then
        Return Date.MinValue

      Else
        Return Date.MaxValue
      End If

    ElseIf IsDate(Value) Then
      Return CDate(Value)

    Else
      Select Case LCase(Trim(Value))
        Case GetResourceString("SmartDateT"), GetResourceString("SmartDateToday"), "."
          Return Now

        Case GetResourceString("SmartDateY"), GetResourceString("SmartDateYesterday"), "-"
          Return DateAdd(DateInterval.Day, -1, Now)

        Case GetResourceString("SmartDateTom"), GetResourceString("SmartDateTomorrow"), "+"
          Return DateAdd(DateInterval.Day, 1, Now)

        Case Else
          Throw New ArgumentException(GetResourceString("StringToDateException"))
      End Select
    End If
  End Function

  ''' <summary>
  ''' Converts a date value into a text representation.
  ''' </summary>
  ''' <remarks>
  ''' The date is considered empty if it matches the min value for
  ''' the Date datatype. If the date is empty, this
  ''' method returns an empty string. Otherwise it returns the date
  ''' value formatted based on the FormatString parameter.
  ''' </remarks>
  ''' <param name="Value">The date value to convert.</param>
  ''' <param name="FormatString">The format string used to format the date into text.</param>
  ''' <returns>Text representation of the date value.</returns>
  Public Shared Function DateToString(ByVal Value As Date, ByVal FormatString As String) As String
    Return DateToString(Value, FormatString, True)
  End Function

  ''' <summary>
  ''' Converts a date value into a text representation.
  ''' </summary>
  ''' <remarks>
  ''' Whether the date value is considered empty is determined by
  ''' the EmptyIsMin parameter value. If the date is empty, this
  ''' method returns an empty string. Otherwise it returns the date
  ''' value formatted based on the FormatString parameter.
  ''' </remarks>
  ''' <param name="Value">The date value to convert.</param>
  ''' <param name="FormatString">The format string used to format the date into text.</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>Text representation of the date value.</returns>
  Public Shared Function DateToString(ByVal Value As Date, ByVal FormatString As String, ByVal EmptyIsMin As Boolean) As String
    If EmptyIsMin AndAlso Value = Date.MinValue Then
      Return ""
    ElseIf Not EmptyIsMin AndAlso Value = Date.MaxValue Then
      Return ""
    Else
      Return Format(Value, FormatString)
    End If
  End Function

#End Region

#Region " Manipulation Functions "

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the CompareTo method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal Value As SmartDate) As Integer
    If Me.IsEmpty AndAlso Value.IsEmpty Then
      Return 0
    Else
      Return mDate.CompareTo(Value.Date)
    End If
  End Function

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the CompareTo method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal Value As Object) As Integer _
      Implements IComparable.CompareTo

    If TypeOf Value Is SmartDate Then
      Return CompareTo(DirectCast(Value, SmartDate))

    Else
      Throw New ArgumentException(GetResourceString("ValueNotSmartDateException"))
    End If

  End Function

  ''' <summary>
  ''' Compares a SmartDate to a text date value.
  ''' </summary>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal Value As String) As Integer
    If Not IsDate(Value) Then
      Throw New ArgumentException(GetResourceString("InvalidDateException"))

    Else
      Return mDate.CompareTo(CDate(Value))
    End If
  End Function

  ''' <summary>
  ''' Compares a SmartDate to a date value.
  ''' </summary>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal Value As Date) As Integer
    Return mDate.CompareTo(Value)
  End Function


  ''' <summary>
  ''' Adds a TimeSpan onto the object.
  ''' </summary>
  Public Function Add(ByVal Value As TimeSpan) As Date
    If IsEmpty Then
      Return mDate
    Else
      Return mDate.Add(Value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a TimeSpan from the object.
  ''' </summary>
  Public Function Subtract(ByVal Value As TimeSpan) As Date
    If IsEmpty Then
      Return mDate
    Else
      Return mDate.Subtract(Value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a Date from the object.
  ''' </summary>
  Public Function Subtract(ByVal Value As Date) As TimeSpan
    If IsEmpty Then
      Return TimeSpan.Zero
    Else
      Return mDate.Subtract(Value)
    End If
  End Function

#End Region

End Class
