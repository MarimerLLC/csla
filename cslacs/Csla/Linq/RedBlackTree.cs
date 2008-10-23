using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using Csla.Properties;

namespace Csla.Linq
{

  public class RedBlackTree<P, T> : ICollection<T>, IBalancedSearch<T>
  {
    public class RedBlackTreeNode<P, T> : IEnumerable<T>
    {
      //P Usage
      protected Comparer<P> Comparer { get; set; }
      public delegate void RootResetHandler(RedBlackTreeNode<P, T> newRoot);
      public event RootResetHandler ResetRoot;
      
      public RedBlackTreeNode<P, T> Left { get; set; }
      public RedBlackTreeNode<P, T> Right { get; set; }
      public RedBlackTreeNode<P, T> Parent { get; set; }
      public RedBlackTree<P, T>.Color Color { get; set; }

      protected PropertyInfo _propInfo;

      public T Item 
      {
        get
        {
          if (_items.Count == 0)
            return default(T);
          else
            return _items[0];
        }
        protected set
        {
          if (value != null)
            _items.Add(value);
        }
      }

      protected List<T> _items = new List<T>();

      public RedBlackTreeNode(T item, string indexProp, RedBlackTreeNode<P, T> parent)
      {
        Comparer = System.Collections.Generic.Comparer<P>.Default;
        Left = null;
        Right = null;
        Parent = parent;
        Color = RedBlackTree<P, T>.Color.Black;
        Item = item;
        _propInfo = typeof(T).GetProperty(indexProp);
      }

      public RedBlackTreeNode<P, T> GrandParent
      {
        get
        {
          if (Parent != null && Parent.Parent != null)
            return Parent.Parent;
          else
            return null;
        }
      }

      public RedBlackTreeNode<P, T> Uncle
      {
        get
        {
          if (GrandParent != null)
          {
            if (ReferenceEquals(GrandParent.Left, Parent))
              return GrandParent.Right;
            else
              return GrandParent.Left;
          }
          return null;
        }
      }

      public RedBlackTreeNode<P, T> Sibling
      {
        get
        {
          if (Parent != null)
          {
            if (ReferenceEquals(Parent.Left, this))
              return Parent.Right;
            else
              return Parent.Left;
          }
          return null;
        }
      }

      private int DoCompare(T a, P b)
      {
        return Comparer.Compare((P)_propInfo.GetValue(a, null), b);
      }

      private int DoCompare(P a, T b)
      {
        return Comparer.Compare(a, (P)_propInfo.GetValue(b, null));
      }

      private int DoCompare(T a, T b)
      {
        return Comparer.Compare((P)_propInfo.GetValue(a, null), (P)_propInfo.GetValue(b, null));
      }

      private RedBlackTreeNode<P, T> FindNode(T item)
      {
        if (Item == null) return null;
        //P Usage
        var compResult = DoCompare(item,Item);
        if (compResult == 0) return this;
        else if (compResult > 0 && Right != null) return Right.FindNode(item);
        else if (compResult < 0 && Left != null) return Left.FindNode(item);
        else return null;
      }

      internal IEnumerable<T> ItemsAtNode()
      {
        foreach (T item in _items)
          yield return item;
      }

      internal IEnumerable<T> FindItemsLessThan(P prop, bool andEqualTo)
      {
        var compResult = DoCompare(prop, Item);
        if (compResult == 0)
        {
          if (andEqualTo) foreach(T item in ItemsAtNode()) yield return item;
          if (Left != null)
            foreach (T item in Left)
              yield return item;
        }
        else if (compResult < 0)
        {
          if (Left != null)
            foreach (T item in Left.FindItemsLessThan(prop, andEqualTo))
              yield return item;

        }
        else if (compResult > 0)
        {
          foreach (T item in ItemsAtNode()) yield return item;
          if (Left != null)
            foreach (T item in Left)
              yield return item;
          if (Right != null)
            foreach (T item in Right.FindItemsLessThan(prop, andEqualTo))
              yield return item;
        }
        else
          yield break;
      }

