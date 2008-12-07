Imports System

Namespace Server

  Friend Interface IDataPortalTarget

    Sub MarkAsChild()
    Sub MarkNew()
    Sub MarkOld()
    Sub DataPortal_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)
    Sub DataPortal_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)
    Sub DataPortal_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)
    Sub Child_OnDataPortalInvoke(ByVal e As DataPortalEventArgs)
    Sub Child_OnDataPortalInvokeComplete(ByVal e As DataPortalEventArgs)
    Sub Child_OnDataPortalException(ByVal e As DataPortalEventArgs, ByVal ex As Exception)

  End Interface

End Namespace