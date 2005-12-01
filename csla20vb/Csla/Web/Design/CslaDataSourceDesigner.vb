Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel

Namespace Web.Design

  ''' <summary>
  ''' Implements designer support for CslaDataSource.
  ''' </summary>
  Public Class CslaDataSourceDesigner
    Inherits DataSourceDesigner

    Private mControl As CslaDataSource = Nothing
    Private mView As CslaDesignerDataSourceView = Nothing

    ''' <summary>
    ''' Initialize the designer component.
    ''' </summary>
    ''' <param name="component">The CslaDataSource control to 
    ''' be designed.</param>
    Public Overrides Sub Initialize(ByVal component As IComponent)

      MyBase.Initialize(component)
      mControl = CType(component, CslaDataSource)

    End Sub

    ''' <summary>
    ''' Returns the default view for this designer.
    ''' </summary>
    ''' <param name="viewName">Ignored</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' This designer supports only a "Default" view.
    ''' </remarks>
    Public Overrides Function GetView(ByVal viewName As String) As DesignerDataSourceView

      If mView Is Nothing Then
        mView = New CslaDesignerDataSourceView(Me, "Default")
      End If
      Return mView

    End Function

    ''' <summary>
    ''' Return a list of available views.
    ''' </summary>
    ''' <remarks>
    ''' This designer supports only a "Default" view.
    ''' </remarks>
    Public Overrides Function GetViewNames() As String()

      Return New String() {"Default"}

    End Function

    ''' <summary>
    ''' Refreshes the schema for the data.
    ''' </summary>
    ''' <param name="preferSilent"></param>
    ''' <remarks></remarks>
    Public Overrides Sub RefreshSchema(ByVal preferSilent As Boolean)

      Me.OnSchemaRefreshed(EventArgs.Empty)

    End Sub

    ''' <summary>
    ''' Get a value indicating whether the control can
    ''' refresh its schema.
    ''' </summary>
    Public Overrides ReadOnly Property CanRefreshSchema() As Boolean
      Get
        Return True
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether the control can
    ''' be resized.
    ''' </summary>
    Public Overrides ReadOnly Property AllowResize() As Boolean
      Get
        Return False
      End Get
    End Property

    ''' <summary>
    ''' Get a reference to the CslaDataSource control being
    ''' designed.
    ''' </summary>
    Friend ReadOnly Property DataSourceControl() As CslaDataSource
      Get
        Return mControl
      End Get
    End Property

  End Class

End Namespace
