Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Server

  ''' <summary>
  ''' Defines an interface to be implemented by
  ''' a factory loader object that returns ObjectFactory
  ''' objects based on the ObjectFactory attributes
  ''' used to decorate CSLA .NET business objects.
  ''' </summary>
  Public Interface IObjectFactoryLoader

    ''' <summary>
    ''' Returns the type of the factory object.
    ''' </summary>
    ''' <param name="factoryName">
    ''' Name of the factory to create, typically
    ''' an assembly qualified type name.
    ''' </param>
    Function GetFactoryType(ByVal factoryName As String) As Type

    ''' <summary>
    ''' Returns an ObjectFactory object.
    ''' </summary>
    ''' <param name="factoryName">
    ''' Name of the factory to create, typically
    ''' an assembly qualified type name.
    ''' </param>
    Function GetFactory(ByVal factoryName As String) As Object

  End Interface

End Namespace