      internal IEnumerable<T> FindItemsGreaterThan(P prop, bool andEqualTo)
      {
        var compResult = DoCompare(prop, Item);
        if (compResult == 0)
        {
          if (andEqualTo) foreach(T item in ItemsAtNode()) yield return item;
          if (Right != null)
            foreach (T item in Right)
              yield return item;
        }
        else if (compResult < 0)
        {
          foreach (T item in ItemsAtNode()) yield return item;
          if (Right != null)
            foreach (T item in Right)
              yield return item;
          if (Left != null)
            foreach (T item in Left.FindItemsGreaterThan(prop, andEqualTo))
              yield return item;
        }
        else if (compResult > 0 && Right != null)
        {
          foreach (T item in Right.FindItemsGreaterThan(prop, andEqualTo))
            yield return item;
        }
        else
          yield break;
      }

      internal IEnumerable<T> FindItemsEqualTo(P prop)
      {
        var compResult = DoCompare(prop, Item);
        if (compResult == 0)
        {
          foreach (T item in ItemsAtNode()) yield return item;
        }
        else if (compResult < 0 && Left != null)
        {
          foreach (T item in Left.FindItemsEqualTo(prop)) yield return item;
        }
        else if (compResult > 0 && Right != null)
        {
          foreach (T item in Right.FindItemsEqualTo(prop)) yield return item;
        }
        else
          yield break;
      }

      private static void UnlinkFromParent(RedBlackTreeNode<P, T> node)
      {
        if (IsLeftOfParent(node))
          node.Parent.Left = null;
        else if (IsRightOfParent(node))
          node.Parent.Right = null;
        node.Parent = null;
      }

      private static bool IsLeftOfParent(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent != null)
          return ReferenceEquals(node.Parent.Left, node);
        else
          return false;
      }

      private static bool IsRightOfParent(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent != null)
          return ReferenceEquals(node.Parent.Right, node);
        else
          return false;
      }

      public int RemoveItemFromNode(T item)
      {
        _items.Remove(item);
        return _items.Count;
      }

      public static RedBlackTreeNode<P, T> ChildNodeThatIsNotNull(RedBlackTreeNode<P, T> node)
      {
        if (node.Left == null && node.Right != null)
          return node.Right;
        else if (node.Left != null && node.Right == null)
          return node.Left;
        else
          return null;
      }

      public static RedBlackTreeNode<P, T> MinimumNodeInRightSubtree(RedBlackTreeNode<P, T> node)
      {
        var nodeVisitor = node.Right;
        while (nodeVisitor.Left != null)
          nodeVisitor = nodeVisitor.Left;
        return nodeVisitor;
      }

