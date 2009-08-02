Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports Csla.Core

Namespace Linq

  Friend Class CslaQueryProvider(Of T As BusinessListBase(Of T, C), C As Core.IEditableBusinessObject)

    Implements IQueryProvider

    Private Sub New()
    End Sub

    Public Sub New(ByVal parent As BusinessListBase(Of T, C))
      _parent = parent
    End Sub

    Private _parent As BusinessListBase(Of T, C)
    Private _filter As LinqBindingList(Of C)

    Private Function Eval(ByVal ex As Expression) As Object
      If TypeOf ex Is ConstantExpression Then
        Return (TryCast(ex, ConstantExpression)).Value
      End If
      Dim lambdax As LambdaExpression = Expression.Lambda(ex)
      Return lambdax.Compile().DynamicInvoke()
    End Function

    Private Function Compile(ByVal ex As Expression) As Object
      Dim callHolder As UnaryExpression = TryCast(ex, UnaryExpression)
      If callHolder IsNot Nothing Then
        Dim theCall As LambdaExpression = TryCast(callHolder.Operand, LambdaExpression)
        If theCall IsNot Nothing Then
          Return theCall.Compile()
        End If
      End If
      Return Eval(ex)
    End Function

    Private Function MethodsEquivalent(ByVal mex As MethodCallExpression, ByVal info As MethodInfo) As Boolean
      Dim parms() As ParameterInfo = info.GetParameters()

      If info.Name = mex.Method.Name AndAlso mex.Arguments.Count = parms.Length Then
        Dim arguments() As Object = New Object(mex.Arguments.Count - 1) {}
        For i As Integer = 0 To mex.Arguments.Count - 1
          arguments(i) = Eval(mex.Arguments(i))
        Next i
        For i As Integer = 0 To parms.Length - 1
          'for each parameter, determine if the type is supported
          If parms(i).ParameterType.Name = arguments(i).GetType().Name Then
            Continue For
          End If
          If parms(i).ParameterType.IsGenericParameter Then
            Continue For
          End If
          If arguments(i).GetType().IsGenericType AndAlso parms(i).ParameterType.Name.StartsWith("Func") Then
            Dim genArgs() As Type = arguments(i).GetType().GetGenericArguments()
            If parms(i).ParameterType.Name = genArgs(0).Name Then
              If genArgs(0).IsGenericType Then

                'check if the second level
                Dim genArgsLevel2 = genArgs(0).GetGenericArguments()
                Dim parmsArgsLevel2 = parms(i).ParameterType.GetGenericArguments()
                For j = 0 To genArgsLevel2.Length - 1
                  If mex.Method.Name = "GroupJoin" Then

                    'Matching on GroupJoin is a complex case where there are only two possibilities for a method match, and its by parameter
                    'count - so we can optimize here
                    If genArgsLevel2(j).Name <> parmsArgsLevel2(j).Name AndAlso Not parmsArgsLevel2(j).IsGenericParameter Then
                      Return False
                    End If
                  Else
                    If genArgsLevel2(j).GetType() IsNot parmsArgsLevel2(j).GetType() AndAlso Not parmsArgsLevel2(j).IsGenericParameter Then
                      Return False
                    End If
                  End If
                Next

              End If
              Continue For
            End If
          End If
          Dim supported As Boolean = False
          Dim supportedInterfaces() As Type = arguments(i).GetType().GetInterfaces()
          For j As Integer = 0 To supportedInterfaces.Length - 1
            If parms(i).ParameterType.Name = supportedInterfaces(j).Name Then
              supported = True
              Exit For
            End If
          Next j
          If (Not supported) Then
            Return False
          End If
        Next i
        Return True
      Else
        Return False
      End If

    End Function

