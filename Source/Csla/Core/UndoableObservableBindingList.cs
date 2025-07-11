//-----------------------------------------------------------------------
// <copyright file="UndoableObservableBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://www.lhotka.net/cslanet/
// </copyright>
// <summary>Represents a binding list that supports undo functionality for its items.</summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using Csla.Properties;
using Csla.Reflection;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Represents a binding list that supports undo functionality for its items.
  /// </summary>
  /// <typeparam name="T">
  /// The type of elements in the list. Must implement <see cref="IUndoableObject"/>.
  /// </typeparam>
  [Serializable]
  public class UndoableObservableBindingList<T> : MobileObservableCollection<T>,
    IUndoableObject, IUseApplicationContext
  {
    // keep a stack of object state values.
    [NotUndoable]
    private readonly Stack<byte[]> _stateStack = new();
    [NotUndoable]
    private bool _bindingEdit;
    [NotUndoable]
    private ApplicationContext _applicationContext = default!;

    /// <inheritdoc />
    ApplicationContext IUseApplicationContext.ApplicationContext { get => ApplicationContext; set => ApplicationContext = value; }

    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    protected ApplicationContext ApplicationContext 
    { 
      get => _applicationContext;
      set
      {
        _applicationContext = value ?? throw new ArgumentNullException(nameof(ApplicationContext));
        OnApplicationContextSet();
      }
    }

    /// <summary>
    /// Method invoked after ApplicationContext
    /// is available.
    /// </summary>
    protected virtual void OnApplicationContextSet()
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public UndoableObservableBindingList()
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether n-level undo
    /// was invoked through IEditableObject. FOR INTERNAL
    /// CSLA .NET USE ONLY!
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool BindingEdit
    {
      get => _bindingEdit;
      set => _bindingEdit = value;
    }

    /// <inheritdoc />
    int IUndoableObject.EditLevel => EditLevel;

    /// <summary>
    /// Returns the current edit level of the object.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected int EditLevel => _stateStack.Count;

    /// <inheritdoc />
    void IUndoableObject.CopyState(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        CopyState(parentEditLevel);
    }

    /// <inheritdoc />
    void IUndoableObject.UndoChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        UndoChanges(parentEditLevel);
    }

    /// <inheritdoc />
    void IUndoableObject.AcceptChanges(int parentEditLevel, bool parentBindingEdit)
    {
      if (!parentBindingEdit)
        AcceptChanges(parentEditLevel);
    }

    /// <summary>
    /// This method is invoked before the CopyState
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CopyingState()
    {
    }

    /// <summary>
    /// This method is invoked after the CopyState
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void CopyStateComplete()
    {
    }

    /// <summary>
    /// Copies the state of the object and places the copy
    /// onto the state stack.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void CopyState(int parentEditLevel)
    {
      CopyingState();

      Type? currentType = GetType();
      var state = new MobileDictionary<string, object?>();

      if (EditLevel + 1 > parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "CopyState"), GetType().Name, null, EditLevel, parentEditLevel - 1);

      // Store items in the list
      var itemList = new List<T>(this);
      state.Add("Items", itemList);

      // Cascade the call to all child objects
      foreach (T item in this)
      {
        if (item is IUndoableObject undoableItem)
          undoableItem.CopyState(EditLevel + 1, BindingEdit);
      }

      // Serialize the state and stack it
      using (MemoryStream buffer = new MemoryStream())
      {
        var formatter = _applicationContext.GetRequiredService<ISerializationFormatter>();
        formatter.Serialize(buffer, state);
        _stateStack.Push(buffer.ToArray());
      }
      CopyStateComplete();
    }

    /// <summary>
    /// This method is invoked before the UndoChanges
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void UndoingChanges()
    {
    }

    /// <summary>
    /// This method is invoked after the UndoChanges
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void UndoChangesComplete()
    {
    }

    /// <summary>
    /// Restores the object's state to the most recently
    /// copied values from the state stack.
    /// </summary>
    /// <remarks>
    /// Restores the state of the object to its
    /// previous value by taking the data out of
    /// the stack and restoring it into the fields
    /// of the object.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void UndoChanges(int parentEditLevel)
    {
      UndoingChanges();

      // If we are a child object we might be asked to
      // undo below the level of stacked states,
      // so just do nothing in that case
      if (EditLevel > 0)
      {
        if (EditLevel - 1 != parentEditLevel)
          throw new UndoException(string.Format(Resources.EditLevelMismatchException, "UndoChanges"), GetType().Name, null, EditLevel, parentEditLevel + 1);

        MobileDictionary<string, object?> state;
        using (MemoryStream buffer = new MemoryStream(_stateStack.Pop()))
        {
          buffer.Position = 0;
          var formatter = _applicationContext.GetRequiredService<ISerializationFormatter>();
          state = (MobileDictionary<string, object?>)(formatter.Deserialize(buffer) ?? throw new InvalidOperationException());
        }

        // Restore items in the list
        if (state.Contains("Items"))
        {
          var itemList = state["Items"] as List<T>;
          using (var listMode = this.LoadListMode)
          {
            this.Clear();
            if (itemList != null)
            {
              foreach (T item in itemList)
              {
                this.Add(item);
              }
            }
          }
        }

        // Cascade the call to all child objects
        foreach (T item in this)
        {
          if (item is IUndoableObject undoableItem)
            undoableItem.UndoChanges(EditLevel, BindingEdit);
        }
      }
      UndoChangesComplete();
    }

    /// <summary>
    /// This method is invoked before the AcceptChanges
    /// operation begins.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void AcceptingChanges()
    {
    }

    /// <summary>
    /// This method is invoked after the AcceptChanges
    /// operation is complete.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void AcceptChangesComplete()
    {
    }

    /// <summary>
    /// Accepts any changes made to the object since the last
    /// state copy was made.
    /// </summary>
    /// <remarks>
    /// The most recent state copy is removed from the state
    /// stack and discarded, thus committing any changes made
    /// to the object's state.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal void AcceptChanges(int parentEditLevel)
    {
      AcceptingChanges();

      if (EditLevel - 1 != parentEditLevel)
        throw new UndoException(string.Format(Resources.EditLevelMismatchException, "AcceptChanges"), GetType().Name, null, EditLevel, parentEditLevel + 1);

      if (EditLevel > 0)
      {
        _stateStack.Pop();

        // Cascade the call to all child objects
        foreach (T item in this)
        {
          if (item is IUndoableObject undoableItem)
            undoableItem.AcceptChanges(EditLevel, BindingEdit);
        }
      }
      AcceptChangesComplete();
    }

    /// <summary>
    /// Reset the edit level of all child objects.
    /// </summary>
    internal static void ResetChildEditLevel(IUndoableObject child, int parentEditLevel, bool bindingEdit)
    {
      int targetLevel = parentEditLevel;
      if (bindingEdit && targetLevel > 0 && child is not FieldManager.FieldDataManager)
        targetLevel--;
      // if item's edit level is too high,
      // reduce it to match list
      while (child.EditLevel > targetLevel)
        child.AcceptChanges(targetLevel, false);
      // if item's edit level is too low,
      // increase it to match list
      while (child.EditLevel < targetLevel)
        child.CopyState(targetLevel, false);
    }

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      if (mode != StateMode.Undo)
      {
        info.AddValue("_bindingEdit", _bindingEdit);
        if (_stateStack.Count > 0)
        {
          var stackArray = _stateStack.ToArray();
          info.AddValue("_stateStack", stackArray);
        }
      }
      base.OnGetState(info, mode);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialization stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      if (mode != StateMode.Undo)
      {
        _bindingEdit = info.GetValue<bool>("_bindingEdit");
        if (info.Values.ContainsKey("_stateStack"))
        {
          var stackArray = (IEnumerable<byte[]>)info.GetRequiredValue<byte[][]>("_stateStack");
          _stateStack.Clear();
          foreach (var item in stackArray.Reverse())
          {
            _stateStack.Push(item);
          }
        }
      }
      base.OnSetState(info, mode);
    }
  }
}