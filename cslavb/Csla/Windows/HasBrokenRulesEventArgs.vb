Imports System.ComponentModel

Namespace Windows

  ''' <summary>
  ''' Event args object containing information about a
  ''' broken rule.
  ''' </summary>
  Public Class HasBrokenRulesEventArgs

    Inherits CslaActionCancelEventArgs

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="commandName">
    ''' Name of the command.
    ''' </param>
    ''' <param name="hasErrors">
    ''' Indicates whether error severity exists.
    ''' </param>
    ''' <param name="hasWarnings">
    ''' Indicates whether warning severity exists.
    ''' </param>
    ''' <param name="hasInformation">
    ''' Indicates whether information severity exists.
    ''' </param>
    ''' <param name="autoShowBrokenRules">
    ''' Indicates whether to automatically show broken rules.
    ''' </param>
    Public Sub New(ByVal commandName As String, ByVal hasErrors As Boolean, _
                   ByVal hasWarnings As Boolean, ByVal hasInformation As Boolean, ByVal autoShowBrokenRules As Boolean)
      MyBase.New(False, commandName)
      _HasErrors = hasErrors
      _HasWarnings = hasWarnings
      _HasInformation = hasInformation
      _AutoShowBrokenRules = autoShowBrokenRules
    End Sub

    ''' <summary>
    ''' Creates a new instance of the object.
    ''' </summary>
    ''' <param name="cancel">
    ''' Indicates whether to cancel.
    ''' </param>
    ''' <param name="commandName">
    ''' Name of the command.
    ''' </param>
    ''' <param name="hasErrors">
    ''' Indicates whether error severity exists.
    ''' </param>
    ''' <param name="hasWarnings">
    ''' Indicates whether warning severity exists.
    ''' </param>
    ''' <param name="hasInformation">
    ''' Indicates whether information severity exists.
    ''' </param>
    ''' <param name="autoShowBrokenRules">
    ''' Indicates whether to automatically show broken rules.
    ''' </param>
    Public Sub New(ByVal cancel As Boolean, ByVal commandName As String, _
                   ByVal hasErrors As Boolean, ByVal hasWarnings As Boolean, ByVal hasInformation As Boolean, ByVal autoShowBrokenRules As Boolean)
      MyBase.New(cancel, commandName)
      _HasErrors = hasErrors
      _HasWarnings = hasWarnings
      _HasInformation = hasInformation
      _AutoShowBrokenRules = autoShowBrokenRules
    End Sub

    Private _HasErrors As Boolean = False
    Private _HasWarnings As Boolean = False
    Private _HasInformation As Boolean = False
    Private _AutoShowBrokenRules As Boolean = False

    ''' <summary>
    ''' Gets a value indicating whether
    ''' an error severity rule exists.
    ''' </summary>
    Public ReadOnly Property HasErrors() As Boolean
      Get
        Return _HasErrors
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether
    ''' a warning severity rule exists.
    ''' </summary>
    Public ReadOnly Property HasWarning() As Boolean
      Get
        Return _HasWarnings
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether
    ''' an information severity rule exists.
    ''' </summary>
    Public ReadOnly Property HasInformation() As Boolean
      Get
        Return _HasInformation
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether
    ''' to show broken rules.
    ''' </summary>
    Public ReadOnly Property AutoShowBrokenRules() As Boolean
      Get
        Return _AutoShowBrokenRules
      End Get
    End Property

  End Class

End Namespace
