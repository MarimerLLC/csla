Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is the base class from which readonly name/value
''' collections should be derived.
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")> _
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class NameValueListBase(Of K, V)
  Inherits Core.ReadOnlyBindingList(Of NameValuePair)

  Implements ICloneable
  Implements Core.IBusinessObject

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
  Public Function ContainsValue(ByVal value As V) As Boolean

    For Each item As NameValuePair In Me
      If item.Value.Equals(value) Then
        Return True
      End If
    Next
    Return False

  End Function

#End Region

#Region " Constructors "

  Protected Sub New()

  End Sub

#End Region

#Region " NameValuePair class "

  ''' <summary>
  ''' Contains a key and value pair.
  ''' </summary>
  <Serializable()> _
  Public Class NameValuePair

    Private mKey As K
    Private mValue As V

    ''' <summary>
    ''' The Key or Name value.
    ''' </summary>
    Public ReadOnly Property Key() As K
      Get
        Return mKey
      End Get
    End Property

    ''' <summary>
    ''' The Value corresponding to the key/name.
    ''' </summary>
    Public Property Value() As V
      Get
        Return mValue
      End Get
      Set(ByVal value As V)
        mValue = value
      End Set
    End Property

    Public Sub New(ByVal key As K, ByVal value As V)
      mKey = key
      mValue = value
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
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
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

    Public Sub New(ByVal collectionType As Type)
      MyBase.New(collectionType)
    End Sub
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
  ''' <param name="Criteria">An object containing criteria values to identify the object.</param>
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
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

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

End Class
