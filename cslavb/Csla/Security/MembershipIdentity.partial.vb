#If CLIENTONLY <> True Then

Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports Csla.DataPortalClient
Imports Csla.Silverlight
Imports Csla.Core

Namespace Security

  Partial Public Class MembershipIdentity
    Inherits ReadOnlyBase(Of MembershipIdentity)
    Implements IIdentity

    ''' <summary>
    ''' Creates an instance of the class
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
      _forceInit = _forceInit + 0
    End Sub

    ''' <summary>
    ''' Authenticates the user's credentials against the ASP.NET
    ''' membership provider.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of object (subclass of MembershipIdentity) to retrieve.
    ''' </typeparam>
    ''' <param name="userName">Username to authenticate.</param>
    ''' <param name="password">Password to authenticate.</param>
    ''' <param name="isRunOnWebServer">
    ''' Specifies whether to access the membership provider locally (true),
    ''' or through the data portal (false) presumably to reach an application
    ''' server.
    ''' </param>
    ''' <returns></returns>
    Public Shared Function GetMembershipIdentity(Of T As MembershipIdentity)(ByVal userName As String, ByVal password As String, ByVal isRunOnWebServer As Boolean) As T
      Dim factory As New IdentityFactory
      Return CType(factory.FetchMembershipIdentity(New Criteria(userName, password, GetType(T), isRunOnWebServer)), T)
    End Function

    ''' <summary>
    ''' Method invoked when the object is deserialized.
    ''' </summary>
    ''' <param name="context">Serialization context.</param>
    Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
      _forceInit = 0
      MyBase.OnDeserialized(context)
    End Sub

  End Class
End Namespace
#End If


