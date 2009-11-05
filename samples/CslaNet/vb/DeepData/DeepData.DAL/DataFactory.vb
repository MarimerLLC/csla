Imports System.Configuration.ConfigurationManager

Public Class DataFactory

  Public Function GetOrderDataObject() As OrderData

    Dim dalType As Type = Type.GetType(AppSettings("OrderData"), True, True)
    Return CType(Activator.CreateInstance(dalType), OrderData)

  End Function

End Class
