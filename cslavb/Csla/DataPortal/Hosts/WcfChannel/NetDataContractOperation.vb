Imports System.ServiceModel
Imports System.Runtime.Serialization
Imports System.Xml
Imports System.ServiceModel.Description

Namespace Server.Hosts.WcfChannel
  ''' <summary>
  ''' Override the DataContract serialization behavior to
  ''' use the <see cref="NetDataContractSerializer"/>.
  ''' </summary>
  Public Class NetDataContractOperationBehavior
    Inherits DataContractSerializerOperationBehavior
#Region "Constructors"

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="operation">Operation description.</param>
    Public Sub New(ByVal operation As OperationDescription)
      MyBase.New(operation)
    End Sub

    ''' <summary>
    ''' Create new instance of object.
    ''' </summary>
    ''' <param name="operation">Operation description.</param>
    ''' <param name="dataContractFormatAttribute">Data contract attribute object.</param>
    Public Sub New(ByVal operation As OperationDescription, ByVal dataContractFormatAttribute As DataContractFormatAttribute)
      MyBase.New(operation, dataContractFormatAttribute)
    End Sub

#End Region

#Region "Overrides"

    ''' <summary>
    ''' Overrided CreateSerializer to return an XmlObjectSerializer which is capable of 
    ''' preserving the object references.
    ''' </summary>
    Public Overrides Function CreateSerializer(ByVal type As Type, ByVal name As String, ByVal ns As String, ByVal knownTypes As IList(Of Type)) As XmlObjectSerializer
      Return New NetDataContractSerializer(name, ns)
    End Function

    ''' <summary>
    ''' Overrided CreateSerializer to return an XmlObjectSerializer which is capable of 
    ''' preserving the object references.
    ''' </summary>
    Public Overrides Function CreateSerializer(ByVal type As Type, ByVal name As XmlDictionaryString, ByVal ns As XmlDictionaryString, ByVal knownTypes As IList(Of Type)) As XmlObjectSerializer
      Return New NetDataContractSerializer(name, ns)
    End Function

#End Region
  End Class
End Namespace
