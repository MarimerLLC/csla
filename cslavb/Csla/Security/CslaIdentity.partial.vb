Imports System
Imports System.Security.Principal
Imports Csla.Serialization
Imports System.Collections.Generic
Imports Csla.Core.FieldManager
Imports System.Runtime.Serialization
Imports Csla.Core

Namespace Security

  ''' <summary>
  ''' Provides a base class to simplify creation of
  ''' a .NET identity object for use with BusinessPrincipalBase.
  ''' </summary>
  Partial Public MustInherit Class CslaIdentity
    Inherits ReadOnlyBase(Of CslaIdentity)
    Implements IIdentity

    Private Shared _forceInit As Integer

#Region "Constructor"

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Protected Sub New()
      _forceInit = _forceInit + 0
    End Sub

#End Region

    ''' <summary>
    ''' Invokes the data portal to get an instance of
    ''' the identity object.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of the CslaIdentity subclass to retrieve.
    ''' </typeparam>
    ''' <param name="criteria">
    ''' Object containing the user's credentials.
    ''' </param>
    ''' <returns></returns>
    Public Shared Shadows Function GetCslaIdentity(Of T As CslaIdentity)(ByVal criteria As Object) As T      
      Return DataPortal.Fetch(Of T)(criteria)
    End Function

    Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
      _forceInit = 0
      MyBase.OnDeserialized(context)
    End Sub

  End Class

End Namespace