      public bool Remove(T item)
      {
        //find the node where the item lives
        var foundNode = FindNode(item);
        //if there is no node where the item lives, the item does not exist here
        if (foundNode == null) return false;
        //try to remove the item from the node, and get back the # of items remaining after that
        //  note - we do not have to screw with nodes until there are zero items remaining
        var remainingItems = foundNode.RemoveItemFromNode(item);
        if (remainingItems == 0) // remember, you can have multiple items matching the criteria
        {
          
          if (foundNode.Left == null && foundNode.Right == null) //both are null
          {
            System.Diagnostics.Debug.WriteLine("Leaf case");
            //not a root removal, just unlink
            if (foundNode.Parent != null)
              UnlinkFromParent(foundNode);
            else
              //removal of last item, reset the root to null
              ResetRoot(null);
          }
          else if (foundNode.Left == null || foundNode.Right == null) //one is null
          //this is the "delete with one child" case
          {
            System.Diagnostics.Debug.WriteLine("One child case");
            var newChildOfParent = ChildNodeThatIsNotNull(foundNode);
            if (foundNode.Parent == null) //we are a root
            {
              //the new root should be the potential orphan
              //the orphan is now the new root, since there was nothing on the other side
              //  this is completley ok
              ResetRoot(newChildOfParent);
            }
            else
            {
              //move the remaining child up to where we used to be
              if (IsLeftOfParent(foundNode)) foundNode.Parent.Left = newChildOfParent;
              else if (IsRightOfParent(foundNode)) foundNode.Parent.Right = newChildOfParent;
              newChildOfParent.Parent = foundNode.Parent;
              foundNode.Parent = null;
            }
            if (foundNode.Color == RedBlackTree<P, T>.Color.Black
              && newChildOfParent.Color == RedBlackTree<P, T>.Color.Red)
              newChildOfParent.Color = RedBlackTree<P, T>.Color.Black;
            else
              RedBlackTreeNode<P, T>.RebalanceDeleteStep1(newChildOfParent);
          }
          else
          {
            System.Diagnostics.Debug.WriteLine("two child case");
            //in this case, we do not care about roots, since by definition it isn't
            //a last removal, and we are not dealing with parent reassignment for the node we found
            // but rather, a leaf node in the tree
            var newSuccessor = MinimumNodeInRightSubtree(foundNode);
            //the found node is going to get the new successors items
            foundNode._items.Clear();
            foundNode._items.AddRange(newSuccessor._items);
            //get a ref to the parent of the successor
            var parentOfSuccessor = newSuccessor.Parent;
            //unlink the successor from its parent
            UnlinkFromParent(newSuccessor);
            //get the right, if needed, and assign it to the old left (lowest item will always be on left
            // of its parent)
            if (newSuccessor.Right != null)
            {
              System.Diagnostics.Debug.WriteLine("two child case:with dangling rightside");
              //the new parents left has to be the right of the node we are moving out
              if (parentOfSuccessor.Left == null && parentOfSuccessor.Right != null)
              {
                System.Diagnostics.Debug.WriteLine("two child case:with dangling rightside:reconnect on the left");
                parentOfSuccessor.Left = newSuccessor.Right;
              }
              else if (parentOfSuccessor.Left != null && parentOfSuccessor.Right == null)
              {
                System.Diagnostics.Debug.WriteLine("two child case:with dangling rightside:reconnect on the right");
                parentOfSuccessor.Right = newSuccessor.Right;
              }
              else
              {
                if (parentOfSuccessor.ShouldGoLeftOf(newSuccessor))
                  parentOfSuccessor.Left = newSuccessor.Right;
                else
                  parentOfSuccessor.Right = newSuccessor.Right;
              }
              newSuccessor.Right.Parent = parentOfSuccessor;
            }
          }
        }

        return true;
      }

