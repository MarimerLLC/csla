Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Server

  ''' <summary>
  ''' Specifies that the data portal
  ''' should invoke a factory object rather than
  ''' the business object.
  ''' </summary>
  ''' <remarks></remarks>
  <AttributeUsage(AttributeTargets.Class, AllowMultiple:=False)> _
  Public Class ObjectFactoryAttribute
    Inherits Attribute

    Friend Shared Function GetObjectFactoryAttribute(ByVal objectType As Type) As ObjectFactoryAttribute
      Dim result = objectType.GetCustomAttributes(GetType(ObjectFactoryAttribute), True)

      If result IsNot Nothing AndAlso result.Length > 0 Then
        Return DirectCast(result(0), ObjectFactoryAttribute)
      Else
        Return Nothing
      End If

    End Function

    Private _factoryTypeName As String
    ''' <summary>
    ''' Assembly qualified type name of the factory object.
    ''' </summary>
    ''' <remarks>
    ''' Factory class must have a parameterless 
    ''' default constructor.
    ''' </remarks>
    Public Property FactoryTypeName() As String
      Get
        Return _factoryTypeName
      End Get
      Private Set(ByVal value As String)
        _factoryTypeName = value
      End Set
    End Property

    Private _createMethodName As String
    ''' <summary>
    ''' Name of the method to call for a create operation.
    ''' </summary>
    ''' <remarks>
    ''' The appropriate overload of this method will be
    ''' invoked based on the parameters passed from the client.
    ''' </remarks>
    Public Property CreateMethodName() As String
      Get
        Return _createMethodName
      End Get
      Private Set(ByVal value As String)
        _createMethodName = value
      End Set
    End Property

    Private _fetchMethodName As String
    ''' <summary>
    ''' Name of the method to call for a fetch operation.
    ''' </summary>
    ''' <remarks>
    ''' The appropriate overload of this method will be
    ''' invoked based on the parameters passed from the client.
    ''' </remarks>
    Public Property FetchMethodName() As String
      Get
        Return _fetchMethodName
      End Get
      Private Set(ByVal value As String)
        _fetchMethodName = value
      End Set
    End Property

    Private _updateMethodName As String
    ''' <summary>
    ''' Name of the method to call for a update operation.
    ''' </summary>
    ''' <remarks>
    ''' The appropriate overload of this method will be
    ''' invoked based on the parameters passed from the client.
    ''' </remarks>
    Public Property UpdateMethodName() As String
      Get
        Return _updateMethodName
      End Get
      Private Set(ByVal value As String)
        _updateMethodName = value
      End Set
    End Property

    Private _deleteMethodName As String
    ''' <summary>
    ''' Name of the method to call for a delete operation.
    ''' </summary>
    ''' <remarks>
    ''' The appropriate overload of this method will be
    ''' invoked based on the parameters passed from the client.
    ''' </remarks>
    Public Property DeleteMethodName() As String
      Get
        Return _deleteMethodName
      End Get
      Private Set(ByVal value As String)
        _deleteMethodName = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the attribute.
    ''' </summary>
    ''' <param name="factoryType">
    ''' Assembly qualified type name of the factory object.
    ''' </param>
    ''' <remarks>
    ''' The method names default to Create, Fetch,
    ''' Update and Delete.
    ''' </remarks>
    Public Sub New(ByVal factoryType As String)
      Me.FactoryTypeName = factoryType
      Me.CreateMethodName = "Create"
      Me.FetchMethodName = "Fetch"
      Me.UpdateMethodName = "Update"
      Me.DeleteMethodName = "Delete"
    End Sub

    ''' <summary>
    ''' Creates an instance of the attribute.
    ''' </summary>
    ''' <param name="factoryType">
    ''' Assembly qualified type name of the factory object.
    ''' </param>
    ''' <param name="createMethod">
    ''' Name of the method to call for a create operation.</param>
    ''' <param name="fetchMethod">
    ''' Name of the method to call for a fetch operation.
    ''' </param>
    ''' <param name="updateMethod">
    ''' Name of the method to call for a update operation.</param>
    ''' <param name="deleteMethod">
    ''' Name of the method to call for a delete operation.</param>
    Public Sub New(ByVal factoryType As String, _
                   ByVal createMethod As String, _
                   ByVal fetchMethod As String, _
                   ByVal updateMethod As String, _
                   ByVal deleteMethod As String)

      Me.FactoryTypeName = factoryType
      Me.CreateMethodName = createMethod
      Me.FetchMethodName = fetchMethod
      Me.UpdateMethodName = updateMethod
      Me.DeleteMethodName = deleteMethod
    End Sub

  End Class
End Namespace

