using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Csla.Core;

namespace Csla.Linq
{
  internal class CslaQueryProvider<T, C> : IQueryProvider
    where T : BusinessListBase<T, C>
    where C : Core.IEditableBusinessObject
  {
    private CslaQueryProvider() { }

    public CslaQueryProvider(BusinessListBase<T, C> parent)
    {
      _parent = parent;
    }

    private BusinessListBase<T, C> _parent;
    private LinqBindingList<C> _filter;

    private object Eval(Expression ex)
    {
      if (ex is ConstantExpression) return ((ConstantExpression)ex).Value;
      LambdaExpression lambdax = Expression.Lambda(ex);
      return lambdax.Compile().DynamicInvoke();
    }

    private object Compile(Expression ex)
    {
      UnaryExpression callHolder = ex as UnaryExpression;
      if (callHolder != null)
      {
        LambdaExpression theCall = callHolder.Operand as LambdaExpression;
        if (theCall != null)
          return theCall.Compile();
      }
      return Eval(ex);
    }

    private bool MethodsEquivalent(MethodCallExpression mex, MethodInfo info)
    {
      ParameterInfo[] parms = info.GetParameters();

      if (info.Name == mex.Method.Name && mex.Arguments.Count == parms.Length)
      {
        object[] arguments = new object[mex.Arguments.Count];
        for (int i = 0; i < mex.Arguments.Count; i++)
          arguments[i] = Eval(mex.Arguments[i]);
        for (int i = 0; i < parms.Length; i++)
        {
          //for each parameter, determine if the type is supported
          if (parms[i].ParameterType.Name == arguments[i].GetType().Name)
            continue;
          if (parms[i].ParameterType.IsGenericParameter)
            continue;
          if (info.Name == "SelectMany" && (parms[i].ParameterType.Name.StartsWith("IEnumerable")))
            continue;
          if (arguments[i].GetType().IsGenericType && parms[i].ParameterType.Name.StartsWith("Func"))
          {
            Type[] genArgs = arguments[i].GetType().GetGenericArguments();
            if (parms[i].ParameterType.Name == genArgs[0].Name)
            {
              if (genArgs[0].IsGenericType)
              {
                //check if the second level
                var genArgsLevel2 = genArgs[0].GetGenericArguments();
                var parmsArgsLevel2 = parms[i].ParameterType.GetGenericArguments();
                for (int j = 0; j < genArgsLevel2.Length; j++)
                  if (mex.Method.Name == "GroupJoin" || mex.Method.Name == "SelectMany")
                  {
                    //Matching on GroupJoin and SelectMany are complex cases where there are only two possibilities for a method match, and its by parameter
                    //count - so we can optimize here
                    if (genArgsLevel2[j].Name != parmsArgsLevel2[j].Name && !parmsArgsLevel2[j].IsGenericParameter)
                      return false;
                  }
                  else
                  {
                    if (genArgsLevel2[j] != parmsArgsLevel2[j] && !parmsArgsLevel2[j].IsGenericParameter)
                      return false;
                  }
              }
              continue;
            }
          }
          bool supported = false;
          Type[] supportedInterfaces = arguments[i].GetType().GetInterfaces();
          for (int j = 0; j < supportedInterfaces.Length; j++)
            if (parms[i].ParameterType.Name == supportedInterfaces[j].Name)
            {
              supported = true;
              break;
            }
          if (!supported) return false;
        }
        return true;
      }
      else return false;

    }




    #region IQueryProvider Members

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
      try
      {
        MethodCallExpression mex = (MethodCallExpression)expression;
        if (typeof(TElement) == typeof(C) && mex.Method.Name == "Where" && _filter == null)
        {
          _filter = new LinqBindingList<C>(_parent, this, expression);
          _filter.BuildFilterIndex();
          return (IQueryable<TElement>)_filter;
        }
        else if (typeof(TElement) == typeof(C) && mex.Method.Name.StartsWith("OrderBy") && _filter == null)
        {
          //raw sort without previous where
          _filter = new LinqBindingList<C>(_parent, this, null);
          _filter.SortByExpression(expression);
          return (IOrderedQueryable<TElement>)_filter;
        }
        else if (typeof(TElement) == typeof(C) && mex.Method.Name.StartsWith("OrderBy"))
        {
          //sort of previous where
          _filter.SortByExpression(expression);
          return (IOrderedQueryable<TElement>)_filter;
        }
        else if (typeof(TElement) == typeof(C) && mex.Method.Name.StartsWith("ThenBy") && _filter == null)
        {
          //raw sort without previous where
          _filter = new LinqBindingList<C>(_parent, this, null);
          _filter.ThenByExpression(expression);
          return (IOrderedQueryable<TElement>)_filter;
        }
        else if (typeof(TElement) == typeof(C) && mex.Method.Name.StartsWith("ThenBy"))
        {
          //sort of previous where
          _filter.ThenByExpression(expression);
          return (IOrderedQueryable<TElement>)_filter;
        }
        else
        {
          //handle non identity projections here
          switch (mex.Method.Name)
          {
            case "Select":

              UnaryExpression selectHolder = (UnaryExpression)mex.Arguments[1];
              LambdaExpression theSelect = (LambdaExpression)selectHolder.Operand;

              Expression<Func<C, TElement>> selectorLambda
                  = Expression.Lambda<Func<C, TElement>>(theSelect.Body, theSelect.Parameters);
              Func<C, TElement> selector = selectorLambda.Compile();
              if (_filter == null)
                return _parent.Select<C, TElement>(selector).AsQueryable<TElement>();
              else
                return _filter.Select<C, TElement>(selector).AsQueryable<TElement>();
            case "Concat":

              return (
                Queryable.Concat<TElement>(
                //at this point, no more filtering, just move it to a concatenated list of items, which we turn to queryable so that the method considers it ok
                  ((IQueryable<TElement>)_filter).ToList<TElement>().AsQueryable<TElement>(),
                //have to eval on the method to make it not a ParameterExpression, but the actual Enumerable inside
                  (IEnumerable<TElement>)Eval(mex.Arguments[1]))
                );

            case "Where":

            default:
              List<C> listFrom;
              if (_filter == null)
                listFrom = _parent.ToList<C>();
              else
                listFrom = _filter.ToList<C>();
              Type listType = typeof(Enumerable);
              MethodInfo[] listMethods = listType.GetMethods();
              List<object> paramList = new List<object>();
              paramList.Add(listFrom);
              int i = 0;
              foreach (object arg in mex.Arguments)
              {

                if (i > 0)
                  if (arg is Expression)
                    paramList.Add(Compile((Expression)arg));
                  else
                    paramList.Add(arg);
                i++;
              }

              foreach (MethodInfo method in listMethods)
                if (MethodsEquivalent(mex, method))
                {
                  Type[] genericArguments = mex.Method.GetGenericArguments();
                  MethodInfo genericMethodInfo = method.MakeGenericMethod(genericArguments);
                  var testObject = genericMethodInfo.Invoke(null, paramList.ToArray());
                  IQueryable<TElement> testObjectQ = ((IEnumerable<TElement>)testObject).AsQueryable<TElement>();
                  return testObjectQ;
                }
              return null;
          }

        }
      }
      catch (System.Reflection.TargetInvocationException tie)
      {
        throw tie.InnerException;
      }
    }