#Region "IQueryProvider Members"

    Public Function CreateQuery(Of TElement)(ByVal expression As Expression) As IQueryable(Of TElement) Implements IQueryProvider.CreateQuery
      Try
        Dim mex As MethodCallExpression = TryCast(expression, MethodCallExpression)
        If GetType(TElement) Is GetType(C) AndAlso mex.Method.Name = "Where" AndAlso _filter Is Nothing Then
          _filter = New LinqBindingList(Of C)(_parent, Me, expression)
          _filter.BuildFilterIndex()
          Return CType(_filter, IQueryable(Of TElement))

        ElseIf GetType(TElement) Is GetType(C) AndAlso mex.Method.Name.StartsWith("OrderBy") AndAlso _filter Is Nothing Then
          'raw sort without previous where
          _filter = New LinqBindingList(Of C)(_parent, Me, Nothing)
          _filter.SortByExpression(expression)
          Return CType(_filter, IOrderedQueryable(Of TElement))

        ElseIf GetType(TElement) Is GetType(C) AndAlso mex.Method.Name.StartsWith("OrderBy") Then
          'sort of previous where
          _filter.SortByExpression(expression)
          Return CType(_filter, IOrderedQueryable(Of TElement))

        ElseIf GetType(TElement) Is GetType(C) AndAlso mex.Method.Name.StartsWith("ThenBy") AndAlso _filter Is Nothing Then
          'raw sort without previous where
          _filter = New LinqBindingList(Of C)(_parent, Me, Nothing)
          _filter.ThenByExpression(expression)
          Return CType(_filter, IOrderedQueryable(Of TElement))

        ElseIf GetType(TElement) Is GetType(C) AndAlso mex.Method.Name.StartsWith("ThenBy") Then
          'sort of previous where
          _filter.ThenByExpression(expression)
          Return CType(_filter, IOrderedQueryable(Of TElement))

        Else
          'handle non identity projections here
          Select Case mex.Method.Name
            Case "Select"

              Dim selectHolder As UnaryExpression = TryCast(mex.Arguments(1), UnaryExpression)
              Dim theSelect As LambdaExpression = TryCast(selectHolder.Operand, LambdaExpression)

              Dim selectorLambda As Expression(Of Func(Of C, TElement)) = Expressions.Expression.Lambda(Of Func(Of C, TElement))(theSelect.Body, theSelect.Parameters)
              Dim selector As Func(Of C, TElement) = selectorLambda.Compile()

              If _filter Is Nothing Then
                Return _parent.Select(selector).AsQueryable()
              Else
                Return _filter.Select(selector).AsQueryable()
              End If
            Case "Concat"

              'at this point, no more filtering, just move it to a concatenated list of items, which we turn to queryable so that the method considers it ok
              'have to eval on the method to make it not a ParameterExpression, but the actual Enumerable inside
              Return (Queryable.Concat(Of TElement)((TryCast(_filter, IQueryable(Of TElement))).ToList().AsQueryable(), TryCast(Eval(mex.Arguments(1)), IEnumerable(Of TElement))))

              'Case "Where" ' TODO: Since there is nothing in this one I'm commenting it out since c# version works differently

            Case Else
              Dim listFrom As List(Of C)
              If _filter Is Nothing Then
                listFrom = _parent.ToList()
              Else
                listFrom = _filter.ToList(Of C)()
              End If
              Dim listType As Type = GetType(Enumerable)
              Dim listMethods() As MethodInfo = listType.GetMethods()
              Dim paramList As List(Of Object) = New List(Of Object)()
              paramList.Add(listFrom)
              Dim i As Integer = 0
              For Each arg As Object In mex.Arguments

                If i > 0 Then
                  If TypeOf arg Is Expression Then
                    'expressions have to be compiled in order to work with the method call on straight Enumerable
                    'somehow, LINQ to objects itself magically does this.  Reflector shows a mess, so I (Aaron) invented my
                    'own way.  God love unit tests!
                    paramList.Add(Compile(TryCast(arg, Expression)))
                  Else
                    paramList.Add(arg)
                  End If
                End If
                i += 1
              Next arg

              For Each method As MethodInfo In listMethods
                If MethodsEquivalent(mex, method) Then
                  Dim genericArguments() As Type = mex.Method.GetGenericArguments()
                  Dim genericMethodInfo As MethodInfo = method.MakeGenericMethod(genericArguments)
                  Dim testObject = genericMethodInfo.Invoke(Nothing, paramList.ToArray())
                  Dim testObjectQ As IQueryable(Of TElement) = (CType(testObject, IEnumerable(Of TElement))).AsQueryable()
                  Return testObjectQ
                End If
              Next method
              Return Nothing
          End Select
        End If
      Catch tie As System.Reflection.TargetInvocationException
        Throw tie.InnerException
      End Try
    End Function

    Public Function CreateQuery(ByVal expression As Expression) As IQueryable Implements IQueryProvider.CreateQuery
      Dim methodName As String = ""

      'handles OfType call
      Dim elementType As Type = TypeSystem.GetElementType(expression.Type)
      If TypeOf expression Is MethodCallExpression Then
        Dim mex As MethodCallExpression = TryCast(expression, MethodCallExpression)
        methodName = mex.Method.Name
        If methodName = "OfType" OrElse methodName = "Cast" Then
          Dim listType As Type = GetType(Enumerable)
          Dim listFrom As List(Of C) = _parent.ToList()
          Dim paramList As List(Of Object) = New List(Of Object)()
          paramList.Add(listFrom)
          For Each method As MethodInfo In listType.GetMethods()
            If method.Name = methodName Then
              Dim genericArguments() As Type = {mex.Method.GetGenericArguments().First()}
              Dim genericMethodInfo As MethodInfo = method.MakeGenericMethod(genericArguments)
              Dim thingWeGotBack As System.Collections.IEnumerable = CType(genericMethodInfo.Invoke(Nothing, paramList.ToArray()), System.Collections.IEnumerable)
              Return thingWeGotBack.AsQueryable()
            End If
          Next method
        End If

        'JF start change
        '2/7/09 - only gets here if mex is a MethodCallExpresion - else it does what it was originally supposed to do.
        If TypeOf elementType Is C AndAlso methodName = "Where" AndAlso _filter Is Nothing Then
          _filter = New LinqBindingList(Of C)(_parent, Me, expression)
          _filter.BuildFilterIndex()
          Return CType(_filter, IQueryable)

        ElseIf TypeOf elementType Is C AndAlso methodName.StartsWith("OrderBy") AndAlso _filter Is Nothing Then
          'raw sort without previous where
          _filter = New LinqBindingList(Of C)(_parent, Me, Nothing)
          _filter.SortByExpression(expression)
          Return CType(_filter, IQueryable)

        ElseIf TypeOf elementType Is C AndAlso methodName.StartsWith("OrderBy") Then
          'sort of previous where
          _filter.SortByExpression(expression)
          Return CType(_filter, IQueryable)

        ElseIf TypeOf elementType Is C AndAlso methodName.StartsWith("ThenBy") AndAlso _filter Is Nothing Then
          'raw sort without previous where
          _filter = New LinqBindingList(Of C)(_parent, Me, Nothing)
          _filter.ThenByExpression(expression)
          Return CType(_filter, IQueryable)

        ElseIf TypeOf elementType Is C AndAlso methodName.StartsWith("ThenBy") Then
          'sort of previous where
          _filter.ThenByExpression(expression)
          Return CType(_filter, IQueryable)

        End If

      End If
      _filter = New LinqBindingList(Of C)(_parent, Me, expression)
      Return _filter
    End Function

    Public Function Execute(Of TResult)(ByVal expression As Expression) As TResult Implements IQueryProvider.Execute
      Return CType(Me.Execute(expression), TResult)
    End Function

    Public Function Execute(ByVal expression As Expression) As Object Implements IQueryProvider.Execute
      Dim mex As MethodCallExpression = TryCast(expression, MethodCallExpression)

      'convert the enumerated collection to a list
      Dim listFrom As List(Of C)

      If _filter IsNot Nothing Then
        listFrom = _filter.ToList(Of C)()
      Else
        listFrom = _parent.ToList()
      End If
      'we are going to call the Enumerable equivalent so we can use it's provider rather than
      '  re-doing all that work on our own
      Dim listType As Type = GetType(Enumerable)
      Dim listMethods() As MethodInfo = listType.GetMethods()
      Dim paramList As List(Of Object) = New List(Of Object)()

      'we are going to pass this a list of items derived from our filtered list, this will be the equivalent of someList.SomeEnumerableExtensionMethod(someparms);
      '  because its an extension method, however, technically, the first parameter is the thing we are calling with the extension method
      paramList.Add(listFrom)
      Dim i As Integer = 0
      'put each argument from the actual method call passed to us into a param list we are going to send, via reflection, to Enumerable.XXX(YYY)
      For Each arg As Object In mex.Arguments
        If i > 0 Then
          If TypeOf arg Is Expression Then
            'expressions have to be compiled in order to work with the method call on straight Enumerable
            paramList.Add(Compile(TryCast(arg, Expression)))
          Else
            paramList.Add(arg)
          End If
        End If
        i += 1
      Next arg
      'now, the happy task of manually finding the right method
      For Each method As MethodInfo In listMethods
        'MethodsEquivalent goes through the nastiness of seeing whether a given MethodCAllExpression maps to a given MethodInfo.  
        If MethodsEquivalent(mex, method) Then
          'the Enumerable call is a generic method call, so deal with that
          Dim genericArguments = mex.Method.GetGenericArguments().Take(method.GetGenericArguments().Length)
          Dim genericMethodInfo As MethodInfo = method.MakeGenericMethod(genericArguments.ToArray())
          Try
            'pray.  If something is going to break, it will do so here
            Return genericMethodInfo.Invoke(Nothing, paramList.ToArray())
          Catch tie As TargetInvocationException
            Throw tie.InnerException
          End Try

        End If
      Next method
      Return Nothing

    End Function

#End Region

  End Class

End Namespace