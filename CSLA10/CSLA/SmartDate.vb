<Serializable()> _
Public NotInheritable Class SmartDate
  Private mDate As Date
  Private mEmptyIsMin As Boolean

#Region " Constructors "

  Public Sub New(Optional ByVal EmptyIsMin As Boolean = True)
    mEmptyIsMin = EmptyIsMin
    If mEmptyIsMin Then
      mDate = Date.MinValue
    Else
      mDate = Date.MaxValue
    End If
  End Sub

  Public Sub New(ByVal Value As Date, Optional ByVal EmptyIsMin As Boolean = True)
    mEmptyIsMin = EmptyIsMin
    mDate = Value
  End Sub

  Public Sub New(ByVal Value As String, Optional ByVal EmptyIsMin As Boolean = True)
    mEmptyIsMin = EmptyIsMin
    Me.Text = Value
  End Sub

#End Region

#Region " Text Support "

  Private mFormat As String = "Short date"

  Public Property FormatString() As String
    Get
      Return mFormat
    End Get
    Set(ByVal Value As String)
      mFormat = Value
    End Set
  End Property

  Public Property Text() As String
    Get
      Return DateToString(mDate, mFormat, mEmptyIsMin)
    End Get
    Set(ByVal Value As String)
      mDate = StringToDate(Value, mEmptyIsMin)
    End Set
  End Property

  Public Overrides Function ToString() As String
    Return Me.Text
  End Function

#End Region

#Region " Date Support "

  Public Property [Date]() As Date
    Get
      Return mDate
    End Get
    Set(ByVal Value As Date)
      mDate = Value
    End Set
  End Property

#End Region

#Region " DBValue "

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

  Public ReadOnly Property IsEmpty() As Boolean
    Get
      If mEmptyIsMin Then
        Return mDate.Equals(Date.MinValue)
      Else
        Return mDate.Equals(Date.MaxValue)
      End If
    End Get
  End Property

  Public ReadOnly Property EmptyIsMin() As Boolean
    Get
      Return mEmptyIsMin
    End Get
  End Property

#End Region

#Region " Conversion Functions "

  Public Shared Function StringToDate(ByVal Value As String, Optional ByVal EmptyIsMin As Boolean = True) As Date
    If Len(Value) = 0 Then
      If EmptyIsMin Then
        Return Date.MinValue

      Else
        Return Date.MaxValue
      End If

    Else
      Return CDate(Value)
    End If
  End Function

  Public Shared Function DateToString(ByVal Value As Date, ByVal FormatString As String, Optional ByVal EmptyIsMin As Boolean = True) As String
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

  Public Function CompareTo(ByVal Value As SmartDate) As Integer
    If Me.IsEmpty AndAlso Value.IsEmpty Then
      Return 0
    Else
      Return mDate.CompareTo(Value.Date)
    End If
  End Function

  Public Sub Add(ByVal Value As TimeSpan)
    mDate.Add(Value)
  End Sub

  Public Sub Subtract(ByVal Value As TimeSpan)
    mDate.Subtract(Value)
  End Sub

#End Region

End Class
