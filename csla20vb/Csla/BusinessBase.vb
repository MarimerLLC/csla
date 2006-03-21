''' <summary>
''' This is the base class from which most business objects
''' will be derived.
''' </summary>
''' <remarks>
''' <para>
''' This class is the core of the CSLA .NET framework. To create
''' a business object, inherit from this class.
''' </para><para>
''' Please refer to 'Expert VB 2005 Business Objects' for
''' full details on the use of this base class to create business
''' objects.
''' </para>
''' </remarks>
''' <typeparam name="T">Type of the business object being defined.</typeparam>
<Serializable()> _
Public MustInherit Class BusinessBase(Of T As BusinessBase(Of T))
  Inherits Core.BusinessBase

#Region " Object ID Value "

  ''' <summary>
  ''' Override this method to return a unique identifying
  ''' value for this object.
  ''' </summary>
  ''' <remarks>
  ''' If you can not provide a unique identifying value, it
  ''' is best if you can generate such a unique value (even
  ''' temporarily). If you can not do that, then return 
  ''' <see langword="Nothing"/> and then manually override the
  ''' <see cref="Equals"/>, <see cref="GetHashCode"/> and
  ''' <see cref="ToString"/> methods in your business object.
  ''' </remarks>
  Protected MustOverride Function GetIdValue() As Object

#End Region

#Region " System.Object Overrides "

  ''' <summary>
  ''' Compares this object for equality with another object, using
  ''' the results of <see cref="GetIdValue"/> to determine
  ''' equality.
  ''' </summary>
  ''' <param name="obj">The object to be compared.</param>
  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is T Then
      Dim id As Object = GetIdValue()
      If id Is Nothing Then
        Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
      End If
      Return DirectCast(obj, T).GetIdValue.Equals(id)

    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Returns a hash code value for this object, based on
  ''' the results of <see cref="GetIdValue"/>.
  ''' </summary>
  Public Overrides Function GetHashCode() As Integer

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.GetHashCode

  End Function

  ''' <summary>
  ''' Returns a text representation of this object by
  ''' returning the <see cref="GetIdValue"/> value
  ''' in text form.
  ''' </summary>
  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.ToString

  End Function

#End Region

#Region " Clone "

  ''' <summary>
  ''' Creates a clone of the object.
  ''' </summary>
  ''' <returns>
  ''' A new object containing the exact data of the original object.
  ''' </returns>
  Public Overridable Function Clone() As T

    Return DirectCast(GetClone(), T)

  End Function

#End Region

#Region " Data Access "

  ''' <summary>
  ''' Saves the object to the database.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' Calling this method starts the save operation, causing the object
  ''' to be inserted, updated or deleted within the database based on the
  ''' object's current state.
  ''' </para><para>
  ''' If <see cref="Core.BusinessBase.IsDeleted" /> is <see langword="true"/>
  ''' the object will be deleted. Otherwise, if <see cref="Core.BusinessBase.IsNew" /> 
  ''' is <see langword="true"/> the object will be inserted. 
  ''' Otherwise the object's data will be updated in the database.
  ''' </para><para>
  ''' All this is contingent on <see cref="Core.BusinessBase.IsDirty" />. If
  ''' this value is <see langword="false"/>, no data operation occurs. 
  ''' It is also contingent on <see cref="Core.BusinessBase.IsValid" />. 
  ''' If this value is <see langword="false"/> an
  ''' exception will be thrown to indicate that the UI attempted to save an
  ''' invalid object.
  ''' </para><para>
  ''' It is important to note that this method returns a new version of the
  ''' business object that contains any data updated during the save operation.
  ''' You MUST update all object references to use this new version of the
  ''' business object in order to have access to the correct object data.
  ''' </para><para>
  ''' You can override this method to add your own custom behaviors to the save
  ''' operation. For instance, you may add some security checks to make sure
  ''' the user can save the object. If all security checks pass, you would then
  ''' invoke the base Save method via <c>MyBase.Save()</c>.
  ''' </para>
  ''' </remarks>
  ''' <returns>A new object containing the saved values.</returns>
  Public Overridable Function Save() As T
    If Me.IsChild Then
      Throw New NotSupportedException( _
        My.Resources.NoSaveChildException)
    End If

    If EditLevel > 0 Then
      Throw New Validation.ValidationException( _
        My.Resources.NoSaveEditingException)
    End If

    If Not IsValid Then
      Throw New Validation.ValidationException( _
        My.Resources.NoSaveInvalidException)
    End If

    If IsDirty Then
      Return DirectCast(DataPortal.Update(Me), T)
    Else
      Return DirectCast(Me, T)
    End If

  End Function

  ''' <summary>
  ''' Saves the object to the database, forcing
  ''' IsNew to <see langword="false"/> and IsDirty to True.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If <see langword="true"/>, triggers overriding IsNew and IsDirty. 
  ''' If <see langword="false"/> then it is the same as calling Save().
  ''' </param>
  ''' <returns>A new object containing the saved values.</returns>
  ''' <remarks>
  ''' This overload is designed for use in web applications
  ''' when implementing the Update method in your 
  ''' data wrapper object.
  ''' </remarks>
  Public Function Save(ByVal forceUpdate As Boolean) As T

    If forceUpdate AndAlso IsNew Then
      ' mark the object as old - which makes it
      ' not dirty
      MarkOld()
      ' now mark the object as dirty so it can save
      MarkDirty(True)
    End If
    Return Me.Save()

  End Function

#End Region

End Class