Imports System
Imports System.Configuration
Imports Csla.Serialization.Mobile
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports Csla.Core
Imports System.Security.Principal
Imports Csla.Properties

Namespace Server.Hosts.Silverlight

    ''' <summary>
    ''' Exposes server-side DataPortal functionality
    ''' through WCF.
    ''' </summary>
    ''' <remarks></remarks>
    <AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)> _
    Public Class WcfPortal
        Implements IWcfPortal

#Region "Factory Loader"

        Private Shared _factoryLoader As IMobileFactoryLoader

        Public Shared Property FactoryLoader() As IMobileFactoryLoader
            Get
                If _factoryLoader Is Nothing Then
                    Dim setting As String = ConfigurationManager.AppSettings("CslaMobileFactoryLoader")

                    If String.IsNullOrEmpty(setting) Then
                        _factoryLoader = CType(Activator.CreateInstance(Type.GetType(setting, True, True)), IMobileFactoryLoader)
                    Else
                        _factoryLoader = New MobileFactoryLoader()
                    End If
                End If

                Return _factoryLoader
            End Get
            Set(ByVal value As IMobileFactoryLoader)
                _factoryLoader = value
            End Set
        End Property

#End Region

#Region "IWcfPortal Members"

        ''' <summary>
        ''' Create a new business object.
        ''' </summary>
        ''' <param name="request">The request parameter object.</param>
        Public Function Create(ByVal request As CriteriaRequest) As WcfResponse Implements IWcfPortal.Create
            Dim result As WcfResponse = New WcfResponse()

            Try
                request = ConvertRequest(request)
                'unpack criteria data into object
                Dim criteria As Object = GetCriteria(request.CriteriaData)

                'load type for business object
                Dim t = Type.GetType(request.TypeName)
                If t Is Nothing Then
                    Throw New InvalidOperationException(String.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName))
                End If

                SetContext(request)
                Dim o As Object = Nothing
                Dim factoryInfo = GetMobileFactoryAttribute(t)
                If factoryInfo Is Nothing Then
                    If criteria IsNot Nothing Then
                        o = Csla.DataPortal.Create(criteria)
                    Else
                        o = Csla.DataPortal.Create(t)
                    End If
                Else

                    If String.IsNullOrEmpty(factoryInfo.CreateMethodName) Then
                        Throw New InvalidOperationException(Resources.CreateMethodNameNotSpecified)
                    End If

                    Dim f As Object = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName)
                    If criteria IsNot Nothing Then
                        o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.CreateMethodName, criteria)
                    Else
                        o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.CreateMethodName)
                    End If
                End If

                result.ObjectData = MobileFormatter.Serialize(o)
                result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext)

            Catch ex As Csla.Reflection.CallMethodException
                result.ErrorData = New WcfErrorInfo(ex.InnerException)
            Catch ex As Exception
                result.ErrorData = New WcfErrorInfo(ex)
            Finally
                ClearContext()
            End Try

            Return ConvertResponse(result)

        End Function

        ''' <summary>
        ''' Get an existing business object.
        ''' </summary>
        ''' <param name="request">The request parameter object.</param>
        Public Function Fetch(ByVal request As CriteriaRequest) As WcfResponse Implements IWcfPortal.Fetch
            Dim result As WcfResponse = New WcfResponse()

            Try
                request = ConvertRequest(request)
                'unpack criteria data into object
                Dim criteria As Object = GetCriteria(request.CriteriaData)

                'load type for business object
                Dim t = Type.GetType(request.TypeName)
                If t Is Nothing Then
                    Throw New InvalidOperationException( _
                        String.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName))
                End If

                SetContext(request)
                Dim o As Object = Nothing
                Dim factoryInfo = GetMobileFactoryAttribute(t)
                If factoryInfo Is Nothing Then
                    If criteria Is Nothing Then
                        o = Csla.DataPortal.Fetch(t)
                    Else
                        o = Csla.DataPortal.Fetch(t, criteria)
                    End If
                Else

                    If String.IsNullOrEmpty(factoryInfo.FetchMethodName) Then
                        Throw New InvalidOperationException(Resources.FetchMethodNameNotSpecified)
                    End If

                    Dim f As Object = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName)
                    If criteria IsNot Nothing Then
                        o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.FetchMethodName, criteria)
                    Else
                        o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.FetchMethodName)
                    End If
                End If

                result.ObjectData = MobileFormatter.Serialize(o)
                result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext)
            Catch ex As Csla.Reflection.CallMethodException
                result.ErrorData = New WcfErrorInfo(ex.InnerException)
            Catch ex As Exception
                result.ErrorData = New WcfErrorInfo(ex)
            Finally
                ClearContext()
            End Try

            Return ConvertResponse(result)

        End Function

        ''' <summary>
        ''' Update a business object
        ''' </summary>
        ''' <param name="request">The request parameter object.</param>
        Public Function Update(ByVal request As UpdateRequest) As WcfResponse Implements IWcfPortal.Update
            Dim result As WcfResponse = New WcfResponse()

            Try
                request = ConvertRequest(request)
                'unpack object
                Dim obj As Object = GetCriteria(request.ObjectData)

                'load type for business object
                Dim t = obj.GetType()

                Dim o As Object = Nothing
                Dim factoryInfo = GetMobileFactoryAttribute(t)
                If factoryInfo Is Nothing Then
                    SetContext(request)
                    o = Csla.DataPortal.Update(obj)
                Else
                    If String.IsNullOrEmpty(String.IsNullOrEmpty(factoryInfo.UpdateMethodName)) Then
                        Throw New InvalidOperationException(Resources.UpdateMethodNameNotSpecified)
                    End If

                    SetContext(request)
                    Dim f As Object = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName)
                    o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.UpdateMethodName, obj)

                End If

                result.ObjectData = MobileFormatter.Serialize(o)
                result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext

            Catch ex As Exception
                result.ErrorData = New WcfErrorInfo(ex)
            Finally
                ClearContext()
            End Try

            Return ConvertResponse(result)
        End Function

        ''' <summary>
        ''' Delete a business object.
        ''' </summary>
        ''' <param name="request">The request parameter object.</param>
        Public Function Delete(ByVal request As CriteriaRequest) As WcfResponse Implements IWcfPortal.Delete
            Dim result As WcfResponse = New WcfResponse()

            Try
                request = ConvertRequest(request)
                'unpack criteria data into object
                Dim criteria As Object = GetCriteria(request.CriteriaData)

                'load type for business object
                Dim t = Type.GetType(request.TypeName)
                If t Is Nothing Then
                    Throw New InvalidOperationException( _
                        String.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName))
                End If

                SetContext(request)
                Dim factoryInfo = GetMobileFactoryAttribute(t)
                If factoryInfo Is Nothing Then
                    Csla.DataPortal.Delete(criteria)
                Else
                    If String.IsNullOrEmpty(factoryInfo.DeleteMethodName) Then
                        Throw New InvalidOperationException(Resources.DeleteMethodNameNotSpecified)
                    End If

                    Dim f As Object = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName)

                    If criteria IsNot Nothing Then
                        Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.DeleteMethodName, criteria)
                    Else
                        Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.DeleteMethodName)
                    End If
                End If

                result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext)

            Catch ex As Csla.Reflection.CallMethodException
                result.ErrorData = New WcfErrorInfo(ex.InnerException)
            Catch ex As Exception
                result.ErrorData = New WcfErrorInfo(ex)
            Finally
                ClearContext()
            End Try

        End Function

#End Region

#Region "Context and Criteria"

        Private Sub SetContext(ByVal request As CriteriaRequest)
            ApplicationContext.SetContext(CType(MobileFormatter.Deserialize(request.ClientContext), ContextDictionary), CType(MobileFormatter.Deserialize(request.GlobalContext), ContextDictionary))
            ApplicationContext.User = CType(MobileFormatter.Deserialize(request.Principal), IPrincipal)
        End Sub

        Private Sub SetContext(ByVal request As UpdateRequest)
            ApplicationContext.SetContext(CType(MobileFormatter.Deserialize(request.ClientContext), ContextDictionary), CType(MobileFormatter.Deserialize(request.GlobalContext), ContextDictionary))
            ApplicationContext.User = CType(MobileFormatter.Deserialize(request.Principal), IPrincipal)
        End Sub

        Private Shared Sub ClearContext(ByVal request As UpdateRequest)
            ApplicationContext.Clear()
            If ApplicationContext.AuthenticationType <> "Windows" Then
                ApplicationContext.User = New System.Security.Principal.GenericPrincipal(New System.Security.Principal.GenericIdentity(String.Empty), New String() {})
            End If
        End Sub

        Private Shared Function GetCriteria(ByVal criteriaData() As Byte) As Object
            Dim criteria As Object = Nothing
            If criteriaData IsNot Nothing Then
                criteria = MobileFormatter.Deserialize(criteriaData)
            End If
            Return criteria
        End Function

#End Region

#Region "Mobile Factory"

        Private Shared Function GetMobileFactoryAttribute(ByVal objectType As Type) As Csla.Silverlight.MobileFactoryAttribute
            Dim result = objectType.GetCustomAttributes(GetType(Csla.Silverlight.MobileFactoryAttribute), True)

            If result IsNot Nothing AndAlso result.Length > 0 Then
                Return CType(result(0), Csla.Silverlight.MobileFactoryAttribute)
            Else
                Return Nothing
            End If

        End Function

#End Region

#Region "Extention Method for Requests"

        ''' <summary>
        ''' Override to convert the request data before it
        ''' is transferred over the network.
        ''' </summary>
        ''' <param name="request">Request object.</param>
        Protected Overridable Function ConvertRequest(ByVal request As UpdateRequest) As UpdateRequest
            Return request
        End Function

        ''' <summary>
        ''' Override to convert the request data before it
        ''' is transferred over the network.
        ''' </summary>
        ''' <param name="request">Request object.</param>
        Protected Overridable Function ConvertRequest(ByVal request As CriteriaRequest) As CriteriaRequest
            Return request
        End Function

        ''' <summary>
        ''' Override to convert the response data after it
        ''' comes back from the network.
        ''' </summary>
        ''' <param name="response">Response object.</param>
        Protected Overridable Function ConvertResponse(ByVal response As WcfResponse) As WcfResponse
            Return response
        End Function

#End Region

    End Class
End Namespace

