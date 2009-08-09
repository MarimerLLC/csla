#Region "Namespace imports"

Imports System
Imports System.Data
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Xml
Imports Csla
Imports Csla.Core
Imports Csla.Validation

#End Region

Namespace Windows

  ''' <summary>
  ''' Extender control providing automation around
  ''' data binding to CSLA .NET business objects.
  ''' </summary>
  <ToolboxItem(True)> _
  <ProvideProperty("ActionType", GetType(Control))> _
  <ProvideProperty("PostSaveAction", GetType(Control))> _
  <ProvideProperty("RebindAfterSave", GetType(Control))> _
  <ProvideProperty("DisableWhenClean", GetType(Control))> _
  <ProvideProperty("CommandName", GetType(Control))> _
  Public Class CslaActionExtender

    Inherits Component
    Implements IExtenderProvider

#Region "Constants"

    Private Const STR_InvalidDataSource As String = "DataSource must be a BindingSource control instance."
    Private Const STR_InvalidBindingSourceCast As String = "DataSource does not cast to a BindingSource."
    Private Const STR_InvalidBusinessObjectBaseCast As String = "The underlying data source does not cast to a CSLA BusinessBase object."
    Private Const STR_ClickingEventDescription As String = "Event fires just before the attempted action."
    Private Const STR_ClickedEventDescription As String = "Event fires after a successful action.  When button is set to Save, this event will only fire upon a successful save.  If button is set to Close, this event will never fire."
    Private Const STR_ErrorEncounteredDescription As String = "Event fires upon encountering any exception during an action."
    Private Const STR_SetForNewEventDescription As String = "Event fires upon a successful save when the PostSaveAction property is set to AndNew."
    Private Const STR_BusinessObjectInvalidEventDescription As String = "Event fires when the object is in an invalid state.  Note that this event will work in conjunction with the InvalidateOnWarnings and InvalidateOnInformation properties."
    Private Const STR_HasBrokenRulesEventDescription As String = "Event fires if there are any broken rules at all, despite severity."
    Private Const STR_ActionTypePropertyDescription As String = "Gets or sets the type for this button."
    Private Const STR_PostSaveActionTypeDescription As String = "Gets or sets the action performed after a save (if ActionType is set to Save)."
    Private Const STR_DataSourcePropertyDescription As String = "Gets or sets the data source to which this button is bound for action purposes."
    Private Const STR_RebindAfterSavePropertyDescription As String = "Determines if the binding source will rebind after business object saves."
    Private Const STR_AutoShowBrokenRulesPropertyDescription As String = "If True, then the broken rules will be displayed in a message box, should the object be invalid."
    Private Const STR_InvalidateOnWarningsPropertyDescription As String = "If True, then the business object will be considered invalid if there are any warnings in the broken rules."
    Private Const STR_InvalidateOnInformationPropertyDescription As String = "If True, then the business object will be considered invalid if there are any information items in the broken rules."
    Private Const STR_DisableWhenCleanPropertyDescription As String = "If True, then the dirtiness of the underlying business object will cause this button to enable or disable."
    Private Const STR_ObjectSavedEventDescription As String = "Fires immediately after the underlying object successfully saves."
    Private Const STR_WarnIfCloseOnDirtyPropertyDescription As String = "If True, then the control (when set to Close mode) will warn the user if the object is currently dirty."
    Private Const STR_DirtyWarningMessagePropertyDescription As String = "Gets or sets the confirmation message that will display if a Close button is pressed and the object is dirty."
    Private Const STR_CloseConfirmation As String = "Are you sure you want to close?"
    Private Const STR_WarnOnCancelPropertyDescription As String = "If True, then the Cancel button will warn when pressed and the object is dirty."
    Private Const STR_WarnOnCancelMessagePropertyDescription As String = "If the WarnOnCancel property is set to True, this is the message to be displayed."
    Private Const STR_DirtyWarningMessagePropertyDefault As String = "Object is currently in a dirty changed."
    Private Const STR_WarnOnCancelMessagePropertyDefault As String = "Are you sure you want to revert to the previous values?"
    Private Const STR_PostSaveActionTypePropertyDescription As String = "Gets or sets the action performed after a save (if ActionType is set to Save)."
    Private Const STR_CommandNamePropertyDescription As String = "Gets or sets the name of this command control for unique identification purposes."
    Private Const STR_ObjectSavingEventDescription As String = "Fires just before a save action is performed."

