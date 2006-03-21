''' <summary>
''' Provides a date data type that understands the concept
''' of an empty date value.
''' </summary>
''' <remarks>
''' See Chapter 5 for a full discussion of the need for this
''' data type and the design choices behind it.
''' </remarks>
<Serializable()> _
Public Structure SmartDate

  Implements IComparable

  Private mDate As Date
  Private mEmptyIsMax As Boolean
  Private mFormat As String
  Private mInitialized As Boolean

#Region " Constructors "

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal emptyIsMin As Boolean)
    mEmptyIsMax = Not emptyIsMin
    If Not mEmptyIsMax Then
      Me.Date = Date.MinValue
    Else
      Me.Date = Date.MaxValue
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
  Public Sub New(ByVal value As Date)
    mEmptyIsMax = False
    Me.Date = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="Value">The initial value of the object.</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As Date, ByVal emptyIsMin As Boolean)
    mEmptyIsMax = Not emptyIsMin
    Me.Date = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="Value">The initial value of the object (as text).</param>
  Public Sub New(ByVal value As String)
    mEmptyIsMax = False
    Me.Text = value
    mInitialized = True
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="Value">The initial value of the object (as text).</param>
  ''' <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As String, ByVal emptyIsMin As Boolean)
    mEmptyIsMax = Not emptyIsMin
    Me.Text = value
    mInitialized = True
  End Sub

#End Region

#Region " Text Support "

  ''' <summary>
  ''' Gets or sets the format string used to format a date
  ''' value when it is returned as text.
  ''' </summary>
  ''' <remarks>
  ''' The format string should follow the requirements for the
  ''' .NET <see cref="System.String.Format"/> statement.
  ''' </remarks>
  ''' <value>A format string.</value>
  Public Property FormatString() As String
    Get
      If mFormat Is Nothing Then
        mFormat = "d"
      End If
      Return mFormat
    End Get
    Set(ByVal value As String)
      mFormat = value
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
  ''' <see cref="FormatString" /> property. The default is the
  ''' short date format (d).
  ''' </para>
  ''' </remarks>
  Public Property Text() As String
    Get
      Return DateToString(Me.Date, FormatString, Not mEmptyIsMax)
    End Get
    Set(ByVal value As String)
      Me.Date = StringToDate(value, Not mEmptyIsMax)
    End Set
  End Property

#End Region

#Region " Date Support "

  ''' <summary>
  ''' Gets or sets the date value.
  ''' </summary>
  Public Property [Date]() As Date
    Get
      If Not mInitialized Then
        mDate = Date.MinValue
        mInitialized = True
      End If
      Return mDate
    End Get
    Set(ByVal value As Date)
      mDate = value
      mInitialized = True
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
  ''' Compares this object to another <see cref="SmartDate"/>
  ''' for equality.
  ''' </summary>
  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is SmartDate Then
      Dim tmp As SmartDate = DirectCast(obj, SmartDate)
      If Me.IsEmpty AndAlso tmp.IsEmpty Then
        Return True
      Else
        Return Me.Date.Equals(tmp.Date)
      End If

    ElseIf TypeOf obj Is Date Then
      Return Me.Date.Equals(DirectCast(obj, Date))

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
    Return Me.Date.GetHashCode
  End Function

#End Region

#Region " DBValue "

  ''' <summary>
  ''' Gets a database-friendly version of the date value.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If the SmartDate contains an empty date, this returns <see cref="DBNull"/>.
  ''' Otherwise the actual date value is returned as type Date.
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
        Return Me.Date
      End If
    End Get
  End Property

#End Region

#Region " Empty Dates "

  ''' <summary>
  ''' Gets a value indicating whether this object contains an empty date.
  ''' </summary>
  Public ReadOnly Property IsEmpty() As Boolean
    Get
      If Not mEmptyIsMax Then
        Return Me.Date.Equals(Date.MinValue)
      Else
        Return Me.Date.Equals(Date.MaxValue)
      End If
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether an empty date is the 
  ''' min or max possible date value.
  ''' </summary>
  ''' <remarks>
  ''' Whether an empty date is considered to be the smallest or largest possible
  ''' date is only important for comparison operations. This allows you to
  ''' compare an empty date with a real date and get a meaningful result.
  ''' </remarks>
  Public ReadOnly Property EmptyIsMin() As Boolean
    Get
      Return Not mEmptyIsMax
    End Get
  End Property

#End Region

#Region " Conversion Functions "

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <returns>A new SmartDate containing the date value.</returns>
  ''' <remarks>
  ''' EmptyIsMin will default to <see langword="true"/>.
  ''' </remarks>
  Public Shared Function Parse(ByVal value As String) As SmartDate

    Return New SmartDate(value)

  End Function

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A new SmartDate containing the date value.</returns>
  Public Shared Function Parse( _
    ByVal value As String, ByVal emptyIsMin As Boolean) As SmartDate

    Return New SmartDate(value, emptyIsMin)

  End Function

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue of the Date datatype.
  ''' </remarks>
  ''' <param name="Value">The text representation of the date.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate(ByVal value As String) As Date
    Return StringToDate(value, True)
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
  Public Shared Function StringToDate( _
    ByVal value As String, ByVal emptyIsMin As Boolean) As Date

    If Len(value) = 0 Then
      If emptyIsMin Then
        Return Date.MinValue

      Else
        Return Date.MaxValue
      End If

    ElseIf IsDate(value) Then
      Return CDate(value)

    Else
      Select Case LCase(Trim(value))
        Case My.Resources.SmartDateT, My.Resources.SmartDateToday, "."
          Return Now

        Case My.Resources.SmartDateY, My.Resources.SmartDateYesterday, "-"
          Return DateAdd(DateInterval.Day, -1, Now)

        Case My.Resources.SmartDateTom, My.Resources.SmartDateTomorrow, "+"
          Return DateAdd(DateInterval.Day, 1, Now)

        Case Else
          Throw New ArgumentException(My.Resources.StringToDateException)
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
  Public Shared Function DateToString( _
    ByVal value As Date, ByVal formatString As String) As String

    Return DateToString(value, formatString, True)
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
  Public Shared Function DateToString( _
    ByVal value As Date, ByVal formatString As String, _
    ByVal emptyIsMin As Boolean) As String

    If emptyIsMin AndAlso value = Date.MinValue Then
      Return ""
    ElseIf Not emptyIsMin AndAlso value = Date.MaxValue Then
      Return ""
    Else
      Return String.Format("{0:" + formatString + "}", value)
    End If
  End Function

