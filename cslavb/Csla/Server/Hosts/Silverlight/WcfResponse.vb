Imports System
Imports System.Runtime.Serialization

Namespace Server.Hosts.Silverlight

    ''' <summary>
    ''' Response message returned from the 
    ''' Silverlight WCF data portal methods.
    ''' </summary>
    <DataContract()> _
    Public Class WcfResponse

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

        Private _ErrorData As WcfErrorInfo
        ''' <summary>
        ''' Serialized error/exception data.
        ''' </summary>
        <DataMember()> _
        Public Property ErrorData() As WcfErrorInfo
            Get
                Return _ErrorData
            End Get
            Set(ByVal value As WcfErrorInfo)
                _ErrorData = value
            End Set
        End Property

        Private _GlobalContext() As Byte
        ''' <summary>
        ''' Serialized global context data.
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

    End Class

End Namespace

