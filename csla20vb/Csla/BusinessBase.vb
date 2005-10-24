''' <summary>
''' This is the base class from which most business objects
''' will be derived.
''' </summary>
''' <remarks>
''' <para>
''' This class is the core of the CSLA .NET framework. To create
''' a business object, inherit from this class.
''' </para><para>
''' Please refer to 'Expert One-on-One VB.NET Business Objects' for
''' full details on the use of this base class to create business
''' objects.
''' </para>
''' </remarks>
<Serializable()> _
Public MustInherit Class BusinessBase(Of T As BusinessBase(Of T))
  Inherits Core.BusinessBase

#Region " Object ID Value "

  Protected MustOverride Function GetIdValue() As Object

#End Region

#Region " System.Object Overrides "

  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is T Then
      Return DirectCast(obj, T).GetIdValue.Equals(GetIdValue)

    Else
      Return False
    End If

  End Function

  Public Overrides Function GetHashCode() As Integer

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.GetHashCode

  End Function

  Public Overrides Function ToString() As String

    Dim id As Object = GetIdValue()
    If id Is Nothing Then
      Throw New ArgumentException(My.Resources.GetIdValueCantBeNull)
    End If
    Return id.ToString

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
  ''' If <see cref="P:Csla.BusinessBase.IsDeleted" /> is True the object
  ''' will be deleted. Otherwise, if <see cref="P:Csla.BusinessBase.IsNew" /> 
  ''' is True the object will be inserted. Otherwise the object's data will 
  ''' be updated in the database.
  ''' </para><para>
  ''' All this is contingent on <see cref="P:Csla.BusinessBase.IsDirty" />. If
  ''' this value is False, no data operation occurs. It is also contingent on
  ''' <see cref="P:Csla.BusinessBase.IsValid" />. If this value is False an
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
      Throw New NotSupportedException(My.Resources.NoSaveChildException)
    End If

    If EditLevel > 0 Then
      Throw New Validation.ValidationException(My.Resources.NoSaveEditingException)
    End If

    If Not IsValid Then
      Throw New Validation.ValidationException(My.Resources.NoSaveInvalidException)
    End If

    If IsDirty Then
      Return DirectCast(DataPortal.Update(Me), T)
    Else
      Return DirectCast(Me, T)
    End If

  End Function

  ''' <summary>
  ''' Saves the object to the database, forcing
  ''' IsNew to False and IsDirty to True.
  ''' </summary>
  ''' <param name="forceUpdate">
  ''' If True, triggers overriding IsNew and IsDirty. 
  ''' If False then it is the same as calling Save().
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
      MarkDirty()
    End If
    Return Me.Save()

  End Function

#End Region

End Class