#End Region

#Region " Manipulation Functions "

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the <see cref="DateTime.CompareTo"/> method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As SmartDate) As Integer
    If Me.IsEmpty AndAlso value.IsEmpty Then
      Return 0
    Else
      Return Me.Date.CompareTo(value.Date)
    End If
  End Function

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the <see cref="DateTime.CompareTo"/> method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="obj">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal obj As Object) As Integer _
      Implements IComparable.CompareTo

    If TypeOf obj Is SmartDate Then
      Return CompareTo(DirectCast(obj, SmartDate))

    Else
      Throw New ArgumentException(My.Resources.ValueNotSmartDateException)
    End If

  End Function

  ''' <summary>
  ''' Compares a SmartDate to a text date value.
  ''' </summary>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As String) As Integer
    Return Me.Date.CompareTo(StringToDate(value, Not mEmptyIsMax))
  End Function

  ''' <summary>
  ''' Compares a SmartDate to a date value.
  ''' </summary>
  ''' <param name="Value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As Date) As Integer
    Return Me.Date.CompareTo(value)
  End Function


  ''' <summary>
  ''' Adds a TimeSpan onto the object.
  ''' </summary>
  Public Function Add(ByVal value As TimeSpan) As Date
    If IsEmpty Then
      Return Me.Date
    Else
      Return Me.Date.Add(value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a TimeSpan from the object.
  ''' </summary>
  Public Function Subtract(ByVal value As TimeSpan) As Date
    If IsEmpty Then
      Return Me.Date
    Else
      Return Me.Date.Subtract(value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a Date from the object.
  ''' </summary>
  Public Function Subtract(ByVal value As Date) As TimeSpan
    If IsEmpty Then
      Return TimeSpan.Zero
    Else
      Return Me.Date.Subtract(value)
    End If
  End Function

#End Region

#Region " Operators "

  Public Shared Operator =( _
    ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean

    Return obj1.Equals(obj2)
  End Operator

  Public Shared Operator <>( _
    ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean

    Return Not obj1.Equals(obj2)
  End Operator

  Public Shared Operator =(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return obj1.Equals(obj2)
  End Operator

  Public Shared Operator <>(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return Not obj1.Equals(obj2)
  End Operator

  Public Shared Operator =(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.Equals(obj2)
  End Operator

  Public Shared Operator <>(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return Not obj1.Equals(obj2)
  End Operator

  Public Shared Operator +( _
    ByVal start As SmartDate, ByVal span As TimeSpan) As SmartDate

    Return New SmartDate(start.Add(span), start.EmptyIsMin)
  End Operator

  Public Shared Operator -( _
    ByVal start As SmartDate, ByVal span As TimeSpan) As SmartDate

    Return New SmartDate(start.Subtract(span), start.EmptyIsMin)
  End Operator

  Public Shared Operator -( _
    ByVal start As SmartDate, ByVal finish As SmartDate) As TimeSpan

    Return start.Subtract(finish.Date)
  End Operator

  Public Shared Operator >( _
    ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean

    Return obj1.CompareTo(obj2) > 0
  End Operator

  Public Shared Operator <( _
    ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean

    Return obj1.CompareTo(obj2) < 0
  End Operator

  Public Shared Operator >(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return obj1.CompareTo(obj2) > 0
  End Operator

  Public Shared Operator <(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return obj1.CompareTo(obj2) < 0
  End Operator

  Public Shared Operator >(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) > 0
  End Operator

  Public Shared Operator <(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) < 0
  End Operator

  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As Date) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

#End Region

End Structure
