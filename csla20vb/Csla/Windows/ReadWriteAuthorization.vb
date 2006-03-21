Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Reflection

Namespace Windows

  ''' <summary>
  ''' Windows Forms extender control that automatically
  ''' enables and disables detail form controls based
  ''' on the authorization settings from a CSLA .NET 
  ''' business object.
  ''' </summary>
  <DesignerCategory("")> _
  <ProvideProperty("ApplyAuthorization", GetType(Control))> _
  Public Class ReadWriteAuthorization
    Inherits System.ComponentModel.Component

    Implements IExtenderProvider

    Private mSources As New Dictionary(Of Control, Boolean)

    Public Sub New(ByVal container As System.ComponentModel.IContainer)
      container.Add(Me)
    End Sub

    ''' <summary>
    ''' Gets a value indicating whether the extender control
    ''' can extend the specified control.
    ''' </summary>
    ''' <param name="extendee">The control to be extended.</param>
    ''' <remarks>
    ''' Any control implementing either a ReadOnly property or
    ''' Enabled property can be extended.
    ''' </remarks>
    Public Function CanExtend( _
      ByVal extendee As Object) As Boolean _
      Implements IExtenderProvider.CanExtend

      If IsPropertyImplemented(extendee, "ReadOnly") OrElse _
          IsPropertyImplemented(extendee, "Enabled") Then
        Return True

      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Gets the custom ApplyAuthorization extender
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    Public Function GetApplyAuthorization( _
      ByVal source As Control) As Boolean

      If mSources.ContainsKey(source) Then
        Return mSources.Item(source)

      Else
        Return False
      End If

    End Function

    ''' <summary>
    ''' Sets the custom ApplyAuthorization extender
    ''' property added to extended controls.
    ''' </summary>
    ''' <param name="source">Control being extended.</param>
    ''' <param name="value">New value of property.</param>
    Public Sub SetApplyAuthorization( _
      ByVal source As Control, ByVal value As Boolean)

      If mSources.ContainsKey(source) Then
        mSources.Item(source) = value
      Else
        mSources.Add(source, value)
      End If

    End Sub

    ''' <summary>
    ''' Causes the ReadWriteAuthorization control
    ''' to apply authorization rules from the business
    ''' object to all extended controls on the form.
    ''' </summary>
    ''' <remarks>
    ''' Call this method to refresh the display of detail
    ''' controls on the form any time the authorization
    ''' rules may have changed. Examples include: after
    ''' a user logs in or out, and after an object has
    ''' been updated, inserted, deleted or retrieved
    ''' from the database.
    ''' </remarks>
    Public Sub ResetControlAuthorization()

      For Each item As KeyValuePair(Of Control, Boolean) In mSources
        If item.Value Then
          ' apply authorization rules
          ApplyAuthorizationRules(item.Key)
        End If
      Next

    End Sub

    Private Sub ApplyAuthorizationRules(ByVal control As Control)

      For Each binding As Binding In control.DataBindings
        ' get the BindingSource if appropriate
        If TypeOf binding.DataSource Is BindingSource Then
          Dim bs As BindingSource = CType(binding.DataSource, BindingSource)
          ' get the BusinessObject if appropriate
          If TypeOf bs.DataSource Is Csla.Core.BusinessBase Then
            Dim ds As Csla.Core.BusinessBase = _
              CType(bs.DataSource, Csla.Core.BusinessBase)
            ' get the object property name
            Dim propertyName As String = _
              binding.BindingMemberInfo.BindingField

            ApplyReadRules(control, binding, propertyName, _
              ds.CanReadProperty(propertyName))
            ApplyWriteRules(control, binding, propertyName, _
              ds.CanWriteProperty(propertyName))

          ElseIf TypeOf bs.DataSource Is Csla.Core.IReadOnlyObject Then
            Dim ds As Csla.Core.IReadOnlyObject = _
              CType(bs.DataSource, Csla.Core.IReadOnlyObject)
            ' get the object property name
            Dim propertyName As String = _
              binding.BindingMemberInfo.BindingField

            ApplyReadRules(control, binding, propertyName, _
              ds.CanReadProperty(propertyName))
          End If
        End If
      Next

    End Sub

    Private Sub ApplyReadRules( _
      ByVal ctl As Control, ByVal binding As Binding, _
      ByVal propertyName As String, ByVal canRead As Boolean)

      ' enable/disable reading of the value
      If canRead Then
        Dim couldRead As Boolean = ctl.Enabled
        ctl.Enabled = True
        RemoveHandler binding.Format, AddressOf ReturnEmpty
        If Not couldRead Then binding.ReadValue()

      Else
        ctl.Enabled = False
        AddHandler binding.Format, AddressOf ReturnEmpty

        ' clear the value displayed by the control
        Dim propertyInfo As PropertyInfo = _
          ctl.GetType.GetProperty(binding.PropertyName, _
            BindingFlags.FlattenHierarchy Or _
            BindingFlags.Instance Or _
            BindingFlags.Public)
        If propertyInfo IsNot Nothing Then
          propertyInfo.SetValue(ctl, _
            GetEmptyValue( _
              Utilities.GetPropertyType(propertyInfo.PropertyType)), _
              New Object() {})
        End If
      End If

    End Sub

    Private Sub ApplyWriteRules( _
      ByVal ctl As Control, ByVal binding As Binding, _
      ByVal propertyName As String, ByVal canWrite As Boolean)

      If TypeOf (ctl) Is Label Then Exit Sub

      ' enable/disable writing of the value
      Dim propertyInfo As PropertyInfo = _
        ctl.GetType.GetProperty("ReadOnly", _
          BindingFlags.FlattenHierarchy Or _
          BindingFlags.Instance Or _
          BindingFlags.Public)
      If propertyInfo IsNot Nothing Then
        Dim couldWrite As Boolean = _
          Not CBool(propertyInfo.GetValue(ctl, New Object() {}))
        propertyInfo.SetValue(ctl, Not canWrite, New Object() {})
        If Not couldWrite AndAlso canWrite Then binding.ReadValue()

      Else
        Dim couldWrite As Boolean = ctl.Enabled
        ctl.Enabled = canWrite
        If Not couldWrite AndAlso canWrite Then binding.ReadValue()
      End If

    End Sub

    Private Sub ReturnEmpty( _
      ByVal sender As Object, ByVal e As ConvertEventArgs)

      e.Value = GetEmptyValue(e.DesiredType)

    End Sub

    Private Function GetEmptyValue(ByVal desiredType As Type) As Object

      Dim result As Object = Nothing
      If desiredType.IsValueType Then
        result = Activator.CreateInstance(desiredType)
      End If
      Return result

    End Function

    Private Shared Function IsPropertyImplemented(ByVal obj As Object, _
      ByVal propertyName As String) As Boolean

      If obj.GetType.GetProperty(propertyName, _
        BindingFlags.FlattenHierarchy Or _
        BindingFlags.Instance Or _
        BindingFlags.Public) IsNot Nothing Then

        Return True

      Else
        Return False
      End If

    End Function

  End Class

End Namespace
