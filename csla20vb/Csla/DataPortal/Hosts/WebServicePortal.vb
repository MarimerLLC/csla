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

    <Serializable()> _
    Public Class CreateRequest

      Private mObjectType As Type
      Public Property ObjectType() As Type
        Get
          Return mObjectType
        End Get
        Set(ByVal value As Type)
          mObjectType = value
        End Set
      End Property

      Private mCriteria As Object
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      Private mContext As Server.DataPortalContext
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    <Serializable()> _
    Public Class FetchRequest

      Private mCriteria As Object
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      Private mContext As Server.DataPortalContext
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    <Serializable()> _
    Public Class UpdateRequest

      Private mObject As Object
      Public Property [Object]() As Object
        Get
          Return mObject
        End Get
        Set(ByVal value As Object)
          mObject = value
        End Set
      End Property

      Private mContext As Server.DataPortalContext
      Public Property Context() As Server.DataPortalContext
        Get
          Return mContext
        End Get
        Set(ByVal value As Server.DataPortalContext)
          mContext = value
        End Set
      End Property

    End Class

    <Serializable()> _
    Public Class DeleteRequest

      Private mCriteria As Object
      Public Property Criteria() As Object
        Get
          Return mCriteria
        End Get
        Set(ByVal value As Object)
          mCriteria = value
        End Set
      End Property

      Private mContext As Server.DataPortalContext
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

    <WebMethod()> _
    Public Function Fetch(ByVal requestData As Byte()) As Byte()

      Dim request As FetchRequest = DirectCast(Deserialize(requestData), FetchRequest)

      Dim portal As New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Fetch(request.Criteria, request.Context)

      Catch ex As Exception
        result = ex
      End Try
      Return Serialize(result)

    End Function

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
