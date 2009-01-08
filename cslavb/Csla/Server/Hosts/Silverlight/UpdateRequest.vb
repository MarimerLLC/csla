Imports System
Imports System.Runtime.Serialization
Imports System.Security.Principal

Namespace Server.Hosts.Silverlight

  ''' <summary>
  ''' Message sent to the Silverlight
  ''' WCF data portal.
  ''' </summary>
  <DataContract()> _
  Public Class UpdateRequest

    Private _ObjectData() As Byte
    ''' <summary>
    ''' Serialized object data.
    ''' </summary>
    <DataMember()> _
    Public Property ObjectData() As Byte()
      Get
        Return _ObjectData
      End Get
      Set(ByVal value As Byte())
        _ObjectData = value
      End Set
    End Property

    Private _Principal() As Byte
    ''' <summary>
    ''' Serialized principal object.
    ''' </summary>
    <DataMember()> _
    Public Property Principal() As Byte()
      Get
        Return _Principal
      End Get
      Set(ByVal value As Byte())
        _Principal = value
      End Set
    End Property

    Private _GlobalContext() As Byte
    ''' <summary>
    ''' Serialized global context object.
    ''' </summary>
    <DataMember()> _
    Public Property GlobalContext() As Byte()
      Get
        Return _GlobalContext
      End Get
      Set(ByVal value As Byte())
        _GlobalContext = value
      End Set
    End Property

    Private _ClientContext() As Byte
    ''' <summary>
    ''' Serialized client context object.
    ''' </summary>
    <DataMember()> _
    Public Property ClientContext() As Byte()
      Get
        Return _ClientContext
      End Get
      Set(ByVal value As Byte())
        _ClientContext = value
      End Set
    End Property

  End Class

End Namespace

