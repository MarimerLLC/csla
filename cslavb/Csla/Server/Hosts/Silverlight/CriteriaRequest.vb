Imports System
Imports System.Runtime.Serialization
Imports System.Security.Principal
Imports Csla.Core

Namespace Server.Hosts.Silverlight

    ''' <summary>
    ''' Message sent to the Silverlight
    ''' WCF data portal.
    ''' </summary>
    ''' <remarks></remarks>
    <DataContract()> _
    Public Class CriteriaRequest

        Private _TypeName As String
        ''' <summary>
        ''' Assembly qualified name of the 
        ''' business object type to create.
        ''' </summary>
        <DataMember()> _
        Public Property TypeName() As String
            Get
                Return _TypeName
            End Get
            Set(ByVal value As String)
                _TypeName = value
            End Set
        End Property

        Private _CriteriaData() As Byte

        ''' <summary>
        ''' Serialized data for the criteria object.
        ''' </summary>
        <DataMember()> _
        Public Property CriteriaData() As Byte()
            Get
                Return _CriteriaData
            End Get
            Set(ByVal value As Byte())
                _CriteriaData = value
            End Set
        End Property

        Private _Principal() As Byte
        ''' <summary>
        ''' Serialized data for the principal object.
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
        ''' Serialized data for the global context object.
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
        ''' Serialized data for the client context object.
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

