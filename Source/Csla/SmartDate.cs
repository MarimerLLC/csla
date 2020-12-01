//-----------------------------------------------------------------------
// <copyright file="SmartDate.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Provides a date data type that understands the concept</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Properties;
using Csla.Serialization.Mobile;

namespace Csla
{

  /// <summary>
  /// Provides a date data type that understands the concept
  /// of an empty date value.
  /// </summary>
  /// <remarks>
  /// See Chapter 5 for a full discussion of the need for this
  /// data type and the design choices behind it.
  /// </remarks>
  [Serializable()]
#if !NETFX_CORE
  [System.ComponentModel.TypeConverter(typeof(Csla.Core.TypeConverters.SmartDateConverter))]
#endif
  public struct SmartDate : Csla.Core.ISmartField,
#if !NETFX_CORE
    IConvertible, 
#endif
    IComparable, IFormattable, IMobileObject
  {
    private DateTime _date;
    private bool _initialized;
    private EmptyValue _emptyValue;
    private string _format;
    private static string _defaultFormat;

#if !NETFX_CORE
    [NonSerialized]
#endif
    [NotUndoable]
    private static Func<string, DateTime?> _customParser;
    
#region EmptyValue enum

    /// <summary>
    /// Indicates the empty value of a
    /// SmartDate.
    /// </summary>
    public enum EmptyValue
    {
      /// <summary>
      /// Indicates that an empty SmartDate
      /// is the smallest date.
      /// </summary>
      MinDate,
      /// <summary>
      /// Indicates that an empty SmartDate
      /// is the largest date.
      /// </summary>
      MaxDate
    }

#endregion

#region Constructors

    static SmartDate()
    {
      _defaultFormat = "d";
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(bool emptyIsMin)
    {
      _emptyValue = GetEmptyValue(emptyIsMin);
      _format = null;
      _initialized = false;
      // provide a dummy value to allow real initialization
      _date = DateTime.MinValue;
      SetEmptyDate(_emptyValue);
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(EmptyValue emptyValue)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = false;
      // provide a dummy value to allow real initialization
      _date = DateTime.MinValue;
      SetEmptyDate(_emptyValue);
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </remarks>
    /// <param name="value">The initial value of the object.</param>
    public SmartDate(DateTime value)
    {
      _emptyValue = Csla.SmartDate.EmptyValue.MinDate;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(DateTime value, bool emptyIsMin)
    {
      _emptyValue = GetEmptyValue(emptyIsMin);
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(DateTime value, EmptyValue emptyValue)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <param name="kind">One of the DateTimeKind values.</param>
    public SmartDate(DateTime value, EmptyValue emptyValue, DateTimeKind kind)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = DateTime.SpecifyKind(value, kind);
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </remarks>
    /// <param name="value">The initial value of the object.</param>
    public SmartDate(DateTime? value)
    {
      _emptyValue = Csla.SmartDate.EmptyValue.MinDate;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      if (value.HasValue)
        Date = value.Value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(DateTime? value, bool emptyIsMin)
    {
      _emptyValue = GetEmptyValue(emptyIsMin);
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      if (value.HasValue)
        Date = value.Value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(DateTime? value, EmptyValue emptyValue)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      if (value.HasValue)
        Date = value.Value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </para><para>
    /// SmartDate maintains the date value as a DateTime,
    /// so the provided DateTimeOffset is converted to a
    /// DateTime in this constructor. You should be aware
    /// that this can lead to a loss of precision in
    /// some cases.
    /// </para>
    /// </remarks>
    /// <param name="value">The initial value of the object.</param>
    public SmartDate(DateTimeOffset value)
    {
      _emptyValue = Csla.SmartDate.EmptyValue.MinDate;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value.DateTime;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <remarks>
    /// SmartDate maintains the date value as a DateTime,
    /// so the provided DateTimeOffset is converted to a
    /// DateTime in this constructor. You should be aware
    /// that this can lead to a loss of precision in
    /// some cases.
    /// </remarks>
    public SmartDate(DateTimeOffset value, bool emptyIsMin)
    {
      _emptyValue = GetEmptyValue(emptyIsMin);
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value.DateTime;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <remarks>
    /// SmartDate maintains the date value as a DateTime,
    /// so the provided DateTimeOffset is converted to a
    /// DateTime in this constructor. You should be aware
    /// that this can lead to a loss of precision in
    /// some cases.
    /// </remarks>
    public SmartDate(DateTimeOffset value, EmptyValue emptyValue)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value.DateTime;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </remarks>
    /// <param name="value">The initial value of the object (as text).</param>
    public SmartDate(string value)
    {
      _emptyValue = EmptyValue.MinDate;
      _format = null;
      _initialized = true;
      _date = DateTime.MinValue;
      this.Text = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object (as text).</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(string value, bool emptyIsMin)
    {
      _emptyValue = GetEmptyValue(emptyIsMin);
      _format = null;
      _initialized = true;
      _date = DateTime.MinValue;
      this.Text = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="value">The initial value of the object (as text).</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(string value, EmptyValue emptyValue)
    {
      _emptyValue = emptyValue;
      _format = null;
      _initialized = true;
      _date = DateTime.MinValue;
      this.Text = value;
    }

    private static EmptyValue GetEmptyValue(bool emptyIsMin)
    {
      if (emptyIsMin)
        return EmptyValue.MinDate;
      else
        return EmptyValue.MaxDate;
    }

    private void SetEmptyDate(EmptyValue emptyValue)
    {
      if (emptyValue == SmartDate.EmptyValue.MinDate)
        this.Date = DateTime.MinValue;
      else
        this.Date = DateTime.MaxValue;
    }

#endregion

#region Text Support

    /// <summary>
    /// Sets the global default format string used by all new
    /// SmartDate values going forward.
    /// </summary>
    /// <remarks>
    /// The default global format string is "d" unless this
    /// method is called to change that value. Existing SmartDate
    /// values are unaffected by this method, only SmartDate
    /// values created after calling this method are affected.
    /// </remarks>
    /// <param name="formatString">
    /// The format string should follow the requirements for the
    /// .NET System.String.Format statement.
    /// </param>
    public static void SetDefaultFormatString(string formatString)
    {
      _defaultFormat = formatString;
    }

    /// <summary>
    /// Gets or sets the format string used to format a date
    /// value when it is returned as text.
    /// </summary>
    /// <remarks>
    /// The format string should follow the requirements for the
    /// .NET System.String.Format statement.
    /// </remarks>
    /// <value>A format string.</value>
    public string FormatString
    {
      get
      {
        if (_format == null)
          _format = _defaultFormat;
        return _format;
      }
      set
      {
        _format = value;
      }
    }

    /// <summary>
    /// Gets or sets the date value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property can be used to set the date value by passing a
    /// text representation of the date. Any text date representation
    /// that can be parsed by the .NET runtime is valid.
    /// </para><para>
    /// When the date value is retrieved via this property, the text
    /// is formatted by using the format specified by the 
    /// <see cref="FormatString" /> property. The default is the
    /// short date format (d).
    /// </para>
    /// </remarks>
    public string Text
    {
      get { return DateToString(this.Date, FormatString, _emptyValue); }
      set { this.Date = StringToDate(value, _emptyValue); }
    }

#endregion

#region Date Support

    /// <summary>
    /// Gets or sets the date value.
    /// </summary>
    public DateTime Date
    {
      get
      {
        if (!_initialized)
        {
          _date = _emptyValue == SmartDate.EmptyValue.MinDate ? DateTime.MinValue : DateTime.MaxValue;
          _initialized = true;
        }
        return _date;
      }
      set
      {
        _date = value;
        _initialized = true;
      }
    }

    /// <summary>
    /// Gets the value as a DateTimeOffset.
    /// </summary>
    public DateTimeOffset ToDateTimeOffset()
    {
      return new DateTimeOffset(this.Date);
    }

    /// <summary>
    /// Gets the value as a DateTime?.
    /// </summary>
    public DateTime? ToNullableDate()
    {
      if (this.IsEmpty)
        return new DateTime?();
      else
        return new DateTime?(this.Date);
    }

#endregion

#region System.Object overrides

    /// <summary>
    /// Returns a text representation of the date value.
    /// </summary>
    public override string ToString()
    {
      return this.Text;
    }

    /// <summary>
    /// Returns a text representation of the date value.
    /// </summary>
    /// <param name="format">
    /// A standard .NET format string.
    /// </param>
    public string ToString(string format)
    {
      if (string.IsNullOrEmpty(format))
        return this.ToString();
      else
        return DateToString(this.Date, format, _emptyValue);
    }

    /// <summary>
    /// Compares this object to another <see cref="SmartDate"/>
    /// for equality.
    /// </summary>
    /// <param name="obj">Object to compare for equality.</param>
    public override bool Equals(object obj)
    {
      if (obj is SmartDate)
      {
        SmartDate tmp = (SmartDate)obj;
        if (this.IsEmpty && tmp.IsEmpty)
          return true;
        else
          return this.Date.Equals(tmp.Date);
      }
      else if (obj is DateTime)
        return this.Date.Equals((DateTime)obj);
      else if (obj is string)
        return (this.CompareTo(obj.ToString()) == 0);
      else
        return false;
    }

    /// <summary>
    /// Returns a hash code for this object.
    /// </summary>
    public override int GetHashCode()
    {
      return this.Date.GetHashCode();
    }

#endregion

#region DBValue

#if !NETFX_CORE
    /// <summary>
    /// Gets a database-friendly version of the date value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the SmartDate contains an empty date, this returns <see cref="DBNull"/>.
    /// Otherwise the actual date value is returned as type Date.
    /// </para><para>
    /// This property is very useful when setting parameter values for
    /// a Command object, since it automatically stores null values into
    /// the database for empty date values.
    /// </para><para>
    /// When you also use the SafeDataReader and its GetSmartDate method,
    /// you can easily read a null value from the database back into a
    /// SmartDate object so it remains considered as an empty date value.
    /// </para>
    /// </remarks>
    public object DBValue
    {
      get
      {
        if (this.IsEmpty)
          return DBNull.Value;
        else
          return this.Date;
      }
    }
#endif

#endregion

#region Empty Dates

    /// <summary>
    /// Gets a value indicating whether this object contains an empty date.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        if (_emptyValue == EmptyValue.MinDate)
          return this.Date.Equals(DateTime.MinValue);
        else
          return this.Date.Equals(DateTime.MaxValue);
      }
    }

    /// <summary>
    /// Gets a value indicating whether an empty date is the 
    /// min or max possible date value.
    /// </summary>
    /// <remarks>
    /// Whether an empty date is considered to be the smallest or largest possible
    /// date is only important for comparison operations. This allows you to
    /// compare an empty date with a real date and get a meaningful result.
    /// </remarks>
    public bool EmptyIsMin
    {
      get { return (_emptyValue == EmptyValue.MinDate); }
    }

#endregion

#region Conversion Functions

    /// <summary>
    /// Gets or sets the custom parser.
    /// 
    /// The CustomParser is called first in TryStringToDate to allow custom parsing.
    /// The parser method must return null if unable to parse and allow SmartDate to try default parsing. 
    /// </summary>
    /// <value>
    /// The custom parser.
    /// </value>
    public static Func<string, DateTime?> CustomParser
    {
      get { return _customParser; }
      set { _customParser = value; }
    }


    /// <summary>
    /// Converts a string value into a SmartDate.
    /// </summary>
    /// <param name="value">String containing the date value.</param>
    /// <returns>A new SmartDate containing the date value.</returns>
    /// <remarks>
    /// EmptyIsMin will default to true.
    /// </remarks>
    public static SmartDate Parse(string value)
    {
      return new SmartDate(value);
    }

    /// <summary>
    /// Converts a string value into a SmartDate.
    /// </summary>
    /// <param name="value">String containing the date value.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>A new SmartDate containing the date value.</returns>
    public static SmartDate Parse(string value, EmptyValue emptyValue)
    {
      return new SmartDate(value, emptyValue);
    }

    /// <summary>
    /// Converts a string value into a SmartDate.
    /// </summary>
    /// <param name="value">String containing the date value.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>A new SmartDate containing the date value.</returns>
    public static SmartDate Parse(string value, bool emptyIsMin)
    {
      return new SmartDate(value, emptyIsMin);
    }

    /// <summary>
    /// Converts a string value into a SmartDate.
    /// </summary>
    /// <param name="value">String containing the date value.</param>
    /// <param name="result">The resulting SmartDate value if the parse was successful.</param>
    /// <returns>A value indicating if the parse was successful.</returns>
    public static bool TryParse(string value, ref SmartDate result)
    {
      return TryParse(value, EmptyValue.MinDate, ref result);
    }

    /// <summary>
    /// Converts a string value into a SmartDate.
    /// </summary>
    /// <param name="value">String containing the date value.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <param name="result">The resulting SmartDate value if the parse was successful.</param>
    /// <returns>A value indicating if the parse was successful.</returns>
    public static bool TryParse(string value, EmptyValue emptyValue, ref SmartDate result)
    {
      System.DateTime dateResult = DateTime.MinValue;
      if (TryStringToDate(value, emptyValue, ref dateResult))
      {
        result = new SmartDate(dateResult, emptyValue);
        return true;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Converts a text date representation into a Date value.
    /// </summary>
    /// <remarks>
    /// An empty string is assumed to represent an empty date. An empty date
    /// is returned as the MinValue of the Date datatype.
    /// </remarks>
    /// <param name="value">The text representation of the date.</param>
    /// <returns>A Date value.</returns>
    public static DateTime StringToDate(string value)
    {
      return StringToDate(value, true);
    }

        /// <summary>
    /// Converts a text date representation into a Date value.
    /// </summary>
    /// <remarks>
    /// An empty string is assumed to represent an empty date. An empty date
    /// is returned as the MinValue or MaxValue of the Date datatype depending
    /// on the EmptyIsMin parameter.
    /// </remarks>
    /// <param name="value">The text representation of the date.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>A Date value.</returns>
    public static DateTime StringToDate(string value, bool emptyIsMin)
    {
      return StringToDate(value, GetEmptyValue(emptyIsMin));
    }

    /// <summary>
    /// Converts a text date representation into a Date value.
    /// </summary>
    /// <remarks>
    /// An empty string is assumed to represent an empty date. An empty date
    /// is returned as the MinValue or MaxValue of the Date datatype depending
    /// on the EmptyIsMin parameter.
    /// </remarks>
    /// <param name="value">The text representation of the date.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>A Date value.</returns>
    public static DateTime StringToDate(string value, EmptyValue emptyValue)
    {
      DateTime result = DateTime.MinValue;
      if (TryStringToDate(value, emptyValue, ref result))
        return result;
      else
        throw new ArgumentException(Resources.StringToDateException);
    }

    private static bool TryStringToDate(string value, EmptyValue emptyValue, ref DateTime result)
    {
      
      DateTime tmp;

      // call custom parser if set...
      if (_customParser != null)
      {
        var tmpValue = _customParser.Invoke(value);
        // i f custom parser returned a value then parsing succeeded
        if (tmpValue.HasValue)
        {
          result = tmpValue.Value;
          return true;
        }
      }

      if (String.IsNullOrEmpty(value))
      {
        result = emptyValue == EmptyValue.MinDate ? DateTime.MinValue : DateTime.MaxValue;
        return true;
      }
      if (DateTime.TryParse(value, out tmp))
      {
        result = tmp;
        return true;
      }
      
      string ldate = value.Trim().ToLower();
      if (ldate == Resources.SmartDateT ||
          ldate == Resources.SmartDateToday ||
          ldate == ".")
      {
        result = DateTime.Now;
        return true;
      }
      if (ldate == Resources.SmartDateY ||
          ldate == Resources.SmartDateYesterday ||
          ldate == "-")
      {
        result = DateTime.Now.AddDays(-1);
        return true;
      }
      if (ldate == Resources.SmartDateTom ||
          ldate == Resources.SmartDateTomorrow ||
          ldate == "+")
      {
        result = DateTime.Now.AddDays(1);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Converts a date value into a text representation.
    /// </summary>
    /// <remarks>
    /// The date is considered empty if it matches the min value for
    /// the Date datatype. If the date is empty, this
    /// method returns an empty string. Otherwise it returns the date
    /// value formatted based on the FormatString parameter.
    /// </remarks>
    /// <param name="value">The date value to convert.</param>
    /// <param name="formatString">The format string used to format the date into text.</param>
    /// <returns>Text representation of the date value.</returns>
    public static string DateToString(
      DateTime value, string formatString)
    {
      return DateToString(value, formatString, true);
    }

    /// <summary>
    /// Converts a date value into a text representation.
    /// </summary>
    /// <remarks>
    /// Whether the date value is considered empty is determined by
    /// the EmptyIsMin parameter value. If the date is empty, this
    /// method returns an empty string. Otherwise it returns the date
    /// value formatted based on the FormatString parameter.
    /// </remarks>
    /// <param name="value">The date value to convert.</param>
    /// <param name="formatString">The format string used to format the date into text.</param>
    /// <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>Text representation of the date value.</returns>
    public static string DateToString(
      DateTime value, string formatString, bool emptyIsMin)
    {
      return DateToString(value, formatString, GetEmptyValue(emptyIsMin));
    }

    /// <summary>
    /// Converts a date value into a text representation.
    /// </summary>
    /// <remarks>
    /// Whether the date value is considered empty is determined by
    /// the EmptyIsMin parameter value. If the date is empty, this
    /// method returns an empty string. Otherwise it returns the date
    /// value formatted based on the FormatString parameter.
    /// </remarks>
    /// <param name="value">The date value to convert.</param>
    /// <param name="formatString">The format string used to format the date into text.</param>
    /// <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>Text representation of the date value.</returns>
    public static string DateToString(
      DateTime value, string formatString, EmptyValue emptyValue)
    {
      if (emptyValue == EmptyValue.MinDate)
      {
        if (value == DateTime.MinValue)
          return string.Empty;
      }
      else
      {
        if (value == DateTime.MaxValue)
          return string.Empty;
      }
      return string.Format("{0:" + formatString + "}", value);
    }

#endregion

#region Manipulation Functions

    /// <summary>
    /// Compares one SmartDate to another.
    /// </summary>
    /// <remarks>
    /// This method works the same as the DateTime.CompareTo method
    /// on the Date datetype, with the exception that it
    /// understands the concept of empty date values.
    /// </remarks>
    /// <param name="value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    public int CompareTo(SmartDate value)
    {
      if (this.IsEmpty && value.IsEmpty)
        return 0;
      else
        return _date.CompareTo(value.Date);
    }

    /// <summary>
    /// Compares one SmartDate to another.
    /// </summary>
    /// <remarks>
    /// This method works the same as the DateTime.CompareTo method
    /// on the Date datetype, with the exception that it
    /// understands the concept of empty date values.
    /// </remarks>
    /// <param name="value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    int IComparable.CompareTo(object value)
    {
      if (value is SmartDate)
        return CompareTo((SmartDate)value);
      else
        throw new ArgumentException(Resources.ValueNotSmartDateException);
    }

    /// <summary>
    /// Compares a SmartDate to a text date value.
    /// </summary>
    /// <param name="value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    public int CompareTo(string value)
    {
      return this.Date.CompareTo(StringToDate(value, _emptyValue));
    }

    /// <summary>
    /// Compares a SmartDate to a date value.
    /// </summary>
    /// <param name="value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    /// <remarks>
    /// SmartDate maintains the date value as a DateTime,
    /// so the provided DateTimeOffset is converted to a
    /// DateTime for this comparison. You should be aware
    /// that this can lead to a loss of precision in
    /// some cases.
    /// </remarks>
    public int CompareTo(DateTimeOffset value)
    {
      return this.Date.CompareTo(value.DateTime);
    }

    /// <summary>
    /// Compares a SmartDate to a date value.
    /// </summary>
    /// <param name="value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    public int CompareTo(DateTime value)
    {
      return this.Date.CompareTo(value);
    }

    /// <summary>
    /// Adds a TimeSpan onto the object.
    /// </summary>
    /// <param name="value">Span to add to the date.</param>
    public DateTime Add(TimeSpan value)
    {
      if (IsEmpty)
        return this.Date;
      else
        return this.Date.Add(value);
    }

    /// <summary>
    /// Subtracts a TimeSpan from the object.
    /// </summary>
    /// <param name="value">Span to subtract from the date.</param>
    public DateTime Subtract(TimeSpan value)
    {
      if (IsEmpty)
        return this.Date;
      else
        return this.Date.Subtract(value);
    }

    /// <summary>
    /// Subtracts a DateTimeOffset from the object.
    /// </summary>
    /// <param name="value">DateTimeOffset to subtract from the date.</param>
    /// <remarks>
    /// SmartDate maintains the date value as a DateTime,
    /// so the provided DateTimeOffset is converted to a
    /// DateTime for this comparison. You should be aware
    /// that this can lead to a loss of precision in
    /// some cases.
    /// </remarks>
    public TimeSpan Subtract(DateTimeOffset value)
    {
      if (IsEmpty)
        return TimeSpan.Zero;
      else
        return this.Date.Subtract(value.DateTime);
    }

    /// <summary>
    /// Subtracts a DateTime from the object.
    /// </summary>
    /// <param name="value">Date to subtract from the date.</param>
    public TimeSpan Subtract(DateTime value)
    {
      if (IsEmpty)
        return TimeSpan.Zero;
      else
        return this.Date.Subtract(value);
    }

#endregion

#region Operators

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator ==(SmartDate obj1, SmartDate obj2)
    {
      return obj1.Equals(obj2);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator !=(SmartDate obj1, SmartDate obj2)
    {
      return !obj1.Equals(obj2);
    }

    /// <summary>
    /// Convert a SmartDate to a String.
    /// </summary>
    /// <param name="obj1">SmartDate value.</param>
    public static implicit operator string(SmartDate obj1)
    {
      return obj1.Text;
    }

    /// <summary>
    /// Convert a SmartDate to a DateTime.
    /// </summary>
    /// <param name="obj1">SmartDate value.</param>
    public static implicit operator System.DateTime(SmartDate obj1)
    {
      return obj1.Date;
    }

    /// <summary>
    /// Convert a SmartDate to a nullable DateTime.
    /// </summary>
    /// <param name="obj1">SmartDate value.</param>
    public static implicit operator System.DateTime?(SmartDate obj1)
    {
      return obj1.ToNullableDate();
    }

    /// <summary>
    /// Convert a SmartDate to a DateTimeOffset.
    /// </summary>
    /// <param name="obj1">SmartDate value.</param>
    public static implicit operator DateTimeOffset(SmartDate obj1)
    {
      return obj1.ToDateTimeOffset();
    }

    /// <summary>
    /// Convert a value to a SmartDate.
    /// </summary>
    /// <param name="dateValue">Value to convert.</param>
    public static explicit operator SmartDate(string dateValue)
    {
      return new SmartDate(dateValue);
    }

    /// <summary>
    /// Convert a value to a SmartDate.
    /// </summary>
    /// <param name="dateValue">Value to convert.</param>
    public static implicit operator SmartDate(System.DateTime dateValue)
    {
      return new SmartDate(dateValue);
    }

    /// <summary>
    /// Convert a value to a SmartDate.
    /// </summary>
    /// <param name="dateValue">Value to convert.</param>
    public static implicit operator SmartDate(System.DateTime? dateValue)
    {
      return new SmartDate(dateValue);
    }

    /// <summary>
    /// Convert a value to a SmartDate.
    /// </summary>
    /// <param name="dateValue">Value to convert.</param>
    public static explicit operator SmartDate(DateTimeOffset dateValue)
    {
      return new SmartDate(dateValue);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator ==(SmartDate obj1, DateTime obj2)
    {
      return obj1.Equals(obj2);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator !=(SmartDate obj1, DateTime obj2)
    {
      return !obj1.Equals(obj2);
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator ==(SmartDate obj1, string obj2)
    {
      return obj1.Equals(obj2);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator !=(SmartDate obj1, string obj2)
    {
      return !obj1.Equals(obj2);
    }

    /// <summary>
    /// Addition operator
    /// </summary>
    /// <param name="start">Original date/time</param>
    /// <param name="span">Span to add</param>
    /// <returns></returns>
    public static SmartDate operator +(SmartDate start, TimeSpan span)
    {
      return new SmartDate(start.Add(span), start.EmptyIsMin);
    }

    /// <summary>
    /// Subtraction operator
    /// </summary>
    /// <param name="start">Original date/time</param>
    /// <param name="span">Span to subtract</param>
    /// <returns></returns>
    public static SmartDate operator -(SmartDate start, TimeSpan span)
    {
      return new SmartDate(start.Subtract(span), start.EmptyIsMin);
    }

    /// <summary>
    /// Subtraction operator
    /// </summary>
    /// <param name="start">Original date/time</param>
    /// <param name="finish">Second date/time</param>
    /// <returns></returns>
    public static TimeSpan operator -(SmartDate start, SmartDate finish)
    {
      return start.Subtract(finish.Date);
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    /// <summary>
    /// Greater than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    /// <summary>
    /// Less than operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    /// <summary>
    /// Greater than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >=(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    /// <summary>
    /// Less than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <=(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) <= 0;
    }

    /// <summary>
    /// Greater than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >=(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    /// <summary>
    /// Less than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <=(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) <= 0;
    }

    /// <summary>
    /// Greater than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator >=(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    /// <summary>
    /// Less than or equals operator
    /// </summary>
    /// <param name="obj1">First object</param>
    /// <param name="obj2">Second object</param>
    /// <returns></returns>
    public static bool operator <=(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) <= 0;
    }

#endregion

#if !NETFX_CORE
#region  IConvertible

    System.TypeCode IConvertible.GetTypeCode()
    {
      return ((IConvertible)_date).GetTypeCode();
    }

    bool IConvertible.ToBoolean(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToBoolean(provider);
    }

    byte IConvertible.ToByte(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToByte(provider);
    }

    char IConvertible.ToChar(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToChar(provider);
    }

    System.DateTime IConvertible.ToDateTime(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToDateTime(provider);
    }

    decimal IConvertible.ToDecimal(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToDecimal(provider);
    }

    double IConvertible.ToDouble(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToDouble(provider);
    }

    short IConvertible.ToInt16(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToInt16(provider);
    }

    int IConvertible.ToInt32(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToInt32(provider);
    }

    long IConvertible.ToInt64(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToInt64(provider);
    }

    sbyte IConvertible.ToSByte(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToSByte(provider);
    }

    float IConvertible.ToSingle(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToSingle(provider);
    }

    string IConvertible.ToString(System.IFormatProvider provider)
    {
      return ((IConvertible)Text).ToString(provider);
    }

    object IConvertible.ToType(System.Type conversionType, System.IFormatProvider provider)
    {
      if (conversionType.Equals(typeof(string)))
        return ((IConvertible)Text).ToType(conversionType, provider);
      else if (conversionType.Equals(typeof(SmartDate)))
        return this;
      else
        return ((IConvertible)_date).ToType(conversionType, provider);
    }

    ushort IConvertible.ToUInt16(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToUInt16(provider);
    }

    uint IConvertible.ToUInt32(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToUInt32(provider);
    }

    ulong IConvertible.ToUInt64(System.IFormatProvider provider)
    {
      return ((IConvertible)_date).ToUInt64(provider);
    }

#endregion

#endif

#region IFormattable Members

    string IFormattable.ToString(string format, IFormatProvider formatProvider)
    {
      return this.ToString(format);
    }

#endregion

#region IMobileObject Members

    void IMobileObject.GetState(SerializationInfo info)
    {
      info.AddValue("SmartDate._date", _date);
      info.AddValue("SmartDate._defaultFormat", _defaultFormat);
      info.AddValue("SmartDate._emptyValue", _emptyValue.ToString());
      info.AddValue("SmartDate._initialized", _initialized);
      info.AddValue("SmartDate._format", _format);
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      _date = info.GetValue<DateTime>("SmartDate._date");
      _defaultFormat = info.GetValue<string>("SmartDate._defaultFormat");
      _emptyValue = (EmptyValue)System.Enum.Parse(typeof(EmptyValue), info.GetValue<string>("SmartDate._emptyValue"), true);
      _format = info.GetValue<string>("SmartDate._format");
      _initialized = info.GetValue<bool>("SmartDate._initialized");
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      //
    }

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      //
    }

#endregion
  }
}