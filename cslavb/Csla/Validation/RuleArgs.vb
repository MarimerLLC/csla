Namespace Validation

  ''' <summary>
  ''' Object providing extra information to methods that
  ''' implement business rules.
  ''' </summary>
  Public Class RuleArgs

    Private _propertyName As String
    Private _propertyFriendlyName As String
    Private _description As String
    Private _severity As RuleSeverity = RuleSeverity.Error
    Private _stopProcessing As Boolean

    ''' <summary>
    ''' The name of the property to be validated.
    ''' </summary>
    Public ReadOnly Property PropertyName() As String
      Get
        Return _propertyName
      End Get
    End Property

    ''' <summary>
    ''' Gets or sets a friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.
    ''' </summary>
    Public Property PropertyFriendlyName() As String
      Get
        Return _propertyFriendlyName
      End Get
      Set(ByVal value As String)
        _propertyFriendlyName = value
      End Set
    End Property

    ''' <summary>
    ''' Set by the rule handler method to describe the broken
    ''' rule.
    ''' </summary>
    ''' <value>A human-readable description of
    ''' the broken rule.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property Description() As String
      Get
        Return _description
      End Get
      Set(ByVal Value As String)
        _description = Value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the severity of the broken rule.
    ''' </summary>
    ''' <value>The severity of the broken rule.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property Severity() As RuleSeverity
      Get
        Return _severity
      End Get
      Set(ByVal value As RuleSeverity)
        _severity = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether this
    ''' broken rule should stop the processing of subsequent
    ''' rules for this property.
    ''' </summary>
    ''' <value><see langword="true" /> if no further
    ''' rules should be process for this property.</value>
    ''' <remarks>
    ''' Setting this property only has an effect if
    ''' the rule method returns <see langword="false" />.
    ''' </remarks>
    Public Property StopProcessing() As Boolean
      Get
        Return _stopProcessing
      End Get
      Set(ByVal value As Boolean)
        _stopProcessing = value
      End Set
    End Property


    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    Public Sub New(ByVal propertyName As String)

      _propertyName = propertyName

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo object for the property.</param>
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo)

      Me.New(propertyInfo.Name)
      _propertyFriendlyName = propertyInfo.FriendlyName

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String)

      Me.New(propertyName)
      _propertyFriendlyName = friendlyName

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> parameter defines only the initial default 
    ''' severity value. If the rule changes this value by setting
    ''' e.Severity, then that new value will become the default for all
    ''' subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, it is recommended that the 
    ''' <b>severity</b> constructor parameter 
    ''' only be used for rule methods that do not explicitly set
    ''' e.Severity.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyName As String, ByVal severity As RuleSeverity)

      Me.New(propertyName)
      _severity = severity

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> parameter defines only the initial default 
    ''' severity value. If the rule changes this value by setting
    ''' e.Severity, then that new value will become the default for all
    ''' subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, it is recommended that the 
    ''' <b>severity</b> constructor parameter 
    ''' only be used for rule methods that do not explicitly set
    ''' e.Severity.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal severity As RuleSeverity)

      Me.New(propertyInfo)
      _severity = severity

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> parameter defines only the initial default 
    ''' severity value. If the rule changes this value by setting
    ''' e.Severity, then that new value will become the default for all
    ''' subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, it is recommended that the 
    ''' <b>severity</b> constructor parameter 
    ''' only be used for rule methods that do not explicitly set
    ''' e.Severity.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal severity As RuleSeverity)

      Me.New(propertyName, friendlyName)
      _severity = severity

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="severity">The default severity for the rule.</param>
    ''' <param name="stopProcessing">
    ''' Initial default value for the StopProcessing property.
    ''' </param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> and <b>stopProcessing</b> parameters 
    ''' define only the initial default values. If the rule 
    ''' changes these values by setting e.Severity or
    ''' e.StopProcessing, then the new values will become 
    ''' the default for all subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, It is recommended that the 
    ''' <b>severity</b> and <b>stopProcessing</b> constructor 
    ''' parameters only be used for rule methods that do 
    ''' not explicitly set e.Severity or e.StopProcessing.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyName As String, _
      ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean)

      Me.New(propertyName, severity)
      _stopProcessing = stopProcessing

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property.</param>
    ''' <param name="severity">The default severity for the rule.</param>
    ''' <param name="stopProcessing">
    ''' Initial default value for the StopProcessing property.
    ''' </param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> and <b>stopProcessing</b> parameters 
    ''' define only the initial default values. If the rule 
    ''' changes these values by setting e.Severity or
    ''' e.StopProcessing, then the new values will become 
    ''' the default for all subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, It is recommended that the 
    ''' <b>severity</b> and <b>stopProcessing</b> constructor 
    ''' parameters only be used for rule methods that do 
    ''' not explicitly set e.Severity or e.StopProcessing.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean)

      Me.New(propertyInfo, severity)
      _stopProcessing = stopProcessing

    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    ''' <param name="severity">The default severity for the rule.</param>
    ''' <param name="stopProcessing">
    ''' Initial default value for the StopProcessing property.
    ''' </param>
    ''' <remarks>
    ''' <para>
    ''' The <b>severity</b> and <b>stopProcessing</b> parameters 
    ''' define only the initial default values. If the rule 
    ''' changes these values by setting e.Severity or
    ''' e.StopProcessing, then the new values will become 
    ''' the default for all subsequent rule invocations.
    ''' </para><para>
    ''' To avoid confusion, It is recommended that the 
    ''' <b>severity</b> and <b>stopProcessing</b> constructor 
    ''' parameters only be used for rule methods that do 
    ''' not explicitly set e.Severity or e.StopProcessing.
    ''' </para>
    ''' </remarks>
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean)

      Me.New(propertyName, friendlyName, severity)
      _stopProcessing = stopProcessing

    End Sub

    ''' <summary>
    ''' Returns a string representation of the object.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return _propertyName
    End Function

    ''' <summary>
    ''' Gets the property name from the RuleArgs
    ''' object, using the friendly name if one
    ''' is defined.
    ''' </summary>
    ''' <param name="e">Object from which to 
    ''' extract the name.</param>
    ''' <returns>
    ''' The friendly property name if it exists,
    ''' otherwise the property name itself.
    ''' </returns>
    Public Shared Function GetPropertyName(ByVal e As RuleArgs) As String

      Dim propName As String

      If String.IsNullOrEmpty(e.PropertyFriendlyName) Then
        propName = e.PropertyName
      Else
        propName = e.PropertyFriendlyName
      End If
      Return propName

    End Function

  End Class

End Namespace
