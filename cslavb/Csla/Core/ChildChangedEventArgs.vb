Imports System.ComponentModel

Namespace Core

  ''' <summary>
  ''' Contains event data about the changed child object.
  ''' </summary>
  Public Class ChildChangedEventArgs
    Inherits EventArgs

    Private _childObject As Object
    ''' <summary>
    ''' Gets a reference to the changed child object.
    ''' </summary>
    Public Property ChildObject() As Object
      Get
        Return _childObject
      End Get
      Private Set(ByVal value As Object)
        _childObject = value
      End Set
    End Property

    Private _propertyChangedArgs As PropertyChangedEventArgs
    ''' <summary>
    ''' Gets the PropertyChangedEventArgs object from the
    ''' child's PropertyChanged event, if the child is
    ''' not a collection or list.
    ''' </summary>
    Public Property PropertyChangedArgs() As PropertyChangedEventArgs
      Get
        Return _propertyChangedArgs
      End Get
      Private Set(ByVal value As PropertyChangedEventArgs)
        _propertyChangedArgs = value
      End Set
    End Property

    Private _listChangedArgs As ListChangedEventArgs
    ''' <summary>
    ''' Gets the ListChangedEventArgs object from the
    ''' child's ListChanged event, if the child is a
    ''' collection or list.
    ''' </summary>
    Public Property ListChangedArgs() As ListChangedEventArgs
      Get
        Return _listChangedArgs
      End Get
      Private Set(ByVal value As ListChangedEventArgs)
        _listChangedArgs = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="source">
    ''' Reference to the object that was changed.
    ''' </param>
    ''' <param name="listArgs">
    ''' ListChangedEventArgs object or null.
    ''' </param>
    ''' <param name="propertyArgs">
    ''' PropertyChangedEventArgs object or null.
    ''' </param>
    Public Sub New(ByVal source As Object, ByVal propertyArgs As PropertyChangedEventArgs, ByVal listArgs As ListChangedEventArgs)
      Me.ChildObject = source
      Me.PropertyChangedArgs = propertyArgs
      Me.ListChangedArgs = listArgs
    End Sub

  End Class

End Namespace
