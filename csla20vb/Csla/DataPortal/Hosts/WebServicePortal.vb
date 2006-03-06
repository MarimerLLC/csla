Imports System.Web
Imports System.Web.Services
Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary

Namespace Server.Hosts

  ' in asmx use web directive like
  ' <%@ WebService Class="Csla.Server.Hosts.WebServicePortal" %>

  ''' <summary>
  ''' Exposes server-side DataPortal functionality
  ''' through Web Services (asmx).
  ''' </summary>
  <WebService(Namespace:="http://ws.lhotka.net/Csla")> _
  Public Class WebServicePortal
    Inherits WebService

#Region " Request classes "

    ''' <summary>
    ''' Request message for creating
    ''' a new business object.
    ''' </summary>
    <Serializable()> _
    Public Class CreateRequest

      Private mObjectType As Type
      Private mCriteria As Object
      Private mContext As Server.DataPortalContext

      ''' <summary>
      ''' Type of business object to create.
      ''' </summary>
      Public Property ObjectType() As Type
        Get
          Return mObjectType
        End Get
        Set(ByVal value As Type)
          mObjectType = value
        End Set
      End Property

      ''' <summary>
      ''' Criteria object describing business object.
      ''' </summary>
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      ''' <summary>
      ''' Data portal context from client.
      ''' </summary>
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    ''' <summary>
    ''' Request message for retrieving
    ''' an existing business object.
    ''' </summary>
    <Serializable()> _
    Public Class FetchRequest

      Private mCriteria As Object
      Private mContext As Server.DataPortalContext

      ''' <summary>
      ''' Criteria object describing business object.
      ''' </summary>
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      ''' <summary>
      ''' Data portal context from client.
      ''' </summary>
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    ''' <summary>
    ''' Request message for updating
    ''' a business object.
    ''' </summary>
    <Serializable()> _
    Public Class UpdateRequest

      Private mObject As Object
      Private mContext As Server.DataPortalContext

      ''' <summary>
      ''' Business object to be updated.
      ''' </summary>
      Public Property [Object]() As Object
        Get
          Return mObject
        End Get
        Set(ByVal value As Object)
          mObject = value
        End Set
      End Property

      ''' <summary>
      ''' Data portal context from client.
      ''' </summary>
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    ''' <summary>
    ''' Request message for deleting
    ''' a business object.
    ''' </summary>
    <Serializable()> _
    Public Class DeleteRequest

      Private mCriteria As Object
      Private mContext As Server.DataPortalContext

      ''' <summary>
      ''' Criteria object describing business object.
      ''' </summary>
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      ''' <summary>
      ''' Data portal context from client.
      ''' </summary>
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

#End Region

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="requestData">Byte stream containing <see cref="CreateRequest" />.</param>
    ''' <returns>Byte stream containing resulting object data.</returns>
    <WebMethod()> _
    Public Function Create(ByVal requestData As Byte()) As Byte()

      Dim request As CreateRequest = DirectCast(Deserialize(requestData), CreateRequest)

      Dim portal As New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Create(request.ObjectType, request.Criteria, request.Context)

      Catch ex As Exception
        result = ex
      End Try
      Return Serialize(result)

    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="requestData">Byte stream containing <see cref="FetchRequest" />.</param>
    ''' <returns>Byte stream containing resulting object data.</returns>
    <WebMethod()> _
    Public Function Fetch(ByVal requestData As Byte()) As Byte()

      Dim request As FetchRequest = _
        DirectCast(Deserialize(requestData), FetchRequest)

      Dim portal As New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Fetch(request.Criteria, request.Context)

      Catch ex As Exception
        result = ex
      End Try
      Return Serialize(result)

    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="requestData">Byte stream containing <see cref="UpdateRequest" />.</param>
    ''' <returns>Byte stream containing resulting object data.</returns>
    <WebMethod()> _
    Public Function Update(ByVal requestData As Byte()) As Byte()

      Dim request As UpdateRequest = DirectCast(Deserialize(requestData), UpdateRequest)

      Dim portal As New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Update(request.Object, request.Context)

      Catch ex As Exception
        result = ex
      End Try
      Return Serialize(result)

    End Function

    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="requestData">Byte stream containing <see cref="DeleteRequest" />.</param>
    ''' <returns>Byte stream containing resulting object data.</returns>
    <WebMethod()> _
    Public Function Delete(ByVal requestData As Byte()) As Byte()

      Dim request As DeleteRequest = DirectCast(Deserialize(requestData), DeleteRequest)

      Dim portal As New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Delete(request.Criteria, request.Context)

      Catch ex As Exception
        result = ex
      End Try
      Return Serialize(result)

    End Function

#Region " Helper functions "

    Private Shared Function Serialize(ByVal obj As Object) As Byte()

      If Not obj Is Nothing Then
        Using buffer As New MemoryStream
          Dim formatter As New BinaryFormatter
          formatter.Serialize(buffer, obj)
          Return buffer.ToArray
        End Using

      Else
        Return Nothing
      End If

    End Function

    Private Shared Function Deserialize(ByVal obj As Byte()) As Object

      If Not obj Is Nothing Then
        Using buffer As New MemoryStream(obj)
          Dim formatter As New BinaryFormatter
          Return formatter.Deserialize(buffer)
        End Using

      Else
        Return Nothing
      End If

    End Function

#End Region

  End Class

End Namespace
