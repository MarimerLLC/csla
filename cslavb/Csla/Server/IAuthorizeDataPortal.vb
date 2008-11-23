Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Server

  ''' <summary>
  ''' Interface to be implemented by a custom
  ''' authorization provider.
  ''' </summary>
  ''' <remarks></remarks>
  Public Interface IAuthorizeDataPortal

    ''' <summary>
    ''' Implement this method to perform custom
    ''' authorization on every data portal call.
    ''' </summary>
    ''' <param name="clientRequest">
    ''' Object containing information about the client request.
    ''' </param>
    ''' <remarks></remarks>
    Sub Authorize(ByVal clientRequest As AuthorizeRequest)

  End Interface

End Namespace

