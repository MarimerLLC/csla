Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Server.Hosts.Silverlight

    ''' <summary>
    ''' Defines an interface to be implemented by
    ''' a factory object that returns MobileFactory
    ''' objects based on the MobileFactory attributes
    ''' used to decorate CSLA Light business objects.
    ''' </summary>
    Public Interface IMobileFactoryLoader

        ''' <summary>
        ''' Returns a MobileFactory object.
        ''' </summary>
        ''' <param name="factoryName">
        ''' Name of the factory to create, typically
        ''' an assembly qualified type name.
        ''' </param>
        Function GetFactory(ByVal factoryName As String) As Object

    End Interface

End Namespace

