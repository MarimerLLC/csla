Imports System.Text

Namespace Validation

  ''' <summary>
  ''' Object providing extra information to methods that
  ''' implement business rules.
  ''' </summary>
  Public Class DecoratedRuleArgs
    Inherits RuleArgs

    Private _decorations As Dictionary(Of String, Object)

#Region "Base Constructors"

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    Public Sub New(ByVal propertyName As String)
      MyBase.New(propertyName)
      _decorations = New Dictionary(Of String, Object)()
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo)
      MyBase.New(propertyInfo)
      _decorations = New Dictionary(Of String, Object)()
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String)
      MyBase.New(propertyName, friendlyName)
      _decorations = New Dictionary(Of String, Object)()
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
      MyBase.New(propertyName, severity)
      _decorations = New Dictionary(Of String, Object)()
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
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
      MyBase.New(propertyInfo, severity)
      _decorations = New Dictionary(Of String, Object)()
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
      MyBase.New(propertyName, friendlyName, severity)
      _decorations = New Dictionary(Of String, Object)()
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
    Public Sub New(ByVal propertyName As String, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean)
      MyBase.New(propertyName, severity, stopProcessing)
      _decorations = New Dictionary(Of String, Object)()
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
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
      MyBase.New(propertyInfo, severity, stopProcessing)
      _decorations = New Dictionary(Of String, Object)()
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
      MyBase.New(propertyName, friendlyName, severity, stopProcessing)
      _decorations = New Dictionary(Of String, Object)()
    End Sub

#End Region

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
    Public Sub New(ByVal propertyName As String, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyInfo)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName, friendlyName)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyName As String, ByVal severity As RuleSeverity, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName, severity)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal severity As RuleSeverity, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyInfo, severity)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="friendlyName">A friendly name for the property, which
    ''' will be used in place of the property name when
    ''' creating the broken rule description string.</param>
    ''' <param name="severity">Initial default severity for the rule.</param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal severity As RuleSeverity, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName, friendlyName, severity)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyName">The name of the property to be validated.</param>
    ''' <param name="severity">The default severity for the rule.</param>
    ''' <param name="stopProcessing">
    ''' Initial default value for the StopProcessing property.
    ''' </param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyName As String, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName, severity, stopProcessing)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Creates an instance of RuleArgs.
    ''' </summary>
    ''' <param name="propertyInfo">The PropertyInfo for the property to be validated.</param>
    ''' <param name="severity">The default severity for the rule.</param>
    ''' <param name="stopProcessing">
    ''' Initial default value for the StopProcessing property.
    ''' </param>
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyInfo As Core.IPropertyInfo, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyInfo, severity, stopProcessing)
      _decorations = args
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
    ''' <param name="args">Reference to a Dictionary containing 
    ''' name/value arguments for use by the rule method.</param>
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
    Public Sub New(ByVal propertyName As String, ByVal friendlyName As String, ByVal severity As RuleSeverity, ByVal stopProcessing As Boolean, ByVal args As Dictionary(Of String, Object))
      MyBase.New(propertyName, friendlyName, severity, stopProcessing)
      _decorations = args
    End Sub

    ''' <summary>
    ''' Gets or sets an argument value for use
    ''' by the rule method.
    ''' </summary>
    ''' <param name="key">The name under which the value is stored.</param>
    ''' <returns></returns>
    Default Public Property Item(ByVal key As String) As Object
      Get
        Dim result As Object = Nothing
        If _decorations.TryGetValue(key, result) Then
          Return result
        Else
          Return Nothing
        End If
      End Get
      Set(ByVal value As Object)
        _decorations(key) = value
      End Set
    End Property

    ''' <summary>
    ''' Return a string representation of
    ''' the object using the rule:// URI
    ''' format.
    ''' </summary>
    Public Overrides Function ToString() As String

      Dim sb As StringBuilder = New StringBuilder()
      sb.Append(MyBase.ToString())
      If _decorations.Count > 0 Then
        sb.Append("?")
        Dim first As Boolean = True
        For Each item As System.Collections.Generic.KeyValuePair(Of String, Object) In _decorations
          If first Then
            first = False
          Else
            sb.Append("&")
          End If
          sb.AppendFormat("{0}={1}", item.Key, item.Value.ToString())
        Next item
      End If
      Return sb.ToString()

    End Function

  End Class

End Namespace
