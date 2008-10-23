Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text

Namespace Core
  ' In case you need more flexibility than the standard delegates. 
  Public Delegate Sub AsyncFactoryDelegate(ByVal completed As [Delegate], ByVal parameters As Object())

  ' Standard factory delegates 
  Public Delegate Sub AsyncFactoryDelegate(Of R)(ByVal completed As EventHandler(Of DataPortalResult(Of R)))
  Public Delegate Sub AsyncFactoryDelegate(Of R, T)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg As T)
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2)
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3)
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3, T4)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3, ByVal arg4 As T4)
  Public Delegate Sub AsyncFactoryDelegate(Of R, T1, T2, T3, T4, T5)(ByVal completed As EventHandler(Of DataPortalResult(Of R)), ByVal arg1 As T1, ByVal arg2 As T2, ByVal arg3 As T3, ByVal arg4 As T4, ByVal arg5 As T5)
End Namespace