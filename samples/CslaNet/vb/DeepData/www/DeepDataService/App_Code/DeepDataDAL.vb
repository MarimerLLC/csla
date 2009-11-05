Option Strict On

Imports System.Collections.Generic
Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols

<WebService(Namespace:="http://ws.lhotka.net/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class DeepDataDAL
  Inherits System.Web.Services.WebService

  <WebMethod()> _
  Public Function GetOrders() As DeepData.DTO.OrderDto()

    Dim df As New DeepData.DAL.DataFactory
    Using dal As DeepData.DAL.OrderData = df.GetOrderDataObject
      Return CType(dal.GetOrders, DeepData.DTO.OrderDto())
    End Using

  End Function

End Class
