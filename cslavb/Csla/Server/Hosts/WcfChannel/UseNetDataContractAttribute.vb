Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.Runtime.Serialization
Imports System.Xml
Imports System.ServiceModel.Description

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Specify that WCF should serialize objects in a .NET
  ''' specific manner to as to preserve complex object
  ''' references and to be able to deserialize the graph
  ''' into the same type as the original objets.
  ''' </summary>
  Public Class UseNetDataContractAttribute
    Inherits Attribute
    Implements IOperationBehavior

#Region "IOperationBehavior Members"

    ''' <summary>
    ''' Not implemented.
    ''' </summary>
    ''' <param name="description">Not implemented.</param>
    ''' <param name="parameters">Not implemented.</param>
    Public Sub AddBindingParameters(ByVal description As OperationDescription, ByVal parameters As BindingParameterCollection) Implements IOperationBehavior.AddBindingParameters
    End Sub

    ''' <summary>
    ''' Apply the client behavior by requiring
    ''' the use of the NetDataContractSerializer.
    ''' </summary>
    ''' <param name="description">Operation description.</param>
    ''' <param name="proxy">Client operation object.</param>
    Public Sub ApplyClientBehavior(ByVal description As OperationDescription, ByVal proxy As System.ServiceModel.Dispatcher.ClientOperation) Implements IOperationBehavior.ApplyClientBehavior
      ReplaceDataContractSerializerOperationBehavior(description)
    End Sub

    ''' <summary>
    ''' Apply the dispatch behavior by requiring
    ''' the use of the NetDataContractSerializer.
    ''' </summary>
    ''' <param name="description">Operation description.</param>
    ''' <param name="dispatch">Dispatch operation object.</param>
    Public Sub ApplyDispatchBehavior(ByVal description As OperationDescription, ByVal dispatch As System.ServiceModel.Dispatcher.DispatchOperation) Implements IOperationBehavior.ApplyDispatchBehavior
      ReplaceDataContractSerializerOperationBehavior(description)
    End Sub

    ''' <summary>
    ''' Not implemented.
    ''' </summary>
    ''' <param name="description">Not implemented.</param>
    Public Sub Validate(ByVal description As OperationDescription) Implements IOperationBehavior.Validate
    End Sub

#End Region

    Private Shared Sub ReplaceDataContractSerializerOperationBehavior(ByVal description As OperationDescription)
      Dim dcsOperationBehavior As DataContractSerializerOperationBehavior = description.Behaviors.Find(Of DataContractSerializerOperationBehavior)()

      If Not dcsOperationBehavior Is Nothing Then
        description.Behaviors.Remove(dcsOperationBehavior)
        description.Behaviors.Add(New NetDataContractOperationBehavior(description))
      End If
    End Sub
  End Class
End Namespace
