Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object providing schema information for a
  ''' business object.
  ''' </summary>
  <Serializable()> _
  Public Class ObjectViewSchema
    Implements IDataSourceViewSchema

    Private mTypeAssemblyName As String = ""
    Private mTypeName As String = ""

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    ''' <param name="assemblyName">The assembly containing
    ''' the business class for which to generate the schema.</param>
    ''' <param name="typeName">The business class for
    ''' which to generate the schema.</param>
    Public Sub New(ByVal assemblyName As String, ByVal typeName As String)

      mTypeAssemblyName = assemblyName
      mTypeName = typeName

    End Sub

    ''' <summary>
    ''' Returns a list of child schemas belonging to the
    ''' object.
    ''' </summary>
    ''' <remarks>This schema object only returns
    ''' schema for the object itself, so GetChildren will
    ''' always return Nothing (null in C#).</remarks>
    Public Function GetChildren() As System.Web.UI.Design.IDataSourceViewSchema() Implements System.Web.UI.Design.IDataSourceViewSchema.GetChildren
      Return Nothing
    End Function

    ''' <summary>
    ''' Returns schema information for each property on
    ''' the object.
    ''' </summary>
    ''' <remarks>All public properties on the object
    ''' will be reflected in this schema list except
    ''' for those properties where the 
    ''' <see cref="BrowsableAttribute">Browsable</see> attribute
    ''' is False.
    ''' </remarks>
    Public Function GetFields() As _
      System.Web.UI.Design.IDataSourceFieldSchema() _
      Implements System.Web.UI.Design.IDataSourceViewSchema.GetFields

      Dim result As New Generic.List(Of ObjectFieldInfo)

      Dim originalPath As String = GetOriginalPath( _
        mTypeAssemblyName, mTypeName)

      Dim fulltrust As System.Security.NamedPermissionSet = _
        New System.Security.NamedPermissionSet("FullTrust")
      Dim tempDomain As AppDomain = AppDomain.CreateDomain( _
        "__temp", _
        AppDomain.CurrentDomain.Evidence, _
        AppDomain.CurrentDomain.SetupInformation, _
        fulltrust, _
        New System.Security.Policy.StrongName() {})
      Try
        ' load the TypeLoader object in the temp AppDomain
        Dim thisAssembly As Assembly = Assembly.GetExecutingAssembly
        Dim loader As TypeLoader = _
          DirectCast(tempDomain.CreateInstanceFromAndUnwrap( _
            thisAssembly.CodeBase, GetType(TypeLoader).FullName), TypeLoader)
        result = loader.GetFields(originalPath, mTypeAssemblyName, mTypeName)

      Finally
        AppDomain.Unload(tempDomain)
      End Try

      Return result.ToArray

    End Function

    Private Function GetOriginalPath(ByVal assemblyName As String, ByVal typeName As String) As String

      Dim t As Type = CslaDataSource.GetType(assemblyName, typeName)
      Dim asm As Assembly = t.Assembly
      Return asm.CodeBase

    End Function

    ''' <summary>
    ''' Returns the name of the schema.
    ''' </summary>
    Public ReadOnly Property Name() As String Implements System.Web.UI.Design.IDataSourceViewSchema.Name
      Get
        Return "Default"
      End Get
    End Property

  End Class

End Namespace