#End Region

#Region "Variable defaults"

    Private Shared DataSourceDefault As Object = Nothing
    Private Shared AutoShowBrokenRulesDefault As Boolean = True
    Private Shared WarnIfCloseOnDirtyDefault As Boolean = True
    Private Shared DirtyWarningMessageDefault As String = STR_DirtyWarningMessagePropertyDefault
    Private Shared WarnOnCancelDefault As Boolean = False
    Private Shared WarnOnCancelMessageDefault As String = STR_WarnOnCancelMessagePropertyDefault

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Creates an instance of the type.
    ''' </summary>
    ''' <param name="container">Container for the component.</param>
    Public Sub New(ByVal container As IContainer)
      MyBase.New()
      _Container = container
      container.Add(Me)
    End Sub

#End Region

#Region "Member variables"

    Private _Sources As New Dictionary(Of Control, CslaActionExtenderProperties)()

    Private _DataSource As Object = DataSourceDefault
    Private _AutoShowBrokenRules As Boolean = AutoShowBrokenRulesDefault
    Private _WarnIfCloseOnDirty As Boolean = WarnIfCloseOnDirtyDefault
    Private _DirtyWarningMessage As String = DirtyWarningMessageDefault
    Private _WarnOnCancel As Boolean = WarnOnCancelDefault
    Private _WarnOnCancelMessage As String = WarnOnCancelMessageDefault
    Private _Container As IContainer = Nothing
    Private _BindingSourceTree As BindingSourceNode = Nothing
    Private _CloseForm As Boolean = False

#End Region

#Region "IExtenderProvider implementation"

    Function CanExtend(ByVal extendee As Object) As Boolean Implements IExtenderProvider.CanExtend
      Dim ret As Boolean = False
      If TypeOf extendee Is IButtonControl Then
        ret = True
      End If
      Return ret
    End Function

#End Region

