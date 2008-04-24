''' <summary>
''' Base type from which Criteria classes can be
''' derived in a business class. 
''' </summary>
<Serializable()> _
Public MustInherit Class CriteriaBase

  Implements ICriteria

  Private _objectType As Type

  ''' <summary>
  ''' Type of the business object to be instantiated by
  ''' the server-side DataPortal. 
  ''' </summary>
  Public ReadOnly Property ObjectType() As Type Implements ICriteria.ObjectType
    Get
      Return _objectType
    End Get
  End Property

  ''' <summary>
  ''' Initializes CriteriaBase with the type of
  ''' business object to be created by the DataPortal.
  ''' </summary>
  ''' <param name="type">The type of the
  ''' business object the data portal should create.</param>
  Protected Sub New(ByVal type As Type)
    _objectType = type
  End Sub

End Class
