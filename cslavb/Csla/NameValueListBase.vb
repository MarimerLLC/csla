Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is the base class from which readonly name/value
''' collections should be derived.
''' </summary>
''' <typeparam name="K">Type of the key values.</typeparam>
''' <typeparam name="V">Type of the values.</typeparam>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")> _
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class NameValueListBase(Of K, V)
  Inherits Core.ReadOnlyBindingList(Of NameValueListBase(Of K, V).NameValuePair)

  Implements ICloneable
  Implements Core.IBusinessObject
  Implements Server.IDataPortalTarget

#Region " Core Implementation "

  ''' <summary>
  ''' Returns the value corresponding to the
  ''' specified key.
  ''' </summary>
  ''' <param name="key">Key value for which to retrieve a value.</param>
  Public Function Value(ByVal key As K) As V

    For Each item As NameValuePair In Me
      If item.Key.Equals(key) Then
        Return item.Value
      End If
    Next
    Return Nothing

  End Function

  ''' <summary>
  ''' Returns the key corresponding to the
  ''' first occurance of the specified value
  ''' in the list.
  ''' </summary>
  ''' <param name="value">Value for which to retrieve the key.</param>
  Public Function Key(ByVal value As V) As K

    For Each item As NameValuePair In Me
      If item.Value.Equals(value) Then
        Return item.Key
      End If
    Next
    Return Nothing

  End Function

  ''' <summary>
  ''' Gets a value indicating whether the list contains the
  ''' specified key.
  ''' </summary>
  ''' <param name="key">Key value for which to search.</param>
  Public Function ContainsKey(ByVal key As K) As Boolean

    For Each item As NameValuePair In Me
      If item.Key.Equals(key) Then
        Return True
      End If
    Next
    Return False

  End Function

  ''' <summary>
  ''' Gets a value indicating whether the list contains the
  ''' specified value.
  ''' </summary>
  ''' <param name="value">Value for which to search.</param>
  Public Function ContainsValue(ByVal value As V) As Boolean

    For Each item As NameValuePair In Me
      If item.Value.Equals(value) Then
        Return True
      End If
    Next
    Return False

  End Function

  ''' <summary>
  ''' Get the item for the first matching
  ''' value in the collection.
  ''' </summary>
  ''' <param name="value">
  ''' Value to search for in the list.
  ''' </param>
  ''' <returns>An item from the list.</returns>
  Public Function GetItemByValue(ByVal value As V) As NameValuePair

    For Each item As NameValuePair In Me
      If item IsNot Nothing AndAlso item.Value.Equals(value) Then
        Return item
      End If
    Next
    Return Nothing

  End Function

  ''' <summary>
  ''' Get the item for the first matching
  ''' key in the collection.
  ''' </summary>
  ''' <param name="key">
  ''' Key to search for in the list.
  ''' </param>
  ''' <returns>An item from the list.</returns>
  Public Function GetItemByKey(ByVal key As K) As NameValuePair

    For Each item As NameValuePair In Me
      If item IsNot Nothing AndAlso item.Key.Equals(key) Then
        Return item
      End If
    Next
    Return Nothing

  End Function

#End Region

#Region " Constructors "

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
  ''' <remarks></remarks>
  Protected Sub New()

    Initialize()

  End Sub

#End Region

