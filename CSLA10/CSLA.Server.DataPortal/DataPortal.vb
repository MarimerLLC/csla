Imports System.Reflection
Imports System.Security.Principal
Imports System.Configuration

''' <summary>
''' Implements the server-side DataPortal as discussed
''' in Chapter 5.
''' </summary>
Public Class DataPortal
  Inherits MarshalByRefObject

#Region " Data Access "

  ''' <summary>
  ''' Called by the client-side DataPortal to create a new object.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <param name="Principal">The user's principal object (if using CSLA .NET security).</param>
  ''' <returns>A populated business object.</returns>
  Public Function Create(ByVal Criteria As Object, ByVal Principal As Object) As Object

    SetPrincipal(Principal)

    ' create an instance of the business object
    Dim obj As Object = CreateBusinessObject(Criteria)

    ' tell the business object to fetch its data
    CallMethod(obj, "DataPortal_Create", Criteria)
    ' return the populated business object as a result
    Return obj

  End Function

  ''' <summary>
  ''' Called by the client-side DataProtal to retrieve an object.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <param name="Principal">The user's principal object (if using CSLA .NET security).</param>
  ''' <returns>A populated business object.</returns>
  Public Function Fetch(ByVal Criteria As Object, ByVal Principal As Object) As Object
    SetPrincipal(Principal)

    ' create an instance of the business object
    Dim obj As Object = CreateBusinessObject(Criteria)

    ' tell the business object to fetch its data
    CallMethod(obj, "DataPortal_Fetch", Criteria)

    ' return the populated business object as a result
    Return obj

  End Function

  ''' <summary>
  ''' Called by the client-side DataPortal to update an object.
  ''' </summary>
  ''' <param name="obj">A reference to the object being updated.</param>
  ''' <param name="Principal">The user's principal object (if using CSLA .NET security).</param>
  ''' <returns>A reference to the newly updated object.</returns>
  Public Function Update(ByVal obj As Object, ByVal Principal As Object) As Object

    SetPrincipal(Principal)

    ' tell the business object to update itself
    CallMethod(obj, "DataPortal_Update")
    Return obj

  End Function

  ''' <summary>
  ''' Called by the client-side DataPortal to delete an object.
  ''' </summary>
  ''' <param name="Criteria">Object-specific criteria.</param>
  ''' <param name="Principal">The user's principal object (if using CSLA .NET security).</param>
  Public Sub Delete(ByVal Criteria As Object, ByVal Principal As Object)

    SetPrincipal(Principal)

    ' create an instance of the business object
    Dim obj As Object = CreateBusinessObject(Criteria)

    ' tell the business object to delete itself
    CallMethod(obj, "DataPortal_Delete", Criteria)

  End Sub

#End Region

#Region " Security "

  Private Function AUTHENTICATION() As String

    Return ConfigurationSettings.AppSettings("Authentication")

  End Function

  Private Sub SetPrincipal(ByVal Principal As Object)
    If AUTHENTICATION() = "Windows" Then
      ' when using integrated security, Principal must be Nothing
      ' and we need to set our policy to use the Windows principal
      If Principal Is Nothing Then
        AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal)
        Exit Sub

      Else
        Throw New Security.SecurityException("No principal object should be passed to DataPortal when using Windows integrated security")
      End If
    End If

    ' we expect Principal to be of type BusinessPrincipal, but
    ' we can't enforce that since it causes a circular reference
    ' with the business library so instead we must use type Object
    ' for the parameter, so here we do a check on the type of the
    ' parameter
    If Principal.ToString = "CSLA.Security.BusinessPrincipal" Then
      ' see if our current principal is
      ' different from the caller's principal
      If Not ReferenceEquals(Principal, System.Threading.Thread.CurrentPrincipal) Then
        ' the caller had a different principal, so change ours to
        ' match the caller's so all our objects use the caller's
        ' security
        System.Threading.Thread.CurrentPrincipal = CType(Principal, IPrincipal)
      End If

    Else
      Throw New Security.SecurityException("Principal must be of type BusinessPrincipal, not " & Principal.ToString)
    End If

  End Sub

#End Region

#Region " Creating the business object "

  Private Function CreateBusinessObject(ByVal Criteria As Object) As Object

    Dim businessType As Type

    If Criteria.GetType.IsSubclassOf(GetType(CriteriaBase)) Then
      ' get the type of the actual business object
      ' from CriteriaBase (using the new scheme)
      businessType = CType(Criteria, CriteriaBase).ObjectType

    Else
      ' get the type of the actual business object
      ' based on the nested class scheme in the book
      businessType = Criteria.GetType.DeclaringType
    End If

    ' create an instance of the business object
    Return Activator.CreateInstance(businessType, True)

  End Function

#End Region

#Region " Calling a method "

  Private Function CallMethod(ByVal obj As Object, ByVal method As String, ByVal ParamArray params() As Object) As Object

    ' call a private method on the object
    'Dim t As Type = obj.GetType
    Dim info As MethodInfo = GetMethod(obj.GetType, method)
    Dim result As Object

    Try
      result = info.Invoke(obj, params)

    Catch e As Exception
      Throw e.GetBaseException
    End Try
    Return result

  End Function

  Private Function GetMethod(ByVal ObjectType As Type, ByVal method As String) As MethodInfo

    Return ObjectType.GetMethod(method, _
      BindingFlags.FlattenHierarchy Or _
      BindingFlags.Instance Or _
      BindingFlags.Public Or _
      BindingFlags.NonPublic)

  End Function

#End Region

End Class
