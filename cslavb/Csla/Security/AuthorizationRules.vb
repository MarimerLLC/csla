Imports System.ComponentModel

Namespace Security

  ''' <summary>
  ''' Maintains a list of allowed and denied user roles
  ''' for each property.
  ''' </summary>
  ''' <remarks></remarks>
  Public Class AuthorizationRules

    Private mBusinessObjectType As Type
    Private mTypeRules As AuthorizationRulesManager
    Private mInstanceRules As AuthorizationRulesManager

    ''' <summary>
    ''' Creates an instance of the object, initializing
    ''' it with the business object type.
    ''' </summary>
    ''' <param name="businessObjectType">
    ''' Type of the business object to which the rules
    ''' apply.
    ''' </param>
    Public Sub New(ByVal businessObjectType As Type)

      mBusinessObjectType = businessObjectType

    End Sub

    Private ReadOnly Property InstanceRules() _
      As AuthorizationRulesManager
      Get
        If mInstanceRules Is Nothing Then
          mInstanceRules = New AuthorizationRulesManager
        End If
        Return mInstanceRules
      End Get
    End Property

    Private ReadOnly Property TypeRules() _
      As AuthorizationRulesManager
      Get
        If mTypeRules Is Nothing Then
          mTypeRules = SharedAuthorizationRules.GetManager(mBusinessObjectType, True)
        End If
        Return mTypeRules
      End Get
    End Property

#Region " Add Per-Instance Roles "

    ''' <summary>
    ''' Specify the roles allowed to read a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceAllowRead( _
      ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied read access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceDenyRead(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to write a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceAllowWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied write access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceDenyWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to execute a given
    ''' method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    ''' <param name="roles">List of roles granted read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceAllowExecute( _
      ByVal methodName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(methodName)
      For Each item As String In roles
        currentRoles.ExecuteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied the right to execute 
    ''' a given method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    ''' <param name="roles">List of roles denied read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub InstanceDenyExecute(ByVal methodName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = InstanceRules.GetRolesForProperty(methodName)
      For Each item As String In roles
        currentRoles.ExecuteDenied.Add(item)
      Next

    End Sub

#End Region

#Region " Add Per-Type Roles "

    ''' <summary>
    ''' Specify the roles allowed to read a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the property.</param>
    ''' <param name="roles">List of roles granted read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowRead( _
      ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.ReadAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to read a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowRead( _
      ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied read access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the property.</param>
    ''' <param name="roles">List of roles denied read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyRead(ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.ReadDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied read access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied read access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyRead(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.ReadDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to write a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the property.</param>
    ''' <param name="roles">List of roles granted write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowWrite(ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.WriteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to write a given
    ''' property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles granted write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied write access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the property.</param>
    ''' <param name="roles">List of roles denied write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyWrite(ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied write access to 
    ''' a given property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    ''' <param name="roles">List of roles denied write access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyWrite(ByVal propertyName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyName)
      For Each item As String In roles
        currentRoles.WriteDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to execute a given
    ''' method.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the method.</param>
    ''' <param name="roles">List of roles granted execute access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowExecute( _
      ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.ExecuteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles allowed to execute a given
    ''' method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    ''' <param name="roles">List of roles granted execute access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of allowed roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub AllowExecute( _
      ByVal methodName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(methodName)
      For Each item As String In roles
        currentRoles.ExecuteAllowed.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied the right to execute 
    ''' a given method.
    ''' </summary>
    ''' <param name="propertyInfo">PropertyInfo for the method.</param>
    ''' <param name="roles">List of roles denied execute access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyExecute(ByVal propertyInfo As Core.IPropertyInfo, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(propertyInfo.Name)
      For Each item As String In roles
        currentRoles.ExecuteDenied.Add(item)
      Next

    End Sub

    ''' <summary>
    ''' Specify the roles denied the right to execute 
    ''' a given method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    ''' <param name="roles">List of roles denied execute access.</param>
    ''' <remarks>
    ''' This method may be called multiple times, with the roles in
    ''' each call being added to the end of the list of denied roles.
    ''' In other words, each call is cumulative, adding more roles
    ''' to the list.
    ''' </remarks>
    Public Sub DenyExecute(ByVal methodName As String, ByVal ParamArray roles() As String)

      Dim currentRoles As RolesForProperty = TypeRules.GetRolesForProperty(methodName)
      For Each item As String In roles
        currentRoles.ExecuteDenied.Add(item)
      Next

    End Sub

#End Region

#Region " Check Roles "

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles granted read access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasReadAllowedRoles( _
      ByVal propertyName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(propertyName).ReadAllowed.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).ReadAllowed.Count > 0
      End If

      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly allowed to read the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsReadAllowed(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(propertyName).IsReadAllowed(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).IsReadAllowed(user)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles denied read access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasReadDeniedRoles(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(propertyName).ReadDenied.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).ReadDenied.Count > 0
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly denied read access to the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsReadDenied(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(propertyName).IsReadDenied(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).IsReadDenied(user)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles granted write access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasWriteAllowedRoles(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(propertyName).WriteAllowed.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).WriteAllowed.Count > 0
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly allowed to set the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsWriteAllowed(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(propertyName).IsWriteAllowed(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).IsWriteAllowed(user)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles denied write access.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function HasWriteDeniedRoles(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(propertyName).WriteDenied.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).WriteDenied.Count > 0
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly denied write access to the property.
    ''' </summary>
    ''' <param name="propertyName">Name of the property.</param>
    Public Function IsWriteDenied(ByVal propertyName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(propertyName).IsWriteDenied(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(propertyName).IsWriteDenied(user)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles granted execute access.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    Public Function HasExecuteAllowedRoles( _
      ByVal methodName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(methodName).ExecuteAllowed.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(methodName).ExecuteAllowed.Count > 0
      End If

      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly allowed to execute the method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    Public Function IsExecuteAllowed(ByVal methodName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(methodName).IsExecuteAllowed(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(methodName).IsExecuteAllowed(user)
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the property has a list
    ''' of roles denied execute access.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    Public Function HasExecuteDeniedRoles(ByVal methodName As String) As Boolean

      Dim result As Boolean
      If InstanceRules.GetRolesForProperty(methodName).ExecuteDenied.Count > 0 Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(methodName).ExecuteDenied.Count > 0
      End If
      Return result

    End Function

    ''' <summary>
    ''' Indicates whether the current user as defined by
    ''' <see cref="Csla.ApplicationContext.User" />
    ''' is explicitly denied execute access to the method.
    ''' </summary>
    ''' <param name="methodName">Name of the method.</param>
    Public Function IsExecuteDenied(ByVal methodName As String) As Boolean

      Dim result As Boolean
      Dim user As System.Security.Principal.IPrincipal = ApplicationContext.User
      If InstanceRules.GetRolesForProperty(methodName).IsExecuteDenied(user) Then
        result = True

      Else
        result = TypeRules.GetRolesForProperty(methodName).IsExecuteDenied(user)
      End If
      Return result

    End Function

#End Region

  End Class

End Namespace
