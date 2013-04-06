using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using Ex = System.Linq.Expressions.Expression;

namespace Csla.Linq
{
  internal class IndexSet<T> : IIndexSet<T>
  {
    private Dictionary<string, IIndex<T>> _internalIndexSet = new Dictionary<string, IIndex<T>>();
    //private Dictionary<string, IRangeTestableIndex<T>> _internalIndexSet = new Dictionary<string, IRangeTestableIndex<T>>();

    public IndexSet()
    {
      PropertyInfo[] allProps = typeof(T).GetProperties();
      foreach (PropertyInfo property in allProps)
      {
        object[] attributes = property.GetCustomAttributes(true);
        foreach (object attribute in attributes)
          if (attribute is IndexableAttribute)
          {
            // TODO: Evaluate this, wouldn't it be better to just compare the interface Type objects directly?
            var isComparable = property.PropertyType.FindInterfaces((Type t, object o) => t.ToString() == o.ToString(), "System.IComparable").Length > 0;

            if (isComparable)
              _internalIndexSet.Add(property.Name, new BalancedTreeIndex<T>(property.Name, attribute as IndexableAttribute));
            else
              _internalIndexSet.Add(property.Name, new Index<T>(property.Name, attribute as IndexableAttribute));
          }
      }
    }

    #region IIndexSet<T> Members

    void IIndexSet<T>.InsertItem(T item)
    {
      foreach (IIndex<T> index in _internalIndexSet.Values)
      {
        if (index.Loaded)
          index.Add(item);
      }
    }

    void IIndexSet<T>.InsertItem(T item, string property)
    {
      if (_internalIndexSet.ContainsKey(property))
        if (_internalIndexSet[property].Loaded)
          _internalIndexSet[property].Add(item);
    }

    void IIndexSet<T>.RemoveItem(T item)
    {
      foreach (IIndex<T> index in _internalIndexSet.Values)
        index.Remove(item);
    }

    void IIndexSet<T>.RemoveItem(T item, string property)
    {
      if (_internalIndexSet.ContainsKey(property))
        _internalIndexSet[property].Remove(item);
    }

    void IIndexSet<T>.ReIndexItem(T item, string property)
    {
      _internalIndexSet[property].Remove(item);
      _internalIndexSet[property].Add(item);
    }

    void IIndexSet<T>.ReIndexItem(T item)
    {
      foreach (IIndex<T> index in _internalIndexSet.Values)
      {
        index.Remove(item);
        index.Add(item);
      }
    }
    
    void IIndexSet<T>.ClearIndexes()
    {
      foreach (IIndex<T> index in _internalIndexSet.Values)
        index.Clear();
    }

    void IIndexSet<T>.ClearIndex(string property)
    {
      if (_internalIndexSet.ContainsKey(property))
          _internalIndexSet[property].Clear();
    }

    bool IIndexSet<T>.HasIndexFor(string property)
    {
      return _internalIndexSet.ContainsKey(property);
    }

    string IIndexSet<T>.HasIndexFor(Expression<Func<T, bool>> expr)
    {
      //if (expr.Body.NodeType == ExpressionType.Equal && expr.Body is BinaryExpression)
      if (expr.Body is BinaryExpression)
      {
          BinaryExpression binExp = (BinaryExpression)expr.Body;
          if (HasIndexablePropertyOnLeft(binExp.Left))
              return ((MemberExpression)binExp.Left).Member.Name;
          else
              return null;
      }
      else
          return null;
    }

    IIndex<T> IIndexSet<T>.this[string property]
    {
      get
      {
        return _internalIndexSet[property];
      }
    }

    private bool HasIndexablePropertyOnLeft(Expression leftSide)
    {
      if (leftSide.NodeType == ExpressionType.MemberAccess)
        return (this as IIndexSet<T>).HasIndexFor(((MemberExpression)leftSide).Member.Name);
      else
        return false;
    }

    private int? GetHashRight(Expression rightSide)
    {
      //rightside is where we get our hash...
      switch (rightSide.NodeType)
      {
        //shortcut constants, dont eval, will be faster
        case ExpressionType.Constant:
          ConstantExpression constExp = (ConstantExpression)rightSide;
          return (constExp.Value.GetHashCode());

        //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
        default:
          //Lambdas can be created from expressions... yay
          LambdaExpression evalRight = Ex.Lambda(rightSide, null);
          //Compile the expression, invoke it, and get the resulting hash
          return (evalRight.Compile().DynamicInvoke(null).GetHashCode());
      }
    }


    private object GetRightValue(Expression rightSide)
    {
      //rightside is where we get our hash...
      switch (rightSide.NodeType)
      {
        //shortcut constants, dont eval, will be faster
        case ExpressionType.Constant:
          ConstantExpression constExp = (ConstantExpression)rightSide;
          return (constExp.Value);

        //if not constant (which is provably terminal in a tree), convert back to Lambda and eval to get the hash.
        default:
          //Lambdas can be created from expressions... yay
          LambdaExpression evalRight = Ex.Lambda(rightSide, null);
          //Compile the expression, invoke it, and get the resulting hash
          return (evalRight.Compile().DynamicInvoke(null));
      }
    }
    IEnumerable<T> IIndexSet<T>.Search(Expression<Func<T, bool>> expr, string property)
    {
      if (expr.Body is BinaryExpression 
          && HasIndexablePropertyOnLeft( ((BinaryExpression)expr.Body).Left ))
      {
        Func<T, bool> exprCompiled = expr.Compile();
        BinaryExpression binExp = (BinaryExpression)expr.Body;
        object val = GetRightValue(binExp.Right);
        IRangeTestableIndex<T> rangedIndex;
        if (_internalIndexSet[property] is IRangeTestableIndex<T>)
        {
          rangedIndex = (IRangeTestableIndex<T>)_internalIndexSet[property];

          switch (binExp.NodeType)
          {
            case ExpressionType.Equal:

              foreach (T item in _internalIndexSet[property].WhereEqual(val, exprCompiled))
                yield return item;
              break;

            case ExpressionType.LessThan:


              foreach (T item in rangedIndex.WhereLessThan(val))
                yield return item;
              break;

            case ExpressionType.LessThanOrEqual:

              foreach (T item in rangedIndex.WhereLessThanOrEqualTo(val))
                yield return item;
              break;

            case ExpressionType.GreaterThan:

              foreach (T item in rangedIndex.WhereGreaterThan(val))
                yield return item;
              break;

            case ExpressionType.GreaterThanOrEqual:

              foreach (T item in rangedIndex.WhereGreaterThanOrEqualTo(val))
                yield return item;
              break;

            default:
              foreach (T item in rangedIndex.Where(expr.Compile()))
                yield return item;
              break;
          }
        }
        else
        {
          switch (binExp.NodeType)
          {
            case ExpressionType.Equal:

              int? rightHash = GetHashRight(binExp.Right);
              foreach (T item in _internalIndexSet[property].WhereEqual(val, exprCompiled))
                yield return item;
              break;

            default:
              foreach (T item in _internalIndexSet[property].Where(expr.Compile()))
                yield return item;
              break;
          }
        }
      }
      else
      {
        foreach(T item in _internalIndexSet[property].Where(expr.Compile()))
          yield return item;
      }
    }


    #endregion

    public void LoadIndex(string property)
    {
      _internalIndexSet[property].LoadComplete();
    }
  }
}
