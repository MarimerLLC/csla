using System;
using Csla.Core;
using Csla.Core.FieldManager;

namespace Csla.Analyzers.Tests.Targets.ITypeSymbolExtensionsTests
{
  public class IsIPropertyInfoWhenSymbolDerivesFromIPropertyInfo
    : IPropertyInfo
  {
    public object DefaultValue
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string FriendlyName
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public int Index
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public string Name
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public RelationshipTypes RelationshipType
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public Type Type
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public IFieldData NewFieldData(string name)
    {
      throw new NotImplementedException();
    }
  }
}