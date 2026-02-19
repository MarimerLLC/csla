//-----------------------------------------------------------------------
// <copyright file="TestDocument.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test document object combining properties and child collection</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessDocumentBase
{
  [Serializable]
  public class TestDocument : BusinessDocumentBase<TestDocument, DocumentLineItem>
  {
    public static readonly PropertyInfo<string> DocumentNumberProperty = RegisterProperty<string>(nameof(DocumentNumber));
    public string DocumentNumber
    {
      get => GetProperty(DocumentNumberProperty);
      set => SetProperty(DocumentNumberProperty, value);
    }

    public static readonly PropertyInfo<DateTime> DocumentDateProperty = RegisterProperty<DateTime>(nameof(DocumentDate));
    public DateTime DocumentDate
    {
      get => GetProperty(DocumentDateProperty);
      set => SetProperty(DocumentDateProperty, value);
    }

    [NotUndoable]
    private string _notUndoableData = string.Empty;

    public string NotUndoableData
    {
      get => _notUndoableData;
      set => _notUndoableData = value;
    }

    public int DeletedCount => DeletedList.Count;

    public void MakeOld() => MarkOld();

    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void DataPortal_Fetch(int id, [Inject] IChildDataPortal<DocumentLineItem> childPortal)
    {
      using (LoadListMode)
      {
        DocumentNumber = "DOC-" + id;
        DocumentDate = DateTime.Today;
        // Simulate loading child items using FetchChild so they are marked old
        for (int i = 0; i < 3; i++)
        {
          var child = childPortal.FetchChild("Item " + i, (i + 1) * 10m);
          Add(child);
        }
      }
      MarkOld();
    }

    [Insert]
    private void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
      Child_Update();
    }

    [Update]
    private void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
      Child_Update();
    }

    [DeleteSelf]
    private void DataPortal_DeleteSelf()
    {
      Child_Update();
    }
  }
}
