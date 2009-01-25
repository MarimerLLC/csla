Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Web.UI
Imports Csla.Core

Namespace Web.Design

  ''' <summary>
  ''' CslaDataSource configuration form.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class CslaDataSourceConfiguration

    Private _control As DataSourceControl

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New()

      ' This call is required by the Windows Form Designer.
      InitializeComponent()

    End Sub

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="control">Reference to the data source control.</param>
    ''' <param name="oldTypeName">Existing type name.</param>
    Public Sub New(ByVal control As DataSourceControl, ByVal oldTypeName As String)

      Me.New()
      _control = control
      DiscoverTypes()
      Me.TypeComboBox.Text = oldTypeName

    End Sub

    ''' <summary>
    ''' Gets the type name entered by the user.
    ''' </summary>
    Public ReadOnly Property TypeName() As String
      Get
        Return Me.TypeComboBox.Text
      End Get
    End Property

    Private Sub DiscoverTypes()

      ' try to get a reference to the type discovery service
      Dim discovery As ITypeDiscoveryService = Nothing
      If Not _control.Site Is Nothing Then
        discovery = CType(_control.Site.GetService(GetType(ITypeDiscoveryService)), ITypeDiscoveryService)
      End If

      If Not discovery Is Nothing Then
        ' saves the cursor and sets the wait cursor
        Dim previousCursor As Cursor = Cursor.Current
        Cursor.Current = Cursors.WaitCursor
        Try
          ' gets all types using the type discovery service
          Dim types As ICollection = discovery.GetTypes(GetType(Object), True)
          TypeComboBox.BeginUpdate()
          TypeComboBox.Items.Clear()
          ' adds the types to the list
          For Each type As Type In types
            If type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(",")) <> "Csla" AndAlso _
                  GetType(IBusinessObject).IsAssignableFrom(type) Then
              Dim name As String = type.AssemblyQualifiedName
              If name.Substring(name.Length - 19, 19) = "PublicKeyToken=null" Then
                name = name.Substring(0, name.IndexOf(",", name.IndexOf(",") + 1))
              End If
              TypeComboBox.Items.Add(name)
            End If
          Next type

        Finally
          Cursor.Current = previousCursor
          TypeComboBox.EndUpdate()
        End Try
      End If

    End Sub

    Private Sub OkButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
      Handles OkButton.Click

      Me.DialogResult = System.Windows.Forms.DialogResult.OK
      Hide()

    End Sub

  End Class

End Namespace
