Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text


Namespace Server.Hosts.Silverlight

  ''' <summary>
  ''' Class containing the default implementation for
  ''' the FactoryLoader delegate used by the
  ''' Silverlight data portal host.
  ''' </summary>
  Public Class MobileFactoryLoader
    Implements IMobileFactoryLoader


    ''' <summary>
    ''' Creates an instance of a mobile factory
    ''' object for use by the data portal.
    ''' </summary>
    ''' <param name="factoryName">
    ''' Type assembly qualified type name for the 
    ''' mobile factory class as
    ''' provided from the MobileFactory attribute
    ''' on the business object.
    ''' </param>
    ''' <returns>
    ''' An instance of the type specified by the
    ''' type name parameter.
    ''' </returns>
    Public Function GetFactory(ByVal factoryName As String) As Object Implements IMobileFactoryLoader.GetFactory
      Dim ft = Type.GetType(factoryName)

      If ft Is Nothing Then
        Throw New InvalidOperationException(String.Format(My.Resources.FactoryTypeNotFoundException, factoryName))
      End If

      Return Activator.CreateInstance(ft)
    End Function

  End Class

End Namespace

