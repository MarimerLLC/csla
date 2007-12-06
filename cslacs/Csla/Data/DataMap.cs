using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Csla.Data
{
  public class DataMap
  {
    #region MapElement

    internal class MemberMapping
    {
      private MemberInfo _from;
      public MemberInfo FromMember
      {
        get { return _from; }
      }

      private MemberInfo _to;
      public MemberInfo ToMember
      {
        get { return _to; }
      }

      public MemberMapping(MemberInfo fromMember, MemberInfo toMember)
      {
        _from = fromMember;
        _to = toMember;
      }
    }

    #endregion

    private Type _sourceType;
    private Type _targetType;
    private List<MemberMapping> _map = new List<MemberMapping>();
    private BindingFlags _fieldFlags = BindingFlags.Public | 
                                       BindingFlags.NonPublic | 
                                       BindingFlags.Instance;
    private BindingFlags _propertyFlags = BindingFlags.Public |
                                          BindingFlags.Instance |
                                          BindingFlags.FlattenHierarchy;

    public DataMap(Type sourceType, Type targetType)
    {
      _sourceType = sourceType;
      _targetType = targetType;
    }

    internal Type SourceType
    {
      get { return _sourceType; }
    }

    internal List<MemberMapping> GetMap()
    {
      return _map;
    }

    public void AddPropertyMapping(string sourceProperty, string targetProperty)
    {
      _map.Add(new MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetProperty(targetProperty, _propertyFlags)));
    }

    public void AddFieldMapping(string sourceField, string targetField)
    {
      _map.Add(new MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetField(targetField, _fieldFlags)));
    }

    public void AddFieldToPropertyMapping(string sourceField, string targetProperty)
    {
      _map.Add(new MemberMapping(_sourceType.GetField(sourceField, _fieldFlags), _targetType.GetProperty(targetProperty, _propertyFlags)));
    }

    public void AddPropertyToFieldMapping(string sourceProperty, string targetField)
    {
      _map.Add(new MemberMapping(_sourceType.GetProperty(sourceProperty, _propertyFlags), _targetType.GetField(targetField, _fieldFlags)));
    }
  }
}
