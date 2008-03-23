Imports System.ComponentModel
Imports Csla.Core
Imports System.Runtime.CompilerServices
Imports System.Linq
Imports System.Linq.Expressions

''' <summary>
''' This is the base class from which readonly collections
''' of readonly objects should be derived.
''' </summary>
''' <typeparam name="T">Type of the list class.</typeparam>
''' <typeparam name="C">Type of child objects contained in the list.</typeparam>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
<Serializable()> _
Public MustInherit Class ReadOnlyListBase( _
  Of T As ReadOnlyListBase(Of T, C), C)
  Inherits Core.ReadOnlyBindingList(Of C)

  Implements Csla.Core.IReadOnlyCollection
  Implements ICloneable
  Implements Server.IDataPortalTarget

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
  Protected Overridable Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_XYZ method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during data access.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during data access.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member"), EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException

  End Sub

#End Region

#Region " ToArray "

  ''' <summary>
  ''' Get an array containing all items in the list.
  ''' </summary>
  Public Function ToArray() As C()

    Dim result As New List(Of C)
    For Each Item As C In Me
      result.Add(Item)
    Next
    Return result.ToArray

  End Function

#End Region

#Region " IDataPortalTarget implementation "

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

#End Region

End Class

Public Module ReadOnlyListBaseExtension
  <Extension()> _
  Public Function Where(Of T As ReadOnlyListBase(Of T, C), C As Core.IReadOnlyObject)(ByVal source As ReadOnlyListBase(Of T, C), ByVal expr As Expression(Of Func(Of C, Boolean))) As IEnumerable(Of C)
    Return source.SearchByExpression(expr)
  End Function

End Module