    public IQueryable CreateQuery(Expression expression)
    {
      string methodName = "";
      //handles OfType call
      Type elementType = TypeSystem.GetElementType(expression.Type);
      if (expression is MethodCallExpression)
      {
        MethodCallExpression mex = (MethodCallExpression)expression;
        methodName = mex.Method.Name;
        if (methodName == "OfType" || methodName == "Cast")
        {
          Type listType = typeof(Enumerable);
          List<C> listFrom = _parent.ToList<C>();
          List<object> paramList = new List<object>();
          paramList.Add(listFrom);
          foreach (MethodInfo method in listType.GetMethods())
            if (method.Name == methodName)
            {
              Type[] genericArguments = { mex.Method.GetGenericArguments().First() };
              MethodInfo genericMethodInfo = method.MakeGenericMethod(genericArguments);
              System.Collections.IEnumerable thingWeGotBack = (System.Collections.IEnumerable)genericMethodInfo.Invoke(null, paramList.ToArray());
              return thingWeGotBack.AsQueryable();
            }
        }

        //JF start change
        //2/7/09 - only gets here if mex is a MethodCallExpresion - else it does what it was originally supposed to do.
        if (elementType == typeof(C) && mex.Method.Name == "Where" && _filter == null)
        {
          _filter = new LinqBindingList<C>(_parent, this, expression);
          _filter.BuildFilterIndex();
          return (IQueryable)_filter;
        }
        else if (elementType == typeof(C) && mex.Method.Name.StartsWith("OrderBy") && _filter == null)
        {
          //raw sort without previous where
          _filter = new LinqBindingList<C>(_parent, this, null);
          _filter.SortByExpression(expression);
          return (IQueryable)_filter;
        }
        else if (elementType == typeof(C) && mex.Method.Name.StartsWith("OrderBy"))
        {
          //sort of previous where
          _filter.SortByExpression(expression);
          return (IQueryable)_filter;
        }
        else if (elementType == typeof(C) && mex.Method.Name.StartsWith("ThenBy") && _filter == null)
        {
          //raw sort without previous where
          _filter = new LinqBindingList<C>(_parent, this, null);
          _filter.ThenByExpression(expression);
          return (IQueryable)_filter;
        }
        //JF end change
        else if (elementType == typeof(C) && mex.Method.Name.StartsWith("ThenBy"))
        {
          //sort of previous where
          _filter.ThenByExpression(expression);
          return (IQueryable)_filter;
        }
      }
      _filter = new LinqBindingList<C>(_parent, this, expression);
      return _filter;
    }


