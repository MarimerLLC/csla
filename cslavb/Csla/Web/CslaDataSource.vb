#If Not CLIENTONLY Then
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
  <DisplayName("CslaDataSource")> _
  <Description("CSLA .NET Data Source Control")> _
  <ToolboxData("<{0}:CslaDataSource runat=""server""></{0}:CslaDataSource>")> _
  Public Class CslaDataSource
    Inherits DataSourceControl

    Private WithEvents _defaultView As CslaDataSourceView

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

      If _defaultView Is Nothing Then
        _defaultView = New CslaDataSourceView(Me, "Default")
      End If
      Return _defaultView

    End Function

    ''' <summary>
    ''' Get or set the name of the assembly (no longer used).
    ''' </summary>
    ''' <value>Obsolete - do not use.</value>
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
    ''' <value>Full type name of the business class,
    ''' including assembly name.</value>
    Public Property TypeName() As String
      Get
        Return CType(Me.GetView("Default"), CslaDataSourceView).TypeName
      End Get
      Set(ByVal value As String)
        CType(Me.GetView("Default"), CslaDataSourceView).TypeName = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set a value indicating whether the
    ''' business object data source supports paging.
    ''' </summary>
    ''' <remarks>
    ''' To support paging, the business object
    ''' (collection) must implement 
    ''' <see cref="Csla.Core.IReportTotalRowCount"/>.
    ''' </remarks>
    Public Property TypeSupportsPaging() As Boolean
      Get
        Return CType(Me.GetView("Default"), CslaDataSourceView).TypeSupportsPaging
      End Get
      Set(ByVal value As Boolean)
        CType(Me.GetView("Default"), CslaDataSourceView).TypeSupportsPaging = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set a value indicating whether the
    ''' business object data source supports sorting.
    ''' </summary>
    Public Property TypeSupportsSorting() As Boolean
      Get
        Return (CType(Me.GetView("Default"), CslaDataSourceView)).TypeSupportsSorting
      End Get
      Set(ByVal value As Boolean)
        CType(Me.GetView("Default"), CslaDataSourceView).TypeSupportsSorting = value
      End Set
    End Property

    Private Shared _typeCache As New Dictionary(Of String, Type)

    ''' <summary>
    ''' Returns a <see cref="Type">Type</see> object based on the
    ''' assembly and type information provided.
    ''' </summary>
    ''' <param name="typeAssemblyName">Optional assembly name.</param>
    ''' <param name="typeName">Full type name of the class,
    ''' including assembly name.</param>
    ''' <remarks></remarks>
    Friend Overloads Shared Function [GetType]( _
      ByVal typeAssemblyName As String, _
      ByVal typeName As String) As Type

      Dim result As Type = Nothing
      If Not String.IsNullOrEmpty(typeAssemblyName) Then
        ' explicit assembly name provided
        result = Type.GetType( _
          String.Format("{0}, {1}", typeName, typeAssemblyName), True, True)

      ElseIf typeName.IndexOf(",") > 0 Then
        ' assembly qualified type name provided
        result = Type.GetType(typeName, True, True)

      Else
        ' no assembly name provided
        result = _typeCache(typeName)
        If result Is Nothing Then
          For Each asm As Assembly In AppDomain.CurrentDomain.GetAssemblies
            result = asm.GetType(typeName, False, True)
            If result IsNot Nothing Then
              _typeCache.Add(typeName, result)
              Exit For
            End If
          Next
        End If
      End If

      If result Is Nothing Then
        Throw New TypeLoadException(String.Format(My.Resources.TypeLoadException, typeName))
      End If

      Return result

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

End Namespace
#End If