#Region "Public properties"

    ''' <summary>
    ''' Gets or sets a reference to the data source object.
    ''' </summary>
    <Category("Data")> _
    <Description(STR_DataSourcePropertyDescription)> _
    <AttributeProvider(GetType(IListSource))> _
    Public Property DataSource() As Object
      Get
        Return _DataSource
      End Get
      Set(ByVal value As Object)
        If value IsNot Nothing Then
          If TypeOf value Is BindingSource Then
            _DataSource = value
          Else
            Throw New ArgumentException(STR_InvalidDataSource)
          End If
        End If
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether to automatically
    ''' show broken rules.
    ''' </summary>
    <Category("Behavior")> _
    <Description(STR_AutoShowBrokenRulesPropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Property AutoShowBrokenRules() As Boolean
      Get
        Return _AutoShowBrokenRules
      End Get
      Set(ByVal value As Boolean)
        _AutoShowBrokenRules = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether to warn the
    ''' user on close when the object is dirty.
    ''' </summary>
    <Category("Behavior")> _
    <Description(STR_WarnIfCloseOnDirtyPropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Property WarnIfCloseOnDirty() As Boolean
      Get
        Return _WarnIfCloseOnDirty
      End Get
      Set(ByVal value As Boolean)
        _WarnIfCloseOnDirty = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the message shown to the user
    ''' in a close on dirty warning.
    ''' </summary>
    <Category("Behavior")> _
    <Description(STR_DirtyWarningMessagePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(STR_DirtyWarningMessagePropertyDefault)> _
    <Localizable(True)> _
    Public Property DirtyWarningMessage() As String
      Get
        Return _DirtyWarningMessage
      End Get
      Set(ByVal value As String)
        _DirtyWarningMessage = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets a value indicating whether to warn
    ''' the user on cancel.
    ''' </summary>
    <Category("Behavior")> _
    <Description(STR_WarnOnCancelPropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(False)> _
    Public Property WarnOnCancel() As Boolean
      Get
        Return _WarnOnCancel
      End Get
      Set(ByVal value As Boolean)
        _WarnOnCancel = value
      End Set
    End Property

    ''' <summary>
    ''' Gets or sets the message shown to the user
    ''' in a warn on cancel.
    ''' </summary>
    <Category("Behavior")> _
    <Description(STR_WarnOnCancelMessagePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(STR_WarnOnCancelMessagePropertyDefault)> _s
    <Localizable(True)> _
    Public Property WarnOnCancelMessage() As String
      Get
        Return _WarnOnCancelMessage
      End Get
      Set(ByVal value As String)
        _WarnOnCancelMessage = value
      End Set
    End Property

#End Region

#Region "Property accessor methods"

#Region "ActionType"

    ''' <summary>
    ''' Gets the action type.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <returns></returns>
    <Category("Csla")> _
    <Description(STR_ActionTypePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(CslaFormAction.None)> _
    Public Function GetActionType(ByVal ctl As Control) As CslaFormAction
      If _Sources.ContainsKey(ctl) Then
        Return (DirectCast(_Sources(ctl), CslaActionExtenderProperties)).ActionType
      Else
        Return CslaActionExtenderProperties.ActionTypeDefault
      End If
    End Function

    ''' <summary>
    ''' Sets the action type.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <param name="value">Value for property.</param>
    <Category("Csla")> _
    <Description(STR_ActionTypePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(CslaFormAction.None)> _
    Public Sub SetActionType(ByVal ctl As Control, ByVal value As CslaFormAction)
      If _Sources.ContainsKey(ctl) Then
        _Sources(ctl).ActionType = value
      Else
        Dim props As New CslaActionExtenderProperties()
        props.ActionType = value
        _Sources.Add(ctl, props)
      End If
    End Sub

#End Region

#Region "PostSaveAction"

    ''' <summary>
    ''' Gets the post save action.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <returns></returns>
    <Category("Csla")> _
    <Description(STR_PostSaveActionTypePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(PostSaveActionType.None)> _
    Public Function GetPostSaveAction(ByVal ctl As Control) As PostSaveActionType
      If _Sources.ContainsKey(ctl) Then
        Return (DirectCast(_Sources(ctl), CslaActionExtenderProperties)).PostSaveAction
      Else
        Return CslaActionExtenderProperties.PostSaveActionDefault
      End If
    End Function

    ''' <summary>
    ''' Sets the post save action.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <param name="value">Value for property.</param>
    <Category("Csla")> _
    <Description(STR_PostSaveActionTypePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(PostSaveActionType.None)> _
    Public Sub SetPostSaveAction(ByVal ctl As Control, ByVal value As PostSaveActionType)
      If _Sources.ContainsKey(ctl) Then
        _Sources(ctl).PostSaveAction = value
      Else
        Dim props As New CslaActionExtenderProperties()
        props.PostSaveAction = value
        _Sources.Add(ctl, props)
      End If
    End Sub

#End Region

#Region "RebindAfterSave"

    ''' <summary>
    ''' Gets the rebind after save value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    <Category("Csla")> _
    <Description(STR_RebindAfterSavePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Function GetRebindAfterSave(ByVal ctl As Control) As Boolean
      If _Sources.ContainsKey(ctl) Then
        Return (DirectCast(_Sources(ctl), CslaActionExtenderProperties)).RebindAfterSave
      Else
        Return CslaActionExtenderProperties.RebindAfterSaveDefault
      End If
    End Function

    ''' <summary>
    ''' Sets the rebind after save value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <param name="value">Value for property.</param>
    <Category("Csla")> _
    <Description(STR_RebindAfterSavePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Sub SetRebindAfterSave(ByVal ctl As Control, ByVal value As Boolean)
      If _Sources.ContainsKey(ctl) Then
        _Sources(ctl).RebindAfterSave = value
      Else
        Dim props As New CslaActionExtenderProperties()
        props.RebindAfterSave = value
        _Sources.Add(ctl, props)
      End If
    End Sub

#End Region

#Region "DisableWhenClean"

    ''' <summary>
    ''' Gets the disable when clean value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    <Category("Csla")> _
    <Description(STR_DisableWhenCleanPropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Function GetDisableWhenClean(ByVal ctl As Control) As Boolean
      If _Sources.ContainsKey(ctl) Then
        Return (DirectCast(_Sources(ctl), CslaActionExtenderProperties)).DisableWhenClean
      Else
        Return CslaActionExtenderProperties.DisableWhenCleanDefault
      End If
    End Function

    ''' <summary>
    ''' Sets the disable when clean value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <param name="value">Value for property.</param>
    <Category("Csla")> _
    <Description(STR_DisableWhenCleanPropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue(True)> _
    Public Sub SetDisableWhenClean(ByVal ctl As Control, ByVal value As Boolean)
      If _Sources.ContainsKey(ctl) Then
        _Sources(ctl).DisableWhenClean = value
      Else
        Dim props As New CslaActionExtenderProperties()
        props.DisableWhenClean = value
        _Sources.Add(ctl, props)
      End If
    End Sub

#End Region

#Region "CommandName"

    ''' <summary>
    ''' Gets the command name value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    <Category("Csla")> _
    <Description(STR_CommandNamePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue("")> _
    Public Function GetCommandName(ByVal ctl As Control) As String
      If _Sources.ContainsKey(ctl) Then
        Return (DirectCast(_Sources(ctl), CslaActionExtenderProperties)).CommandName
      Else
        Return CslaActionExtenderProperties.CommandNameDefault
      End If
    End Function

    ''' <summary>
    ''' Sets the command name value.
    ''' </summary>
    ''' <param name="ctl">Reference to control.</param>
    ''' <param name="value">Value for property.</param>
    <Category("Csla")> _
    <Description(STR_CommandNamePropertyDescription)> _
    <Bindable(True)> _
    <DefaultValue("")> _
    Public Sub SetCommandName(ByVal ctl As Control, ByVal value As String)
      If _Sources.ContainsKey(ctl) Then
        _Sources(ctl).CommandName = value
      Else
        Dim props As New CslaActionExtenderProperties()
        props.CommandName = value
        _Sources.Add(ctl, props)
      End If
    End Sub
