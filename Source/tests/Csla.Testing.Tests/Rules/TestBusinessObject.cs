//-----------------------------------------------------------------------
// <copyright file="TestBusinessObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Sample business object used by the rule tester tests</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Testing.Tests.Rules
{
  /// <summary>
  /// Sample business object with managed properties, used as the target for
  /// rules under test.
  /// </summary>
  [Serializable]
  public class TestBusinessObject : BusinessBase<TestBusinessObject>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<int> AgeProperty = RegisterProperty<int>(nameof(Age));
    public int Age
    {
      get => GetProperty(AgeProperty);
      set => SetProperty(AgeProperty, value);
    }

    public static readonly PropertyInfo<string> DisplayNameProperty = RegisterProperty<string>(nameof(DisplayName));
    public string DisplayName
    {
      get => GetProperty(DisplayNameProperty);
      set => SetProperty(DisplayNameProperty, value);
    }

    /// <summary>
    /// A lazy loaded property. Reading it through the rules engine must be
    /// skipped when no field data exists.
    /// </summary>
    public static readonly PropertyInfo<string> LazyProperty =
      RegisterProperty<string>(nameof(Lazy), "Lazy", null, RelationshipTypes.LazyLoad);
    public string Lazy
    {
      get => LazyGetProperty(LazyProperty, () => "generated");
      set => SetProperty(LazyProperty, value);
    }

    /// <summary>
    /// Loads a value into the lazy property, so a test can exercise both the
    /// "no field data" and "field data present" paths.
    /// </summary>
    public void LoadLazyValue(string value) => LoadProperty(LazyProperty, value);

    /// <summary>
    /// Creates an instance with values loaded directly into the managed fields.
    /// </summary>
    public static TestBusinessObject Create(ApplicationContext applicationContext, string name, int age)
    {
      var obj = applicationContext.CreateInstanceDI<TestBusinessObject>();
      obj.LoadProperty(NameProperty, name);
      obj.LoadProperty(AgeProperty, age);
      return obj;
    }
  }
}
