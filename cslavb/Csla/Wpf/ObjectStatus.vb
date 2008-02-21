Imports System.Windows
Imports System.ComponentModel
Imports Csla.Core

Namespace Wpf

  ''' <summary>
  ''' Container for other UI controls that exposes
  ''' various status values from the CSLA .NET
  ''' business object acting as DataContext.
  ''' </summary>
  ''' <remarks>
  ''' This control provides access to the IsDirty,
  ''' IsNew, IsDeleted, IsValid and IsSavable properties
  ''' of a business object. The purpose behind this
  ''' control is to expose those properties in a way
  ''' that supports WFP data binding against those
  ''' values.
  ''' </remarks>
  Public Class ObjectStatus
    Inherits DataDecoratorBase

#Region "Dependency Properties"

    Private Shared ReadOnly IsDeletedProperty As DependencyProperty = DependencyProperty.Register("IsDeleted", GetType(Boolean), GetType(ObjectStatus), New FrameworkPropertyMetadata(False), Nothing)
    Private Shared ReadOnly IsDirtyProperty As DependencyProperty = DependencyProperty.Register("IsDirty", GetType(Boolean), GetType(ObjectStatus), New FrameworkPropertyMetadata(False), Nothing)
    Private Shared ReadOnly IsNewProperty As DependencyProperty = DependencyProperty.Register("IsNew", GetType(Boolean), GetType(ObjectStatus), New FrameworkPropertyMetadata(False), Nothing)
    Private Shared ReadOnly IsSavableProperty As DependencyProperty = DependencyProperty.Register("IsSavable", GetType(Boolean), GetType(ObjectStatus), New FrameworkPropertyMetadata(False), Nothing)
    Private Shared ReadOnly IsValidProperty As DependencyProperty = DependencyProperty.Register("IsValid", GetType(Boolean), GetType(ObjectStatus), New FrameworkPropertyMetadata(False), Nothing)

    ''' <summary>
    ''' Exposes the IsDeleted property of the
    ''' DataContext business object.
    ''' </summary>
    Public Property IsDeleted() As Boolean
      Get
        Return CBool(MyBase.GetValue(IsDeletedProperty))
      End Get
      Set(ByVal value As Boolean)
        Dim old As Boolean = IsDeleted
        MyBase.SetValue(IsDeletedProperty, value)
        OnPropertyChanged(New DependencyPropertyChangedEventArgs(IsDeletedProperty, old, value))
      End Set
    End Property

    ''' <summary>
    ''' Exposes the IsDirty property of the
    ''' DataContext business object.
    ''' </summary>
    Public Property IsDirty() As Boolean
      Get
        Return CBool(MyBase.GetValue(IsDirtyProperty))
      End Get
      Set(ByVal value As Boolean)
        Dim old As Boolean = IsDirty
        MyBase.SetValue(IsDirtyProperty, value)
        If old <> value Then
          OnPropertyChanged(New DependencyPropertyChangedEventArgs(IsDirtyProperty, old, value))
        End If
      End Set
    End Property

    ''' <summary>
    ''' Exposes the IsNew property of the
    ''' DataContext business object.
    ''' </summary>
    Public Property IsNew() As Boolean
      Get
        Return CBool(MyBase.GetValue(IsNewProperty))
      End Get
      Set(ByVal value As Boolean)
        Dim old As Boolean = IsNew
        MyBase.SetValue(IsNewProperty, value)
        If old <> value Then
          OnPropertyChanged(New DependencyPropertyChangedEventArgs(IsNewProperty, old, value))
        End If
      End Set
    End Property

    ''' <summary>
    ''' Exposes the IsSavable property of the
    ''' DataContext business object.
    ''' </summary>
    Public Property IsSavable() As Boolean
      Get
        Return CBool(MyBase.GetValue(IsSavableProperty))
      End Get
      Set(ByVal value As Boolean)
        Dim old As Boolean = IsSavable
        MyBase.SetValue(IsSavableProperty, value)
        If old <> value Then
          OnPropertyChanged(New DependencyPropertyChangedEventArgs(IsSavableProperty, old, value))
        End If
      End Set
    End Property

    ''' <summary>
    ''' Exposes the IsValid property of the
    ''' DataContext business object.
    ''' </summary>
    Public Property IsValid() As Boolean
      Get
        Return CBool(MyBase.GetValue(IsValidProperty))
      End Get
      Set(ByVal value As Boolean)
        Dim old As Boolean = IsValid
        MyBase.SetValue(IsValidProperty, value)
        If old <> value Then
          OnPropertyChanged(New DependencyPropertyChangedEventArgs(IsValidProperty, old, value))
        End If
      End Set
    End Property

#End Region

    ''' <summary>
    ''' This method is called when the data
    ''' object to which the control is bound
    ''' has changed.
    ''' </summary>
    Protected Overrides Sub DataObjectChanged()
      Refresh()
    End Sub

    ''' <summary>
    ''' This method is called when a property
    ''' of the data object to which the 
    ''' control is bound has changed.
    ''' </summary>
    Protected Overrides Sub DataPropertyChanged(ByVal e As PropertyChangedEventArgs)
      Refresh()
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an IBindingList, and the 
    ''' ListChanged event was raised by
    ''' the data object.
    ''' </summary>
    Protected Overrides Sub DataBindingListChanged(ByVal e As ListChangedEventArgs)
      Refresh()
    End Sub

    ''' <summary>
    ''' This method is called if the data
    ''' object is an INotifyCollectionChanged, 
    ''' and the CollectionChanged event was 
    ''' raised by the data object.
    ''' </summary>
    Protected Overrides Sub DataObservableCollectionChanged(ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
      Refresh()
    End Sub

    ''' <summary>
    ''' Refreshes the control's property
    ''' values to reflect the values of
    ''' the underlying business object.
    ''' </summary>
    Public Sub Refresh()
      Dim source As IEditableBusinessObject = TryCast(DataObject, IEditableBusinessObject)
      If Not source Is Nothing Then
        If IsDeleted <> source.IsDeleted Then
          IsDeleted = source.IsDeleted
        End If
        If IsDirty <> source.IsDirty Then
          IsDirty = source.IsDirty
        End If
        If IsNew <> source.IsNew Then
          IsNew = source.IsNew
        End If
        If IsSavable <> source.IsSavable Then
          IsSavable = source.IsSavable
        End If
        If IsValid <> source.IsValid Then
          IsValid = source.IsValid
        End If
      Else
        Dim sourceList As IEditableCollection = TryCast(DataObject, IEditableCollection)
        If Not sourceList Is Nothing Then
          If IsDirty <> sourceList.IsDirty Then
            IsDirty = sourceList.IsDirty
          End If
          If IsValid <> sourceList.IsValid Then
            IsValid = sourceList.IsValid
          End If
          If IsSavable <> sourceList.IsSavable Then
            IsSavable = sourceList.IsSavable
          End If
          IsDeleted = False
          IsNew = False
        End If
      End If
    End Sub

  End Class

End Namespace