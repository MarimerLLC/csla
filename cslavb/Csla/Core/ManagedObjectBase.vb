Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Text
Imports Csla.Core.FieldManager
Imports Csla.Core.LoadManager
Imports System.ComponentModel
Imports Csla.Reflection
Imports Csla.Serialization.Mobile
Imports Csla.Serialization

Namespace Core

  ''' <summary>
  ''' Base class for an object that is serializable
  ''' using MobileFormatter.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public MustInherit Class ManagedObjectBase
    Inherits MobileObject
    Implements INotifyPropertyChanged

#Region "Field Manager"

    Private _fieldManager As FieldDataManager

    ''' <summary>
    '''  Gets a reference to the field mananger
    ''' for this object.
    ''' </summary>
    Protected ReadOnly Property FieldManager() As FieldDataManager
      Get
        If _fieldManager Is Nothing Then
          _fieldManager = New FieldDataManager([GetType]())
        End If
        Return _fieldManager
      End Get
    End Property

#End Region

#Region "Register Properties"

    ''' <summary> 
    ''' Indicates that the specified property belongs 
    ''' to the type. 
    ''' </summary> 
    ''' <typeparam name="P"> 
    ''' Type of property. 
    ''' </typeparam> 
    ''' <param name="objectType"> 
    ''' Type of object to which the property belongs. 
    ''' </param> 
    ''' <param name="info"> 
    ''' PropertyInfo object for the property. 
    ''' </param> 
    ''' <returns> 
    ''' The provided IPropertyInfo object. 
    ''' </returns> 
    Protected Shared Function RegisterProperty(Of P)(ByVal objectType As Type, ByVal info As PropertyInfo(Of P)) As PropertyInfo(Of P)
      Return Core.FieldManager.PropertyInfoManager.RegisterProperty(Of P)(objectType, info)
    End Function

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the business object type.
    ''' </summary>
    ''' <typeparam name="T">Type of object to which the property belongs.</typeparam>
    ''' <typeparam name="P">Type of property</typeparam>
    ''' <param name="propertyLambdaExpression">Property Expression</param>
    ''' <returns>The provided IPropertyInfo object.</returns>
    Protected Shared Function RegisterProperty(Of T, P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, P))) As PropertyInfo(Of P)
      Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
      Return RegisterProperty(GetType(T), New PropertyInfo(Of P)(reflectedPropertyInfo.Name))
    End Function

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the business object type.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="P">Type of property</typeparam>
    ''' <param name="propertyLambdaExpression">Property Expression</param>
    ''' <param name="friendlyName">Friendly description for a property to be used in databinding</param>
    ''' <returns>The provided IPropertyInfo object.</returns>
    Protected Shared Function RegisterProperty(Of T, P)(ByVal propertyLambdaExpression As Expression(Of Func(Of T, P)), ByVal friendlyName As String) As PropertyInfo(Of P)
      Dim reflectedPropertyInfo As PropertyInfo = Reflect(Of T).GetProperty(propertyLambdaExpression)
      Return RegisterProperty(GetType(T), New PropertyInfo(Of P)(reflectedPropertyInfo.Name, friendlyName))
    End Function

#End Region

#Region "Read Properties"

    ''' <summary> 
    ''' Gets a property's value from the list of 
    ''' managed field values, converting the 
    ''' value to an appropriate type. 
    ''' </summary> 
    ''' <typeparam name="F"> 
    ''' Type of the field. 
    ''' </typeparam> 
    ''' <typeparam name="P"> 
    ''' Type of the property. 
    ''' </typeparam> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    Protected Function ReadPropertyConvert(Of F, P)(ByVal propertyInfo As PropertyInfo(Of F)) As P
      Return Utilities.CoerceValue(Of P)(GetType(F), Nothing, ReadProperty(Of F)(propertyInfo))
    End Function

    ''' <summary> 
    ''' Gets a property's value as a specified type. 
    ''' </summary> 
    ''' <typeparam name="P"> 
    ''' Type of the property. 
    ''' </typeparam> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    Protected Function ReadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P)) As P
      Dim result As P = Nothing
      Dim data As FieldManager.IFieldData = FieldManager.GetFieldData(propertyInfo)
      If data IsNot Nothing Then
        Dim fd As FieldManager.IFieldData(Of P) = TryCast(data, FieldManager.IFieldData(Of P))
        If fd IsNot Nothing Then
          result = fd.Value
        Else
          result = DirectCast(data.Value, P)
        End If
      Else
        result = propertyInfo.DefaultValue
        FieldManager.LoadFieldData(Of P)(propertyInfo, result)
      End If
      Return result
    End Function

    ''' <summary> 
    ''' Gets a property's value as a specified type. 
    ''' </summary> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    Protected Function ReadProperty(ByVal propertyInfo As IPropertyInfo) As Object
      Dim info = FieldManager.GetFieldData(propertyInfo)
      If info IsNot Nothing Then
        Return info.Value
      Else
        Return Nothing
      End If
    End Function

#End Region

#Region "Load Properties"

    ''' <summary> 
    ''' Loads a property's managed field with the 
    ''' supplied value calling PropertyHasChanged 
    ''' if the value does change. 
    ''' </summary> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    ''' <param name="newValue"> 
    ''' The new value for the property.</param> 
    ''' <remarks> 
    ''' No authorization checks occur when this method is called, 
    ''' and no PropertyChanging or PropertyChanged events are raised. 
    ''' Loading values does not cause validation rules to be 
    ''' invoked. 
    ''' </remarks> 
    Protected Sub LoadPropertyConvert(Of P, F)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As F)
      Try
        Dim oldValue As P = Nothing
        Dim fieldData = FieldManager.GetFieldData(propertyInfo)
        If fieldData Is Nothing Then
          oldValue = propertyInfo.DefaultValue
          fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)
        Else
          Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
          If fd IsNot Nothing Then
            oldValue = fd.Value
          Else
            oldValue = DirectCast(fieldData.Value, P)
          End If
        End If
        LoadPropertyValue(Of P)(propertyInfo, oldValue, Utilities.CoerceValue(Of P)(GetType(F), oldValue, newValue), False)
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
      End Try
    End Sub

    ''' <summary> 
    ''' Loads a property's managed field with the 
    ''' supplied value calling PropertyHasChanged 
    ''' if the value does change. 
    ''' </summary> 
    ''' <typeparam name="P"> 
    ''' Type of the property. 
    ''' </typeparam> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    ''' <param name="newValue"> 
    ''' The new value for the property.</param> 
    ''' <remarks> 
    ''' No authorization checks occur when this method is called, 
    ''' and no PropertyChanging or PropertyChanged events are raised. 
    ''' Loading values does not cause validation rules to be 
    ''' invoked. 
    ''' </remarks> 
    Protected Sub LoadProperty(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal newValue As P)
      Try
        Dim oldValue As P = Nothing
        Dim fieldData = FieldManager.GetFieldData(propertyInfo)
        If fieldData Is Nothing Then
          oldValue = propertyInfo.DefaultValue
          fieldData = FieldManager.LoadFieldData(Of P)(propertyInfo, oldValue)
        Else
          Dim fd = TryCast(fieldData, FieldManager.IFieldData(Of P))
          If fd IsNot Nothing Then
            oldValue = fd.Value
          Else
            oldValue = DirectCast(fieldData.Value, P)
          End If
        End If
        LoadPropertyValue(Of P)(propertyInfo, oldValue, newValue, False)
      Catch ex As Exception
        Throw New PropertyLoadException(String.Format(My.Resources.PropertyLoadException, propertyInfo.Name, ex.Message))
      End Try
    End Sub

    Private Sub LoadPropertyValue(Of P)(ByVal propertyInfo As PropertyInfo(Of P), ByVal oldValue As P, ByVal newValue As P, ByVal markDirty As Boolean)
      Dim valuesDiffer = False
      If oldValue Is Nothing Then
        valuesDiffer = newValue IsNot Nothing
      Else
        valuesDiffer = Not (oldValue.Equals(newValue))
      End If

      If valuesDiffer Then
        FieldManager.LoadFieldData(Of P)(propertyInfo, newValue)
      End If
    End Sub

    ''' <summary> 
    ''' Loads a property's managed field with the 
    ''' supplied value calling PropertyHasChanged 
    ''' if the value does change. 
    ''' </summary> 
    ''' <param name="propertyInfo"> 
    ''' PropertyInfo object containing property metadata.</param> 
    ''' <param name="newValue"> 
    ''' The new value for the property.</param> 
    ''' <remarks> 
    ''' No authorization checks occur when this method is called, 
    ''' and no PropertyChanging or PropertyChanged events are raised. 
    ''' Loading values does not cause validation rules to be 
    ''' invoked. 
    ''' </remarks> 
    Protected Sub LoadProperty(ByVal propertyInfo As IPropertyInfo, ByVal newValue As Object)
      FieldManager.LoadFieldData(propertyInfo, newValue)
    End Sub

#End Region

#Region "INotifyPropertyChanged Members"

    <NonSerialized()> _
    <NotUndoable()> _
    Private _propertyChanged As PropertyChangedEventHandler

    Private Custom Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
      AddHandler(ByVal value As PropertyChangedEventHandler)
        _propertyChanged = DirectCast([Delegate].Combine(_propertyChanged, value), PropertyChangedEventHandler)
      End AddHandler
      RemoveHandler(ByVal value As PropertyChangedEventHandler)
        _propertyChanged = DirectCast([Delegate].Remove(_propertyChanged, value), PropertyChangedEventHandler)
      End RemoveHandler
      RaiseEvent(ByVal sender As Object, ByVal e As PropertyChangedEventArgs)
        'TODO: This was a throw exception but I think this is correct
        If _propertyChanged IsNot Nothing Then
          _propertyChanged.Invoke(sender, e)
        End If
      End RaiseEvent
    End Event

    ''' <summary>
    ''' Raises the PropertyChanged event.
    ''' </summary>
    ''' <param name="propertyName">Name of the changed property.</param>
    Protected Sub OnPropertyChanged(ByVal propertyName As String)
      'TODO: I also changed this from the code that is now in the RaiseEvent to this
      RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

#End Region

#Region "MobileObject"

    ''' <summary>
    ''' Override this method to manually retrieve child
    ''' object data from the serializations stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="formatter">Reference to the MobileFormatter.</param>
    Protected Overloads Overrides Sub OnGetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      If _fieldManager IsNot Nothing Then
        Dim child As SerializationInfo = formatter.SerializeObject(_fieldManager)
        info.AddChild("_fieldManager", child.ReferenceId)
      End If

      MyBase.OnGetChildren(info, formatter)
    End Sub

    ''' <summary>
    ''' Override this method to manually serialize child
    ''' objects into the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="formatter">Reference to the MobileFormatter.</param>
    Protected Overloads Overrides Sub OnSetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter)
      If info.Children.ContainsKey("_fieldManager") Then
        Dim referenceId As Integer = info.Children("_fieldManager").ReferenceId
        _fieldManager = DirectCast(formatter.GetObject(referenceId), FieldDataManager)
      End If

      MyBase.OnSetChildren(info, formatter)
    End Sub

#End Region
  End Class
End Namespace