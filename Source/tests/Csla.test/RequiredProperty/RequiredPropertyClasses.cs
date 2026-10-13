//-----------------------------------------------------------------------
// <copyright file="RequiredPropertyClasses.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business classes using RelationshipTypes.Required</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.RequiredProperty
{
  [Serializable]
  public class RequiredRoot : BusinessBase<RequiredRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.Required);
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<byte[]> DataProperty = RegisterProperty<byte[]>(nameof(Data), RelationshipTypes.Required);
    public byte[] Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    public static readonly PropertyInfo<string> NotesProperty = RegisterProperty<string>(nameof(Notes));
    public string Notes
    {
      get => GetProperty(NotesProperty);
      set => SetProperty(NotesProperty, value);
    }

    public static readonly PropertyInfo<RequiredChild> LazyChildProperty = RegisterProperty<RequiredChild>(nameof(LazyChild), RelationshipTypes.Required | RelationshipTypes.LazyLoad);
    public RequiredChild LazyChild => ReadProperty(LazyChildProperty);

    [Create]
    private void Create(bool setName)
    {
      LoadProperty(NameProperty, setName ? "name" : null);
      LoadProperty(DataProperty, new byte[] { 1 });
      LoadProperty(NotesProperty, null);
    }

    [Fetch]
    private void Fetch(bool setName, bool loadNullLazyChild)
    {
      LoadProperty(NameProperty, setName ? "name" : null);
      LoadProperty(DataProperty, new byte[] { 1 });
      LoadProperty(NotesProperty, null);
      if (loadNullLazyChild)
        LoadProperty(LazyChildProperty, null);
    }

    [Insert]
    private void Insert()
    { }

    [Update]
    private void Update()
    { }

    [Delete]
    private void Delete(bool criteria)
    { }

    [DeleteSelf]
    private void DeleteSelf()
    { }
  }

  [Serializable]
  public class RequiredPrivateFieldRoot : BusinessBase<RequiredPrivateFieldRoot>
  {
    public static readonly PropertyInfo<string> CodeProperty = RegisterProperty<string>(nameof(Code), RelationshipTypes.Required | RelationshipTypes.PrivateField);
    private string _code = CodeProperty.DefaultValue;
    public string Code
    {
      get => GetProperty(CodeProperty, _code);
      set => SetProperty(CodeProperty, ref _code, value);
    }

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id => GetProperty(IdProperty);

    [Create]
    private void Create(bool setCode)
    {
      LoadProperty(IdProperty, 1);
      _code = setCode ? "code" : null;
    }
  }

  [Serializable]
  public class RequiredChild : BusinessBase<RequiredChild>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.Required);
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<byte[]> DataProperty = RegisterProperty<byte[]>(nameof(Data), RelationshipTypes.Required);
    public byte[] Data
    {
      get => GetProperty(DataProperty);
      set => SetProperty(DataProperty, value);
    }

    [CreateChild]
    private void Create(bool setName)
    {
      LoadProperty(NameProperty, setName ? "name" : null);
      LoadProperty(DataProperty, new byte[] { 1 });
    }

    [FetchChild]
    private void Fetch(bool setName)
    {
      LoadProperty(NameProperty, setName ? "name" : null);
      LoadProperty(DataProperty, new byte[] { 1 });
    }

    [InsertChild]
    private void Insert()
    { }

    [UpdateChild]
    private void Update()
    { }

    [DeleteSelfChild]
    private void DeleteSelf()
    { }
  }

  [Serializable]
  public class RequiredReadOnly : ReadOnlyBase<RequiredReadOnly>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.Required);
    public string Name => GetProperty(NameProperty);

    [Fetch]
    private void Fetch(bool setName)
    {
      LoadProperty(NameProperty, setName ? "name" : null);
    }
  }

  [Serializable]
  public class RequiredCommand : CommandBase<RequiredCommand>
  {
    public static readonly PropertyInfo<bool> SetResultProperty = RegisterProperty<bool>(nameof(SetResult));
    public bool SetResult
    {
      get => ReadProperty(SetResultProperty);
      set => LoadProperty(SetResultProperty, value);
    }

    public static readonly PropertyInfo<string> ResultProperty = RegisterProperty<string>(nameof(Result), RelationshipTypes.Required);
    public string Result => ReadProperty(ResultProperty);

    [Create]
    private void Create(bool setResult)
    {
      SetResult = setResult;
    }

    [Execute]
    private void Execute()
    {
      LoadProperty(ResultProperty, SetResult ? "result" : null);
    }
  }

  [Serializable]
  public class RequiredUntouchedCommand : CommandBase<RequiredUntouchedCommand>
  {
    public static readonly PropertyInfo<byte[]> DataProperty = RegisterProperty<byte[]>(nameof(Data), RelationshipTypes.Required);
    public byte[] Data => ReadProperty(DataProperty);

    [Create]
    private void Create()
    { }

    [Execute]
    private void Execute()
    { }
  }

  [Serializable]
  [Csla.Server.ObjectFactory("Csla.Test.RequiredProperty.RequiredFactoryRootFactory, Csla.Tests")]
  public class RequiredFactoryRoot : BusinessBase<RequiredFactoryRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name), RelationshipTypes.Required);
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }
  }

  public class RequiredFactoryRootFactory : Csla.Server.ObjectFactory
  {
    public RequiredFactoryRootFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Fetch(bool setName)
    {
      var obj = ApplicationContext.CreateInstanceDI<RequiredFactoryRoot>();
      LoadProperty(obj, RequiredFactoryRoot.NameProperty, setName ? "name" : null);
      MarkOld(obj);
      return obj;
    }

    public object Update(RequiredFactoryRoot obj)
    {
      // simulate a factory whose update or delete leaves a required property null
      LoadProperty(obj, RequiredFactoryRoot.NameProperty, null);
      if (obj.IsDeleted)
        MarkNew(obj);
      else
        MarkOld(obj);
      return obj;
    }

    // the client looks this method up for a deleted object, but the
    // server-side data portal calls Update, which handles the delete
    public void Delete(RequiredFactoryRoot obj)
    { }
  }
}
