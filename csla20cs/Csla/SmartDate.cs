using System;
using Csla.Properties;

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
  public struct SmartDate : IComparable
  {

    private DateTime _date;
    private bool _initialized;
    private bool _emptyIsMax;

    #region Constructors

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(bool emptyIsMin)
    {
      _emptyIsMax = !emptyIsMin;
      _format = null;
      _initialized = false;
      // provide a dummy value to allow real initialization
      _date = DateTime.MinValue;
      if (!_emptyIsMax)
        Date = DateTime.MinValue;
      else
        Date = DateTime.MaxValue;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </remarks>
    /// <param name="Value">The initial value of the object.</param>
    public SmartDate(DateTime value)
    {
      _emptyIsMax = false;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="Value">The initial value of the object.</param>
    /// <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(DateTime value, bool emptyIsMin)
    {
      _emptyIsMax = !emptyIsMin;
      _format = null;
      _initialized = false;
      _date = DateTime.MinValue;
      Date = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <remarks>
    /// The SmartDate created will use the min possible
    /// date to represent an empty date.
    /// </remarks>
    /// <param name="Value">The initial value of the object (as text).</param>
    public SmartDate(string value)
    {
      _emptyIsMax = false;
      _format = null;
      _initialized = true;
      _date = DateTime.MinValue;
      this.Text = value;
    }

    /// <summary>
    /// Creates a new SmartDate object.
    /// </summary>
    /// <param name="Value">The initial value of the object (as text).</param>
    /// <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    public SmartDate(string value, bool emptyIsMin)
    {
      _emptyIsMax = !emptyIsMin;
      _format = null;
      _initialized = true;
      _date = DateTime.MinValue;
      this.Text = value;
    }

    #endregion

    #region Text Support

    private string _format;

    /// <summary>
    /// Gets or sets the format string used to format a date
    /// value when it is returned as text.
    /// </summary>
    /// <remarks>
    /// The format string should follow the requirements for the
    /// .NET String.Format() statement.
    /// </remarks>
    /// <value>A format string.</value>
    public string FormatString
    {
      get
      {
        if (_format == null)
          _format = "d";
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
    /// <see cref="P:Csla.SmartDate.FormatString" /> property. The default is the
    /// "Short Date" format.
    /// </para>
    /// </remarks>
    /// <returns></returns>
    public string Text
    {
      get { return DateToString(this.Date, FormatString, !_emptyIsMax); }
      set { this.Date = StringToDate(value, !_emptyIsMax); }
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
          _date = DateTime.MinValue;
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
    /// Returns True if the object is equal to this SmartDate.
    /// </summary>
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

    /// <summary>
    /// Gets a database-friendly version of the date value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the SmartDate contains an empty date, this returns DBNull. Otherwise
    /// the actual date value is returned as type Date.
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

    #endregion

    #region Empty Dates

    /// <summary>
    /// Indicates whether this object contains an empty date.
    /// </summary>
    /// <returns>True if the date is empty.</returns>
    public bool IsEmpty
    {
      get
      {
        if (!_emptyIsMax)
          return this.Date.Equals(DateTime.MinValue);
        else
          return this.Date.Equals(DateTime.MaxValue);
      }
    }

    /// <summary>
    /// Indicates whether an empty date is the min or max possible date value.
    /// </summary>
    /// <remarks>
    /// Whether an empty date is considered to be the smallest or largest possible
    /// date is only important for comparison operations. This allows you to
    /// compare an empty date with a real date and get a meaningful result.
    /// </remarks>
    /// <returns>True if an empty date is the smallest date, False if it is the largest.</returns>
    public bool EmptyIsMin
    {
      get { return !_emptyIsMax; }
    }

    #endregion

    #region Conversion Functions

    /// <summary>
    /// Converts a text date representation into a Date value.
    /// </summary>
    /// <remarks>
    /// An empty string is assumed to represent an empty date. An empty date
    /// is returned as the MinValue of the Date datatype.
    /// </remarks>
    /// <param name="Value">The text representation of the date.</param>
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
    /// <param name="Value">The text representation of the date.</param>
    /// <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>A Date value.</returns>
    public static DateTime StringToDate(string value, bool emptyIsMin)
    {
      DateTime tmp;
      if (String.IsNullOrEmpty(value))
      {
        if (emptyIsMin)
          return DateTime.MinValue;
        else
          return DateTime.MaxValue;
      }
      else if (DateTime.TryParse(value, out tmp))
        return tmp; //DateTime.Parse(value);
      else
      {
        string ldate = value.Trim().ToLower();
        if (ldate == Resources.SmartDateT ||
            ldate == Resources.SmartDateToday ||
            ldate == ".")
          return DateTime.Now;
        if (ldate == Resources.SmartDateY ||
            ldate == Resources.SmartDateYesterday ||
            ldate == "-")
          return DateTime.Now.AddDays(-1);
        if (ldate == Resources.SmartDateTom ||
            ldate == Resources.SmartDateTomorrow ||
            ldate == "+")
          return DateTime.Now.AddDays(1);
        throw new ArgumentException(Resources.StringToDateException);
      }
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
    /// <param name="Value">The date value to convert.</param>
    /// <param name="FormatString">The format string used to format the date into text.</param>
    /// <returns>Text representation of the date value.</returns>
    public static string DateToString(DateTime value, string formatString)
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
    /// <param name="Value">The date value to convert.</param>
    /// <param name="FormatString">The format string used to format the date into text.</param>
    /// <param name="EmptyIsMin">Indicates whether an empty date is the min or max date value.</param>
    /// <returns>Text representation of the date value.</returns>
    public static string DateToString(DateTime value, string formatString, bool emptyIsMin)
    {
      if (emptyIsMin && value == DateTime.MinValue)
        return string.Empty;
      else if (!emptyIsMin && value == DateTime.MaxValue)
        return string.Empty;
      else
        return string.Format("{0:" + formatString + "}", value);
    }

    #endregion

    #region Manipulation Functions

    /// <summary>
    /// Compares one SmartDate to another.
    /// </summary>
    /// <remarks>
    /// This method works the same as the CompareTo method
    /// on the Date datetype, with the exception that it
    /// understands the concept of empty date values.
    /// </remarks>
    /// <param name="Value">The date to which we are being compared.</param>
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
    /// This method works the same as the CompareTo method
    /// on the Date datetype, with the exception that it
    /// understands the concept of empty date values.
    /// </remarks>
    /// <param name="obj">The date to which we are being compared.</param>
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
    /// <param name="Value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    public int CompareTo(string value)
    {
      return this.Date.CompareTo(StringToDate(value, !_emptyIsMax));
    }

    /// <summary>
    /// Compares a SmartDate to a date value.
    /// </summary>
    /// <param name="Value">The date to which we are being compared.</param>
    /// <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
    public int CompareTo(DateTime value)
    {
      return this.Date.CompareTo(value);
    }

    /// <summary>
    /// Adds a TimeSpan onto the object.
    /// </summary>
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
    public DateTime Subtract(TimeSpan value)
    {
      if (IsEmpty)
        return this.Date;
      else
        return this.Date.Subtract(value);
    }

    /// <summary>
    /// Subtracts a DateTime from the object.
    /// </summary>
    public TimeSpan Subtract(DateTime value)
    {
      if (IsEmpty)
        return TimeSpan.Zero;
      else
        return this.Date.Subtract(value);
    }

    #endregion

    #region Operators

    public static bool operator ==(SmartDate obj1, SmartDate obj2)
    {
      return obj1.Equals(obj2);
    }

    public static bool operator !=(SmartDate obj1, SmartDate obj2)
    {
      return !obj1.Equals(obj2);
    }

    public static bool operator ==(SmartDate obj1, DateTime obj2)
    {
      return obj1.Equals(obj2);
    }

    public static bool operator !=(SmartDate obj1, DateTime obj2)
    {
      return !obj1.Equals(obj2);
    }

    public static bool operator ==(SmartDate obj1, string obj2)
    {
      return obj1.Equals(obj2);
    }

    public static bool operator !=(SmartDate obj1, string obj2)
    {
      return !obj1.Equals(obj2);
    }

    public static SmartDate operator +(SmartDate start, TimeSpan span)
    {
      return new SmartDate(start.Add(span), start.EmptyIsMin);
    }

    public static SmartDate operator -(SmartDate start, TimeSpan span)
    {
      return new SmartDate(start.Subtract(span), start.EmptyIsMin);
    }

    public static TimeSpan operator -(SmartDate start, SmartDate finish)
    {
      return start.Subtract(finish.Date);
    }

    public static bool operator >(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    public static bool operator <(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    public static bool operator >(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    public static bool operator <(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    public static bool operator >(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) > 0;
    }

    public static bool operator <(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    public static bool operator >=(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    public static bool operator <=(SmartDate obj1, SmartDate obj2)
    {
      return obj1.CompareTo(obj2) <= 0;
    }

    public static bool operator >=(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    public static bool operator <=(SmartDate obj1, DateTime obj2)
    {
      return obj1.CompareTo(obj2) <= 0;
    }

    public static bool operator >=(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) >= 0;
    }

    public static bool operator <=(SmartDate obj1, string obj2)
    {
      return obj1.CompareTo(obj2) < 0;
    }

    #endregion

  }
}