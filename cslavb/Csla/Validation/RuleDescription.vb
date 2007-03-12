Namespace Validation

  ''' <summary>
  ''' Parses a rule:// URI to provide
  ''' easy access to the parts of the URI.
  ''' </summary>
  Public Class RuleDescription

    Private mScheme As String
    Private mMethodName As String
    Private mPropertyName As String
    Private mArguments As Dictionary(Of String, String)

    ''' <summary>
    ''' Creates an instance of the object
    ''' by parsing the provided rule:// URI.
    ''' </summary>
    ''' <param name="ruleString">The rule:// URI.</param>
    Public Sub New(ByVal ruleString As String)

      Dim uri As Uri = New Uri(ruleString)

      mScheme = uri.GetLeftPart(UriPartial.Scheme)
      mMethodName = uri.Host
      mPropertyName = uri.LocalPath.Substring(1)

      Dim args As String = uri.Query
      mArguments = New Dictionary(Of String, String)()
      Dim argArray As String() = args.Split("&"c)
      For Each arg As String In argArray
        Dim argParams As String() = arg.Split("="c)
        mArguments.Add(uri.UnescapeDataString(argParams(0)), uri.UnescapeDataString(argParams(1)))
      Next arg

    End Sub

    ''' <summary>
    ''' Parses a rule:// URI.
    ''' </summary>
    ''' <param name="ruleString">
    ''' Text representation of a rule:// URI.</param>
    ''' <returns>A populated RuleDescription object.</returns>
    Public Shared Function Parse(ByVal ruleString As String) As RuleDescription

      Return New RuleDescription(ruleString)

    End Function

    ''' <summary>
    ''' Gets the scheme of the URI 
    ''' (should always be rule://).
    ''' </summary>
    Public ReadOnly Property Scheme() As String
      Get
        Return mScheme
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the rule method.
    ''' </summary>
    Public ReadOnly Property MethodName() As String
      Get
        Return mMethodName
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the property with which
    ''' the rule is associated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return mPropertyName
      End Get
    End Property

    ''' <summary>
    ''' Gets a Dictionary containing the
    ''' name/value arguments provided to
    ''' the rule method.
    ''' </summary>
    Public ReadOnly Property Arguments() As Dictionary(Of String, String)
      Get
        Return mArguments
      End Get
    End Property

  End Class

End Namespace