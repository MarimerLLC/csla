
Namespace Serialization.Mobile

  ''' <summary>
  ''' Placeholder for null child objects.
  ''' </summary>
  ''' <remarks></remarks>
  <Serializable()> _
  Public NotInheritable Class NullPlaceholder
    Implements IMobileObject


#Region " Constructors "

    Public Sub New()
      'Nothing
    End Sub

#End Region

#Region " IMobileObject Members "

    Public Sub GetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.GetChildren
      'Nothing
    End Sub

    Public Sub GetState(ByVal info As SerializationInfo) Implements IMobileObject.GetState
      'Nothing
    End Sub

    Public Sub SetChildren(ByVal info As SerializationInfo, ByVal formatter As MobileFormatter) Implements IMobileObject.SetChildren
      'Nothing
    End Sub

    Public Sub SetState(ByVal info As SerializationInfo) Implements IMobileObject.SetState
      'Nothing
    End Sub

#End Region
    
  End Class

End Namespace

