''' <summary>
''' Base type from which Criteria classes can be
''' derived in a business class. 
''' </summary>
<Serializable()> _
Public MustInherit Class CriteriaBase
  Public ObjectType As Type

  ''' <summary>
  ''' Initializes CriteriaBase with the type of
  ''' business object to be created by the DataPortal.
  ''' </summary>
  Public Sub New(ByVal Type As Type)
    ObjectType = Type
  End Sub

End Class
