namespace Csla.Test.PerfTest;

public class TestChild1 : BusinessBase<TestChild1>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    _ = portal;
  }
}
public class TestChild2 : BusinessBase<TestChild2>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild3 : BusinessBase<TestChild3>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild4 : BusinessBase<TestChild4>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild5 : BusinessBase<TestChild5>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild6 : BusinessBase<TestChild6>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild7 : BusinessBase<TestChild7>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild8 : BusinessBase<TestChild8>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild9 : BusinessBase<TestChild9>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild10 : BusinessBase<TestChild10>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild11 : BusinessBase<TestChild11>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild12 : BusinessBase<TestChild12>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild13 : BusinessBase<TestChild13>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild14 : BusinessBase<TestChild14>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild15 : BusinessBase<TestChild15>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild16 : BusinessBase<TestChild16>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild17 : BusinessBase<TestChild17>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild18 : BusinessBase<TestChild18>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild19 : BusinessBase<TestChild19>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild20 : BusinessBase<TestChild20>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild21 : BusinessBase<TestChild21>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild22 : BusinessBase<TestChild22>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild23 : BusinessBase<TestChild23>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild24 : BusinessBase<TestChild24>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild25 : BusinessBase<TestChild25>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild26 : BusinessBase<TestChild26>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild27 : BusinessBase<TestChild27>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild28 : BusinessBase<TestChild28>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild29 : BusinessBase<TestChild29>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild30 : BusinessBase<TestChild30>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild31 : BusinessBase<TestChild31>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild32 : BusinessBase<TestChild32>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild33 : BusinessBase<TestChild33>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild34 : BusinessBase<TestChild34>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild35 : BusinessBase<TestChild35>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild36 : BusinessBase<TestChild36>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild37 : BusinessBase<TestChild37>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild38 : BusinessBase<TestChild38>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild39 : BusinessBase<TestChild39>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild40 : BusinessBase<TestChild40>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild41 : BusinessBase<TestChild41>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild42 : BusinessBase<TestChild42>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild43 : BusinessBase<TestChild43>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild44 : BusinessBase<TestChild44>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild45 : BusinessBase<TestChild45>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild46 : BusinessBase<TestChild46>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild47 : BusinessBase<TestChild47>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild48 : BusinessBase<TestChild48>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild49 : BusinessBase<TestChild49>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild50 : BusinessBase<TestChild50>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild51 : BusinessBase<TestChild51>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild52 : BusinessBase<TestChild52>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild53 : BusinessBase<TestChild53>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild54 : BusinessBase<TestChild54>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild55 : BusinessBase<TestChild55>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild56 : BusinessBase<TestChild56>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild57 : BusinessBase<TestChild57>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild58 : BusinessBase<TestChild58>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild59 : BusinessBase<TestChild59>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild60 : BusinessBase<TestChild60>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild61 : BusinessBase<TestChild61>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild62 : BusinessBase<TestChild62>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild63 : BusinessBase<TestChild63>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild64 : BusinessBase<TestChild64>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild65 : BusinessBase<TestChild65>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild66 : BusinessBase<TestChild66>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild67 : BusinessBase<TestChild67>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild68 : BusinessBase<TestChild68>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild69 : BusinessBase<TestChild69>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild70 : BusinessBase<TestChild70>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild71 : BusinessBase<TestChild71>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild72 : BusinessBase<TestChild72>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild73 : BusinessBase<TestChild73>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild74 : BusinessBase<TestChild74>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild75 : BusinessBase<TestChild75>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild76 : BusinessBase<TestChild76>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild77 : BusinessBase<TestChild77>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild78 : BusinessBase<TestChild78>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild79 : BusinessBase<TestChild79>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild80 : BusinessBase<TestChild80>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild81 : BusinessBase<TestChild81>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild82 : BusinessBase<TestChild82>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild83 : BusinessBase<TestChild83>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild84 : BusinessBase<TestChild84>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild85 : BusinessBase<TestChild85>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild86 : BusinessBase<TestChild86>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild87 : BusinessBase<TestChild87>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild88 : BusinessBase<TestChild88>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild89 : BusinessBase<TestChild89>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild90 : BusinessBase<TestChild90>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild91 : BusinessBase<TestChild91>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild92 : BusinessBase<TestChild92>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild93 : BusinessBase<TestChild93>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild94 : BusinessBase<TestChild94>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild95 : BusinessBase<TestChild95>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild96 : BusinessBase<TestChild96>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild97 : BusinessBase<TestChild97>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild98 : BusinessBase<TestChild98>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
public class TestChild99 : BusinessBase<TestChild99>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  public static readonly PropertyInfo<string> Name2Property = RegisterProperty<string>(nameof(Name2));
  public string Name2
  {
    get => GetProperty(Name2Property);
    set => SetProperty(Name2Property, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1ValueProperty = RegisterProperty<TestChild1>(nameof(TestChild1Value));
  public TestChild1 TestChild1Value
  {
    get => GetProperty(TestChild1ValueProperty);
    set => SetProperty(TestChild1ValueProperty, value);
  }

  [CreateChild]
  private void Create([Inject] IChildDataPortal<TestChild1> portal)
  {
    LoadProperty(TestChild1ValueProperty, portal.CreateChild());
  }
}
