Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Server

  ''' <summary>
  ''' Class containing the default implementation for
  ''' the FactoryLoader delegate used by the
  ''' Silverlight data portal host.
  ''' </summary>
  Public Class ObjectFactoryLoader
    Implements IObjectFactoryLoader

    ''' <summary>
    ''' Gets the type of the factory.
    ''' </summary>
    ''' <param name="factoryName">
    ''' Type assembly qualified type name for the 
    ''' object factory class as
    ''' provided from the ObjectFactory attribute
    ''' on the business object.
    ''' </param>
    Public Function GetFactoryType(ByVal factoryName As String) As System.Type Implements IObjectFactoryLoader.GetFactoryType
      Return Type.GetType(factoryName)
    End Function

    ''' <summary>
    ''' Creates an instance of an object factory
    ''' object for use by the data portal.
    ''' </summary>
    ''' <param name="factoryName">
    ''' Type assembly qualified type name for the 
    ''' object factory class as
    ''' provided from the ObjectFactory attribute
    ''' on the business object.
    ''' </param>
    ''' <returns>
    ''' An instance of the type specified by the
    ''' type name parameter.
    ''' </returns>
    Public Function GetFactory(ByVal factoryName As String) As Object Implements IObjectFactoryLoader.GetFactory
      Dim ft = Type.GetType(factoryName)
      If ft Is Nothing Then
        Throw New InvalidOperationException(String.Format(My.Resources.FactoryTypeNotFoundException, factoryName))
      End If
      Return Activator.CreateInstance(ft)
    End Function

    
  End Class
End Namespace

