//-----------------------------------------------------------------------
// <copyright file="InjectCommandObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Command object whose factory Execute method uses [Inject]</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ObjectFactory
{
  [Server.ObjectFactory("Csla.Test.ObjectFactory.InjectCommandObjectFactory, Csla.Tests")]
  [Serializable]
  public class InjectCommandObject : CommandBase<InjectCommandObject>
  {
    public static readonly PropertyInfo<string> ValueProperty = RegisterProperty<InjectCommandObject, string>(p => p.Value);
    public string Value
    {
      get => ReadProperty(ValueProperty);
      set => LoadProperty(ValueProperty, value);
    }

    public static InjectCommandObject Execute(IDataPortal<InjectCommandObject> dataPortal)
    {
      var cmd = dataPortal.Create();
      return dataPortal.Execute(cmd);
    }
  }

  public class InjectCommandObjectFactory : Csla.Server.ObjectFactory
  {
    public InjectCommandObjectFactory(ApplicationContext applicationContext) : base(applicationContext)
    {
    }

    [RunLocal]
    public object Create()
    {
      return ApplicationContext.CreateInstanceDI<InjectCommandObject>();
    }

    public object Execute(InjectCommandObject command, [Inject] IFactoryTestService service)
    {
      command.Value = service.GetValue();
      return command;
    }
  }
}
