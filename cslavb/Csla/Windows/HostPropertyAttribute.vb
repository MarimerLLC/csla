Namespace Windows

  ''' <summary>
  ''' HostPropertyAttribute is used on components to 
  ''' indicate the property on the component that is to be used as the 
  ''' parent container control in conjunction with HostComponentDesigner.
  ''' </summary>
  Public Class HostPropertyAttribute
    Inherits Attribute

#Region "Property Fields"

    Private _hostPropertyName As String = String.Empty

#End Region

#Region "Properties"

    ''' <summary>
    ''' HostPropertyName gets the host property name.
    ''' </summary>
    Public ReadOnly Property HostPropertyName() As String
      Get
        Return (_hostPropertyName)
      End Get
    End Property

#End Region

#Region "Constructors"

    ''' <summary>
    ''' Constructor creates a new HostPropertyAttribute object instance using the information supplied.
    ''' </summary>
    ''' <param name="hostPropertyName">The name of the host property.</param>
    Public Sub New(ByVal hostPropertyName As String)
      _hostPropertyName = hostPropertyName
    End Sub

#End Region

  End Class

End Namespace