      private static void RebalanceDeleteStep1(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent != null)
          RebalanceDeleteStep2(node);
      }

      private static void RebalanceDeleteStep2(RedBlackTreeNode<P, T> node)
      {
        if (node.Sibling != null && node.Sibling.Color == RedBlackTree<P, T>.Color.Red)
        {
          node.Parent.Color = RedBlackTree<P, T>.Color.Red;
          node.Sibling.Color = RedBlackTree<P, T>.Color.Black;
          if (IsLeftOfParent(node))
            RotateLeft(node.Parent);
          else
            RotateRight(node.Parent);
        }
        RebalanceDeleteStep3(node);
      }

      private static bool IsNodeBlack(RedBlackTreeNode<P, T> node)
      {
        if (node == null) return true;
        else return node.Color == RedBlackTree<P, T>.Color.Black;
      }
      
      private static void RebalanceDeleteStep3(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent == null || node.Sibling == null)
        {
          int i = 1;
        }
        if ((IsNodeBlack(node.Parent)) &&
            (IsNodeBlack(node.Sibling)) &&
          // a null node is black, so this passes if the siblings left child is null
            (node.Sibling == null || IsNodeBlack(node.Sibling.Left)) &&
          // same as prev condition, but for right side
            (node.Sibling == null || IsNodeBlack(node.Sibling.Right))
           )
        {
          if (node.Sibling != null) node.Sibling.Color = RedBlackTree<P, T>.Color.Red;
          if (node.Parent != null)
            RebalanceDeleteStep1(node.Parent);
        }
        else
          RebalanceDeleteStep4(node);

      }

      private static void RebalanceDeleteStep4(RedBlackTreeNode<P, T> node)
      {
        if ((node.Parent != null && node.Parent.Color == RedBlackTree<P, T>.Color.Red) &&
             (IsNodeBlack(node.Sibling)) &&
             (node.Sibling == null || IsNodeBlack(node.Sibling.Left)) &&
             (node.Sibling == null || IsNodeBlack(node.Sibling.Right))
          )
        {
          if (node.Sibling != null) node.Sibling.Color = RedBlackTree<P, T>.Color.Red;
          node.Parent.Color = RedBlackTree<P, T>.Color.Black;
        }
        else
          RebalanceDeleteStep5(node);

      }

      private static void RebalanceDeleteStep5(RedBlackTreeNode<P, T> node)
      {
        if (
          IsLeftOfParent(node) &&
          IsNodeBlack(node.Sibling) &&
          (node.Sibling != null && (!IsNodeBlack(node.Sibling.Left))) &&
          (node.Sibling != null && (IsNodeBlack(node.Sibling.Right)))
          )
        {
          if (node.Sibling != null) node.Sibling.Color = RedBlackTree<P, T>.Color.Red;
          if (node.Sibling != null && node.Sibling.Left != null) node.Sibling.Left.Color = RedBlackTree<P, T>.Color.Black;
          RotateRight(node.Sibling);
        }
        else if (
          IsRightOfParent(node) &&
          IsNodeBlack(node.Sibling) &&
          (node.Sibling != null && (!IsNodeBlack(node.Sibling.Right))) &&
          (node.Sibling != null && (IsNodeBlack(node.Sibling.Left)))
          )
        {
          if (node.Sibling != null) node.Sibling.Color = RedBlackTree<P, T>.Color.Red;
          if (node.Sibling != null && node.Sibling.Right != null) node.Sibling.Right.Color = RedBlackTree<P, T>.Color.Black;
          RotateLeft(node.Sibling);
        }
        RebalanceDeleteStep6(node);
      }

      private static void RebalanceDeleteStep6(RedBlackTreeNode<P, T> node)
      {
        if (node.Sibling != null) node.Sibling.Color = node.Parent.Color;
        node.Parent.Color = RedBlackTree<P, T>.Color.Black;
        if (IsLeftOfParent(node))
        {
          if (node.Sibling != null && node.Sibling.Right != null)
            node.Sibling.Right.Color = RedBlackTree<P, T>.Color.Black;
          RotateLeft(node.Parent);
        }
        else
        {
          if (node.Sibling != null && node.Sibling.Left != null)
            node.Sibling.Left.Color = RedBlackTree<P, T>.Color.Black;
          RotateRight(node.Parent);
        }
      }

      private bool ShouldGoLeftOf(RedBlackTreeNode<P, T> node)
      {
        if (Item != null && node.Item != null)
        {
          //P usage
          var compResult = Comparer.Compare((P)_propInfo.GetValue(node.Item, null), (P)_propInfo.GetValue(Item, null));
          if (compResult < 0)
            return true;
        }
        return false;
      }
      
      public void Insert(T newItem, int depth)
      {
        var frameCount = depth;

        if (Item == null)
        {
          Item = newItem;
          Rebalance();
          return;
        }


        if (depth > 100)
        {
          var i = 1;
          return;
        }

        //P Usage
        var compResult = DoCompare(newItem, Item);
        if (frameCount > 20)
        {
          var xx = 1;
          //  System.Diagnostics.Trace.WriteLine("Item = " + (P)_propInfo.GetValue(Item, null));
        //  System.Diagnostics.Trace.WriteLine("newItem = " + (P)_propInfo.GetValue(newItem, null));
        //  System.Diagnostics.Trace.WriteLine("parentItem = " + (P)_propInfo.GetValue(Parent.Item, null));
        //  System.Diagnostics.Trace.WriteLine("compResult = " + compResult);
        }
        if (compResult < 0)
        {


            if (Left != null && Left.Item != null)
            {
              //System.Diagnostics.Trace.WriteLineIf(frameCount > 100, "Left not null case");
              Left.Insert(newItem, depth + 1);
            }
            else
            {
              //System.Diagnostics.Trace.WriteLineIf(frameCount > 100, "Left null case");
              Left = new RedBlackTreeNode<P, T>(newItem, _propInfo.Name, this);
              Left.Color = RedBlackTree<P, T>.Color.Red;
              Left.Rebalance();
            }
        }
        else if (compResult == 0)
          Item = newItem;
        else
        {
          if (Right != null && Right.Item != null)
          {
            //System.Diagnostics.Trace.WriteLineIf(frameCount > 100, "Right not null case");
            Right.Insert(newItem, depth + 1);
          }
          else
          {
            //System.Diagnostics.Trace.WriteLineIf(frameCount > 100, "Right null case");
            Right = new RedBlackTreeNode<P, T>(newItem, _propInfo.Name, this);
            Right.Color = RedBlackTree<P, T>.Color.Red;
            Right.Rebalance();
          }
        }
      }

      internal void Rebalance()
      {
        //ACE: To try balancing, uncomment this line
        //RebalanceStep1(this);
      }

      private static void RebalanceStep1(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent == null)
          node.Color = RedBlackTree<P, T>.Color.Black;
        else
          RebalanceStep2(node);
      }

      private static void RebalanceStep2(RedBlackTreeNode<P, T> node)
      {
        if (node.Parent.Color == RedBlackTree<P, T>.Color.Black)
          return;
        else
          RebalanceStep3(node);
      }

      private static void RebalanceStep3(RedBlackTreeNode<P, T> node)
      {
        if (node.Uncle != null && node.Uncle.Color == RedBlackTree<P, T>.Color.Red)
        {
          node.Parent.Color = RedBlackTree<P, T>.Color.Black;
          node.Uncle.Color = RedBlackTree<P, T>.Color.Black;
          node.GrandParent.Color = RedBlackTree<P, T>.Color.Red;
          RebalanceStep1(node.GrandParent);
        }
        else
          RebalanceStep4(node);
      }

      private static void RebalanceStep4(RedBlackTreeNode<P, T> node)
      {
        if (
            ReferenceEquals(node, node.Parent.Right) && 
            ReferenceEquals(node.Parent, node.GrandParent.Left)
           )
        {
          RotateLeft(node.Parent);
          node = node.Left;
        }
        else if (
          ReferenceEquals(node, node.Parent.Left) &&
          ReferenceEquals(node.Parent, node.GrandParent.Right)
          )
        {
          RotateRight(node.Parent);
          node = node.Right;
        }
        RebalanceStep5(node);
      }

      private static void RebalanceStep5(RedBlackTreeNode<P, T> node)
      {
        node.Parent.Color = RedBlackTree<P, T>.Color.Black;
        node.GrandParent.Color = RedBlackTree<P, T>.Color.Red;
        if (ReferenceEquals(node, node.Parent.Left) &&
            ReferenceEquals(node.Parent, node.GrandParent.Left))
          RotateRight(node.GrandParent);
        else
          RotateLeft(node.GrandParent);
      }

      private static void RotateRight(RedBlackTreeNode<P, T> node)
      {
        var pivot = node.Left;
        var root = node;
        if (pivot == null) return;
        //this moves the node to the right of the pivot node to
        // the left node of the root
        if (pivot.Right != null)
        {
          pivot.Right.Parent = root;
        }
        root.Left = pivot.Right;
        
        //this makes it such that the parent of the root points to the pivot,
        // not the root
        if (root.Parent != null)
        {
          var rootParent = root.Parent;
          if (ReferenceEquals(rootParent.Left, root)) //we are the left of the root
            rootParent.Left = pivot; //make the left part point to pivot, which now takes the root role
          else
            rootParent.Right = pivot; //make the right part point to pivot, which now takes the root role
          pivot.Parent = rootParent;
        }
        else
        {
          if (root.ResetRoot != null)
          {
            root.ResetRoot(pivot);
          }
        }
        root.Parent = pivot; //the root should now point up to its new parent
        pivot.Right = root;
      }

      private static void RotateLeft(RedBlackTreeNode<P, T> node)
      {
        var pivot = node.Right;
        if (pivot == null) return;
        var root = node;
        if (pivot.Left != null)
          pivot.Left.Parent = root;
        root.Right = pivot.Left;
        if (root.Parent != null)
        {
          var rootParent = root.Parent;
          if (ReferenceEquals(rootParent.Right, root)) //we are the left of the root
            rootParent.Right = pivot; //make the left part point to pivot, which now takes the root role
          else
            rootParent.Left = pivot; //make the right part point to pivot, which now takes the root role
          pivot.Parent = rootParent;
        }
        else
        {
          if (root.ResetRoot != null)
          {
            root.ResetRoot(pivot);
          }
        }
        root.Parent = pivot;
        pivot.Left = root;
      }

      #region IEnumerable<T> Members

      public IEnumerator<T> GetEnumerator()
      {
        if (Left != null)
          foreach (T item in Left)
            yield return item;
        if (Right != null)
          foreach (T item in Right)
            yield return item;
        if (_items != null)
          foreach (T item in _items)
            yield return item;
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new NotImplementedException();
      }

      #endregion

      public override string ToString()
      {
        var myString = "";
        if (Item != null) myString += "Color:" + Color.ToString() + " Item:" + Item.ToString() + "\n";
        if (Left != null) myString += "Left: " + Left.ToString();
        if (Right != null) myString += "Right: " + Right.ToString();
        return myString;
      }
    }

    private int _countCache = 0;
    private string _propertyName;
    
    public enum Color{ Red, Black }

    public RedBlackTreeNode<P, T> Root { get; set; }

    public RedBlackTree(string indexProp)
    {
      _propertyName = indexProp;
      Root = new RedBlackTree<P, T>.RedBlackTreeNode<P, T>(default(T), indexProp, null);
      Root.ResetRoot += rootReset;
    }

    private void rootReset(RedBlackTreeNode<P, T> newRoot )
    {
      if (newRoot == null)
        Root = new RedBlackTree<P, T>.RedBlackTreeNode<P, T>(default(T), _propertyName, null);
      else
      {
        newRoot.ResetRoot += rootReset;
        Root.ResetRoot -= rootReset;
        Root = newRoot;
        Root.Parent = null;
      }
    }
    
    #region ICollection<T> Members

    public void Add(T item)
    {
      Root.Insert(item, 1);
      _countCache++;
    }

    public void Clear()
    {
      Root = new RedBlackTree<P, T>.RedBlackTreeNode<P, T>(default(T), _propertyName, null);
    }

    public bool Contains(T item)
    {
      throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      if (object.ReferenceEquals(array, null))
      {
        throw new ArgumentNullException(
            Resources.NullArrayReference,
            "array"
            );
      }

      if (arrayIndex < 0)
      {
        throw new ArgumentOutOfRangeException(
            Resources.IndexIsOutOfRange,
            "index"
            );
      }

      if (array.Rank > 1)
      {
        throw new ArgumentException(
            Resources.ArrayIsMultiDimensional,
            "array"
            );
      }

      foreach (object o in this)
      {
        array.SetValue(o, arrayIndex);
        arrayIndex++;
      }
    }

    public int Count
    {
      get { 
        //todo - change this to cache after we know this actually works
        return Root.ToArray().Count(); 
      }
    }

    public bool IsReadOnly
    {
      get { throw new NotImplementedException(); }
    }

    public bool Remove(T item)
    {
      //we have to say whether we actually removed or not
      var removed = Root.Remove(item);
      if (removed)
        _countCache--;
      return removed;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      foreach (T item in Root)
        yield return item;
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

    public override string ToString()
    {
      return Root.ToString();
    }

    #region IBalancedSearch<T> Members

    public IEnumerable<T> ItemsLessThan(object pivot)
    {
      var typedPivot = (P) pivot;
      return Root.FindItemsLessThan(typedPivot, false);
    }

    public IEnumerable<T> ItemsGreaterThan(object pivot)
    {
      var typedPivot = (P)pivot;
      return Root.FindItemsGreaterThan(typedPivot, false);
    }

    public IEnumerable<T> ItemsLessThanOrEqualTo(object pivot)
    {
      var typedPivot = (P)pivot;
      return Root.FindItemsLessThan(typedPivot, true);
    }

    public IEnumerable<T> ItemsGreaterThanOrEqualTo(object pivot)
    {
      var typedPivot = (P)pivot;
      return Root.FindItemsGreaterThan(typedPivot, true);
    }

    public IEnumerable<T> ItemsEqualTo(object pivot)
    {
      System.Diagnostics.Trace.WriteLine((P)pivot);
      var typedPivot = (P)pivot;
      return Root.FindItemsEqualTo(typedPivot);
    }

    #endregion


  }

}
