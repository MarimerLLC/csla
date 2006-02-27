Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object providing schema information for a
  ''' business object.
  ''' </summary>
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
      Dim t As Type = CslaDataSource.GetType(mTypeAssemblyName, mTypeName)
      If GetType(IEnumerable).IsAssignableFrom(t) Then
        ' this is a list so get the item type
        t = Utilities.GetChildItemType(t)
      End If
      Dim props As PropertyDescriptorCollection = _
        TypeDescriptor.GetProperties(t)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(New ObjectFieldInfo(item))
        End If
      Next
      Return result.ToArray

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