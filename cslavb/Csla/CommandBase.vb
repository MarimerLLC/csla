Imports System
Imports System.ComponentModel
#If Not SILVERLIGHT Then
Imports Csla.Server
#End If
Imports Csla.Core
Imports Csla.Serialization.Mobile
Imports Csla.Serialization
Imports Csla.DataPortalClient

''' <summary>
''' This is the base class from which command 
''' objects will be derived.
''' </summary>
''' <remarks>
''' <para>
''' Command objects allow the execution of arbitrary server-side
''' functionality. Most often, this involves the invocation of
''' a stored procedure in the database, but can involve any other
''' type of stateless, atomic call to the server instead.
''' </para><para>
''' To implement a command object, inherit from CommandBase and
''' override the DataPortal_Execute method. In this method you can
''' implement any server-side code as required.
''' </para><para>
''' To pass data to/from the server, use instance variables within
''' the command object itself. The command object is instantiated on
''' the client, and is passed by value to the server where the 
''' DataPortal_Execute method is invoked. The command object is then
''' returned to the client by value.
''' </para>
''' </remarks>
<Serializable()> _
Public MustInherit Class CommandBase
  Inherits ManagedObjectBase

  Implements Core.ICommandObject
  Implements Server.IDataPortalTarget

#Region " Constructors "

  ''' <summary>
  ''' Creates an instance of the object.
  ''' </summary>
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

#Region " Data access "

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Create(ByVal criteria As Object)
    Throw New NotSupportedException(Csla.Resources.CreateNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Fetch(ByVal criteria As Object)
    Throw New NotSupportedException(Csla.Resources.FetchNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Update()
    Throw New NotSupportedException(Csla.Resources.UpdateNotSupportedException)
  End Sub

  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId:="criteria")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")> _
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")> _
  Private Sub DataPortal_Delete(ByVal criteria As Object)
    Throw New NotSupportedException(Csla.Resources.DeleteNotSupportedException)
  End Sub

  ''' <summary>
  ''' Override this method to implement any server-side code
  ''' that is to be run when the command is executed.
  ''' </summary>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  Protected Overridable Sub DataPortal_Execute()
    Throw New NotSupportedException(Csla.Resources.ExecuteNotSupportedException)
  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal prior to calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvoke( _
    ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal after calling the 
  ''' requested DataPortal_xyz method.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalInvokeComplete( _
    ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete

  End Sub

  ''' <summary>
  ''' Called by the server-side DataPortal if an exception
  ''' occurs during server-side processing.
  ''' </summary>
  ''' <param name="e">The DataPortalContext object passed to the DataPortal.</param>
  ''' <param name="ex">The Exception thrown during processing.</param>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId:="Member")> _
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Protected Overridable Sub DataPortal_OnDataPortalException( _
    ByVal e As DataPortalEventArgs, ByVal ex As Exception) Implements Server.IDataPortalTarget.DataPortal_OnDataPortalException

  End Sub

#End Region

#Region " IDataPortalTarget implementation "

  Private Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As System.Exception) Implements Server.IDataPortalTarget.Child_OnDataPortalException

  End Sub

  Private Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvoke

  End Sub

  Private Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs) Implements Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete

  End Sub

  Private Sub MarkAsChild() Implements Server.IDataPortalTarget.MarkAsChild

  End Sub

  Private Sub MarkNew() Implements Server.IDataPortalTarget.MarkNew

  End Sub

  Private Sub MarkOld() Implements Server.IDataPortalTarget.MarkOld

  End Sub

#End Region

End Class
