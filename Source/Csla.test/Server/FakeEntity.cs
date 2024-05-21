using Csla.Core;
using Csla.Serialization.Mobile;

namespace Csla.Server.Tests
{
  public class FakeEntity : IEditableBusinessObject, IUndoableObject, ISavable, IMobileObject, IBusinessObject
  {
    public FakeEntity()
    {
      BusyChanged += FakeEntity_BusyChanged;
      UnhandledAsyncException += FakeEntity_UnhandledAsyncException;
      Saved += FakeEntity_Saved;
    }

    public event BusyChangedEventHandler BusyChanged;
    public event EventHandler<Core.ErrorEventArgs> UnhandledAsyncException;
    public event EventHandler<SavedEventArgs> Saved;

    private void FakeEntity_Saved(object sender, SavedEventArgs e)
    {
    }

    private void FakeEntity_UnhandledAsyncException(object sender, Core.ErrorEventArgs e)
    {
    }

    private void FakeEntity_BusyChanged(object sender, BusyChangedEventArgs e)
    {
    }
    public int EditLevelAdded { get; set; }

    public int Identity { get; set; }

    public int EditLevel { get; set; }

    public bool IsValid { get; set; }

    public bool IsSelfValid { get; set; }

    public bool IsDirty { get; set; }

    public bool IsSelfDirty { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsNew { get; set; }

    public bool IsSavable { get; set; }

    public bool IsChild { get; set; }

    public bool IsBusy { get; set; }

    public bool IsSelfBusy { get; set; }

    public void AcceptChanges(int parentEditLevel, bool parentBindingEdit) { }
    public void ApplyEdit() { }
    public void BeginEdit() { }
    public void CancelEdit() { }
    public void CopyState(int parentEditLevel, bool parentBindingEdit) { }
    public void Delete() { }
    public void DeleteChild() { }
    public void GetChildren(SerializationInfo info, MobileFormatter formatter) { }
    public void GetState(SerializationInfo info) { }
    public object Save() { return string.Empty; }
    public object Save(bool forceUpdate) { return string.Empty; }
    public Task SaveAndMergeAsync() { return Task.CompletedTask; }
    public Task SaveAndMergeAsync(bool forceUpdate) { return Task.CompletedTask; }
    public Task<object> SaveAsync() { return Task.FromResult(new object()); }
    public Task<object> SaveAsync(bool forceUpdate) { return Task.FromResult(new object()); }
    public void SaveComplete(object newObject) { }
    public void SetChildren(SerializationInfo info, MobileFormatter formatter) { }
    public void SetParent(IParent parent) { }
    public void SetState(SerializationInfo info) { }
    public void UndoChanges(int parentEditLevel, bool parentBindingEdit) { }
  }

}
