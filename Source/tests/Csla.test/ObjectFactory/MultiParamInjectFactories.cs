//-----------------------------------------------------------------------
// <copyright file="MultiParamInjectFactories.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory fixtures exercising multiple criteria parameters</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ObjectFactory
{
  /// <summary>
  /// Simple service used to verify dependency injection into
  /// object factory methods and constructors.
  /// </summary>
  public interface IFactoryTestService
  {
    string GetValue();
  }

  public class FactoryTestService : IFactoryTestService
  {
    public string GetValue() => "injected";
  }

  /// <summary>
  /// Factory whose create/fetch methods accept multiple criteria parameters.
  /// </summary>
  public class MultiParamRootFactory : Csla.Server.ObjectFactory
  {
    public MultiParamRootFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Create(string text, int number)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"Create {text} {number}";
      obj.MarkAsNew();
      return obj;
    }

    public object Fetch(string text, int number)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"Fetch {text} {number}";
      obj.MarkAsOld();
      return obj;
    }
  }

  /// <summary>
  /// Factory whose methods mix criteria parameters with [Inject] parameters.
  /// </summary>
  public class InjectRootFactory : Csla.Server.ObjectFactory
  {
    public InjectRootFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Fetch(string id, [Inject] IFactoryTestService service)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"{id}:{service.GetValue()}";
      obj.MarkAsOld();
      return obj;
    }

    public object Create(string text, int number, [Inject] IFactoryTestService service)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"{text} {number} {service.GetValue()}";
      obj.MarkAsNew();
      return obj;
    }
  }

  /// <summary>
  /// Factory that receives its dependency through constructor injection.
  /// </summary>
  public class CtorInjectRootFactory : Csla.Server.ObjectFactory
  {
    private readonly IFactoryTestService _service;

    public CtorInjectRootFactory(ApplicationContext applicationContext, IFactoryTestService service)
      : base(applicationContext)
    {
      _service = service;
    }

    public object Fetch()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = _service.GetValue();
      obj.MarkAsOld();
      return obj;
    }
  }

  /// <summary>
  /// Factory with parameterless, single-criteria and multi-criteria overloads
  /// of the same operation, to exercise overload disambiguation.
  /// </summary>
  public class OverloadRootFactory : Csla.Server.ObjectFactory
  {
    public OverloadRootFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public object Fetch()
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = "Fetch0";
      obj.MarkAsOld();
      return obj;
    }

    public object Fetch(string text)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"Fetch1 {text}";
      obj.MarkAsOld();
      return obj;
    }

    public object Fetch(string text, int number)
    {
      var obj = ApplicationContext.CreateInstanceDI<Root>();
      obj.Data = $"Fetch2 {text} {number}";
      obj.MarkAsOld();
      return obj;
    }
  }
}
