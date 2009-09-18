Imports System
Imports System.ComponentModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports Csla.Core
Imports Csla.Reflection

''' <summary>
''' This is the base class from which most business objects
''' will be derived.
''' </summary>
''' <remarks>
''' <para>
''' This class is the core of the CSLA .NET framework. To create
''' a business object, inherit from this class.
''' </para><para>
''' Please refer to 'Expert VB 2008 Business Objects' for
''' full details on the use of this base class to create business
''' objects.
''' </para>
''' </remarks>
''' <typeparam name="T">Type of the business object being defined.</typeparam>
<Serializable()> _
Public MustInherit Class BusinessBase(Of T As BusinessBase(Of T))
  Inherits Core.BusinessBase

  Implements Core.ISavable



#Region " Object ID Value "

  ''' <summary>
  ''' Override this method to return a unique identifying
  ''' value for this object.
  ''' </summary>
  Protected Overridable Function GetIdValue() As Object
    Return Nothing
  End Function

#End Region

#Region " System.Object Overrides "

  ''' <summary>
  ''' Returns a text representation of this object by
  ''' returning the <see cref="GetIdValue"/> value
  ''' in text form.
  ''' </summary>
  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Return MyBase.ToString
    End If
    Return id.ToString

  End Function

#End Region

#Region " Clone "

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overloads Function Clone() As T

    Return CType(GetClone(), T)

  End Function

#End Region

