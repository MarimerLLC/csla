using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace cslalighttest.Engine
{
  public class TypeTester
  {
    private Type _type;
    public string Name { get { return _type.Name; } }

    private ObservableCollection<MethodTester> _methods = new ObservableCollection<MethodTester>();
    public ObservableCollection<MethodTester> Methods
    {
      get { return _methods; }
    }

    public TypeTester(Type type)
    {
      if(!type.IsDefined(typeof(TestClassAttribute), true))
        throw new ArgumentException("Type must have a TestClass attribute");
      if (!type.IsPublic)
        throw new ArgumentException("Type must be public");

      _type = type;

      foreach (MethodInfo method in _type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
      {
        if (method.IsPublic && method.IsDefined(typeof(TestMethodAttribute), true))
        {
          MethodTester tester = new MethodTester(method);
          _methods.Add(tester);
        }
      }
    }

    public void RunTests()
    {
      object instance = Activator.CreateInstance(_type);
      foreach (MethodTester tester in _methods)
        tester.RunTest(instance);
    }
  }
}