#End Region

#End Region

#Region "Event declarations"

    ''' <summary>
    ''' Event indicating the user is clicking on the control.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_ClickingEventDescription)> _
    Public Event Clicking As EventHandler(Of CslaActionCancelEventArgs)

    ''' <summary>
    ''' Event indicating the user clicked on the control.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_ClickedEventDescription)> _
    Public Event Clicked As EventHandler(Of CslaActionEventArgs)

    ''' <summary>
    ''' Event indicating an error was encountered.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_ErrorEncounteredDescription)> _
    Public Event ErrorEncountered As EventHandler(Of ErrorEncounteredEventArgs)

    ''' <summary>
    ''' Event indicating the object is set for new.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_SetForNewEventDescription)> _
    Public Event SetForNew As EventHandler(Of CslaActionEventArgs)

    ''' <summary>
    ''' Event indicating the business object is in an invalid state.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_BusinessObjectInvalidEventDescription)> _
    Public Event BusinessObjectInvalid As EventHandler(Of CslaActionEventArgs)

    ''' <summary>
    ''' Event indicating the business object has broken rules.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_HasBrokenRulesEventDescription)> _
    Public Event HasBrokenRules As EventHandler(Of HasBrokenRulesEventArgs)

    ''' <summary>
    ''' Event indicating that the object is saving.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_ObjectSavingEventDescription)> _
    Public Event ObjectSaving As EventHandler(Of CslaActionCancelEventArgs)

    ''' <summary>
    ''' Event indicating that the object has been saved.
    ''' </summary>
    <Category("Csla")> _
    <Description(STR_ObjectSavedEventDescription)> _
    Public Event ObjectSaved As EventHandler(Of CslaActionEventArgs)

#End Region

#Region "OnEvent methods"

    ''' <summary>
    ''' Raises the Clicking event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnClicking(ByVal e As CslaActionCancelEventArgs)
      RaiseEvent Clicking(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the Clicked event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnClicked(ByVal e As CslaActionEventArgs)
      RaiseEvent Clicked(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the ErrorEncountered event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnErrorEncountered(ByVal e As ErrorEncounteredEventArgs)
      RaiseEvent ErrorEncountered(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the SetForNew event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnSetForNew(ByVal e As CslaActionEventArgs)
      RaiseEvent SetForNew(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the BusinessObjectInvalid event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnBusinessObjectInvalid(ByVal e As CslaActionEventArgs)
      RaiseEvent BusinessObjectInvalid(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the HasBrokenRules event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnHasBrokenRules(ByVal e As HasBrokenRulesEventArgs)
      RaiseEvent HasBrokenRules(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the ObjectSaving event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnObjectSaving(ByVal e As CslaActionCancelEventArgs)
      RaiseEvent ObjectSaving(Me, e)
    End Sub

    ''' <summary>
    ''' Raises the ObjectSaved event.
    ''' </summary>
    ''' <param name="e">Event arguments.</param>
    Protected Overridable Sub OnObjectSaved(ByVal e As CslaActionEventArgs)
      RaiseEvent ObjectSaved(Me, e)
    End Sub

#End Region

#Region "Public methods"

    ''' <summary>
    ''' Resets all action behaviors.
    ''' </summary>
    ''' <param name="objectToBind">Target object.</param>
    Public Sub ResetActionBehaviors(ByVal objectToBind As ISavable)

      InitializeControls(True)

      Dim rootSource As BindingSource = TryCast(_DataSource, BindingSource)

      If rootSource IsNot Nothing Then
        RemoveHandler rootSource.CurrentItemChanged, AddressOf BusinessObject_PropertyChanged
        AddHandler rootSource.CurrentItemChanged, AddressOf BusinessObject_PropertyChanged
      End If

      _BindingSourceTree = BindingSourceHelper.InitializeBindingSourceTree(_Container, rootSource)
      _BindingSourceTree.Bind(objectToBind)

    End Sub

#End Region

#Region "Protected methods"

    ''' <summary>
    ''' Method invoked when the target control is clicked.
    ''' </summary>
    ''' <param name="sender">Object originating action.</param>
    ''' <param name="e">Arguments.</param>
    Protected Sub OnClick(ByVal sender As Object, ByVal e As System.EventArgs)

      Dim ctl As Control = DirectCast(sender, Control)
      Dim props As CslaActionExtenderProperties = TryCast(_Sources(ctl), CslaActionExtenderProperties)

      If props.ActionType <> CslaFormAction.None Then

        Try

          Dim raiseClicked As Boolean = True
          Dim args As New CslaActionCancelEventArgs(False, props.CommandName)

          OnClicking(args)

          If Not args.Cancel Then

            ' perform action
            Dim savableObject As Csla.Core.ISavable = Nothing
            Dim trackableObject As Csla.Core.ITrackStatus = Nothing

            Dim source As BindingSource = Nothing
            If _DataSource IsNot Nothing Then
              source = TryCast(_DataSource, BindingSource)
              If source IsNot Nothing Then
                savableObject = TryCast(source.DataSource, Csla.Core.ISavable)
                trackableObject = TryCast(source.DataSource, Csla.Core.ITrackStatus)
              Else
                OnErrorEncountered(New ErrorEncounteredEventArgs(props.CommandName, New InvalidCastException(STR_InvalidBindingSourceCast)))
              End If
              If savableObject Is Nothing OrElse trackableObject Is Nothing Then
                OnErrorEncountered(New ErrorEncounteredEventArgs(props.CommandName, New InvalidCastException(STR_InvalidBusinessObjectBaseCast)))
              End If
            End If

            Dim diagResult As DialogResult

            Select Case props.ActionType

              Case CslaFormAction.Save

                Dim okToContinue As Boolean = True

                If TypeOf (savableObject) Is Csla.Core.BusinessBase Then

                  Dim businessObj As Csla.Core.BusinessBase = DirectCast(savableObject, Csla.Core.BusinessBase)

                  If businessObj.BrokenRulesCollection.Count > 0 Then

                    Dim argsHasBrokenRules As New HasBrokenRulesEventArgs(props.CommandName, _
                                                  businessObj.BrokenRulesCollection.ErrorCount > 0, _
                                                  businessObj.BrokenRulesCollection.WarningCount > 0, _
                                                  businessObj.BrokenRulesCollection.InformationCount > 0, _
                                                  _AutoShowBrokenRules)

                    OnHasBrokenRules(argsHasBrokenRules)

                    okToContinue = Not argsHasBrokenRules.Cancel
                    'in case the client changed it
                    _AutoShowBrokenRules = argsHasBrokenRules.AutoShowBrokenRules

                  End If

                End If

                If okToContinue Then

                  If TypeOf (savableObject) Is Csla.Core.BusinessBase Then
                    Dim businessObject As Csla.Core.BusinessBase = DirectCast(savableObject, Csla.Core.BusinessBase)
                    If _AutoShowBrokenRules AndAlso businessObject.BrokenRulesCollection.Count > 0 Then
                      MessageBox.Show(businessObject.BrokenRulesCollection.ToString())
                    End If
                  End If

                  Dim objectValid As Boolean = trackableObject.IsValid

                  If objectValid Then

                    Dim savingArgs As CslaActionCancelEventArgs = New CslaActionCancelEventArgs(False, props.CommandName)
                    OnObjectSaving(savingArgs)

                    If Not savingArgs.Cancel Then

                      _BindingSourceTree.Apply()

                      'save
                      Dim objectToSave As Csla.Core.ISavable = savableObject
                      If Csla.ApplicationContext.AutoCloneOnUpdate Then
                        objectToSave = TryCast((DirectCast(savableObject, ICloneable)).Clone(), Csla.Core.ISavable)
                      End If

                      If objectToSave IsNot Nothing Then

                        Dim saveableObject As Csla.Core.ISavable = TryCast(objectToSave, Csla.Core.ISavable)

                        Try
                          savableObject = DirectCast(saveableObject.Save(), Csla.Core.ISavable)

                          OnObjectSaved(New CslaActionEventArgs(props.CommandName))

                          Select Case props.PostSaveAction
                            Case PostSaveActionType.None

                              If source IsNot Nothing AndAlso props.RebindAfterSave Then
                                _BindingSourceTree.Bind(savableObject)
                              End If

                            Case PostSaveActionType.AndClose

                              CloseForm()

                            Case PostSaveActionType.AndNew

                              OnSetForNew(New CslaActionEventArgs(props.CommandName))

                          End Select
                        Catch ex As Exception
                          OnErrorEncountered(New ErrorEncounteredEventArgs(props.CommandName, New ObjectSaveException(ex)))
                          raiseClicked = False
                        End Try
                      Else
                        ' did not find bound object so don't bother raising the Clicked event
                        raiseClicked = False
                      End If

                      _BindingSourceTree.SetEvents(True)

                    End If

                  Else
                    OnBusinessObjectInvalid(New CslaActionEventArgs(props.CommandName))
                    ' object not valid or has broken rules set to invalidate it due to this control's properties
                    raiseClicked = False
                  End If
                Else
                  ' process was canceled from the HasBrokenRules event
                  raiseClicked = False
                End If

              Case CslaFormAction.Cancel

                diagResult = System.Windows.Forms.DialogResult.Yes
                If _WarnOnCancel AndAlso trackableObject.IsDirty Then
                  diagResult = MessageBox.Show(_WarnOnCancelMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                End If

                If diagResult = System.Windows.Forms.DialogResult.Yes Then
                  _BindingSourceTree.Cancel(savableObject)
                End If

              Case CslaFormAction.Close

                diagResult = System.Windows.Forms.DialogResult.Yes
                If trackableObject.IsDirty OrElse trackableObject.IsNew Then
                  If _WarnIfCloseOnDirty Then
                    diagResult = MessageBox.Show(_DirtyWarningMessage + Environment.NewLine + STR_CloseConfirmation, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                  End If
                End If

                If diagResult = System.Windows.Forms.DialogResult.Yes Then
                  _BindingSourceTree.Close()
                  _CloseForm = True
                End If

            End Select

            If raiseClicked Then
              If props.ActionType = CslaFormAction.Save AndAlso source IsNot Nothing AndAlso props.RebindAfterSave Then
                ' For some strange reason, this has to be done down here.
                ' Putting it in the Select Case AfterSave... does not work.
                _BindingSourceTree.ResetBindings(False)
                InitializeControls(True)
              End If

              OnClicked(New CslaActionEventArgs(props.CommandName))
            End If
          End If

          If _CloseForm Then
            CloseForm()
          End If

        Catch ex As Exception
          OnErrorEncountered(New ErrorEncounteredEventArgs(props.CommandName, ex))
        End Try

      End If

    End Sub

#End Region

#Region "Private methods"

    Private Sub ResetControls()
      InitializeControls(False)
    End Sub

    Private Sub InitializeControls(ByVal initialEnabling As Boolean)
      ' controls will not be enabled until the BusinessObjectPropertyChanged event fires or if it's in an appropriate state now
      Dim extendedControls As New List(Of Control)()
      For Each pair As KeyValuePair(Of Control, CslaActionExtenderProperties) In _Sources
        If pair.Value.ActionType <> CslaFormAction.None Then
          Dim ctl As Control = pair.Key
          If initialEnabling Then
            ChangeEnabled(ctl, Not pair.Value.DisableWhenClean)
            RemoveHandler pair.Key.Click, AddressOf OnClick
            AddHandler pair.Key.Click, AddressOf OnClick
          End If
          InitializeControl(ctl)
          extendedControls.Add(ctl)
        End If
      Next
    End Sub

    Private Sub InitializeControl(ByVal ctl As Control)
      If Not ctl.Enabled Then
        Dim businessObject As Csla.Core.ISavable = GetBusinessObject()
        If businessObject IsNot Nothing Then
          Dim trackableObject As ITrackStatus = TryCast(businessObject, ITrackStatus)
          If trackableObject IsNot Nothing Then
            ChangeEnabled(ctl, trackableObject.IsNew OrElse trackableObject.IsDirty OrElse trackableObject.IsDeleted)
          End If
        End If
      End If
    End Sub

    Private Sub ChangeEnabled(ByVal ctl As Control, ByVal newEnabled As Boolean)
      ' only do this if it's changed to avoid flicker
      If ctl.Enabled <> newEnabled Then
        ctl.Enabled = newEnabled
      End If
    End Sub

    Private Sub CloseForm()
      If _Sources.Count > 0 Then
        Dim enumerator As Dictionary(Of Control, CslaActionExtenderProperties).Enumerator = _Sources.GetEnumerator()
        If enumerator.MoveNext() Then
          Dim ctl As Control = enumerator.Current.Key
          Dim frm As Form = GetParentForm(ctl)
          If frm IsNot Nothing Then
            frm.Close()
          End If
        End If
      End If
    End Sub

    Private Function GetParentForm(ByVal thisControl As Control) As Form
      Dim frm As Form = Nothing

      If TypeOf thisControl.Parent Is Form Then
        frm = DirectCast(thisControl.Parent, Form)
      Else
        frm = GetParentForm(thisControl.Parent)
      End If

      Return frm
    End Function

    Private Function GetBusinessObject() As Csla.Core.ISavable
      Dim businessObject As Csla.Core.ISavable = Nothing
      Dim source As BindingSource = TryCast(_DataSource, BindingSource)
      If source IsNot Nothing Then
        businessObject = TryCast(source.DataSource, Csla.Core.ISavable)
      End If
      Return businessObject
    End Function

#End Region

#Region "Event methods"

    Private Sub BusinessObject_PropertyChanged(ByVal sender As Object, ByVal e As EventArgs)
      ResetControls()
    End Sub

#End Region

  End Class

End Namespace
