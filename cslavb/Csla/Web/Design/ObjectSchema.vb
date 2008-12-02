#If Not CLIENTONLY Then
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

    Private _typeName As String = ""
    Private _designer As CslaDataSourceDesigner

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="typeName">Type name for
    ''' which the schema should be generated.</param>
    Public Sub New(ByVal designer As CslaDataSourceDesigner, ByVal typeName As String)

      _typeName = typeName
      _designer = designer

    End Sub


    ''' <summary>
    ''' Returns a single element array containing the
    ''' schema for the CSLA .NET business object.
    ''' </summary>
    Public Function GetViews() As  _
      System.Web.UI.Design.IDataSourceViewSchema() _
      Implements System.Web.UI.Design.IDataSourceSchema.GetViews

      Dim result As IDataSourceViewSchema()
      result = New IDataSourceViewSchema() _
        {New ObjectViewSchema(_designer, _typeName)}
      Return result

    End Function

  End Class

End Namespace
#End If