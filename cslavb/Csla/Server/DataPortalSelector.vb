Imports System
Imports System.Configuration
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla.Properties

Namespace Server

  ''' <summary>
  ''' Selects the appropriate data portal implementation
  ''' to invoke based on the object and configuration.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class DataPortalSelector
    Implements IDataPortalServer

#Region "IDataPortalServer Members"

    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Create(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Create
      Try
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType)

        If context.FactoryInfo Is Nothing Then
          Dim dp = New SimpleDataPortal()
          Return dp.Create(objectType, criteria, context)
        Else
          Dim dp = New FactoryDataPortal()
          Return dp.Create(objectType, criteria, context)
        End If
      Catch ex As DataPortalException
        Throw
      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Create " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to retrieve.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Fetch(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Fetch
      Try
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType)

        If context.FactoryInfo Is Nothing Then
          Dim dp = New SimpleDataPortal()
          Return dp.Fetch(objectType, criteria, context)
        Else
          Dim dp = New FactoryDataPortal()
          Return dp.Fetch(objectType, criteria, context)
        End If
      Catch ex As DataPortalException
        Throw
      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Fetch " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="obj">Business object to update.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Update(ByVal obj As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Update
      Try
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType())

        If context.FactoryInfo Is Nothing Then
          Dim dp = New SimpleDataPortal()
          Return dp.Update(obj, context)
        Else
          Dim dp = New FactoryDataPortal()
          Return dp.Update(obj, context)
        End If
      Catch ex As DataPortalException
        Throw
      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Update " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="objectType">Type of business object to create.</param>
    ''' <param name="criteria">Criteria object describing business object.</param>
    ''' <param name="context">
    ''' <see cref="Server.DataPortalContext" /> object passed to the server.
    ''' </param>
    Public Function Delete(ByVal objectType As System.Type, ByVal criteria As Object, ByVal context As DataPortalContext) As DataPortalResult Implements IDataPortalServer.Delete
      Try
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType)

        If context.FactoryInfo Is Nothing Then
          Dim dp = New SimpleDataPortal()
          Return dp.Delete(objectType, criteria, context)
        Else
          Dim dp = New FactoryDataPortal()
          Return dp.Delete(objectType, criteria, context)
        End If
      Catch ex As DataPortalException
        Throw
      Catch ex As Exception
        Throw New DataPortalException("DataPortal.Delete " + Resources.FailedOnServer, ex, New DataPortalResult())
      End Try
    End Function

#End Region

  End Class

End Namespace

