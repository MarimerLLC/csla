Imports Csla.Server.Hosts.WcfChannel

Namespace Server.Hosts
  ''' <summary>
  ''' Exposes server-side DataPortal functionality
  ''' through WCF.
  ''' </summary>
  Public Class WcfPortal
    Implements IWcfPortal
#Region "IWcfPortal Members"

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    Public Function Create(ByVal request As CreateRequest) As WcfResponse Implements IWcfPortal.Create
      Dim portal As Server.DataPortal = New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Create(request.ObjectType, request.Criteria, request.Context)
      Catch ex As Exception
        result = ex
      End Try
      Return New WcfResponse(result)
    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    Public Function Fetch(ByVal request As FetchRequest) As WcfResponse Implements IWcfPortal.Fetch
      Dim portal As Server.DataPortal = New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Fetch(request.ObjectType, request.Criteria, request.Context)
      Catch ex As Exception
        result = ex
      End Try
      Return New WcfResponse(result)
    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    Public Function Update(ByVal request As UpdateRequest) As WcfResponse Implements IWcfPortal.Update
      Dim portal As Server.DataPortal = New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Update(request.Object, request.Context)
      Catch ex As Exception
        result = ex
      End Try
      Return New WcfResponse(result)
    End Function

    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    Public Function Delete(ByVal request As DeleteRequest) As WcfResponse Implements IWcfPortal.Delete
      Dim portal As Server.DataPortal = New Server.DataPortal
      Dim result As Object
      Try
        result = portal.Delete(request.Criteria, request.Context)
      Catch ex As Exception
        result = ex
      End Try
      Return New WcfResponse(result)
    End Function

#End Region
  End Class
End Namespace