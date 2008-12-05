Namespace Validation

  ''' <summary>
  ''' Parses a rule:// URI to provide
  ''' easy access to the parts of the URI.
  ''' </summary>
  Public Class RuleDescription

    Private _scheme As String
    Private _typeName As String
    Private _methodName As String
    Private _propertyName As String
    Private _arguments As Dictionary(Of String, String)

    ''' <summary>
    ''' Creates an instance of the object
    ''' by parsing the provided rule:// URI.
    ''' </summary>
    ''' <param name="ruleString">The rule:// URI.</param>
    Public Sub New(ByVal ruleString As String)

      Dim uri As Uri = New Uri(ruleString)

      _scheme = uri.Scheme + uri.SchemeDelimiter
      _typeName = uri.UnescapeDataString(uri.Host)

      Dim parts = uri.LocalPath.Split("/"c)
      _methodName = parts(1)
      _propertyName = parts(2)

      Dim args As String = uri.Query
      If (Not String.IsNullOrEmpty(args)) Then
        If args.StartsWith("?") Then
          args = args.Remove(0, 1)
        End If
        _arguments = New Dictionary(Of String, String)()
        Dim argArray As String() = args.Split("&"c)
        For Each arg As String In argArray
          Dim argParams As String() = arg.Split("="c)
          _arguments.Add( _
            System.Uri.UnescapeDataString(argParams(0)), _
            System.Uri.UnescapeDataString(argParams(1)))
        Next arg
      End If

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
        Return _scheme
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the type containing
    ''' the rule method.
    ''' </summary>
    Public ReadOnly Property MethodTypeName() As String
      Get
        Return _methodName
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the rule method.
    ''' </summary>
    Public ReadOnly Property MethodName() As String
      Get
        Return _methodName
      End Get
    End Property

    ''' <summary>
    ''' Gets the name of the property with which
    ''' the rule is associated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return _propertyName
      End Get
    End Property

    ''' <summary>
    ''' Gets a Dictionary containing the
    ''' name/value arguments provided to
    ''' the rule method.
    ''' </summary>
    Public ReadOnly Property Arguments() As Dictionary(Of String, String)
      Get
        Return _arguments
      End Get
    End Property

  End Class

End Namespace