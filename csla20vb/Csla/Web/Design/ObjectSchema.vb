Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object providing access to schema information for
  ''' a business object.
  ''' </summary>
  ''' <remarks>
  ''' This object returns only one view, which corresponds
  ''' to the business object used by data binding.
  ''' </remarks>
  Public Class ObjectSchema
    Implements IDataSourceSchema

    Private mTypeAssemblyName As String = ""
    Private mTypeName As String = ""

    Public Sub New(ByVal assemblyName As String, ByVal typeName As String)

      mTypeAssemblyName = assemblyName
      mTypeName = typeName

    End Sub

    ''' <summary>
    ''' Returns a single element array containing the
    ''' schema for the CSLA .NET business object.
    ''' </summary>
    Public Function GetViews() As _
      System.Web.UI.Design.IDataSourceViewSchema() _
      Implements System.Web.UI.Design.IDataSourceSchema.GetViews

      Return New IDataSourceViewSchema() _
        {New ObjectViewSchema(mTypeAssemblyName, mTypeName)}
    End Function

  End Class

End Namespace