#Region " Data Access "

  ''' <summary>
  ''' Saves the object to the database.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Calling this method starts the save operation, causing the object
  ''' to be inserted, updated or deleted within the database based on the
  ''' object's current state.
  ''' </para><para>
  ''' If <see cref="Core.BusinessBase.IsDeleted" /> is <see langword="true"/>
  ''' the object will be deleted. Otherwise, if <see cref="Core.BusinessBase.IsNew" /> 
  ''' is <see langword="true"/> the object will be inserted. 
  ''' Otherwise the object's data will be updated in the database.
  ''' </para><para>
  ''' All this is contingent on <see cref="Core.BusinessBase.IsDirty" />. If
  ''' this value is <see langword="false"/>, no data operation occurs. 
  ''' It is also contingent on <see cref="Core.BusinessBase.IsValid" />. 
  ''' If this value is <see langword="false"/> an
  ''' exception will be thrown to indicate that the UI attempted to save an
  ''' invalid object.
  ''' </para><para>
  ''' It is important to note that this method returns a new version of the
  ''' business object that contains any data updated during the save operation.
  ''' You MUST update all object references to use this new version of the
  ''' business object in order to have access to the correct object data.
  ''' </para><para>
  ''' You can override this method to add your own custom behaviors to the save
  ''' operation. For instance, you may add some security checks to make sure
  ''' the user can save the object. If all security checks pass, you would then
  ''' invoke the base Save method via <c>MyBase.Save()</c>.
  ''' </para>
  ''' </remarks>
  ''' <returns>A new object containing the saved values.</returns>
  Public Overridable Function Save() As T

    If Me.IsChild Then
      Throw New NotSupportedException(My.Resources.NoSaveChildException)
    End If

    If EditLevel > 0 Then
      Throw New Validation.ValidationException(My.Resources.NoSaveEditingException)
    End If

    If Not IsValid AndAlso Not IsDeleted Then
      Throw New Validation.ValidationException(My.Resources.NoSaveInvalidException)
    End If

    If IsBusy Then
      Throw New Validation.ValidationException(My.Resources.BusyObjectsMayNotBeSaved)
    End If

    Dim result As T
    If IsDirty Then
      result = CType(DataPortal.Update(Me), T)
    Else
      result = CType(Me, T)
    End If

    OnSaved(result, Nothing, Nothing)
    Return result

  End Function

  ''' <summary>
  ''' Saves the object to the database, forcing
  ''' IsNew to <see langword="false"/> and IsDirty to True.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
  ''' If <see langword="false"/> then it is the same as calling Save().
  ''' </param>
  ''' <returns>A new object containing the saved values.</returns>
  ''' <remarks>
  ''' This overload is designed for use in web applications
  ''' when implementing the Update method in your 
  ''' data wrapper object.
  ''' </remarks>
  Public Function Save(ByVal forceUpdate As Boolean) As T

    If forceUpdate AndAlso IsNew Then
      ' mark the object as old - which makes it
      ' not dirty
      MarkOld()
      ' now mark the object as dirty so it can save
      MarkDirty(True)
    End If
    Return Me.Save()

  End Function

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  Public Sub BeginSave() Implements Core.ISavable.BeginSave
    Dim saved As EventHandler(Of SavedEventArgs) = Nothing
    BeginSave(saved)
  End Sub

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  ''' <param name="userState">User state data.</param>
  Public Sub BeginSave(ByVal userState As Object) Implements Core.ISavable.BeginSave
    BeginSave(False, Nothing, userState)
  End Sub

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  ''' <param name="handler">
  ''' Method called when the operation is complete.
  ''' </param>
  Public Sub BeginSave(ByVal handler As EventHandler(Of Core.SavedEventArgs))
    BeginSave(False, handler, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
  ''' If <see langword="false"/> then it is the same as calling Save().
  ''' </param>
  ''' <param name="handler">
  ''' Method called when the operation is complete.
  ''' </param>
  ''' <param name="userState">User state data.</param>
  Public Overridable Sub BeginSave( _
    ByVal forceUpdate As Boolean, _
    ByVal handler As EventHandler(Of Core.SavedEventArgs), _
    ByVal userState As Object)

    If forceUpdate AndAlso IsNew Then
      ' mark the object as old - which makes it
      ' not dirty
      MarkOld()
      ' now mark the object as dirty so it can save
      MarkDirty(True)
    End If

    If Me.IsChild Then
      Dim [error] As New NotSupportedException(My.Resources.NoSaveChildException)
      OnSaved(Nothing, [error], userState)
      If handler IsNot Nothing Then
        handler(Me, New Core.SavedEventArgs(Nothing, [error], userState))
      End If

    ElseIf EditLevel > 0 Then
      Dim [error] As New Validation.ValidationException(My.Resources.NoSaveEditingException)
      OnSaved(Nothing, [error], userState)
      If handler IsNot Nothing Then
        handler(Me, New Core.SavedEventArgs(Nothing, [error], userState))
      End If

    ElseIf (Not IsValid) AndAlso (Not IsDeleted) Then
      Dim [error] As New Validation.ValidationException(My.Resources.NoSaveInvalidException)
      OnSaved(Nothing, [error], userState)
      If handler IsNot Nothing Then
        handler(Me, New Core.SavedEventArgs(Nothing, [error], userState))
      End If

    ElseIf IsBusy Then
      Dim [error] As New Validation.ValidationException(My.Resources.BusyObjectsMayNotBeSaved)
      OnSaved(Nothing, [error], userState)
      If handler IsNot Nothing Then
        handler(Me, New Core.SavedEventArgs(Nothing, [error], userState))
      End If
    Else
      If IsDirty Then
        MarkBusy()
        DataPortal.BeginUpdate(Of T)( _
          Me, AddressOf BeginUpdateComplete, New SaveState(userState, handler))
      Else
        OnSaved(CType(Me, T), Nothing, userState)
        If handler IsNot Nothing Then
          handler(Me, New Core.SavedEventArgs(Me, Nothing, userState))
        End If
      End If
    End If

  End Sub

  Private Sub BeginUpdateComplete( _
    ByVal sender As Object, ByVal e As DataPortalResult(Of T))

    Dim args = CType(e.UserState, SaveState)
    Dim handler = args.Handler
    Dim result As T = e.Object
    OnSaved(result, e.Error, args.UserState)
    If handler IsNot Nothing Then
      handler( _
        result, New Core.SavedEventArgs(result, e.Error, args.UserState))
    End If
    MarkIdle()

  End Sub

  Private Class SaveState

    Public Sub New(ByVal userState As Object, ByVal handler As EventHandler(Of Core.SavedEventArgs))
      _userstate = userState
      _handler = handler
    End Sub

    Private _userstate As Object
    Public Property UserState() As Object
      Get
        Return _userstate
      End Get
      Set(ByVal value As Object)
        _userstate = value
      End Set
    End Property

    Private _handler As EventHandler(Of Core.SavedEventArgs)
    Public Property Handler() As EventHandler(Of Core.SavedEventArgs)
      Get
        Return _handler
      End Get
      Set(ByVal value As EventHandler(Of Core.SavedEventArgs))
        _handler = value
      End Set
    End Property

  End Class

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
  ''' If <see langword="false"/> then it is the same as calling Save().
  ''' </param>
  ''' <remarks>
  ''' This overload is designed for use in web applications
  ''' when implementing the Update method in your 
  ''' data wrapper object.
  ''' </remarks>
  Public Sub BeginSave(ByVal forceUpdate As Boolean)
    Me.BeginSave(False, Nothing)
  End Sub

  ''' <summary>
  ''' Starts an async operation to save the object to the database.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
  ''' If <see langword="false"/> then it is the same as calling Save().
  ''' </param>
  ''' <param name="handler">
  ''' Delegate reference to a callback handler that will
  ''' be invoked when the async operation is complete.
  ''' </param>
  ''' <remarks>
  ''' This overload is designed for use in web applications
  ''' when implementing the Update method in your 
  ''' data wrapper object.
  ''' </remarks>
  Public Sub BeginSave(ByVal forceUpdate As Boolean, ByVal handler As EventHandler(Of Core.SavedEventArgs))
    Me.BeginSave(forceUpdate, handler, Nothing)
  End Sub

  ''' <summary>
  ''' Saves the object to the database, forcing
  ''' IsNew to <see langword="false"/> and IsDirty to True.
  ''' </summary>
  ''' <param name="handler">
  ''' Delegate reference to a callback handler that will
  ''' be invoked when the async operation is complete.
  ''' </param>
  ''' <param name="userState">User state data.</param>
  ''' <remarks>
  ''' This overload is designed for use in web applications
  ''' when implementing the Update method in your 
  ''' data wrapper object.
  ''' </remarks>
  Public Sub BeginSave(ByVal handler As EventHandler(Of Core.SavedEventArgs), ByVal userState As Object)
    Me.BeginSave(False, handler, userState)
  End Sub

#End Region

#Region " ISavable Members "

  Private Function ISavable_Save() As Object Implements Core.ISavable.Save
    Return Save()
  End Function

  Private Sub ISavable_SaveComplete(ByVal newObject As Object) Implements Core.ISavable.SaveComplete
    OnSaved(CType(newObject, T), Nothing, Nothing)
  End Sub

  <NonSerialized()> _
  <NotUndoable()> _
  Private _nonSerializableSavedHandlers As EventHandler(Of Core.SavedEventArgs)
  <NotUndoable()> _
  Private _serializableSavedHandlers As EventHandler(Of Core.SavedEventArgs)

  ''' <summary>
  ''' Event raised when an object has been saved.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")> _
  Public Custom Event Saved As EventHandler(Of Core.SavedEventArgs) Implements Core.ISavable.Saved
    AddHandler(ByVal value As EventHandler(Of Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        _serializableSavedHandlers = CType(System.Delegate.Combine(_serializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      Else
        _nonSerializableSavedHandlers = CType(System.Delegate.Combine(_nonSerializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      End If
    End AddHandler
    RemoveHandler(ByVal value As EventHandler(Of Core.SavedEventArgs))
      If value.Method.IsPublic AndAlso (value.Method.DeclaringType.IsSerializable OrElse value.Method.IsStatic) Then
        _serializableSavedHandlers = CType(System.Delegate.Remove(_serializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      Else
        _nonSerializableSavedHandlers = CType(System.Delegate.Remove(_nonSerializableSavedHandlers, value), EventHandler(Of Core.SavedEventArgs))
      End If
    End RemoveHandler
    RaiseEvent(ByVal sender As System.Object, ByVal e As Core.SavedEventArgs)
      If Not _nonSerializableSavedHandlers Is Nothing Then
        _nonSerializableSavedHandlers.Invoke(Me, e)
      End If
      If Not _serializableSavedHandlers Is Nothing Then
        _serializableSavedHandlers.Invoke(Me, e)
      End If
    End RaiseEvent
  End Event

  ''' <summary>
  ''' Raises the Saved event, indicating that the
  ''' object has been saved, and providing a reference
  ''' to the new object instance.
  ''' </summary>
  ''' <param name="newObject">The new object instance.</param>
  ''' <param name="e">Exception that occurred during operation.</param>
  ''' <param name="userState">User state object.</param>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Sub OnSaved(ByVal newObject As T, ByVal e As Exception, ByVal userState As Object)
    MarkIdle()
    Dim args As New Core.SavedEventArgs(newObject, e, userState)
    RaiseEvent Saved(Me, args)
  End Sub
#End Region

#Region " Register Properties "

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">
  ''' Type of property.
  ''' </typeparam>
  ''' <param name="info">
  ''' PropertyInfo object for the property.
  ''' </param>
  ''' <returns>
  ''' The provided IPropertyInfo object.
  ''' </returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal info As PropertyInfo(Of P)) As PropertyInfo(Of P)

    Return Core.FieldManager.PropertyInfoManager.RegisterProperty(Of P)(GetType(T), info)

  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <returns></returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object))) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="relationship">Relationship with referenced object.</param>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal relationship As RelationshipTypes) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, reflectedPropertyInfo.Name, relationship))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <returns></returns>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <param name="relationship">Default Value for the property</param>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String, ByVal relationship As RelationshipTypes) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName, relationship))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <param name="defaultValue">Default Value for the property</param>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String, ByVal defaultValue As P) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName, defaultValue))
  End Function

  ''' <summary>
  ''' Indicates that the specified property belongs
  ''' to the business object type.
  ''' </summary>
  ''' <typeparam name="P">Type of property</typeparam>
  ''' <param name="propertyLambdaExpression">Property Expression</param>
  ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
  ''' <param name="defaultValue">Default Value for the property</param>
  ''' <param name="relationship">Relationship with referenced object.</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Protected Overloads Shared Function RegisterProperty(Of P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, Object)), ByVal friendlyName As String, ByVal defaultValue As P, ByVal relationship As RelationshipTypes) As PropertyInfo(Of P)
    Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
    Return RegisterProperty(New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName, defaultValue, relationship))
  End Function

#End Region

End Class