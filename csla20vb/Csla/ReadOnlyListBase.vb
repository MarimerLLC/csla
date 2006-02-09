Imports System.ComponentModel
Imports Csla.Core

''' <summary>
''' This is the base class from which readonly collections
''' of readonly objects should be derived.
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class ReadOnlyListBase( _
  Of T As ReadOnlyListBase(Of T, C), C)
  Inherits Core.ReadOnlyBindingList(Of C)

  Implements Csla.Core.IReadOnlyCollection
  Implements ICloneable

#Region " Constructors "

  Protected Sub New()

  End Sub

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
  Public Overloads Function Clone() As T

    Return DirectCast(GetClone(), T)

  End Function

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
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
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
