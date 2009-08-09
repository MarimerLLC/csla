Imports System
Imports System.Runtime.Serialization

Namespace Server.Hosts.Silverlight

  ''' <summary>
  ''' Message containing details about any
  ''' server-side exception.
  ''' </summary>
  <DataContract()> _
  Public Class WcfErrorInfo

    Private _ExceptionTypeName As String
    ''' <summary>
    ''' Type name of the exception object.
    ''' </summary>
    <DataMember()> _
    Public Property ExceptionTypeName() As String
      Get
        Return _ExceptionTypeName
      End Get
      Set(ByVal value As String)
        _ExceptionTypeName = value
      End Set
    End Property

    Private _Message As String
    ''' <summary>
    ''' Message from the exception object.
    ''' </summary>
    <DataMember()> _
    Public Property Message() As String
      Get
        Return _Message
      End Get
      Set(ByVal value As String)
        _Message = value
      End Set
    End Property

    Private _StackTrace As String
    ''' <summary>
    ''' Stack trace from the exception object.
    ''' </summary>
    <DataMember()> _
    Public Property StackTrace() As String
      Get
        Return _StackTrace
      End Get
      Set(ByVal value As String)
        _StackTrace = value
      End Set
    End Property

    Private _Source As String
    ''' <summary>
    ''' Source of the exception object.
    ''' </summary>
    <DataMember()> _
    Public Property Source() As String
      Get
        Return _Source
      End Get
      Set(ByVal value As String)
        _Source = value
      End Set
    End Property

    Private _TargetSiteName As String
    ''' <summary>
    ''' Target site name from the exception object.
    ''' </summary>
    <DataMember()> _
    Public Property TargetSiteName() As String
      Get
        Return _TargetSiteName
      End Get
      Set(ByVal value As String)
        _TargetSiteName = value
      End Set
    End Property

    Private _InnerError As WcfErrorInfo
    ''' <summary>
    ''' WcfErrorInfo object containing information
    ''' about any inner exception of the original
    ''' exception.
    ''' </summary>
    <DataMember()> _
    Public Property InnerError() As WcfErrorInfo
      Get
        Return _InnerError
      End Get
      Set(ByVal value As WcfErrorInfo)
        _InnerError = value
      End Set
    End Property

    Public Sub New(ByVal ex As Exception)
      Me.ExceptionTypeName = ex.GetType().FullName
      Me.Message = ex.Message
      Me.StackTrace = ex.StackTrace
      Me.Source = ex.Source

      If ex.TargetSite IsNot Nothing Then
        Me.TargetSiteName = ex.TargetSite.Name
      End If

      If ex.InnerException IsNot Nothing Then
        Me.InnerError = New WcfErrorInfo(ex.InnerException)
      End If

    End Sub

  End Class

End Namespace

