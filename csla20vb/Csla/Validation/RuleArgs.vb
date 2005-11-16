Namespace Validation

  ''' <summary>
  ''' Object providing extra information to methods that
  ''' implement business rules.
  ''' </summary>
  Public Class RuleArgs
    Private mPropertyName As String
    Private mDescription As String

    ''' <summary>
    ''' The name of the property to be validated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return mPropertyName
      End Get
    End Property

    ''' <summary>
    ''' Set by the rule handler method to describe the broken
    ''' rule.
    ''' </summary>
    Public Property Description() As String
      Get
        Return mDescription
      End Get
      Set(ByVal Value As String)
        mDescription = Value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    Public Sub New(ByVal propertyName As String)
      mPropertyName = propertyName
    End Sub

    ''' <summary>
    ''' Returns a string representation of the object.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return mPropertyName
    End Function

  End Class

End Namespace
