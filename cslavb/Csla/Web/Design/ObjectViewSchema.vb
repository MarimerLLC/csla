Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object providing schema information for a
  ''' business object.
  ''' </summary>
  <Serializable()> _
  Public Class ObjectViewSchema
    Implements IDataSourceViewSchema

    Private _typeName As String = ""
    Private _designer As CslaDataSourceDesigner

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    ''' <param name="typeName">The business class for
    ''' which to generate the schema.</param>
    Public Sub New(ByVal site As CslaDataSourceDesigner, ByVal typeName As String)

      _typeName = typeName
      _designer = site

    End Sub

    ''' <summary>
    ''' Returns a list of child schemas belonging to the
    ''' object.
    ''' </summary>
    ''' <remarks>This schema object only returns
    ''' schema for the object itself, so GetChildren will
    ''' always return Nothing (null in C#).</remarks>
    Public Function GetChildren() As System.Web.UI.Design.IDataSourceViewSchema() _
      Implements System.Web.UI.Design.IDataSourceViewSchema.GetChildren

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
    Public Function GetFields() As  _
      System.Web.UI.Design.IDataSourceFieldSchema() _
      Implements System.Web.UI.Design.IDataSourceViewSchema.GetFields

      Dim typeService As ITypeResolutionService = Nothing
      Dim result As New List(Of ObjectFieldInfo)()

      If _designer IsNot Nothing Then
        Dim objectType As Type
        Try
          If _designer IsNot Nothing Then
            typeService = DirectCast( _
              _designer.Site.GetService( _
              GetType(ITypeResolutionService)), ITypeResolutionService)

            objectType = typeService.GetType(_typeName, True, False)

            If GetType(IEnumerable).IsAssignableFrom(objectType) Then
              ' this is a list so get the item type
              objectType = Utilities.GetChildItemType(objectType)
            End If
            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(objectType)
            For Each item As PropertyDescriptor In props
              If item.IsBrowsable Then
                result.Add(New ObjectFieldInfo(item))
              End If
            Next item
          End If

        Catch
          ' do nothing - just swallow exception
        End Try
      End If
      Return result.ToArray()

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