''' <summary>
''' Marks a DataPortal_XYZ method to run within
''' the specified transactional context.
''' </summary>
''' <remarks>
''' <para>
''' Each business object method may be marked with this attribute
''' to indicate which type of transactional technology should
''' be used by the server-side DataPortal. The possible options
''' are listed in the
''' <see cref="TransactionalTypes">TransactionalTypes</see> enum.
''' </para><para>
''' If the Transactional attribute is not applied to a 
''' DataPortal_XYZ method then the
''' <see cref="TransactionalTypes.Manual">Manual</see> option
''' is assumed.
''' </para><para>
''' If the Transactional attribute is applied with no explicit
''' choice for transactionType then the
''' <see cref="TransactionalTypes.EnterpriseServices">EnterpriseServices</see> 
''' option is assumed.
''' </para><para>
''' Both the EnterpriseServices and TransactionScope options provide
''' 2-phase distributed transactional support.
''' </para>
''' </remarks>
<AttributeUsage(AttributeTargets.Method)> _
Public NotInheritable Class TransactionalAttribute
  Inherits Attribute

  Private mType As TransactionalTypes

  ''' <summary>
  ''' Marks a method to run within a COM+
  ''' transactional context.
  ''' </summary>
  Public Sub New()
    mType = TransactionalTypes.EnterpriseServices
  End Sub

  ''' <summary>
  ''' Marks a method to run within the specified
  ''' type of transactional context.
  ''' </summary>
  ''' <param name="transactionType">
  ''' Specifies the transactional context within which the
  ''' method should run.</param>
  Public Sub New(ByVal transactionType As TransactionalTypes)
    mType = transactionType
  End Sub

  ''' <summary>
  ''' Gets the type of transaction requested by the
  ''' business object method.
  ''' </summary>
  Public ReadOnly Property TransactionType() As TransactionalTypes
    Get
      Return mType
    End Get
  End Property

End Class