    public TResult Execute<TResult>(Expression expression)
    {
      return (TResult)this.Execute(expression);
    }

    public object Execute(Expression expression)
    {

      MethodCallExpression mex = (MethodCallExpression)expression;

      //convert the enumerated collection to a list
      List<C> listFrom;

      if (_filter != null)
        listFrom = _filter.ToList<C>();
      else
        listFrom = _parent.ToList<C>();
      //we are going to call the Enumerable equivalent so we can use it's provider rather than
      //  re-doing all that work on our own
      Type listType = typeof(Enumerable);
      MethodInfo[] listMethods = listType.GetMethods();
      List<object> paramList = new List<object>();

      //we are going to pass this a list of items derived from our filtered list, this will be the equivalent of someList.SomeEnumerableExtensionMethod(someparms);
      //  because its an extension method, however, technically, the first parameter is the thing we are calling with the extension method
      paramList.Add(listFrom);
      int i = 0;
      //put each argument from the actual method call passed to us into a param list we are going to send, via reflection, to Enumerable.XXX(YYY)
      foreach (object arg in mex.Arguments)
      {
        if (i > 0)
          if (arg is Expression)
            //expressions have to be compiled in order to work with the method call on straight Enumerable
            paramList.Add(Compile((Expression)arg));
          else
            paramList.Add(arg);
        i++;
      }
      //now, the happy task of manually finding the right method
      foreach (MethodInfo method in listMethods)
        //MethodsEquivalent goes through the nastiness of seeing whether a given MethodCAllExpression maps to a given MethodInfo.  
        if (MethodsEquivalent(mex, method))
        {
          //the Enumerable call is a generic method call, so deal with that
          var someGenericArgs = mex.Method.GetGenericArguments().Take(method.GetGenericArguments().Length);
          MethodInfo genericMethodInfo = method.MakeGenericMethod(someGenericArgs.ToArray());
          try
          {
            //pray.  If something is going to break, it will do so here
            return genericMethodInfo.Invoke(null, paramList.ToArray());
          }
          catch (TargetInvocationException tie)
          {
            throw tie.InnerException;
          }

        }
      return null;

    }

    #endregion
  }
}
