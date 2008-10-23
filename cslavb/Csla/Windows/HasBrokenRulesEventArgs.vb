Imports System.ComponentModel

Namespace Windows

  Public Class HasBrokenRulesEventArgs

    Inherits CslaActionCancelEventArgs

    Public Sub New(ByVal commandName As String, ByVal hasErrors As Boolean, ByVal hasWarnings As Boolean, ByVal hasInformation As Boolean, ByVal autoShowBrokenRules As Boolean)
      MyBase.New(False, commandName)
      _HasErrors = hasErrors
      _HasWarnings = hasWarnings
      _HasInformation = hasInformation
      _AutoShowBrokenRules = autoShowBrokenRules
    End Sub

    Public Sub New(ByVal cancel As Boolean, ByVal commandName As String, ByVal hasErrors As Boolean, ByVal hasWarnings As Boolean, ByVal hasInformation As Boolean, ByVal autoShowBrokenRules As Boolean)
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

    Public ReadOnly Property HasErrors() As Boolean
      Get
        Return _HasErrors
      End Get
    End Property

    Public ReadOnly Property HasWarning() As Boolean
      Get
        Return _HasWarnings
      End Get
    End Property

    Public ReadOnly Property HasInformation() As Boolean
      Get
        Return _HasInformation
      End Get
    End Property

    Public ReadOnly Property AutoShowBrokenRules() As Boolean
      Get
        Return _AutoShowBrokenRules
      End Get
    End Property

  End Class

End Namespace
