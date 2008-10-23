#Region "Namespace imports"

Imports System
Imports System.Data
Imports System.ComponentModel
Imports System.Configuration
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports Csla

#End Region

Namespace Windows

  Friend Class CslaActionExtenderProperties

#Region "Defaults"

    Friend Shared ActionTypeDefault As CslaFormAction = CslaFormAction.None
    Friend Shared PostSaveActionDefault As PostSaveActionType = PostSaveActionType.None
    Friend Shared RebindAfterSaveDefault As Boolean = True
    Friend Shared DisableWhenCleanDefault As Boolean = False
    Friend Shared CommandNameDefault As String = String.Empty

#End Region

#Region "Member variables"

    Protected _ActionType As CslaFormAction = ActionTypeDefault
    Protected _PostSaveAction As PostSaveActionType = PostSaveActionDefault
    Protected _RebindAfterSave As Boolean = RebindAfterSaveDefault
    Protected _DisableWhenClean As Boolean = DisableWhenCleanDefault
    Protected _CommandName As String = CommandNameDefault

#End Region

#Region "Public properties"

    Public Property ActionType() As CslaFormAction
      Get
        Return _ActionType
      End Get
      Set(ByVal value As CslaFormAction)
        _ActionType = value
      End Set
    End Property

    Public Property PostSaveAction() As PostSaveActionType
      Get
        Return _PostSaveAction
      End Get
      Set(ByVal value As PostSaveActionType)
        _PostSaveAction = value
      End Set
    End Property

    Public Property RebindAfterSave() As Boolean
      Get
        Return _RebindAfterSave
      End Get
      Set(ByVal value As Boolean)
        _RebindAfterSave = value
      End Set
    End Property

    Public Property DisableWhenClean() As Boolean
      Get
        Return _DisableWhenClean
      End Get
      Set(ByVal value As Boolean)
        _DisableWhenClean = value
      End Set
    End Property

    Public Property CommandName() As String
      Get
        Return _CommandName
      End Get
      Set(ByVal value As String)
        _CommandName = value
      End Set
    End Property

#End Region

  End Class

End Namespace
