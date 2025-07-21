namespace Csla.Test.PerfTest;

public class ChildType1 : BusinessBase<ChildType1>
{
  public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
  public int Id
  {
    get => GetProperty(IdProperty);
    set => SetProperty(IdProperty, value);
  }

  [FetchChild]
  private async Task Fetch(int id)
  {
    LoadProperty(IdProperty, id);
  }
}

public class ChildType2 : BusinessBase<ChildType2>
{
}

public class ChildType3 : BusinessBase<ChildType3>
{
}

public class ChildType4 : BusinessBase<ChildType4>
{
}

public class ChildType5 : BusinessBase<ChildType5>
{
}

public class ChildType11 : BusinessBase<ChildType11>
{
}

public class ChildType12 : BusinessBase<ChildType12>
{
}

public class ChildType13 : BusinessBase<ChildType13>
{
}

public class ChildType14 : BusinessBase<ChildType14>
{
}

public class ChildType15 : BusinessBase<ChildType15>
{
}