#Region " Initialize "

  ''' <summary>
  ''' Override this method to set up event handlers so user
  ''' code in a partial class can respond to events raised by
  ''' generated code.
  ''' </summary>
  Protected Overridable Sub Initialize()
    ' allows a generated class to set up events to be
    ' handled by a partial class containing user code
  End Sub

#End Region

#Region " NameValuePair class "

  ''' <summary>
  ''' Contains a key and value pair.
  ''' </summary>
  <Serializable()> _
  Public Class NameValuePair
    Inherits MobileObject

    Private _key As K
    Private _value As V

    ''' <summary>
    ''' The Key or Name value.
    ''' </summary>
    Public ReadOnly Property Key() As K
      Get
        Return _key
      End Get
    End Property

    ''' <summary>
    ''' The Value corresponding to the key/name.
    ''' </summary>
    Public ReadOnly Property Value() As V
      Get
        Return _value
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="key">The key.</param>
    ''' <param name="value">The value.</param>
    Public Sub New(ByVal key As K, ByVal value As V)
      _key = key
      _value = value
    End Sub

    ''' <summary>
    ''' Returns the string representation of the
    ''' value for this item.
    ''' </summary>
    Public Overrides Function ToString() As String
      Return _value.ToString
    End Function

    ''' <summary>
    ''' Override this method to manually get custom field
    ''' values from the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="mode">Serialization mode.</param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      MyBase.OnGetState(info, mode)
      info.AddValue("NameValuePair._key", _key)
      info.AddValue("NameValuePair._value", _value)
    End Sub

    ''' <summary>
    ''' Override this method to manually set custom field
    ''' values into the serialization stream.
    ''' </summary>
    ''' <param name="info">Serialization info.</param>
    ''' <param name="mode">Serialization mode.</param>
    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo, ByVal mode As Core.StateMode)
      MyBase.OnSetState(info, mode)
      _key = info.GetValue(Of K)("NameValuePair._key")
      _value = info.GetValue(Of V)("NameValuePair._value")
    End Sub

  End Class

#End Region

#Region " ICloneable "

  Private Function ICloneable_Clone() As Object Implements ICloneable.Clone

    Return GetClone()

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>A new object containing the exact data of the original object.</returns>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Function GetClone() As Object

    Return ObjectCloner.Clone(Me)

  End Function

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overloads Function Clone() As NameValueListBase(Of K, V)

    Return DirectCast(GetClone(), NameValueListBase(Of K, V))

  End Function

#End Region

#Region " Criteria "

  ''' <summary>
  ''' Default Criteria for retrieving simple
  ''' name/value lists.
  ''' </summary>
  ''' <remarks>
  ''' This criteria merely specifies the type of
  ''' collection to be retrieved. That type information
  ''' is used by the DataPortal to create the correct
  ''' type of collection object during data retrieval.
  ''' </remarks>
  <Serializable()> _
  Protected Class Criteria
    Inherits CriteriaBase

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="collectionType">
    ''' The <see cref="Type"/> of the business
    ''' collection class.
    ''' </param>
    Public Sub New(ByVal collectionType As Type)
      MyBase.New(collectionType)
    End Sub

    'TODO: Do we need to implement the default ctor?

  End Class

#End Region

#Region " Data Access "

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  Private Sub DataPortal_Create(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.CreateNotSupportedException)
  End Sub

  ''' <summary>
  ''' Override this method to allow retrieval of an existing business
  ''' object based on data in the database.
  ''' </summary>
  ''' <param name="criteria">An object containing criteria values to identify the object.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Fetch(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.FetchNotSupportedException)
  End Sub

  Private Sub DataPortal_Update()
    Throw New NotSupportedException(My.Resources.UpdateNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  Private Sub DataPortal_Delete(ByVal criteria As Object)
    Throw New NotSupportedException(My.Resources.DeleteNotSupportedException)
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub IDataPortalTarget_DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

  End Sub

#End Region

#Region " IDataPortalTarget implementation "

  Private Sub CheckRules() Implements Server.IDataPortalTarget.CheckRules

  End Sub

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

  Private Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke
    IDataPortalTarget_DataPortal_OnDataPortalInvoke(e)
  End Sub

  Private Sub IDataPortalTarget_DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete
    Me.DataPortal_OnDataPortalInvokeComplete(e)
  End Sub

  Private Sub IDataPortalTarget_DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException
    Me.DataPortal_OnDataPortalException(e, ex)
  End Sub

  Private Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke

  End Sub

  Private Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete

  End Sub

  Private Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As System.Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException

  End Sub

#End Region

End Class
