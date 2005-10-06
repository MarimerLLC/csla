''' <summary>
''' Base type from which Criteria classes can be
''' derived in a business class. 
''' </summary>
<Serializable()> _
Public MustInherit Class CriteriaBase

  Private mObjectType As Type

  ''' <summary>
  ''' Type of the business object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  Public ReadOnly Property ObjectType() As Type
    Get
      Return mObjectType
    End Get
  End Property

  ''' <summary>
  ''' Initializes CriteriaBase with the type of
  ''' business object to be created by the DataPortal.
  ''' </summary>
  Protected Sub New(ByVal type As Type)
    mObjectType = type
  End Sub

End Class
