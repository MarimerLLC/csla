Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web

  ''' <summary>
  ''' A Web Forms data binding control designed to support
  ''' CSLA .NET business objects as data sources.
  ''' </summary>
  <Designer(GetType(Csla.Web.Design.CslaDataSourceDesigner))> _
  <ToolboxData("<{0}:CslaDataSource runat=""server""></{0}:CslaDataSource>")> _
  Public Class CslaDataSource
    Inherits DataSourceControl

    Private WithEvents mDefaultView As CslaDataSourceView

    ''' <summary>
    ''' Event raised when an object is to be created and
    ''' populated with data.
    ''' </summary>
    ''' <remarks>Handle this event in a page and set
    ''' e.BusinessObject to the populated business object.
    ''' </remarks>
    Public Event SelectObject As EventHandler(Of SelectObjectArgs)

    ''' <summary>
    ''' Event raised when an object is to be populated with data
    ''' and inserted.
    ''' </summary>
    ''' <remarks>Handle this event in a page to create an
    ''' instance of the object, load the object with data and
    ''' insert the object into the database.</remarks>
    Public Event InsertObject As EventHandler(Of InsertObjectArgs)

    ''' <summary>
    ''' Event raised when an object is to be updated.
    ''' </summary>
    ''' <remarks>Handle this event in a page to update an
    ''' existing instance of an object with new data and then
    ''' save the object into the database.</remarks>
    Public Event UpdateObject As EventHandler(Of UpdateObjectArgs)

    ''' <summary>
    ''' Event raised when an object is to be deleted.
    ''' </summary>
    ''' <remarks>Handle this event in a page to delete
    ''' an object from the database.</remarks>
    Public Event DeleteObject As EventHandler(Of DeleteObjectArgs)

    ''' <summary>
    ''' Returns the default view for this data control.
    ''' </summary>
    ''' <param name="viewName">Ignored.</param>
    ''' <returns></returns>
    ''' <remarks>This control only contains a "Default" view.</remarks>
    Protected Overrides Function GetView(ByVal viewName As String) As System.Web.UI.DataSourceView

      If mDefaultView Is Nothing Then
        mDefaultView = New CslaDataSourceView(Me, "Default")
      End If
      Return mDefaultView

    End Function

    ''' <summary>
    ''' Get or set the name of the assembly containing the 
    ''' business object class to be used as a data source.
    ''' </summary>
    ''' <value>Assembly name containing the business class.</value>
    Public Property TypeAssemblyName() As String
      Get
        Return CType(Me.GetView("Default"), CslaDataSourceView).TypeAssemblyName
      End Get
      Set(ByVal value As String)
        CType(Me.GetView("Default"), CslaDataSourceView).TypeAssemblyName = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set the full type name of the business object
    ''' class to be used as a data source.
    ''' </summary>
    ''' <value>Full type name of the business class.</value>
    Public Property TypeName() As String
      Get
        Return CType(Me.GetView("Default"), CslaDataSourceView).TypeName
      End Get
      Set(ByVal value As String)
        CType(Me.GetView("Default"), CslaDataSourceView).TypeName = value
      End Set
    End Property

    ''' <summary>
    ''' Returns a <see cref="Type">Type</see> object based on the
    ''' assembly and type information provided.
    ''' </summary>
    ''' <param name="assemblyName">(Optional) Assembly name containing the type.</param>
    ''' <param name="typeName">Full type name of the class.</param>
    ''' <remarks></remarks>
    Friend Overloads Shared Function [GetType]( _
      ByVal assemblyName As String, ByVal typeName As String) As Type

      If Len(assemblyName) > 0 Then
        Dim asm As Assembly = Assembly.Load(assemblyName)
        Return asm.GetType(typeName, True, True)

      Else
        Return Type.GetType(typeName, True, True)
      End If

    End Function

    ''' <summary>
    ''' Returns a list of views available for this control.
    ''' </summary>
    ''' <remarks>This control only provides the "Default" view.</remarks>
    Protected Overrides Function GetViewNames() As System.Collections.ICollection

      Return New String() {"Default"}

    End Function

    ''' <summary>
    ''' Raises the SelectObject event.
    ''' </summary>
    Friend Sub OnSelectObject(ByVal e As SelectObjectArgs)

      RaiseEvent SelectObject(Me, e)

    End Sub

    ''' <summary>
    ''' Raises the InsertObject event.
    ''' </summary>
    Friend Sub OnInsertObject(ByVal e As InsertObjectArgs)

      RaiseEvent InsertObject(Me, e)

    End Sub

    ''' <summary>
    ''' Raises the UpdateObject event.
    ''' </summary>
    Friend Sub OnUpdateObject(ByVal e As UpdateObjectArgs)

      RaiseEvent UpdateObject(Me, e)

    End Sub

    ''' <summary>
    ''' Raises the DeleteObject event.
    ''' </summary>
    Friend Sub OnDeleteObject(ByVal e As DeleteObjectArgs)

      RaiseEvent DeleteObject(Me, e)

    End Sub

  End Class

  ''' <summary>
  ''' Argument object used in the SelectObject event.
  ''' </summary>
  Public Class SelectObjectArgs
    Inherits EventArgs

    Private mBusinessObject As Object

    ''' <summary>
    ''' Get or set a reference to the business object
    ''' that is created and populated by the SelectObject
    ''' event handler in the web page.
    ''' </summary>
    ''' <value>A reference to a CSLA .NET business object.</value>
    Public Property BusinessObject() As Object
      Get
        Return mBusinessObject
      End Get
      Set(ByVal value As Object)
        mBusinessObject = value
      End Set
    End Property

  End Class

  ''' <summary>
  ''' Argument object used in the InsertObject event.
  ''' </summary>
  Public Class InsertObjectArgs
    Inherits EventArgs

    Private mValues As System.Collections.IDictionary
    Private mRowsAffected As Integer

    ''' <summary>
    ''' Gets or sets the number of rows affected
    ''' while handling this event.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' The code handling the event should set this
    ''' value to indicate the number of rows affected
    ''' by the operation.
    ''' </remarks>
    Public Property RowsAffected() As Integer
      Get
        Return mRowsAffected
      End Get
      Set(ByVal value As Integer)
        mRowsAffected = value
      End Set
    End Property

    ''' <summary>
    ''' The list of data values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property Values() As System.Collections.IDictionary
      Get
        Return mValues
      End Get
    End Property

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    Public Sub New(ByVal values As System.Collections.IDictionary)

      mValues = values

    End Sub

  End Class

  ''' <summary>
  ''' Argument object used in the UpdateObject event.
  ''' </summary>
  Public Class UpdateObjectArgs
    Inherits EventArgs

    Private mKeys As System.Collections.IDictionary
    Private mValues As System.Collections.IDictionary
    Private mOldValues As System.Collections.IDictionary
    Private mRowsAffected As Integer

    ''' <summary>
    ''' Gets or sets the number of rows affected
    ''' while handling this event.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' The code handling the event should set this
    ''' value to indicate the number of rows affected
    ''' by the operation.
    ''' </remarks>
    Public Property RowsAffected() As Integer
      Get
        Return mRowsAffected
      End Get
      Set(ByVal value As Integer)
        mRowsAffected = value
      End Set
    End Property

    ''' <summary>
    ''' The list of key values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property Keys() As System.Collections.IDictionary
      Get
        Return mKeys
      End Get
    End Property

    ''' <summary>
    ''' The list of data values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property Values() As System.Collections.IDictionary
      Get
        Return mValues
      End Get
    End Property

    ''' <summary>
    ''' The list of old data values maintained by
    ''' data binding.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to take the list of values, put them
    ''' into a business object and to save that object
    ''' into the database.</remarks>
    Public ReadOnly Property OldValues() As System.Collections.IDictionary
      Get
        Return mOldValues
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New(ByVal keys As System.Collections.IDictionary, ByVal values As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary)

      mKeys = keys
      mValues = values
      mOldValues = oldValues

    End Sub

  End Class

  ''' <summary>
  ''' Argument object used in the DeleteObject event.
  ''' </summary>
  Public Class DeleteObjectArgs
    Inherits EventArgs

    Private mKeys As System.Collections.IDictionary
    Private mOldValues As System.Collections.IDictionary
    Private mRowsAffected As Integer

    ''' <summary>
    ''' Gets or sets the number of rows affected
    ''' while handling this event.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' The code handling the event should set this
    ''' value to indicate the number of rows affected
    ''' by the operation.
    ''' </remarks>
    Public Property RowsAffected() As Integer
      Get
        Return mRowsAffected
      End Get
      Set(ByVal value As Integer)
        mRowsAffected = value
      End Set
    End Property

    ''' <summary>
    ''' The list of key values entered by the user.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to use the values to identify the 
    ''' object to be deleted.</remarks>
    Public ReadOnly Property Keys() As System.Collections.IDictionary
      Get
        Return mKeys
      End Get
    End Property

    ''' <summary>
    ''' The list of old data values maintained by
    ''' data binding.
    ''' </summary>
    ''' <remarks>It is up to the event handler in the
    ''' web page to use the values to identify the 
    ''' object to be deleted.</remarks>
    Public ReadOnly Property OldValues() As System.Collections.IDictionary
      Get
        Return mOldValues
      End Get
    End Property

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    Public Sub New(ByVal keys As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary)

      mKeys = keys
      mOldValues = oldValues

    End Sub

  End Class

End Namespace
