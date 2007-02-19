Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Windows.Forms.Design

Namespace Web.Design

  ''' <summary>
  ''' Implements designer support for CslaDataSource.
  ''' </summary>
  Public Class CslaDataSourceDesigner
    Inherits DataSourceDesigner

    Private mControl As DataSourceControl = Nothing
    Private mView As CslaDesignerDataSourceView = Nothing

    ''' <summary>
    ''' Initialize the designer component.
    ''' </summary>
    ''' <param name="component">The CslaDataSource control to 
    ''' be designed.</param>
    Public Overrides Sub Initialize(ByVal component As IComponent)

      MyBase.Initialize(component)
      mControl = CType(component, DataSourceControl)

    End Sub

    Friend ReadOnly Property Site() As System.ComponentModel.ISite
      Get
        Return mControl.Site
      End Get
    End Property
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
    ''' Invoke the design time configuration
    ''' support provided by the control.
    ''' </summary>
    Public Overrides Sub Configure()

      InvokeTransactedChange(mControl, AddressOf ConfigureCallback, Nothing, "ConfigureDataSource")

    End Sub

    Private Function ConfigureCallback(ByVal context As Object) As Boolean

      Dim result As Boolean = False
      Dim oldTypeName As String
      If String.IsNullOrEmpty(CType(DataSourceControl, CslaDataSource).TypeAssemblyName) Then
        oldTypeName = (CType(DataSourceControl, CslaDataSource)).TypeName

      Else
        oldTypeName = String.Format("{0}, {1}", _
          CType(DataSourceControl, CslaDataSource).TypeName, CType(DataSourceControl, CslaDataSource).TypeAssemblyName)
      End If

      Dim uiService As IUIService = CType(mControl.Site.GetService(GetType(IUIService)), IUIService)
      Dim cfg As CslaDataSourceConfiguration = New CslaDataSourceConfiguration(mControl, oldTypeName)
      If uiService.ShowDialog(cfg) = System.Windows.Forms.DialogResult.OK Then
        SuppressDataSourceEvents()
        Try
          CType(DataSourceControl, CslaDataSource).TypeAssemblyName = ""
          CType(DataSourceControl, CslaDataSource).TypeName = cfg.TypeName
          OnDataSourceChanged(EventArgs.Empty)
          result = True

        Finally
          ResumeDataSourceEvents()
        End Try
      End If
      cfg.Dispose()
      Return result

    End Function

    ''' <summary>
    ''' Get a value indicating whether this control
    ''' supports design time configuration.
    ''' </summary>
    Public Overrides ReadOnly Property CanConfigure() As Boolean
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
        Return DirectCast(mControl, CslaDataSource)
      End Get
    End Property

  End Class

End Namespace
