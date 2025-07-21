namespace Csla.Test.PerfTest;

public class TestItem : BusinessBase<TestItem>
{
  [Create, Fetch]
  [RunLocal]
  private async Task CreateFetch([Inject] IChildDataPortalFactory childDataPortalFactory)
  {
    Child1List = await childDataPortalFactory.GetPortal<ChildList>().FetchChildAsync();
    TestChild1 = childDataPortalFactory.GetPortal<TestChild1>().CreateChild();
    TestChild2 = childDataPortalFactory.GetPortal<TestChild2>().CreateChild();
    TestChild3 = childDataPortalFactory.GetPortal<TestChild3>().CreateChild();
    TestChild4 = childDataPortalFactory.GetPortal<TestChild4>().CreateChild();
    TestChild5 = childDataPortalFactory.GetPortal<TestChild5>().CreateChild();
    TestChild6 = childDataPortalFactory.GetPortal<TestChild6>().CreateChild();
    TestChild7 = childDataPortalFactory.GetPortal<TestChild7>().CreateChild();
    TestChild8 = childDataPortalFactory.GetPortal<TestChild8>().CreateChild();
    TestChild9 = childDataPortalFactory.GetPortal<TestChild9>().CreateChild();
    TestChild10 = childDataPortalFactory.GetPortal<TestChild10>().CreateChild();
    TestChild11 = childDataPortalFactory.GetPortal<TestChild11>().CreateChild();
    TestChild12 = childDataPortalFactory.GetPortal<TestChild12>().CreateChild();
    TestChild13 = childDataPortalFactory.GetPortal<TestChild13>().CreateChild();
    TestChild14 = childDataPortalFactory.GetPortal<TestChild14>().CreateChild();
    TestChild15 = childDataPortalFactory.GetPortal<TestChild15>().CreateChild();
    TestChild16 = childDataPortalFactory.GetPortal<TestChild16>().CreateChild();
    TestChild17 = childDataPortalFactory.GetPortal<TestChild17>().CreateChild();
    TestChild18 = childDataPortalFactory.GetPortal<TestChild18>().CreateChild();
    TestChild19 = childDataPortalFactory.GetPortal<TestChild19>().CreateChild();
    TestChild20 = childDataPortalFactory.GetPortal<TestChild20>().CreateChild();
    TestChild21 = childDataPortalFactory.GetPortal<TestChild21>().CreateChild();
    TestChild22 = childDataPortalFactory.GetPortal<TestChild22>().CreateChild();
    TestChild23 = childDataPortalFactory.GetPortal<TestChild23>().CreateChild();
    TestChild24 = childDataPortalFactory.GetPortal<TestChild24>().CreateChild();
    TestChild25 = childDataPortalFactory.GetPortal<TestChild25>().CreateChild();
    TestChild26 = childDataPortalFactory.GetPortal<TestChild26>().CreateChild();
    TestChild27 = childDataPortalFactory.GetPortal<TestChild27>().CreateChild();
    TestChild28 = childDataPortalFactory.GetPortal<TestChild28>().CreateChild();
    TestChild29 = childDataPortalFactory.GetPortal<TestChild29>().CreateChild();
    TestChild30 = childDataPortalFactory.GetPortal<TestChild30>().CreateChild();
    TestChild31 = childDataPortalFactory.GetPortal<TestChild31>().CreateChild();
    TestChild32 = childDataPortalFactory.GetPortal<TestChild32>().CreateChild();
    TestChild33 = childDataPortalFactory.GetPortal<TestChild33>().CreateChild();
    TestChild34 = childDataPortalFactory.GetPortal<TestChild34>().CreateChild();
    TestChild35 = childDataPortalFactory.GetPortal<TestChild35>().CreateChild();
    TestChild36 = childDataPortalFactory.GetPortal<TestChild36>().CreateChild();
    TestChild37 = childDataPortalFactory.GetPortal<TestChild37>().CreateChild();
    TestChild38 = childDataPortalFactory.GetPortal<TestChild38>().CreateChild();
    TestChild39 = childDataPortalFactory.GetPortal<TestChild39>().CreateChild();
    TestChild40 = childDataPortalFactory.GetPortal<TestChild40>().CreateChild();
    TestChild41 = childDataPortalFactory.GetPortal<TestChild41>().CreateChild();
    TestChild42 = childDataPortalFactory.GetPortal<TestChild42>().CreateChild();
    TestChild43 = childDataPortalFactory.GetPortal<TestChild43>().CreateChild();
    TestChild44 = childDataPortalFactory.GetPortal<TestChild44>().CreateChild();
    TestChild45 = childDataPortalFactory.GetPortal<TestChild45>().CreateChild();
    TestChild46 = childDataPortalFactory.GetPortal<TestChild46>().CreateChild();
    TestChild47 = childDataPortalFactory.GetPortal<TestChild47>().CreateChild();
    TestChild48 = childDataPortalFactory.GetPortal<TestChild48>().CreateChild();
    TestChild49 = childDataPortalFactory.GetPortal<TestChild49>().CreateChild();

    TestChild50 = childDataPortalFactory.GetPortal<TestChild50>().CreateChild();
    TestChild51 = childDataPortalFactory.GetPortal<TestChild51>().CreateChild();
    TestChild52 = childDataPortalFactory.GetPortal<TestChild52>().CreateChild();
    TestChild53 = childDataPortalFactory.GetPortal<TestChild53>().CreateChild();
    TestChild54 = childDataPortalFactory.GetPortal<TestChild54>().CreateChild();
    TestChild55 = childDataPortalFactory.GetPortal<TestChild55>().CreateChild();
    TestChild56 = childDataPortalFactory.GetPortal<TestChild56>().CreateChild();
    TestChild57 = childDataPortalFactory.GetPortal<TestChild57>().CreateChild();
    TestChild58 = childDataPortalFactory.GetPortal<TestChild58>().CreateChild();
    TestChild59 = childDataPortalFactory.GetPortal<TestChild59>().CreateChild();
    TestChild60 = childDataPortalFactory.GetPortal<TestChild60>().CreateChild();
    TestChild61 = childDataPortalFactory.GetPortal<TestChild61>().CreateChild();
    TestChild62 = childDataPortalFactory.GetPortal<TestChild62>().CreateChild();
    TestChild63 = childDataPortalFactory.GetPortal<TestChild63>().CreateChild();
    TestChild64 = childDataPortalFactory.GetPortal<TestChild64>().CreateChild();
    TestChild65 = childDataPortalFactory.GetPortal<TestChild65>().CreateChild();
    TestChild66 = childDataPortalFactory.GetPortal<TestChild66>().CreateChild();
    TestChild67 = childDataPortalFactory.GetPortal<TestChild67>().CreateChild();
    TestChild68 = childDataPortalFactory.GetPortal<TestChild68>().CreateChild();
    TestChild69 = childDataPortalFactory.GetPortal<TestChild69>().CreateChild();
    TestChild70 = childDataPortalFactory.GetPortal<TestChild70>().CreateChild();
    TestChild71 = childDataPortalFactory.GetPortal<TestChild71>().CreateChild();
    TestChild72 = childDataPortalFactory.GetPortal<TestChild72>().CreateChild();
    TestChild73 = childDataPortalFactory.GetPortal<TestChild73>().CreateChild();
    TestChild74 = childDataPortalFactory.GetPortal<TestChild74>().CreateChild();
    TestChild75 = childDataPortalFactory.GetPortal<TestChild75>().CreateChild();
    TestChild76 = childDataPortalFactory.GetPortal<TestChild76>().CreateChild();
    TestChild77 = childDataPortalFactory.GetPortal<TestChild77>().CreateChild();
    TestChild78 = childDataPortalFactory.GetPortal<TestChild78>().CreateChild();
    TestChild79 = childDataPortalFactory.GetPortal<TestChild79>().CreateChild();
    TestChild80 = childDataPortalFactory.GetPortal<TestChild80>().CreateChild();
    TestChild81 = childDataPortalFactory.GetPortal<TestChild81>().CreateChild();

    TestChild82 = childDataPortalFactory.GetPortal<TestChild82>().CreateChild();
    TestChild83 = childDataPortalFactory.GetPortal<TestChild83>().CreateChild();
    TestChild84 = childDataPortalFactory.GetPortal<TestChild84>().CreateChild();
    TestChild85 = childDataPortalFactory.GetPortal<TestChild85>().CreateChild();
    TestChild86 = childDataPortalFactory.GetPortal<TestChild86>().CreateChild();
    TestChild87 = childDataPortalFactory.GetPortal<TestChild87>().CreateChild();
    TestChild88 = childDataPortalFactory.GetPortal<TestChild88>().CreateChild();
    TestChild89 = childDataPortalFactory.GetPortal<TestChild89>().CreateChild();
    TestChild90 = childDataPortalFactory.GetPortal<TestChild90>().CreateChild();
    TestChild91 = childDataPortalFactory.GetPortal<TestChild91>().CreateChild();
    TestChild92 = childDataPortalFactory.GetPortal<TestChild92>().CreateChild();
    TestChild93 = childDataPortalFactory.GetPortal<TestChild93>().CreateChild();
    TestChild94 = childDataPortalFactory.GetPortal<TestChild94>().CreateChild();
    TestChild95 = childDataPortalFactory.GetPortal<TestChild95>().CreateChild();
    TestChild96 = childDataPortalFactory.GetPortal<TestChild96>().CreateChild();
    TestChild97 = childDataPortalFactory.GetPortal<TestChild97>().CreateChild();
    TestChild98 = childDataPortalFactory.GetPortal<TestChild98>().CreateChild();
    TestChild99 = childDataPortalFactory.GetPortal<TestChild99>().CreateChild();

  }

  public override string ToString()
  {
    return "TestItem";
  }

  public static readonly PropertyInfo<ChildList> Child1ListProperty = RegisterProperty<ChildList>(nameof(Child1List));
  public ChildList Child1List
  {
    get => GetProperty(Child1ListProperty);
    set => SetProperty(Child1ListProperty, value);
  }

  public static readonly PropertyInfo<TestChild1> TestChild1Property = RegisterProperty<TestChild1>(nameof(TestChild1));
  public TestChild1 TestChild1
  {
    get => GetProperty(TestChild1Property);
    set => SetProperty(TestChild1Property, value);
  }

  public static readonly PropertyInfo<TestChild2> TestChild2Property = RegisterProperty<TestChild2>(nameof(TestChild2));
  public TestChild2 TestChild2
  {
    get => GetProperty(TestChild2Property);
    set => SetProperty(TestChild2Property, value);
  }

  public static readonly PropertyInfo<TestChild3> TestChild3Property = RegisterProperty<TestChild3>(nameof(TestChild3));
  public TestChild3 TestChild3
  {
    get => GetProperty(TestChild3Property);
    set => SetProperty(TestChild3Property, value);
  }

  public static readonly PropertyInfo<TestChild4> TestChild4Property = RegisterProperty<TestChild4>(nameof(TestChild4));
  public TestChild4 TestChild4
  {
    get => GetProperty(TestChild4Property);
    set => SetProperty(TestChild4Property, value);
  }

  public static readonly PropertyInfo<TestChild5> TestChild5Property = RegisterProperty<TestChild5>(nameof(TestChild5));
  public TestChild5 TestChild5
  {
    get => GetProperty(TestChild5Property);
    set => SetProperty(TestChild5Property, value);
  }

  public static readonly PropertyInfo<TestChild6> TestChild6Property = RegisterProperty<TestChild6>(nameof(TestChild6));
  public TestChild6 TestChild6
  {
    get => GetProperty(TestChild6Property);
    set => SetProperty(TestChild6Property, value);
  }

  public static readonly PropertyInfo<TestChild7> TestChild7Property = RegisterProperty<TestChild7>(nameof(TestChild7));
  public TestChild7 TestChild7
  {
    get => GetProperty(TestChild7Property);
    set => SetProperty(TestChild7Property, value);
  }

  public static readonly PropertyInfo<TestChild8> TestChild8Property = RegisterProperty<TestChild8>(nameof(TestChild8));
  public TestChild8 TestChild8
  {
    get => GetProperty(TestChild8Property);
    set => SetProperty(TestChild8Property, value);
  }

  public static readonly PropertyInfo<TestChild9> TestChild9Property = RegisterProperty<TestChild9>(nameof(TestChild9));
  public TestChild9 TestChild9
  {
    get => GetProperty(TestChild9Property);
    set => SetProperty(TestChild9Property, value);
  }

  public static readonly PropertyInfo<TestChild10> TestChild10Property = RegisterProperty<TestChild10>(nameof(TestChild10));
  public TestChild10 TestChild10
  {
    get => GetProperty(TestChild10Property);
    set => SetProperty(TestChild10Property, value);
  }

  public static readonly PropertyInfo<TestChild11> TestChild11Property = RegisterProperty<TestChild11>(nameof(TestChild11));
  public TestChild11 TestChild11
  {
    get => GetProperty(TestChild11Property);
    set => SetProperty(TestChild11Property, value);
  }

  public static readonly PropertyInfo<TestChild12> TestChild12Property = RegisterProperty<TestChild12>(nameof(TestChild12));
  public TestChild12 TestChild12
  {
    get => GetProperty(TestChild12Property);
    set => SetProperty(TestChild12Property, value);
  }

  public static readonly PropertyInfo<TestChild13> TestChild13Property = RegisterProperty<TestChild13>(nameof(TestChild13));
  public TestChild13 TestChild13
  {
    get => GetProperty(TestChild13Property);
    set => SetProperty(TestChild13Property, value);
  }

  public static readonly PropertyInfo<TestChild14> TestChild14Property = RegisterProperty<TestChild14>(nameof(TestChild14));
  public TestChild14 TestChild14
  {
    get => GetProperty(TestChild14Property);
    set => SetProperty(TestChild14Property, value);
  }

  public static readonly PropertyInfo<TestChild15> TestChild15Property = RegisterProperty<TestChild15>(nameof(TestChild15));
  public TestChild15 TestChild15
  {
    get => GetProperty(TestChild15Property);
    set => SetProperty(TestChild15Property, value);
  }

  public static readonly PropertyInfo<TestChild16> TestChild16Property = RegisterProperty<TestChild16>(nameof(TestChild16));
  public TestChild16 TestChild16
  {
    get => GetProperty(TestChild16Property);
    set => SetProperty(TestChild16Property, value);
  }

  public static readonly PropertyInfo<TestChild17> TestChild17Property = RegisterProperty<TestChild17>(nameof(TestChild17));
  public TestChild17 TestChild17
  {
    get => GetProperty(TestChild17Property);
    set => SetProperty(TestChild17Property, value);
  }

  public static readonly PropertyInfo<TestChild18> TestChild18Property = RegisterProperty<TestChild18>(nameof(TestChild18));
  public TestChild18 TestChild18
  {
    get => GetProperty(TestChild18Property);
    set => SetProperty(TestChild18Property, value);
  }

  public static readonly PropertyInfo<TestChild19> TestChild19Property = RegisterProperty<TestChild19>(nameof(TestChild19));
  public TestChild19 TestChild19
  {
    get => GetProperty(TestChild19Property);
    set => SetProperty(TestChild19Property, value);
  }

  public static readonly PropertyInfo<TestChild20> TestChild20Property = RegisterProperty<TestChild20>(nameof(TestChild20));
  public TestChild20 TestChild20
  {
    get => GetProperty(TestChild20Property);
    set => SetProperty(TestChild20Property, value);
  }

  public static readonly PropertyInfo<TestChild21> TestChild21Property = RegisterProperty<TestChild21>(nameof(TestChild21));
  public TestChild21 TestChild21
  {
    get => GetProperty(TestChild21Property);
    set => SetProperty(TestChild21Property, value);
  }

  public static readonly PropertyInfo<TestChild22> TestChild22Property = RegisterProperty<TestChild22>(nameof(TestChild22));
  public TestChild22 TestChild22
  {
    get => GetProperty(TestChild22Property);
    set => SetProperty(TestChild22Property, value);
  }

  public static readonly PropertyInfo<TestChild23> TestChild23Property = RegisterProperty<TestChild23>(nameof(TestChild23));
  public TestChild23 TestChild23
  {
    get => GetProperty(TestChild23Property);
    set => SetProperty(TestChild23Property, value);
  }

  public static readonly PropertyInfo<TestChild24> TestChild24Property = RegisterProperty<TestChild24>(nameof(TestChild24));
  public TestChild24 TestChild24
  {
    get => GetProperty(TestChild24Property);
    set => SetProperty(TestChild24Property, value);
  }

  public static readonly PropertyInfo<TestChild25> TestChild25Property = RegisterProperty<TestChild25>(nameof(TestChild25));
  public TestChild25 TestChild25
  {
    get => GetProperty(TestChild25Property);
    set => SetProperty(TestChild25Property, value);
  }

  public static readonly PropertyInfo<TestChild26> TestChild26Property = RegisterProperty<TestChild26>(nameof(TestChild26));
  public TestChild26 TestChild26
  {
    get => GetProperty(TestChild26Property);
    set => SetProperty(TestChild26Property, value);
  }

  public static readonly PropertyInfo<TestChild27> TestChild27Property = RegisterProperty<TestChild27>(nameof(TestChild27));
  public TestChild27 TestChild27
  {
    get => GetProperty(TestChild27Property);
    set => SetProperty(TestChild27Property, value);
  }

  public static readonly PropertyInfo<TestChild28> TestChild28Property = RegisterProperty<TestChild28>(nameof(TestChild28));
  public TestChild28 TestChild28
  {
    get => GetProperty(TestChild28Property);
    set => SetProperty(TestChild28Property, value);
  }

  public static readonly PropertyInfo<TestChild29> TestChild29Property = RegisterProperty<TestChild29>(nameof(TestChild29));
  public TestChild29 TestChild29
  {
    get => GetProperty(TestChild29Property);
    set => SetProperty(TestChild29Property, value);
  }

  public static readonly PropertyInfo<TestChild30> TestChild30Property = RegisterProperty<TestChild30>(nameof(TestChild30));
  public TestChild30 TestChild30
  {
    get => GetProperty(TestChild30Property);
    set => SetProperty(TestChild30Property, value);
  }

  public static readonly PropertyInfo<TestChild31> TestChild31Property = RegisterProperty<TestChild31>(nameof(TestChild31));
  public TestChild31 TestChild31
  {
    get => GetProperty(TestChild31Property);
    set => SetProperty(TestChild31Property, value);
  }

  public static readonly PropertyInfo<TestChild32> TestChild32Property = RegisterProperty<TestChild32>(nameof(TestChild32));
  public TestChild32 TestChild32
  {
    get => GetProperty(TestChild32Property);
    set => SetProperty(TestChild32Property, value);
  }

  public static readonly PropertyInfo<TestChild33> TestChild33Property = RegisterProperty<TestChild33>(nameof(TestChild33));
  public TestChild33 TestChild33
  {
    get => GetProperty(TestChild33Property);
    set => SetProperty(TestChild33Property, value);
  }

  public static readonly PropertyInfo<TestChild34> TestChild34Property = RegisterProperty<TestChild34>(nameof(TestChild34));
  public TestChild34 TestChild34
  {
    get => GetProperty(TestChild34Property);
    set => SetProperty(TestChild34Property, value);
  }

  public static readonly PropertyInfo<TestChild35> TestChild35Property = RegisterProperty<TestChild35>(nameof(TestChild35));
  public TestChild35 TestChild35
  {
    get => GetProperty(TestChild35Property);
    set => SetProperty(TestChild35Property, value);
  }

  public static readonly PropertyInfo<TestChild36> TestChild36Property = RegisterProperty<TestChild36>(nameof(TestChild36));
  public TestChild36 TestChild36
  {
    get => GetProperty(TestChild36Property);
    set => SetProperty(TestChild36Property, value);
  }

  public static readonly PropertyInfo<TestChild37> TestChild37Property = RegisterProperty<TestChild37>(nameof(TestChild37));
  public TestChild37 TestChild37
  {
    get => GetProperty(TestChild37Property);
    set => SetProperty(TestChild37Property, value);
  }

  public static readonly PropertyInfo<TestChild38> TestChild38Property = RegisterProperty<TestChild38>(nameof(TestChild38));
  public TestChild38 TestChild38
  {
    get => GetProperty(TestChild38Property);
    set => SetProperty(TestChild38Property, value);
  }

  public static readonly PropertyInfo<TestChild39> TestChild39Property = RegisterProperty<TestChild39>(nameof(TestChild39));
  public TestChild39 TestChild39
  {
    get => GetProperty(TestChild39Property);
    set => SetProperty(TestChild39Property, value);
  }

  public static readonly PropertyInfo<TestChild40> TestChild40Property = RegisterProperty<TestChild40>(nameof(TestChild40));
  public TestChild40 TestChild40
  {
    get => GetProperty(TestChild40Property);
    set => SetProperty(TestChild40Property, value);
  }

  public static readonly PropertyInfo<TestChild41> TestChild41Property = RegisterProperty<TestChild41>(nameof(TestChild41));
  public TestChild41 TestChild41
  {
    get => GetProperty(TestChild41Property);
    set => SetProperty(TestChild41Property, value);
  }

  public static readonly PropertyInfo<TestChild42> TestChild42Property = RegisterProperty<TestChild42>(nameof(TestChild42));
  public TestChild42 TestChild42
  {
    get => GetProperty(TestChild42Property);
    set => SetProperty(TestChild42Property, value);
  }

  public static readonly PropertyInfo<TestChild43> TestChild43Property = RegisterProperty<TestChild43>(nameof(TestChild43));
  public TestChild43 TestChild43
  {
    get => GetProperty(TestChild43Property);
    set => SetProperty(TestChild43Property, value);
  }

  public static readonly PropertyInfo<TestChild44> TestChild44Property = RegisterProperty<TestChild44>(nameof(TestChild44));
  public TestChild44 TestChild44
  {
    get => GetProperty(TestChild44Property);
    set => SetProperty(TestChild44Property, value);
  }

  public static readonly PropertyInfo<TestChild45> TestChild45Property = RegisterProperty<TestChild45>(nameof(TestChild45));
  public TestChild45 TestChild45
  {
    get => GetProperty(TestChild45Property);
    set => SetProperty(TestChild45Property, value);
  }

  public static readonly PropertyInfo<TestChild46> TestChild46Property = RegisterProperty<TestChild46>(nameof(TestChild46));
  public TestChild46 TestChild46
  {
    get => GetProperty(TestChild46Property);
    set => SetProperty(TestChild46Property, value);
  }

  public static readonly PropertyInfo<TestChild47> TestChild47Property = RegisterProperty<TestChild47>(nameof(TestChild47));
  public TestChild47 TestChild47
  {
    get => GetProperty(TestChild47Property);
    set => SetProperty(TestChild47Property, value);
  }

  public static readonly PropertyInfo<TestChild48> TestChild48Property = RegisterProperty<TestChild48>(nameof(TestChild48));
  public TestChild48 TestChild48
  {
    get => GetProperty(TestChild48Property);
    set => SetProperty(TestChild48Property, value);
  }

  public static readonly PropertyInfo<TestChild49> TestChild49Property = RegisterProperty<TestChild49>(nameof(TestChild49));
  public TestChild49 TestChild49
  {
    get => GetProperty(TestChild49Property);
    set => SetProperty(TestChild49Property, value);
  }

  public static readonly PropertyInfo<TestChild50> TestChild50Property = RegisterProperty<TestChild50>(nameof(TestChild50));
  public TestChild50 TestChild50
  {
    get => GetProperty(TestChild50Property);
    set => SetProperty(TestChild50Property, value);
  }

  public static readonly PropertyInfo<TestChild51> TestChild51Property = RegisterProperty<TestChild51>(nameof(TestChild51));
  public TestChild51 TestChild51
  {
    get => GetProperty(TestChild51Property);
    set => SetProperty(TestChild51Property, value);
  }

  public static readonly PropertyInfo<TestChild52> TestChild52Property = RegisterProperty<TestChild52>(nameof(TestChild52));
  public TestChild52 TestChild52
  {
    get => GetProperty(TestChild52Property);
    set => SetProperty(TestChild52Property, value);
  }

  public static readonly PropertyInfo<TestChild53> TestChild53Property = RegisterProperty<TestChild53>(nameof(TestChild53));
  public TestChild53 TestChild53
  {
    get => GetProperty(TestChild53Property);
    set => SetProperty(TestChild53Property, value);
  }

  public static readonly PropertyInfo<TestChild54> TestChild54Property = RegisterProperty<TestChild54>(nameof(TestChild54));
  public TestChild54 TestChild54
  {
    get => GetProperty(TestChild54Property);
    set => SetProperty(TestChild54Property, value);
  }

  public static readonly PropertyInfo<TestChild55> TestChild55Property = RegisterProperty<TestChild55>(nameof(TestChild55));
  public TestChild55 TestChild55
  {
    get => GetProperty(TestChild55Property);
    set => SetProperty(TestChild55Property, value);
  }

  public static readonly PropertyInfo<TestChild56> TestChild56Property = RegisterProperty<TestChild56>(nameof(TestChild56));
  public TestChild56 TestChild56
  {
    get => GetProperty(TestChild56Property);
    set => SetProperty(TestChild56Property, value);
  }

  public static readonly PropertyInfo<TestChild57> TestChild57Property = RegisterProperty<TestChild57>(nameof(TestChild57));
  public TestChild57 TestChild57
  {
    get => GetProperty(TestChild57Property);
    set => SetProperty(TestChild57Property, value);
  }

  public static readonly PropertyInfo<TestChild58> TestChild58Property = RegisterProperty<TestChild58>(nameof(TestChild58));
  public TestChild58 TestChild58
  {
    get => GetProperty(TestChild58Property);
    set => SetProperty(TestChild58Property, value);
  }

  public static readonly PropertyInfo<TestChild59> TestChild59Property = RegisterProperty<TestChild59>(nameof(TestChild59));
  public TestChild59 TestChild59
  {
    get => GetProperty(TestChild59Property);
    set => SetProperty(TestChild59Property, value);
  }

  public static readonly PropertyInfo<TestChild60> TestChild60Property = RegisterProperty<TestChild60>(nameof(TestChild60));
  public TestChild60 TestChild60
  {
    get => GetProperty(TestChild60Property);
    set => SetProperty(TestChild60Property, value);
  }

  public static readonly PropertyInfo<TestChild61> TestChild61Property = RegisterProperty<TestChild61>(nameof(TestChild61));
  public TestChild61 TestChild61
  {
    get => GetProperty(TestChild61Property);
    set => SetProperty(TestChild61Property, value);
  }

  public static readonly PropertyInfo<TestChild62> TestChild62Property = RegisterProperty<TestChild62>(nameof(TestChild62));
  public TestChild62 TestChild62
  {
    get => GetProperty(TestChild62Property);
    set => SetProperty(TestChild62Property, value);
  }

  public static readonly PropertyInfo<TestChild63> TestChild63Property = RegisterProperty<TestChild63>(nameof(TestChild63));
  public TestChild63 TestChild63
  {
    get => GetProperty(TestChild63Property);
    set => SetProperty(TestChild63Property, value);
  }

  public static readonly PropertyInfo<TestChild64> TestChild64Property = RegisterProperty<TestChild64>(nameof(TestChild64));
  public TestChild64 TestChild64
  {
    get => GetProperty(TestChild64Property);
    set => SetProperty(TestChild64Property, value);
  }

  public static readonly PropertyInfo<TestChild65> TestChild65Property = RegisterProperty<TestChild65>(nameof(TestChild65));
  public TestChild65 TestChild65
  {
    get => GetProperty(TestChild65Property);
    set => SetProperty(TestChild65Property, value);
  }

  public static readonly PropertyInfo<TestChild66> TestChild66Property = RegisterProperty<TestChild66>(nameof(TestChild66));
  public TestChild66 TestChild66
  {
    get => GetProperty(TestChild66Property);
    set => SetProperty(TestChild66Property, value);
  }

  public static readonly PropertyInfo<TestChild67> TestChild67Property = RegisterProperty<TestChild67>(nameof(TestChild67));
  public TestChild67 TestChild67
  {
    get => GetProperty(TestChild67Property);
    set => SetProperty(TestChild67Property, value);
  }

  public static readonly PropertyInfo<TestChild68> TestChild68Property = RegisterProperty<TestChild68>(nameof(TestChild68));
  public TestChild68 TestChild68
  {
    get => GetProperty(TestChild68Property);
    set => SetProperty(TestChild68Property, value);
  }

  public static readonly PropertyInfo<TestChild69> TestChild69Property = RegisterProperty<TestChild69>(nameof(TestChild69));
  public TestChild69 TestChild69
  {
    get => GetProperty(TestChild69Property);
    set => SetProperty(TestChild69Property, value);
  }

  public static readonly PropertyInfo<TestChild70> TestChild70Property = RegisterProperty<TestChild70>(nameof(TestChild70));
  public TestChild70 TestChild70
  {
    get => GetProperty(TestChild70Property);
    set => SetProperty(TestChild70Property, value);
  }

  public static readonly PropertyInfo<TestChild71> TestChild71Property = RegisterProperty<TestChild71>(nameof(TestChild71));
  public TestChild71 TestChild71
  {
    get => GetProperty(TestChild71Property);
    set => SetProperty(TestChild71Property, value);
  }

  public static readonly PropertyInfo<TestChild72> TestChild72Property = RegisterProperty<TestChild72>(nameof(TestChild72));
  public TestChild72 TestChild72
  {
    get => GetProperty(TestChild72Property);
    set => SetProperty(TestChild72Property, value);
  }

  public static readonly PropertyInfo<TestChild73> TestChild73Property = RegisterProperty<TestChild73>(nameof(TestChild73));
  public TestChild73 TestChild73
  {
    get => GetProperty(TestChild73Property);
    set => SetProperty(TestChild73Property, value);
  }

  public static readonly PropertyInfo<TestChild74> TestChild74Property = RegisterProperty<TestChild74>(nameof(TestChild74));
  public TestChild74 TestChild74
  {
    get => GetProperty(TestChild74Property);
    set => SetProperty(TestChild74Property, value);
  }

  public static readonly PropertyInfo<TestChild75> TestChild75Property = RegisterProperty<TestChild75>(nameof(TestChild75));
  public TestChild75 TestChild75
  {
    get => GetProperty(TestChild75Property);
    set => SetProperty(TestChild75Property, value);
  }

  public static readonly PropertyInfo<TestChild76> TestChild76Property = RegisterProperty<TestChild76>(nameof(TestChild76));
  public TestChild76 TestChild76
  {
    get => GetProperty(TestChild76Property);
    set => SetProperty(TestChild76Property, value);
  }

  public static readonly PropertyInfo<TestChild77> TestChild77Property = RegisterProperty<TestChild77>(nameof(TestChild77));
  public TestChild77 TestChild77
  {
    get => GetProperty(TestChild77Property);
    set => SetProperty(TestChild77Property, value);
  }

  public static readonly PropertyInfo<TestChild78> TestChild78Property = RegisterProperty<TestChild78>(nameof(TestChild78));
  public TestChild78 TestChild78
  {
    get => GetProperty(TestChild78Property);
    set => SetProperty(TestChild78Property, value);
  }

  public static readonly PropertyInfo<TestChild79> TestChild79Property = RegisterProperty<TestChild79>(nameof(TestChild79));
  public TestChild79 TestChild79
  {
    get => GetProperty(TestChild79Property);
    set => SetProperty(TestChild79Property, value);
  }

  public static readonly PropertyInfo<TestChild80> TestChild80Property = RegisterProperty<TestChild80>(nameof(TestChild80));
  public TestChild80 TestChild80
  {
    get => GetProperty(TestChild80Property);
    set => SetProperty(TestChild80Property, value);
  }

  public static readonly PropertyInfo<TestChild81> TestChild81Property = RegisterProperty<TestChild81>(nameof(TestChild81));
  public TestChild81 TestChild81
  {
    get => GetProperty(TestChild81Property);
    set => SetProperty(TestChild81Property, value);
  }

  public static readonly PropertyInfo<TestChild82> TestChild82Property = RegisterProperty<TestChild82>(nameof(TestChild82));
  public TestChild82 TestChild82
  {
    get => GetProperty(TestChild82Property);
    set => SetProperty(TestChild82Property, value);
  }

  public static readonly PropertyInfo<TestChild83> TestChild83Property = RegisterProperty<TestChild83>(nameof(TestChild83));
  public TestChild83 TestChild83
  {
    get => GetProperty(TestChild83Property);
    set => SetProperty(TestChild83Property, value);
  }

  public static readonly PropertyInfo<TestChild84> TestChild84Property = RegisterProperty<TestChild84>(nameof(TestChild84));
  public TestChild84 TestChild84
  {
    get => GetProperty(TestChild84Property);
    set => SetProperty(TestChild84Property, value);
  }

  public static readonly PropertyInfo<TestChild85> TestChild85Property = RegisterProperty<TestChild85>(nameof(TestChild85));
  public TestChild85 TestChild85
  {
    get => GetProperty(TestChild85Property);
    set => SetProperty(TestChild85Property, value);
  }

  public static readonly PropertyInfo<TestChild86> TestChild86Property = RegisterProperty<TestChild86>(nameof(TestChild86));
  public TestChild86 TestChild86
  {
    get => GetProperty(TestChild86Property);
    set => SetProperty(TestChild86Property, value);
  }

  public static readonly PropertyInfo<TestChild87> TestChild87Property = RegisterProperty<TestChild87>(nameof(TestChild87));
  public TestChild87 TestChild87
  {
    get => GetProperty(TestChild87Property);
    set => SetProperty(TestChild87Property, value);
  }

  public static readonly PropertyInfo<TestChild88> TestChild88Property = RegisterProperty<TestChild88>(nameof(TestChild88));
  public TestChild88 TestChild88
  {
    get => GetProperty(TestChild88Property);
    set => SetProperty(TestChild88Property, value);
  }

  public static readonly PropertyInfo<TestChild89> TestChild89Property = RegisterProperty<TestChild89>(nameof(TestChild89));
  public TestChild89 TestChild89
  {
    get => GetProperty(TestChild89Property);
    set => SetProperty(TestChild89Property, value);
  }

  public static readonly PropertyInfo<TestChild90> TestChild90Property = RegisterProperty<TestChild90>(nameof(TestChild90));
  public TestChild90 TestChild90
  {
    get => GetProperty(TestChild90Property);
    set => SetProperty(TestChild90Property, value);
  }

  public static readonly PropertyInfo<TestChild91> TestChild91Property = RegisterProperty<TestChild91>(nameof(TestChild91));
  public TestChild91 TestChild91
  {
    get => GetProperty(TestChild91Property);
    set => SetProperty(TestChild91Property, value);
  }

  public static readonly PropertyInfo<TestChild92> TestChild92Property = RegisterProperty<TestChild92>(nameof(TestChild92));
  public TestChild92 TestChild92
  {
    get => GetProperty(TestChild92Property);
    set => SetProperty(TestChild92Property, value);
  }

  public static readonly PropertyInfo<TestChild93> TestChild93Property = RegisterProperty<TestChild93>(nameof(TestChild93));
  public TestChild93 TestChild93
  {
    get => GetProperty(TestChild93Property);
    set => SetProperty(TestChild93Property, value);
  }

  public static readonly PropertyInfo<TestChild94> TestChild94Property = RegisterProperty<TestChild94>(nameof(TestChild94));
  public TestChild94 TestChild94
  {
    get => GetProperty(TestChild94Property);
    set => SetProperty(TestChild94Property, value);
  }

  public static readonly PropertyInfo<TestChild95> TestChild95Property = RegisterProperty<TestChild95>(nameof(TestChild95));
  public TestChild95 TestChild95
  {
    get => GetProperty(TestChild95Property);
    set => SetProperty(TestChild95Property, value);
  }

  public static readonly PropertyInfo<TestChild96> TestChild96Property = RegisterProperty<TestChild96>(nameof(TestChild96));
  public TestChild96 TestChild96
  {
    get => GetProperty(TestChild96Property);
    set => SetProperty(TestChild96Property, value);
  }

  public static readonly PropertyInfo<TestChild97> TestChild97Property = RegisterProperty<TestChild97>(nameof(TestChild97));
  public TestChild97 TestChild97
  {
    get => GetProperty(TestChild97Property);
    set => SetProperty(TestChild97Property, value);
  }

  public static readonly PropertyInfo<TestChild98> TestChild98Property = RegisterProperty<TestChild98>(nameof(TestChild98));
  public TestChild98 TestChild98
  {
    get => GetProperty(TestChild98Property);
    set => SetProperty(TestChild98Property, value);
  }

  public static readonly PropertyInfo<TestChild99> TestChild99Property = RegisterProperty<TestChild99>(nameof(TestChild99));
  public TestChild99 TestChild99
  {
    get => GetProperty(TestChild99Property);
    set => SetProperty(TestChild99Property, value);
  }

  public static readonly PropertyInfo<ChildType1> Child1Property = RegisterProperty<ChildType1>(nameof(Child1));
  public ChildType1 Child1
  {
    get => GetProperty(Child1Property);
    set => SetProperty(Child1Property, value);
  }

  public static readonly PropertyInfo<ChildType2> Child2Property = RegisterProperty<ChildType2>(nameof(Child2));
  public ChildType2 Child2
  {
    get => GetProperty(Child2Property);
    set => SetProperty(Child2Property, value);
  }

  public static readonly PropertyInfo<ChildType3> Child3Property = RegisterProperty<ChildType3>(nameof(Child3));
  public ChildType3 Child3
  {
    get => GetProperty(Child3Property);
    set => SetProperty(Child3Property, value);
  }

  public static readonly PropertyInfo<ChildType4> Child4Property = RegisterProperty<ChildType4>(nameof(Child4));
  public ChildType4 Child4
  {
    get => GetProperty(Child4Property);
    set => SetProperty(Child4Property, value);
  }

  public static readonly PropertyInfo<ChildType5> Child5Property = RegisterProperty<ChildType5>(nameof(Child5));
  public ChildType5 Child5
  {
    get => GetProperty(Child5Property);
    set => SetProperty(Child5Property, value);
  }

  public static readonly PropertyInfo<ChildType1> Child11Property = RegisterProperty<ChildType1>(nameof(Child11));
  public ChildType1 Child11
  {
    get => GetProperty(Child11Property);
    set => SetProperty(Child11Property, value);
  }

  public static readonly PropertyInfo<ChildType12> Child12Property = RegisterProperty<ChildType12>(nameof(Child12));
  public ChildType12 Child12
  {
    get => GetProperty(Child12Property);
    set => SetProperty(Child12Property, value);
  }

  public static readonly PropertyInfo<ChildType13> Child13Property = RegisterProperty<ChildType13>(nameof(Child13));
  public ChildType13 Child13
  {
    get => GetProperty(Child13Property);
    set => SetProperty(Child13Property, value);
  }

  public static readonly PropertyInfo<ChildType14> Child14Property = RegisterProperty<ChildType14>(nameof(Child14));
  public ChildType14 Child14
  {
    get => GetProperty(Child14Property);
    set => SetProperty(Child14Property, value);
  }

  public static readonly PropertyInfo<ChildType15> Child15Property = RegisterProperty<ChildType15>(nameof(Child15));
  public ChildType15 Child15
  {
    get => GetProperty(Child15Property);
    set => SetProperty(Child15Property, value);
  }

  public static readonly PropertyInfo<bool> BoolValueProperty = RegisterProperty<bool>(nameof(BoolValue));
  public bool BoolValue
  {
    get => GetProperty(BoolValueProperty);
    set => SetProperty(BoolValueProperty, value);
  }

  public static readonly PropertyInfo<int> IntValueProperty = RegisterProperty<int>(nameof(IntValue));
  public int IntValue
  {
    get => GetProperty(IntValueProperty);
    set => SetProperty(IntValueProperty, value);
  }

  public static readonly PropertyInfo<DateTime> DateTimeValueProperty = RegisterProperty<DateTime>(nameof(DateTimeValue));
  public DateTime DateTimeValue
  {
    get => GetProperty(DateTimeValueProperty);
    set => SetProperty(DateTimeValueProperty, value);
  }

  public static readonly PropertyInfo<Guid> GuidValueProperty = RegisterProperty<Guid>(nameof(GuidValue));
  public Guid GuidValue
  {
    get => GetProperty(GuidValueProperty);
    set => SetProperty(GuidValueProperty, value);
  }

  public static readonly PropertyInfo<float> FloatValueProperty = RegisterProperty<float>(nameof(FloatValue));
  public float FloatValue
  {
    get => GetProperty(FloatValueProperty);
    set => SetProperty(FloatValueProperty, value);
  }

  public static readonly PropertyInfo<string> Value1Property = RegisterProperty<string>(nameof(Value1));
  public string Value1
  {
    get => GetProperty(Value1Property);
    set => SetProperty(Value1Property, value);
  }


  public static readonly PropertyInfo<string> Value2Property = RegisterProperty<string>(nameof(Value2));
  public string Value2
  {
    get => GetProperty(Value2Property);
    set => SetProperty(Value2Property, value);
  }


  public static readonly PropertyInfo<string> Value3Property = RegisterProperty<string>(nameof(Value3));
  public string Value3
  {
    get => GetProperty(Value3Property);
    set => SetProperty(Value3Property, value);
  }


  public static readonly PropertyInfo<string> Value4Property = RegisterProperty<string>(nameof(Value4));
  public string Value4
  {
    get => GetProperty(Value4Property);
    set => SetProperty(Value4Property, value);
  }


  public static readonly PropertyInfo<string> Value5Property = RegisterProperty<string>(nameof(Value5));
  public string Value5
  {
    get => GetProperty(Value5Property);
    set => SetProperty(Value5Property, value);
  }


  public static readonly PropertyInfo<string> Value6Property = RegisterProperty<string>(nameof(Value6));
  public string Value6
  {
    get => GetProperty(Value6Property);
    set => SetProperty(Value6Property, value);
  }


  public static readonly PropertyInfo<string> Value7Property = RegisterProperty<string>(nameof(Value7));
  public string Value7
  {
    get => GetProperty(Value7Property);
    set => SetProperty(Value7Property, value);
  }


  public static readonly PropertyInfo<string> Value8Property = RegisterProperty<string>(nameof(Value8));
  public string Value8
  {
    get => GetProperty(Value8Property);
    set => SetProperty(Value8Property, value);
  }


  public static readonly PropertyInfo<string> Value9Property = RegisterProperty<string>(nameof(Value9));
  public string Value9
  {
    get => GetProperty(Value9Property);
    set => SetProperty(Value9Property, value);
  }


  public static readonly PropertyInfo<string> Value10Property = RegisterProperty<string>(nameof(Value10));
  public string Value10
  {
    get => GetProperty(Value10Property);
    set => SetProperty(Value10Property, value);
  }


  public static readonly PropertyInfo<string> Value11Property = RegisterProperty<string>(nameof(Value11));
  public string Value11
  {
    get => GetProperty(Value11Property);
    set => SetProperty(Value11Property, value);
  }


  public static readonly PropertyInfo<string> Value12Property = RegisterProperty<string>(nameof(Value12));
  public string Value12
  {
    get => GetProperty(Value12Property);
    set => SetProperty(Value12Property, value);
  }


  public static readonly PropertyInfo<string> Value13Property = RegisterProperty<string>(nameof(Value13));
  public string Value13
  {
    get => GetProperty(Value13Property);
    set => SetProperty(Value13Property, value);
  }


  public static readonly PropertyInfo<string> Value14Property = RegisterProperty<string>(nameof(Value14));
  public string Value14
  {
    get => GetProperty(Value14Property);
    set => SetProperty(Value14Property, value);
  }


  public static readonly PropertyInfo<string> Value15Property = RegisterProperty<string>(nameof(Value15));
  public string Value15
  {
    get => GetProperty(Value15Property);
    set => SetProperty(Value15Property, value);
  }


  public static readonly PropertyInfo<string> Value16Property = RegisterProperty<string>(nameof(Value16));
  public string Value16
  {
    get => GetProperty(Value16Property);
    set => SetProperty(Value16Property, value);
  }


  public static readonly PropertyInfo<string> Value17Property = RegisterProperty<string>(nameof(Value17));
  public string Value17
  {
    get => GetProperty(Value17Property);
    set => SetProperty(Value17Property, value);
  }


  public static readonly PropertyInfo<string> Value18Property = RegisterProperty<string>(nameof(Value18));
  public string Value18
  {
    get => GetProperty(Value18Property);
    set => SetProperty(Value18Property, value);
  }


  public static readonly PropertyInfo<string> Value19Property = RegisterProperty<string>(nameof(Value19));
  public string Value19
  {
    get => GetProperty(Value19Property);
    set => SetProperty(Value19Property, value);
  }


  public static readonly PropertyInfo<string> Value20Property = RegisterProperty<string>(nameof(Value20));
  public string Value20
  {
    get => GetProperty(Value20Property);
    set => SetProperty(Value20Property, value);
  }


  public static readonly PropertyInfo<string> Value21Property = RegisterProperty<string>(nameof(Value21));
  public string Value21
  {
    get => GetProperty(Value21Property);
    set => SetProperty(Value21Property, value);
  }


  public static readonly PropertyInfo<string> Value22Property = RegisterProperty<string>(nameof(Value22));
  public string Value22
  {
    get => GetProperty(Value22Property);
    set => SetProperty(Value22Property, value);
  }


  public static readonly PropertyInfo<string> Value23Property = RegisterProperty<string>(nameof(Value23));
  public string Value23
  {
    get => GetProperty(Value23Property);
    set => SetProperty(Value23Property, value);
  }


  public static readonly PropertyInfo<string> Value24Property = RegisterProperty<string>(nameof(Value24));
  public string Value24
  {
    get => GetProperty(Value24Property);
    set => SetProperty(Value24Property, value);
  }


  public static readonly PropertyInfo<string> Value25Property = RegisterProperty<string>(nameof(Value25));
  public string Value25
  {
    get => GetProperty(Value25Property);
    set => SetProperty(Value25Property, value);
  }


  public static readonly PropertyInfo<string> Value26Property = RegisterProperty<string>(nameof(Value26));
  public string Value26
  {
    get => GetProperty(Value26Property);
    set => SetProperty(Value26Property, value);
  }


  public static readonly PropertyInfo<string> Value27Property = RegisterProperty<string>(nameof(Value27));
  public string Value27
  {
    get => GetProperty(Value27Property);
    set => SetProperty(Value27Property, value);
  }


  public static readonly PropertyInfo<string> Value28Property = RegisterProperty<string>(nameof(Value28));
  public string Value28
  {
    get => GetProperty(Value28Property);
    set => SetProperty(Value28Property, value);
  }


  public static readonly PropertyInfo<string> Value29Property = RegisterProperty<string>(nameof(Value29));
  public string Value29
  {
    get => GetProperty(Value29Property);
    set => SetProperty(Value29Property, value);
  }


  public static readonly PropertyInfo<string> Value30Property = RegisterProperty<string>(nameof(Value30));
  public string Value30
  {
    get => GetProperty(Value30Property);
    set => SetProperty(Value30Property, value);
  }


  public static readonly PropertyInfo<string> Value31Property = RegisterProperty<string>(nameof(Value31));
  public string Value31
  {
    get => GetProperty(Value31Property);
    set => SetProperty(Value31Property, value);
  }


  public static readonly PropertyInfo<string> Value32Property = RegisterProperty<string>(nameof(Value32));
  public string Value32
  {
    get => GetProperty(Value32Property);
    set => SetProperty(Value32Property, value);
  }


  public static readonly PropertyInfo<string> Value33Property = RegisterProperty<string>(nameof(Value33));
  public string Value33
  {
    get => GetProperty(Value33Property);
    set => SetProperty(Value33Property, value);
  }


  public static readonly PropertyInfo<string> Value34Property = RegisterProperty<string>(nameof(Value34));
  public string Value34
  {
    get => GetProperty(Value34Property);
    set => SetProperty(Value34Property, value);
  }


  public static readonly PropertyInfo<string> Value35Property = RegisterProperty<string>(nameof(Value35));
  public string Value35
  {
    get => GetProperty(Value35Property);
    set => SetProperty(Value35Property, value);
  }


  public static readonly PropertyInfo<string> Value36Property = RegisterProperty<string>(nameof(Value36));
  public string Value36
  {
    get => GetProperty(Value36Property);
    set => SetProperty(Value36Property, value);
  }


  public static readonly PropertyInfo<string> Value37Property = RegisterProperty<string>(nameof(Value37));
  public string Value37
  {
    get => GetProperty(Value37Property);
    set => SetProperty(Value37Property, value);
  }


  public static readonly PropertyInfo<string> Value38Property = RegisterProperty<string>(nameof(Value38));
  public string Value38
  {
    get => GetProperty(Value38Property);
    set => SetProperty(Value38Property, value);
  }


  public static readonly PropertyInfo<string> Value39Property = RegisterProperty<string>(nameof(Value39));
  public string Value39
  {
    get => GetProperty(Value39Property);
    set => SetProperty(Value39Property, value);
  }


  public static readonly PropertyInfo<string> Value40Property = RegisterProperty<string>(nameof(Value40));
  public string Value40
  {
    get => GetProperty(Value40Property);
    set => SetProperty(Value40Property, value);
  }


  public static readonly PropertyInfo<string> Value41Property = RegisterProperty<string>(nameof(Value41));
  public string Value41
  {
    get => GetProperty(Value41Property);
    set => SetProperty(Value41Property, value);
  }


  public static readonly PropertyInfo<string> Value42Property = RegisterProperty<string>(nameof(Value42));
  public string Value42
  {
    get => GetProperty(Value42Property);
    set => SetProperty(Value42Property, value);
  }


  public static readonly PropertyInfo<string> Value43Property = RegisterProperty<string>(nameof(Value43));
  public string Value43
  {
    get => GetProperty(Value43Property);
    set => SetProperty(Value43Property, value);
  }


  public static readonly PropertyInfo<string> Value44Property = RegisterProperty<string>(nameof(Value44));
  public string Value44
  {
    get => GetProperty(Value44Property);
    set => SetProperty(Value44Property, value);
  }


  public static readonly PropertyInfo<string> Value45Property = RegisterProperty<string>(nameof(Value45));
  public string Value45
  {
    get => GetProperty(Value45Property);
    set => SetProperty(Value45Property, value);
  }


  public static readonly PropertyInfo<string> Value46Property = RegisterProperty<string>(nameof(Value46));
  public string Value46
  {
    get => GetProperty(Value46Property);
    set => SetProperty(Value46Property, value);
  }


  public static readonly PropertyInfo<string> Value47Property = RegisterProperty<string>(nameof(Value47));
  public string Value47
  {
    get => GetProperty(Value47Property);
    set => SetProperty(Value47Property, value);
  }


  public static readonly PropertyInfo<string> Value48Property = RegisterProperty<string>(nameof(Value48));
  public string Value48
  {
    get => GetProperty(Value48Property);
    set => SetProperty(Value48Property, value);
  }


  public static readonly PropertyInfo<string> Value49Property = RegisterProperty<string>(nameof(Value49));
  public string Value49
  {
    get => GetProperty(Value49Property);
    set => SetProperty(Value49Property, value);
  }


  public static readonly PropertyInfo<string> Value50Property = RegisterProperty<string>(nameof(Value50));
  public string Value50
  {
    get => GetProperty(Value50Property);
    set => SetProperty(Value50Property, value);
  }


  public static readonly PropertyInfo<string> Value51Property = RegisterProperty<string>(nameof(Value51));
  public string Value51
  {
    get => GetProperty(Value51Property);
    set => SetProperty(Value51Property, value);
  }


  public static readonly PropertyInfo<string> Value52Property = RegisterProperty<string>(nameof(Value52));
  public string Value52
  {
    get => GetProperty(Value52Property);
    set => SetProperty(Value52Property, value);
  }


  public static readonly PropertyInfo<string> Value53Property = RegisterProperty<string>(nameof(Value53));
  public string Value53
  {
    get => GetProperty(Value53Property);
    set => SetProperty(Value53Property, value);
  }


  public static readonly PropertyInfo<string> Value54Property = RegisterProperty<string>(nameof(Value54));
  public string Value54
  {
    get => GetProperty(Value54Property);
    set => SetProperty(Value54Property, value);
  }


  public static readonly PropertyInfo<string> Value55Property = RegisterProperty<string>(nameof(Value55));
  public string Value55
  {
    get => GetProperty(Value55Property);
    set => SetProperty(Value55Property, value);
  }


  public static readonly PropertyInfo<string> Value56Property = RegisterProperty<string>(nameof(Value56));
  public string Value56
  {
    get => GetProperty(Value56Property);
    set => SetProperty(Value56Property, value);
  }


  public static readonly PropertyInfo<string> Value57Property = RegisterProperty<string>(nameof(Value57));
  public string Value57
  {
    get => GetProperty(Value57Property);
    set => SetProperty(Value57Property, value);
  }


  public static readonly PropertyInfo<string> Value58Property = RegisterProperty<string>(nameof(Value58));
  public string Value58
  {
    get => GetProperty(Value58Property);
    set => SetProperty(Value58Property, value);
  }


  public static readonly PropertyInfo<string> Value59Property = RegisterProperty<string>(nameof(Value59));
  public string Value59
  {
    get => GetProperty(Value59Property);
    set => SetProperty(Value59Property, value);
  }


  public static readonly PropertyInfo<string> Value60Property = RegisterProperty<string>(nameof(Value60));
  public string Value60
  {
    get => GetProperty(Value60Property);
    set => SetProperty(Value60Property, value);
  }


  public static readonly PropertyInfo<string> Value61Property = RegisterProperty<string>(nameof(Value61));
  public string Value61
  {
    get => GetProperty(Value61Property);
    set => SetProperty(Value61Property, value);
  }


  public static readonly PropertyInfo<string> Value62Property = RegisterProperty<string>(nameof(Value62));
  public string Value62
  {
    get => GetProperty(Value62Property);
    set => SetProperty(Value62Property, value);
  }


  public static readonly PropertyInfo<string> Value63Property = RegisterProperty<string>(nameof(Value63));
  public string Value63
  {
    get => GetProperty(Value63Property);
    set => SetProperty(Value63Property, value);
  }


  public static readonly PropertyInfo<string> Value64Property = RegisterProperty<string>(nameof(Value64));
  public string Value64
  {
    get => GetProperty(Value64Property);
    set => SetProperty(Value64Property, value);
  }


  public static readonly PropertyInfo<string> Value65Property = RegisterProperty<string>(nameof(Value65));
  public string Value65
  {
    get => GetProperty(Value65Property);
    set => SetProperty(Value65Property, value);
  }


  public static readonly PropertyInfo<string> Value66Property = RegisterProperty<string>(nameof(Value66));
  public string Value66
  {
    get => GetProperty(Value66Property);
    set => SetProperty(Value66Property, value);
  }


  public static readonly PropertyInfo<string> Value67Property = RegisterProperty<string>(nameof(Value67));
  public string Value67
  {
    get => GetProperty(Value67Property);
    set => SetProperty(Value67Property, value);
  }


  public static readonly PropertyInfo<string> Value68Property = RegisterProperty<string>(nameof(Value68));
  public string Value68
  {
    get => GetProperty(Value68Property);
    set => SetProperty(Value68Property, value);
  }


  public static readonly PropertyInfo<string> Value69Property = RegisterProperty<string>(nameof(Value69));
  public string Value69
  {
    get => GetProperty(Value69Property);
    set => SetProperty(Value69Property, value);
  }


  public static readonly PropertyInfo<string> Value70Property = RegisterProperty<string>(nameof(Value70));
  public string Value70
  {
    get => GetProperty(Value70Property);
    set => SetProperty(Value70Property, value);
  }


  public static readonly PropertyInfo<string> Value71Property = RegisterProperty<string>(nameof(Value71));
  public string Value71
  {
    get => GetProperty(Value71Property);
    set => SetProperty(Value71Property, value);
  }


  public static readonly PropertyInfo<string> Value72Property = RegisterProperty<string>(nameof(Value72));
  public string Value72
  {
    get => GetProperty(Value72Property);
    set => SetProperty(Value72Property, value);
  }


  public static readonly PropertyInfo<string> Value73Property = RegisterProperty<string>(nameof(Value73));
  public string Value73
  {
    get => GetProperty(Value73Property);
    set => SetProperty(Value73Property, value);
  }


  public static readonly PropertyInfo<string> Value74Property = RegisterProperty<string>(nameof(Value74));
  public string Value74
  {
    get => GetProperty(Value74Property);
    set => SetProperty(Value74Property, value);
  }


  public static readonly PropertyInfo<string> Value75Property = RegisterProperty<string>(nameof(Value75));
  public string Value75
  {
    get => GetProperty(Value75Property);
    set => SetProperty(Value75Property, value);
  }


  public static readonly PropertyInfo<string> Value76Property = RegisterProperty<string>(nameof(Value76));
  public string Value76
  {
    get => GetProperty(Value76Property);
    set => SetProperty(Value76Property, value);
  }


  public static readonly PropertyInfo<string> Value77Property = RegisterProperty<string>(nameof(Value77));
  public string Value77
  {
    get => GetProperty(Value77Property);
    set => SetProperty(Value77Property, value);
  }


  public static readonly PropertyInfo<string> Value78Property = RegisterProperty<string>(nameof(Value78));
  public string Value78
  {
    get => GetProperty(Value78Property);
    set => SetProperty(Value78Property, value);
  }


  public static readonly PropertyInfo<string> Value79Property = RegisterProperty<string>(nameof(Value79));
  public string Value79
  {
    get => GetProperty(Value79Property);
    set => SetProperty(Value79Property, value);
  }


  public static readonly PropertyInfo<string> Value80Property = RegisterProperty<string>(nameof(Value80));
  public string Value80
  {
    get => GetProperty(Value80Property);
    set => SetProperty(Value80Property, value);
  }


  public static readonly PropertyInfo<string> Value81Property = RegisterProperty<string>(nameof(Value81));
  public string Value81
  {
    get => GetProperty(Value81Property);
    set => SetProperty(Value81Property, value);
  }


  public static readonly PropertyInfo<string> Value82Property = RegisterProperty<string>(nameof(Value82));
  public string Value82
  {
    get => GetProperty(Value82Property);
    set => SetProperty(Value82Property, value);
  }


  public static readonly PropertyInfo<string> Value83Property = RegisterProperty<string>(nameof(Value83));
  public string Value83
  {
    get => GetProperty(Value83Property);
    set => SetProperty(Value83Property, value);
  }


  public static readonly PropertyInfo<string> Value84Property = RegisterProperty<string>(nameof(Value84));
  public string Value84
  {
    get => GetProperty(Value84Property);
    set => SetProperty(Value84Property, value);
  }


  public static readonly PropertyInfo<string> Value85Property = RegisterProperty<string>(nameof(Value85));
  public string Value85
  {
    get => GetProperty(Value85Property);
    set => SetProperty(Value85Property, value);
  }


  public static readonly PropertyInfo<string> Value86Property = RegisterProperty<string>(nameof(Value86));
  public string Value86
  {
    get => GetProperty(Value86Property);
    set => SetProperty(Value86Property, value);
  }


  public static readonly PropertyInfo<string> Value87Property = RegisterProperty<string>(nameof(Value87));
  public string Value87
  {
    get => GetProperty(Value87Property);
    set => SetProperty(Value87Property, value);
  }


  public static readonly PropertyInfo<string> Value88Property = RegisterProperty<string>(nameof(Value88));
  public string Value88
  {
    get => GetProperty(Value88Property);
    set => SetProperty(Value88Property, value);
  }


  public static readonly PropertyInfo<string> Value89Property = RegisterProperty<string>(nameof(Value89));
  public string Value89
  {
    get => GetProperty(Value89Property);
    set => SetProperty(Value89Property, value);
  }


  public static readonly PropertyInfo<string> Value90Property = RegisterProperty<string>(nameof(Value90));
  public string Value90
  {
    get => GetProperty(Value90Property);
    set => SetProperty(Value90Property, value);
  }


  public static readonly PropertyInfo<string> Value91Property = RegisterProperty<string>(nameof(Value91));
  public string Value91
  {
    get => GetProperty(Value91Property);
    set => SetProperty(Value91Property, value);
  }


  public static readonly PropertyInfo<string> Value92Property = RegisterProperty<string>(nameof(Value92));
  public string Value92
  {
    get => GetProperty(Value92Property);
    set => SetProperty(Value92Property, value);
  }


  public static readonly PropertyInfo<string> Value93Property = RegisterProperty<string>(nameof(Value93));
  public string Value93
  {
    get => GetProperty(Value93Property);
    set => SetProperty(Value93Property, value);
  }


  public static readonly PropertyInfo<string> Value94Property = RegisterProperty<string>(nameof(Value94));
  public string Value94
  {
    get => GetProperty(Value94Property);
    set => SetProperty(Value94Property, value);
  }


  public static readonly PropertyInfo<string> Value95Property = RegisterProperty<string>(nameof(Value95));
  public string Value95
  {
    get => GetProperty(Value95Property);
    set => SetProperty(Value95Property, value);
  }


  public static readonly PropertyInfo<string> Value96Property = RegisterProperty<string>(nameof(Value96));
  public string Value96
  {
    get => GetProperty(Value96Property);
    set => SetProperty(Value96Property, value);
  }


  public static readonly PropertyInfo<string> Value97Property = RegisterProperty<string>(nameof(Value97));
  public string Value97
  {
    get => GetProperty(Value97Property);
    set => SetProperty(Value97Property, value);
  }


  public static readonly PropertyInfo<string> Value98Property = RegisterProperty<string>(nameof(Value98));
  public string Value98
  {
    get => GetProperty(Value98Property);
    set => SetProperty(Value98Property, value);
  }


  public static readonly PropertyInfo<string> Value99Property = RegisterProperty<string>(nameof(Value99));
  public string Value99
  {
    get => GetProperty(Value99Property);
    set => SetProperty(Value99Property, value);
  }


  public static readonly PropertyInfo<string> Value100Property = RegisterProperty<string>(nameof(Value100));
  public string Value100
  {
    get => GetProperty(Value100Property);
    set => SetProperty(Value100Property, value);
  }


  public static readonly PropertyInfo<string> Value101Property = RegisterProperty<string>(nameof(Value101));
  public string Value101
  {
    get => GetProperty(Value101Property);
    set => SetProperty(Value101Property, value);
  }


  public static readonly PropertyInfo<string> Value102Property = RegisterProperty<string>(nameof(Value102));
  public string Value102
  {
    get => GetProperty(Value102Property);
    set => SetProperty(Value102Property, value);
  }


  public static readonly PropertyInfo<string> Value103Property = RegisterProperty<string>(nameof(Value103));
  public string Value103
  {
    get => GetProperty(Value103Property);
    set => SetProperty(Value103Property, value);
  }


  public static readonly PropertyInfo<string> Value104Property = RegisterProperty<string>(nameof(Value104));
  public string Value104
  {
    get => GetProperty(Value104Property);
    set => SetProperty(Value104Property, value);
  }


  public static readonly PropertyInfo<string> Value105Property = RegisterProperty<string>(nameof(Value105));
  public string Value105
  {
    get => GetProperty(Value105Property);
    set => SetProperty(Value105Property, value);
  }


  public static readonly PropertyInfo<string> Value106Property = RegisterProperty<string>(nameof(Value106));
  public string Value106
  {
    get => GetProperty(Value106Property);
    set => SetProperty(Value106Property, value);
  }


  public static readonly PropertyInfo<string> Value107Property = RegisterProperty<string>(nameof(Value107));
  public string Value107
  {
    get => GetProperty(Value107Property);
    set => SetProperty(Value107Property, value);
  }


  public static readonly PropertyInfo<string> Value108Property = RegisterProperty<string>(nameof(Value108));
  public string Value108
  {
    get => GetProperty(Value108Property);
    set => SetProperty(Value108Property, value);
  }


  public static readonly PropertyInfo<string> Value109Property = RegisterProperty<string>(nameof(Value109));
  public string Value109
  {
    get => GetProperty(Value109Property);
    set => SetProperty(Value109Property, value);
  }


  public static readonly PropertyInfo<string> Value110Property = RegisterProperty<string>(nameof(Value110));
  public string Value110
  {
    get => GetProperty(Value110Property);
    set => SetProperty(Value110Property, value);
  }


  public static readonly PropertyInfo<string> Value111Property = RegisterProperty<string>(nameof(Value111));
  public string Value111
  {
    get => GetProperty(Value111Property);
    set => SetProperty(Value111Property, value);
  }


  public static readonly PropertyInfo<string> Value112Property = RegisterProperty<string>(nameof(Value112));
  public string Value112
  {
    get => GetProperty(Value112Property);
    set => SetProperty(Value112Property, value);
  }


  public static readonly PropertyInfo<string> Value113Property = RegisterProperty<string>(nameof(Value113));
  public string Value113
  {
    get => GetProperty(Value113Property);
    set => SetProperty(Value113Property, value);
  }


  public static readonly PropertyInfo<string> Value114Property = RegisterProperty<string>(nameof(Value114));
  public string Value114
  {
    get => GetProperty(Value114Property);
    set => SetProperty(Value114Property, value);
  }


  public static readonly PropertyInfo<string> Value115Property = RegisterProperty<string>(nameof(Value115));
  public string Value115
  {
    get => GetProperty(Value115Property);
    set => SetProperty(Value115Property, value);
  }


  public static readonly PropertyInfo<string> Value116Property = RegisterProperty<string>(nameof(Value116));
  public string Value116
  {
    get => GetProperty(Value116Property);
    set => SetProperty(Value116Property, value);
  }


  public static readonly PropertyInfo<string> Value117Property = RegisterProperty<string>(nameof(Value117));
  public string Value117
  {
    get => GetProperty(Value117Property);
    set => SetProperty(Value117Property, value);
  }


  public static readonly PropertyInfo<string> Value118Property = RegisterProperty<string>(nameof(Value118));
  public string Value118
  {
    get => GetProperty(Value118Property);
    set => SetProperty(Value118Property, value);
  }


  public static readonly PropertyInfo<string> Value119Property = RegisterProperty<string>(nameof(Value119));
  public string Value119
  {
    get => GetProperty(Value119Property);
    set => SetProperty(Value119Property, value);
  }


  public static readonly PropertyInfo<string> Value120Property = RegisterProperty<string>(nameof(Value120));
  public string Value120
  {
    get => GetProperty(Value120Property);
    set => SetProperty(Value120Property, value);
  }


  public static readonly PropertyInfo<string> Value121Property = RegisterProperty<string>(nameof(Value121));
  public string Value121
  {
    get => GetProperty(Value121Property);
    set => SetProperty(Value121Property, value);
  }


  public static readonly PropertyInfo<string> Value122Property = RegisterProperty<string>(nameof(Value122));
  public string Value122
  {
    get => GetProperty(Value122Property);
    set => SetProperty(Value122Property, value);
  }


  public static readonly PropertyInfo<string> Value123Property = RegisterProperty<string>(nameof(Value123));
  public string Value123
  {
    get => GetProperty(Value123Property);
    set => SetProperty(Value123Property, value);
  }


  public static readonly PropertyInfo<string> Value124Property = RegisterProperty<string>(nameof(Value124));
  public string Value124
  {
    get => GetProperty(Value124Property);
    set => SetProperty(Value124Property, value);
  }


  public static readonly PropertyInfo<string> Value125Property = RegisterProperty<string>(nameof(Value125));
  public string Value125
  {
    get => GetProperty(Value125Property);
    set => SetProperty(Value125Property, value);
  }


  public static readonly PropertyInfo<string> Value126Property = RegisterProperty<string>(nameof(Value126));
  public string Value126
  {
    get => GetProperty(Value126Property);
    set => SetProperty(Value126Property, value);
  }


  public static readonly PropertyInfo<string> Value127Property = RegisterProperty<string>(nameof(Value127));
  public string Value127
  {
    get => GetProperty(Value127Property);
    set => SetProperty(Value127Property, value);
  }


  public static readonly PropertyInfo<string> Value128Property = RegisterProperty<string>(nameof(Value128));
  public string Value128
  {
    get => GetProperty(Value128Property);
    set => SetProperty(Value128Property, value);
  }


  public static readonly PropertyInfo<string> Value129Property = RegisterProperty<string>(nameof(Value129));
  public string Value129
  {
    get => GetProperty(Value129Property);
    set => SetProperty(Value129Property, value);
  }


  public static readonly PropertyInfo<string> Value130Property = RegisterProperty<string>(nameof(Value130));
  public string Value130
  {
    get => GetProperty(Value130Property);
    set => SetProperty(Value130Property, value);
  }


  public static readonly PropertyInfo<string> Value131Property = RegisterProperty<string>(nameof(Value131));
  public string Value131
  {
    get => GetProperty(Value131Property);
    set => SetProperty(Value131Property, value);
  }


  public static readonly PropertyInfo<string> Value132Property = RegisterProperty<string>(nameof(Value132));
  public string Value132
  {
    get => GetProperty(Value132Property);
    set => SetProperty(Value132Property, value);
  }


  public static readonly PropertyInfo<string> Value133Property = RegisterProperty<string>(nameof(Value133));
  public string Value133
  {
    get => GetProperty(Value133Property);
    set => SetProperty(Value133Property, value);
  }


  public static readonly PropertyInfo<string> Value134Property = RegisterProperty<string>(nameof(Value134));
  public string Value134
  {
    get => GetProperty(Value134Property);
    set => SetProperty(Value134Property, value);
  }


  public static readonly PropertyInfo<string> Value135Property = RegisterProperty<string>(nameof(Value135));
  public string Value135
  {
    get => GetProperty(Value135Property);
    set => SetProperty(Value135Property, value);
  }


  public static readonly PropertyInfo<string> Value136Property = RegisterProperty<string>(nameof(Value136));
  public string Value136
  {
    get => GetProperty(Value136Property);
    set => SetProperty(Value136Property, value);
  }


  public static readonly PropertyInfo<string> Value137Property = RegisterProperty<string>(nameof(Value137));
  public string Value137
  {
    get => GetProperty(Value137Property);
    set => SetProperty(Value137Property, value);
  }


  public static readonly PropertyInfo<string> Value138Property = RegisterProperty<string>(nameof(Value138));
  public string Value138
  {
    get => GetProperty(Value138Property);
    set => SetProperty(Value138Property, value);
  }


  public static readonly PropertyInfo<string> Value139Property = RegisterProperty<string>(nameof(Value139));
  public string Value139
  {
    get => GetProperty(Value139Property);
    set => SetProperty(Value139Property, value);
  }


  public static readonly PropertyInfo<string> Value140Property = RegisterProperty<string>(nameof(Value140));
  public string Value140
  {
    get => GetProperty(Value140Property);
    set => SetProperty(Value140Property, value);
  }


  public static readonly PropertyInfo<string> Value141Property = RegisterProperty<string>(nameof(Value141));
  public string Value141
  {
    get => GetProperty(Value141Property);
    set => SetProperty(Value141Property, value);
  }


  public static readonly PropertyInfo<string> Value142Property = RegisterProperty<string>(nameof(Value142));
  public string Value142
  {
    get => GetProperty(Value142Property);
    set => SetProperty(Value142Property, value);
  }


  public static readonly PropertyInfo<string> Value143Property = RegisterProperty<string>(nameof(Value143));
  public string Value143
  {
    get => GetProperty(Value143Property);
    set => SetProperty(Value143Property, value);
  }


  public static readonly PropertyInfo<string> Value144Property = RegisterProperty<string>(nameof(Value144));
  public string Value144
  {
    get => GetProperty(Value144Property);
    set => SetProperty(Value144Property, value);
  }


  public static readonly PropertyInfo<string> Value145Property = RegisterProperty<string>(nameof(Value145));
  public string Value145
  {
    get => GetProperty(Value145Property);
    set => SetProperty(Value145Property, value);
  }


  public static readonly PropertyInfo<string> Value146Property = RegisterProperty<string>(nameof(Value146));
  public string Value146
  {
    get => GetProperty(Value146Property);
    set => SetProperty(Value146Property, value);
  }


  public static readonly PropertyInfo<string> Value147Property = RegisterProperty<string>(nameof(Value147));
  public string Value147
  {
    get => GetProperty(Value147Property);
    set => SetProperty(Value147Property, value);
  }


  public static readonly PropertyInfo<string> Value148Property = RegisterProperty<string>(nameof(Value148));
  public string Value148
  {
    get => GetProperty(Value148Property);
    set => SetProperty(Value148Property, value);
  }


  public static readonly PropertyInfo<string> Value149Property = RegisterProperty<string>(nameof(Value149));
  public string Value149
  {
    get => GetProperty(Value149Property);
    set => SetProperty(Value149Property, value);
  }


  public static readonly PropertyInfo<string> Value150Property = RegisterProperty<string>(nameof(Value150));
  public string Value150
  {
    get => GetProperty(Value150Property);
    set => SetProperty(Value150Property, value);
  }


  public static readonly PropertyInfo<string> Value151Property = RegisterProperty<string>(nameof(Value151));
  public string Value151
  {
    get => GetProperty(Value151Property);
    set => SetProperty(Value151Property, value);
  }


  public static readonly PropertyInfo<string> Value152Property = RegisterProperty<string>(nameof(Value152));
  public string Value152
  {
    get => GetProperty(Value152Property);
    set => SetProperty(Value152Property, value);
  }


  public static readonly PropertyInfo<string> Value153Property = RegisterProperty<string>(nameof(Value153));
  public string Value153
  {
    get => GetProperty(Value153Property);
    set => SetProperty(Value153Property, value);
  }


  public static readonly PropertyInfo<string> Value154Property = RegisterProperty<string>(nameof(Value154));
  public string Value154
  {
    get => GetProperty(Value154Property);
    set => SetProperty(Value154Property, value);
  }


  public static readonly PropertyInfo<string> Value155Property = RegisterProperty<string>(nameof(Value155));
  public string Value155
  {
    get => GetProperty(Value155Property);
    set => SetProperty(Value155Property, value);
  }


  public static readonly PropertyInfo<string> Value156Property = RegisterProperty<string>(nameof(Value156));
  public string Value156
  {
    get => GetProperty(Value156Property);
    set => SetProperty(Value156Property, value);
  }


  public static readonly PropertyInfo<string> Value157Property = RegisterProperty<string>(nameof(Value157));
  public string Value157
  {
    get => GetProperty(Value157Property);
    set => SetProperty(Value157Property, value);
  }


  public static readonly PropertyInfo<string> Value158Property = RegisterProperty<string>(nameof(Value158));
  public string Value158
  {
    get => GetProperty(Value158Property);
    set => SetProperty(Value158Property, value);
  }


  public static readonly PropertyInfo<string> Value159Property = RegisterProperty<string>(nameof(Value159));
  public string Value159
  {
    get => GetProperty(Value159Property);
    set => SetProperty(Value159Property, value);
  }


  public static readonly PropertyInfo<string> Value160Property = RegisterProperty<string>(nameof(Value160));
  public string Value160
  {
    get => GetProperty(Value160Property);
    set => SetProperty(Value160Property, value);
  }


  public static readonly PropertyInfo<string> Value161Property = RegisterProperty<string>(nameof(Value161));
  public string Value161
  {
    get => GetProperty(Value161Property);
    set => SetProperty(Value161Property, value);
  }


  public static readonly PropertyInfo<string> Value162Property = RegisterProperty<string>(nameof(Value162));
  public string Value162
  {
    get => GetProperty(Value162Property);
    set => SetProperty(Value162Property, value);
  }


  public static readonly PropertyInfo<string> Value163Property = RegisterProperty<string>(nameof(Value163));
  public string Value163
  {
    get => GetProperty(Value163Property);
    set => SetProperty(Value163Property, value);
  }


  public static readonly PropertyInfo<string> Value164Property = RegisterProperty<string>(nameof(Value164));
  public string Value164
  {
    get => GetProperty(Value164Property);
    set => SetProperty(Value164Property, value);
  }


  public static readonly PropertyInfo<string> Value165Property = RegisterProperty<string>(nameof(Value165));
  public string Value165
  {
    get => GetProperty(Value165Property);
    set => SetProperty(Value165Property, value);
  }


  public static readonly PropertyInfo<string> Value166Property = RegisterProperty<string>(nameof(Value166));
  public string Value166
  {
    get => GetProperty(Value166Property);
    set => SetProperty(Value166Property, value);
  }


  public static readonly PropertyInfo<string> Value167Property = RegisterProperty<string>(nameof(Value167));
  public string Value167
  {
    get => GetProperty(Value167Property);
    set => SetProperty(Value167Property, value);
  }


  public static readonly PropertyInfo<string> Value168Property = RegisterProperty<string>(nameof(Value168));
  public string Value168
  {
    get => GetProperty(Value168Property);
    set => SetProperty(Value168Property, value);
  }


  public static readonly PropertyInfo<string> Value169Property = RegisterProperty<string>(nameof(Value169));
  public string Value169
  {
    get => GetProperty(Value169Property);
    set => SetProperty(Value169Property, value);
  }


  public static readonly PropertyInfo<string> Value170Property = RegisterProperty<string>(nameof(Value170));
  public string Value170
  {
    get => GetProperty(Value170Property);
    set => SetProperty(Value170Property, value);
  }


  public static readonly PropertyInfo<string> Value171Property = RegisterProperty<string>(nameof(Value171));
  public string Value171
  {
    get => GetProperty(Value171Property);
    set => SetProperty(Value171Property, value);
  }


  public static readonly PropertyInfo<string> Value172Property = RegisterProperty<string>(nameof(Value172));
  public string Value172
  {
    get => GetProperty(Value172Property);
    set => SetProperty(Value172Property, value);
  }


  public static readonly PropertyInfo<string> Value173Property = RegisterProperty<string>(nameof(Value173));
  public string Value173
  {
    get => GetProperty(Value173Property);
    set => SetProperty(Value173Property, value);
  }


  public static readonly PropertyInfo<string> Value174Property = RegisterProperty<string>(nameof(Value174));
  public string Value174
  {
    get => GetProperty(Value174Property);
    set => SetProperty(Value174Property, value);
  }


  public static readonly PropertyInfo<string> Value175Property = RegisterProperty<string>(nameof(Value175));
  public string Value175
  {
    get => GetProperty(Value175Property);
    set => SetProperty(Value175Property, value);
  }


  public static readonly PropertyInfo<string> Value176Property = RegisterProperty<string>(nameof(Value176));
  public string Value176
  {
    get => GetProperty(Value176Property);
    set => SetProperty(Value176Property, value);
  }


  public static readonly PropertyInfo<string> Value177Property = RegisterProperty<string>(nameof(Value177));
  public string Value177
  {
    get => GetProperty(Value177Property);
    set => SetProperty(Value177Property, value);
  }


  public static readonly PropertyInfo<string> Value178Property = RegisterProperty<string>(nameof(Value178));
  public string Value178
  {
    get => GetProperty(Value178Property);
    set => SetProperty(Value178Property, value);
  }


  public static readonly PropertyInfo<string> Value179Property = RegisterProperty<string>(nameof(Value179));
  public string Value179
  {
    get => GetProperty(Value179Property);
    set => SetProperty(Value179Property, value);
  }


  public static readonly PropertyInfo<string> Value180Property = RegisterProperty<string>(nameof(Value180));
  public string Value180
  {
    get => GetProperty(Value180Property);
    set => SetProperty(Value180Property, value);
  }


  public static readonly PropertyInfo<string> Value181Property = RegisterProperty<string>(nameof(Value181));
  public string Value181
  {
    get => GetProperty(Value181Property);
    set => SetProperty(Value181Property, value);
  }


  public static readonly PropertyInfo<string> Value182Property = RegisterProperty<string>(nameof(Value182));
  public string Value182
  {
    get => GetProperty(Value182Property);
    set => SetProperty(Value182Property, value);
  }


  public static readonly PropertyInfo<string> Value183Property = RegisterProperty<string>(nameof(Value183));
  public string Value183
  {
    get => GetProperty(Value183Property);
    set => SetProperty(Value183Property, value);
  }


  public static readonly PropertyInfo<string> Value184Property = RegisterProperty<string>(nameof(Value184));
  public string Value184
  {
    get => GetProperty(Value184Property);
    set => SetProperty(Value184Property, value);
  }


  public static readonly PropertyInfo<string> Value185Property = RegisterProperty<string>(nameof(Value185));
  public string Value185
  {
    get => GetProperty(Value185Property);
    set => SetProperty(Value185Property, value);
  }


  public static readonly PropertyInfo<string> Value186Property = RegisterProperty<string>(nameof(Value186));
  public string Value186
  {
    get => GetProperty(Value186Property);
    set => SetProperty(Value186Property, value);
  }


  public static readonly PropertyInfo<string> Value187Property = RegisterProperty<string>(nameof(Value187));
  public string Value187
  {
    get => GetProperty(Value187Property);
    set => SetProperty(Value187Property, value);
  }


  public static readonly PropertyInfo<string> Value188Property = RegisterProperty<string>(nameof(Value188));
  public string Value188
  {
    get => GetProperty(Value188Property);
    set => SetProperty(Value188Property, value);
  }


  public static readonly PropertyInfo<string> Value189Property = RegisterProperty<string>(nameof(Value189));
  public string Value189
  {
    get => GetProperty(Value189Property);
    set => SetProperty(Value189Property, value);
  }


  public static readonly PropertyInfo<string> Value190Property = RegisterProperty<string>(nameof(Value190));
  public string Value190
  {
    get => GetProperty(Value190Property);
    set => SetProperty(Value190Property, value);
  }


  public static readonly PropertyInfo<string> Value191Property = RegisterProperty<string>(nameof(Value191));
  public string Value191
  {
    get => GetProperty(Value191Property);
    set => SetProperty(Value191Property, value);
  }


  public static readonly PropertyInfo<string> Value192Property = RegisterProperty<string>(nameof(Value192));
  public string Value192
  {
    get => GetProperty(Value192Property);
    set => SetProperty(Value192Property, value);
  }


  public static readonly PropertyInfo<string> Value193Property = RegisterProperty<string>(nameof(Value193));
  public string Value193
  {
    get => GetProperty(Value193Property);
    set => SetProperty(Value193Property, value);
  }


  public static readonly PropertyInfo<string> Value194Property = RegisterProperty<string>(nameof(Value194));
  public string Value194
  {
    get => GetProperty(Value194Property);
    set => SetProperty(Value194Property, value);
  }


  public static readonly PropertyInfo<string> Value195Property = RegisterProperty<string>(nameof(Value195));
  public string Value195
  {
    get => GetProperty(Value195Property);
    set => SetProperty(Value195Property, value);
  }


  public static readonly PropertyInfo<string> Value196Property = RegisterProperty<string>(nameof(Value196));
  public string Value196
  {
    get => GetProperty(Value196Property);
    set => SetProperty(Value196Property, value);
  }


  public static readonly PropertyInfo<string> Value197Property = RegisterProperty<string>(nameof(Value197));
  public string Value197
  {
    get => GetProperty(Value197Property);
    set => SetProperty(Value197Property, value);
  }


  public static readonly PropertyInfo<string> Value198Property = RegisterProperty<string>(nameof(Value198));
  public string Value198
  {
    get => GetProperty(Value198Property);
    set => SetProperty(Value198Property, value);
  }


  public static readonly PropertyInfo<string> Value199Property = RegisterProperty<string>(nameof(Value199));
  public string Value199
  {
    get => GetProperty(Value199Property);
    set => SetProperty(Value199Property, value);
  }


  public static readonly PropertyInfo<string> Value200Property = RegisterProperty<string>(nameof(Value200));
  public string Value200
  {
    get => GetProperty(Value200Property);
    set => SetProperty(Value200Property, value);
  }


  public static readonly PropertyInfo<string> Value201Property = RegisterProperty<string>(nameof(Value201));
  public string Value201
  {
    get => GetProperty(Value201Property);
    set => SetProperty(Value201Property, value);
  }


  public static readonly PropertyInfo<string> Value202Property = RegisterProperty<string>(nameof(Value202));
  public string Value202
  {
    get => GetProperty(Value202Property);
    set => SetProperty(Value202Property, value);
  }


  public static readonly PropertyInfo<string> Value203Property = RegisterProperty<string>(nameof(Value203));
  public string Value203
  {
    get => GetProperty(Value203Property);
    set => SetProperty(Value203Property, value);
  }


  public static readonly PropertyInfo<string> Value204Property = RegisterProperty<string>(nameof(Value204));
  public string Value204
  {
    get => GetProperty(Value204Property);
    set => SetProperty(Value204Property, value);
  }


  public static readonly PropertyInfo<string> Value205Property = RegisterProperty<string>(nameof(Value205));
  public string Value205
  {
    get => GetProperty(Value205Property);
    set => SetProperty(Value205Property, value);
  }


  public static readonly PropertyInfo<string> Value206Property = RegisterProperty<string>(nameof(Value206));
  public string Value206
  {
    get => GetProperty(Value206Property);
    set => SetProperty(Value206Property, value);
  }


  public static readonly PropertyInfo<string> Value207Property = RegisterProperty<string>(nameof(Value207));
  public string Value207
  {
    get => GetProperty(Value207Property);
    set => SetProperty(Value207Property, value);
  }


  public static readonly PropertyInfo<string> Value208Property = RegisterProperty<string>(nameof(Value208));
  public string Value208
  {
    get => GetProperty(Value208Property);
    set => SetProperty(Value208Property, value);
  }


  public static readonly PropertyInfo<string> Value209Property = RegisterProperty<string>(nameof(Value209));
  public string Value209
  {
    get => GetProperty(Value209Property);
    set => SetProperty(Value209Property, value);
  }


  public static readonly PropertyInfo<string> Value210Property = RegisterProperty<string>(nameof(Value210));
  public string Value210
  {
    get => GetProperty(Value210Property);
    set => SetProperty(Value210Property, value);
  }


  public static readonly PropertyInfo<string> Value211Property = RegisterProperty<string>(nameof(Value211));
  public string Value211
  {
    get => GetProperty(Value211Property);
    set => SetProperty(Value211Property, value);
  }


  public static readonly PropertyInfo<string> Value212Property = RegisterProperty<string>(nameof(Value212));
  public string Value212
  {
    get => GetProperty(Value212Property);
    set => SetProperty(Value212Property, value);
  }


  public static readonly PropertyInfo<string> Value213Property = RegisterProperty<string>(nameof(Value213));
  public string Value213
  {
    get => GetProperty(Value213Property);
    set => SetProperty(Value213Property, value);
  }


  public static readonly PropertyInfo<string> Value214Property = RegisterProperty<string>(nameof(Value214));
  public string Value214
  {
    get => GetProperty(Value214Property);
    set => SetProperty(Value214Property, value);
  }


  public static readonly PropertyInfo<string> Value215Property = RegisterProperty<string>(nameof(Value215));
  public string Value215
  {
    get => GetProperty(Value215Property);
    set => SetProperty(Value215Property, value);
  }


  public static readonly PropertyInfo<string> Value216Property = RegisterProperty<string>(nameof(Value216));
  public string Value216
  {
    get => GetProperty(Value216Property);
    set => SetProperty(Value216Property, value);
  }


  public static readonly PropertyInfo<string> Value217Property = RegisterProperty<string>(nameof(Value217));
  public string Value217
  {
    get => GetProperty(Value217Property);
    set => SetProperty(Value217Property, value);
  }


  public static readonly PropertyInfo<string> Value218Property = RegisterProperty<string>(nameof(Value218));
  public string Value218
  {
    get => GetProperty(Value218Property);
    set => SetProperty(Value218Property, value);
  }


  public static readonly PropertyInfo<string> Value219Property = RegisterProperty<string>(nameof(Value219));
  public string Value219
  {
    get => GetProperty(Value219Property);
    set => SetProperty(Value219Property, value);
  }


  public static readonly PropertyInfo<string> Value220Property = RegisterProperty<string>(nameof(Value220));
  public string Value220
  {
    get => GetProperty(Value220Property);
    set => SetProperty(Value220Property, value);
  }


  public static readonly PropertyInfo<string> Value221Property = RegisterProperty<string>(nameof(Value221));
  public string Value221
  {
    get => GetProperty(Value221Property);
    set => SetProperty(Value221Property, value);
  }


  public static readonly PropertyInfo<string> Value222Property = RegisterProperty<string>(nameof(Value222));
  public string Value222
  {
    get => GetProperty(Value222Property);
    set => SetProperty(Value222Property, value);
  }


  public static readonly PropertyInfo<string> Value223Property = RegisterProperty<string>(nameof(Value223));
  public string Value223
  {
    get => GetProperty(Value223Property);
    set => SetProperty(Value223Property, value);
  }


  public static readonly PropertyInfo<string> Value224Property = RegisterProperty<string>(nameof(Value224));
  public string Value224
  {
    get => GetProperty(Value224Property);
    set => SetProperty(Value224Property, value);
  }


  public static readonly PropertyInfo<string> Value225Property = RegisterProperty<string>(nameof(Value225));
  public string Value225
  {
    get => GetProperty(Value225Property);
    set => SetProperty(Value225Property, value);
  }


  public static readonly PropertyInfo<string> Value226Property = RegisterProperty<string>(nameof(Value226));
  public string Value226
  {
    get => GetProperty(Value226Property);
    set => SetProperty(Value226Property, value);
  }


  public static readonly PropertyInfo<string> Value227Property = RegisterProperty<string>(nameof(Value227));
  public string Value227
  {
    get => GetProperty(Value227Property);
    set => SetProperty(Value227Property, value);
  }


  public static readonly PropertyInfo<string> Value228Property = RegisterProperty<string>(nameof(Value228));
  public string Value228
  {
    get => GetProperty(Value228Property);
    set => SetProperty(Value228Property, value);
  }


  public static readonly PropertyInfo<string> Value229Property = RegisterProperty<string>(nameof(Value229));
  public string Value229
  {
    get => GetProperty(Value229Property);
    set => SetProperty(Value229Property, value);
  }


  public static readonly PropertyInfo<string> Value230Property = RegisterProperty<string>(nameof(Value230));
  public string Value230
  {
    get => GetProperty(Value230Property);
    set => SetProperty(Value230Property, value);
  }


  public static readonly PropertyInfo<string> Value231Property = RegisterProperty<string>(nameof(Value231));
  public string Value231
  {
    get => GetProperty(Value231Property);
    set => SetProperty(Value231Property, value);
  }


  public static readonly PropertyInfo<string> Value232Property = RegisterProperty<string>(nameof(Value232));
  public string Value232
  {
    get => GetProperty(Value232Property);
    set => SetProperty(Value232Property, value);
  }


  public static readonly PropertyInfo<string> Value233Property = RegisterProperty<string>(nameof(Value233));
  public string Value233
  {
    get => GetProperty(Value233Property);
    set => SetProperty(Value233Property, value);
  }


  public static readonly PropertyInfo<string> Value234Property = RegisterProperty<string>(nameof(Value234));
  public string Value234
  {
    get => GetProperty(Value234Property);
    set => SetProperty(Value234Property, value);
  }


  public static readonly PropertyInfo<string> Value235Property = RegisterProperty<string>(nameof(Value235));
  public string Value235
  {
    get => GetProperty(Value235Property);
    set => SetProperty(Value235Property, value);
  }


  public static readonly PropertyInfo<string> Value236Property = RegisterProperty<string>(nameof(Value236));
  public string Value236
  {
    get => GetProperty(Value236Property);
    set => SetProperty(Value236Property, value);
  }


  public static readonly PropertyInfo<string> Value237Property = RegisterProperty<string>(nameof(Value237));
  public string Value237
  {
    get => GetProperty(Value237Property);
    set => SetProperty(Value237Property, value);
  }


  public static readonly PropertyInfo<string> Value238Property = RegisterProperty<string>(nameof(Value238));
  public string Value238
  {
    get => GetProperty(Value238Property);
    set => SetProperty(Value238Property, value);
  }


  public static readonly PropertyInfo<string> Value239Property = RegisterProperty<string>(nameof(Value239));
  public string Value239
  {
    get => GetProperty(Value239Property);
    set => SetProperty(Value239Property, value);
  }


  public static readonly PropertyInfo<string> Value240Property = RegisterProperty<string>(nameof(Value240));
  public string Value240
  {
    get => GetProperty(Value240Property);
    set => SetProperty(Value240Property, value);
  }


  public static readonly PropertyInfo<string> Value241Property = RegisterProperty<string>(nameof(Value241));
  public string Value241
  {
    get => GetProperty(Value241Property);
    set => SetProperty(Value241Property, value);
  }


  public static readonly PropertyInfo<string> Value242Property = RegisterProperty<string>(nameof(Value242));
  public string Value242
  {
    get => GetProperty(Value242Property);
    set => SetProperty(Value242Property, value);
  }


  public static readonly PropertyInfo<string> Value243Property = RegisterProperty<string>(nameof(Value243));
  public string Value243
  {
    get => GetProperty(Value243Property);
    set => SetProperty(Value243Property, value);
  }


  public static readonly PropertyInfo<string> Value244Property = RegisterProperty<string>(nameof(Value244));
  public string Value244
  {
    get => GetProperty(Value244Property);
    set => SetProperty(Value244Property, value);
  }


  public static readonly PropertyInfo<string> Value245Property = RegisterProperty<string>(nameof(Value245));
  public string Value245
  {
    get => GetProperty(Value245Property);
    set => SetProperty(Value245Property, value);
  }


  public static readonly PropertyInfo<string> Value246Property = RegisterProperty<string>(nameof(Value246));
  public string Value246
  {
    get => GetProperty(Value246Property);
    set => SetProperty(Value246Property, value);
  }


  public static readonly PropertyInfo<string> Value247Property = RegisterProperty<string>(nameof(Value247));
  public string Value247
  {
    get => GetProperty(Value247Property);
    set => SetProperty(Value247Property, value);
  }


  public static readonly PropertyInfo<string> Value248Property = RegisterProperty<string>(nameof(Value248));
  public string Value248
  {
    get => GetProperty(Value248Property);
    set => SetProperty(Value248Property, value);
  }


  public static readonly PropertyInfo<string> Value249Property = RegisterProperty<string>(nameof(Value249));
  public string Value249
  {
    get => GetProperty(Value249Property);
    set => SetProperty(Value249Property, value);
  }


  public static readonly PropertyInfo<string> Value250Property = RegisterProperty<string>(nameof(Value250));
  public string Value250
  {
    get => GetProperty(Value250Property);
    set => SetProperty(Value250Property, value);
  }


  public static readonly PropertyInfo<string> Value251Property = RegisterProperty<string>(nameof(Value251));
  public string Value251
  {
    get => GetProperty(Value251Property);
    set => SetProperty(Value251Property, value);
  }


  public static readonly PropertyInfo<string> Value252Property = RegisterProperty<string>(nameof(Value252));
  public string Value252
  {
    get => GetProperty(Value252Property);
    set => SetProperty(Value252Property, value);
  }


  public static readonly PropertyInfo<string> Value253Property = RegisterProperty<string>(nameof(Value253));
  public string Value253
  {
    get => GetProperty(Value253Property);
    set => SetProperty(Value253Property, value);
  }


  public static readonly PropertyInfo<string> Value254Property = RegisterProperty<string>(nameof(Value254));
  public string Value254
  {
    get => GetProperty(Value254Property);
    set => SetProperty(Value254Property, value);
  }


  public static readonly PropertyInfo<string> Value255Property = RegisterProperty<string>(nameof(Value255));
  public string Value255
  {
    get => GetProperty(Value255Property);
    set => SetProperty(Value255Property, value);
  }


  public static readonly PropertyInfo<string> Value256Property = RegisterProperty<string>(nameof(Value256));
  public string Value256
  {
    get => GetProperty(Value256Property);
    set => SetProperty(Value256Property, value);
  }


  public static readonly PropertyInfo<string> Value257Property = RegisterProperty<string>(nameof(Value257));
  public string Value257
  {
    get => GetProperty(Value257Property);
    set => SetProperty(Value257Property, value);
  }


  public static readonly PropertyInfo<string> Value258Property = RegisterProperty<string>(nameof(Value258));
  public string Value258
  {
    get => GetProperty(Value258Property);
    set => SetProperty(Value258Property, value);
  }


  public static readonly PropertyInfo<string> Value259Property = RegisterProperty<string>(nameof(Value259));
  public string Value259
  {
    get => GetProperty(Value259Property);
    set => SetProperty(Value259Property, value);
  }


  public static readonly PropertyInfo<string> Value260Property = RegisterProperty<string>(nameof(Value260));
  public string Value260
  {
    get => GetProperty(Value260Property);
    set => SetProperty(Value260Property, value);
  }


  public static readonly PropertyInfo<string> Value261Property = RegisterProperty<string>(nameof(Value261));
  public string Value261
  {
    get => GetProperty(Value261Property);
    set => SetProperty(Value261Property, value);
  }


  public static readonly PropertyInfo<string> Value262Property = RegisterProperty<string>(nameof(Value262));
  public string Value262
  {
    get => GetProperty(Value262Property);
    set => SetProperty(Value262Property, value);
  }


  public static readonly PropertyInfo<string> Value263Property = RegisterProperty<string>(nameof(Value263));
  public string Value263
  {
    get => GetProperty(Value263Property);
    set => SetProperty(Value263Property, value);
  }


  public static readonly PropertyInfo<string> Value264Property = RegisterProperty<string>(nameof(Value264));
  public string Value264
  {
    get => GetProperty(Value264Property);
    set => SetProperty(Value264Property, value);
  }


  public static readonly PropertyInfo<string> Value265Property = RegisterProperty<string>(nameof(Value265));
  public string Value265
  {
    get => GetProperty(Value265Property);
    set => SetProperty(Value265Property, value);
  }


  public static readonly PropertyInfo<string> Value266Property = RegisterProperty<string>(nameof(Value266));
  public string Value266
  {
    get => GetProperty(Value266Property);
    set => SetProperty(Value266Property, value);
  }


  public static readonly PropertyInfo<string> Value267Property = RegisterProperty<string>(nameof(Value267));
  public string Value267
  {
    get => GetProperty(Value267Property);
    set => SetProperty(Value267Property, value);
  }


  public static readonly PropertyInfo<string> Value268Property = RegisterProperty<string>(nameof(Value268));
  public string Value268
  {
    get => GetProperty(Value268Property);
    set => SetProperty(Value268Property, value);
  }


  public static readonly PropertyInfo<string> Value269Property = RegisterProperty<string>(nameof(Value269));
  public string Value269
  {
    get => GetProperty(Value269Property);
    set => SetProperty(Value269Property, value);
  }


  public static readonly PropertyInfo<string> Value270Property = RegisterProperty<string>(nameof(Value270));
  public string Value270
  {
    get => GetProperty(Value270Property);
    set => SetProperty(Value270Property, value);
  }


  public static readonly PropertyInfo<string> Value271Property = RegisterProperty<string>(nameof(Value271));
  public string Value271
  {
    get => GetProperty(Value271Property);
    set => SetProperty(Value271Property, value);
  }


  public static readonly PropertyInfo<string> Value272Property = RegisterProperty<string>(nameof(Value272));
  public string Value272
  {
    get => GetProperty(Value272Property);
    set => SetProperty(Value272Property, value);
  }


  public static readonly PropertyInfo<string> Value273Property = RegisterProperty<string>(nameof(Value273));
  public string Value273
  {
    get => GetProperty(Value273Property);
    set => SetProperty(Value273Property, value);
  }


  public static readonly PropertyInfo<string> Value274Property = RegisterProperty<string>(nameof(Value274));
  public string Value274
  {
    get => GetProperty(Value274Property);
    set => SetProperty(Value274Property, value);
  }


  public static readonly PropertyInfo<string> Value275Property = RegisterProperty<string>(nameof(Value275));
  public string Value275
  {
    get => GetProperty(Value275Property);
    set => SetProperty(Value275Property, value);
  }


  public static readonly PropertyInfo<string> Value276Property = RegisterProperty<string>(nameof(Value276));
  public string Value276
  {
    get => GetProperty(Value276Property);
    set => SetProperty(Value276Property, value);
  }


  public static readonly PropertyInfo<string> Value277Property = RegisterProperty<string>(nameof(Value277));
  public string Value277
  {
    get => GetProperty(Value277Property);
    set => SetProperty(Value277Property, value);
  }


  public static readonly PropertyInfo<string> Value278Property = RegisterProperty<string>(nameof(Value278));
  public string Value278
  {
    get => GetProperty(Value278Property);
    set => SetProperty(Value278Property, value);
  }


  public static readonly PropertyInfo<string> Value279Property = RegisterProperty<string>(nameof(Value279));
  public string Value279
  {
    get => GetProperty(Value279Property);
    set => SetProperty(Value279Property, value);
  }


  public static readonly PropertyInfo<string> Value280Property = RegisterProperty<string>(nameof(Value280));
  public string Value280
  {
    get => GetProperty(Value280Property);
    set => SetProperty(Value280Property, value);
  }


  public static readonly PropertyInfo<string> Value281Property = RegisterProperty<string>(nameof(Value281));
  public string Value281
  {
    get => GetProperty(Value281Property);
    set => SetProperty(Value281Property, value);
  }


  public static readonly PropertyInfo<string> Value282Property = RegisterProperty<string>(nameof(Value282));
  public string Value282
  {
    get => GetProperty(Value282Property);
    set => SetProperty(Value282Property, value);
  }


  public static readonly PropertyInfo<string> Value283Property = RegisterProperty<string>(nameof(Value283));
  public string Value283
  {
    get => GetProperty(Value283Property);
    set => SetProperty(Value283Property, value);
  }


  public static readonly PropertyInfo<string> Value284Property = RegisterProperty<string>(nameof(Value284));
  public string Value284
  {
    get => GetProperty(Value284Property);
    set => SetProperty(Value284Property, value);
  }


  public static readonly PropertyInfo<string> Value285Property = RegisterProperty<string>(nameof(Value285));
  public string Value285
  {
    get => GetProperty(Value285Property);
    set => SetProperty(Value285Property, value);
  }


  public static readonly PropertyInfo<string> Value286Property = RegisterProperty<string>(nameof(Value286));
  public string Value286
  {
    get => GetProperty(Value286Property);
    set => SetProperty(Value286Property, value);
  }


  public static readonly PropertyInfo<string> Value287Property = RegisterProperty<string>(nameof(Value287));
  public string Value287
  {
    get => GetProperty(Value287Property);
    set => SetProperty(Value287Property, value);
  }


  public static readonly PropertyInfo<string> Value288Property = RegisterProperty<string>(nameof(Value288));
  public string Value288
  {
    get => GetProperty(Value288Property);
    set => SetProperty(Value288Property, value);
  }


  public static readonly PropertyInfo<string> Value289Property = RegisterProperty<string>(nameof(Value289));
  public string Value289
  {
    get => GetProperty(Value289Property);
    set => SetProperty(Value289Property, value);
  }


  public static readonly PropertyInfo<string> Value290Property = RegisterProperty<string>(nameof(Value290));
  public string Value290
  {
    get => GetProperty(Value290Property);
    set => SetProperty(Value290Property, value);
  }


  public static readonly PropertyInfo<string> Value291Property = RegisterProperty<string>(nameof(Value291));
  public string Value291
  {
    get => GetProperty(Value291Property);
    set => SetProperty(Value291Property, value);
  }


  public static readonly PropertyInfo<string> Value292Property = RegisterProperty<string>(nameof(Value292));
  public string Value292
  {
    get => GetProperty(Value292Property);
    set => SetProperty(Value292Property, value);
  }


  public static readonly PropertyInfo<string> Value293Property = RegisterProperty<string>(nameof(Value293));
  public string Value293
  {
    get => GetProperty(Value293Property);
    set => SetProperty(Value293Property, value);
  }


  public static readonly PropertyInfo<string> Value294Property = RegisterProperty<string>(nameof(Value294));
  public string Value294
  {
    get => GetProperty(Value294Property);
    set => SetProperty(Value294Property, value);
  }


  public static readonly PropertyInfo<string> Value295Property = RegisterProperty<string>(nameof(Value295));
  public string Value295
  {
    get => GetProperty(Value295Property);
    set => SetProperty(Value295Property, value);
  }


  public static readonly PropertyInfo<string> Value296Property = RegisterProperty<string>(nameof(Value296));
  public string Value296
  {
    get => GetProperty(Value296Property);
    set => SetProperty(Value296Property, value);
  }


  public static readonly PropertyInfo<string> Value297Property = RegisterProperty<string>(nameof(Value297));
  public string Value297
  {
    get => GetProperty(Value297Property);
    set => SetProperty(Value297Property, value);
  }


  public static readonly PropertyInfo<string> Value298Property = RegisterProperty<string>(nameof(Value298));
  public string Value298
  {
    get => GetProperty(Value298Property);
    set => SetProperty(Value298Property, value);
  }


  public static readonly PropertyInfo<string> Value299Property = RegisterProperty<string>(nameof(Value299));
  public string Value299
  {
    get => GetProperty(Value299Property);
    set => SetProperty(Value299Property, value);
  }


  public static readonly PropertyInfo<string> Value300Property = RegisterProperty<string>(nameof(Value300));
  public string Value300
  {
    get => GetProperty(Value300Property);
    set => SetProperty(Value300Property, value);
  }


  public static readonly PropertyInfo<string> Value301Property = RegisterProperty<string>(nameof(Value301));
  public string Value301
  {
    get => GetProperty(Value301Property);
    set => SetProperty(Value301Property, value);
  }


  public static readonly PropertyInfo<string> Value302Property = RegisterProperty<string>(nameof(Value302));
  public string Value302
  {
    get => GetProperty(Value302Property);
    set => SetProperty(Value302Property, value);
  }


  public static readonly PropertyInfo<string> Value303Property = RegisterProperty<string>(nameof(Value303));
  public string Value303
  {
    get => GetProperty(Value303Property);
    set => SetProperty(Value303Property, value);
  }


  public static readonly PropertyInfo<string> Value304Property = RegisterProperty<string>(nameof(Value304));
  public string Value304
  {
    get => GetProperty(Value304Property);
    set => SetProperty(Value304Property, value);
  }


  public static readonly PropertyInfo<string> Value305Property = RegisterProperty<string>(nameof(Value305));
  public string Value305
  {
    get => GetProperty(Value305Property);
    set => SetProperty(Value305Property, value);
  }


  public static readonly PropertyInfo<string> Value306Property = RegisterProperty<string>(nameof(Value306));
  public string Value306
  {
    get => GetProperty(Value306Property);
    set => SetProperty(Value306Property, value);
  }


  public static readonly PropertyInfo<string> Value307Property = RegisterProperty<string>(nameof(Value307));
  public string Value307
  {
    get => GetProperty(Value307Property);
    set => SetProperty(Value307Property, value);
  }


  public static readonly PropertyInfo<string> Value308Property = RegisterProperty<string>(nameof(Value308));
  public string Value308
  {
    get => GetProperty(Value308Property);
    set => SetProperty(Value308Property, value);
  }


  public static readonly PropertyInfo<string> Value309Property = RegisterProperty<string>(nameof(Value309));
  public string Value309
  {
    get => GetProperty(Value309Property);
    set => SetProperty(Value309Property, value);
  }


  public static readonly PropertyInfo<string> Value310Property = RegisterProperty<string>(nameof(Value310));
  public string Value310
  {
    get => GetProperty(Value310Property);
    set => SetProperty(Value310Property, value);
  }


  public static readonly PropertyInfo<string> Value311Property = RegisterProperty<string>(nameof(Value311));
  public string Value311
  {
    get => GetProperty(Value311Property);
    set => SetProperty(Value311Property, value);
  }


  public static readonly PropertyInfo<string> Value312Property = RegisterProperty<string>(nameof(Value312));
  public string Value312
  {
    get => GetProperty(Value312Property);
    set => SetProperty(Value312Property, value);
  }


  public static readonly PropertyInfo<string> Value313Property = RegisterProperty<string>(nameof(Value313));
  public string Value313
  {
    get => GetProperty(Value313Property);
    set => SetProperty(Value313Property, value);
  }


  public static readonly PropertyInfo<string> Value314Property = RegisterProperty<string>(nameof(Value314));
  public string Value314
  {
    get => GetProperty(Value314Property);
    set => SetProperty(Value314Property, value);
  }


  public static readonly PropertyInfo<string> Value315Property = RegisterProperty<string>(nameof(Value315));
  public string Value315
  {
    get => GetProperty(Value315Property);
    set => SetProperty(Value315Property, value);
  }


  public static readonly PropertyInfo<string> Value316Property = RegisterProperty<string>(nameof(Value316));
  public string Value316
  {
    get => GetProperty(Value316Property);
    set => SetProperty(Value316Property, value);
  }


  public static readonly PropertyInfo<string> Value317Property = RegisterProperty<string>(nameof(Value317));
  public string Value317
  {
    get => GetProperty(Value317Property);
    set => SetProperty(Value317Property, value);
  }


  public static readonly PropertyInfo<string> Value318Property = RegisterProperty<string>(nameof(Value318));
  public string Value318
  {
    get => GetProperty(Value318Property);
    set => SetProperty(Value318Property, value);
  }


  public static readonly PropertyInfo<string> Value319Property = RegisterProperty<string>(nameof(Value319));
  public string Value319
  {
    get => GetProperty(Value319Property);
    set => SetProperty(Value319Property, value);
  }


  public static readonly PropertyInfo<string> Value320Property = RegisterProperty<string>(nameof(Value320));
  public string Value320
  {
    get => GetProperty(Value320Property);
    set => SetProperty(Value320Property, value);
  }


  public static readonly PropertyInfo<string> Value321Property = RegisterProperty<string>(nameof(Value321));
  public string Value321
  {
    get => GetProperty(Value321Property);
    set => SetProperty(Value321Property, value);
  }


  public static readonly PropertyInfo<string> Value322Property = RegisterProperty<string>(nameof(Value322));
  public string Value322
  {
    get => GetProperty(Value322Property);
    set => SetProperty(Value322Property, value);
  }


  public static readonly PropertyInfo<string> Value323Property = RegisterProperty<string>(nameof(Value323));
  public string Value323
  {
    get => GetProperty(Value323Property);
    set => SetProperty(Value323Property, value);
  }


  public static readonly PropertyInfo<string> Value324Property = RegisterProperty<string>(nameof(Value324));
  public string Value324
  {
    get => GetProperty(Value324Property);
    set => SetProperty(Value324Property, value);
  }


  public static readonly PropertyInfo<string> Value325Property = RegisterProperty<string>(nameof(Value325));
  public string Value325
  {
    get => GetProperty(Value325Property);
    set => SetProperty(Value325Property, value);
  }


  public static readonly PropertyInfo<string> Value326Property = RegisterProperty<string>(nameof(Value326));
  public string Value326
  {
    get => GetProperty(Value326Property);
    set => SetProperty(Value326Property, value);
  }


  public static readonly PropertyInfo<string> Value327Property = RegisterProperty<string>(nameof(Value327));
  public string Value327
  {
    get => GetProperty(Value327Property);
    set => SetProperty(Value327Property, value);
  }


  public static readonly PropertyInfo<string> Value328Property = RegisterProperty<string>(nameof(Value328));
  public string Value328
  {
    get => GetProperty(Value328Property);
    set => SetProperty(Value328Property, value);
  }


  public static readonly PropertyInfo<string> Value329Property = RegisterProperty<string>(nameof(Value329));
  public string Value329
  {
    get => GetProperty(Value329Property);
    set => SetProperty(Value329Property, value);
  }


  public static readonly PropertyInfo<string> Value330Property = RegisterProperty<string>(nameof(Value330));
  public string Value330
  {
    get => GetProperty(Value330Property);
    set => SetProperty(Value330Property, value);
  }


  public static readonly PropertyInfo<string> Value331Property = RegisterProperty<string>(nameof(Value331));
  public string Value331
  {
    get => GetProperty(Value331Property);
    set => SetProperty(Value331Property, value);
  }


  public static readonly PropertyInfo<string> Value332Property = RegisterProperty<string>(nameof(Value332));
  public string Value332
  {
    get => GetProperty(Value332Property);
    set => SetProperty(Value332Property, value);
  }


  public static readonly PropertyInfo<string> Value333Property = RegisterProperty<string>(nameof(Value333));
  public string Value333
  {
    get => GetProperty(Value333Property);
    set => SetProperty(Value333Property, value);
  }


  public static readonly PropertyInfo<string> Value334Property = RegisterProperty<string>(nameof(Value334));
  public string Value334
  {
    get => GetProperty(Value334Property);
    set => SetProperty(Value334Property, value);
  }


  public static readonly PropertyInfo<string> Value335Property = RegisterProperty<string>(nameof(Value335));
  public string Value335
  {
    get => GetProperty(Value335Property);
    set => SetProperty(Value335Property, value);
  }


  public static readonly PropertyInfo<string> Value336Property = RegisterProperty<string>(nameof(Value336));
  public string Value336
  {
    get => GetProperty(Value336Property);
    set => SetProperty(Value336Property, value);
  }


  public static readonly PropertyInfo<string> Value337Property = RegisterProperty<string>(nameof(Value337));
  public string Value337
  {
    get => GetProperty(Value337Property);
    set => SetProperty(Value337Property, value);
  }


  public static readonly PropertyInfo<string> Value338Property = RegisterProperty<string>(nameof(Value338));
  public string Value338
  {
    get => GetProperty(Value338Property);
    set => SetProperty(Value338Property, value);
  }


  public static readonly PropertyInfo<string> Value339Property = RegisterProperty<string>(nameof(Value339));
  public string Value339
  {
    get => GetProperty(Value339Property);
    set => SetProperty(Value339Property, value);
  }


  public static readonly PropertyInfo<string> Value340Property = RegisterProperty<string>(nameof(Value340));
  public string Value340
  {
    get => GetProperty(Value340Property);
    set => SetProperty(Value340Property, value);
  }


  public static readonly PropertyInfo<string> Value341Property = RegisterProperty<string>(nameof(Value341));
  public string Value341
  {
    get => GetProperty(Value341Property);
    set => SetProperty(Value341Property, value);
  }


  public static readonly PropertyInfo<string> Value342Property = RegisterProperty<string>(nameof(Value342));
  public string Value342
  {
    get => GetProperty(Value342Property);
    set => SetProperty(Value342Property, value);
  }


  public static readonly PropertyInfo<string> Value343Property = RegisterProperty<string>(nameof(Value343));
  public string Value343
  {
    get => GetProperty(Value343Property);
    set => SetProperty(Value343Property, value);
  }


  public static readonly PropertyInfo<string> Value344Property = RegisterProperty<string>(nameof(Value344));
  public string Value344
  {
    get => GetProperty(Value344Property);
    set => SetProperty(Value344Property, value);
  }


  public static readonly PropertyInfo<string> Value345Property = RegisterProperty<string>(nameof(Value345));
  public string Value345
  {
    get => GetProperty(Value345Property);
    set => SetProperty(Value345Property, value);
  }


  public static readonly PropertyInfo<string> Value346Property = RegisterProperty<string>(nameof(Value346));
  public string Value346
  {
    get => GetProperty(Value346Property);
    set => SetProperty(Value346Property, value);
  }


  public static readonly PropertyInfo<string> Value347Property = RegisterProperty<string>(nameof(Value347));
  public string Value347
  {
    get => GetProperty(Value347Property);
    set => SetProperty(Value347Property, value);
  }


  public static readonly PropertyInfo<string> Value348Property = RegisterProperty<string>(nameof(Value348));
  public string Value348
  {
    get => GetProperty(Value348Property);
    set => SetProperty(Value348Property, value);
  }


  public static readonly PropertyInfo<string> Value349Property = RegisterProperty<string>(nameof(Value349));
  public string Value349
  {
    get => GetProperty(Value349Property);
    set => SetProperty(Value349Property, value);
  }


  public static readonly PropertyInfo<string> Value350Property = RegisterProperty<string>(nameof(Value350));
  public string Value350
  {
    get => GetProperty(Value350Property);
    set => SetProperty(Value350Property, value);
  }


  public static readonly PropertyInfo<string> Value351Property = RegisterProperty<string>(nameof(Value351));
  public string Value351
  {
    get => GetProperty(Value351Property);
    set => SetProperty(Value351Property, value);
  }


  public static readonly PropertyInfo<string> Value352Property = RegisterProperty<string>(nameof(Value352));
  public string Value352
  {
    get => GetProperty(Value352Property);
    set => SetProperty(Value352Property, value);
  }


  public static readonly PropertyInfo<string> Value353Property = RegisterProperty<string>(nameof(Value353));
  public string Value353
  {
    get => GetProperty(Value353Property);
    set => SetProperty(Value353Property, value);
  }


  public static readonly PropertyInfo<string> Value354Property = RegisterProperty<string>(nameof(Value354));
  public string Value354
  {
    get => GetProperty(Value354Property);
    set => SetProperty(Value354Property, value);
  }


  public static readonly PropertyInfo<string> Value355Property = RegisterProperty<string>(nameof(Value355));
  public string Value355
  {
    get => GetProperty(Value355Property);
    set => SetProperty(Value355Property, value);
  }


  public static readonly PropertyInfo<string> Value356Property = RegisterProperty<string>(nameof(Value356));
  public string Value356
  {
    get => GetProperty(Value356Property);
    set => SetProperty(Value356Property, value);
  }


  public static readonly PropertyInfo<string> Value357Property = RegisterProperty<string>(nameof(Value357));
  public string Value357
  {
    get => GetProperty(Value357Property);
    set => SetProperty(Value357Property, value);
  }


  public static readonly PropertyInfo<string> Value358Property = RegisterProperty<string>(nameof(Value358));
  public string Value358
  {
    get => GetProperty(Value358Property);
    set => SetProperty(Value358Property, value);
  }


  public static readonly PropertyInfo<string> Value359Property = RegisterProperty<string>(nameof(Value359));
  public string Value359
  {
    get => GetProperty(Value359Property);
    set => SetProperty(Value359Property, value);
  }


  public static readonly PropertyInfo<string> Value360Property = RegisterProperty<string>(nameof(Value360));
  public string Value360
  {
    get => GetProperty(Value360Property);
    set => SetProperty(Value360Property, value);
  }


  public static readonly PropertyInfo<string> Value361Property = RegisterProperty<string>(nameof(Value361));
  public string Value361
  {
    get => GetProperty(Value361Property);
    set => SetProperty(Value361Property, value);
  }


  public static readonly PropertyInfo<string> Value362Property = RegisterProperty<string>(nameof(Value362));
  public string Value362
  {
    get => GetProperty(Value362Property);
    set => SetProperty(Value362Property, value);
  }


  public static readonly PropertyInfo<string> Value363Property = RegisterProperty<string>(nameof(Value363));
  public string Value363
  {
    get => GetProperty(Value363Property);
    set => SetProperty(Value363Property, value);
  }


  public static readonly PropertyInfo<string> Value364Property = RegisterProperty<string>(nameof(Value364));
  public string Value364
  {
    get => GetProperty(Value364Property);
    set => SetProperty(Value364Property, value);
  }


  public static readonly PropertyInfo<string> Value365Property = RegisterProperty<string>(nameof(Value365));
  public string Value365
  {
    get => GetProperty(Value365Property);
    set => SetProperty(Value365Property, value);
  }


  public static readonly PropertyInfo<string> Value366Property = RegisterProperty<string>(nameof(Value366));
  public string Value366
  {
    get => GetProperty(Value366Property);
    set => SetProperty(Value366Property, value);
  }


  public static readonly PropertyInfo<string> Value367Property = RegisterProperty<string>(nameof(Value367));
  public string Value367
  {
    get => GetProperty(Value367Property);
    set => SetProperty(Value367Property, value);
  }


  public static readonly PropertyInfo<string> Value368Property = RegisterProperty<string>(nameof(Value368));
  public string Value368
  {
    get => GetProperty(Value368Property);
    set => SetProperty(Value368Property, value);
  }


  public static readonly PropertyInfo<string> Value369Property = RegisterProperty<string>(nameof(Value369));
  public string Value369
  {
    get => GetProperty(Value369Property);
    set => SetProperty(Value369Property, value);
  }


  public static readonly PropertyInfo<string> Value370Property = RegisterProperty<string>(nameof(Value370));
  public string Value370
  {
    get => GetProperty(Value370Property);
    set => SetProperty(Value370Property, value);
  }


  public static readonly PropertyInfo<string> Value371Property = RegisterProperty<string>(nameof(Value371));
  public string Value371
  {
    get => GetProperty(Value371Property);
    set => SetProperty(Value371Property, value);
  }


  public static readonly PropertyInfo<string> Value372Property = RegisterProperty<string>(nameof(Value372));
  public string Value372
  {
    get => GetProperty(Value372Property);
    set => SetProperty(Value372Property, value);
  }


  public static readonly PropertyInfo<string> Value373Property = RegisterProperty<string>(nameof(Value373));
  public string Value373
  {
    get => GetProperty(Value373Property);
    set => SetProperty(Value373Property, value);
  }


  public static readonly PropertyInfo<string> Value374Property = RegisterProperty<string>(nameof(Value374));
  public string Value374
  {
    get => GetProperty(Value374Property);
    set => SetProperty(Value374Property, value);
  }


  public static readonly PropertyInfo<string> Value375Property = RegisterProperty<string>(nameof(Value375));
  public string Value375
  {
    get => GetProperty(Value375Property);
    set => SetProperty(Value375Property, value);
  }


  public static readonly PropertyInfo<string> Value376Property = RegisterProperty<string>(nameof(Value376));
  public string Value376
  {
    get => GetProperty(Value376Property);
    set => SetProperty(Value376Property, value);
  }


  public static readonly PropertyInfo<string> Value377Property = RegisterProperty<string>(nameof(Value377));
  public string Value377
  {
    get => GetProperty(Value377Property);
    set => SetProperty(Value377Property, value);
  }


  public static readonly PropertyInfo<string> Value378Property = RegisterProperty<string>(nameof(Value378));
  public string Value378
  {
    get => GetProperty(Value378Property);
    set => SetProperty(Value378Property, value);
  }


  public static readonly PropertyInfo<string> Value379Property = RegisterProperty<string>(nameof(Value379));
  public string Value379
  {
    get => GetProperty(Value379Property);
    set => SetProperty(Value379Property, value);
  }


  public static readonly PropertyInfo<string> Value380Property = RegisterProperty<string>(nameof(Value380));
  public string Value380
  {
    get => GetProperty(Value380Property);
    set => SetProperty(Value380Property, value);
  }


  public static readonly PropertyInfo<string> Value381Property = RegisterProperty<string>(nameof(Value381));
  public string Value381
  {
    get => GetProperty(Value381Property);
    set => SetProperty(Value381Property, value);
  }


  public static readonly PropertyInfo<string> Value382Property = RegisterProperty<string>(nameof(Value382));
  public string Value382
  {
    get => GetProperty(Value382Property);
    set => SetProperty(Value382Property, value);
  }


  public static readonly PropertyInfo<string> Value383Property = RegisterProperty<string>(nameof(Value383));
  public string Value383
  {
    get => GetProperty(Value383Property);
    set => SetProperty(Value383Property, value);
  }


  public static readonly PropertyInfo<string> Value384Property = RegisterProperty<string>(nameof(Value384));
  public string Value384
  {
    get => GetProperty(Value384Property);
    set => SetProperty(Value384Property, value);
  }


  public static readonly PropertyInfo<string> Value385Property = RegisterProperty<string>(nameof(Value385));
  public string Value385
  {
    get => GetProperty(Value385Property);
    set => SetProperty(Value385Property, value);
  }


  public static readonly PropertyInfo<string> Value386Property = RegisterProperty<string>(nameof(Value386));
  public string Value386
  {
    get => GetProperty(Value386Property);
    set => SetProperty(Value386Property, value);
  }


  public static readonly PropertyInfo<string> Value387Property = RegisterProperty<string>(nameof(Value387));
  public string Value387
  {
    get => GetProperty(Value387Property);
    set => SetProperty(Value387Property, value);
  }


  public static readonly PropertyInfo<string> Value388Property = RegisterProperty<string>(nameof(Value388));
  public string Value388
  {
    get => GetProperty(Value388Property);
    set => SetProperty(Value388Property, value);
  }


  public static readonly PropertyInfo<string> Value389Property = RegisterProperty<string>(nameof(Value389));
  public string Value389
  {
    get => GetProperty(Value389Property);
    set => SetProperty(Value389Property, value);
  }


  public static readonly PropertyInfo<string> Value390Property = RegisterProperty<string>(nameof(Value390));
  public string Value390
  {
    get => GetProperty(Value390Property);
    set => SetProperty(Value390Property, value);
  }


  public static readonly PropertyInfo<string> Value391Property = RegisterProperty<string>(nameof(Value391));
  public string Value391
  {
    get => GetProperty(Value391Property);
    set => SetProperty(Value391Property, value);
  }


  public static readonly PropertyInfo<string> Value392Property = RegisterProperty<string>(nameof(Value392));
  public string Value392
  {
    get => GetProperty(Value392Property);
    set => SetProperty(Value392Property, value);
  }


  public static readonly PropertyInfo<string> Value393Property = RegisterProperty<string>(nameof(Value393));
  public string Value393
  {
    get => GetProperty(Value393Property);
    set => SetProperty(Value393Property, value);
  }


  public static readonly PropertyInfo<string> Value394Property = RegisterProperty<string>(nameof(Value394));
  public string Value394
  {
    get => GetProperty(Value394Property);
    set => SetProperty(Value394Property, value);
  }


  public static readonly PropertyInfo<string> Value395Property = RegisterProperty<string>(nameof(Value395));
  public string Value395
  {
    get => GetProperty(Value395Property);
    set => SetProperty(Value395Property, value);
  }


  public static readonly PropertyInfo<string> Value396Property = RegisterProperty<string>(nameof(Value396));
  public string Value396
  {
    get => GetProperty(Value396Property);
    set => SetProperty(Value396Property, value);
  }


  public static readonly PropertyInfo<string> Value397Property = RegisterProperty<string>(nameof(Value397));
  public string Value397
  {
    get => GetProperty(Value397Property);
    set => SetProperty(Value397Property, value);
  }


  public static readonly PropertyInfo<string> Value398Property = RegisterProperty<string>(nameof(Value398));
  public string Value398
  {
    get => GetProperty(Value398Property);
    set => SetProperty(Value398Property, value);
  }


  public static readonly PropertyInfo<string> Value399Property = RegisterProperty<string>(nameof(Value399));
  public string Value399
  {
    get => GetProperty(Value399Property);
    set => SetProperty(Value399Property, value);
  }


  public static readonly PropertyInfo<string> Value400Property = RegisterProperty<string>(nameof(Value400));
  public string Value400
  {
    get => GetProperty(Value400Property);
    set => SetProperty(Value400Property, value);
  }


  public static readonly PropertyInfo<string> Value401Property = RegisterProperty<string>(nameof(Value401));
  public string Value401
  {
    get => GetProperty(Value401Property);
    set => SetProperty(Value401Property, value);
  }


  public static readonly PropertyInfo<string> Value402Property = RegisterProperty<string>(nameof(Value402));
  public string Value402
  {
    get => GetProperty(Value402Property);
    set => SetProperty(Value402Property, value);
  }


  public static readonly PropertyInfo<string> Value403Property = RegisterProperty<string>(nameof(Value403));
  public string Value403
  {
    get => GetProperty(Value403Property);
    set => SetProperty(Value403Property, value);
  }


  public static readonly PropertyInfo<string> Value404Property = RegisterProperty<string>(nameof(Value404));
  public string Value404
  {
    get => GetProperty(Value404Property);
    set => SetProperty(Value404Property, value);
  }


  public static readonly PropertyInfo<string> Value405Property = RegisterProperty<string>(nameof(Value405));
  public string Value405
  {
    get => GetProperty(Value405Property);
    set => SetProperty(Value405Property, value);
  }


  public static readonly PropertyInfo<string> Value406Property = RegisterProperty<string>(nameof(Value406));
  public string Value406
  {
    get => GetProperty(Value406Property);
    set => SetProperty(Value406Property, value);
  }


  public static readonly PropertyInfo<string> Value407Property = RegisterProperty<string>(nameof(Value407));
  public string Value407
  {
    get => GetProperty(Value407Property);
    set => SetProperty(Value407Property, value);
  }


  public static readonly PropertyInfo<string> Value408Property = RegisterProperty<string>(nameof(Value408));
  public string Value408
  {
    get => GetProperty(Value408Property);
    set => SetProperty(Value408Property, value);
  }


  public static readonly PropertyInfo<string> Value409Property = RegisterProperty<string>(nameof(Value409));
  public string Value409
  {
    get => GetProperty(Value409Property);
    set => SetProperty(Value409Property, value);
  }


  public static readonly PropertyInfo<string> Value410Property = RegisterProperty<string>(nameof(Value410));
  public string Value410
  {
    get => GetProperty(Value410Property);
    set => SetProperty(Value410Property, value);
  }


  public static readonly PropertyInfo<string> Value411Property = RegisterProperty<string>(nameof(Value411));
  public string Value411
  {
    get => GetProperty(Value411Property);
    set => SetProperty(Value411Property, value);
  }


  public static readonly PropertyInfo<string> Value412Property = RegisterProperty<string>(nameof(Value412));
  public string Value412
  {
    get => GetProperty(Value412Property);
    set => SetProperty(Value412Property, value);
  }


  public static readonly PropertyInfo<string> Value413Property = RegisterProperty<string>(nameof(Value413));
  public string Value413
  {
    get => GetProperty(Value413Property);
    set => SetProperty(Value413Property, value);
  }


  public static readonly PropertyInfo<string> Value414Property = RegisterProperty<string>(nameof(Value414));
  public string Value414
  {
    get => GetProperty(Value414Property);
    set => SetProperty(Value414Property, value);
  }


  public static readonly PropertyInfo<string> Value415Property = RegisterProperty<string>(nameof(Value415));
  public string Value415
  {
    get => GetProperty(Value415Property);
    set => SetProperty(Value415Property, value);
  }


  public static readonly PropertyInfo<string> Value416Property = RegisterProperty<string>(nameof(Value416));
  public string Value416
  {
    get => GetProperty(Value416Property);
    set => SetProperty(Value416Property, value);
  }


  public static readonly PropertyInfo<string> Value417Property = RegisterProperty<string>(nameof(Value417));
  public string Value417
  {
    get => GetProperty(Value417Property);
    set => SetProperty(Value417Property, value);
  }


  public static readonly PropertyInfo<string> Value418Property = RegisterProperty<string>(nameof(Value418));
  public string Value418
  {
    get => GetProperty(Value418Property);
    set => SetProperty(Value418Property, value);
  }


  public static readonly PropertyInfo<string> Value419Property = RegisterProperty<string>(nameof(Value419));
  public string Value419
  {
    get => GetProperty(Value419Property);
    set => SetProperty(Value419Property, value);
  }


  public static readonly PropertyInfo<string> Value420Property = RegisterProperty<string>(nameof(Value420));
  public string Value420
  {
    get => GetProperty(Value420Property);
    set => SetProperty(Value420Property, value);
  }


  public static readonly PropertyInfo<string> Value421Property = RegisterProperty<string>(nameof(Value421));
  public string Value421
  {
    get => GetProperty(Value421Property);
    set => SetProperty(Value421Property, value);
  }


  public static readonly PropertyInfo<string> Value422Property = RegisterProperty<string>(nameof(Value422));
  public string Value422
  {
    get => GetProperty(Value422Property);
    set => SetProperty(Value422Property, value);
  }


  public static readonly PropertyInfo<string> Value423Property = RegisterProperty<string>(nameof(Value423));
  public string Value423
  {
    get => GetProperty(Value423Property);
    set => SetProperty(Value423Property, value);
  }


  public static readonly PropertyInfo<string> Value424Property = RegisterProperty<string>(nameof(Value424));
  public string Value424
  {
    get => GetProperty(Value424Property);
    set => SetProperty(Value424Property, value);
  }


  public static readonly PropertyInfo<string> Value425Property = RegisterProperty<string>(nameof(Value425));
  public string Value425
  {
    get => GetProperty(Value425Property);
    set => SetProperty(Value425Property, value);
  }


  public static readonly PropertyInfo<string> Value426Property = RegisterProperty<string>(nameof(Value426));
  public string Value426
  {
    get => GetProperty(Value426Property);
    set => SetProperty(Value426Property, value);
  }


  public static readonly PropertyInfo<string> Value427Property = RegisterProperty<string>(nameof(Value427));
  public string Value427
  {
    get => GetProperty(Value427Property);
    set => SetProperty(Value427Property, value);
  }


  public static readonly PropertyInfo<string> Value428Property = RegisterProperty<string>(nameof(Value428));
  public string Value428
  {
    get => GetProperty(Value428Property);
    set => SetProperty(Value428Property, value);
  }


  public static readonly PropertyInfo<string> Value429Property = RegisterProperty<string>(nameof(Value429));
  public string Value429
  {
    get => GetProperty(Value429Property);
    set => SetProperty(Value429Property, value);
  }


  public static readonly PropertyInfo<string> Value430Property = RegisterProperty<string>(nameof(Value430));
  public string Value430
  {
    get => GetProperty(Value430Property);
    set => SetProperty(Value430Property, value);
  }


  public static readonly PropertyInfo<string> Value431Property = RegisterProperty<string>(nameof(Value431));
  public string Value431
  {
    get => GetProperty(Value431Property);
    set => SetProperty(Value431Property, value);
  }


  public static readonly PropertyInfo<string> Value432Property = RegisterProperty<string>(nameof(Value432));
  public string Value432
  {
    get => GetProperty(Value432Property);
    set => SetProperty(Value432Property, value);
  }


  public static readonly PropertyInfo<string> Value433Property = RegisterProperty<string>(nameof(Value433));
  public string Value433
  {
    get => GetProperty(Value433Property);
    set => SetProperty(Value433Property, value);
  }


  public static readonly PropertyInfo<string> Value434Property = RegisterProperty<string>(nameof(Value434));
  public string Value434
  {
    get => GetProperty(Value434Property);
    set => SetProperty(Value434Property, value);
  }


  public static readonly PropertyInfo<string> Value435Property = RegisterProperty<string>(nameof(Value435));
  public string Value435
  {
    get => GetProperty(Value435Property);
    set => SetProperty(Value435Property, value);
  }


  public static readonly PropertyInfo<string> Value436Property = RegisterProperty<string>(nameof(Value436));
  public string Value436
  {
    get => GetProperty(Value436Property);
    set => SetProperty(Value436Property, value);
  }


  public static readonly PropertyInfo<string> Value437Property = RegisterProperty<string>(nameof(Value437));
  public string Value437
  {
    get => GetProperty(Value437Property);
    set => SetProperty(Value437Property, value);
  }


  public static readonly PropertyInfo<string> Value438Property = RegisterProperty<string>(nameof(Value438));
  public string Value438
  {
    get => GetProperty(Value438Property);
    set => SetProperty(Value438Property, value);
  }


  public static readonly PropertyInfo<string> Value439Property = RegisterProperty<string>(nameof(Value439));
  public string Value439
  {
    get => GetProperty(Value439Property);
    set => SetProperty(Value439Property, value);
  }


  public static readonly PropertyInfo<string> Value440Property = RegisterProperty<string>(nameof(Value440));
  public string Value440
  {
    get => GetProperty(Value440Property);
    set => SetProperty(Value440Property, value);
  }


  public static readonly PropertyInfo<string> Value441Property = RegisterProperty<string>(nameof(Value441));
  public string Value441
  {
    get => GetProperty(Value441Property);
    set => SetProperty(Value441Property, value);
  }


  public static readonly PropertyInfo<string> Value442Property = RegisterProperty<string>(nameof(Value442));
  public string Value442
  {
    get => GetProperty(Value442Property);
    set => SetProperty(Value442Property, value);
  }


  public static readonly PropertyInfo<string> Value443Property = RegisterProperty<string>(nameof(Value443));
  public string Value443
  {
    get => GetProperty(Value443Property);
    set => SetProperty(Value443Property, value);
  }


  public static readonly PropertyInfo<string> Value444Property = RegisterProperty<string>(nameof(Value444));
  public string Value444
  {
    get => GetProperty(Value444Property);
    set => SetProperty(Value444Property, value);
  }


  public static readonly PropertyInfo<string> Value445Property = RegisterProperty<string>(nameof(Value445));
  public string Value445
  {
    get => GetProperty(Value445Property);
    set => SetProperty(Value445Property, value);
  }


  public static readonly PropertyInfo<string> Value446Property = RegisterProperty<string>(nameof(Value446));
  public string Value446
  {
    get => GetProperty(Value446Property);
    set => SetProperty(Value446Property, value);
  }


  public static readonly PropertyInfo<string> Value447Property = RegisterProperty<string>(nameof(Value447));
  public string Value447
  {
    get => GetProperty(Value447Property);
    set => SetProperty(Value447Property, value);
  }


  public static readonly PropertyInfo<string> Value448Property = RegisterProperty<string>(nameof(Value448));
  public string Value448
  {
    get => GetProperty(Value448Property);
    set => SetProperty(Value448Property, value);
  }


  public static readonly PropertyInfo<string> Value449Property = RegisterProperty<string>(nameof(Value449));
  public string Value449
  {
    get => GetProperty(Value449Property);
    set => SetProperty(Value449Property, value);
  }


  public static readonly PropertyInfo<string> Value450Property = RegisterProperty<string>(nameof(Value450));
  public string Value450
  {
    get => GetProperty(Value450Property);
    set => SetProperty(Value450Property, value);
  }


  public static readonly PropertyInfo<string> Value451Property = RegisterProperty<string>(nameof(Value451));
  public string Value451
  {
    get => GetProperty(Value451Property);
    set => SetProperty(Value451Property, value);
  }


  public static readonly PropertyInfo<string> Value452Property = RegisterProperty<string>(nameof(Value452));
  public string Value452
  {
    get => GetProperty(Value452Property);
    set => SetProperty(Value452Property, value);
  }


  public static readonly PropertyInfo<string> Value453Property = RegisterProperty<string>(nameof(Value453));
  public string Value453
  {
    get => GetProperty(Value453Property);
    set => SetProperty(Value453Property, value);
  }


  public static readonly PropertyInfo<string> Value454Property = RegisterProperty<string>(nameof(Value454));
  public string Value454
  {
    get => GetProperty(Value454Property);
    set => SetProperty(Value454Property, value);
  }


  public static readonly PropertyInfo<string> Value455Property = RegisterProperty<string>(nameof(Value455));
  public string Value455
  {
    get => GetProperty(Value455Property);
    set => SetProperty(Value455Property, value);
  }


  public static readonly PropertyInfo<string> Value456Property = RegisterProperty<string>(nameof(Value456));
  public string Value456
  {
    get => GetProperty(Value456Property);
    set => SetProperty(Value456Property, value);
  }


  public static readonly PropertyInfo<string> Value457Property = RegisterProperty<string>(nameof(Value457));
  public string Value457
  {
    get => GetProperty(Value457Property);
    set => SetProperty(Value457Property, value);
  }


  public static readonly PropertyInfo<string> Value458Property = RegisterProperty<string>(nameof(Value458));
  public string Value458
  {
    get => GetProperty(Value458Property);
    set => SetProperty(Value458Property, value);
  }


  public static readonly PropertyInfo<string> Value459Property = RegisterProperty<string>(nameof(Value459));
  public string Value459
  {
    get => GetProperty(Value459Property);
    set => SetProperty(Value459Property, value);
  }


  public static readonly PropertyInfo<string> Value460Property = RegisterProperty<string>(nameof(Value460));
  public string Value460
  {
    get => GetProperty(Value460Property);
    set => SetProperty(Value460Property, value);
  }


  public static readonly PropertyInfo<string> Value461Property = RegisterProperty<string>(nameof(Value461));
  public string Value461
  {
    get => GetProperty(Value461Property);
    set => SetProperty(Value461Property, value);
  }


  public static readonly PropertyInfo<string> Value462Property = RegisterProperty<string>(nameof(Value462));
  public string Value462
  {
    get => GetProperty(Value462Property);
    set => SetProperty(Value462Property, value);
  }


  public static readonly PropertyInfo<string> Value463Property = RegisterProperty<string>(nameof(Value463));
  public string Value463
  {
    get => GetProperty(Value463Property);
    set => SetProperty(Value463Property, value);
  }


  public static readonly PropertyInfo<string> Value464Property = RegisterProperty<string>(nameof(Value464));
  public string Value464
  {
    get => GetProperty(Value464Property);
    set => SetProperty(Value464Property, value);
  }


  public static readonly PropertyInfo<string> Value465Property = RegisterProperty<string>(nameof(Value465));
  public string Value465
  {
    get => GetProperty(Value465Property);
    set => SetProperty(Value465Property, value);
  }


  public static readonly PropertyInfo<string> Value466Property = RegisterProperty<string>(nameof(Value466));
  public string Value466
  {
    get => GetProperty(Value466Property);
    set => SetProperty(Value466Property, value);
  }


  public static readonly PropertyInfo<string> Value467Property = RegisterProperty<string>(nameof(Value467));
  public string Value467
  {
    get => GetProperty(Value467Property);
    set => SetProperty(Value467Property, value);
  }


  public static readonly PropertyInfo<string> Value468Property = RegisterProperty<string>(nameof(Value468));
  public string Value468
  {
    get => GetProperty(Value468Property);
    set => SetProperty(Value468Property, value);
  }


  public static readonly PropertyInfo<string> Value469Property = RegisterProperty<string>(nameof(Value469));
  public string Value469
  {
    get => GetProperty(Value469Property);
    set => SetProperty(Value469Property, value);
  }


  public static readonly PropertyInfo<string> Value470Property = RegisterProperty<string>(nameof(Value470));
  public string Value470
  {
    get => GetProperty(Value470Property);
    set => SetProperty(Value470Property, value);
  }


  public static readonly PropertyInfo<string> Value471Property = RegisterProperty<string>(nameof(Value471));
  public string Value471
  {
    get => GetProperty(Value471Property);
    set => SetProperty(Value471Property, value);
  }


  public static readonly PropertyInfo<string> Value472Property = RegisterProperty<string>(nameof(Value472));
  public string Value472
  {
    get => GetProperty(Value472Property);
    set => SetProperty(Value472Property, value);
  }


  public static readonly PropertyInfo<string> Value473Property = RegisterProperty<string>(nameof(Value473));
  public string Value473
  {
    get => GetProperty(Value473Property);
    set => SetProperty(Value473Property, value);
  }


  public static readonly PropertyInfo<string> Value474Property = RegisterProperty<string>(nameof(Value474));
  public string Value474
  {
    get => GetProperty(Value474Property);
    set => SetProperty(Value474Property, value);
  }


  public static readonly PropertyInfo<string> Value475Property = RegisterProperty<string>(nameof(Value475));
  public string Value475
  {
    get => GetProperty(Value475Property);
    set => SetProperty(Value475Property, value);
  }


  public static readonly PropertyInfo<string> Value476Property = RegisterProperty<string>(nameof(Value476));
  public string Value476
  {
    get => GetProperty(Value476Property);
    set => SetProperty(Value476Property, value);
  }


  public static readonly PropertyInfo<string> Value477Property = RegisterProperty<string>(nameof(Value477));
  public string Value477
  {
    get => GetProperty(Value477Property);
    set => SetProperty(Value477Property, value);
  }


  public static readonly PropertyInfo<string> Value478Property = RegisterProperty<string>(nameof(Value478));
  public string Value478
  {
    get => GetProperty(Value478Property);
    set => SetProperty(Value478Property, value);
  }


  public static readonly PropertyInfo<string> Value479Property = RegisterProperty<string>(nameof(Value479));
  public string Value479
  {
    get => GetProperty(Value479Property);
    set => SetProperty(Value479Property, value);
  }


  public static readonly PropertyInfo<string> Value480Property = RegisterProperty<string>(nameof(Value480));
  public string Value480
  {
    get => GetProperty(Value480Property);
    set => SetProperty(Value480Property, value);
  }


  public static readonly PropertyInfo<string> Value481Property = RegisterProperty<string>(nameof(Value481));
  public string Value481
  {
    get => GetProperty(Value481Property);
    set => SetProperty(Value481Property, value);
  }


  public static readonly PropertyInfo<string> Value482Property = RegisterProperty<string>(nameof(Value482));
  public string Value482
  {
    get => GetProperty(Value482Property);
    set => SetProperty(Value482Property, value);
  }


  public static readonly PropertyInfo<string> Value483Property = RegisterProperty<string>(nameof(Value483));
  public string Value483
  {
    get => GetProperty(Value483Property);
    set => SetProperty(Value483Property, value);
  }


  public static readonly PropertyInfo<string> Value484Property = RegisterProperty<string>(nameof(Value484));
  public string Value484
  {
    get => GetProperty(Value484Property);
    set => SetProperty(Value484Property, value);
  }


  public static readonly PropertyInfo<string> Value485Property = RegisterProperty<string>(nameof(Value485));
  public string Value485
  {
    get => GetProperty(Value485Property);
    set => SetProperty(Value485Property, value);
  }


  public static readonly PropertyInfo<string> Value486Property = RegisterProperty<string>(nameof(Value486));
  public string Value486
  {
    get => GetProperty(Value486Property);
    set => SetProperty(Value486Property, value);
  }


  public static readonly PropertyInfo<string> Value487Property = RegisterProperty<string>(nameof(Value487));
  public string Value487
  {
    get => GetProperty(Value487Property);
    set => SetProperty(Value487Property, value);
  }


  public static readonly PropertyInfo<string> Value488Property = RegisterProperty<string>(nameof(Value488));
  public string Value488
  {
    get => GetProperty(Value488Property);
    set => SetProperty(Value488Property, value);
  }


  public static readonly PropertyInfo<string> Value489Property = RegisterProperty<string>(nameof(Value489));
  public string Value489
  {
    get => GetProperty(Value489Property);
    set => SetProperty(Value489Property, value);
  }


  public static readonly PropertyInfo<string> Value490Property = RegisterProperty<string>(nameof(Value490));
  public string Value490
  {
    get => GetProperty(Value490Property);
    set => SetProperty(Value490Property, value);
  }


  public static readonly PropertyInfo<string> Value491Property = RegisterProperty<string>(nameof(Value491));
  public string Value491
  {
    get => GetProperty(Value491Property);
    set => SetProperty(Value491Property, value);
  }


  public static readonly PropertyInfo<string> Value492Property = RegisterProperty<string>(nameof(Value492));
  public string Value492
  {
    get => GetProperty(Value492Property);
    set => SetProperty(Value492Property, value);
  }


  public static readonly PropertyInfo<string> Value493Property = RegisterProperty<string>(nameof(Value493));
  public string Value493
  {
    get => GetProperty(Value493Property);
    set => SetProperty(Value493Property, value);
  }


  public static readonly PropertyInfo<string> Value494Property = RegisterProperty<string>(nameof(Value494));
  public string Value494
  {
    get => GetProperty(Value494Property);
    set => SetProperty(Value494Property, value);
  }


  public static readonly PropertyInfo<string> Value495Property = RegisterProperty<string>(nameof(Value495));
  public string Value495
  {
    get => GetProperty(Value495Property);
    set => SetProperty(Value495Property, value);
  }


  public static readonly PropertyInfo<string> Value496Property = RegisterProperty<string>(nameof(Value496));
  public string Value496
  {
    get => GetProperty(Value496Property);
    set => SetProperty(Value496Property, value);
  }


  public static readonly PropertyInfo<string> Value497Property = RegisterProperty<string>(nameof(Value497));
  public string Value497
  {
    get => GetProperty(Value497Property);
    set => SetProperty(Value497Property, value);
  }


  public static readonly PropertyInfo<string> Value498Property = RegisterProperty<string>(nameof(Value498));
  public string Value498
  {
    get => GetProperty(Value498Property);
    set => SetProperty(Value498Property, value);
  }


  public static readonly PropertyInfo<string> Value499Property = RegisterProperty<string>(nameof(Value499));
  public string Value499
  {
    get => GetProperty(Value499Property);
    set => SetProperty(Value499Property, value);
  }


  public static readonly PropertyInfo<string> Value500Property = RegisterProperty<string>(nameof(Value500));
  public string Value500
  {
    get => GetProperty(Value500Property);
    set => SetProperty(Value500Property, value);
  }


  public static readonly PropertyInfo<string> Value501Property = RegisterProperty<string>(nameof(Value501));
  public string Value501
  {
    get => GetProperty(Value501Property);
    set => SetProperty(Value501Property, value);
  }


  public static readonly PropertyInfo<string> Value502Property = RegisterProperty<string>(nameof(Value502));
  public string Value502
  {
    get => GetProperty(Value502Property);
    set => SetProperty(Value502Property, value);
  }


  public static readonly PropertyInfo<string> Value503Property = RegisterProperty<string>(nameof(Value503));
  public string Value503
  {
    get => GetProperty(Value503Property);
    set => SetProperty(Value503Property, value);
  }


  public static readonly PropertyInfo<string> Value504Property = RegisterProperty<string>(nameof(Value504));
  public string Value504
  {
    get => GetProperty(Value504Property);
    set => SetProperty(Value504Property, value);
  }


  public static readonly PropertyInfo<string> Value505Property = RegisterProperty<string>(nameof(Value505));
  public string Value505
  {
    get => GetProperty(Value505Property);
    set => SetProperty(Value505Property, value);
  }


  public static readonly PropertyInfo<string> Value506Property = RegisterProperty<string>(nameof(Value506));
  public string Value506
  {
    get => GetProperty(Value506Property);
    set => SetProperty(Value506Property, value);
  }


  public static readonly PropertyInfo<string> Value507Property = RegisterProperty<string>(nameof(Value507));
  public string Value507
  {
    get => GetProperty(Value507Property);
    set => SetProperty(Value507Property, value);
  }


  public static readonly PropertyInfo<string> Value508Property = RegisterProperty<string>(nameof(Value508));
  public string Value508
  {
    get => GetProperty(Value508Property);
    set => SetProperty(Value508Property, value);
  }


  public static readonly PropertyInfo<string> Value509Property = RegisterProperty<string>(nameof(Value509));
  public string Value509
  {
    get => GetProperty(Value509Property);
    set => SetProperty(Value509Property, value);
  }


  public static readonly PropertyInfo<string> Value510Property = RegisterProperty<string>(nameof(Value510));
  public string Value510
  {
    get => GetProperty(Value510Property);
    set => SetProperty(Value510Property, value);
  }


  public static readonly PropertyInfo<string> Value511Property = RegisterProperty<string>(nameof(Value511));
  public string Value511
  {
    get => GetProperty(Value511Property);
    set => SetProperty(Value511Property, value);
  }


  public static readonly PropertyInfo<string> Value512Property = RegisterProperty<string>(nameof(Value512));
  public string Value512
  {
    get => GetProperty(Value512Property);
    set => SetProperty(Value512Property, value);
  }


  public static readonly PropertyInfo<string> Value513Property = RegisterProperty<string>(nameof(Value513));
  public string Value513
  {
    get => GetProperty(Value513Property);
    set => SetProperty(Value513Property, value);
  }


  public static readonly PropertyInfo<string> Value514Property = RegisterProperty<string>(nameof(Value514));
  public string Value514
  {
    get => GetProperty(Value514Property);
    set => SetProperty(Value514Property, value);
  }


  public static readonly PropertyInfo<string> Value515Property = RegisterProperty<string>(nameof(Value515));
  public string Value515
  {
    get => GetProperty(Value515Property);
    set => SetProperty(Value515Property, value);
  }


  public static readonly PropertyInfo<string> Value516Property = RegisterProperty<string>(nameof(Value516));
  public string Value516
  {
    get => GetProperty(Value516Property);
    set => SetProperty(Value516Property, value);
  }


  public static readonly PropertyInfo<string> Value517Property = RegisterProperty<string>(nameof(Value517));
  public string Value517
  {
    get => GetProperty(Value517Property);
    set => SetProperty(Value517Property, value);
  }


  public static readonly PropertyInfo<string> Value518Property = RegisterProperty<string>(nameof(Value518));
  public string Value518
  {
    get => GetProperty(Value518Property);
    set => SetProperty(Value518Property, value);
  }


  public static readonly PropertyInfo<string> Value519Property = RegisterProperty<string>(nameof(Value519));
  public string Value519
  {
    get => GetProperty(Value519Property);
    set => SetProperty(Value519Property, value);
  }


  public static readonly PropertyInfo<string> Value520Property = RegisterProperty<string>(nameof(Value520));
  public string Value520
  {
    get => GetProperty(Value520Property);
    set => SetProperty(Value520Property, value);
  }


  public static readonly PropertyInfo<string> Value521Property = RegisterProperty<string>(nameof(Value521));
  public string Value521
  {
    get => GetProperty(Value521Property);
    set => SetProperty(Value521Property, value);
  }


  public static readonly PropertyInfo<string> Value522Property = RegisterProperty<string>(nameof(Value522));
  public string Value522
  {
    get => GetProperty(Value522Property);
    set => SetProperty(Value522Property, value);
  }


  public static readonly PropertyInfo<string> Value523Property = RegisterProperty<string>(nameof(Value523));
  public string Value523
  {
    get => GetProperty(Value523Property);
    set => SetProperty(Value523Property, value);
  }


  public static readonly PropertyInfo<string> Value524Property = RegisterProperty<string>(nameof(Value524));
  public string Value524
  {
    get => GetProperty(Value524Property);
    set => SetProperty(Value524Property, value);
  }


  public static readonly PropertyInfo<string> Value525Property = RegisterProperty<string>(nameof(Value525));
  public string Value525
  {
    get => GetProperty(Value525Property);
    set => SetProperty(Value525Property, value);
  }


  public static readonly PropertyInfo<string> Value526Property = RegisterProperty<string>(nameof(Value526));
  public string Value526
  {
    get => GetProperty(Value526Property);
    set => SetProperty(Value526Property, value);
  }


  public static readonly PropertyInfo<string> Value527Property = RegisterProperty<string>(nameof(Value527));
  public string Value527
  {
    get => GetProperty(Value527Property);
    set => SetProperty(Value527Property, value);
  }


  public static readonly PropertyInfo<string> Value528Property = RegisterProperty<string>(nameof(Value528));
  public string Value528
  {
    get => GetProperty(Value528Property);
    set => SetProperty(Value528Property, value);
  }


  public static readonly PropertyInfo<string> Value529Property = RegisterProperty<string>(nameof(Value529));
  public string Value529
  {
    get => GetProperty(Value529Property);
    set => SetProperty(Value529Property, value);
  }


  public static readonly PropertyInfo<string> Value530Property = RegisterProperty<string>(nameof(Value530));
  public string Value530
  {
    get => GetProperty(Value530Property);
    set => SetProperty(Value530Property, value);
  }


  public static readonly PropertyInfo<string> Value531Property = RegisterProperty<string>(nameof(Value531));
  public string Value531
  {
    get => GetProperty(Value531Property);
    set => SetProperty(Value531Property, value);
  }


  public static readonly PropertyInfo<string> Value532Property = RegisterProperty<string>(nameof(Value532));
  public string Value532
  {
    get => GetProperty(Value532Property);
    set => SetProperty(Value532Property, value);
  }


  public static readonly PropertyInfo<string> Value533Property = RegisterProperty<string>(nameof(Value533));
  public string Value533
  {
    get => GetProperty(Value533Property);
    set => SetProperty(Value533Property, value);
  }


  public static readonly PropertyInfo<string> Value534Property = RegisterProperty<string>(nameof(Value534));
  public string Value534
  {
    get => GetProperty(Value534Property);
    set => SetProperty(Value534Property, value);
  }


  public static readonly PropertyInfo<string> Value535Property = RegisterProperty<string>(nameof(Value535));
  public string Value535
  {
    get => GetProperty(Value535Property);
    set => SetProperty(Value535Property, value);
  }


  public static readonly PropertyInfo<string> Value536Property = RegisterProperty<string>(nameof(Value536));
  public string Value536
  {
    get => GetProperty(Value536Property);
    set => SetProperty(Value536Property, value);
  }


  public static readonly PropertyInfo<string> Value537Property = RegisterProperty<string>(nameof(Value537));
  public string Value537
  {
    get => GetProperty(Value537Property);
    set => SetProperty(Value537Property, value);
  }


  public static readonly PropertyInfo<string> Value538Property = RegisterProperty<string>(nameof(Value538));
  public string Value538
  {
    get => GetProperty(Value538Property);
    set => SetProperty(Value538Property, value);
  }


  public static readonly PropertyInfo<string> Value539Property = RegisterProperty<string>(nameof(Value539));
  public string Value539
  {
    get => GetProperty(Value539Property);
    set => SetProperty(Value539Property, value);
  }


  public static readonly PropertyInfo<string> Value540Property = RegisterProperty<string>(nameof(Value540));
  public string Value540
  {
    get => GetProperty(Value540Property);
    set => SetProperty(Value540Property, value);
  }


  public static readonly PropertyInfo<string> Value541Property = RegisterProperty<string>(nameof(Value541));
  public string Value541
  {
    get => GetProperty(Value541Property);
    set => SetProperty(Value541Property, value);
  }


  public static readonly PropertyInfo<string> Value542Property = RegisterProperty<string>(nameof(Value542));
  public string Value542
  {
    get => GetProperty(Value542Property);
    set => SetProperty(Value542Property, value);
  }


  public static readonly PropertyInfo<string> Value543Property = RegisterProperty<string>(nameof(Value543));
  public string Value543
  {
    get => GetProperty(Value543Property);
    set => SetProperty(Value543Property, value);
  }


  public static readonly PropertyInfo<string> Value544Property = RegisterProperty<string>(nameof(Value544));
  public string Value544
  {
    get => GetProperty(Value544Property);
    set => SetProperty(Value544Property, value);
  }


  public static readonly PropertyInfo<string> Value545Property = RegisterProperty<string>(nameof(Value545));
  public string Value545
  {
    get => GetProperty(Value545Property);
    set => SetProperty(Value545Property, value);
  }


  public static readonly PropertyInfo<string> Value546Property = RegisterProperty<string>(nameof(Value546));
  public string Value546
  {
    get => GetProperty(Value546Property);
    set => SetProperty(Value546Property, value);
  }


  public static readonly PropertyInfo<string> Value547Property = RegisterProperty<string>(nameof(Value547));
  public string Value547
  {
    get => GetProperty(Value547Property);
    set => SetProperty(Value547Property, value);
  }


  public static readonly PropertyInfo<string> Value548Property = RegisterProperty<string>(nameof(Value548));
  public string Value548
  {
    get => GetProperty(Value548Property);
    set => SetProperty(Value548Property, value);
  }


  public static readonly PropertyInfo<string> Value549Property = RegisterProperty<string>(nameof(Value549));
  public string Value549
  {
    get => GetProperty(Value549Property);
    set => SetProperty(Value549Property, value);
  }


  public static readonly PropertyInfo<string> Value550Property = RegisterProperty<string>(nameof(Value550));
  public string Value550
  {
    get => GetProperty(Value550Property);
    set => SetProperty(Value550Property, value);
  }


  public static readonly PropertyInfo<string> Value551Property = RegisterProperty<string>(nameof(Value551));
  public string Value551
  {
    get => GetProperty(Value551Property);
    set => SetProperty(Value551Property, value);
  }


  public static readonly PropertyInfo<string> Value552Property = RegisterProperty<string>(nameof(Value552));
  public string Value552
  {
    get => GetProperty(Value552Property);
    set => SetProperty(Value552Property, value);
  }


  public static readonly PropertyInfo<string> Value553Property = RegisterProperty<string>(nameof(Value553));
  public string Value553
  {
    get => GetProperty(Value553Property);
    set => SetProperty(Value553Property, value);
  }


  public static readonly PropertyInfo<string> Value554Property = RegisterProperty<string>(nameof(Value554));
  public string Value554
  {
    get => GetProperty(Value554Property);
    set => SetProperty(Value554Property, value);
  }


  public static readonly PropertyInfo<string> Value555Property = RegisterProperty<string>(nameof(Value555));
  public string Value555
  {
    get => GetProperty(Value555Property);
    set => SetProperty(Value555Property, value);
  }


  public static readonly PropertyInfo<string> Value556Property = RegisterProperty<string>(nameof(Value556));
  public string Value556
  {
    get => GetProperty(Value556Property);
    set => SetProperty(Value556Property, value);
  }


  public static readonly PropertyInfo<string> Value557Property = RegisterProperty<string>(nameof(Value557));
  public string Value557
  {
    get => GetProperty(Value557Property);
    set => SetProperty(Value557Property, value);
  }


  public static readonly PropertyInfo<string> Value558Property = RegisterProperty<string>(nameof(Value558));
  public string Value558
  {
    get => GetProperty(Value558Property);
    set => SetProperty(Value558Property, value);
  }


  public static readonly PropertyInfo<string> Value559Property = RegisterProperty<string>(nameof(Value559));
  public string Value559
  {
    get => GetProperty(Value559Property);
    set => SetProperty(Value559Property, value);
  }


  public static readonly PropertyInfo<string> Value560Property = RegisterProperty<string>(nameof(Value560));
  public string Value560
  {
    get => GetProperty(Value560Property);
    set => SetProperty(Value560Property, value);
  }


  public static readonly PropertyInfo<string> Value561Property = RegisterProperty<string>(nameof(Value561));
  public string Value561
  {
    get => GetProperty(Value561Property);
    set => SetProperty(Value561Property, value);
  }


  public static readonly PropertyInfo<string> Value562Property = RegisterProperty<string>(nameof(Value562));
  public string Value562
  {
    get => GetProperty(Value562Property);
    set => SetProperty(Value562Property, value);
  }


  public static readonly PropertyInfo<string> Value563Property = RegisterProperty<string>(nameof(Value563));
  public string Value563
  {
    get => GetProperty(Value563Property);
    set => SetProperty(Value563Property, value);
  }


  public static readonly PropertyInfo<string> Value564Property = RegisterProperty<string>(nameof(Value564));
  public string Value564
  {
    get => GetProperty(Value564Property);
    set => SetProperty(Value564Property, value);
  }


  public static readonly PropertyInfo<string> Value565Property = RegisterProperty<string>(nameof(Value565));
  public string Value565
  {
    get => GetProperty(Value565Property);
    set => SetProperty(Value565Property, value);
  }


  public static readonly PropertyInfo<string> Value566Property = RegisterProperty<string>(nameof(Value566));
  public string Value566
  {
    get => GetProperty(Value566Property);
    set => SetProperty(Value566Property, value);
  }


  public static readonly PropertyInfo<string> Value567Property = RegisterProperty<string>(nameof(Value567));
  public string Value567
  {
    get => GetProperty(Value567Property);
    set => SetProperty(Value567Property, value);
  }


  public static readonly PropertyInfo<string> Value568Property = RegisterProperty<string>(nameof(Value568));
  public string Value568
  {
    get => GetProperty(Value568Property);
    set => SetProperty(Value568Property, value);
  }


  public static readonly PropertyInfo<string> Value569Property = RegisterProperty<string>(nameof(Value569));
  public string Value569
  {
    get => GetProperty(Value569Property);
    set => SetProperty(Value569Property, value);
  }


  public static readonly PropertyInfo<string> Value570Property = RegisterProperty<string>(nameof(Value570));
  public string Value570
  {
    get => GetProperty(Value570Property);
    set => SetProperty(Value570Property, value);
  }


  public static readonly PropertyInfo<string> Value571Property = RegisterProperty<string>(nameof(Value571));
  public string Value571
  {
    get => GetProperty(Value571Property);
    set => SetProperty(Value571Property, value);
  }


  public static readonly PropertyInfo<string> Value572Property = RegisterProperty<string>(nameof(Value572));
  public string Value572
  {
    get => GetProperty(Value572Property);
    set => SetProperty(Value572Property, value);
  }


  public static readonly PropertyInfo<string> Value573Property = RegisterProperty<string>(nameof(Value573));
  public string Value573
  {
    get => GetProperty(Value573Property);
    set => SetProperty(Value573Property, value);
  }


  public static readonly PropertyInfo<string> Value574Property = RegisterProperty<string>(nameof(Value574));
  public string Value574
  {
    get => GetProperty(Value574Property);
    set => SetProperty(Value574Property, value);
  }


  public static readonly PropertyInfo<string> Value575Property = RegisterProperty<string>(nameof(Value575));
  public string Value575
  {
    get => GetProperty(Value575Property);
    set => SetProperty(Value575Property, value);
  }


  public static readonly PropertyInfo<string> Value576Property = RegisterProperty<string>(nameof(Value576));
  public string Value576
  {
    get => GetProperty(Value576Property);
    set => SetProperty(Value576Property, value);
  }


  public static readonly PropertyInfo<string> Value577Property = RegisterProperty<string>(nameof(Value577));
  public string Value577
  {
    get => GetProperty(Value577Property);
    set => SetProperty(Value577Property, value);
  }


  public static readonly PropertyInfo<string> Value578Property = RegisterProperty<string>(nameof(Value578));
  public string Value578
  {
    get => GetProperty(Value578Property);
    set => SetProperty(Value578Property, value);
  }


  public static readonly PropertyInfo<string> Value579Property = RegisterProperty<string>(nameof(Value579));
  public string Value579
  {
    get => GetProperty(Value579Property);
    set => SetProperty(Value579Property, value);
  }


  public static readonly PropertyInfo<string> Value580Property = RegisterProperty<string>(nameof(Value580));
  public string Value580
  {
    get => GetProperty(Value580Property);
    set => SetProperty(Value580Property, value);
  }


  public static readonly PropertyInfo<string> Value581Property = RegisterProperty<string>(nameof(Value581));
  public string Value581
  {
    get => GetProperty(Value581Property);
    set => SetProperty(Value581Property, value);
  }


  public static readonly PropertyInfo<string> Value582Property = RegisterProperty<string>(nameof(Value582));
  public string Value582
  {
    get => GetProperty(Value582Property);
    set => SetProperty(Value582Property, value);
  }


  public static readonly PropertyInfo<string> Value583Property = RegisterProperty<string>(nameof(Value583));
  public string Value583
  {
    get => GetProperty(Value583Property);
    set => SetProperty(Value583Property, value);
  }


  public static readonly PropertyInfo<string> Value584Property = RegisterProperty<string>(nameof(Value584));
  public string Value584
  {
    get => GetProperty(Value584Property);
    set => SetProperty(Value584Property, value);
  }


  public static readonly PropertyInfo<string> Value585Property = RegisterProperty<string>(nameof(Value585));
  public string Value585
  {
    get => GetProperty(Value585Property);
    set => SetProperty(Value585Property, value);
  }


  public static readonly PropertyInfo<string> Value586Property = RegisterProperty<string>(nameof(Value586));
  public string Value586
  {
    get => GetProperty(Value586Property);
    set => SetProperty(Value586Property, value);
  }


  public static readonly PropertyInfo<string> Value587Property = RegisterProperty<string>(nameof(Value587));
  public string Value587
  {
    get => GetProperty(Value587Property);
    set => SetProperty(Value587Property, value);
  }


  public static readonly PropertyInfo<string> Value588Property = RegisterProperty<string>(nameof(Value588));
  public string Value588
  {
    get => GetProperty(Value588Property);
    set => SetProperty(Value588Property, value);
  }


  public static readonly PropertyInfo<string> Value589Property = RegisterProperty<string>(nameof(Value589));
  public string Value589
  {
    get => GetProperty(Value589Property);
    set => SetProperty(Value589Property, value);
  }


  public static readonly PropertyInfo<string> Value590Property = RegisterProperty<string>(nameof(Value590));
  public string Value590
  {
    get => GetProperty(Value590Property);
    set => SetProperty(Value590Property, value);
  }


  public static readonly PropertyInfo<string> Value591Property = RegisterProperty<string>(nameof(Value591));
  public string Value591
  {
    get => GetProperty(Value591Property);
    set => SetProperty(Value591Property, value);
  }


  public static readonly PropertyInfo<string> Value592Property = RegisterProperty<string>(nameof(Value592));
  public string Value592
  {
    get => GetProperty(Value592Property);
    set => SetProperty(Value592Property, value);
  }


  public static readonly PropertyInfo<string> Value593Property = RegisterProperty<string>(nameof(Value593));
  public string Value593
  {
    get => GetProperty(Value593Property);
    set => SetProperty(Value593Property, value);
  }


  public static readonly PropertyInfo<string> Value594Property = RegisterProperty<string>(nameof(Value594));
  public string Value594
  {
    get => GetProperty(Value594Property);
    set => SetProperty(Value594Property, value);
  }


  public static readonly PropertyInfo<string> Value595Property = RegisterProperty<string>(nameof(Value595));
  public string Value595
  {
    get => GetProperty(Value595Property);
    set => SetProperty(Value595Property, value);
  }


  public static readonly PropertyInfo<string> Value596Property = RegisterProperty<string>(nameof(Value596));
  public string Value596
  {
    get => GetProperty(Value596Property);
    set => SetProperty(Value596Property, value);
  }


  public static readonly PropertyInfo<string> Value597Property = RegisterProperty<string>(nameof(Value597));
  public string Value597
  {
    get => GetProperty(Value597Property);
    set => SetProperty(Value597Property, value);
  }


  public static readonly PropertyInfo<string> Value598Property = RegisterProperty<string>(nameof(Value598));
  public string Value598
  {
    get => GetProperty(Value598Property);
    set => SetProperty(Value598Property, value);
  }


  public static readonly PropertyInfo<string> Value599Property = RegisterProperty<string>(nameof(Value599));
  public string Value599
  {
    get => GetProperty(Value599Property);
    set => SetProperty(Value599Property, value);
  }


  public static readonly PropertyInfo<string> Value600Property = RegisterProperty<string>(nameof(Value600));
  public string Value600
  {
    get => GetProperty(Value600Property);
    set => SetProperty(Value600Property, value);
  }


  public static readonly PropertyInfo<string> Value601Property = RegisterProperty<string>(nameof(Value601));
  public string Value601
  {
    get => GetProperty(Value601Property);
    set => SetProperty(Value601Property, value);
  }


  public static readonly PropertyInfo<string> Value602Property = RegisterProperty<string>(nameof(Value602));
  public string Value602
  {
    get => GetProperty(Value602Property);
    set => SetProperty(Value602Property, value);
  }


  public static readonly PropertyInfo<string> Value603Property = RegisterProperty<string>(nameof(Value603));
  public string Value603
  {
    get => GetProperty(Value603Property);
    set => SetProperty(Value603Property, value);
  }


  public static readonly PropertyInfo<string> Value604Property = RegisterProperty<string>(nameof(Value604));
  public string Value604
  {
    get => GetProperty(Value604Property);
    set => SetProperty(Value604Property, value);
  }


  public static readonly PropertyInfo<string> Value605Property = RegisterProperty<string>(nameof(Value605));
  public string Value605
  {
    get => GetProperty(Value605Property);
    set => SetProperty(Value605Property, value);
  }


  public static readonly PropertyInfo<string> Value606Property = RegisterProperty<string>(nameof(Value606));
  public string Value606
  {
    get => GetProperty(Value606Property);
    set => SetProperty(Value606Property, value);
  }


  public static readonly PropertyInfo<string> Value607Property = RegisterProperty<string>(nameof(Value607));
  public string Value607
  {
    get => GetProperty(Value607Property);
    set => SetProperty(Value607Property, value);
  }


  public static readonly PropertyInfo<string> Value608Property = RegisterProperty<string>(nameof(Value608));
  public string Value608
  {
    get => GetProperty(Value608Property);
    set => SetProperty(Value608Property, value);
  }


  public static readonly PropertyInfo<string> Value609Property = RegisterProperty<string>(nameof(Value609));
  public string Value609
  {
    get => GetProperty(Value609Property);
    set => SetProperty(Value609Property, value);
  }


  public static readonly PropertyInfo<string> Value610Property = RegisterProperty<string>(nameof(Value610));
  public string Value610
  {
    get => GetProperty(Value610Property);
    set => SetProperty(Value610Property, value);
  }


  public static readonly PropertyInfo<string> Value611Property = RegisterProperty<string>(nameof(Value611));
  public string Value611
  {
    get => GetProperty(Value611Property);
    set => SetProperty(Value611Property, value);
  }


  public static readonly PropertyInfo<string> Value612Property = RegisterProperty<string>(nameof(Value612));
  public string Value612
  {
    get => GetProperty(Value612Property);
    set => SetProperty(Value612Property, value);
  }


  public static readonly PropertyInfo<string> Value613Property = RegisterProperty<string>(nameof(Value613));
  public string Value613
  {
    get => GetProperty(Value613Property);
    set => SetProperty(Value613Property, value);
  }


  public static readonly PropertyInfo<string> Value614Property = RegisterProperty<string>(nameof(Value614));
  public string Value614
  {
    get => GetProperty(Value614Property);
    set => SetProperty(Value614Property, value);
  }


  public static readonly PropertyInfo<string> Value615Property = RegisterProperty<string>(nameof(Value615));
  public string Value615
  {
    get => GetProperty(Value615Property);
    set => SetProperty(Value615Property, value);
  }


  public static readonly PropertyInfo<string> Value616Property = RegisterProperty<string>(nameof(Value616));
  public string Value616
  {
    get => GetProperty(Value616Property);
    set => SetProperty(Value616Property, value);
  }


  public static readonly PropertyInfo<string> Value617Property = RegisterProperty<string>(nameof(Value617));
  public string Value617
  {
    get => GetProperty(Value617Property);
    set => SetProperty(Value617Property, value);
  }


  public static readonly PropertyInfo<string> Value618Property = RegisterProperty<string>(nameof(Value618));
  public string Value618
  {
    get => GetProperty(Value618Property);
    set => SetProperty(Value618Property, value);
  }


  public static readonly PropertyInfo<string> Value619Property = RegisterProperty<string>(nameof(Value619));
  public string Value619
  {
    get => GetProperty(Value619Property);
    set => SetProperty(Value619Property, value);
  }


  public static readonly PropertyInfo<string> Value620Property = RegisterProperty<string>(nameof(Value620));
  public string Value620
  {
    get => GetProperty(Value620Property);
    set => SetProperty(Value620Property, value);
  }


  public static readonly PropertyInfo<string> Value621Property = RegisterProperty<string>(nameof(Value621));
  public string Value621
  {
    get => GetProperty(Value621Property);
    set => SetProperty(Value621Property, value);
  }


  public static readonly PropertyInfo<string> Value622Property = RegisterProperty<string>(nameof(Value622));
  public string Value622
  {
    get => GetProperty(Value622Property);
    set => SetProperty(Value622Property, value);
  }


  public static readonly PropertyInfo<string> Value623Property = RegisterProperty<string>(nameof(Value623));
  public string Value623
  {
    get => GetProperty(Value623Property);
    set => SetProperty(Value623Property, value);
  }


  public static readonly PropertyInfo<string> Value624Property = RegisterProperty<string>(nameof(Value624));
  public string Value624
  {
    get => GetProperty(Value624Property);
    set => SetProperty(Value624Property, value);
  }


  public static readonly PropertyInfo<string> Value625Property = RegisterProperty<string>(nameof(Value625));
  public string Value625
  {
    get => GetProperty(Value625Property);
    set => SetProperty(Value625Property, value);
  }


  public static readonly PropertyInfo<string> Value626Property = RegisterProperty<string>(nameof(Value626));
  public string Value626
  {
    get => GetProperty(Value626Property);
    set => SetProperty(Value626Property, value);
  }


  public static readonly PropertyInfo<string> Value627Property = RegisterProperty<string>(nameof(Value627));
  public string Value627
  {
    get => GetProperty(Value627Property);
    set => SetProperty(Value627Property, value);
  }


  public static readonly PropertyInfo<string> Value628Property = RegisterProperty<string>(nameof(Value628));
  public string Value628
  {
    get => GetProperty(Value628Property);
    set => SetProperty(Value628Property, value);
  }


  public static readonly PropertyInfo<string> Value629Property = RegisterProperty<string>(nameof(Value629));
  public string Value629
  {
    get => GetProperty(Value629Property);
    set => SetProperty(Value629Property, value);
  }


  public static readonly PropertyInfo<string> Value630Property = RegisterProperty<string>(nameof(Value630));
  public string Value630
  {
    get => GetProperty(Value630Property);
    set => SetProperty(Value630Property, value);
  }


  public static readonly PropertyInfo<string> Value631Property = RegisterProperty<string>(nameof(Value631));
  public string Value631
  {
    get => GetProperty(Value631Property);
    set => SetProperty(Value631Property, value);
  }


  public static readonly PropertyInfo<string> Value632Property = RegisterProperty<string>(nameof(Value632));
  public string Value632
  {
    get => GetProperty(Value632Property);
    set => SetProperty(Value632Property, value);
  }


  public static readonly PropertyInfo<string> Value633Property = RegisterProperty<string>(nameof(Value633));
  public string Value633
  {
    get => GetProperty(Value633Property);
    set => SetProperty(Value633Property, value);
  }


  public static readonly PropertyInfo<string> Value634Property = RegisterProperty<string>(nameof(Value634));
  public string Value634
  {
    get => GetProperty(Value634Property);
    set => SetProperty(Value634Property, value);
  }


  public static readonly PropertyInfo<string> Value635Property = RegisterProperty<string>(nameof(Value635));
  public string Value635
  {
    get => GetProperty(Value635Property);
    set => SetProperty(Value635Property, value);
  }


  public static readonly PropertyInfo<string> Value636Property = RegisterProperty<string>(nameof(Value636));
  public string Value636
  {
    get => GetProperty(Value636Property);
    set => SetProperty(Value636Property, value);
  }


  public static readonly PropertyInfo<string> Value637Property = RegisterProperty<string>(nameof(Value637));
  public string Value637
  {
    get => GetProperty(Value637Property);
    set => SetProperty(Value637Property, value);
  }


  public static readonly PropertyInfo<string> Value638Property = RegisterProperty<string>(nameof(Value638));
  public string Value638
  {
    get => GetProperty(Value638Property);
    set => SetProperty(Value638Property, value);
  }


  public static readonly PropertyInfo<string> Value639Property = RegisterProperty<string>(nameof(Value639));
  public string Value639
  {
    get => GetProperty(Value639Property);
    set => SetProperty(Value639Property, value);
  }


  public static readonly PropertyInfo<string> Value640Property = RegisterProperty<string>(nameof(Value640));
  public string Value640
  {
    get => GetProperty(Value640Property);
    set => SetProperty(Value640Property, value);
  }


  public static readonly PropertyInfo<string> Value641Property = RegisterProperty<string>(nameof(Value641));
  public string Value641
  {
    get => GetProperty(Value641Property);
    set => SetProperty(Value641Property, value);
  }


  public static readonly PropertyInfo<string> Value642Property = RegisterProperty<string>(nameof(Value642));
  public string Value642
  {
    get => GetProperty(Value642Property);
    set => SetProperty(Value642Property, value);
  }


  public static readonly PropertyInfo<string> Value643Property = RegisterProperty<string>(nameof(Value643));
  public string Value643
  {
    get => GetProperty(Value643Property);
    set => SetProperty(Value643Property, value);
  }


  public static readonly PropertyInfo<string> Value644Property = RegisterProperty<string>(nameof(Value644));
  public string Value644
  {
    get => GetProperty(Value644Property);
    set => SetProperty(Value644Property, value);
  }


  public static readonly PropertyInfo<string> Value645Property = RegisterProperty<string>(nameof(Value645));
  public string Value645
  {
    get => GetProperty(Value645Property);
    set => SetProperty(Value645Property, value);
  }


  public static readonly PropertyInfo<string> Value646Property = RegisterProperty<string>(nameof(Value646));
  public string Value646
  {
    get => GetProperty(Value646Property);
    set => SetProperty(Value646Property, value);
  }


  public static readonly PropertyInfo<string> Value647Property = RegisterProperty<string>(nameof(Value647));
  public string Value647
  {
    get => GetProperty(Value647Property);
    set => SetProperty(Value647Property, value);
  }


  public static readonly PropertyInfo<string> Value648Property = RegisterProperty<string>(nameof(Value648));
  public string Value648
  {
    get => GetProperty(Value648Property);
    set => SetProperty(Value648Property, value);
  }


  public static readonly PropertyInfo<string> Value649Property = RegisterProperty<string>(nameof(Value649));
  public string Value649
  {
    get => GetProperty(Value649Property);
    set => SetProperty(Value649Property, value);
  }


  public static readonly PropertyInfo<string> Value650Property = RegisterProperty<string>(nameof(Value650));
  public string Value650
  {
    get => GetProperty(Value650Property);
    set => SetProperty(Value650Property, value);
  }


  public static readonly PropertyInfo<string> Value651Property = RegisterProperty<string>(nameof(Value651));
  public string Value651
  {
    get => GetProperty(Value651Property);
    set => SetProperty(Value651Property, value);
  }


  public static readonly PropertyInfo<string> Value652Property = RegisterProperty<string>(nameof(Value652));
  public string Value652
  {
    get => GetProperty(Value652Property);
    set => SetProperty(Value652Property, value);
  }


  public static readonly PropertyInfo<string> Value653Property = RegisterProperty<string>(nameof(Value653));
  public string Value653
  {
    get => GetProperty(Value653Property);
    set => SetProperty(Value653Property, value);
  }


  public static readonly PropertyInfo<string> Value654Property = RegisterProperty<string>(nameof(Value654));
  public string Value654
  {
    get => GetProperty(Value654Property);
    set => SetProperty(Value654Property, value);
  }


  public static readonly PropertyInfo<string> Value655Property = RegisterProperty<string>(nameof(Value655));
  public string Value655
  {
    get => GetProperty(Value655Property);
    set => SetProperty(Value655Property, value);
  }


  public static readonly PropertyInfo<string> Value656Property = RegisterProperty<string>(nameof(Value656));
  public string Value656
  {
    get => GetProperty(Value656Property);
    set => SetProperty(Value656Property, value);
  }


  public static readonly PropertyInfo<string> Value657Property = RegisterProperty<string>(nameof(Value657));
  public string Value657
  {
    get => GetProperty(Value657Property);
    set => SetProperty(Value657Property, value);
  }


  public static readonly PropertyInfo<string> Value658Property = RegisterProperty<string>(nameof(Value658));
  public string Value658
  {
    get => GetProperty(Value658Property);
    set => SetProperty(Value658Property, value);
  }


  public static readonly PropertyInfo<string> Value659Property = RegisterProperty<string>(nameof(Value659));
  public string Value659
  {
    get => GetProperty(Value659Property);
    set => SetProperty(Value659Property, value);
  }


  public static readonly PropertyInfo<string> Value660Property = RegisterProperty<string>(nameof(Value660));
  public string Value660
  {
    get => GetProperty(Value660Property);
    set => SetProperty(Value660Property, value);
  }


  public static readonly PropertyInfo<string> Value661Property = RegisterProperty<string>(nameof(Value661));
  public string Value661
  {
    get => GetProperty(Value661Property);
    set => SetProperty(Value661Property, value);
  }


  public static readonly PropertyInfo<string> Value662Property = RegisterProperty<string>(nameof(Value662));
  public string Value662
  {
    get => GetProperty(Value662Property);
    set => SetProperty(Value662Property, value);
  }


  public static readonly PropertyInfo<string> Value663Property = RegisterProperty<string>(nameof(Value663));
  public string Value663
  {
    get => GetProperty(Value663Property);
    set => SetProperty(Value663Property, value);
  }


  public static readonly PropertyInfo<string> Value664Property = RegisterProperty<string>(nameof(Value664));
  public string Value664
  {
    get => GetProperty(Value664Property);
    set => SetProperty(Value664Property, value);
  }


  public static readonly PropertyInfo<string> Value665Property = RegisterProperty<string>(nameof(Value665));
  public string Value665
  {
    get => GetProperty(Value665Property);
    set => SetProperty(Value665Property, value);
  }


  public static readonly PropertyInfo<string> Value666Property = RegisterProperty<string>(nameof(Value666));
  public string Value666
  {
    get => GetProperty(Value666Property);
    set => SetProperty(Value666Property, value);
  }


  public static readonly PropertyInfo<string> Value667Property = RegisterProperty<string>(nameof(Value667));
  public string Value667
  {
    get => GetProperty(Value667Property);
    set => SetProperty(Value667Property, value);
  }


  public static readonly PropertyInfo<string> Value668Property = RegisterProperty<string>(nameof(Value668));
  public string Value668
  {
    get => GetProperty(Value668Property);
    set => SetProperty(Value668Property, value);
  }


  public static readonly PropertyInfo<string> Value669Property = RegisterProperty<string>(nameof(Value669));
  public string Value669
  {
    get => GetProperty(Value669Property);
    set => SetProperty(Value669Property, value);
  }


  public static readonly PropertyInfo<string> Value670Property = RegisterProperty<string>(nameof(Value670));
  public string Value670
  {
    get => GetProperty(Value670Property);
    set => SetProperty(Value670Property, value);
  }


  public static readonly PropertyInfo<string> Value671Property = RegisterProperty<string>(nameof(Value671));
  public string Value671
  {
    get => GetProperty(Value671Property);
    set => SetProperty(Value671Property, value);
  }


  public static readonly PropertyInfo<string> Value672Property = RegisterProperty<string>(nameof(Value672));
  public string Value672
  {
    get => GetProperty(Value672Property);
    set => SetProperty(Value672Property, value);
  }


  public static readonly PropertyInfo<string> Value673Property = RegisterProperty<string>(nameof(Value673));
  public string Value673
  {
    get => GetProperty(Value673Property);
    set => SetProperty(Value673Property, value);
  }


  public static readonly PropertyInfo<string> Value674Property = RegisterProperty<string>(nameof(Value674));
  public string Value674
  {
    get => GetProperty(Value674Property);
    set => SetProperty(Value674Property, value);
  }


  public static readonly PropertyInfo<string> Value675Property = RegisterProperty<string>(nameof(Value675));
  public string Value675
  {
    get => GetProperty(Value675Property);
    set => SetProperty(Value675Property, value);
  }


  public static readonly PropertyInfo<string> Value676Property = RegisterProperty<string>(nameof(Value676));
  public string Value676
  {
    get => GetProperty(Value676Property);
    set => SetProperty(Value676Property, value);
  }


  public static readonly PropertyInfo<string> Value677Property = RegisterProperty<string>(nameof(Value677));
  public string Value677
  {
    get => GetProperty(Value677Property);
    set => SetProperty(Value677Property, value);
  }


  public static readonly PropertyInfo<string> Value678Property = RegisterProperty<string>(nameof(Value678));
  public string Value678
  {
    get => GetProperty(Value678Property);
    set => SetProperty(Value678Property, value);
  }


  public static readonly PropertyInfo<string> Value679Property = RegisterProperty<string>(nameof(Value679));
  public string Value679
  {
    get => GetProperty(Value679Property);
    set => SetProperty(Value679Property, value);
  }


  public static readonly PropertyInfo<string> Value680Property = RegisterProperty<string>(nameof(Value680));
  public string Value680
  {
    get => GetProperty(Value680Property);
    set => SetProperty(Value680Property, value);
  }


  public static readonly PropertyInfo<string> Value681Property = RegisterProperty<string>(nameof(Value681));
  public string Value681
  {
    get => GetProperty(Value681Property);
    set => SetProperty(Value681Property, value);
  }


  public static readonly PropertyInfo<string> Value682Property = RegisterProperty<string>(nameof(Value682));
  public string Value682
  {
    get => GetProperty(Value682Property);
    set => SetProperty(Value682Property, value);
  }


  public static readonly PropertyInfo<string> Value683Property = RegisterProperty<string>(nameof(Value683));
  public string Value683
  {
    get => GetProperty(Value683Property);
    set => SetProperty(Value683Property, value);
  }


  public static readonly PropertyInfo<string> Value684Property = RegisterProperty<string>(nameof(Value684));
  public string Value684
  {
    get => GetProperty(Value684Property);
    set => SetProperty(Value684Property, value);
  }


  public static readonly PropertyInfo<string> Value685Property = RegisterProperty<string>(nameof(Value685));
  public string Value685
  {
    get => GetProperty(Value685Property);
    set => SetProperty(Value685Property, value);
  }


  public static readonly PropertyInfo<string> Value686Property = RegisterProperty<string>(nameof(Value686));
  public string Value686
  {
    get => GetProperty(Value686Property);
    set => SetProperty(Value686Property, value);
  }


  public static readonly PropertyInfo<string> Value687Property = RegisterProperty<string>(nameof(Value687));
  public string Value687
  {
    get => GetProperty(Value687Property);
    set => SetProperty(Value687Property, value);
  }


  public static readonly PropertyInfo<string> Value688Property = RegisterProperty<string>(nameof(Value688));
  public string Value688
  {
    get => GetProperty(Value688Property);
    set => SetProperty(Value688Property, value);
  }


  public static readonly PropertyInfo<string> Value689Property = RegisterProperty<string>(nameof(Value689));
  public string Value689
  {
    get => GetProperty(Value689Property);
    set => SetProperty(Value689Property, value);
  }


  public static readonly PropertyInfo<string> Value690Property = RegisterProperty<string>(nameof(Value690));
  public string Value690
  {
    get => GetProperty(Value690Property);
    set => SetProperty(Value690Property, value);
  }


  public static readonly PropertyInfo<string> Value691Property = RegisterProperty<string>(nameof(Value691));
  public string Value691
  {
    get => GetProperty(Value691Property);
    set => SetProperty(Value691Property, value);
  }


  public static readonly PropertyInfo<string> Value692Property = RegisterProperty<string>(nameof(Value692));
  public string Value692
  {
    get => GetProperty(Value692Property);
    set => SetProperty(Value692Property, value);
  }


  public static readonly PropertyInfo<string> Value693Property = RegisterProperty<string>(nameof(Value693));
  public string Value693
  {
    get => GetProperty(Value693Property);
    set => SetProperty(Value693Property, value);
  }


  public static readonly PropertyInfo<string> Value694Property = RegisterProperty<string>(nameof(Value694));
  public string Value694
  {
    get => GetProperty(Value694Property);
    set => SetProperty(Value694Property, value);
  }


  public static readonly PropertyInfo<string> Value695Property = RegisterProperty<string>(nameof(Value695));
  public string Value695
  {
    get => GetProperty(Value695Property);
    set => SetProperty(Value695Property, value);
  }


  public static readonly PropertyInfo<string> Value696Property = RegisterProperty<string>(nameof(Value696));
  public string Value696
  {
    get => GetProperty(Value696Property);
    set => SetProperty(Value696Property, value);
  }


  public static readonly PropertyInfo<string> Value697Property = RegisterProperty<string>(nameof(Value697));
  public string Value697
  {
    get => GetProperty(Value697Property);
    set => SetProperty(Value697Property, value);
  }


  public static readonly PropertyInfo<string> Value698Property = RegisterProperty<string>(nameof(Value698));
  public string Value698
  {
    get => GetProperty(Value698Property);
    set => SetProperty(Value698Property, value);
  }


  public static readonly PropertyInfo<string> Value699Property = RegisterProperty<string>(nameof(Value699));
  public string Value699
  {
    get => GetProperty(Value699Property);
    set => SetProperty(Value699Property, value);
  }


  public static readonly PropertyInfo<string> Value700Property = RegisterProperty<string>(nameof(Value700));
  public string Value700
  {
    get => GetProperty(Value700Property);
    set => SetProperty(Value700Property, value);
  }


  public static readonly PropertyInfo<string> Value701Property = RegisterProperty<string>(nameof(Value701));
  public string Value701
  {
    get => GetProperty(Value701Property);
    set => SetProperty(Value701Property, value);
  }


  public static readonly PropertyInfo<string> Value702Property = RegisterProperty<string>(nameof(Value702));
  public string Value702
  {
    get => GetProperty(Value702Property);
    set => SetProperty(Value702Property, value);
  }


  public static readonly PropertyInfo<string> Value703Property = RegisterProperty<string>(nameof(Value703));
  public string Value703
  {
    get => GetProperty(Value703Property);
    set => SetProperty(Value703Property, value);
  }


  public static readonly PropertyInfo<string> Value704Property = RegisterProperty<string>(nameof(Value704));
  public string Value704
  {
    get => GetProperty(Value704Property);
    set => SetProperty(Value704Property, value);
  }


  public static readonly PropertyInfo<string> Value705Property = RegisterProperty<string>(nameof(Value705));
  public string Value705
  {
    get => GetProperty(Value705Property);
    set => SetProperty(Value705Property, value);
  }


  public static readonly PropertyInfo<string> Value706Property = RegisterProperty<string>(nameof(Value706));
  public string Value706
  {
    get => GetProperty(Value706Property);
    set => SetProperty(Value706Property, value);
  }


  public static readonly PropertyInfo<string> Value707Property = RegisterProperty<string>(nameof(Value707));
  public string Value707
  {
    get => GetProperty(Value707Property);
    set => SetProperty(Value707Property, value);
  }


  public static readonly PropertyInfo<string> Value708Property = RegisterProperty<string>(nameof(Value708));
  public string Value708
  {
    get => GetProperty(Value708Property);
    set => SetProperty(Value708Property, value);
  }


  public static readonly PropertyInfo<string> Value709Property = RegisterProperty<string>(nameof(Value709));
  public string Value709
  {
    get => GetProperty(Value709Property);
    set => SetProperty(Value709Property, value);
  }


  public static readonly PropertyInfo<string> Value710Property = RegisterProperty<string>(nameof(Value710));
  public string Value710
  {
    get => GetProperty(Value710Property);
    set => SetProperty(Value710Property, value);
  }


  public static readonly PropertyInfo<string> Value711Property = RegisterProperty<string>(nameof(Value711));
  public string Value711
  {
    get => GetProperty(Value711Property);
    set => SetProperty(Value711Property, value);
  }


  public static readonly PropertyInfo<string> Value712Property = RegisterProperty<string>(nameof(Value712));
  public string Value712
  {
    get => GetProperty(Value712Property);
    set => SetProperty(Value712Property, value);
  }


  public static readonly PropertyInfo<string> Value713Property = RegisterProperty<string>(nameof(Value713));
  public string Value713
  {
    get => GetProperty(Value713Property);
    set => SetProperty(Value713Property, value);
  }


  public static readonly PropertyInfo<string> Value714Property = RegisterProperty<string>(nameof(Value714));
  public string Value714
  {
    get => GetProperty(Value714Property);
    set => SetProperty(Value714Property, value);
  }


  public static readonly PropertyInfo<string> Value715Property = RegisterProperty<string>(nameof(Value715));
  public string Value715
  {
    get => GetProperty(Value715Property);
    set => SetProperty(Value715Property, value);
  }


  public static readonly PropertyInfo<string> Value716Property = RegisterProperty<string>(nameof(Value716));
  public string Value716
  {
    get => GetProperty(Value716Property);
    set => SetProperty(Value716Property, value);
  }


  public static readonly PropertyInfo<string> Value717Property = RegisterProperty<string>(nameof(Value717));
  public string Value717
  {
    get => GetProperty(Value717Property);
    set => SetProperty(Value717Property, value);
  }


  public static readonly PropertyInfo<string> Value718Property = RegisterProperty<string>(nameof(Value718));
  public string Value718
  {
    get => GetProperty(Value718Property);
    set => SetProperty(Value718Property, value);
  }


  public static readonly PropertyInfo<string> Value719Property = RegisterProperty<string>(nameof(Value719));
  public string Value719
  {
    get => GetProperty(Value719Property);
    set => SetProperty(Value719Property, value);
  }


  public static readonly PropertyInfo<string> Value720Property = RegisterProperty<string>(nameof(Value720));
  public string Value720
  {
    get => GetProperty(Value720Property);
    set => SetProperty(Value720Property, value);
  }


  public static readonly PropertyInfo<string> Value721Property = RegisterProperty<string>(nameof(Value721));
  public string Value721
  {
    get => GetProperty(Value721Property);
    set => SetProperty(Value721Property, value);
  }


  public static readonly PropertyInfo<string> Value722Property = RegisterProperty<string>(nameof(Value722));
  public string Value722
  {
    get => GetProperty(Value722Property);
    set => SetProperty(Value722Property, value);
  }


  public static readonly PropertyInfo<string> Value723Property = RegisterProperty<string>(nameof(Value723));
  public string Value723
  {
    get => GetProperty(Value723Property);
    set => SetProperty(Value723Property, value);
  }


  public static readonly PropertyInfo<string> Value724Property = RegisterProperty<string>(nameof(Value724));
  public string Value724
  {
    get => GetProperty(Value724Property);
    set => SetProperty(Value724Property, value);
  }


  public static readonly PropertyInfo<string> Value725Property = RegisterProperty<string>(nameof(Value725));
  public string Value725
  {
    get => GetProperty(Value725Property);
    set => SetProperty(Value725Property, value);
  }


  public static readonly PropertyInfo<string> Value726Property = RegisterProperty<string>(nameof(Value726));
  public string Value726
  {
    get => GetProperty(Value726Property);
    set => SetProperty(Value726Property, value);
  }


  public static readonly PropertyInfo<string> Value727Property = RegisterProperty<string>(nameof(Value727));
  public string Value727
  {
    get => GetProperty(Value727Property);
    set => SetProperty(Value727Property, value);
  }


  public static readonly PropertyInfo<string> Value728Property = RegisterProperty<string>(nameof(Value728));
  public string Value728
  {
    get => GetProperty(Value728Property);
    set => SetProperty(Value728Property, value);
  }


  public static readonly PropertyInfo<string> Value729Property = RegisterProperty<string>(nameof(Value729));
  public string Value729
  {
    get => GetProperty(Value729Property);
    set => SetProperty(Value729Property, value);
  }


  public static readonly PropertyInfo<string> Value730Property = RegisterProperty<string>(nameof(Value730));
  public string Value730
  {
    get => GetProperty(Value730Property);
    set => SetProperty(Value730Property, value);
  }


  public static readonly PropertyInfo<string> Value731Property = RegisterProperty<string>(nameof(Value731));
  public string Value731
  {
    get => GetProperty(Value731Property);
    set => SetProperty(Value731Property, value);
  }


  public static readonly PropertyInfo<string> Value732Property = RegisterProperty<string>(nameof(Value732));
  public string Value732
  {
    get => GetProperty(Value732Property);
    set => SetProperty(Value732Property, value);
  }


  public static readonly PropertyInfo<string> Value733Property = RegisterProperty<string>(nameof(Value733));
  public string Value733
  {
    get => GetProperty(Value733Property);
    set => SetProperty(Value733Property, value);
  }


  public static readonly PropertyInfo<string> Value734Property = RegisterProperty<string>(nameof(Value734));
  public string Value734
  {
    get => GetProperty(Value734Property);
    set => SetProperty(Value734Property, value);
  }


  public static readonly PropertyInfo<string> Value735Property = RegisterProperty<string>(nameof(Value735));
  public string Value735
  {
    get => GetProperty(Value735Property);
    set => SetProperty(Value735Property, value);
  }


  public static readonly PropertyInfo<string> Value736Property = RegisterProperty<string>(nameof(Value736));
  public string Value736
  {
    get => GetProperty(Value736Property);
    set => SetProperty(Value736Property, value);
  }


  public static readonly PropertyInfo<string> Value737Property = RegisterProperty<string>(nameof(Value737));
  public string Value737
  {
    get => GetProperty(Value737Property);
    set => SetProperty(Value737Property, value);
  }


  public static readonly PropertyInfo<string> Value738Property = RegisterProperty<string>(nameof(Value738));
  public string Value738
  {
    get => GetProperty(Value738Property);
    set => SetProperty(Value738Property, value);
  }


  public static readonly PropertyInfo<string> Value739Property = RegisterProperty<string>(nameof(Value739));
  public string Value739
  {
    get => GetProperty(Value739Property);
    set => SetProperty(Value739Property, value);
  }


  public static readonly PropertyInfo<string> Value740Property = RegisterProperty<string>(nameof(Value740));
  public string Value740
  {
    get => GetProperty(Value740Property);
    set => SetProperty(Value740Property, value);
  }


  public static readonly PropertyInfo<string> Value741Property = RegisterProperty<string>(nameof(Value741));
  public string Value741
  {
    get => GetProperty(Value741Property);
    set => SetProperty(Value741Property, value);
  }


  public static readonly PropertyInfo<string> Value742Property = RegisterProperty<string>(nameof(Value742));
  public string Value742
  {
    get => GetProperty(Value742Property);
    set => SetProperty(Value742Property, value);
  }


  public static readonly PropertyInfo<string> Value743Property = RegisterProperty<string>(nameof(Value743));
  public string Value743
  {
    get => GetProperty(Value743Property);
    set => SetProperty(Value743Property, value);
  }


  public static readonly PropertyInfo<string> Value744Property = RegisterProperty<string>(nameof(Value744));
  public string Value744
  {
    get => GetProperty(Value744Property);
    set => SetProperty(Value744Property, value);
  }


  public static readonly PropertyInfo<string> Value745Property = RegisterProperty<string>(nameof(Value745));
  public string Value745
  {
    get => GetProperty(Value745Property);
    set => SetProperty(Value745Property, value);
  }


  public static readonly PropertyInfo<string> Value746Property = RegisterProperty<string>(nameof(Value746));
  public string Value746
  {
    get => GetProperty(Value746Property);
    set => SetProperty(Value746Property, value);
  }


  public static readonly PropertyInfo<string> Value747Property = RegisterProperty<string>(nameof(Value747));
  public string Value747
  {
    get => GetProperty(Value747Property);
    set => SetProperty(Value747Property, value);
  }


  public static readonly PropertyInfo<string> Value748Property = RegisterProperty<string>(nameof(Value748));
  public string Value748
  {
    get => GetProperty(Value748Property);
    set => SetProperty(Value748Property, value);
  }


  public static readonly PropertyInfo<string> Value749Property = RegisterProperty<string>(nameof(Value749));
  public string Value749
  {
    get => GetProperty(Value749Property);
    set => SetProperty(Value749Property, value);
  }


  public static readonly PropertyInfo<string> Value750Property = RegisterProperty<string>(nameof(Value750));
  public string Value750
  {
    get => GetProperty(Value750Property);
    set => SetProperty(Value750Property, value);
  }


  public static readonly PropertyInfo<string> Value751Property = RegisterProperty<string>(nameof(Value751));
  public string Value751
  {
    get => GetProperty(Value751Property);
    set => SetProperty(Value751Property, value);
  }


  public static readonly PropertyInfo<string> Value752Property = RegisterProperty<string>(nameof(Value752));
  public string Value752
  {
    get => GetProperty(Value752Property);
    set => SetProperty(Value752Property, value);
  }


  public static readonly PropertyInfo<string> Value753Property = RegisterProperty<string>(nameof(Value753));
  public string Value753
  {
    get => GetProperty(Value753Property);
    set => SetProperty(Value753Property, value);
  }


  public static readonly PropertyInfo<string> Value754Property = RegisterProperty<string>(nameof(Value754));
  public string Value754
  {
    get => GetProperty(Value754Property);
    set => SetProperty(Value754Property, value);
  }


  public static readonly PropertyInfo<string> Value755Property = RegisterProperty<string>(nameof(Value755));
  public string Value755
  {
    get => GetProperty(Value755Property);
    set => SetProperty(Value755Property, value);
  }


  public static readonly PropertyInfo<string> Value756Property = RegisterProperty<string>(nameof(Value756));
  public string Value756
  {
    get => GetProperty(Value756Property);
    set => SetProperty(Value756Property, value);
  }


  public static readonly PropertyInfo<string> Value757Property = RegisterProperty<string>(nameof(Value757));
  public string Value757
  {
    get => GetProperty(Value757Property);
    set => SetProperty(Value757Property, value);
  }


  public static readonly PropertyInfo<string> Value758Property = RegisterProperty<string>(nameof(Value758));
  public string Value758
  {
    get => GetProperty(Value758Property);
    set => SetProperty(Value758Property, value);
  }


  public static readonly PropertyInfo<string> Value759Property = RegisterProperty<string>(nameof(Value759));
  public string Value759
  {
    get => GetProperty(Value759Property);
    set => SetProperty(Value759Property, value);
  }


  public static readonly PropertyInfo<string> Value760Property = RegisterProperty<string>(nameof(Value760));
  public string Value760
  {
    get => GetProperty(Value760Property);
    set => SetProperty(Value760Property, value);
  }


  public static readonly PropertyInfo<string> Value761Property = RegisterProperty<string>(nameof(Value761));
  public string Value761
  {
    get => GetProperty(Value761Property);
    set => SetProperty(Value761Property, value);
  }


  public static readonly PropertyInfo<string> Value762Property = RegisterProperty<string>(nameof(Value762));
  public string Value762
  {
    get => GetProperty(Value762Property);
    set => SetProperty(Value762Property, value);
  }


  public static readonly PropertyInfo<string> Value763Property = RegisterProperty<string>(nameof(Value763));
  public string Value763
  {
    get => GetProperty(Value763Property);
    set => SetProperty(Value763Property, value);
  }


  public static readonly PropertyInfo<string> Value764Property = RegisterProperty<string>(nameof(Value764));
  public string Value764
  {
    get => GetProperty(Value764Property);
    set => SetProperty(Value764Property, value);
  }


  public static readonly PropertyInfo<string> Value765Property = RegisterProperty<string>(nameof(Value765));
  public string Value765
  {
    get => GetProperty(Value765Property);
    set => SetProperty(Value765Property, value);
  }


  public static readonly PropertyInfo<string> Value766Property = RegisterProperty<string>(nameof(Value766));
  public string Value766
  {
    get => GetProperty(Value766Property);
    set => SetProperty(Value766Property, value);
  }


  public static readonly PropertyInfo<string> Value767Property = RegisterProperty<string>(nameof(Value767));
  public string Value767
  {
    get => GetProperty(Value767Property);
    set => SetProperty(Value767Property, value);
  }


  public static readonly PropertyInfo<string> Value768Property = RegisterProperty<string>(nameof(Value768));
  public string Value768
  {
    get => GetProperty(Value768Property);
    set => SetProperty(Value768Property, value);
  }


  public static readonly PropertyInfo<string> Value769Property = RegisterProperty<string>(nameof(Value769));
  public string Value769
  {
    get => GetProperty(Value769Property);
    set => SetProperty(Value769Property, value);
  }


  public static readonly PropertyInfo<string> Value770Property = RegisterProperty<string>(nameof(Value770));
  public string Value770
  {
    get => GetProperty(Value770Property);
    set => SetProperty(Value770Property, value);
  }


  public static readonly PropertyInfo<string> Value771Property = RegisterProperty<string>(nameof(Value771));
  public string Value771
  {
    get => GetProperty(Value771Property);
    set => SetProperty(Value771Property, value);
  }


  public static readonly PropertyInfo<string> Value772Property = RegisterProperty<string>(nameof(Value772));
  public string Value772
  {
    get => GetProperty(Value772Property);
    set => SetProperty(Value772Property, value);
  }


  public static readonly PropertyInfo<string> Value773Property = RegisterProperty<string>(nameof(Value773));
  public string Value773
  {
    get => GetProperty(Value773Property);
    set => SetProperty(Value773Property, value);
  }


  public static readonly PropertyInfo<string> Value774Property = RegisterProperty<string>(nameof(Value774));
  public string Value774
  {
    get => GetProperty(Value774Property);
    set => SetProperty(Value774Property, value);
  }


  public static readonly PropertyInfo<string> Value775Property = RegisterProperty<string>(nameof(Value775));
  public string Value775
  {
    get => GetProperty(Value775Property);
    set => SetProperty(Value775Property, value);
  }


  public static readonly PropertyInfo<string> Value776Property = RegisterProperty<string>(nameof(Value776));
  public string Value776
  {
    get => GetProperty(Value776Property);
    set => SetProperty(Value776Property, value);
  }


  public static readonly PropertyInfo<string> Value777Property = RegisterProperty<string>(nameof(Value777));
  public string Value777
  {
    get => GetProperty(Value777Property);
    set => SetProperty(Value777Property, value);
  }


  public static readonly PropertyInfo<string> Value778Property = RegisterProperty<string>(nameof(Value778));
  public string Value778
  {
    get => GetProperty(Value778Property);
    set => SetProperty(Value778Property, value);
  }


  public static readonly PropertyInfo<string> Value779Property = RegisterProperty<string>(nameof(Value779));
  public string Value779
  {
    get => GetProperty(Value779Property);
    set => SetProperty(Value779Property, value);
  }


  public static readonly PropertyInfo<string> Value780Property = RegisterProperty<string>(nameof(Value780));
  public string Value780
  {
    get => GetProperty(Value780Property);
    set => SetProperty(Value780Property, value);
  }


  public static readonly PropertyInfo<string> Value781Property = RegisterProperty<string>(nameof(Value781));
  public string Value781
  {
    get => GetProperty(Value781Property);
    set => SetProperty(Value781Property, value);
  }


  public static readonly PropertyInfo<string> Value782Property = RegisterProperty<string>(nameof(Value782));
  public string Value782
  {
    get => GetProperty(Value782Property);
    set => SetProperty(Value782Property, value);
  }


  public static readonly PropertyInfo<string> Value783Property = RegisterProperty<string>(nameof(Value783));
  public string Value783
  {
    get => GetProperty(Value783Property);
    set => SetProperty(Value783Property, value);
  }


  public static readonly PropertyInfo<string> Value784Property = RegisterProperty<string>(nameof(Value784));
  public string Value784
  {
    get => GetProperty(Value784Property);
    set => SetProperty(Value784Property, value);
  }


  public static readonly PropertyInfo<string> Value785Property = RegisterProperty<string>(nameof(Value785));
  public string Value785
  {
    get => GetProperty(Value785Property);
    set => SetProperty(Value785Property, value);
  }


  public static readonly PropertyInfo<string> Value786Property = RegisterProperty<string>(nameof(Value786));
  public string Value786
  {
    get => GetProperty(Value786Property);
    set => SetProperty(Value786Property, value);
  }


  public static readonly PropertyInfo<string> Value787Property = RegisterProperty<string>(nameof(Value787));
  public string Value787
  {
    get => GetProperty(Value787Property);
    set => SetProperty(Value787Property, value);
  }


  public static readonly PropertyInfo<string> Value788Property = RegisterProperty<string>(nameof(Value788));
  public string Value788
  {
    get => GetProperty(Value788Property);
    set => SetProperty(Value788Property, value);
  }


  public static readonly PropertyInfo<string> Value789Property = RegisterProperty<string>(nameof(Value789));
  public string Value789
  {
    get => GetProperty(Value789Property);
    set => SetProperty(Value789Property, value);
  }


  public static readonly PropertyInfo<string> Value790Property = RegisterProperty<string>(nameof(Value790));
  public string Value790
  {
    get => GetProperty(Value790Property);
    set => SetProperty(Value790Property, value);
  }


  public static readonly PropertyInfo<string> Value791Property = RegisterProperty<string>(nameof(Value791));
  public string Value791
  {
    get => GetProperty(Value791Property);
    set => SetProperty(Value791Property, value);
  }


  public static readonly PropertyInfo<string> Value792Property = RegisterProperty<string>(nameof(Value792));
  public string Value792
  {
    get => GetProperty(Value792Property);
    set => SetProperty(Value792Property, value);
  }


  public static readonly PropertyInfo<string> Value793Property = RegisterProperty<string>(nameof(Value793));
  public string Value793
  {
    get => GetProperty(Value793Property);
    set => SetProperty(Value793Property, value);
  }


  public static readonly PropertyInfo<string> Value794Property = RegisterProperty<string>(nameof(Value794));
  public string Value794
  {
    get => GetProperty(Value794Property);
    set => SetProperty(Value794Property, value);
  }


  public static readonly PropertyInfo<string> Value795Property = RegisterProperty<string>(nameof(Value795));
  public string Value795
  {
    get => GetProperty(Value795Property);
    set => SetProperty(Value795Property, value);
  }


  public static readonly PropertyInfo<string> Value796Property = RegisterProperty<string>(nameof(Value796));
  public string Value796
  {
    get => GetProperty(Value796Property);
    set => SetProperty(Value796Property, value);
  }


  public static readonly PropertyInfo<string> Value797Property = RegisterProperty<string>(nameof(Value797));
  public string Value797
  {
    get => GetProperty(Value797Property);
    set => SetProperty(Value797Property, value);
  }


  public static readonly PropertyInfo<string> Value798Property = RegisterProperty<string>(nameof(Value798));
  public string Value798
  {
    get => GetProperty(Value798Property);
    set => SetProperty(Value798Property, value);
  }


  public static readonly PropertyInfo<string> Value799Property = RegisterProperty<string>(nameof(Value799));
  public string Value799
  {
    get => GetProperty(Value799Property);
    set => SetProperty(Value799Property, value);
  }


  public static readonly PropertyInfo<string> Value800Property = RegisterProperty<string>(nameof(Value800));
  public string Value800
  {
    get => GetProperty(Value800Property);
    set => SetProperty(Value800Property, value);
  }


  public static readonly PropertyInfo<string> Value801Property = RegisterProperty<string>(nameof(Value801));
  public string Value801
  {
    get => GetProperty(Value801Property);
    set => SetProperty(Value801Property, value);
  }


  public static readonly PropertyInfo<string> Value802Property = RegisterProperty<string>(nameof(Value802));
  public string Value802
  {
    get => GetProperty(Value802Property);
    set => SetProperty(Value802Property, value);
  }


  public static readonly PropertyInfo<string> Value803Property = RegisterProperty<string>(nameof(Value803));
  public string Value803
  {
    get => GetProperty(Value803Property);
    set => SetProperty(Value803Property, value);
  }


  public static readonly PropertyInfo<string> Value804Property = RegisterProperty<string>(nameof(Value804));
  public string Value804
  {
    get => GetProperty(Value804Property);
    set => SetProperty(Value804Property, value);
  }


  public static readonly PropertyInfo<string> Value805Property = RegisterProperty<string>(nameof(Value805));
  public string Value805
  {
    get => GetProperty(Value805Property);
    set => SetProperty(Value805Property, value);
  }


  public static readonly PropertyInfo<string> Value806Property = RegisterProperty<string>(nameof(Value806));
  public string Value806
  {
    get => GetProperty(Value806Property);
    set => SetProperty(Value806Property, value);
  }


  public static readonly PropertyInfo<string> Value807Property = RegisterProperty<string>(nameof(Value807));
  public string Value807
  {
    get => GetProperty(Value807Property);
    set => SetProperty(Value807Property, value);
  }


  public static readonly PropertyInfo<string> Value808Property = RegisterProperty<string>(nameof(Value808));
  public string Value808
  {
    get => GetProperty(Value808Property);
    set => SetProperty(Value808Property, value);
  }


  public static readonly PropertyInfo<string> Value809Property = RegisterProperty<string>(nameof(Value809));
  public string Value809
  {
    get => GetProperty(Value809Property);
    set => SetProperty(Value809Property, value);
  }


  public static readonly PropertyInfo<string> Value810Property = RegisterProperty<string>(nameof(Value810));
  public string Value810
  {
    get => GetProperty(Value810Property);
    set => SetProperty(Value810Property, value);
  }


  public static readonly PropertyInfo<string> Value811Property = RegisterProperty<string>(nameof(Value811));
  public string Value811
  {
    get => GetProperty(Value811Property);
    set => SetProperty(Value811Property, value);
  }


  public static readonly PropertyInfo<string> Value812Property = RegisterProperty<string>(nameof(Value812));
  public string Value812
  {
    get => GetProperty(Value812Property);
    set => SetProperty(Value812Property, value);
  }


  public static readonly PropertyInfo<string> Value813Property = RegisterProperty<string>(nameof(Value813));
  public string Value813
  {
    get => GetProperty(Value813Property);
    set => SetProperty(Value813Property, value);
  }


  public static readonly PropertyInfo<string> Value814Property = RegisterProperty<string>(nameof(Value814));
  public string Value814
  {
    get => GetProperty(Value814Property);
    set => SetProperty(Value814Property, value);
  }


  public static readonly PropertyInfo<string> Value815Property = RegisterProperty<string>(nameof(Value815));
  public string Value815
  {
    get => GetProperty(Value815Property);
    set => SetProperty(Value815Property, value);
  }


  public static readonly PropertyInfo<string> Value816Property = RegisterProperty<string>(nameof(Value816));
  public string Value816
  {
    get => GetProperty(Value816Property);
    set => SetProperty(Value816Property, value);
  }


  public static readonly PropertyInfo<string> Value817Property = RegisterProperty<string>(nameof(Value817));
  public string Value817
  {
    get => GetProperty(Value817Property);
    set => SetProperty(Value817Property, value);
  }


  public static readonly PropertyInfo<string> Value818Property = RegisterProperty<string>(nameof(Value818));
  public string Value818
  {
    get => GetProperty(Value818Property);
    set => SetProperty(Value818Property, value);
  }


  public static readonly PropertyInfo<string> Value819Property = RegisterProperty<string>(nameof(Value819));
  public string Value819
  {
    get => GetProperty(Value819Property);
    set => SetProperty(Value819Property, value);
  }


  public static readonly PropertyInfo<string> Value820Property = RegisterProperty<string>(nameof(Value820));
  public string Value820
  {
    get => GetProperty(Value820Property);
    set => SetProperty(Value820Property, value);
  }


  public static readonly PropertyInfo<string> Value821Property = RegisterProperty<string>(nameof(Value821));
  public string Value821
  {
    get => GetProperty(Value821Property);
    set => SetProperty(Value821Property, value);
  }


  public static readonly PropertyInfo<string> Value822Property = RegisterProperty<string>(nameof(Value822));
  public string Value822
  {
    get => GetProperty(Value822Property);
    set => SetProperty(Value822Property, value);
  }


  public static readonly PropertyInfo<string> Value823Property = RegisterProperty<string>(nameof(Value823));
  public string Value823
  {
    get => GetProperty(Value823Property);
    set => SetProperty(Value823Property, value);
  }


  public static readonly PropertyInfo<string> Value824Property = RegisterProperty<string>(nameof(Value824));
  public string Value824
  {
    get => GetProperty(Value824Property);
    set => SetProperty(Value824Property, value);
  }


  public static readonly PropertyInfo<string> Value825Property = RegisterProperty<string>(nameof(Value825));
  public string Value825
  {
    get => GetProperty(Value825Property);
    set => SetProperty(Value825Property, value);
  }


  public static readonly PropertyInfo<string> Value826Property = RegisterProperty<string>(nameof(Value826));
  public string Value826
  {
    get => GetProperty(Value826Property);
    set => SetProperty(Value826Property, value);
  }


  public static readonly PropertyInfo<string> Value827Property = RegisterProperty<string>(nameof(Value827));
  public string Value827
  {
    get => GetProperty(Value827Property);
    set => SetProperty(Value827Property, value);
  }


  public static readonly PropertyInfo<string> Value828Property = RegisterProperty<string>(nameof(Value828));
  public string Value828
  {
    get => GetProperty(Value828Property);
    set => SetProperty(Value828Property, value);
  }


  public static readonly PropertyInfo<string> Value829Property = RegisterProperty<string>(nameof(Value829));
  public string Value829
  {
    get => GetProperty(Value829Property);
    set => SetProperty(Value829Property, value);
  }


  public static readonly PropertyInfo<string> Value830Property = RegisterProperty<string>(nameof(Value830));
  public string Value830
  {
    get => GetProperty(Value830Property);
    set => SetProperty(Value830Property, value);
  }


  public static readonly PropertyInfo<string> Value831Property = RegisterProperty<string>(nameof(Value831));
  public string Value831
  {
    get => GetProperty(Value831Property);
    set => SetProperty(Value831Property, value);
  }


  public static readonly PropertyInfo<string> Value832Property = RegisterProperty<string>(nameof(Value832));
  public string Value832
  {
    get => GetProperty(Value832Property);
    set => SetProperty(Value832Property, value);
  }


  public static readonly PropertyInfo<string> Value833Property = RegisterProperty<string>(nameof(Value833));
  public string Value833
  {
    get => GetProperty(Value833Property);
    set => SetProperty(Value833Property, value);
  }


  public static readonly PropertyInfo<string> Value834Property = RegisterProperty<string>(nameof(Value834));
  public string Value834
  {
    get => GetProperty(Value834Property);
    set => SetProperty(Value834Property, value);
  }


  public static readonly PropertyInfo<string> Value835Property = RegisterProperty<string>(nameof(Value835));
  public string Value835
  {
    get => GetProperty(Value835Property);
    set => SetProperty(Value835Property, value);
  }


  public static readonly PropertyInfo<string> Value836Property = RegisterProperty<string>(nameof(Value836));
  public string Value836
  {
    get => GetProperty(Value836Property);
    set => SetProperty(Value836Property, value);
  }


  public static readonly PropertyInfo<string> Value837Property = RegisterProperty<string>(nameof(Value837));
  public string Value837
  {
    get => GetProperty(Value837Property);
    set => SetProperty(Value837Property, value);
  }


  public static readonly PropertyInfo<string> Value838Property = RegisterProperty<string>(nameof(Value838));
  public string Value838
  {
    get => GetProperty(Value838Property);
    set => SetProperty(Value838Property, value);
  }


  public static readonly PropertyInfo<string> Value839Property = RegisterProperty<string>(nameof(Value839));
  public string Value839
  {
    get => GetProperty(Value839Property);
    set => SetProperty(Value839Property, value);
  }


  public static readonly PropertyInfo<string> Value840Property = RegisterProperty<string>(nameof(Value840));
  public string Value840
  {
    get => GetProperty(Value840Property);
    set => SetProperty(Value840Property, value);
  }


  public static readonly PropertyInfo<string> Value841Property = RegisterProperty<string>(nameof(Value841));
  public string Value841
  {
    get => GetProperty(Value841Property);
    set => SetProperty(Value841Property, value);
  }


  public static readonly PropertyInfo<string> Value842Property = RegisterProperty<string>(nameof(Value842));
  public string Value842
  {
    get => GetProperty(Value842Property);
    set => SetProperty(Value842Property, value);
  }


  public static readonly PropertyInfo<string> Value843Property = RegisterProperty<string>(nameof(Value843));
  public string Value843
  {
    get => GetProperty(Value843Property);
    set => SetProperty(Value843Property, value);
  }


  public static readonly PropertyInfo<string> Value844Property = RegisterProperty<string>(nameof(Value844));
  public string Value844
  {
    get => GetProperty(Value844Property);
    set => SetProperty(Value844Property, value);
  }


  public static readonly PropertyInfo<string> Value845Property = RegisterProperty<string>(nameof(Value845));
  public string Value845
  {
    get => GetProperty(Value845Property);
    set => SetProperty(Value845Property, value);
  }


  public static readonly PropertyInfo<string> Value846Property = RegisterProperty<string>(nameof(Value846));
  public string Value846
  {
    get => GetProperty(Value846Property);
    set => SetProperty(Value846Property, value);
  }


  public static readonly PropertyInfo<string> Value847Property = RegisterProperty<string>(nameof(Value847));
  public string Value847
  {
    get => GetProperty(Value847Property);
    set => SetProperty(Value847Property, value);
  }


  public static readonly PropertyInfo<string> Value848Property = RegisterProperty<string>(nameof(Value848));
  public string Value848
  {
    get => GetProperty(Value848Property);
    set => SetProperty(Value848Property, value);
  }


  public static readonly PropertyInfo<string> Value849Property = RegisterProperty<string>(nameof(Value849));
  public string Value849
  {
    get => GetProperty(Value849Property);
    set => SetProperty(Value849Property, value);
  }


  public static readonly PropertyInfo<string> Value850Property = RegisterProperty<string>(nameof(Value850));
  public string Value850
  {
    get => GetProperty(Value850Property);
    set => SetProperty(Value850Property, value);
  }


  public static readonly PropertyInfo<string> Value851Property = RegisterProperty<string>(nameof(Value851));
  public string Value851
  {
    get => GetProperty(Value851Property);
    set => SetProperty(Value851Property, value);
  }


  public static readonly PropertyInfo<string> Value852Property = RegisterProperty<string>(nameof(Value852));
  public string Value852
  {
    get => GetProperty(Value852Property);
    set => SetProperty(Value852Property, value);
  }


  public static readonly PropertyInfo<string> Value853Property = RegisterProperty<string>(nameof(Value853));
  public string Value853
  {
    get => GetProperty(Value853Property);
    set => SetProperty(Value853Property, value);
  }


  public static readonly PropertyInfo<string> Value854Property = RegisterProperty<string>(nameof(Value854));
  public string Value854
  {
    get => GetProperty(Value854Property);
    set => SetProperty(Value854Property, value);
  }


  public static readonly PropertyInfo<string> Value855Property = RegisterProperty<string>(nameof(Value855));
  public string Value855
  {
    get => GetProperty(Value855Property);
    set => SetProperty(Value855Property, value);
  }


  public static readonly PropertyInfo<string> Value856Property = RegisterProperty<string>(nameof(Value856));
  public string Value856
  {
    get => GetProperty(Value856Property);
    set => SetProperty(Value856Property, value);
  }


  public static readonly PropertyInfo<string> Value857Property = RegisterProperty<string>(nameof(Value857));
  public string Value857
  {
    get => GetProperty(Value857Property);
    set => SetProperty(Value857Property, value);
  }


  public static readonly PropertyInfo<string> Value858Property = RegisterProperty<string>(nameof(Value858));
  public string Value858
  {
    get => GetProperty(Value858Property);
    set => SetProperty(Value858Property, value);
  }


  public static readonly PropertyInfo<string> Value859Property = RegisterProperty<string>(nameof(Value859));
  public string Value859
  {
    get => GetProperty(Value859Property);
    set => SetProperty(Value859Property, value);
  }


  public static readonly PropertyInfo<string> Value860Property = RegisterProperty<string>(nameof(Value860));
  public string Value860
  {
    get => GetProperty(Value860Property);
    set => SetProperty(Value860Property, value);
  }


  public static readonly PropertyInfo<string> Value861Property = RegisterProperty<string>(nameof(Value861));
  public string Value861
  {
    get => GetProperty(Value861Property);
    set => SetProperty(Value861Property, value);
  }


  public static readonly PropertyInfo<string> Value862Property = RegisterProperty<string>(nameof(Value862));
  public string Value862
  {
    get => GetProperty(Value862Property);
    set => SetProperty(Value862Property, value);
  }


  public static readonly PropertyInfo<string> Value863Property = RegisterProperty<string>(nameof(Value863));
  public string Value863
  {
    get => GetProperty(Value863Property);
    set => SetProperty(Value863Property, value);
  }


  public static readonly PropertyInfo<string> Value864Property = RegisterProperty<string>(nameof(Value864));
  public string Value864
  {
    get => GetProperty(Value864Property);
    set => SetProperty(Value864Property, value);
  }


  public static readonly PropertyInfo<string> Value865Property = RegisterProperty<string>(nameof(Value865));
  public string Value865
  {
    get => GetProperty(Value865Property);
    set => SetProperty(Value865Property, value);
  }


  public static readonly PropertyInfo<string> Value866Property = RegisterProperty<string>(nameof(Value866));
  public string Value866
  {
    get => GetProperty(Value866Property);
    set => SetProperty(Value866Property, value);
  }


  public static readonly PropertyInfo<string> Value867Property = RegisterProperty<string>(nameof(Value867));
  public string Value867
  {
    get => GetProperty(Value867Property);
    set => SetProperty(Value867Property, value);
  }


  public static readonly PropertyInfo<string> Value868Property = RegisterProperty<string>(nameof(Value868));
  public string Value868
  {
    get => GetProperty(Value868Property);
    set => SetProperty(Value868Property, value);
  }


  public static readonly PropertyInfo<string> Value869Property = RegisterProperty<string>(nameof(Value869));
  public string Value869
  {
    get => GetProperty(Value869Property);
    set => SetProperty(Value869Property, value);
  }


  public static readonly PropertyInfo<string> Value870Property = RegisterProperty<string>(nameof(Value870));
  public string Value870
  {
    get => GetProperty(Value870Property);
    set => SetProperty(Value870Property, value);
  }


  public static readonly PropertyInfo<string> Value871Property = RegisterProperty<string>(nameof(Value871));
  public string Value871
  {
    get => GetProperty(Value871Property);
    set => SetProperty(Value871Property, value);
  }


  public static readonly PropertyInfo<string> Value872Property = RegisterProperty<string>(nameof(Value872));
  public string Value872
  {
    get => GetProperty(Value872Property);
    set => SetProperty(Value872Property, value);
  }


  public static readonly PropertyInfo<string> Value873Property = RegisterProperty<string>(nameof(Value873));
  public string Value873
  {
    get => GetProperty(Value873Property);
    set => SetProperty(Value873Property, value);
  }


  public static readonly PropertyInfo<string> Value874Property = RegisterProperty<string>(nameof(Value874));
  public string Value874
  {
    get => GetProperty(Value874Property);
    set => SetProperty(Value874Property, value);
  }


  public static readonly PropertyInfo<string> Value875Property = RegisterProperty<string>(nameof(Value875));
  public string Value875
  {
    get => GetProperty(Value875Property);
    set => SetProperty(Value875Property, value);
  }


  public static readonly PropertyInfo<string> Value876Property = RegisterProperty<string>(nameof(Value876));
  public string Value876
  {
    get => GetProperty(Value876Property);
    set => SetProperty(Value876Property, value);
  }


  public static readonly PropertyInfo<string> Value877Property = RegisterProperty<string>(nameof(Value877));
  public string Value877
  {
    get => GetProperty(Value877Property);
    set => SetProperty(Value877Property, value);
  }


  public static readonly PropertyInfo<string> Value878Property = RegisterProperty<string>(nameof(Value878));
  public string Value878
  {
    get => GetProperty(Value878Property);
    set => SetProperty(Value878Property, value);
  }


  public static readonly PropertyInfo<string> Value879Property = RegisterProperty<string>(nameof(Value879));
  public string Value879
  {
    get => GetProperty(Value879Property);
    set => SetProperty(Value879Property, value);
  }


  public static readonly PropertyInfo<string> Value880Property = RegisterProperty<string>(nameof(Value880));
  public string Value880
  {
    get => GetProperty(Value880Property);
    set => SetProperty(Value880Property, value);
  }


  public static readonly PropertyInfo<string> Value881Property = RegisterProperty<string>(nameof(Value881));
  public string Value881
  {
    get => GetProperty(Value881Property);
    set => SetProperty(Value881Property, value);
  }


  public static readonly PropertyInfo<string> Value882Property = RegisterProperty<string>(nameof(Value882));
  public string Value882
  {
    get => GetProperty(Value882Property);
    set => SetProperty(Value882Property, value);
  }


  public static readonly PropertyInfo<string> Value883Property = RegisterProperty<string>(nameof(Value883));
  public string Value883
  {
    get => GetProperty(Value883Property);
    set => SetProperty(Value883Property, value);
  }


  public static readonly PropertyInfo<string> Value884Property = RegisterProperty<string>(nameof(Value884));
  public string Value884
  {
    get => GetProperty(Value884Property);
    set => SetProperty(Value884Property, value);
  }


  public static readonly PropertyInfo<string> Value885Property = RegisterProperty<string>(nameof(Value885));
  public string Value885
  {
    get => GetProperty(Value885Property);
    set => SetProperty(Value885Property, value);
  }


  public static readonly PropertyInfo<string> Value886Property = RegisterProperty<string>(nameof(Value886));
  public string Value886
  {
    get => GetProperty(Value886Property);
    set => SetProperty(Value886Property, value);
  }


  public static readonly PropertyInfo<string> Value887Property = RegisterProperty<string>(nameof(Value887));
  public string Value887
  {
    get => GetProperty(Value887Property);
    set => SetProperty(Value887Property, value);
  }


  public static readonly PropertyInfo<string> Value888Property = RegisterProperty<string>(nameof(Value888));
  public string Value888
  {
    get => GetProperty(Value888Property);
    set => SetProperty(Value888Property, value);
  }


  public static readonly PropertyInfo<string> Value889Property = RegisterProperty<string>(nameof(Value889));
  public string Value889
  {
    get => GetProperty(Value889Property);
    set => SetProperty(Value889Property, value);
  }


  public static readonly PropertyInfo<string> Value890Property = RegisterProperty<string>(nameof(Value890));
  public string Value890
  {
    get => GetProperty(Value890Property);
    set => SetProperty(Value890Property, value);
  }


  public static readonly PropertyInfo<string> Value891Property = RegisterProperty<string>(nameof(Value891));
  public string Value891
  {
    get => GetProperty(Value891Property);
    set => SetProperty(Value891Property, value);
  }


  public static readonly PropertyInfo<string> Value892Property = RegisterProperty<string>(nameof(Value892));
  public string Value892
  {
    get => GetProperty(Value892Property);
    set => SetProperty(Value892Property, value);
  }


  public static readonly PropertyInfo<string> Value893Property = RegisterProperty<string>(nameof(Value893));
  public string Value893
  {
    get => GetProperty(Value893Property);
    set => SetProperty(Value893Property, value);
  }


  public static readonly PropertyInfo<string> Value894Property = RegisterProperty<string>(nameof(Value894));
  public string Value894
  {
    get => GetProperty(Value894Property);
    set => SetProperty(Value894Property, value);
  }


  public static readonly PropertyInfo<string> Value895Property = RegisterProperty<string>(nameof(Value895));
  public string Value895
  {
    get => GetProperty(Value895Property);
    set => SetProperty(Value895Property, value);
  }


  public static readonly PropertyInfo<string> Value896Property = RegisterProperty<string>(nameof(Value896));
  public string Value896
  {
    get => GetProperty(Value896Property);
    set => SetProperty(Value896Property, value);
  }


  public static readonly PropertyInfo<string> Value897Property = RegisterProperty<string>(nameof(Value897));
  public string Value897
  {
    get => GetProperty(Value897Property);
    set => SetProperty(Value897Property, value);
  }


  public static readonly PropertyInfo<string> Value898Property = RegisterProperty<string>(nameof(Value898));
  public string Value898
  {
    get => GetProperty(Value898Property);
    set => SetProperty(Value898Property, value);
  }


  public static readonly PropertyInfo<string> Value899Property = RegisterProperty<string>(nameof(Value899));
  public string Value899
  {
    get => GetProperty(Value899Property);
    set => SetProperty(Value899Property, value);
  }


  public static readonly PropertyInfo<string> Value900Property = RegisterProperty<string>(nameof(Value900));
  public string Value900
  {
    get => GetProperty(Value900Property);
    set => SetProperty(Value900Property, value);
  }


  public static readonly PropertyInfo<string> Value901Property = RegisterProperty<string>(nameof(Value901));
  public string Value901
  {
    get => GetProperty(Value901Property);
    set => SetProperty(Value901Property, value);
  }


  public static readonly PropertyInfo<string> Value902Property = RegisterProperty<string>(nameof(Value902));
  public string Value902
  {
    get => GetProperty(Value902Property);
    set => SetProperty(Value902Property, value);
  }


  public static readonly PropertyInfo<string> Value903Property = RegisterProperty<string>(nameof(Value903));
  public string Value903
  {
    get => GetProperty(Value903Property);
    set => SetProperty(Value903Property, value);
  }


  public static readonly PropertyInfo<string> Value904Property = RegisterProperty<string>(nameof(Value904));
  public string Value904
  {
    get => GetProperty(Value904Property);
    set => SetProperty(Value904Property, value);
  }


  public static readonly PropertyInfo<string> Value905Property = RegisterProperty<string>(nameof(Value905));
  public string Value905
  {
    get => GetProperty(Value905Property);
    set => SetProperty(Value905Property, value);
  }


  public static readonly PropertyInfo<string> Value906Property = RegisterProperty<string>(nameof(Value906));
  public string Value906
  {
    get => GetProperty(Value906Property);
    set => SetProperty(Value906Property, value);
  }


  public static readonly PropertyInfo<string> Value907Property = RegisterProperty<string>(nameof(Value907));
  public string Value907
  {
    get => GetProperty(Value907Property);
    set => SetProperty(Value907Property, value);
  }


  public static readonly PropertyInfo<string> Value908Property = RegisterProperty<string>(nameof(Value908));
  public string Value908
  {
    get => GetProperty(Value908Property);
    set => SetProperty(Value908Property, value);
  }


  public static readonly PropertyInfo<string> Value909Property = RegisterProperty<string>(nameof(Value909));
  public string Value909
  {
    get => GetProperty(Value909Property);
    set => SetProperty(Value909Property, value);
  }


  public static readonly PropertyInfo<string> Value910Property = RegisterProperty<string>(nameof(Value910));
  public string Value910
  {
    get => GetProperty(Value910Property);
    set => SetProperty(Value910Property, value);
  }


  public static readonly PropertyInfo<string> Value911Property = RegisterProperty<string>(nameof(Value911));
  public string Value911
  {
    get => GetProperty(Value911Property);
    set => SetProperty(Value911Property, value);
  }


  public static readonly PropertyInfo<string> Value912Property = RegisterProperty<string>(nameof(Value912));
  public string Value912
  {
    get => GetProperty(Value912Property);
    set => SetProperty(Value912Property, value);
  }


  public static readonly PropertyInfo<string> Value913Property = RegisterProperty<string>(nameof(Value913));
  public string Value913
  {
    get => GetProperty(Value913Property);
    set => SetProperty(Value913Property, value);
  }


  public static readonly PropertyInfo<string> Value914Property = RegisterProperty<string>(nameof(Value914));
  public string Value914
  {
    get => GetProperty(Value914Property);
    set => SetProperty(Value914Property, value);
  }


  public static readonly PropertyInfo<string> Value915Property = RegisterProperty<string>(nameof(Value915));
  public string Value915
  {
    get => GetProperty(Value915Property);
    set => SetProperty(Value915Property, value);
  }


  public static readonly PropertyInfo<string> Value916Property = RegisterProperty<string>(nameof(Value916));
  public string Value916
  {
    get => GetProperty(Value916Property);
    set => SetProperty(Value916Property, value);
  }


  public static readonly PropertyInfo<string> Value917Property = RegisterProperty<string>(nameof(Value917));
  public string Value917
  {
    get => GetProperty(Value917Property);
    set => SetProperty(Value917Property, value);
  }


  public static readonly PropertyInfo<string> Value918Property = RegisterProperty<string>(nameof(Value918));
  public string Value918
  {
    get => GetProperty(Value918Property);
    set => SetProperty(Value918Property, value);
  }


  public static readonly PropertyInfo<string> Value919Property = RegisterProperty<string>(nameof(Value919));
  public string Value919
  {
    get => GetProperty(Value919Property);
    set => SetProperty(Value919Property, value);
  }


  public static readonly PropertyInfo<string> Value920Property = RegisterProperty<string>(nameof(Value920));
  public string Value920
  {
    get => GetProperty(Value920Property);
    set => SetProperty(Value920Property, value);
  }


  public static readonly PropertyInfo<string> Value921Property = RegisterProperty<string>(nameof(Value921));
  public string Value921
  {
    get => GetProperty(Value921Property);
    set => SetProperty(Value921Property, value);
  }


  public static readonly PropertyInfo<string> Value922Property = RegisterProperty<string>(nameof(Value922));
  public string Value922
  {
    get => GetProperty(Value922Property);
    set => SetProperty(Value922Property, value);
  }


  public static readonly PropertyInfo<string> Value923Property = RegisterProperty<string>(nameof(Value923));
  public string Value923
  {
    get => GetProperty(Value923Property);
    set => SetProperty(Value923Property, value);
  }


  public static readonly PropertyInfo<string> Value924Property = RegisterProperty<string>(nameof(Value924));
  public string Value924
  {
    get => GetProperty(Value924Property);
    set => SetProperty(Value924Property, value);
  }


  public static readonly PropertyInfo<string> Value925Property = RegisterProperty<string>(nameof(Value925));
  public string Value925
  {
    get => GetProperty(Value925Property);
    set => SetProperty(Value925Property, value);
  }


  public static readonly PropertyInfo<string> Value926Property = RegisterProperty<string>(nameof(Value926));
  public string Value926
  {
    get => GetProperty(Value926Property);
    set => SetProperty(Value926Property, value);
  }


  public static readonly PropertyInfo<string> Value927Property = RegisterProperty<string>(nameof(Value927));
  public string Value927
  {
    get => GetProperty(Value927Property);
    set => SetProperty(Value927Property, value);
  }


  public static readonly PropertyInfo<string> Value928Property = RegisterProperty<string>(nameof(Value928));
  public string Value928
  {
    get => GetProperty(Value928Property);
    set => SetProperty(Value928Property, value);
  }


  public static readonly PropertyInfo<string> Value929Property = RegisterProperty<string>(nameof(Value929));
  public string Value929
  {
    get => GetProperty(Value929Property);
    set => SetProperty(Value929Property, value);
  }


  public static readonly PropertyInfo<string> Value930Property = RegisterProperty<string>(nameof(Value930));
  public string Value930
  {
    get => GetProperty(Value930Property);
    set => SetProperty(Value930Property, value);
  }


  public static readonly PropertyInfo<string> Value931Property = RegisterProperty<string>(nameof(Value931));
  public string Value931
  {
    get => GetProperty(Value931Property);
    set => SetProperty(Value931Property, value);
  }


  public static readonly PropertyInfo<string> Value932Property = RegisterProperty<string>(nameof(Value932));
  public string Value932
  {
    get => GetProperty(Value932Property);
    set => SetProperty(Value932Property, value);
  }


  public static readonly PropertyInfo<string> Value933Property = RegisterProperty<string>(nameof(Value933));
  public string Value933
  {
    get => GetProperty(Value933Property);
    set => SetProperty(Value933Property, value);
  }


  public static readonly PropertyInfo<string> Value934Property = RegisterProperty<string>(nameof(Value934));
  public string Value934
  {
    get => GetProperty(Value934Property);
    set => SetProperty(Value934Property, value);
  }


  public static readonly PropertyInfo<string> Value935Property = RegisterProperty<string>(nameof(Value935));
  public string Value935
  {
    get => GetProperty(Value935Property);
    set => SetProperty(Value935Property, value);
  }


  public static readonly PropertyInfo<string> Value936Property = RegisterProperty<string>(nameof(Value936));
  public string Value936
  {
    get => GetProperty(Value936Property);
    set => SetProperty(Value936Property, value);
  }


  public static readonly PropertyInfo<string> Value937Property = RegisterProperty<string>(nameof(Value937));
  public string Value937
  {
    get => GetProperty(Value937Property);
    set => SetProperty(Value937Property, value);
  }


  public static readonly PropertyInfo<string> Value938Property = RegisterProperty<string>(nameof(Value938));
  public string Value938
  {
    get => GetProperty(Value938Property);
    set => SetProperty(Value938Property, value);
  }


  public static readonly PropertyInfo<string> Value939Property = RegisterProperty<string>(nameof(Value939));
  public string Value939
  {
    get => GetProperty(Value939Property);
    set => SetProperty(Value939Property, value);
  }


  public static readonly PropertyInfo<string> Value940Property = RegisterProperty<string>(nameof(Value940));
  public string Value940
  {
    get => GetProperty(Value940Property);
    set => SetProperty(Value940Property, value);
  }


  public static readonly PropertyInfo<string> Value941Property = RegisterProperty<string>(nameof(Value941));
  public string Value941
  {
    get => GetProperty(Value941Property);
    set => SetProperty(Value941Property, value);
  }


  public static readonly PropertyInfo<string> Value942Property = RegisterProperty<string>(nameof(Value942));
  public string Value942
  {
    get => GetProperty(Value942Property);
    set => SetProperty(Value942Property, value);
  }


  public static readonly PropertyInfo<string> Value943Property = RegisterProperty<string>(nameof(Value943));
  public string Value943
  {
    get => GetProperty(Value943Property);
    set => SetProperty(Value943Property, value);
  }


  public static readonly PropertyInfo<string> Value944Property = RegisterProperty<string>(nameof(Value944));
  public string Value944
  {
    get => GetProperty(Value944Property);
    set => SetProperty(Value944Property, value);
  }


  public static readonly PropertyInfo<string> Value945Property = RegisterProperty<string>(nameof(Value945));
  public string Value945
  {
    get => GetProperty(Value945Property);
    set => SetProperty(Value945Property, value);
  }


  public static readonly PropertyInfo<string> Value946Property = RegisterProperty<string>(nameof(Value946));
  public string Value946
  {
    get => GetProperty(Value946Property);
    set => SetProperty(Value946Property, value);
  }


  public static readonly PropertyInfo<string> Value947Property = RegisterProperty<string>(nameof(Value947));
  public string Value947
  {
    get => GetProperty(Value947Property);
    set => SetProperty(Value947Property, value);
  }


  public static readonly PropertyInfo<string> Value948Property = RegisterProperty<string>(nameof(Value948));
  public string Value948
  {
    get => GetProperty(Value948Property);
    set => SetProperty(Value948Property, value);
  }


  public static readonly PropertyInfo<string> Value949Property = RegisterProperty<string>(nameof(Value949));
  public string Value949
  {
    get => GetProperty(Value949Property);
    set => SetProperty(Value949Property, value);
  }


  public static readonly PropertyInfo<string> Value950Property = RegisterProperty<string>(nameof(Value950));
  public string Value950
  {
    get => GetProperty(Value950Property);
    set => SetProperty(Value950Property, value);
  }


  public static readonly PropertyInfo<string> Value951Property = RegisterProperty<string>(nameof(Value951));
  public string Value951
  {
    get => GetProperty(Value951Property);
    set => SetProperty(Value951Property, value);
  }


  public static readonly PropertyInfo<string> Value952Property = RegisterProperty<string>(nameof(Value952));
  public string Value952
  {
    get => GetProperty(Value952Property);
    set => SetProperty(Value952Property, value);
  }


  public static readonly PropertyInfo<string> Value953Property = RegisterProperty<string>(nameof(Value953));
  public string Value953
  {
    get => GetProperty(Value953Property);
    set => SetProperty(Value953Property, value);
  }


  public static readonly PropertyInfo<string> Value954Property = RegisterProperty<string>(nameof(Value954));
  public string Value954
  {
    get => GetProperty(Value954Property);
    set => SetProperty(Value954Property, value);
  }


  public static readonly PropertyInfo<string> Value955Property = RegisterProperty<string>(nameof(Value955));
  public string Value955
  {
    get => GetProperty(Value955Property);
    set => SetProperty(Value955Property, value);
  }


  public static readonly PropertyInfo<string> Value956Property = RegisterProperty<string>(nameof(Value956));
  public string Value956
  {
    get => GetProperty(Value956Property);
    set => SetProperty(Value956Property, value);
  }


  public static readonly PropertyInfo<string> Value957Property = RegisterProperty<string>(nameof(Value957));
  public string Value957
  {
    get => GetProperty(Value957Property);
    set => SetProperty(Value957Property, value);
  }


  public static readonly PropertyInfo<string> Value958Property = RegisterProperty<string>(nameof(Value958));
  public string Value958
  {
    get => GetProperty(Value958Property);
    set => SetProperty(Value958Property, value);
  }


  public static readonly PropertyInfo<string> Value959Property = RegisterProperty<string>(nameof(Value959));
  public string Value959
  {
    get => GetProperty(Value959Property);
    set => SetProperty(Value959Property, value);
  }


  public static readonly PropertyInfo<string> Value960Property = RegisterProperty<string>(nameof(Value960));
  public string Value960
  {
    get => GetProperty(Value960Property);
    set => SetProperty(Value960Property, value);
  }


  public static readonly PropertyInfo<string> Value961Property = RegisterProperty<string>(nameof(Value961));
  public string Value961
  {
    get => GetProperty(Value961Property);
    set => SetProperty(Value961Property, value);
  }


  public static readonly PropertyInfo<string> Value962Property = RegisterProperty<string>(nameof(Value962));
  public string Value962
  {
    get => GetProperty(Value962Property);
    set => SetProperty(Value962Property, value);
  }


  public static readonly PropertyInfo<string> Value963Property = RegisterProperty<string>(nameof(Value963));
  public string Value963
  {
    get => GetProperty(Value963Property);
    set => SetProperty(Value963Property, value);
  }


  public static readonly PropertyInfo<string> Value964Property = RegisterProperty<string>(nameof(Value964));
  public string Value964
  {
    get => GetProperty(Value964Property);
    set => SetProperty(Value964Property, value);
  }


  public static readonly PropertyInfo<string> Value965Property = RegisterProperty<string>(nameof(Value965));
  public string Value965
  {
    get => GetProperty(Value965Property);
    set => SetProperty(Value965Property, value);
  }


  public static readonly PropertyInfo<string> Value966Property = RegisterProperty<string>(nameof(Value966));
  public string Value966
  {
    get => GetProperty(Value966Property);
    set => SetProperty(Value966Property, value);
  }


  public static readonly PropertyInfo<string> Value967Property = RegisterProperty<string>(nameof(Value967));
  public string Value967
  {
    get => GetProperty(Value967Property);
    set => SetProperty(Value967Property, value);
  }


  public static readonly PropertyInfo<string> Value968Property = RegisterProperty<string>(nameof(Value968));
  public string Value968
  {
    get => GetProperty(Value968Property);
    set => SetProperty(Value968Property, value);
  }


  public static readonly PropertyInfo<string> Value969Property = RegisterProperty<string>(nameof(Value969));
  public string Value969
  {
    get => GetProperty(Value969Property);
    set => SetProperty(Value969Property, value);
  }


  public static readonly PropertyInfo<string> Value970Property = RegisterProperty<string>(nameof(Value970));
  public string Value970
  {
    get => GetProperty(Value970Property);
    set => SetProperty(Value970Property, value);
  }


  public static readonly PropertyInfo<string> Value971Property = RegisterProperty<string>(nameof(Value971));
  public string Value971
  {
    get => GetProperty(Value971Property);
    set => SetProperty(Value971Property, value);
  }


  public static readonly PropertyInfo<string> Value972Property = RegisterProperty<string>(nameof(Value972));
  public string Value972
  {
    get => GetProperty(Value972Property);
    set => SetProperty(Value972Property, value);
  }


  public static readonly PropertyInfo<string> Value973Property = RegisterProperty<string>(nameof(Value973));
  public string Value973
  {
    get => GetProperty(Value973Property);
    set => SetProperty(Value973Property, value);
  }


  public static readonly PropertyInfo<string> Value974Property = RegisterProperty<string>(nameof(Value974));
  public string Value974
  {
    get => GetProperty(Value974Property);
    set => SetProperty(Value974Property, value);
  }


  public static readonly PropertyInfo<string> Value975Property = RegisterProperty<string>(nameof(Value975));
  public string Value975
  {
    get => GetProperty(Value975Property);
    set => SetProperty(Value975Property, value);
  }


  public static readonly PropertyInfo<string> Value976Property = RegisterProperty<string>(nameof(Value976));
  public string Value976
  {
    get => GetProperty(Value976Property);
    set => SetProperty(Value976Property, value);
  }


  public static readonly PropertyInfo<string> Value977Property = RegisterProperty<string>(nameof(Value977));
  public string Value977
  {
    get => GetProperty(Value977Property);
    set => SetProperty(Value977Property, value);
  }


  public static readonly PropertyInfo<string> Value978Property = RegisterProperty<string>(nameof(Value978));
  public string Value978
  {
    get => GetProperty(Value978Property);
    set => SetProperty(Value978Property, value);
  }


  public static readonly PropertyInfo<string> Value979Property = RegisterProperty<string>(nameof(Value979));
  public string Value979
  {
    get => GetProperty(Value979Property);
    set => SetProperty(Value979Property, value);
  }


  public static readonly PropertyInfo<string> Value980Property = RegisterProperty<string>(nameof(Value980));
  public string Value980
  {
    get => GetProperty(Value980Property);
    set => SetProperty(Value980Property, value);
  }


  public static readonly PropertyInfo<string> Value981Property = RegisterProperty<string>(nameof(Value981));
  public string Value981
  {
    get => GetProperty(Value981Property);
    set => SetProperty(Value981Property, value);
  }


  public static readonly PropertyInfo<string> Value982Property = RegisterProperty<string>(nameof(Value982));
  public string Value982
  {
    get => GetProperty(Value982Property);
    set => SetProperty(Value982Property, value);
  }


  public static readonly PropertyInfo<string> Value983Property = RegisterProperty<string>(nameof(Value983));
  public string Value983
  {
    get => GetProperty(Value983Property);
    set => SetProperty(Value983Property, value);
  }


  public static readonly PropertyInfo<string> Value984Property = RegisterProperty<string>(nameof(Value984));
  public string Value984
  {
    get => GetProperty(Value984Property);
    set => SetProperty(Value984Property, value);
  }


  public static readonly PropertyInfo<string> Value985Property = RegisterProperty<string>(nameof(Value985));
  public string Value985
  {
    get => GetProperty(Value985Property);
    set => SetProperty(Value985Property, value);
  }


  public static readonly PropertyInfo<string> Value986Property = RegisterProperty<string>(nameof(Value986));
  public string Value986
  {
    get => GetProperty(Value986Property);
    set => SetProperty(Value986Property, value);
  }


  public static readonly PropertyInfo<string> Value987Property = RegisterProperty<string>(nameof(Value987));
  public string Value987
  {
    get => GetProperty(Value987Property);
    set => SetProperty(Value987Property, value);
  }


  public static readonly PropertyInfo<string> Value988Property = RegisterProperty<string>(nameof(Value988));
  public string Value988
  {
    get => GetProperty(Value988Property);
    set => SetProperty(Value988Property, value);
  }


  public static readonly PropertyInfo<string> Value989Property = RegisterProperty<string>(nameof(Value989));
  public string Value989
  {
    get => GetProperty(Value989Property);
    set => SetProperty(Value989Property, value);
  }


  public static readonly PropertyInfo<string> Value990Property = RegisterProperty<string>(nameof(Value990));
  public string Value990
  {
    get => GetProperty(Value990Property);
    set => SetProperty(Value990Property, value);
  }


  public static readonly PropertyInfo<string> Value991Property = RegisterProperty<string>(nameof(Value991));
  public string Value991
  {
    get => GetProperty(Value991Property);
    set => SetProperty(Value991Property, value);
  }


  public static readonly PropertyInfo<string> Value992Property = RegisterProperty<string>(nameof(Value992));
  public string Value992
  {
    get => GetProperty(Value992Property);
    set => SetProperty(Value992Property, value);
  }


  public static readonly PropertyInfo<string> Value993Property = RegisterProperty<string>(nameof(Value993));
  public string Value993
  {
    get => GetProperty(Value993Property);
    set => SetProperty(Value993Property, value);
  }


  public static readonly PropertyInfo<string> Value994Property = RegisterProperty<string>(nameof(Value994));
  public string Value994
  {
    get => GetProperty(Value994Property);
    set => SetProperty(Value994Property, value);
  }


  public static readonly PropertyInfo<string> Value995Property = RegisterProperty<string>(nameof(Value995));
  public string Value995
  {
    get => GetProperty(Value995Property);
    set => SetProperty(Value995Property, value);
  }


  public static readonly PropertyInfo<string> Value996Property = RegisterProperty<string>(nameof(Value996));
  public string Value996
  {
    get => GetProperty(Value996Property);
    set => SetProperty(Value996Property, value);
  }


  public static readonly PropertyInfo<string> Value997Property = RegisterProperty<string>(nameof(Value997));
  public string Value997
  {
    get => GetProperty(Value997Property);
    set => SetProperty(Value997Property, value);
  }


  public static readonly PropertyInfo<string> Value998Property = RegisterProperty<string>(nameof(Value998));
  public string Value998
  {
    get => GetProperty(Value998Property);
    set => SetProperty(Value998Property, value);
  }


  public static readonly PropertyInfo<string> Value999Property = RegisterProperty<string>(nameof(Value999));
  public string Value999
  {
    get => GetProperty(Value999Property);
    set => SetProperty(Value999Property, value);
  }


  public static readonly PropertyInfo<string> Value1000Property = RegisterProperty<string>(nameof(Value1000));
  public string Value1000
  {
    get => GetProperty(Value1000Property);
    set => SetProperty(Value1000Property, value);
  }


  public static readonly PropertyInfo<string> Value1001Property = RegisterProperty<string>(nameof(Value1001));
  public string Value1001
  {
    get => GetProperty(Value1001Property);
    set => SetProperty(Value1001Property, value);
  }


  public static readonly PropertyInfo<string> Value1002Property = RegisterProperty<string>(nameof(Value1002));
  public string Value1002
  {
    get => GetProperty(Value1002Property);
    set => SetProperty(Value1002Property, value);
  }


  public static readonly PropertyInfo<string> Value1003Property = RegisterProperty<string>(nameof(Value1003));
  public string Value1003
  {
    get => GetProperty(Value1003Property);
    set => SetProperty(Value1003Property, value);
  }


  public static readonly PropertyInfo<string> Value1004Property = RegisterProperty<string>(nameof(Value1004));
  public string Value1004
  {
    get => GetProperty(Value1004Property);
    set => SetProperty(Value1004Property, value);
  }


  public static readonly PropertyInfo<string> Value1005Property = RegisterProperty<string>(nameof(Value1005));
  public string Value1005
  {
    get => GetProperty(Value1005Property);
    set => SetProperty(Value1005Property, value);
  }


  public static readonly PropertyInfo<string> Value1006Property = RegisterProperty<string>(nameof(Value1006));
  public string Value1006
  {
    get => GetProperty(Value1006Property);
    set => SetProperty(Value1006Property, value);
  }


  public static readonly PropertyInfo<string> Value1007Property = RegisterProperty<string>(nameof(Value1007));
  public string Value1007
  {
    get => GetProperty(Value1007Property);
    set => SetProperty(Value1007Property, value);
  }


  public static readonly PropertyInfo<string> Value1008Property = RegisterProperty<string>(nameof(Value1008));
  public string Value1008
  {
    get => GetProperty(Value1008Property);
    set => SetProperty(Value1008Property, value);
  }


  public static readonly PropertyInfo<string> Value1009Property = RegisterProperty<string>(nameof(Value1009));
  public string Value1009
  {
    get => GetProperty(Value1009Property);
    set => SetProperty(Value1009Property, value);
  }


  public static readonly PropertyInfo<string> Value1010Property = RegisterProperty<string>(nameof(Value1010));
  public string Value1010
  {
    get => GetProperty(Value1010Property);
    set => SetProperty(Value1010Property, value);
  }


  public static readonly PropertyInfo<string> Value1011Property = RegisterProperty<string>(nameof(Value1011));
  public string Value1011
  {
    get => GetProperty(Value1011Property);
    set => SetProperty(Value1011Property, value);
  }


  public static readonly PropertyInfo<string> Value1012Property = RegisterProperty<string>(nameof(Value1012));
  public string Value1012
  {
    get => GetProperty(Value1012Property);
    set => SetProperty(Value1012Property, value);
  }


  public static readonly PropertyInfo<string> Value1013Property = RegisterProperty<string>(nameof(Value1013));
  public string Value1013
  {
    get => GetProperty(Value1013Property);
    set => SetProperty(Value1013Property, value);
  }


  public static readonly PropertyInfo<string> Value1014Property = RegisterProperty<string>(nameof(Value1014));
  public string Value1014
  {
    get => GetProperty(Value1014Property);
    set => SetProperty(Value1014Property, value);
  }


  public static readonly PropertyInfo<string> Value1015Property = RegisterProperty<string>(nameof(Value1015));
  public string Value1015
  {
    get => GetProperty(Value1015Property);
    set => SetProperty(Value1015Property, value);
  }


  public static readonly PropertyInfo<string> Value1016Property = RegisterProperty<string>(nameof(Value1016));
  public string Value1016
  {
    get => GetProperty(Value1016Property);
    set => SetProperty(Value1016Property, value);
  }


  public static readonly PropertyInfo<string> Value1017Property = RegisterProperty<string>(nameof(Value1017));
  public string Value1017
  {
    get => GetProperty(Value1017Property);
    set => SetProperty(Value1017Property, value);
  }


  public static readonly PropertyInfo<string> Value1018Property = RegisterProperty<string>(nameof(Value1018));
  public string Value1018
  {
    get => GetProperty(Value1018Property);
    set => SetProperty(Value1018Property, value);
  }


  public static readonly PropertyInfo<string> Value1019Property = RegisterProperty<string>(nameof(Value1019));
  public string Value1019
  {
    get => GetProperty(Value1019Property);
    set => SetProperty(Value1019Property, value);
  }


  public static readonly PropertyInfo<string> Value1020Property = RegisterProperty<string>(nameof(Value1020));
  public string Value1020
  {
    get => GetProperty(Value1020Property);
    set => SetProperty(Value1020Property, value);
  }


  public static readonly PropertyInfo<string> Value1021Property = RegisterProperty<string>(nameof(Value1021));
  public string Value1021
  {
    get => GetProperty(Value1021Property);
    set => SetProperty(Value1021Property, value);
  }


  public static readonly PropertyInfo<string> Value1022Property = RegisterProperty<string>(nameof(Value1022));
  public string Value1022
  {
    get => GetProperty(Value1022Property);
    set => SetProperty(Value1022Property, value);
  }


  public static readonly PropertyInfo<string> Value1023Property = RegisterProperty<string>(nameof(Value1023));
  public string Value1023
  {
    get => GetProperty(Value1023Property);
    set => SetProperty(Value1023Property, value);
  }


  public static readonly PropertyInfo<string> Value1024Property = RegisterProperty<string>(nameof(Value1024));
  public string Value1024
  {
    get => GetProperty(Value1024Property);
    set => SetProperty(Value1024Property, value);
  }


  public static readonly PropertyInfo<string> Value1025Property = RegisterProperty<string>(nameof(Value1025));
  public string Value1025
  {
    get => GetProperty(Value1025Property);
    set => SetProperty(Value1025Property, value);
  }


  public static readonly PropertyInfo<string> Value1026Property = RegisterProperty<string>(nameof(Value1026));
  public string Value1026
  {
    get => GetProperty(Value1026Property);
    set => SetProperty(Value1026Property, value);
  }


  public static readonly PropertyInfo<string> Value1027Property = RegisterProperty<string>(nameof(Value1027));
  public string Value1027
  {
    get => GetProperty(Value1027Property);
    set => SetProperty(Value1027Property, value);
  }


  public static readonly PropertyInfo<string> Value1028Property = RegisterProperty<string>(nameof(Value1028));
  public string Value1028
  {
    get => GetProperty(Value1028Property);
    set => SetProperty(Value1028Property, value);
  }


  public static readonly PropertyInfo<string> Value1029Property = RegisterProperty<string>(nameof(Value1029));
  public string Value1029
  {
    get => GetProperty(Value1029Property);
    set => SetProperty(Value1029Property, value);
  }


  public static readonly PropertyInfo<string> Value1030Property = RegisterProperty<string>(nameof(Value1030));
  public string Value1030
  {
    get => GetProperty(Value1030Property);
    set => SetProperty(Value1030Property, value);
  }


  public static readonly PropertyInfo<string> Value1031Property = RegisterProperty<string>(nameof(Value1031));
  public string Value1031
  {
    get => GetProperty(Value1031Property);
    set => SetProperty(Value1031Property, value);
  }


  public static readonly PropertyInfo<string> Value1032Property = RegisterProperty<string>(nameof(Value1032));
  public string Value1032
  {
    get => GetProperty(Value1032Property);
    set => SetProperty(Value1032Property, value);
  }


  public static readonly PropertyInfo<string> Value1033Property = RegisterProperty<string>(nameof(Value1033));
  public string Value1033
  {
    get => GetProperty(Value1033Property);
    set => SetProperty(Value1033Property, value);
  }


  public static readonly PropertyInfo<string> Value1034Property = RegisterProperty<string>(nameof(Value1034));
  public string Value1034
  {
    get => GetProperty(Value1034Property);
    set => SetProperty(Value1034Property, value);
  }


  public static readonly PropertyInfo<string> Value1035Property = RegisterProperty<string>(nameof(Value1035));
  public string Value1035
  {
    get => GetProperty(Value1035Property);
    set => SetProperty(Value1035Property, value);
  }


  public static readonly PropertyInfo<string> Value1036Property = RegisterProperty<string>(nameof(Value1036));
  public string Value1036
  {
    get => GetProperty(Value1036Property);
    set => SetProperty(Value1036Property, value);
  }


  public static readonly PropertyInfo<string> Value1037Property = RegisterProperty<string>(nameof(Value1037));
  public string Value1037
  {
    get => GetProperty(Value1037Property);
    set => SetProperty(Value1037Property, value);
  }


  public static readonly PropertyInfo<string> Value1038Property = RegisterProperty<string>(nameof(Value1038));
  public string Value1038
  {
    get => GetProperty(Value1038Property);
    set => SetProperty(Value1038Property, value);
  }


  public static readonly PropertyInfo<string> Value1039Property = RegisterProperty<string>(nameof(Value1039));
  public string Value1039
  {
    get => GetProperty(Value1039Property);
    set => SetProperty(Value1039Property, value);
  }


  public static readonly PropertyInfo<string> Value1040Property = RegisterProperty<string>(nameof(Value1040));
  public string Value1040
  {
    get => GetProperty(Value1040Property);
    set => SetProperty(Value1040Property, value);
  }


  public static readonly PropertyInfo<string> Value1041Property = RegisterProperty<string>(nameof(Value1041));
  public string Value1041
  {
    get => GetProperty(Value1041Property);
    set => SetProperty(Value1041Property, value);
  }


  public static readonly PropertyInfo<string> Value1042Property = RegisterProperty<string>(nameof(Value1042));
  public string Value1042
  {
    get => GetProperty(Value1042Property);
    set => SetProperty(Value1042Property, value);
  }


  public static readonly PropertyInfo<string> Value1043Property = RegisterProperty<string>(nameof(Value1043));
  public string Value1043
  {
    get => GetProperty(Value1043Property);
    set => SetProperty(Value1043Property, value);
  }


  public static readonly PropertyInfo<string> Value1044Property = RegisterProperty<string>(nameof(Value1044));
  public string Value1044
  {
    get => GetProperty(Value1044Property);
    set => SetProperty(Value1044Property, value);
  }


  public static readonly PropertyInfo<string> Value1045Property = RegisterProperty<string>(nameof(Value1045));
  public string Value1045
  {
    get => GetProperty(Value1045Property);
    set => SetProperty(Value1045Property, value);
  }


  public static readonly PropertyInfo<string> Value1046Property = RegisterProperty<string>(nameof(Value1046));
  public string Value1046
  {
    get => GetProperty(Value1046Property);
    set => SetProperty(Value1046Property, value);
  }


  public static readonly PropertyInfo<string> Value1047Property = RegisterProperty<string>(nameof(Value1047));
  public string Value1047
  {
    get => GetProperty(Value1047Property);
    set => SetProperty(Value1047Property, value);
  }


  public static readonly PropertyInfo<string> Value1048Property = RegisterProperty<string>(nameof(Value1048));
  public string Value1048
  {
    get => GetProperty(Value1048Property);
    set => SetProperty(Value1048Property, value);
  }


  public static readonly PropertyInfo<string> Value1049Property = RegisterProperty<string>(nameof(Value1049));
  public string Value1049
  {
    get => GetProperty(Value1049Property);
    set => SetProperty(Value1049Property, value);
  }


  public static readonly PropertyInfo<string> Value1050Property = RegisterProperty<string>(nameof(Value1050));
  public string Value1050
  {
    get => GetProperty(Value1050Property);
    set => SetProperty(Value1050Property, value);
  }


  public static readonly PropertyInfo<string> Value1051Property = RegisterProperty<string>(nameof(Value1051));
  public string Value1051
  {
    get => GetProperty(Value1051Property);
    set => SetProperty(Value1051Property, value);
  }


  public static readonly PropertyInfo<string> Value1052Property = RegisterProperty<string>(nameof(Value1052));
  public string Value1052
  {
    get => GetProperty(Value1052Property);
    set => SetProperty(Value1052Property, value);
  }


  public static readonly PropertyInfo<string> Value1053Property = RegisterProperty<string>(nameof(Value1053));
  public string Value1053
  {
    get => GetProperty(Value1053Property);
    set => SetProperty(Value1053Property, value);
  }


  public static readonly PropertyInfo<string> Value1054Property = RegisterProperty<string>(nameof(Value1054));
  public string Value1054
  {
    get => GetProperty(Value1054Property);
    set => SetProperty(Value1054Property, value);
  }


  public static readonly PropertyInfo<string> Value1055Property = RegisterProperty<string>(nameof(Value1055));
  public string Value1055
  {
    get => GetProperty(Value1055Property);
    set => SetProperty(Value1055Property, value);
  }


  public static readonly PropertyInfo<string> Value1056Property = RegisterProperty<string>(nameof(Value1056));
  public string Value1056
  {
    get => GetProperty(Value1056Property);
    set => SetProperty(Value1056Property, value);
  }


  public static readonly PropertyInfo<string> Value1057Property = RegisterProperty<string>(nameof(Value1057));
  public string Value1057
  {
    get => GetProperty(Value1057Property);
    set => SetProperty(Value1057Property, value);
  }


  public static readonly PropertyInfo<string> Value1058Property = RegisterProperty<string>(nameof(Value1058));
  public string Value1058
  {
    get => GetProperty(Value1058Property);
    set => SetProperty(Value1058Property, value);
  }


  public static readonly PropertyInfo<string> Value1059Property = RegisterProperty<string>(nameof(Value1059));
  public string Value1059
  {
    get => GetProperty(Value1059Property);
    set => SetProperty(Value1059Property, value);
  }


  public static readonly PropertyInfo<string> Value1060Property = RegisterProperty<string>(nameof(Value1060));
  public string Value1060
  {
    get => GetProperty(Value1060Property);
    set => SetProperty(Value1060Property, value);
  }


  public static readonly PropertyInfo<string> Value1061Property = RegisterProperty<string>(nameof(Value1061));
  public string Value1061
  {
    get => GetProperty(Value1061Property);
    set => SetProperty(Value1061Property, value);
  }


  public static readonly PropertyInfo<string> Value1062Property = RegisterProperty<string>(nameof(Value1062));
  public string Value1062
  {
    get => GetProperty(Value1062Property);
    set => SetProperty(Value1062Property, value);
  }


  public static readonly PropertyInfo<string> Value1063Property = RegisterProperty<string>(nameof(Value1063));
  public string Value1063
  {
    get => GetProperty(Value1063Property);
    set => SetProperty(Value1063Property, value);
  }


  public static readonly PropertyInfo<string> Value1064Property = RegisterProperty<string>(nameof(Value1064));
  public string Value1064
  {
    get => GetProperty(Value1064Property);
    set => SetProperty(Value1064Property, value);
  }


  public static readonly PropertyInfo<string> Value1065Property = RegisterProperty<string>(nameof(Value1065));
  public string Value1065
  {
    get => GetProperty(Value1065Property);
    set => SetProperty(Value1065Property, value);
  }


  public static readonly PropertyInfo<string> Value1066Property = RegisterProperty<string>(nameof(Value1066));
  public string Value1066
  {
    get => GetProperty(Value1066Property);
    set => SetProperty(Value1066Property, value);
  }


  public static readonly PropertyInfo<string> Value1067Property = RegisterProperty<string>(nameof(Value1067));
  public string Value1067
  {
    get => GetProperty(Value1067Property);
    set => SetProperty(Value1067Property, value);
  }


  public static readonly PropertyInfo<string> Value1068Property = RegisterProperty<string>(nameof(Value1068));
  public string Value1068
  {
    get => GetProperty(Value1068Property);
    set => SetProperty(Value1068Property, value);
  }


  public static readonly PropertyInfo<string> Value1069Property = RegisterProperty<string>(nameof(Value1069));
  public string Value1069
  {
    get => GetProperty(Value1069Property);
    set => SetProperty(Value1069Property, value);
  }


  public static readonly PropertyInfo<string> Value1070Property = RegisterProperty<string>(nameof(Value1070));
  public string Value1070
  {
    get => GetProperty(Value1070Property);
    set => SetProperty(Value1070Property, value);
  }


  public static readonly PropertyInfo<string> Value1071Property = RegisterProperty<string>(nameof(Value1071));
  public string Value1071
  {
    get => GetProperty(Value1071Property);
    set => SetProperty(Value1071Property, value);
  }


  public static readonly PropertyInfo<string> Value1072Property = RegisterProperty<string>(nameof(Value1072));
  public string Value1072
  {
    get => GetProperty(Value1072Property);
    set => SetProperty(Value1072Property, value);
  }


  public static readonly PropertyInfo<string> Value1073Property = RegisterProperty<string>(nameof(Value1073));
  public string Value1073
  {
    get => GetProperty(Value1073Property);
    set => SetProperty(Value1073Property, value);
  }


  public static readonly PropertyInfo<string> Value1074Property = RegisterProperty<string>(nameof(Value1074));
  public string Value1074
  {
    get => GetProperty(Value1074Property);
    set => SetProperty(Value1074Property, value);
  }


  public static readonly PropertyInfo<string> Value1075Property = RegisterProperty<string>(nameof(Value1075));
  public string Value1075
  {
    get => GetProperty(Value1075Property);
    set => SetProperty(Value1075Property, value);
  }


  public static readonly PropertyInfo<string> Value1076Property = RegisterProperty<string>(nameof(Value1076));
  public string Value1076
  {
    get => GetProperty(Value1076Property);
    set => SetProperty(Value1076Property, value);
  }


  public static readonly PropertyInfo<string> Value1077Property = RegisterProperty<string>(nameof(Value1077));
  public string Value1077
  {
    get => GetProperty(Value1077Property);
    set => SetProperty(Value1077Property, value);
  }


  public static readonly PropertyInfo<string> Value1078Property = RegisterProperty<string>(nameof(Value1078));
  public string Value1078
  {
    get => GetProperty(Value1078Property);
    set => SetProperty(Value1078Property, value);
  }


  public static readonly PropertyInfo<string> Value1079Property = RegisterProperty<string>(nameof(Value1079));
  public string Value1079
  {
    get => GetProperty(Value1079Property);
    set => SetProperty(Value1079Property, value);
  }


  public static readonly PropertyInfo<string> Value1080Property = RegisterProperty<string>(nameof(Value1080));
  public string Value1080
  {
    get => GetProperty(Value1080Property);
    set => SetProperty(Value1080Property, value);
  }


  public static readonly PropertyInfo<string> Value1081Property = RegisterProperty<string>(nameof(Value1081));
  public string Value1081
  {
    get => GetProperty(Value1081Property);
    set => SetProperty(Value1081Property, value);
  }


  public static readonly PropertyInfo<string> Value1082Property = RegisterProperty<string>(nameof(Value1082));
  public string Value1082
  {
    get => GetProperty(Value1082Property);
    set => SetProperty(Value1082Property, value);
  }


  public static readonly PropertyInfo<string> Value1083Property = RegisterProperty<string>(nameof(Value1083));
  public string Value1083
  {
    get => GetProperty(Value1083Property);
    set => SetProperty(Value1083Property, value);
  }


  public static readonly PropertyInfo<string> Value1084Property = RegisterProperty<string>(nameof(Value1084));
  public string Value1084
  {
    get => GetProperty(Value1084Property);
    set => SetProperty(Value1084Property, value);
  }


  public static readonly PropertyInfo<string> Value1085Property = RegisterProperty<string>(nameof(Value1085));
  public string Value1085
  {
    get => GetProperty(Value1085Property);
    set => SetProperty(Value1085Property, value);
  }


  public static readonly PropertyInfo<string> Value1086Property = RegisterProperty<string>(nameof(Value1086));
  public string Value1086
  {
    get => GetProperty(Value1086Property);
    set => SetProperty(Value1086Property, value);
  }


  public static readonly PropertyInfo<string> Value1087Property = RegisterProperty<string>(nameof(Value1087));
  public string Value1087
  {
    get => GetProperty(Value1087Property);
    set => SetProperty(Value1087Property, value);
  }


  public static readonly PropertyInfo<string> Value1088Property = RegisterProperty<string>(nameof(Value1088));
  public string Value1088
  {
    get => GetProperty(Value1088Property);
    set => SetProperty(Value1088Property, value);
  }


  public static readonly PropertyInfo<string> Value1089Property = RegisterProperty<string>(nameof(Value1089));
  public string Value1089
  {
    get => GetProperty(Value1089Property);
    set => SetProperty(Value1089Property, value);
  }


  public static readonly PropertyInfo<string> Value1090Property = RegisterProperty<string>(nameof(Value1090));
  public string Value1090
  {
    get => GetProperty(Value1090Property);
    set => SetProperty(Value1090Property, value);
  }


  public static readonly PropertyInfo<string> Value1091Property = RegisterProperty<string>(nameof(Value1091));
  public string Value1091
  {
    get => GetProperty(Value1091Property);
    set => SetProperty(Value1091Property, value);
  }


  public static readonly PropertyInfo<string> Value1092Property = RegisterProperty<string>(nameof(Value1092));
  public string Value1092
  {
    get => GetProperty(Value1092Property);
    set => SetProperty(Value1092Property, value);
  }


  public static readonly PropertyInfo<string> Value1093Property = RegisterProperty<string>(nameof(Value1093));
  public string Value1093
  {
    get => GetProperty(Value1093Property);
    set => SetProperty(Value1093Property, value);
  }


  public static readonly PropertyInfo<string> Value1094Property = RegisterProperty<string>(nameof(Value1094));
  public string Value1094
  {
    get => GetProperty(Value1094Property);
    set => SetProperty(Value1094Property, value);
  }


  public static readonly PropertyInfo<string> Value1095Property = RegisterProperty<string>(nameof(Value1095));
  public string Value1095
  {
    get => GetProperty(Value1095Property);
    set => SetProperty(Value1095Property, value);
  }


  public static readonly PropertyInfo<string> Value1096Property = RegisterProperty<string>(nameof(Value1096));
  public string Value1096
  {
    get => GetProperty(Value1096Property);
    set => SetProperty(Value1096Property, value);
  }


  public static readonly PropertyInfo<string> Value1097Property = RegisterProperty<string>(nameof(Value1097));
  public string Value1097
  {
    get => GetProperty(Value1097Property);
    set => SetProperty(Value1097Property, value);
  }


  public static readonly PropertyInfo<string> Value1098Property = RegisterProperty<string>(nameof(Value1098));
  public string Value1098
  {
    get => GetProperty(Value1098Property);
    set => SetProperty(Value1098Property, value);
  }


  public static readonly PropertyInfo<string> Value1099Property = RegisterProperty<string>(nameof(Value1099));
  public string Value1099
  {
    get => GetProperty(Value1099Property);
    set => SetProperty(Value1099Property, value);
  }


  public static readonly PropertyInfo<string> Value1100Property = RegisterProperty<string>(nameof(Value1100));
  public string Value1100
  {
    get => GetProperty(Value1100Property);
    set => SetProperty(Value1100Property, value);
  }


  public static readonly PropertyInfo<string> Value1101Property = RegisterProperty<string>(nameof(Value1101));
  public string Value1101
  {
    get => GetProperty(Value1101Property);
    set => SetProperty(Value1101Property, value);
  }


  public static readonly PropertyInfo<string> Value1102Property = RegisterProperty<string>(nameof(Value1102));
  public string Value1102
  {
    get => GetProperty(Value1102Property);
    set => SetProperty(Value1102Property, value);
  }


  public static readonly PropertyInfo<string> Value1103Property = RegisterProperty<string>(nameof(Value1103));
  public string Value1103
  {
    get => GetProperty(Value1103Property);
    set => SetProperty(Value1103Property, value);
  }


  public static readonly PropertyInfo<string> Value1104Property = RegisterProperty<string>(nameof(Value1104));
  public string Value1104
  {
    get => GetProperty(Value1104Property);
    set => SetProperty(Value1104Property, value);
  }


  public static readonly PropertyInfo<string> Value1105Property = RegisterProperty<string>(nameof(Value1105));
  public string Value1105
  {
    get => GetProperty(Value1105Property);
    set => SetProperty(Value1105Property, value);
  }


  public static readonly PropertyInfo<string> Value1106Property = RegisterProperty<string>(nameof(Value1106));
  public string Value1106
  {
    get => GetProperty(Value1106Property);
    set => SetProperty(Value1106Property, value);
  }


  public static readonly PropertyInfo<string> Value1107Property = RegisterProperty<string>(nameof(Value1107));
  public string Value1107
  {
    get => GetProperty(Value1107Property);
    set => SetProperty(Value1107Property, value);
  }


  public static readonly PropertyInfo<string> Value1108Property = RegisterProperty<string>(nameof(Value1108));
  public string Value1108
  {
    get => GetProperty(Value1108Property);
    set => SetProperty(Value1108Property, value);
  }


  public static readonly PropertyInfo<string> Value1109Property = RegisterProperty<string>(nameof(Value1109));
  public string Value1109
  {
    get => GetProperty(Value1109Property);
    set => SetProperty(Value1109Property, value);
  }


  public static readonly PropertyInfo<string> Value1110Property = RegisterProperty<string>(nameof(Value1110));
  public string Value1110
  {
    get => GetProperty(Value1110Property);
    set => SetProperty(Value1110Property, value);
  }


  public static readonly PropertyInfo<string> Value1111Property = RegisterProperty<string>(nameof(Value1111));
  public string Value1111
  {
    get => GetProperty(Value1111Property);
    set => SetProperty(Value1111Property, value);
  }


  public static readonly PropertyInfo<string> Value1112Property = RegisterProperty<string>(nameof(Value1112));
  public string Value1112
  {
    get => GetProperty(Value1112Property);
    set => SetProperty(Value1112Property, value);
  }


  public static readonly PropertyInfo<string> Value1113Property = RegisterProperty<string>(nameof(Value1113));
  public string Value1113
  {
    get => GetProperty(Value1113Property);
    set => SetProperty(Value1113Property, value);
  }


  public static readonly PropertyInfo<string> Value1114Property = RegisterProperty<string>(nameof(Value1114));
  public string Value1114
  {
    get => GetProperty(Value1114Property);
    set => SetProperty(Value1114Property, value);
  }


  public static readonly PropertyInfo<string> Value1115Property = RegisterProperty<string>(nameof(Value1115));
  public string Value1115
  {
    get => GetProperty(Value1115Property);
    set => SetProperty(Value1115Property, value);
  }


  public static readonly PropertyInfo<string> Value1116Property = RegisterProperty<string>(nameof(Value1116));
  public string Value1116
  {
    get => GetProperty(Value1116Property);
    set => SetProperty(Value1116Property, value);
  }


  public static readonly PropertyInfo<string> Value1117Property = RegisterProperty<string>(nameof(Value1117));
  public string Value1117
  {
    get => GetProperty(Value1117Property);
    set => SetProperty(Value1117Property, value);
  }


  public static readonly PropertyInfo<string> Value1118Property = RegisterProperty<string>(nameof(Value1118));
  public string Value1118
  {
    get => GetProperty(Value1118Property);
    set => SetProperty(Value1118Property, value);
  }


  public static readonly PropertyInfo<string> Value1119Property = RegisterProperty<string>(nameof(Value1119));
  public string Value1119
  {
    get => GetProperty(Value1119Property);
    set => SetProperty(Value1119Property, value);
  }


  public static readonly PropertyInfo<string> Value1120Property = RegisterProperty<string>(nameof(Value1120));
  public string Value1120
  {
    get => GetProperty(Value1120Property);
    set => SetProperty(Value1120Property, value);
  }


  public static readonly PropertyInfo<string> Value1121Property = RegisterProperty<string>(nameof(Value1121));
  public string Value1121
  {
    get => GetProperty(Value1121Property);
    set => SetProperty(Value1121Property, value);
  }


  public static readonly PropertyInfo<string> Value1122Property = RegisterProperty<string>(nameof(Value1122));
  public string Value1122
  {
    get => GetProperty(Value1122Property);
    set => SetProperty(Value1122Property, value);
  }


  public static readonly PropertyInfo<string> Value1123Property = RegisterProperty<string>(nameof(Value1123));
  public string Value1123
  {
    get => GetProperty(Value1123Property);
    set => SetProperty(Value1123Property, value);
  }


  public static readonly PropertyInfo<string> Value1124Property = RegisterProperty<string>(nameof(Value1124));
  public string Value1124
  {
    get => GetProperty(Value1124Property);
    set => SetProperty(Value1124Property, value);
  }


  public static readonly PropertyInfo<string> Value1125Property = RegisterProperty<string>(nameof(Value1125));
  public string Value1125
  {
    get => GetProperty(Value1125Property);
    set => SetProperty(Value1125Property, value);
  }


  public static readonly PropertyInfo<string> Value1126Property = RegisterProperty<string>(nameof(Value1126));
  public string Value1126
  {
    get => GetProperty(Value1126Property);
    set => SetProperty(Value1126Property, value);
  }


  public static readonly PropertyInfo<string> Value1127Property = RegisterProperty<string>(nameof(Value1127));
  public string Value1127
  {
    get => GetProperty(Value1127Property);
    set => SetProperty(Value1127Property, value);
  }


  public static readonly PropertyInfo<string> Value1128Property = RegisterProperty<string>(nameof(Value1128));
  public string Value1128
  {
    get => GetProperty(Value1128Property);
    set => SetProperty(Value1128Property, value);
  }


  public static readonly PropertyInfo<string> Value1129Property = RegisterProperty<string>(nameof(Value1129));
  public string Value1129
  {
    get => GetProperty(Value1129Property);
    set => SetProperty(Value1129Property, value);
  }


  public static readonly PropertyInfo<string> Value1130Property = RegisterProperty<string>(nameof(Value1130));
  public string Value1130
  {
    get => GetProperty(Value1130Property);
    set => SetProperty(Value1130Property, value);
  }


  public static readonly PropertyInfo<string> Value1131Property = RegisterProperty<string>(nameof(Value1131));
  public string Value1131
  {
    get => GetProperty(Value1131Property);
    set => SetProperty(Value1131Property, value);
  }


  public static readonly PropertyInfo<string> Value1132Property = RegisterProperty<string>(nameof(Value1132));
  public string Value1132
  {
    get => GetProperty(Value1132Property);
    set => SetProperty(Value1132Property, value);
  }


  public static readonly PropertyInfo<string> Value1133Property = RegisterProperty<string>(nameof(Value1133));
  public string Value1133
  {
    get => GetProperty(Value1133Property);
    set => SetProperty(Value1133Property, value);
  }


  public static readonly PropertyInfo<string> Value1134Property = RegisterProperty<string>(nameof(Value1134));
  public string Value1134
  {
    get => GetProperty(Value1134Property);
    set => SetProperty(Value1134Property, value);
  }


  public static readonly PropertyInfo<string> Value1135Property = RegisterProperty<string>(nameof(Value1135));
  public string Value1135
  {
    get => GetProperty(Value1135Property);
    set => SetProperty(Value1135Property, value);
  }


  public static readonly PropertyInfo<string> Value1136Property = RegisterProperty<string>(nameof(Value1136));
  public string Value1136
  {
    get => GetProperty(Value1136Property);
    set => SetProperty(Value1136Property, value);
  }


  public static readonly PropertyInfo<string> Value1137Property = RegisterProperty<string>(nameof(Value1137));
  public string Value1137
  {
    get => GetProperty(Value1137Property);
    set => SetProperty(Value1137Property, value);
  }


  public static readonly PropertyInfo<string> Value1138Property = RegisterProperty<string>(nameof(Value1138));
  public string Value1138
  {
    get => GetProperty(Value1138Property);
    set => SetProperty(Value1138Property, value);
  }


  public static readonly PropertyInfo<string> Value1139Property = RegisterProperty<string>(nameof(Value1139));
  public string Value1139
  {
    get => GetProperty(Value1139Property);
    set => SetProperty(Value1139Property, value);
  }


  public static readonly PropertyInfo<string> Value1140Property = RegisterProperty<string>(nameof(Value1140));
  public string Value1140
  {
    get => GetProperty(Value1140Property);
    set => SetProperty(Value1140Property, value);
  }


  public static readonly PropertyInfo<string> Value1141Property = RegisterProperty<string>(nameof(Value1141));
  public string Value1141
  {
    get => GetProperty(Value1141Property);
    set => SetProperty(Value1141Property, value);
  }


  public static readonly PropertyInfo<string> Value1142Property = RegisterProperty<string>(nameof(Value1142));
  public string Value1142
  {
    get => GetProperty(Value1142Property);
    set => SetProperty(Value1142Property, value);
  }


  public static readonly PropertyInfo<string> Value1143Property = RegisterProperty<string>(nameof(Value1143));
  public string Value1143
  {
    get => GetProperty(Value1143Property);
    set => SetProperty(Value1143Property, value);
  }


  public static readonly PropertyInfo<string> Value1144Property = RegisterProperty<string>(nameof(Value1144));
  public string Value1144
  {
    get => GetProperty(Value1144Property);
    set => SetProperty(Value1144Property, value);
  }


  public static readonly PropertyInfo<string> Value1145Property = RegisterProperty<string>(nameof(Value1145));
  public string Value1145
  {
    get => GetProperty(Value1145Property);
    set => SetProperty(Value1145Property, value);
  }


  public static readonly PropertyInfo<string> Value1146Property = RegisterProperty<string>(nameof(Value1146));
  public string Value1146
  {
    get => GetProperty(Value1146Property);
    set => SetProperty(Value1146Property, value);
  }


  public static readonly PropertyInfo<string> Value1147Property = RegisterProperty<string>(nameof(Value1147));
  public string Value1147
  {
    get => GetProperty(Value1147Property);
    set => SetProperty(Value1147Property, value);
  }


  public static readonly PropertyInfo<string> Value1148Property = RegisterProperty<string>(nameof(Value1148));
  public string Value1148
  {
    get => GetProperty(Value1148Property);
    set => SetProperty(Value1148Property, value);
  }


  public static readonly PropertyInfo<string> Value1149Property = RegisterProperty<string>(nameof(Value1149));
  public string Value1149
  {
    get => GetProperty(Value1149Property);
    set => SetProperty(Value1149Property, value);
  }


  public static readonly PropertyInfo<string> Value1150Property = RegisterProperty<string>(nameof(Value1150));
  public string Value1150
  {
    get => GetProperty(Value1150Property);
    set => SetProperty(Value1150Property, value);
  }


  public static readonly PropertyInfo<string> Value1151Property = RegisterProperty<string>(nameof(Value1151));
  public string Value1151
  {
    get => GetProperty(Value1151Property);
    set => SetProperty(Value1151Property, value);
  }


  public static readonly PropertyInfo<string> Value1152Property = RegisterProperty<string>(nameof(Value1152));
  public string Value1152
  {
    get => GetProperty(Value1152Property);
    set => SetProperty(Value1152Property, value);
  }


  public static readonly PropertyInfo<string> Value1153Property = RegisterProperty<string>(nameof(Value1153));
  public string Value1153
  {
    get => GetProperty(Value1153Property);
    set => SetProperty(Value1153Property, value);
  }


  public static readonly PropertyInfo<string> Value1154Property = RegisterProperty<string>(nameof(Value1154));
  public string Value1154
  {
    get => GetProperty(Value1154Property);
    set => SetProperty(Value1154Property, value);
  }


  public static readonly PropertyInfo<string> Value1155Property = RegisterProperty<string>(nameof(Value1155));
  public string Value1155
  {
    get => GetProperty(Value1155Property);
    set => SetProperty(Value1155Property, value);
  }


  public static readonly PropertyInfo<string> Value1156Property = RegisterProperty<string>(nameof(Value1156));
  public string Value1156
  {
    get => GetProperty(Value1156Property);
    set => SetProperty(Value1156Property, value);
  }


  public static readonly PropertyInfo<string> Value1157Property = RegisterProperty<string>(nameof(Value1157));
  public string Value1157
  {
    get => GetProperty(Value1157Property);
    set => SetProperty(Value1157Property, value);
  }


  public static readonly PropertyInfo<string> Value1158Property = RegisterProperty<string>(nameof(Value1158));
  public string Value1158
  {
    get => GetProperty(Value1158Property);
    set => SetProperty(Value1158Property, value);
  }


  public static readonly PropertyInfo<string> Value1159Property = RegisterProperty<string>(nameof(Value1159));
  public string Value1159
  {
    get => GetProperty(Value1159Property);
    set => SetProperty(Value1159Property, value);
  }


  public static readonly PropertyInfo<string> Value1160Property = RegisterProperty<string>(nameof(Value1160));
  public string Value1160
  {
    get => GetProperty(Value1160Property);
    set => SetProperty(Value1160Property, value);
  }


  public static readonly PropertyInfo<string> Value1161Property = RegisterProperty<string>(nameof(Value1161));
  public string Value1161
  {
    get => GetProperty(Value1161Property);
    set => SetProperty(Value1161Property, value);
  }


  public static readonly PropertyInfo<string> Value1162Property = RegisterProperty<string>(nameof(Value1162));
  public string Value1162
  {
    get => GetProperty(Value1162Property);
    set => SetProperty(Value1162Property, value);
  }


  public static readonly PropertyInfo<string> Value1163Property = RegisterProperty<string>(nameof(Value1163));
  public string Value1163
  {
    get => GetProperty(Value1163Property);
    set => SetProperty(Value1163Property, value);
  }


  public static readonly PropertyInfo<string> Value1164Property = RegisterProperty<string>(nameof(Value1164));
  public string Value1164
  {
    get => GetProperty(Value1164Property);
    set => SetProperty(Value1164Property, value);
  }


  public static readonly PropertyInfo<string> Value1165Property = RegisterProperty<string>(nameof(Value1165));
  public string Value1165
  {
    get => GetProperty(Value1165Property);
    set => SetProperty(Value1165Property, value);
  }


  public static readonly PropertyInfo<string> Value1166Property = RegisterProperty<string>(nameof(Value1166));
  public string Value1166
  {
    get => GetProperty(Value1166Property);
    set => SetProperty(Value1166Property, value);
  }


  public static readonly PropertyInfo<string> Value1167Property = RegisterProperty<string>(nameof(Value1167));
  public string Value1167
  {
    get => GetProperty(Value1167Property);
    set => SetProperty(Value1167Property, value);
  }


  public static readonly PropertyInfo<string> Value1168Property = RegisterProperty<string>(nameof(Value1168));
  public string Value1168
  {
    get => GetProperty(Value1168Property);
    set => SetProperty(Value1168Property, value);
  }


  public static readonly PropertyInfo<string> Value1169Property = RegisterProperty<string>(nameof(Value1169));
  public string Value1169
  {
    get => GetProperty(Value1169Property);
    set => SetProperty(Value1169Property, value);
  }


  public static readonly PropertyInfo<string> Value1170Property = RegisterProperty<string>(nameof(Value1170));
  public string Value1170
  {
    get => GetProperty(Value1170Property);
    set => SetProperty(Value1170Property, value);
  }


  public static readonly PropertyInfo<string> Value1171Property = RegisterProperty<string>(nameof(Value1171));
  public string Value1171
  {
    get => GetProperty(Value1171Property);
    set => SetProperty(Value1171Property, value);
  }


  public static readonly PropertyInfo<string> Value1172Property = RegisterProperty<string>(nameof(Value1172));
  public string Value1172
  {
    get => GetProperty(Value1172Property);
    set => SetProperty(Value1172Property, value);
  }


  public static readonly PropertyInfo<string> Value1173Property = RegisterProperty<string>(nameof(Value1173));
  public string Value1173
  {
    get => GetProperty(Value1173Property);
    set => SetProperty(Value1173Property, value);
  }


  public static readonly PropertyInfo<string> Value1174Property = RegisterProperty<string>(nameof(Value1174));
  public string Value1174
  {
    get => GetProperty(Value1174Property);
    set => SetProperty(Value1174Property, value);
  }


  public static readonly PropertyInfo<string> Value1175Property = RegisterProperty<string>(nameof(Value1175));
  public string Value1175
  {
    get => GetProperty(Value1175Property);
    set => SetProperty(Value1175Property, value);
  }


  public static readonly PropertyInfo<string> Value1176Property = RegisterProperty<string>(nameof(Value1176));
  public string Value1176
  {
    get => GetProperty(Value1176Property);
    set => SetProperty(Value1176Property, value);
  }


  public static readonly PropertyInfo<string> Value1177Property = RegisterProperty<string>(nameof(Value1177));
  public string Value1177
  {
    get => GetProperty(Value1177Property);
    set => SetProperty(Value1177Property, value);
  }


  public static readonly PropertyInfo<string> Value1178Property = RegisterProperty<string>(nameof(Value1178));
  public string Value1178
  {
    get => GetProperty(Value1178Property);
    set => SetProperty(Value1178Property, value);
  }


  public static readonly PropertyInfo<string> Value1179Property = RegisterProperty<string>(nameof(Value1179));
  public string Value1179
  {
    get => GetProperty(Value1179Property);
    set => SetProperty(Value1179Property, value);
  }


  public static readonly PropertyInfo<string> Value1180Property = RegisterProperty<string>(nameof(Value1180));
  public string Value1180
  {
    get => GetProperty(Value1180Property);
    set => SetProperty(Value1180Property, value);
  }


  public static readonly PropertyInfo<string> Value1181Property = RegisterProperty<string>(nameof(Value1181));
  public string Value1181
  {
    get => GetProperty(Value1181Property);
    set => SetProperty(Value1181Property, value);
  }


  public static readonly PropertyInfo<string> Value1182Property = RegisterProperty<string>(nameof(Value1182));
  public string Value1182
  {
    get => GetProperty(Value1182Property);
    set => SetProperty(Value1182Property, value);
  }


  public static readonly PropertyInfo<string> Value1183Property = RegisterProperty<string>(nameof(Value1183));
  public string Value1183
  {
    get => GetProperty(Value1183Property);
    set => SetProperty(Value1183Property, value);
  }


  public static readonly PropertyInfo<string> Value1184Property = RegisterProperty<string>(nameof(Value1184));
  public string Value1184
  {
    get => GetProperty(Value1184Property);
    set => SetProperty(Value1184Property, value);
  }


  public static readonly PropertyInfo<string> Value1185Property = RegisterProperty<string>(nameof(Value1185));
  public string Value1185
  {
    get => GetProperty(Value1185Property);
    set => SetProperty(Value1185Property, value);
  }


  public static readonly PropertyInfo<string> Value1186Property = RegisterProperty<string>(nameof(Value1186));
  public string Value1186
  {
    get => GetProperty(Value1186Property);
    set => SetProperty(Value1186Property, value);
  }


  public static readonly PropertyInfo<string> Value1187Property = RegisterProperty<string>(nameof(Value1187));
  public string Value1187
  {
    get => GetProperty(Value1187Property);
    set => SetProperty(Value1187Property, value);
  }


  public static readonly PropertyInfo<string> Value1188Property = RegisterProperty<string>(nameof(Value1188));
  public string Value1188
  {
    get => GetProperty(Value1188Property);
    set => SetProperty(Value1188Property, value);
  }


  public static readonly PropertyInfo<string> Value1189Property = RegisterProperty<string>(nameof(Value1189));
  public string Value1189
  {
    get => GetProperty(Value1189Property);
    set => SetProperty(Value1189Property, value);
  }


  public static readonly PropertyInfo<string> Value1190Property = RegisterProperty<string>(nameof(Value1190));
  public string Value1190
  {
    get => GetProperty(Value1190Property);
    set => SetProperty(Value1190Property, value);
  }


  public static readonly PropertyInfo<string> Value1191Property = RegisterProperty<string>(nameof(Value1191));
  public string Value1191
  {
    get => GetProperty(Value1191Property);
    set => SetProperty(Value1191Property, value);
  }


  public static readonly PropertyInfo<string> Value1192Property = RegisterProperty<string>(nameof(Value1192));
  public string Value1192
  {
    get => GetProperty(Value1192Property);
    set => SetProperty(Value1192Property, value);
  }


  public static readonly PropertyInfo<string> Value1193Property = RegisterProperty<string>(nameof(Value1193));
  public string Value1193
  {
    get => GetProperty(Value1193Property);
    set => SetProperty(Value1193Property, value);
  }


  public static readonly PropertyInfo<string> Value1194Property = RegisterProperty<string>(nameof(Value1194));
  public string Value1194
  {
    get => GetProperty(Value1194Property);
    set => SetProperty(Value1194Property, value);
  }


  public static readonly PropertyInfo<string> Value1195Property = RegisterProperty<string>(nameof(Value1195));
  public string Value1195
  {
    get => GetProperty(Value1195Property);
    set => SetProperty(Value1195Property, value);
  }


  public static readonly PropertyInfo<string> Value1196Property = RegisterProperty<string>(nameof(Value1196));
  public string Value1196
  {
    get => GetProperty(Value1196Property);
    set => SetProperty(Value1196Property, value);
  }


  public static readonly PropertyInfo<string> Value1197Property = RegisterProperty<string>(nameof(Value1197));
  public string Value1197
  {
    get => GetProperty(Value1197Property);
    set => SetProperty(Value1197Property, value);
  }


  public static readonly PropertyInfo<string> Value1198Property = RegisterProperty<string>(nameof(Value1198));
  public string Value1198
  {
    get => GetProperty(Value1198Property);
    set => SetProperty(Value1198Property, value);
  }


  public static readonly PropertyInfo<string> Value1199Property = RegisterProperty<string>(nameof(Value1199));
  public string Value1199
  {
    get => GetProperty(Value1199Property);
    set => SetProperty(Value1199Property, value);
  }


  public static readonly PropertyInfo<string> Value1200Property = RegisterProperty<string>(nameof(Value1200));
  public string Value1200
  {
    get => GetProperty(Value1200Property);
    set => SetProperty(Value1200Property, value);
  }


  public static readonly PropertyInfo<string> Value1201Property = RegisterProperty<string>(nameof(Value1201));
  public string Value1201
  {
    get => GetProperty(Value1201Property);
    set => SetProperty(Value1201Property, value);
  }


  public static readonly PropertyInfo<string> Value1202Property = RegisterProperty<string>(nameof(Value1202));
  public string Value1202
  {
    get => GetProperty(Value1202Property);
    set => SetProperty(Value1202Property, value);
  }


  public static readonly PropertyInfo<string> Value1203Property = RegisterProperty<string>(nameof(Value1203));
  public string Value1203
  {
    get => GetProperty(Value1203Property);
    set => SetProperty(Value1203Property, value);
  }


  public static readonly PropertyInfo<string> Value1204Property = RegisterProperty<string>(nameof(Value1204));
  public string Value1204
  {
    get => GetProperty(Value1204Property);
    set => SetProperty(Value1204Property, value);
  }


  public static readonly PropertyInfo<string> Value1205Property = RegisterProperty<string>(nameof(Value1205));
  public string Value1205
  {
    get => GetProperty(Value1205Property);
    set => SetProperty(Value1205Property, value);
  }


  public static readonly PropertyInfo<string> Value1206Property = RegisterProperty<string>(nameof(Value1206));
  public string Value1206
  {
    get => GetProperty(Value1206Property);
    set => SetProperty(Value1206Property, value);
  }


  public static readonly PropertyInfo<string> Value1207Property = RegisterProperty<string>(nameof(Value1207));
  public string Value1207
  {
    get => GetProperty(Value1207Property);
    set => SetProperty(Value1207Property, value);
  }


  public static readonly PropertyInfo<string> Value1208Property = RegisterProperty<string>(nameof(Value1208));
  public string Value1208
  {
    get => GetProperty(Value1208Property);
    set => SetProperty(Value1208Property, value);
  }


  public static readonly PropertyInfo<string> Value1209Property = RegisterProperty<string>(nameof(Value1209));
  public string Value1209
  {
    get => GetProperty(Value1209Property);
    set => SetProperty(Value1209Property, value);
  }


  public static readonly PropertyInfo<string> Value1210Property = RegisterProperty<string>(nameof(Value1210));
  public string Value1210
  {
    get => GetProperty(Value1210Property);
    set => SetProperty(Value1210Property, value);
  }


  public static readonly PropertyInfo<string> Value1211Property = RegisterProperty<string>(nameof(Value1211));
  public string Value1211
  {
    get => GetProperty(Value1211Property);
    set => SetProperty(Value1211Property, value);
  }


  public static readonly PropertyInfo<string> Value1212Property = RegisterProperty<string>(nameof(Value1212));
  public string Value1212
  {
    get => GetProperty(Value1212Property);
    set => SetProperty(Value1212Property, value);
  }


  public static readonly PropertyInfo<string> Value1213Property = RegisterProperty<string>(nameof(Value1213));
  public string Value1213
  {
    get => GetProperty(Value1213Property);
    set => SetProperty(Value1213Property, value);
  }


  public static readonly PropertyInfo<string> Value1214Property = RegisterProperty<string>(nameof(Value1214));
  public string Value1214
  {
    get => GetProperty(Value1214Property);
    set => SetProperty(Value1214Property, value);
  }


  public static readonly PropertyInfo<string> Value1215Property = RegisterProperty<string>(nameof(Value1215));
  public string Value1215
  {
    get => GetProperty(Value1215Property);
    set => SetProperty(Value1215Property, value);
  }


  public static readonly PropertyInfo<string> Value1216Property = RegisterProperty<string>(nameof(Value1216));
  public string Value1216
  {
    get => GetProperty(Value1216Property);
    set => SetProperty(Value1216Property, value);
  }


  public static readonly PropertyInfo<string> Value1217Property = RegisterProperty<string>(nameof(Value1217));
  public string Value1217
  {
    get => GetProperty(Value1217Property);
    set => SetProperty(Value1217Property, value);
  }


  public static readonly PropertyInfo<string> Value1218Property = RegisterProperty<string>(nameof(Value1218));
  public string Value1218
  {
    get => GetProperty(Value1218Property);
    set => SetProperty(Value1218Property, value);
  }


  public static readonly PropertyInfo<string> Value1219Property = RegisterProperty<string>(nameof(Value1219));
  public string Value1219
  {
    get => GetProperty(Value1219Property);
    set => SetProperty(Value1219Property, value);
  }


  public static readonly PropertyInfo<string> Value1220Property = RegisterProperty<string>(nameof(Value1220));
  public string Value1220
  {
    get => GetProperty(Value1220Property);
    set => SetProperty(Value1220Property, value);
  }


  public static readonly PropertyInfo<string> Value1221Property = RegisterProperty<string>(nameof(Value1221));
  public string Value1221
  {
    get => GetProperty(Value1221Property);
    set => SetProperty(Value1221Property, value);
  }


  public static readonly PropertyInfo<string> Value1222Property = RegisterProperty<string>(nameof(Value1222));
  public string Value1222
  {
    get => GetProperty(Value1222Property);
    set => SetProperty(Value1222Property, value);
  }


  public static readonly PropertyInfo<string> Value1223Property = RegisterProperty<string>(nameof(Value1223));
  public string Value1223
  {
    get => GetProperty(Value1223Property);
    set => SetProperty(Value1223Property, value);
  }


  public static readonly PropertyInfo<string> Value1224Property = RegisterProperty<string>(nameof(Value1224));
  public string Value1224
  {
    get => GetProperty(Value1224Property);
    set => SetProperty(Value1224Property, value);
  }


  public static readonly PropertyInfo<string> Value1225Property = RegisterProperty<string>(nameof(Value1225));
  public string Value1225
  {
    get => GetProperty(Value1225Property);
    set => SetProperty(Value1225Property, value);
  }


  public static readonly PropertyInfo<string> Value1226Property = RegisterProperty<string>(nameof(Value1226));
  public string Value1226
  {
    get => GetProperty(Value1226Property);
    set => SetProperty(Value1226Property, value);
  }


  public static readonly PropertyInfo<string> Value1227Property = RegisterProperty<string>(nameof(Value1227));
  public string Value1227
  {
    get => GetProperty(Value1227Property);
    set => SetProperty(Value1227Property, value);
  }


  public static readonly PropertyInfo<string> Value1228Property = RegisterProperty<string>(nameof(Value1228));
  public string Value1228
  {
    get => GetProperty(Value1228Property);
    set => SetProperty(Value1228Property, value);
  }


  public static readonly PropertyInfo<string> Value1229Property = RegisterProperty<string>(nameof(Value1229));
  public string Value1229
  {
    get => GetProperty(Value1229Property);
    set => SetProperty(Value1229Property, value);
  }


  public static readonly PropertyInfo<string> Value1230Property = RegisterProperty<string>(nameof(Value1230));
  public string Value1230
  {
    get => GetProperty(Value1230Property);
    set => SetProperty(Value1230Property, value);
  }


  public static readonly PropertyInfo<string> Value1231Property = RegisterProperty<string>(nameof(Value1231));
  public string Value1231
  {
    get => GetProperty(Value1231Property);
    set => SetProperty(Value1231Property, value);
  }


  public static readonly PropertyInfo<string> Value1232Property = RegisterProperty<string>(nameof(Value1232));
  public string Value1232
  {
    get => GetProperty(Value1232Property);
    set => SetProperty(Value1232Property, value);
  }


  public static readonly PropertyInfo<string> Value1233Property = RegisterProperty<string>(nameof(Value1233));
  public string Value1233
  {
    get => GetProperty(Value1233Property);
    set => SetProperty(Value1233Property, value);
  }


  public static readonly PropertyInfo<string> Value1234Property = RegisterProperty<string>(nameof(Value1234));
  public string Value1234
  {
    get => GetProperty(Value1234Property);
    set => SetProperty(Value1234Property, value);
  }


  public static readonly PropertyInfo<string> Value1235Property = RegisterProperty<string>(nameof(Value1235));
  public string Value1235
  {
    get => GetProperty(Value1235Property);
    set => SetProperty(Value1235Property, value);
  }


  public static readonly PropertyInfo<string> Value1236Property = RegisterProperty<string>(nameof(Value1236));
  public string Value1236
  {
    get => GetProperty(Value1236Property);
    set => SetProperty(Value1236Property, value);
  }


  public static readonly PropertyInfo<string> Value1237Property = RegisterProperty<string>(nameof(Value1237));
  public string Value1237
  {
    get => GetProperty(Value1237Property);
    set => SetProperty(Value1237Property, value);
  }


  public static readonly PropertyInfo<string> Value1238Property = RegisterProperty<string>(nameof(Value1238));
  public string Value1238
  {
    get => GetProperty(Value1238Property);
    set => SetProperty(Value1238Property, value);
  }


  public static readonly PropertyInfo<string> Value1239Property = RegisterProperty<string>(nameof(Value1239));
  public string Value1239
  {
    get => GetProperty(Value1239Property);
    set => SetProperty(Value1239Property, value);
  }


  public static readonly PropertyInfo<string> Value1240Property = RegisterProperty<string>(nameof(Value1240));
  public string Value1240
  {
    get => GetProperty(Value1240Property);
    set => SetProperty(Value1240Property, value);
  }


  public static readonly PropertyInfo<string> Value1241Property = RegisterProperty<string>(nameof(Value1241));
  public string Value1241
  {
    get => GetProperty(Value1241Property);
    set => SetProperty(Value1241Property, value);
  }


  public static readonly PropertyInfo<string> Value1242Property = RegisterProperty<string>(nameof(Value1242));
  public string Value1242
  {
    get => GetProperty(Value1242Property);
    set => SetProperty(Value1242Property, value);
  }


  public static readonly PropertyInfo<string> Value1243Property = RegisterProperty<string>(nameof(Value1243));
  public string Value1243
  {
    get => GetProperty(Value1243Property);
    set => SetProperty(Value1243Property, value);
  }


  public static readonly PropertyInfo<string> Value1244Property = RegisterProperty<string>(nameof(Value1244));
  public string Value1244
  {
    get => GetProperty(Value1244Property);
    set => SetProperty(Value1244Property, value);
  }


  public static readonly PropertyInfo<string> Value1245Property = RegisterProperty<string>(nameof(Value1245));
  public string Value1245
  {
    get => GetProperty(Value1245Property);
    set => SetProperty(Value1245Property, value);
  }


  public static readonly PropertyInfo<string> Value1246Property = RegisterProperty<string>(nameof(Value1246));
  public string Value1246
  {
    get => GetProperty(Value1246Property);
    set => SetProperty(Value1246Property, value);
  }


  public static readonly PropertyInfo<string> Value1247Property = RegisterProperty<string>(nameof(Value1247));
  public string Value1247
  {
    get => GetProperty(Value1247Property);
    set => SetProperty(Value1247Property, value);
  }


  public static readonly PropertyInfo<string> Value1248Property = RegisterProperty<string>(nameof(Value1248));
  public string Value1248
  {
    get => GetProperty(Value1248Property);
    set => SetProperty(Value1248Property, value);
  }


  public static readonly PropertyInfo<string> Value1249Property = RegisterProperty<string>(nameof(Value1249));
  public string Value1249
  {
    get => GetProperty(Value1249Property);
    set => SetProperty(Value1249Property, value);
  }


  public static readonly PropertyInfo<string> Value1250Property = RegisterProperty<string>(nameof(Value1250));
  public string Value1250
  {
    get => GetProperty(Value1250Property);
    set => SetProperty(Value1250Property, value);
  }


  public static readonly PropertyInfo<string> Value1251Property = RegisterProperty<string>(nameof(Value1251));
  public string Value1251
  {
    get => GetProperty(Value1251Property);
    set => SetProperty(Value1251Property, value);
  }


  public static readonly PropertyInfo<string> Value1252Property = RegisterProperty<string>(nameof(Value1252));
  public string Value1252
  {
    get => GetProperty(Value1252Property);
    set => SetProperty(Value1252Property, value);
  }


  public static readonly PropertyInfo<string> Value1253Property = RegisterProperty<string>(nameof(Value1253));
  public string Value1253
  {
    get => GetProperty(Value1253Property);
    set => SetProperty(Value1253Property, value);
  }


  public static readonly PropertyInfo<string> Value1254Property = RegisterProperty<string>(nameof(Value1254));
  public string Value1254
  {
    get => GetProperty(Value1254Property);
    set => SetProperty(Value1254Property, value);
  }


  public static readonly PropertyInfo<string> Value1255Property = RegisterProperty<string>(nameof(Value1255));
  public string Value1255
  {
    get => GetProperty(Value1255Property);
    set => SetProperty(Value1255Property, value);
  }


  public static readonly PropertyInfo<string> Value1256Property = RegisterProperty<string>(nameof(Value1256));
  public string Value1256
  {
    get => GetProperty(Value1256Property);
    set => SetProperty(Value1256Property, value);
  }


  public static readonly PropertyInfo<string> Value1257Property = RegisterProperty<string>(nameof(Value1257));
  public string Value1257
  {
    get => GetProperty(Value1257Property);
    set => SetProperty(Value1257Property, value);
  }


  public static readonly PropertyInfo<string> Value1258Property = RegisterProperty<string>(nameof(Value1258));
  public string Value1258
  {
    get => GetProperty(Value1258Property);
    set => SetProperty(Value1258Property, value);
  }


  public static readonly PropertyInfo<string> Value1259Property = RegisterProperty<string>(nameof(Value1259));
  public string Value1259
  {
    get => GetProperty(Value1259Property);
    set => SetProperty(Value1259Property, value);
  }


  public static readonly PropertyInfo<string> Value1260Property = RegisterProperty<string>(nameof(Value1260));
  public string Value1260
  {
    get => GetProperty(Value1260Property);
    set => SetProperty(Value1260Property, value);
  }


  public static readonly PropertyInfo<string> Value1261Property = RegisterProperty<string>(nameof(Value1261));
  public string Value1261
  {
    get => GetProperty(Value1261Property);
    set => SetProperty(Value1261Property, value);
  }


  public static readonly PropertyInfo<string> Value1262Property = RegisterProperty<string>(nameof(Value1262));
  public string Value1262
  {
    get => GetProperty(Value1262Property);
    set => SetProperty(Value1262Property, value);
  }


  public static readonly PropertyInfo<string> Value1263Property = RegisterProperty<string>(nameof(Value1263));
  public string Value1263
  {
    get => GetProperty(Value1263Property);
    set => SetProperty(Value1263Property, value);
  }


  public static readonly PropertyInfo<string> Value1264Property = RegisterProperty<string>(nameof(Value1264));
  public string Value1264
  {
    get => GetProperty(Value1264Property);
    set => SetProperty(Value1264Property, value);
  }


  public static readonly PropertyInfo<string> Value1265Property = RegisterProperty<string>(nameof(Value1265));
  public string Value1265
  {
    get => GetProperty(Value1265Property);
    set => SetProperty(Value1265Property, value);
  }


  public static readonly PropertyInfo<string> Value1266Property = RegisterProperty<string>(nameof(Value1266));
  public string Value1266
  {
    get => GetProperty(Value1266Property);
    set => SetProperty(Value1266Property, value);
  }


  public static readonly PropertyInfo<string> Value1267Property = RegisterProperty<string>(nameof(Value1267));
  public string Value1267
  {
    get => GetProperty(Value1267Property);
    set => SetProperty(Value1267Property, value);
  }


  public static readonly PropertyInfo<string> Value1268Property = RegisterProperty<string>(nameof(Value1268));
  public string Value1268
  {
    get => GetProperty(Value1268Property);
    set => SetProperty(Value1268Property, value);
  }


  public static readonly PropertyInfo<string> Value1269Property = RegisterProperty<string>(nameof(Value1269));
  public string Value1269
  {
    get => GetProperty(Value1269Property);
    set => SetProperty(Value1269Property, value);
  }


  public static readonly PropertyInfo<string> Value1270Property = RegisterProperty<string>(nameof(Value1270));
  public string Value1270
  {
    get => GetProperty(Value1270Property);
    set => SetProperty(Value1270Property, value);
  }


  public static readonly PropertyInfo<string> Value1271Property = RegisterProperty<string>(nameof(Value1271));
  public string Value1271
  {
    get => GetProperty(Value1271Property);
    set => SetProperty(Value1271Property, value);
  }


  public static readonly PropertyInfo<string> Value1272Property = RegisterProperty<string>(nameof(Value1272));
  public string Value1272
  {
    get => GetProperty(Value1272Property);
    set => SetProperty(Value1272Property, value);
  }


  public static readonly PropertyInfo<string> Value1273Property = RegisterProperty<string>(nameof(Value1273));
  public string Value1273
  {
    get => GetProperty(Value1273Property);
    set => SetProperty(Value1273Property, value);
  }


  public static readonly PropertyInfo<string> Value1274Property = RegisterProperty<string>(nameof(Value1274));
  public string Value1274
  {
    get => GetProperty(Value1274Property);
    set => SetProperty(Value1274Property, value);
  }


  public static readonly PropertyInfo<string> Value1275Property = RegisterProperty<string>(nameof(Value1275));
  public string Value1275
  {
    get => GetProperty(Value1275Property);
    set => SetProperty(Value1275Property, value);
  }


  public static readonly PropertyInfo<string> Value1276Property = RegisterProperty<string>(nameof(Value1276));
  public string Value1276
  {
    get => GetProperty(Value1276Property);
    set => SetProperty(Value1276Property, value);
  }


  public static readonly PropertyInfo<string> Value1277Property = RegisterProperty<string>(nameof(Value1277));
  public string Value1277
  {
    get => GetProperty(Value1277Property);
    set => SetProperty(Value1277Property, value);
  }


  public static readonly PropertyInfo<string> Value1278Property = RegisterProperty<string>(nameof(Value1278));
  public string Value1278
  {
    get => GetProperty(Value1278Property);
    set => SetProperty(Value1278Property, value);
  }


  public static readonly PropertyInfo<string> Value1279Property = RegisterProperty<string>(nameof(Value1279));
  public string Value1279
  {
    get => GetProperty(Value1279Property);
    set => SetProperty(Value1279Property, value);
  }


  public static readonly PropertyInfo<string> Value1280Property = RegisterProperty<string>(nameof(Value1280));
  public string Value1280
  {
    get => GetProperty(Value1280Property);
    set => SetProperty(Value1280Property, value);
  }


  public static readonly PropertyInfo<string> Value1281Property = RegisterProperty<string>(nameof(Value1281));
  public string Value1281
  {
    get => GetProperty(Value1281Property);
    set => SetProperty(Value1281Property, value);
  }


  public static readonly PropertyInfo<string> Value1282Property = RegisterProperty<string>(nameof(Value1282));
  public string Value1282
  {
    get => GetProperty(Value1282Property);
    set => SetProperty(Value1282Property, value);
  }


  public static readonly PropertyInfo<string> Value1283Property = RegisterProperty<string>(nameof(Value1283));
  public string Value1283
  {
    get => GetProperty(Value1283Property);
    set => SetProperty(Value1283Property, value);
  }


  public static readonly PropertyInfo<string> Value1284Property = RegisterProperty<string>(nameof(Value1284));
  public string Value1284
  {
    get => GetProperty(Value1284Property);
    set => SetProperty(Value1284Property, value);
  }


  public static readonly PropertyInfo<string> Value1285Property = RegisterProperty<string>(nameof(Value1285));
  public string Value1285
  {
    get => GetProperty(Value1285Property);
    set => SetProperty(Value1285Property, value);
  }


  public static readonly PropertyInfo<string> Value1286Property = RegisterProperty<string>(nameof(Value1286));
  public string Value1286
  {
    get => GetProperty(Value1286Property);
    set => SetProperty(Value1286Property, value);
  }


  public static readonly PropertyInfo<string> Value1287Property = RegisterProperty<string>(nameof(Value1287));
  public string Value1287
  {
    get => GetProperty(Value1287Property);
    set => SetProperty(Value1287Property, value);
  }


  public static readonly PropertyInfo<string> Value1288Property = RegisterProperty<string>(nameof(Value1288));
  public string Value1288
  {
    get => GetProperty(Value1288Property);
    set => SetProperty(Value1288Property, value);
  }


  public static readonly PropertyInfo<string> Value1289Property = RegisterProperty<string>(nameof(Value1289));
  public string Value1289
  {
    get => GetProperty(Value1289Property);
    set => SetProperty(Value1289Property, value);
  }


  public static readonly PropertyInfo<string> Value1290Property = RegisterProperty<string>(nameof(Value1290));
  public string Value1290
  {
    get => GetProperty(Value1290Property);
    set => SetProperty(Value1290Property, value);
  }


  public static readonly PropertyInfo<string> Value1291Property = RegisterProperty<string>(nameof(Value1291));
  public string Value1291
  {
    get => GetProperty(Value1291Property);
    set => SetProperty(Value1291Property, value);
  }


  public static readonly PropertyInfo<string> Value1292Property = RegisterProperty<string>(nameof(Value1292));
  public string Value1292
  {
    get => GetProperty(Value1292Property);
    set => SetProperty(Value1292Property, value);
  }


  public static readonly PropertyInfo<string> Value1293Property = RegisterProperty<string>(nameof(Value1293));
  public string Value1293
  {
    get => GetProperty(Value1293Property);
    set => SetProperty(Value1293Property, value);
  }


  public static readonly PropertyInfo<string> Value1294Property = RegisterProperty<string>(nameof(Value1294));
  public string Value1294
  {
    get => GetProperty(Value1294Property);
    set => SetProperty(Value1294Property, value);
  }


  public static readonly PropertyInfo<string> Value1295Property = RegisterProperty<string>(nameof(Value1295));
  public string Value1295
  {
    get => GetProperty(Value1295Property);
    set => SetProperty(Value1295Property, value);
  }


  public static readonly PropertyInfo<string> Value1296Property = RegisterProperty<string>(nameof(Value1296));
  public string Value1296
  {
    get => GetProperty(Value1296Property);
    set => SetProperty(Value1296Property, value);
  }


  public static readonly PropertyInfo<string> Value1297Property = RegisterProperty<string>(nameof(Value1297));
  public string Value1297
  {
    get => GetProperty(Value1297Property);
    set => SetProperty(Value1297Property, value);
  }


  public static readonly PropertyInfo<string> Value1298Property = RegisterProperty<string>(nameof(Value1298));
  public string Value1298
  {
    get => GetProperty(Value1298Property);
    set => SetProperty(Value1298Property, value);
  }


  public static readonly PropertyInfo<string> Value1299Property = RegisterProperty<string>(nameof(Value1299));
  public string Value1299
  {
    get => GetProperty(Value1299Property);
    set => SetProperty(Value1299Property, value);
  }


  public static readonly PropertyInfo<string> Value1300Property = RegisterProperty<string>(nameof(Value1300));
  public string Value1300
  {
    get => GetProperty(Value1300Property);
    set => SetProperty(Value1300Property, value);
  }


  public static readonly PropertyInfo<string> Value1301Property = RegisterProperty<string>(nameof(Value1301));
  public string Value1301
  {
    get => GetProperty(Value1301Property);
    set => SetProperty(Value1301Property, value);
  }


  public static readonly PropertyInfo<string> Value1302Property = RegisterProperty<string>(nameof(Value1302));
  public string Value1302
  {
    get => GetProperty(Value1302Property);
    set => SetProperty(Value1302Property, value);
  }


  public static readonly PropertyInfo<string> Value1303Property = RegisterProperty<string>(nameof(Value1303));
  public string Value1303
  {
    get => GetProperty(Value1303Property);
    set => SetProperty(Value1303Property, value);
  }


  public static readonly PropertyInfo<string> Value1304Property = RegisterProperty<string>(nameof(Value1304));
  public string Value1304
  {
    get => GetProperty(Value1304Property);
    set => SetProperty(Value1304Property, value);
  }


  public static readonly PropertyInfo<string> Value1305Property = RegisterProperty<string>(nameof(Value1305));
  public string Value1305
  {
    get => GetProperty(Value1305Property);
    set => SetProperty(Value1305Property, value);
  }


  public static readonly PropertyInfo<string> Value1306Property = RegisterProperty<string>(nameof(Value1306));
  public string Value1306
  {
    get => GetProperty(Value1306Property);
    set => SetProperty(Value1306Property, value);
  }


  public static readonly PropertyInfo<string> Value1307Property = RegisterProperty<string>(nameof(Value1307));
  public string Value1307
  {
    get => GetProperty(Value1307Property);
    set => SetProperty(Value1307Property, value);
  }


  public static readonly PropertyInfo<string> Value1308Property = RegisterProperty<string>(nameof(Value1308));
  public string Value1308
  {
    get => GetProperty(Value1308Property);
    set => SetProperty(Value1308Property, value);
  }


  public static readonly PropertyInfo<string> Value1309Property = RegisterProperty<string>(nameof(Value1309));
  public string Value1309
  {
    get => GetProperty(Value1309Property);
    set => SetProperty(Value1309Property, value);
  }


  public static readonly PropertyInfo<string> Value1310Property = RegisterProperty<string>(nameof(Value1310));
  public string Value1310
  {
    get => GetProperty(Value1310Property);
    set => SetProperty(Value1310Property, value);
  }


  public static readonly PropertyInfo<string> Value1311Property = RegisterProperty<string>(nameof(Value1311));
  public string Value1311
  {
    get => GetProperty(Value1311Property);
    set => SetProperty(Value1311Property, value);
  }


  public static readonly PropertyInfo<string> Value1312Property = RegisterProperty<string>(nameof(Value1312));
  public string Value1312
  {
    get => GetProperty(Value1312Property);
    set => SetProperty(Value1312Property, value);
  }


  public static readonly PropertyInfo<string> Value1313Property = RegisterProperty<string>(nameof(Value1313));
  public string Value1313
  {
    get => GetProperty(Value1313Property);
    set => SetProperty(Value1313Property, value);
  }


  public static readonly PropertyInfo<string> Value1314Property = RegisterProperty<string>(nameof(Value1314));
  public string Value1314
  {
    get => GetProperty(Value1314Property);
    set => SetProperty(Value1314Property, value);
  }


  public static readonly PropertyInfo<string> Value1315Property = RegisterProperty<string>(nameof(Value1315));
  public string Value1315
  {
    get => GetProperty(Value1315Property);
    set => SetProperty(Value1315Property, value);
  }


  public static readonly PropertyInfo<string> Value1316Property = RegisterProperty<string>(nameof(Value1316));
  public string Value1316
  {
    get => GetProperty(Value1316Property);
    set => SetProperty(Value1316Property, value);
  }


  public static readonly PropertyInfo<string> Value1317Property = RegisterProperty<string>(nameof(Value1317));
  public string Value1317
  {
    get => GetProperty(Value1317Property);
    set => SetProperty(Value1317Property, value);
  }


  public static readonly PropertyInfo<string> Value1318Property = RegisterProperty<string>(nameof(Value1318));
  public string Value1318
  {
    get => GetProperty(Value1318Property);
    set => SetProperty(Value1318Property, value);
  }


  public static readonly PropertyInfo<string> Value1319Property = RegisterProperty<string>(nameof(Value1319));
  public string Value1319
  {
    get => GetProperty(Value1319Property);
    set => SetProperty(Value1319Property, value);
  }


  public static readonly PropertyInfo<string> Value1320Property = RegisterProperty<string>(nameof(Value1320));
  public string Value1320
  {
    get => GetProperty(Value1320Property);
    set => SetProperty(Value1320Property, value);
  }


  public static readonly PropertyInfo<string> Value1321Property = RegisterProperty<string>(nameof(Value1321));
  public string Value1321
  {
    get => GetProperty(Value1321Property);
    set => SetProperty(Value1321Property, value);
  }


  public static readonly PropertyInfo<string> Value1322Property = RegisterProperty<string>(nameof(Value1322));
  public string Value1322
  {
    get => GetProperty(Value1322Property);
    set => SetProperty(Value1322Property, value);
  }


  public static readonly PropertyInfo<string> Value1323Property = RegisterProperty<string>(nameof(Value1323));
  public string Value1323
  {
    get => GetProperty(Value1323Property);
    set => SetProperty(Value1323Property, value);
  }


  public static readonly PropertyInfo<string> Value1324Property = RegisterProperty<string>(nameof(Value1324));
  public string Value1324
  {
    get => GetProperty(Value1324Property);
    set => SetProperty(Value1324Property, value);
  }


  public static readonly PropertyInfo<string> Value1325Property = RegisterProperty<string>(nameof(Value1325));
  public string Value1325
  {
    get => GetProperty(Value1325Property);
    set => SetProperty(Value1325Property, value);
  }


  public static readonly PropertyInfo<string> Value1326Property = RegisterProperty<string>(nameof(Value1326));
  public string Value1326
  {
    get => GetProperty(Value1326Property);
    set => SetProperty(Value1326Property, value);
  }


  public static readonly PropertyInfo<string> Value1327Property = RegisterProperty<string>(nameof(Value1327));
  public string Value1327
  {
    get => GetProperty(Value1327Property);
    set => SetProperty(Value1327Property, value);
  }


  public static readonly PropertyInfo<string> Value1328Property = RegisterProperty<string>(nameof(Value1328));
  public string Value1328
  {
    get => GetProperty(Value1328Property);
    set => SetProperty(Value1328Property, value);
  }


  public static readonly PropertyInfo<string> Value1329Property = RegisterProperty<string>(nameof(Value1329));
  public string Value1329
  {
    get => GetProperty(Value1329Property);
    set => SetProperty(Value1329Property, value);
  }


  public static readonly PropertyInfo<string> Value1330Property = RegisterProperty<string>(nameof(Value1330));
  public string Value1330
  {
    get => GetProperty(Value1330Property);
    set => SetProperty(Value1330Property, value);
  }


  public static readonly PropertyInfo<string> Value1331Property = RegisterProperty<string>(nameof(Value1331));
  public string Value1331
  {
    get => GetProperty(Value1331Property);
    set => SetProperty(Value1331Property, value);
  }


  public static readonly PropertyInfo<string> Value1332Property = RegisterProperty<string>(nameof(Value1332));
  public string Value1332
  {
    get => GetProperty(Value1332Property);
    set => SetProperty(Value1332Property, value);
  }


  public static readonly PropertyInfo<string> Value1333Property = RegisterProperty<string>(nameof(Value1333));
  public string Value1333
  {
    get => GetProperty(Value1333Property);
    set => SetProperty(Value1333Property, value);
  }


  public static readonly PropertyInfo<string> Value1334Property = RegisterProperty<string>(nameof(Value1334));
  public string Value1334
  {
    get => GetProperty(Value1334Property);
    set => SetProperty(Value1334Property, value);
  }


  public static readonly PropertyInfo<string> Value1335Property = RegisterProperty<string>(nameof(Value1335));
  public string Value1335
  {
    get => GetProperty(Value1335Property);
    set => SetProperty(Value1335Property, value);
  }


  public static readonly PropertyInfo<string> Value1336Property = RegisterProperty<string>(nameof(Value1336));
  public string Value1336
  {
    get => GetProperty(Value1336Property);
    set => SetProperty(Value1336Property, value);
  }


  public static readonly PropertyInfo<string> Value1337Property = RegisterProperty<string>(nameof(Value1337));
  public string Value1337
  {
    get => GetProperty(Value1337Property);
    set => SetProperty(Value1337Property, value);
  }


  public static readonly PropertyInfo<string> Value1338Property = RegisterProperty<string>(nameof(Value1338));
  public string Value1338
  {
    get => GetProperty(Value1338Property);
    set => SetProperty(Value1338Property, value);
  }


  public static readonly PropertyInfo<string> Value1339Property = RegisterProperty<string>(nameof(Value1339));
  public string Value1339
  {
    get => GetProperty(Value1339Property);
    set => SetProperty(Value1339Property, value);
  }


  public static readonly PropertyInfo<string> Value1340Property = RegisterProperty<string>(nameof(Value1340));
  public string Value1340
  {
    get => GetProperty(Value1340Property);
    set => SetProperty(Value1340Property, value);
  }


  public static readonly PropertyInfo<string> Value1341Property = RegisterProperty<string>(nameof(Value1341));
  public string Value1341
  {
    get => GetProperty(Value1341Property);
    set => SetProperty(Value1341Property, value);
  }


  public static readonly PropertyInfo<string> Value1342Property = RegisterProperty<string>(nameof(Value1342));
  public string Value1342
  {
    get => GetProperty(Value1342Property);
    set => SetProperty(Value1342Property, value);
  }


  public static readonly PropertyInfo<string> Value1343Property = RegisterProperty<string>(nameof(Value1343));
  public string Value1343
  {
    get => GetProperty(Value1343Property);
    set => SetProperty(Value1343Property, value);
  }


  public static readonly PropertyInfo<string> Value1344Property = RegisterProperty<string>(nameof(Value1344));
  public string Value1344
  {
    get => GetProperty(Value1344Property);
    set => SetProperty(Value1344Property, value);
  }


  public static readonly PropertyInfo<string> Value1345Property = RegisterProperty<string>(nameof(Value1345));
  public string Value1345
  {
    get => GetProperty(Value1345Property);
    set => SetProperty(Value1345Property, value);
  }


  public static readonly PropertyInfo<string> Value1346Property = RegisterProperty<string>(nameof(Value1346));
  public string Value1346
  {
    get => GetProperty(Value1346Property);
    set => SetProperty(Value1346Property, value);
  }


  public static readonly PropertyInfo<string> Value1347Property = RegisterProperty<string>(nameof(Value1347));
  public string Value1347
  {
    get => GetProperty(Value1347Property);
    set => SetProperty(Value1347Property, value);
  }


  public static readonly PropertyInfo<string> Value1348Property = RegisterProperty<string>(nameof(Value1348));
  public string Value1348
  {
    get => GetProperty(Value1348Property);
    set => SetProperty(Value1348Property, value);
  }


  public static readonly PropertyInfo<string> Value1349Property = RegisterProperty<string>(nameof(Value1349));
  public string Value1349
  {
    get => GetProperty(Value1349Property);
    set => SetProperty(Value1349Property, value);
  }


  public static readonly PropertyInfo<string> Value1350Property = RegisterProperty<string>(nameof(Value1350));
  public string Value1350
  {
    get => GetProperty(Value1350Property);
    set => SetProperty(Value1350Property, value);
  }


  public static readonly PropertyInfo<string> Value1351Property = RegisterProperty<string>(nameof(Value1351));
  public string Value1351
  {
    get => GetProperty(Value1351Property);
    set => SetProperty(Value1351Property, value);
  }


  public static readonly PropertyInfo<string> Value1352Property = RegisterProperty<string>(nameof(Value1352));
  public string Value1352
  {
    get => GetProperty(Value1352Property);
    set => SetProperty(Value1352Property, value);
  }


  public static readonly PropertyInfo<string> Value1353Property = RegisterProperty<string>(nameof(Value1353));
  public string Value1353
  {
    get => GetProperty(Value1353Property);
    set => SetProperty(Value1353Property, value);
  }


  public static readonly PropertyInfo<string> Value1354Property = RegisterProperty<string>(nameof(Value1354));
  public string Value1354
  {
    get => GetProperty(Value1354Property);
    set => SetProperty(Value1354Property, value);
  }


  public static readonly PropertyInfo<string> Value1355Property = RegisterProperty<string>(nameof(Value1355));
  public string Value1355
  {
    get => GetProperty(Value1355Property);
    set => SetProperty(Value1355Property, value);
  }


  public static readonly PropertyInfo<string> Value1356Property = RegisterProperty<string>(nameof(Value1356));
  public string Value1356
  {
    get => GetProperty(Value1356Property);
    set => SetProperty(Value1356Property, value);
  }


  public static readonly PropertyInfo<string> Value1357Property = RegisterProperty<string>(nameof(Value1357));
  public string Value1357
  {
    get => GetProperty(Value1357Property);
    set => SetProperty(Value1357Property, value);
  }


  public static readonly PropertyInfo<string> Value1358Property = RegisterProperty<string>(nameof(Value1358));
  public string Value1358
  {
    get => GetProperty(Value1358Property);
    set => SetProperty(Value1358Property, value);
  }


  public static readonly PropertyInfo<string> Value1359Property = RegisterProperty<string>(nameof(Value1359));
  public string Value1359
  {
    get => GetProperty(Value1359Property);
    set => SetProperty(Value1359Property, value);
  }


  public static readonly PropertyInfo<string> Value1360Property = RegisterProperty<string>(nameof(Value1360));
  public string Value1360
  {
    get => GetProperty(Value1360Property);
    set => SetProperty(Value1360Property, value);
  }


  public static readonly PropertyInfo<string> Value1361Property = RegisterProperty<string>(nameof(Value1361));
  public string Value1361
  {
    get => GetProperty(Value1361Property);
    set => SetProperty(Value1361Property, value);
  }


  public static readonly PropertyInfo<string> Value1362Property = RegisterProperty<string>(nameof(Value1362));
  public string Value1362
  {
    get => GetProperty(Value1362Property);
    set => SetProperty(Value1362Property, value);
  }


  public static readonly PropertyInfo<string> Value1363Property = RegisterProperty<string>(nameof(Value1363));
  public string Value1363
  {
    get => GetProperty(Value1363Property);
    set => SetProperty(Value1363Property, value);
  }


  public static readonly PropertyInfo<string> Value1364Property = RegisterProperty<string>(nameof(Value1364));
  public string Value1364
  {
    get => GetProperty(Value1364Property);
    set => SetProperty(Value1364Property, value);
  }


  public static readonly PropertyInfo<string> Value1365Property = RegisterProperty<string>(nameof(Value1365));
  public string Value1365
  {
    get => GetProperty(Value1365Property);
    set => SetProperty(Value1365Property, value);
  }


  public static readonly PropertyInfo<string> Value1366Property = RegisterProperty<string>(nameof(Value1366));
  public string Value1366
  {
    get => GetProperty(Value1366Property);
    set => SetProperty(Value1366Property, value);
  }


  public static readonly PropertyInfo<string> Value1367Property = RegisterProperty<string>(nameof(Value1367));
  public string Value1367
  {
    get => GetProperty(Value1367Property);
    set => SetProperty(Value1367Property, value);
  }


  public static readonly PropertyInfo<string> Value1368Property = RegisterProperty<string>(nameof(Value1368));
  public string Value1368
  {
    get => GetProperty(Value1368Property);
    set => SetProperty(Value1368Property, value);
  }


  public static readonly PropertyInfo<string> Value1369Property = RegisterProperty<string>(nameof(Value1369));
  public string Value1369
  {
    get => GetProperty(Value1369Property);
    set => SetProperty(Value1369Property, value);
  }


  public static readonly PropertyInfo<string> Value1370Property = RegisterProperty<string>(nameof(Value1370));
  public string Value1370
  {
    get => GetProperty(Value1370Property);
    set => SetProperty(Value1370Property, value);
  }


  public static readonly PropertyInfo<string> Value1371Property = RegisterProperty<string>(nameof(Value1371));
  public string Value1371
  {
    get => GetProperty(Value1371Property);
    set => SetProperty(Value1371Property, value);
  }


  public static readonly PropertyInfo<string> Value1372Property = RegisterProperty<string>(nameof(Value1372));
  public string Value1372
  {
    get => GetProperty(Value1372Property);
    set => SetProperty(Value1372Property, value);
  }


  public static readonly PropertyInfo<string> Value1373Property = RegisterProperty<string>(nameof(Value1373));
  public string Value1373
  {
    get => GetProperty(Value1373Property);
    set => SetProperty(Value1373Property, value);
  }


  public static readonly PropertyInfo<string> Value1374Property = RegisterProperty<string>(nameof(Value1374));
  public string Value1374
  {
    get => GetProperty(Value1374Property);
    set => SetProperty(Value1374Property, value);
  }


  public static readonly PropertyInfo<string> Value1375Property = RegisterProperty<string>(nameof(Value1375));
  public string Value1375
  {
    get => GetProperty(Value1375Property);
    set => SetProperty(Value1375Property, value);
  }


  public static readonly PropertyInfo<string> Value1376Property = RegisterProperty<string>(nameof(Value1376));
  public string Value1376
  {
    get => GetProperty(Value1376Property);
    set => SetProperty(Value1376Property, value);
  }


  public static readonly PropertyInfo<string> Value1377Property = RegisterProperty<string>(nameof(Value1377));
  public string Value1377
  {
    get => GetProperty(Value1377Property);
    set => SetProperty(Value1377Property, value);
  }


  public static readonly PropertyInfo<string> Value1378Property = RegisterProperty<string>(nameof(Value1378));
  public string Value1378
  {
    get => GetProperty(Value1378Property);
    set => SetProperty(Value1378Property, value);
  }


  public static readonly PropertyInfo<string> Value1379Property = RegisterProperty<string>(nameof(Value1379));
  public string Value1379
  {
    get => GetProperty(Value1379Property);
    set => SetProperty(Value1379Property, value);
  }


  public static readonly PropertyInfo<string> Value1380Property = RegisterProperty<string>(nameof(Value1380));
  public string Value1380
  {
    get => GetProperty(Value1380Property);
    set => SetProperty(Value1380Property, value);
  }


  public static readonly PropertyInfo<string> Value1381Property = RegisterProperty<string>(nameof(Value1381));
  public string Value1381
  {
    get => GetProperty(Value1381Property);
    set => SetProperty(Value1381Property, value);
  }


  public static readonly PropertyInfo<string> Value1382Property = RegisterProperty<string>(nameof(Value1382));
  public string Value1382
  {
    get => GetProperty(Value1382Property);
    set => SetProperty(Value1382Property, value);
  }


  public static readonly PropertyInfo<string> Value1383Property = RegisterProperty<string>(nameof(Value1383));
  public string Value1383
  {
    get => GetProperty(Value1383Property);
    set => SetProperty(Value1383Property, value);
  }


  public static readonly PropertyInfo<string> Value1384Property = RegisterProperty<string>(nameof(Value1384));
  public string Value1384
  {
    get => GetProperty(Value1384Property);
    set => SetProperty(Value1384Property, value);
  }


  public static readonly PropertyInfo<string> Value1385Property = RegisterProperty<string>(nameof(Value1385));
  public string Value1385
  {
    get => GetProperty(Value1385Property);
    set => SetProperty(Value1385Property, value);
  }


  public static readonly PropertyInfo<string> Value1386Property = RegisterProperty<string>(nameof(Value1386));
  public string Value1386
  {
    get => GetProperty(Value1386Property);
    set => SetProperty(Value1386Property, value);
  }


  public static readonly PropertyInfo<string> Value1387Property = RegisterProperty<string>(nameof(Value1387));
  public string Value1387
  {
    get => GetProperty(Value1387Property);
    set => SetProperty(Value1387Property, value);
  }


  public static readonly PropertyInfo<string> Value1388Property = RegisterProperty<string>(nameof(Value1388));
  public string Value1388
  {
    get => GetProperty(Value1388Property);
    set => SetProperty(Value1388Property, value);
  }


  public static readonly PropertyInfo<string> Value1389Property = RegisterProperty<string>(nameof(Value1389));
  public string Value1389
  {
    get => GetProperty(Value1389Property);
    set => SetProperty(Value1389Property, value);
  }


  public static readonly PropertyInfo<string> Value1390Property = RegisterProperty<string>(nameof(Value1390));
  public string Value1390
  {
    get => GetProperty(Value1390Property);
    set => SetProperty(Value1390Property, value);
  }


  public static readonly PropertyInfo<string> Value1391Property = RegisterProperty<string>(nameof(Value1391));
  public string Value1391
  {
    get => GetProperty(Value1391Property);
    set => SetProperty(Value1391Property, value);
  }


  public static readonly PropertyInfo<string> Value1392Property = RegisterProperty<string>(nameof(Value1392));
  public string Value1392
  {
    get => GetProperty(Value1392Property);
    set => SetProperty(Value1392Property, value);
  }


  public static readonly PropertyInfo<string> Value1393Property = RegisterProperty<string>(nameof(Value1393));
  public string Value1393
  {
    get => GetProperty(Value1393Property);
    set => SetProperty(Value1393Property, value);
  }


  public static readonly PropertyInfo<string> Value1394Property = RegisterProperty<string>(nameof(Value1394));
  public string Value1394
  {
    get => GetProperty(Value1394Property);
    set => SetProperty(Value1394Property, value);
  }


  public static readonly PropertyInfo<string> Value1395Property = RegisterProperty<string>(nameof(Value1395));
  public string Value1395
  {
    get => GetProperty(Value1395Property);
    set => SetProperty(Value1395Property, value);
  }


  public static readonly PropertyInfo<string> Value1396Property = RegisterProperty<string>(nameof(Value1396));
  public string Value1396
  {
    get => GetProperty(Value1396Property);
    set => SetProperty(Value1396Property, value);
  }


  public static readonly PropertyInfo<string> Value1397Property = RegisterProperty<string>(nameof(Value1397));
  public string Value1397
  {
    get => GetProperty(Value1397Property);
    set => SetProperty(Value1397Property, value);
  }


  public static readonly PropertyInfo<string> Value1398Property = RegisterProperty<string>(nameof(Value1398));
  public string Value1398
  {
    get => GetProperty(Value1398Property);
    set => SetProperty(Value1398Property, value);
  }


  public static readonly PropertyInfo<string> Value1399Property = RegisterProperty<string>(nameof(Value1399));
  public string Value1399
  {
    get => GetProperty(Value1399Property);
    set => SetProperty(Value1399Property, value);
  }


  public static readonly PropertyInfo<string> Value1400Property = RegisterProperty<string>(nameof(Value1400));
  public string Value1400
  {
    get => GetProperty(Value1400Property);
    set => SetProperty(Value1400Property, value);
  }


  public static readonly PropertyInfo<string> Value1401Property = RegisterProperty<string>(nameof(Value1401));
  public string Value1401
  {
    get => GetProperty(Value1401Property);
    set => SetProperty(Value1401Property, value);
  }


  public static readonly PropertyInfo<string> Value1402Property = RegisterProperty<string>(nameof(Value1402));
  public string Value1402
  {
    get => GetProperty(Value1402Property);
    set => SetProperty(Value1402Property, value);
  }


  public static readonly PropertyInfo<string> Value1403Property = RegisterProperty<string>(nameof(Value1403));
  public string Value1403
  {
    get => GetProperty(Value1403Property);
    set => SetProperty(Value1403Property, value);
  }


  public static readonly PropertyInfo<string> Value1404Property = RegisterProperty<string>(nameof(Value1404));
  public string Value1404
  {
    get => GetProperty(Value1404Property);
    set => SetProperty(Value1404Property, value);
  }


  public static readonly PropertyInfo<string> Value1405Property = RegisterProperty<string>(nameof(Value1405));
  public string Value1405
  {
    get => GetProperty(Value1405Property);
    set => SetProperty(Value1405Property, value);
  }


  public static readonly PropertyInfo<string> Value1406Property = RegisterProperty<string>(nameof(Value1406));
  public string Value1406
  {
    get => GetProperty(Value1406Property);
    set => SetProperty(Value1406Property, value);
  }


  public static readonly PropertyInfo<string> Value1407Property = RegisterProperty<string>(nameof(Value1407));
  public string Value1407
  {
    get => GetProperty(Value1407Property);
    set => SetProperty(Value1407Property, value);
  }


  public static readonly PropertyInfo<string> Value1408Property = RegisterProperty<string>(nameof(Value1408));
  public string Value1408
  {
    get => GetProperty(Value1408Property);
    set => SetProperty(Value1408Property, value);
  }


  public static readonly PropertyInfo<string> Value1409Property = RegisterProperty<string>(nameof(Value1409));
  public string Value1409
  {
    get => GetProperty(Value1409Property);
    set => SetProperty(Value1409Property, value);
  }


  public static readonly PropertyInfo<string> Value1410Property = RegisterProperty<string>(nameof(Value1410));
  public string Value1410
  {
    get => GetProperty(Value1410Property);
    set => SetProperty(Value1410Property, value);
  }


  public static readonly PropertyInfo<string> Value1411Property = RegisterProperty<string>(nameof(Value1411));
  public string Value1411
  {
    get => GetProperty(Value1411Property);
    set => SetProperty(Value1411Property, value);
  }


  public static readonly PropertyInfo<string> Value1412Property = RegisterProperty<string>(nameof(Value1412));
  public string Value1412
  {
    get => GetProperty(Value1412Property);
    set => SetProperty(Value1412Property, value);
  }


  public static readonly PropertyInfo<string> Value1413Property = RegisterProperty<string>(nameof(Value1413));
  public string Value1413
  {
    get => GetProperty(Value1413Property);
    set => SetProperty(Value1413Property, value);
  }


  public static readonly PropertyInfo<string> Value1414Property = RegisterProperty<string>(nameof(Value1414));
  public string Value1414
  {
    get => GetProperty(Value1414Property);
    set => SetProperty(Value1414Property, value);
  }


  public static readonly PropertyInfo<string> Value1415Property = RegisterProperty<string>(nameof(Value1415));
  public string Value1415
  {
    get => GetProperty(Value1415Property);
    set => SetProperty(Value1415Property, value);
  }


  public static readonly PropertyInfo<string> Value1416Property = RegisterProperty<string>(nameof(Value1416));
  public string Value1416
  {
    get => GetProperty(Value1416Property);
    set => SetProperty(Value1416Property, value);
  }


  public static readonly PropertyInfo<string> Value1417Property = RegisterProperty<string>(nameof(Value1417));
  public string Value1417
  {
    get => GetProperty(Value1417Property);
    set => SetProperty(Value1417Property, value);
  }


  public static readonly PropertyInfo<string> Value1418Property = RegisterProperty<string>(nameof(Value1418));
  public string Value1418
  {
    get => GetProperty(Value1418Property);
    set => SetProperty(Value1418Property, value);
  }


  public static readonly PropertyInfo<string> Value1419Property = RegisterProperty<string>(nameof(Value1419));
  public string Value1419
  {
    get => GetProperty(Value1419Property);
    set => SetProperty(Value1419Property, value);
  }


  public static readonly PropertyInfo<string> Value1420Property = RegisterProperty<string>(nameof(Value1420));
  public string Value1420
  {
    get => GetProperty(Value1420Property);
    set => SetProperty(Value1420Property, value);
  }


  public static readonly PropertyInfo<string> Value1421Property = RegisterProperty<string>(nameof(Value1421));
  public string Value1421
  {
    get => GetProperty(Value1421Property);
    set => SetProperty(Value1421Property, value);
  }


  public static readonly PropertyInfo<string> Value1422Property = RegisterProperty<string>(nameof(Value1422));
  public string Value1422
  {
    get => GetProperty(Value1422Property);
    set => SetProperty(Value1422Property, value);
  }


  public static readonly PropertyInfo<string> Value1423Property = RegisterProperty<string>(nameof(Value1423));
  public string Value1423
  {
    get => GetProperty(Value1423Property);
    set => SetProperty(Value1423Property, value);
  }


  public static readonly PropertyInfo<string> Value1424Property = RegisterProperty<string>(nameof(Value1424));
  public string Value1424
  {
    get => GetProperty(Value1424Property);
    set => SetProperty(Value1424Property, value);
  }


  public static readonly PropertyInfo<string> Value1425Property = RegisterProperty<string>(nameof(Value1425));
  public string Value1425
  {
    get => GetProperty(Value1425Property);
    set => SetProperty(Value1425Property, value);
  }


  public static readonly PropertyInfo<string> Value1426Property = RegisterProperty<string>(nameof(Value1426));
  public string Value1426
  {
    get => GetProperty(Value1426Property);
    set => SetProperty(Value1426Property, value);
  }


  public static readonly PropertyInfo<string> Value1427Property = RegisterProperty<string>(nameof(Value1427));
  public string Value1427
  {
    get => GetProperty(Value1427Property);
    set => SetProperty(Value1427Property, value);
  }


  public static readonly PropertyInfo<string> Value1428Property = RegisterProperty<string>(nameof(Value1428));
  public string Value1428
  {
    get => GetProperty(Value1428Property);
    set => SetProperty(Value1428Property, value);
  }


  public static readonly PropertyInfo<string> Value1429Property = RegisterProperty<string>(nameof(Value1429));
  public string Value1429
  {
    get => GetProperty(Value1429Property);
    set => SetProperty(Value1429Property, value);
  }


  public static readonly PropertyInfo<string> Value1430Property = RegisterProperty<string>(nameof(Value1430));
  public string Value1430
  {
    get => GetProperty(Value1430Property);
    set => SetProperty(Value1430Property, value);
  }


  public static readonly PropertyInfo<string> Value1431Property = RegisterProperty<string>(nameof(Value1431));
  public string Value1431
  {
    get => GetProperty(Value1431Property);
    set => SetProperty(Value1431Property, value);
  }


  public static readonly PropertyInfo<string> Value1432Property = RegisterProperty<string>(nameof(Value1432));
  public string Value1432
  {
    get => GetProperty(Value1432Property);
    set => SetProperty(Value1432Property, value);
  }


  public static readonly PropertyInfo<string> Value1433Property = RegisterProperty<string>(nameof(Value1433));
  public string Value1433
  {
    get => GetProperty(Value1433Property);
    set => SetProperty(Value1433Property, value);
  }


  public static readonly PropertyInfo<string> Value1434Property = RegisterProperty<string>(nameof(Value1434));
  public string Value1434
  {
    get => GetProperty(Value1434Property);
    set => SetProperty(Value1434Property, value);
  }


  public static readonly PropertyInfo<string> Value1435Property = RegisterProperty<string>(nameof(Value1435));
  public string Value1435
  {
    get => GetProperty(Value1435Property);
    set => SetProperty(Value1435Property, value);
  }


  public static readonly PropertyInfo<string> Value1436Property = RegisterProperty<string>(nameof(Value1436));
  public string Value1436
  {
    get => GetProperty(Value1436Property);
    set => SetProperty(Value1436Property, value);
  }


  public static readonly PropertyInfo<string> Value1437Property = RegisterProperty<string>(nameof(Value1437));
  public string Value1437
  {
    get => GetProperty(Value1437Property);
    set => SetProperty(Value1437Property, value);
  }


  public static readonly PropertyInfo<string> Value1438Property = RegisterProperty<string>(nameof(Value1438));
  public string Value1438
  {
    get => GetProperty(Value1438Property);
    set => SetProperty(Value1438Property, value);
  }


  public static readonly PropertyInfo<string> Value1439Property = RegisterProperty<string>(nameof(Value1439));
  public string Value1439
  {
    get => GetProperty(Value1439Property);
    set => SetProperty(Value1439Property, value);
  }


  public static readonly PropertyInfo<string> Value1440Property = RegisterProperty<string>(nameof(Value1440));
  public string Value1440
  {
    get => GetProperty(Value1440Property);
    set => SetProperty(Value1440Property, value);
  }


  public static readonly PropertyInfo<string> Value1441Property = RegisterProperty<string>(nameof(Value1441));
  public string Value1441
  {
    get => GetProperty(Value1441Property);
    set => SetProperty(Value1441Property, value);
  }


  public static readonly PropertyInfo<string> Value1442Property = RegisterProperty<string>(nameof(Value1442));
  public string Value1442
  {
    get => GetProperty(Value1442Property);
    set => SetProperty(Value1442Property, value);
  }


  public static readonly PropertyInfo<string> Value1443Property = RegisterProperty<string>(nameof(Value1443));
  public string Value1443
  {
    get => GetProperty(Value1443Property);
    set => SetProperty(Value1443Property, value);
  }


  public static readonly PropertyInfo<string> Value1444Property = RegisterProperty<string>(nameof(Value1444));
  public string Value1444
  {
    get => GetProperty(Value1444Property);
    set => SetProperty(Value1444Property, value);
  }


  public static readonly PropertyInfo<string> Value1445Property = RegisterProperty<string>(nameof(Value1445));
  public string Value1445
  {
    get => GetProperty(Value1445Property);
    set => SetProperty(Value1445Property, value);
  }


  public static readonly PropertyInfo<string> Value1446Property = RegisterProperty<string>(nameof(Value1446));
  public string Value1446
  {
    get => GetProperty(Value1446Property);
    set => SetProperty(Value1446Property, value);
  }


  public static readonly PropertyInfo<string> Value1447Property = RegisterProperty<string>(nameof(Value1447));
  public string Value1447
  {
    get => GetProperty(Value1447Property);
    set => SetProperty(Value1447Property, value);
  }


  public static readonly PropertyInfo<string> Value1448Property = RegisterProperty<string>(nameof(Value1448));
  public string Value1448
  {
    get => GetProperty(Value1448Property);
    set => SetProperty(Value1448Property, value);
  }


  public static readonly PropertyInfo<string> Value1449Property = RegisterProperty<string>(nameof(Value1449));
  public string Value1449
  {
    get => GetProperty(Value1449Property);
    set => SetProperty(Value1449Property, value);
  }


  public static readonly PropertyInfo<string> Value1450Property = RegisterProperty<string>(nameof(Value1450));
  public string Value1450
  {
    get => GetProperty(Value1450Property);
    set => SetProperty(Value1450Property, value);
  }


  public static readonly PropertyInfo<string> Value1451Property = RegisterProperty<string>(nameof(Value1451));
  public string Value1451
  {
    get => GetProperty(Value1451Property);
    set => SetProperty(Value1451Property, value);
  }


  public static readonly PropertyInfo<string> Value1452Property = RegisterProperty<string>(nameof(Value1452));
  public string Value1452
  {
    get => GetProperty(Value1452Property);
    set => SetProperty(Value1452Property, value);
  }


  public static readonly PropertyInfo<string> Value1453Property = RegisterProperty<string>(nameof(Value1453));
  public string Value1453
  {
    get => GetProperty(Value1453Property);
    set => SetProperty(Value1453Property, value);
  }


  public static readonly PropertyInfo<string> Value1454Property = RegisterProperty<string>(nameof(Value1454));
  public string Value1454
  {
    get => GetProperty(Value1454Property);
    set => SetProperty(Value1454Property, value);
  }


  public static readonly PropertyInfo<string> Value1455Property = RegisterProperty<string>(nameof(Value1455));
  public string Value1455
  {
    get => GetProperty(Value1455Property);
    set => SetProperty(Value1455Property, value);
  }


  public static readonly PropertyInfo<string> Value1456Property = RegisterProperty<string>(nameof(Value1456));
  public string Value1456
  {
    get => GetProperty(Value1456Property);
    set => SetProperty(Value1456Property, value);
  }


  public static readonly PropertyInfo<string> Value1457Property = RegisterProperty<string>(nameof(Value1457));
  public string Value1457
  {
    get => GetProperty(Value1457Property);
    set => SetProperty(Value1457Property, value);
  }


  public static readonly PropertyInfo<string> Value1458Property = RegisterProperty<string>(nameof(Value1458));
  public string Value1458
  {
    get => GetProperty(Value1458Property);
    set => SetProperty(Value1458Property, value);
  }


  public static readonly PropertyInfo<string> Value1459Property = RegisterProperty<string>(nameof(Value1459));
  public string Value1459
  {
    get => GetProperty(Value1459Property);
    set => SetProperty(Value1459Property, value);
  }


  public static readonly PropertyInfo<string> Value1460Property = RegisterProperty<string>(nameof(Value1460));
  public string Value1460
  {
    get => GetProperty(Value1460Property);
    set => SetProperty(Value1460Property, value);
  }


  public static readonly PropertyInfo<string> Value1461Property = RegisterProperty<string>(nameof(Value1461));
  public string Value1461
  {
    get => GetProperty(Value1461Property);
    set => SetProperty(Value1461Property, value);
  }


  public static readonly PropertyInfo<string> Value1462Property = RegisterProperty<string>(nameof(Value1462));
  public string Value1462
  {
    get => GetProperty(Value1462Property);
    set => SetProperty(Value1462Property, value);
  }


  public static readonly PropertyInfo<string> Value1463Property = RegisterProperty<string>(nameof(Value1463));
  public string Value1463
  {
    get => GetProperty(Value1463Property);
    set => SetProperty(Value1463Property, value);
  }


  public static readonly PropertyInfo<string> Value1464Property = RegisterProperty<string>(nameof(Value1464));
  public string Value1464
  {
    get => GetProperty(Value1464Property);
    set => SetProperty(Value1464Property, value);
  }


  public static readonly PropertyInfo<string> Value1465Property = RegisterProperty<string>(nameof(Value1465));
  public string Value1465
  {
    get => GetProperty(Value1465Property);
    set => SetProperty(Value1465Property, value);
  }


  public static readonly PropertyInfo<string> Value1466Property = RegisterProperty<string>(nameof(Value1466));
  public string Value1466
  {
    get => GetProperty(Value1466Property);
    set => SetProperty(Value1466Property, value);
  }


  public static readonly PropertyInfo<string> Value1467Property = RegisterProperty<string>(nameof(Value1467));
  public string Value1467
  {
    get => GetProperty(Value1467Property);
    set => SetProperty(Value1467Property, value);
  }


  public static readonly PropertyInfo<string> Value1468Property = RegisterProperty<string>(nameof(Value1468));
  public string Value1468
  {
    get => GetProperty(Value1468Property);
    set => SetProperty(Value1468Property, value);
  }


  public static readonly PropertyInfo<string> Value1469Property = RegisterProperty<string>(nameof(Value1469));
  public string Value1469
  {
    get => GetProperty(Value1469Property);
    set => SetProperty(Value1469Property, value);
  }


  public static readonly PropertyInfo<string> Value1470Property = RegisterProperty<string>(nameof(Value1470));
  public string Value1470
  {
    get => GetProperty(Value1470Property);
    set => SetProperty(Value1470Property, value);
  }


  public static readonly PropertyInfo<string> Value1471Property = RegisterProperty<string>(nameof(Value1471));
  public string Value1471
  {
    get => GetProperty(Value1471Property);
    set => SetProperty(Value1471Property, value);
  }


  public static readonly PropertyInfo<string> Value1472Property = RegisterProperty<string>(nameof(Value1472));
  public string Value1472
  {
    get => GetProperty(Value1472Property);
    set => SetProperty(Value1472Property, value);
  }


  public static readonly PropertyInfo<string> Value1473Property = RegisterProperty<string>(nameof(Value1473));
  public string Value1473
  {
    get => GetProperty(Value1473Property);
    set => SetProperty(Value1473Property, value);
  }


  public static readonly PropertyInfo<string> Value1474Property = RegisterProperty<string>(nameof(Value1474));
  public string Value1474
  {
    get => GetProperty(Value1474Property);
    set => SetProperty(Value1474Property, value);
  }


  public static readonly PropertyInfo<string> Value1475Property = RegisterProperty<string>(nameof(Value1475));
  public string Value1475
  {
    get => GetProperty(Value1475Property);
    set => SetProperty(Value1475Property, value);
  }


  public static readonly PropertyInfo<string> Value1476Property = RegisterProperty<string>(nameof(Value1476));
  public string Value1476
  {
    get => GetProperty(Value1476Property);
    set => SetProperty(Value1476Property, value);
  }


  public static readonly PropertyInfo<string> Value1477Property = RegisterProperty<string>(nameof(Value1477));
  public string Value1477
  {
    get => GetProperty(Value1477Property);
    set => SetProperty(Value1477Property, value);
  }


  public static readonly PropertyInfo<string> Value1478Property = RegisterProperty<string>(nameof(Value1478));
  public string Value1478
  {
    get => GetProperty(Value1478Property);
    set => SetProperty(Value1478Property, value);
  }


  public static readonly PropertyInfo<string> Value1479Property = RegisterProperty<string>(nameof(Value1479));
  public string Value1479
  {
    get => GetProperty(Value1479Property);
    set => SetProperty(Value1479Property, value);
  }


  public static readonly PropertyInfo<string> Value1480Property = RegisterProperty<string>(nameof(Value1480));
  public string Value1480
  {
    get => GetProperty(Value1480Property);
    set => SetProperty(Value1480Property, value);
  }


  public static readonly PropertyInfo<string> Value1481Property = RegisterProperty<string>(nameof(Value1481));
  public string Value1481
  {
    get => GetProperty(Value1481Property);
    set => SetProperty(Value1481Property, value);
  }


  public static readonly PropertyInfo<string> Value1482Property = RegisterProperty<string>(nameof(Value1482));
  public string Value1482
  {
    get => GetProperty(Value1482Property);
    set => SetProperty(Value1482Property, value);
  }


  public static readonly PropertyInfo<string> Value1483Property = RegisterProperty<string>(nameof(Value1483));
  public string Value1483
  {
    get => GetProperty(Value1483Property);
    set => SetProperty(Value1483Property, value);
  }


  public static readonly PropertyInfo<string> Value1484Property = RegisterProperty<string>(nameof(Value1484));
  public string Value1484
  {
    get => GetProperty(Value1484Property);
    set => SetProperty(Value1484Property, value);
  }


  public static readonly PropertyInfo<string> Value1485Property = RegisterProperty<string>(nameof(Value1485));
  public string Value1485
  {
    get => GetProperty(Value1485Property);
    set => SetProperty(Value1485Property, value);
  }


  public static readonly PropertyInfo<string> Value1486Property = RegisterProperty<string>(nameof(Value1486));
  public string Value1486
  {
    get => GetProperty(Value1486Property);
    set => SetProperty(Value1486Property, value);
  }


  public static readonly PropertyInfo<string> Value1487Property = RegisterProperty<string>(nameof(Value1487));
  public string Value1487
  {
    get => GetProperty(Value1487Property);
    set => SetProperty(Value1487Property, value);
  }


  public static readonly PropertyInfo<string> Value1488Property = RegisterProperty<string>(nameof(Value1488));
  public string Value1488
  {
    get => GetProperty(Value1488Property);
    set => SetProperty(Value1488Property, value);
  }


  public static readonly PropertyInfo<string> Value1489Property = RegisterProperty<string>(nameof(Value1489));
  public string Value1489
  {
    get => GetProperty(Value1489Property);
    set => SetProperty(Value1489Property, value);
  }


  public static readonly PropertyInfo<string> Value1490Property = RegisterProperty<string>(nameof(Value1490));
  public string Value1490
  {
    get => GetProperty(Value1490Property);
    set => SetProperty(Value1490Property, value);
  }


  public static readonly PropertyInfo<string> Value1491Property = RegisterProperty<string>(nameof(Value1491));
  public string Value1491
  {
    get => GetProperty(Value1491Property);
    set => SetProperty(Value1491Property, value);
  }


  public static readonly PropertyInfo<string> Value1492Property = RegisterProperty<string>(nameof(Value1492));
  public string Value1492
  {
    get => GetProperty(Value1492Property);
    set => SetProperty(Value1492Property, value);
  }


  public static readonly PropertyInfo<string> Value1493Property = RegisterProperty<string>(nameof(Value1493));
  public string Value1493
  {
    get => GetProperty(Value1493Property);
    set => SetProperty(Value1493Property, value);
  }


  public static readonly PropertyInfo<string> Value1494Property = RegisterProperty<string>(nameof(Value1494));
  public string Value1494
  {
    get => GetProperty(Value1494Property);
    set => SetProperty(Value1494Property, value);
  }


  public static readonly PropertyInfo<string> Value1495Property = RegisterProperty<string>(nameof(Value1495));
  public string Value1495
  {
    get => GetProperty(Value1495Property);
    set => SetProperty(Value1495Property, value);
  }


  public static readonly PropertyInfo<string> Value1496Property = RegisterProperty<string>(nameof(Value1496));
  public string Value1496
  {
    get => GetProperty(Value1496Property);
    set => SetProperty(Value1496Property, value);
  }


  public static readonly PropertyInfo<string> Value1497Property = RegisterProperty<string>(nameof(Value1497));
  public string Value1497
  {
    get => GetProperty(Value1497Property);
    set => SetProperty(Value1497Property, value);
  }


  public static readonly PropertyInfo<string> Value1498Property = RegisterProperty<string>(nameof(Value1498));
  public string Value1498
  {
    get => GetProperty(Value1498Property);
    set => SetProperty(Value1498Property, value);
  }


  public static readonly PropertyInfo<string> Value1499Property = RegisterProperty<string>(nameof(Value1499));
  public string Value1499
  {
    get => GetProperty(Value1499Property);
    set => SetProperty(Value1499Property, value);
  }


  public static readonly PropertyInfo<string> Value1500Property = RegisterProperty<string>(nameof(Value1500));
  public string Value1500
  {
    get => GetProperty(Value1500Property);
    set => SetProperty(Value1500Property, value);
  }


  public static readonly PropertyInfo<string> Value1501Property = RegisterProperty<string>(nameof(Value1501));
  public string Value1501
  {
    get => GetProperty(Value1501Property);
    set => SetProperty(Value1501Property, value);
  }


  public static readonly PropertyInfo<string> Value1502Property = RegisterProperty<string>(nameof(Value1502));
  public string Value1502
  {
    get => GetProperty(Value1502Property);
    set => SetProperty(Value1502Property, value);
  }


  public static readonly PropertyInfo<string> Value1503Property = RegisterProperty<string>(nameof(Value1503));
  public string Value1503
  {
    get => GetProperty(Value1503Property);
    set => SetProperty(Value1503Property, value);
  }


  public static readonly PropertyInfo<string> Value1504Property = RegisterProperty<string>(nameof(Value1504));
  public string Value1504
  {
    get => GetProperty(Value1504Property);
    set => SetProperty(Value1504Property, value);
  }


  public static readonly PropertyInfo<string> Value1505Property = RegisterProperty<string>(nameof(Value1505));
  public string Value1505
  {
    get => GetProperty(Value1505Property);
    set => SetProperty(Value1505Property, value);
  }


  public static readonly PropertyInfo<string> Value1506Property = RegisterProperty<string>(nameof(Value1506));
  public string Value1506
  {
    get => GetProperty(Value1506Property);
    set => SetProperty(Value1506Property, value);
  }


  public static readonly PropertyInfo<string> Value1507Property = RegisterProperty<string>(nameof(Value1507));
  public string Value1507
  {
    get => GetProperty(Value1507Property);
    set => SetProperty(Value1507Property, value);
  }


  public static readonly PropertyInfo<string> Value1508Property = RegisterProperty<string>(nameof(Value1508));
  public string Value1508
  {
    get => GetProperty(Value1508Property);
    set => SetProperty(Value1508Property, value);
  }


  public static readonly PropertyInfo<string> Value1509Property = RegisterProperty<string>(nameof(Value1509));
  public string Value1509
  {
    get => GetProperty(Value1509Property);
    set => SetProperty(Value1509Property, value);
  }


  public static readonly PropertyInfo<string> Value1510Property = RegisterProperty<string>(nameof(Value1510));
  public string Value1510
  {
    get => GetProperty(Value1510Property);
    set => SetProperty(Value1510Property, value);
  }


  public static readonly PropertyInfo<string> Value1511Property = RegisterProperty<string>(nameof(Value1511));
  public string Value1511
  {
    get => GetProperty(Value1511Property);
    set => SetProperty(Value1511Property, value);
  }


  public static readonly PropertyInfo<string> Value1512Property = RegisterProperty<string>(nameof(Value1512));
  public string Value1512
  {
    get => GetProperty(Value1512Property);
    set => SetProperty(Value1512Property, value);
  }


  public static readonly PropertyInfo<string> Value1513Property = RegisterProperty<string>(nameof(Value1513));
  public string Value1513
  {
    get => GetProperty(Value1513Property);
    set => SetProperty(Value1513Property, value);
  }


  public static readonly PropertyInfo<string> Value1514Property = RegisterProperty<string>(nameof(Value1514));
  public string Value1514
  {
    get => GetProperty(Value1514Property);
    set => SetProperty(Value1514Property, value);
  }


  public static readonly PropertyInfo<string> Value1515Property = RegisterProperty<string>(nameof(Value1515));
  public string Value1515
  {
    get => GetProperty(Value1515Property);
    set => SetProperty(Value1515Property, value);
  }


  public static readonly PropertyInfo<string> Value1516Property = RegisterProperty<string>(nameof(Value1516));
  public string Value1516
  {
    get => GetProperty(Value1516Property);
    set => SetProperty(Value1516Property, value);
  }


  public static readonly PropertyInfo<string> Value1517Property = RegisterProperty<string>(nameof(Value1517));
  public string Value1517
  {
    get => GetProperty(Value1517Property);
    set => SetProperty(Value1517Property, value);
  }


  public static readonly PropertyInfo<string> Value1518Property = RegisterProperty<string>(nameof(Value1518));
  public string Value1518
  {
    get => GetProperty(Value1518Property);
    set => SetProperty(Value1518Property, value);
  }


  public static readonly PropertyInfo<string> Value1519Property = RegisterProperty<string>(nameof(Value1519));
  public string Value1519
  {
    get => GetProperty(Value1519Property);
    set => SetProperty(Value1519Property, value);
  }


  public static readonly PropertyInfo<string> Value1520Property = RegisterProperty<string>(nameof(Value1520));
  public string Value1520
  {
    get => GetProperty(Value1520Property);
    set => SetProperty(Value1520Property, value);
  }


  public static readonly PropertyInfo<string> Value1521Property = RegisterProperty<string>(nameof(Value1521));
  public string Value1521
  {
    get => GetProperty(Value1521Property);
    set => SetProperty(Value1521Property, value);
  }


  public static readonly PropertyInfo<string> Value1522Property = RegisterProperty<string>(nameof(Value1522));
  public string Value1522
  {
    get => GetProperty(Value1522Property);
    set => SetProperty(Value1522Property, value);
  }


  public static readonly PropertyInfo<string> Value1523Property = RegisterProperty<string>(nameof(Value1523));
  public string Value1523
  {
    get => GetProperty(Value1523Property);
    set => SetProperty(Value1523Property, value);
  }


  public static readonly PropertyInfo<string> Value1524Property = RegisterProperty<string>(nameof(Value1524));
  public string Value1524
  {
    get => GetProperty(Value1524Property);
    set => SetProperty(Value1524Property, value);
  }


  public static readonly PropertyInfo<string> Value1525Property = RegisterProperty<string>(nameof(Value1525));
  public string Value1525
  {
    get => GetProperty(Value1525Property);
    set => SetProperty(Value1525Property, value);
  }


  public static readonly PropertyInfo<string> Value1526Property = RegisterProperty<string>(nameof(Value1526));
  public string Value1526
  {
    get => GetProperty(Value1526Property);
    set => SetProperty(Value1526Property, value);
  }


  public static readonly PropertyInfo<string> Value1527Property = RegisterProperty<string>(nameof(Value1527));
  public string Value1527
  {
    get => GetProperty(Value1527Property);
    set => SetProperty(Value1527Property, value);
  }


  public static readonly PropertyInfo<string> Value1528Property = RegisterProperty<string>(nameof(Value1528));
  public string Value1528
  {
    get => GetProperty(Value1528Property);
    set => SetProperty(Value1528Property, value);
  }


  public static readonly PropertyInfo<string> Value1529Property = RegisterProperty<string>(nameof(Value1529));
  public string Value1529
  {
    get => GetProperty(Value1529Property);
    set => SetProperty(Value1529Property, value);
  }


  public static readonly PropertyInfo<string> Value1530Property = RegisterProperty<string>(nameof(Value1530));
  public string Value1530
  {
    get => GetProperty(Value1530Property);
    set => SetProperty(Value1530Property, value);
  }


  public static readonly PropertyInfo<string> Value1531Property = RegisterProperty<string>(nameof(Value1531));
  public string Value1531
  {
    get => GetProperty(Value1531Property);
    set => SetProperty(Value1531Property, value);
  }


  public static readonly PropertyInfo<string> Value1532Property = RegisterProperty<string>(nameof(Value1532));
  public string Value1532
  {
    get => GetProperty(Value1532Property);
    set => SetProperty(Value1532Property, value);
  }


  public static readonly PropertyInfo<string> Value1533Property = RegisterProperty<string>(nameof(Value1533));
  public string Value1533
  {
    get => GetProperty(Value1533Property);
    set => SetProperty(Value1533Property, value);
  }


  public static readonly PropertyInfo<string> Value1534Property = RegisterProperty<string>(nameof(Value1534));
  public string Value1534
  {
    get => GetProperty(Value1534Property);
    set => SetProperty(Value1534Property, value);
  }


  public static readonly PropertyInfo<string> Value1535Property = RegisterProperty<string>(nameof(Value1535));
  public string Value1535
  {
    get => GetProperty(Value1535Property);
    set => SetProperty(Value1535Property, value);
  }


  public static readonly PropertyInfo<string> Value1536Property = RegisterProperty<string>(nameof(Value1536));
  public string Value1536
  {
    get => GetProperty(Value1536Property);
    set => SetProperty(Value1536Property, value);
  }


  public static readonly PropertyInfo<string> Value1537Property = RegisterProperty<string>(nameof(Value1537));
  public string Value1537
  {
    get => GetProperty(Value1537Property);
    set => SetProperty(Value1537Property, value);
  }


  public static readonly PropertyInfo<string> Value1538Property = RegisterProperty<string>(nameof(Value1538));
  public string Value1538
  {
    get => GetProperty(Value1538Property);
    set => SetProperty(Value1538Property, value);
  }


  public static readonly PropertyInfo<string> Value1539Property = RegisterProperty<string>(nameof(Value1539));
  public string Value1539
  {
    get => GetProperty(Value1539Property);
    set => SetProperty(Value1539Property, value);
  }


  public static readonly PropertyInfo<string> Value1540Property = RegisterProperty<string>(nameof(Value1540));
  public string Value1540
  {
    get => GetProperty(Value1540Property);
    set => SetProperty(Value1540Property, value);
  }


  public static readonly PropertyInfo<string> Value1541Property = RegisterProperty<string>(nameof(Value1541));
  public string Value1541
  {
    get => GetProperty(Value1541Property);
    set => SetProperty(Value1541Property, value);
  }


  public static readonly PropertyInfo<string> Value1542Property = RegisterProperty<string>(nameof(Value1542));
  public string Value1542
  {
    get => GetProperty(Value1542Property);
    set => SetProperty(Value1542Property, value);
  }


  public static readonly PropertyInfo<string> Value1543Property = RegisterProperty<string>(nameof(Value1543));
  public string Value1543
  {
    get => GetProperty(Value1543Property);
    set => SetProperty(Value1543Property, value);
  }


  public static readonly PropertyInfo<string> Value1544Property = RegisterProperty<string>(nameof(Value1544));
  public string Value1544
  {
    get => GetProperty(Value1544Property);
    set => SetProperty(Value1544Property, value);
  }


  public static readonly PropertyInfo<string> Value1545Property = RegisterProperty<string>(nameof(Value1545));
  public string Value1545
  {
    get => GetProperty(Value1545Property);
    set => SetProperty(Value1545Property, value);
  }


  public static readonly PropertyInfo<string> Value1546Property = RegisterProperty<string>(nameof(Value1546));
  public string Value1546
  {
    get => GetProperty(Value1546Property);
    set => SetProperty(Value1546Property, value);
  }


  public static readonly PropertyInfo<string> Value1547Property = RegisterProperty<string>(nameof(Value1547));
  public string Value1547
  {
    get => GetProperty(Value1547Property);
    set => SetProperty(Value1547Property, value);
  }


  public static readonly PropertyInfo<string> Value1548Property = RegisterProperty<string>(nameof(Value1548));
  public string Value1548
  {
    get => GetProperty(Value1548Property);
    set => SetProperty(Value1548Property, value);
  }


  public static readonly PropertyInfo<string> Value1549Property = RegisterProperty<string>(nameof(Value1549));
  public string Value1549
  {
    get => GetProperty(Value1549Property);
    set => SetProperty(Value1549Property, value);
  }


  public static readonly PropertyInfo<string> Value1550Property = RegisterProperty<string>(nameof(Value1550));
  public string Value1550
  {
    get => GetProperty(Value1550Property);
    set => SetProperty(Value1550Property, value);
  }


  public static readonly PropertyInfo<string> Value1551Property = RegisterProperty<string>(nameof(Value1551));
  public string Value1551
  {
    get => GetProperty(Value1551Property);
    set => SetProperty(Value1551Property, value);
  }


  public static readonly PropertyInfo<string> Value1552Property = RegisterProperty<string>(nameof(Value1552));
  public string Value1552
  {
    get => GetProperty(Value1552Property);
    set => SetProperty(Value1552Property, value);
  }


  public static readonly PropertyInfo<string> Value1553Property = RegisterProperty<string>(nameof(Value1553));
  public string Value1553
  {
    get => GetProperty(Value1553Property);
    set => SetProperty(Value1553Property, value);
  }


  public static readonly PropertyInfo<string> Value1554Property = RegisterProperty<string>(nameof(Value1554));
  public string Value1554
  {
    get => GetProperty(Value1554Property);
    set => SetProperty(Value1554Property, value);
  }


  public static readonly PropertyInfo<string> Value1555Property = RegisterProperty<string>(nameof(Value1555));
  public string Value1555
  {
    get => GetProperty(Value1555Property);
    set => SetProperty(Value1555Property, value);
  }


  public static readonly PropertyInfo<string> Value1556Property = RegisterProperty<string>(nameof(Value1556));
  public string Value1556
  {
    get => GetProperty(Value1556Property);
    set => SetProperty(Value1556Property, value);
  }


  public static readonly PropertyInfo<string> Value1557Property = RegisterProperty<string>(nameof(Value1557));
  public string Value1557
  {
    get => GetProperty(Value1557Property);
    set => SetProperty(Value1557Property, value);
  }


  public static readonly PropertyInfo<string> Value1558Property = RegisterProperty<string>(nameof(Value1558));
  public string Value1558
  {
    get => GetProperty(Value1558Property);
    set => SetProperty(Value1558Property, value);
  }


  public static readonly PropertyInfo<string> Value1559Property = RegisterProperty<string>(nameof(Value1559));
  public string Value1559
  {
    get => GetProperty(Value1559Property);
    set => SetProperty(Value1559Property, value);
  }


  public static readonly PropertyInfo<string> Value1560Property = RegisterProperty<string>(nameof(Value1560));
  public string Value1560
  {
    get => GetProperty(Value1560Property);
    set => SetProperty(Value1560Property, value);
  }


  public static readonly PropertyInfo<string> Value1561Property = RegisterProperty<string>(nameof(Value1561));
  public string Value1561
  {
    get => GetProperty(Value1561Property);
    set => SetProperty(Value1561Property, value);
  }


  public static readonly PropertyInfo<string> Value1562Property = RegisterProperty<string>(nameof(Value1562));
  public string Value1562
  {
    get => GetProperty(Value1562Property);
    set => SetProperty(Value1562Property, value);
  }


  public static readonly PropertyInfo<string> Value1563Property = RegisterProperty<string>(nameof(Value1563));
  public string Value1563
  {
    get => GetProperty(Value1563Property);
    set => SetProperty(Value1563Property, value);
  }


  public static readonly PropertyInfo<string> Value1564Property = RegisterProperty<string>(nameof(Value1564));
  public string Value1564
  {
    get => GetProperty(Value1564Property);
    set => SetProperty(Value1564Property, value);
  }


  public static readonly PropertyInfo<string> Value1565Property = RegisterProperty<string>(nameof(Value1565));
  public string Value1565
  {
    get => GetProperty(Value1565Property);
    set => SetProperty(Value1565Property, value);
  }


  public static readonly PropertyInfo<string> Value1566Property = RegisterProperty<string>(nameof(Value1566));
  public string Value1566
  {
    get => GetProperty(Value1566Property);
    set => SetProperty(Value1566Property, value);
  }


  public static readonly PropertyInfo<string> Value1567Property = RegisterProperty<string>(nameof(Value1567));
  public string Value1567
  {
    get => GetProperty(Value1567Property);
    set => SetProperty(Value1567Property, value);
  }


  public static readonly PropertyInfo<string> Value1568Property = RegisterProperty<string>(nameof(Value1568));
  public string Value1568
  {
    get => GetProperty(Value1568Property);
    set => SetProperty(Value1568Property, value);
  }


  public static readonly PropertyInfo<string> Value1569Property = RegisterProperty<string>(nameof(Value1569));
  public string Value1569
  {
    get => GetProperty(Value1569Property);
    set => SetProperty(Value1569Property, value);
  }


  public static readonly PropertyInfo<string> Value1570Property = RegisterProperty<string>(nameof(Value1570));
  public string Value1570
  {
    get => GetProperty(Value1570Property);
    set => SetProperty(Value1570Property, value);
  }


  public static readonly PropertyInfo<string> Value1571Property = RegisterProperty<string>(nameof(Value1571));
  public string Value1571
  {
    get => GetProperty(Value1571Property);
    set => SetProperty(Value1571Property, value);
  }


  public static readonly PropertyInfo<string> Value1572Property = RegisterProperty<string>(nameof(Value1572));
  public string Value1572
  {
    get => GetProperty(Value1572Property);
    set => SetProperty(Value1572Property, value);
  }


  public static readonly PropertyInfo<string> Value1573Property = RegisterProperty<string>(nameof(Value1573));
  public string Value1573
  {
    get => GetProperty(Value1573Property);
    set => SetProperty(Value1573Property, value);
  }


  public static readonly PropertyInfo<string> Value1574Property = RegisterProperty<string>(nameof(Value1574));
  public string Value1574
  {
    get => GetProperty(Value1574Property);
    set => SetProperty(Value1574Property, value);
  }


  public static readonly PropertyInfo<string> Value1575Property = RegisterProperty<string>(nameof(Value1575));
  public string Value1575
  {
    get => GetProperty(Value1575Property);
    set => SetProperty(Value1575Property, value);
  }


  public static readonly PropertyInfo<string> Value1576Property = RegisterProperty<string>(nameof(Value1576));
  public string Value1576
  {
    get => GetProperty(Value1576Property);
    set => SetProperty(Value1576Property, value);
  }


  public static readonly PropertyInfo<string> Value1577Property = RegisterProperty<string>(nameof(Value1577));
  public string Value1577
  {
    get => GetProperty(Value1577Property);
    set => SetProperty(Value1577Property, value);
  }


  public static readonly PropertyInfo<string> Value1578Property = RegisterProperty<string>(nameof(Value1578));
  public string Value1578
  {
    get => GetProperty(Value1578Property);
    set => SetProperty(Value1578Property, value);
  }


  public static readonly PropertyInfo<string> Value1579Property = RegisterProperty<string>(nameof(Value1579));
  public string Value1579
  {
    get => GetProperty(Value1579Property);
    set => SetProperty(Value1579Property, value);
  }


  public static readonly PropertyInfo<string> Value1580Property = RegisterProperty<string>(nameof(Value1580));
  public string Value1580
  {
    get => GetProperty(Value1580Property);
    set => SetProperty(Value1580Property, value);
  }


  public static readonly PropertyInfo<string> Value1581Property = RegisterProperty<string>(nameof(Value1581));
  public string Value1581
  {
    get => GetProperty(Value1581Property);
    set => SetProperty(Value1581Property, value);
  }


  public static readonly PropertyInfo<string> Value1582Property = RegisterProperty<string>(nameof(Value1582));
  public string Value1582
  {
    get => GetProperty(Value1582Property);
    set => SetProperty(Value1582Property, value);
  }


  public static readonly PropertyInfo<string> Value1583Property = RegisterProperty<string>(nameof(Value1583));
  public string Value1583
  {
    get => GetProperty(Value1583Property);
    set => SetProperty(Value1583Property, value);
  }


  public static readonly PropertyInfo<string> Value1584Property = RegisterProperty<string>(nameof(Value1584));
  public string Value1584
  {
    get => GetProperty(Value1584Property);
    set => SetProperty(Value1584Property, value);
  }


  public static readonly PropertyInfo<string> Value1585Property = RegisterProperty<string>(nameof(Value1585));
  public string Value1585
  {
    get => GetProperty(Value1585Property);
    set => SetProperty(Value1585Property, value);
  }


  public static readonly PropertyInfo<string> Value1586Property = RegisterProperty<string>(nameof(Value1586));
  public string Value1586
  {
    get => GetProperty(Value1586Property);
    set => SetProperty(Value1586Property, value);
  }


  public static readonly PropertyInfo<string> Value1587Property = RegisterProperty<string>(nameof(Value1587));
  public string Value1587
  {
    get => GetProperty(Value1587Property);
    set => SetProperty(Value1587Property, value);
  }


  public static readonly PropertyInfo<string> Value1588Property = RegisterProperty<string>(nameof(Value1588));
  public string Value1588
  {
    get => GetProperty(Value1588Property);
    set => SetProperty(Value1588Property, value);
  }


  public static readonly PropertyInfo<string> Value1589Property = RegisterProperty<string>(nameof(Value1589));
  public string Value1589
  {
    get => GetProperty(Value1589Property);
    set => SetProperty(Value1589Property, value);
  }


  public static readonly PropertyInfo<string> Value1590Property = RegisterProperty<string>(nameof(Value1590));
  public string Value1590
  {
    get => GetProperty(Value1590Property);
    set => SetProperty(Value1590Property, value);
  }


  public static readonly PropertyInfo<string> Value1591Property = RegisterProperty<string>(nameof(Value1591));
  public string Value1591
  {
    get => GetProperty(Value1591Property);
    set => SetProperty(Value1591Property, value);
  }


  public static readonly PropertyInfo<string> Value1592Property = RegisterProperty<string>(nameof(Value1592));
  public string Value1592
  {
    get => GetProperty(Value1592Property);
    set => SetProperty(Value1592Property, value);
  }


  public static readonly PropertyInfo<string> Value1593Property = RegisterProperty<string>(nameof(Value1593));
  public string Value1593
  {
    get => GetProperty(Value1593Property);
    set => SetProperty(Value1593Property, value);
  }


  public static readonly PropertyInfo<string> Value1594Property = RegisterProperty<string>(nameof(Value1594));
  public string Value1594
  {
    get => GetProperty(Value1594Property);
    set => SetProperty(Value1594Property, value);
  }


  public static readonly PropertyInfo<string> Value1595Property = RegisterProperty<string>(nameof(Value1595));
  public string Value1595
  {
    get => GetProperty(Value1595Property);
    set => SetProperty(Value1595Property, value);
  }


  public static readonly PropertyInfo<string> Value1596Property = RegisterProperty<string>(nameof(Value1596));
  public string Value1596
  {
    get => GetProperty(Value1596Property);
    set => SetProperty(Value1596Property, value);
  }


  public static readonly PropertyInfo<string> Value1597Property = RegisterProperty<string>(nameof(Value1597));
  public string Value1597
  {
    get => GetProperty(Value1597Property);
    set => SetProperty(Value1597Property, value);
  }


  public static readonly PropertyInfo<string> Value1598Property = RegisterProperty<string>(nameof(Value1598));
  public string Value1598
  {
    get => GetProperty(Value1598Property);
    set => SetProperty(Value1598Property, value);
  }


  public static readonly PropertyInfo<string> Value1599Property = RegisterProperty<string>(nameof(Value1599));
  public string Value1599
  {
    get => GetProperty(Value1599Property);
    set => SetProperty(Value1599Property, value);
  }


  public static readonly PropertyInfo<string> Value1600Property = RegisterProperty<string>(nameof(Value1600));
  public string Value1600
  {
    get => GetProperty(Value1600Property);
    set => SetProperty(Value1600Property, value);
  }


  public static readonly PropertyInfo<string> Value1601Property = RegisterProperty<string>(nameof(Value1601));
  public string Value1601
  {
    get => GetProperty(Value1601Property);
    set => SetProperty(Value1601Property, value);
  }


  public static readonly PropertyInfo<string> Value1602Property = RegisterProperty<string>(nameof(Value1602));
  public string Value1602
  {
    get => GetProperty(Value1602Property);
    set => SetProperty(Value1602Property, value);
  }


  public static readonly PropertyInfo<string> Value1603Property = RegisterProperty<string>(nameof(Value1603));
  public string Value1603
  {
    get => GetProperty(Value1603Property);
    set => SetProperty(Value1603Property, value);
  }


  public static readonly PropertyInfo<string> Value1604Property = RegisterProperty<string>(nameof(Value1604));
  public string Value1604
  {
    get => GetProperty(Value1604Property);
    set => SetProperty(Value1604Property, value);
  }


  public static readonly PropertyInfo<string> Value1605Property = RegisterProperty<string>(nameof(Value1605));
  public string Value1605
  {
    get => GetProperty(Value1605Property);
    set => SetProperty(Value1605Property, value);
  }


  public static readonly PropertyInfo<string> Value1606Property = RegisterProperty<string>(nameof(Value1606));
  public string Value1606
  {
    get => GetProperty(Value1606Property);
    set => SetProperty(Value1606Property, value);
  }


  public static readonly PropertyInfo<string> Value1607Property = RegisterProperty<string>(nameof(Value1607));
  public string Value1607
  {
    get => GetProperty(Value1607Property);
    set => SetProperty(Value1607Property, value);
  }


  public static readonly PropertyInfo<string> Value1608Property = RegisterProperty<string>(nameof(Value1608));
  public string Value1608
  {
    get => GetProperty(Value1608Property);
    set => SetProperty(Value1608Property, value);
  }


  public static readonly PropertyInfo<string> Value1609Property = RegisterProperty<string>(nameof(Value1609));
  public string Value1609
  {
    get => GetProperty(Value1609Property);
    set => SetProperty(Value1609Property, value);
  }


  public static readonly PropertyInfo<string> Value1610Property = RegisterProperty<string>(nameof(Value1610));
  public string Value1610
  {
    get => GetProperty(Value1610Property);
    set => SetProperty(Value1610Property, value);
  }


  public static readonly PropertyInfo<string> Value1611Property = RegisterProperty<string>(nameof(Value1611));
  public string Value1611
  {
    get => GetProperty(Value1611Property);
    set => SetProperty(Value1611Property, value);
  }


  public static readonly PropertyInfo<string> Value1612Property = RegisterProperty<string>(nameof(Value1612));
  public string Value1612
  {
    get => GetProperty(Value1612Property);
    set => SetProperty(Value1612Property, value);
  }


  public static readonly PropertyInfo<string> Value1613Property = RegisterProperty<string>(nameof(Value1613));
  public string Value1613
  {
    get => GetProperty(Value1613Property);
    set => SetProperty(Value1613Property, value);
  }


  public static readonly PropertyInfo<string> Value1614Property = RegisterProperty<string>(nameof(Value1614));
  public string Value1614
  {
    get => GetProperty(Value1614Property);
    set => SetProperty(Value1614Property, value);
  }


  public static readonly PropertyInfo<string> Value1615Property = RegisterProperty<string>(nameof(Value1615));
  public string Value1615
  {
    get => GetProperty(Value1615Property);
    set => SetProperty(Value1615Property, value);
  }


  public static readonly PropertyInfo<string> Value1616Property = RegisterProperty<string>(nameof(Value1616));
  public string Value1616
  {
    get => GetProperty(Value1616Property);
    set => SetProperty(Value1616Property, value);
  }


  public static readonly PropertyInfo<string> Value1617Property = RegisterProperty<string>(nameof(Value1617));
  public string Value1617
  {
    get => GetProperty(Value1617Property);
    set => SetProperty(Value1617Property, value);
  }


  public static readonly PropertyInfo<string> Value1618Property = RegisterProperty<string>(nameof(Value1618));
  public string Value1618
  {
    get => GetProperty(Value1618Property);
    set => SetProperty(Value1618Property, value);
  }


  public static readonly PropertyInfo<string> Value1619Property = RegisterProperty<string>(nameof(Value1619));
  public string Value1619
  {
    get => GetProperty(Value1619Property);
    set => SetProperty(Value1619Property, value);
  }


  public static readonly PropertyInfo<string> Value1620Property = RegisterProperty<string>(nameof(Value1620));
  public string Value1620
  {
    get => GetProperty(Value1620Property);
    set => SetProperty(Value1620Property, value);
  }


  public static readonly PropertyInfo<string> Value1621Property = RegisterProperty<string>(nameof(Value1621));
  public string Value1621
  {
    get => GetProperty(Value1621Property);
    set => SetProperty(Value1621Property, value);
  }


  public static readonly PropertyInfo<string> Value1622Property = RegisterProperty<string>(nameof(Value1622));
  public string Value1622
  {
    get => GetProperty(Value1622Property);
    set => SetProperty(Value1622Property, value);
  }


  public static readonly PropertyInfo<string> Value1623Property = RegisterProperty<string>(nameof(Value1623));
  public string Value1623
  {
    get => GetProperty(Value1623Property);
    set => SetProperty(Value1623Property, value);
  }


  public static readonly PropertyInfo<string> Value1624Property = RegisterProperty<string>(nameof(Value1624));
  public string Value1624
  {
    get => GetProperty(Value1624Property);
    set => SetProperty(Value1624Property, value);
  }


  public static readonly PropertyInfo<string> Value1625Property = RegisterProperty<string>(nameof(Value1625));
  public string Value1625
  {
    get => GetProperty(Value1625Property);
    set => SetProperty(Value1625Property, value);
  }


  public static readonly PropertyInfo<string> Value1626Property = RegisterProperty<string>(nameof(Value1626));
  public string Value1626
  {
    get => GetProperty(Value1626Property);
    set => SetProperty(Value1626Property, value);
  }


  public static readonly PropertyInfo<string> Value1627Property = RegisterProperty<string>(nameof(Value1627));
  public string Value1627
  {
    get => GetProperty(Value1627Property);
    set => SetProperty(Value1627Property, value);
  }


  public static readonly PropertyInfo<string> Value1628Property = RegisterProperty<string>(nameof(Value1628));
  public string Value1628
  {
    get => GetProperty(Value1628Property);
    set => SetProperty(Value1628Property, value);
  }


  public static readonly PropertyInfo<string> Value1629Property = RegisterProperty<string>(nameof(Value1629));
  public string Value1629
  {
    get => GetProperty(Value1629Property);
    set => SetProperty(Value1629Property, value);
  }


  public static readonly PropertyInfo<string> Value1630Property = RegisterProperty<string>(nameof(Value1630));
  public string Value1630
  {
    get => GetProperty(Value1630Property);
    set => SetProperty(Value1630Property, value);
  }


  public static readonly PropertyInfo<string> Value1631Property = RegisterProperty<string>(nameof(Value1631));
  public string Value1631
  {
    get => GetProperty(Value1631Property);
    set => SetProperty(Value1631Property, value);
  }


  public static readonly PropertyInfo<string> Value1632Property = RegisterProperty<string>(nameof(Value1632));
  public string Value1632
  {
    get => GetProperty(Value1632Property);
    set => SetProperty(Value1632Property, value);
  }


  public static readonly PropertyInfo<string> Value1633Property = RegisterProperty<string>(nameof(Value1633));
  public string Value1633
  {
    get => GetProperty(Value1633Property);
    set => SetProperty(Value1633Property, value);
  }


  public static readonly PropertyInfo<string> Value1634Property = RegisterProperty<string>(nameof(Value1634));
  public string Value1634
  {
    get => GetProperty(Value1634Property);
    set => SetProperty(Value1634Property, value);
  }


  public static readonly PropertyInfo<string> Value1635Property = RegisterProperty<string>(nameof(Value1635));
  public string Value1635
  {
    get => GetProperty(Value1635Property);
    set => SetProperty(Value1635Property, value);
  }


  public static readonly PropertyInfo<string> Value1636Property = RegisterProperty<string>(nameof(Value1636));
  public string Value1636
  {
    get => GetProperty(Value1636Property);
    set => SetProperty(Value1636Property, value);
  }


  public static readonly PropertyInfo<string> Value1637Property = RegisterProperty<string>(nameof(Value1637));
  public string Value1637
  {
    get => GetProperty(Value1637Property);
    set => SetProperty(Value1637Property, value);
  }


  public static readonly PropertyInfo<string> Value1638Property = RegisterProperty<string>(nameof(Value1638));
  public string Value1638
  {
    get => GetProperty(Value1638Property);
    set => SetProperty(Value1638Property, value);
  }


  public static readonly PropertyInfo<string> Value1639Property = RegisterProperty<string>(nameof(Value1639));
  public string Value1639
  {
    get => GetProperty(Value1639Property);
    set => SetProperty(Value1639Property, value);
  }


  public static readonly PropertyInfo<string> Value1640Property = RegisterProperty<string>(nameof(Value1640));
  public string Value1640
  {
    get => GetProperty(Value1640Property);
    set => SetProperty(Value1640Property, value);
  }


  public static readonly PropertyInfo<string> Value1641Property = RegisterProperty<string>(nameof(Value1641));
  public string Value1641
  {
    get => GetProperty(Value1641Property);
    set => SetProperty(Value1641Property, value);
  }


  public static readonly PropertyInfo<string> Value1642Property = RegisterProperty<string>(nameof(Value1642));
  public string Value1642
  {
    get => GetProperty(Value1642Property);
    set => SetProperty(Value1642Property, value);
  }


  public static readonly PropertyInfo<string> Value1643Property = RegisterProperty<string>(nameof(Value1643));
  public string Value1643
  {
    get => GetProperty(Value1643Property);
    set => SetProperty(Value1643Property, value);
  }


  public static readonly PropertyInfo<string> Value1644Property = RegisterProperty<string>(nameof(Value1644));
  public string Value1644
  {
    get => GetProperty(Value1644Property);
    set => SetProperty(Value1644Property, value);
  }


  public static readonly PropertyInfo<string> Value1645Property = RegisterProperty<string>(nameof(Value1645));
  public string Value1645
  {
    get => GetProperty(Value1645Property);
    set => SetProperty(Value1645Property, value);
  }


  public static readonly PropertyInfo<string> Value1646Property = RegisterProperty<string>(nameof(Value1646));
  public string Value1646
  {
    get => GetProperty(Value1646Property);
    set => SetProperty(Value1646Property, value);
  }


  public static readonly PropertyInfo<string> Value1647Property = RegisterProperty<string>(nameof(Value1647));
  public string Value1647
  {
    get => GetProperty(Value1647Property);
    set => SetProperty(Value1647Property, value);
  }


  public static readonly PropertyInfo<string> Value1648Property = RegisterProperty<string>(nameof(Value1648));
  public string Value1648
  {
    get => GetProperty(Value1648Property);
    set => SetProperty(Value1648Property, value);
  }


  public static readonly PropertyInfo<string> Value1649Property = RegisterProperty<string>(nameof(Value1649));
  public string Value1649
  {
    get => GetProperty(Value1649Property);
    set => SetProperty(Value1649Property, value);
  }


  public static readonly PropertyInfo<string> Value1650Property = RegisterProperty<string>(nameof(Value1650));
  public string Value1650
  {
    get => GetProperty(Value1650Property);
    set => SetProperty(Value1650Property, value);
  }


  public static readonly PropertyInfo<string> Value1651Property = RegisterProperty<string>(nameof(Value1651));
  public string Value1651
  {
    get => GetProperty(Value1651Property);
    set => SetProperty(Value1651Property, value);
  }


  public static readonly PropertyInfo<string> Value1652Property = RegisterProperty<string>(nameof(Value1652));
  public string Value1652
  {
    get => GetProperty(Value1652Property);
    set => SetProperty(Value1652Property, value);
  }


  public static readonly PropertyInfo<string> Value1653Property = RegisterProperty<string>(nameof(Value1653));
  public string Value1653
  {
    get => GetProperty(Value1653Property);
    set => SetProperty(Value1653Property, value);
  }


  public static readonly PropertyInfo<string> Value1654Property = RegisterProperty<string>(nameof(Value1654));
  public string Value1654
  {
    get => GetProperty(Value1654Property);
    set => SetProperty(Value1654Property, value);
  }


  public static readonly PropertyInfo<string> Value1655Property = RegisterProperty<string>(nameof(Value1655));
  public string Value1655
  {
    get => GetProperty(Value1655Property);
    set => SetProperty(Value1655Property, value);
  }


  public static readonly PropertyInfo<string> Value1656Property = RegisterProperty<string>(nameof(Value1656));
  public string Value1656
  {
    get => GetProperty(Value1656Property);
    set => SetProperty(Value1656Property, value);
  }


  public static readonly PropertyInfo<string> Value1657Property = RegisterProperty<string>(nameof(Value1657));
  public string Value1657
  {
    get => GetProperty(Value1657Property);
    set => SetProperty(Value1657Property, value);
  }


  public static readonly PropertyInfo<string> Value1658Property = RegisterProperty<string>(nameof(Value1658));
  public string Value1658
  {
    get => GetProperty(Value1658Property);
    set => SetProperty(Value1658Property, value);
  }


  public static readonly PropertyInfo<string> Value1659Property = RegisterProperty<string>(nameof(Value1659));
  public string Value1659
  {
    get => GetProperty(Value1659Property);
    set => SetProperty(Value1659Property, value);
  }


  public static readonly PropertyInfo<string> Value1660Property = RegisterProperty<string>(nameof(Value1660));
  public string Value1660
  {
    get => GetProperty(Value1660Property);
    set => SetProperty(Value1660Property, value);
  }


  public static readonly PropertyInfo<string> Value1661Property = RegisterProperty<string>(nameof(Value1661));
  public string Value1661
  {
    get => GetProperty(Value1661Property);
    set => SetProperty(Value1661Property, value);
  }


  public static readonly PropertyInfo<string> Value1662Property = RegisterProperty<string>(nameof(Value1662));
  public string Value1662
  {
    get => GetProperty(Value1662Property);
    set => SetProperty(Value1662Property, value);
  }


  public static readonly PropertyInfo<string> Value1663Property = RegisterProperty<string>(nameof(Value1663));
  public string Value1663
  {
    get => GetProperty(Value1663Property);
    set => SetProperty(Value1663Property, value);
  }


  public static readonly PropertyInfo<string> Value1664Property = RegisterProperty<string>(nameof(Value1664));
  public string Value1664
  {
    get => GetProperty(Value1664Property);
    set => SetProperty(Value1664Property, value);
  }


  public static readonly PropertyInfo<string> Value1665Property = RegisterProperty<string>(nameof(Value1665));
  public string Value1665
  {
    get => GetProperty(Value1665Property);
    set => SetProperty(Value1665Property, value);
  }


  public static readonly PropertyInfo<string> Value1666Property = RegisterProperty<string>(nameof(Value1666));
  public string Value1666
  {
    get => GetProperty(Value1666Property);
    set => SetProperty(Value1666Property, value);
  }


  public static readonly PropertyInfo<string> Value1667Property = RegisterProperty<string>(nameof(Value1667));
  public string Value1667
  {
    get => GetProperty(Value1667Property);
    set => SetProperty(Value1667Property, value);
  }


  public static readonly PropertyInfo<string> Value1668Property = RegisterProperty<string>(nameof(Value1668));
  public string Value1668
  {
    get => GetProperty(Value1668Property);
    set => SetProperty(Value1668Property, value);
  }


  public static readonly PropertyInfo<string> Value1669Property = RegisterProperty<string>(nameof(Value1669));
  public string Value1669
  {
    get => GetProperty(Value1669Property);
    set => SetProperty(Value1669Property, value);
  }


  public static readonly PropertyInfo<string> Value1670Property = RegisterProperty<string>(nameof(Value1670));
  public string Value1670
  {
    get => GetProperty(Value1670Property);
    set => SetProperty(Value1670Property, value);
  }


  public static readonly PropertyInfo<string> Value1671Property = RegisterProperty<string>(nameof(Value1671));
  public string Value1671
  {
    get => GetProperty(Value1671Property);
    set => SetProperty(Value1671Property, value);
  }


  public static readonly PropertyInfo<string> Value1672Property = RegisterProperty<string>(nameof(Value1672));
  public string Value1672
  {
    get => GetProperty(Value1672Property);
    set => SetProperty(Value1672Property, value);
  }


  public static readonly PropertyInfo<string> Value1673Property = RegisterProperty<string>(nameof(Value1673));
  public string Value1673
  {
    get => GetProperty(Value1673Property);
    set => SetProperty(Value1673Property, value);
  }


  public static readonly PropertyInfo<string> Value1674Property = RegisterProperty<string>(nameof(Value1674));
  public string Value1674
  {
    get => GetProperty(Value1674Property);
    set => SetProperty(Value1674Property, value);
  }


  public static readonly PropertyInfo<string> Value1675Property = RegisterProperty<string>(nameof(Value1675));
  public string Value1675
  {
    get => GetProperty(Value1675Property);
    set => SetProperty(Value1675Property, value);
  }


  public static readonly PropertyInfo<string> Value1676Property = RegisterProperty<string>(nameof(Value1676));
  public string Value1676
  {
    get => GetProperty(Value1676Property);
    set => SetProperty(Value1676Property, value);
  }


  public static readonly PropertyInfo<string> Value1677Property = RegisterProperty<string>(nameof(Value1677));
  public string Value1677
  {
    get => GetProperty(Value1677Property);
    set => SetProperty(Value1677Property, value);
  }


  public static readonly PropertyInfo<string> Value1678Property = RegisterProperty<string>(nameof(Value1678));
  public string Value1678
  {
    get => GetProperty(Value1678Property);
    set => SetProperty(Value1678Property, value);
  }


  public static readonly PropertyInfo<string> Value1679Property = RegisterProperty<string>(nameof(Value1679));
  public string Value1679
  {
    get => GetProperty(Value1679Property);
    set => SetProperty(Value1679Property, value);
  }


  public static readonly PropertyInfo<string> Value1680Property = RegisterProperty<string>(nameof(Value1680));
  public string Value1680
  {
    get => GetProperty(Value1680Property);
    set => SetProperty(Value1680Property, value);
  }


  public static readonly PropertyInfo<string> Value1681Property = RegisterProperty<string>(nameof(Value1681));
  public string Value1681
  {
    get => GetProperty(Value1681Property);
    set => SetProperty(Value1681Property, value);
  }


  public static readonly PropertyInfo<string> Value1682Property = RegisterProperty<string>(nameof(Value1682));
  public string Value1682
  {
    get => GetProperty(Value1682Property);
    set => SetProperty(Value1682Property, value);
  }


  public static readonly PropertyInfo<string> Value1683Property = RegisterProperty<string>(nameof(Value1683));
  public string Value1683
  {
    get => GetProperty(Value1683Property);
    set => SetProperty(Value1683Property, value);
  }


  public static readonly PropertyInfo<string> Value1684Property = RegisterProperty<string>(nameof(Value1684));
  public string Value1684
  {
    get => GetProperty(Value1684Property);
    set => SetProperty(Value1684Property, value);
  }


  public static readonly PropertyInfo<string> Value1685Property = RegisterProperty<string>(nameof(Value1685));
  public string Value1685
  {
    get => GetProperty(Value1685Property);
    set => SetProperty(Value1685Property, value);
  }


  public static readonly PropertyInfo<string> Value1686Property = RegisterProperty<string>(nameof(Value1686));
  public string Value1686
  {
    get => GetProperty(Value1686Property);
    set => SetProperty(Value1686Property, value);
  }


  public static readonly PropertyInfo<string> Value1687Property = RegisterProperty<string>(nameof(Value1687));
  public string Value1687
  {
    get => GetProperty(Value1687Property);
    set => SetProperty(Value1687Property, value);
  }


  public static readonly PropertyInfo<string> Value1688Property = RegisterProperty<string>(nameof(Value1688));
  public string Value1688
  {
    get => GetProperty(Value1688Property);
    set => SetProperty(Value1688Property, value);
  }


  public static readonly PropertyInfo<string> Value1689Property = RegisterProperty<string>(nameof(Value1689));
  public string Value1689
  {
    get => GetProperty(Value1689Property);
    set => SetProperty(Value1689Property, value);
  }


  public static readonly PropertyInfo<string> Value1690Property = RegisterProperty<string>(nameof(Value1690));
  public string Value1690
  {
    get => GetProperty(Value1690Property);
    set => SetProperty(Value1690Property, value);
  }


  public static readonly PropertyInfo<string> Value1691Property = RegisterProperty<string>(nameof(Value1691));
  public string Value1691
  {
    get => GetProperty(Value1691Property);
    set => SetProperty(Value1691Property, value);
  }


  public static readonly PropertyInfo<string> Value1692Property = RegisterProperty<string>(nameof(Value1692));
  public string Value1692
  {
    get => GetProperty(Value1692Property);
    set => SetProperty(Value1692Property, value);
  }


  public static readonly PropertyInfo<string> Value1693Property = RegisterProperty<string>(nameof(Value1693));
  public string Value1693
  {
    get => GetProperty(Value1693Property);
    set => SetProperty(Value1693Property, value);
  }


  public static readonly PropertyInfo<string> Value1694Property = RegisterProperty<string>(nameof(Value1694));
  public string Value1694
  {
    get => GetProperty(Value1694Property);
    set => SetProperty(Value1694Property, value);
  }


  public static readonly PropertyInfo<string> Value1695Property = RegisterProperty<string>(nameof(Value1695));
  public string Value1695
  {
    get => GetProperty(Value1695Property);
    set => SetProperty(Value1695Property, value);
  }


  public static readonly PropertyInfo<string> Value1696Property = RegisterProperty<string>(nameof(Value1696));
  public string Value1696
  {
    get => GetProperty(Value1696Property);
    set => SetProperty(Value1696Property, value);
  }


  public static readonly PropertyInfo<string> Value1697Property = RegisterProperty<string>(nameof(Value1697));
  public string Value1697
  {
    get => GetProperty(Value1697Property);
    set => SetProperty(Value1697Property, value);
  }


  public static readonly PropertyInfo<string> Value1698Property = RegisterProperty<string>(nameof(Value1698));
  public string Value1698
  {
    get => GetProperty(Value1698Property);
    set => SetProperty(Value1698Property, value);
  }


  public static readonly PropertyInfo<string> Value1699Property = RegisterProperty<string>(nameof(Value1699));
  public string Value1699
  {
    get => GetProperty(Value1699Property);
    set => SetProperty(Value1699Property, value);
  }


  public static readonly PropertyInfo<string> Value1700Property = RegisterProperty<string>(nameof(Value1700));
  public string Value1700
  {
    get => GetProperty(Value1700Property);
    set => SetProperty(Value1700Property, value);
  }


  public static readonly PropertyInfo<string> Value1701Property = RegisterProperty<string>(nameof(Value1701));
  public string Value1701
  {
    get => GetProperty(Value1701Property);
    set => SetProperty(Value1701Property, value);
  }


  public static readonly PropertyInfo<string> Value1702Property = RegisterProperty<string>(nameof(Value1702));
  public string Value1702
  {
    get => GetProperty(Value1702Property);
    set => SetProperty(Value1702Property, value);
  }


  public static readonly PropertyInfo<string> Value1703Property = RegisterProperty<string>(nameof(Value1703));
  public string Value1703
  {
    get => GetProperty(Value1703Property);
    set => SetProperty(Value1703Property, value);
  }


  public static readonly PropertyInfo<string> Value1704Property = RegisterProperty<string>(nameof(Value1704));
  public string Value1704
  {
    get => GetProperty(Value1704Property);
    set => SetProperty(Value1704Property, value);
  }


  public static readonly PropertyInfo<string> Value1705Property = RegisterProperty<string>(nameof(Value1705));
  public string Value1705
  {
    get => GetProperty(Value1705Property);
    set => SetProperty(Value1705Property, value);
  }


  public static readonly PropertyInfo<string> Value1706Property = RegisterProperty<string>(nameof(Value1706));
  public string Value1706
  {
    get => GetProperty(Value1706Property);
    set => SetProperty(Value1706Property, value);
  }


  public static readonly PropertyInfo<string> Value1707Property = RegisterProperty<string>(nameof(Value1707));
  public string Value1707
  {
    get => GetProperty(Value1707Property);
    set => SetProperty(Value1707Property, value);
  }


  public static readonly PropertyInfo<string> Value1708Property = RegisterProperty<string>(nameof(Value1708));
  public string Value1708
  {
    get => GetProperty(Value1708Property);
    set => SetProperty(Value1708Property, value);
  }


  public static readonly PropertyInfo<string> Value1709Property = RegisterProperty<string>(nameof(Value1709));
  public string Value1709
  {
    get => GetProperty(Value1709Property);
    set => SetProperty(Value1709Property, value);
  }


  public static readonly PropertyInfo<string> Value1710Property = RegisterProperty<string>(nameof(Value1710));
  public string Value1710
  {
    get => GetProperty(Value1710Property);
    set => SetProperty(Value1710Property, value);
  }


  public static readonly PropertyInfo<string> Value1711Property = RegisterProperty<string>(nameof(Value1711));
  public string Value1711
  {
    get => GetProperty(Value1711Property);
    set => SetProperty(Value1711Property, value);
  }


  public static readonly PropertyInfo<string> Value1712Property = RegisterProperty<string>(nameof(Value1712));
  public string Value1712
  {
    get => GetProperty(Value1712Property);
    set => SetProperty(Value1712Property, value);
  }


  public static readonly PropertyInfo<string> Value1713Property = RegisterProperty<string>(nameof(Value1713));
  public string Value1713
  {
    get => GetProperty(Value1713Property);
    set => SetProperty(Value1713Property, value);
  }


  public static readonly PropertyInfo<string> Value1714Property = RegisterProperty<string>(nameof(Value1714));
  public string Value1714
  {
    get => GetProperty(Value1714Property);
    set => SetProperty(Value1714Property, value);
  }


  public static readonly PropertyInfo<string> Value1715Property = RegisterProperty<string>(nameof(Value1715));
  public string Value1715
  {
    get => GetProperty(Value1715Property);
    set => SetProperty(Value1715Property, value);
  }


  public static readonly PropertyInfo<string> Value1716Property = RegisterProperty<string>(nameof(Value1716));
  public string Value1716
  {
    get => GetProperty(Value1716Property);
    set => SetProperty(Value1716Property, value);
  }


  public static readonly PropertyInfo<string> Value1717Property = RegisterProperty<string>(nameof(Value1717));
  public string Value1717
  {
    get => GetProperty(Value1717Property);
    set => SetProperty(Value1717Property, value);
  }


  public static readonly PropertyInfo<string> Value1718Property = RegisterProperty<string>(nameof(Value1718));
  public string Value1718
  {
    get => GetProperty(Value1718Property);
    set => SetProperty(Value1718Property, value);
  }


  public static readonly PropertyInfo<string> Value1719Property = RegisterProperty<string>(nameof(Value1719));
  public string Value1719
  {
    get => GetProperty(Value1719Property);
    set => SetProperty(Value1719Property, value);
  }


  public static readonly PropertyInfo<string> Value1720Property = RegisterProperty<string>(nameof(Value1720));
  public string Value1720
  {
    get => GetProperty(Value1720Property);
    set => SetProperty(Value1720Property, value);
  }


  public static readonly PropertyInfo<string> Value1721Property = RegisterProperty<string>(nameof(Value1721));
  public string Value1721
  {
    get => GetProperty(Value1721Property);
    set => SetProperty(Value1721Property, value);
  }


  public static readonly PropertyInfo<string> Value1722Property = RegisterProperty<string>(nameof(Value1722));
  public string Value1722
  {
    get => GetProperty(Value1722Property);
    set => SetProperty(Value1722Property, value);
  }


  public static readonly PropertyInfo<string> Value1723Property = RegisterProperty<string>(nameof(Value1723));
  public string Value1723
  {
    get => GetProperty(Value1723Property);
    set => SetProperty(Value1723Property, value);
  }


  public static readonly PropertyInfo<string> Value1724Property = RegisterProperty<string>(nameof(Value1724));
  public string Value1724
  {
    get => GetProperty(Value1724Property);
    set => SetProperty(Value1724Property, value);
  }


  public static readonly PropertyInfo<string> Value1725Property = RegisterProperty<string>(nameof(Value1725));
  public string Value1725
  {
    get => GetProperty(Value1725Property);
    set => SetProperty(Value1725Property, value);
  }


  public static readonly PropertyInfo<string> Value1726Property = RegisterProperty<string>(nameof(Value1726));
  public string Value1726
  {
    get => GetProperty(Value1726Property);
    set => SetProperty(Value1726Property, value);
  }


  public static readonly PropertyInfo<string> Value1727Property = RegisterProperty<string>(nameof(Value1727));
  public string Value1727
  {
    get => GetProperty(Value1727Property);
    set => SetProperty(Value1727Property, value);
  }


  public static readonly PropertyInfo<string> Value1728Property = RegisterProperty<string>(nameof(Value1728));
  public string Value1728
  {
    get => GetProperty(Value1728Property);
    set => SetProperty(Value1728Property, value);
  }


  public static readonly PropertyInfo<string> Value1729Property = RegisterProperty<string>(nameof(Value1729));
  public string Value1729
  {
    get => GetProperty(Value1729Property);
    set => SetProperty(Value1729Property, value);
  }


  public static readonly PropertyInfo<string> Value1730Property = RegisterProperty<string>(nameof(Value1730));
  public string Value1730
  {
    get => GetProperty(Value1730Property);
    set => SetProperty(Value1730Property, value);
  }


  public static readonly PropertyInfo<string> Value1731Property = RegisterProperty<string>(nameof(Value1731));
  public string Value1731
  {
    get => GetProperty(Value1731Property);
    set => SetProperty(Value1731Property, value);
  }


  public static readonly PropertyInfo<string> Value1732Property = RegisterProperty<string>(nameof(Value1732));
  public string Value1732
  {
    get => GetProperty(Value1732Property);
    set => SetProperty(Value1732Property, value);
  }


  public static readonly PropertyInfo<string> Value1733Property = RegisterProperty<string>(nameof(Value1733));
  public string Value1733
  {
    get => GetProperty(Value1733Property);
    set => SetProperty(Value1733Property, value);
  }


  public static readonly PropertyInfo<string> Value1734Property = RegisterProperty<string>(nameof(Value1734));
  public string Value1734
  {
    get => GetProperty(Value1734Property);
    set => SetProperty(Value1734Property, value);
  }


  public static readonly PropertyInfo<string> Value1735Property = RegisterProperty<string>(nameof(Value1735));
  public string Value1735
  {
    get => GetProperty(Value1735Property);
    set => SetProperty(Value1735Property, value);
  }


  public static readonly PropertyInfo<string> Value1736Property = RegisterProperty<string>(nameof(Value1736));
  public string Value1736
  {
    get => GetProperty(Value1736Property);
    set => SetProperty(Value1736Property, value);
  }


  public static readonly PropertyInfo<string> Value1737Property = RegisterProperty<string>(nameof(Value1737));
  public string Value1737
  {
    get => GetProperty(Value1737Property);
    set => SetProperty(Value1737Property, value);
  }


  public static readonly PropertyInfo<string> Value1738Property = RegisterProperty<string>(nameof(Value1738));
  public string Value1738
  {
    get => GetProperty(Value1738Property);
    set => SetProperty(Value1738Property, value);
  }


  public static readonly PropertyInfo<string> Value1739Property = RegisterProperty<string>(nameof(Value1739));
  public string Value1739
  {
    get => GetProperty(Value1739Property);
    set => SetProperty(Value1739Property, value);
  }


  public static readonly PropertyInfo<string> Value1740Property = RegisterProperty<string>(nameof(Value1740));
  public string Value1740
  {
    get => GetProperty(Value1740Property);
    set => SetProperty(Value1740Property, value);
  }


  public static readonly PropertyInfo<string> Value1741Property = RegisterProperty<string>(nameof(Value1741));
  public string Value1741
  {
    get => GetProperty(Value1741Property);
    set => SetProperty(Value1741Property, value);
  }


  public static readonly PropertyInfo<string> Value1742Property = RegisterProperty<string>(nameof(Value1742));
  public string Value1742
  {
    get => GetProperty(Value1742Property);
    set => SetProperty(Value1742Property, value);
  }


  public static readonly PropertyInfo<string> Value1743Property = RegisterProperty<string>(nameof(Value1743));
  public string Value1743
  {
    get => GetProperty(Value1743Property);
    set => SetProperty(Value1743Property, value);
  }


  public static readonly PropertyInfo<string> Value1744Property = RegisterProperty<string>(nameof(Value1744));
  public string Value1744
  {
    get => GetProperty(Value1744Property);
    set => SetProperty(Value1744Property, value);
  }


  public static readonly PropertyInfo<string> Value1745Property = RegisterProperty<string>(nameof(Value1745));
  public string Value1745
  {
    get => GetProperty(Value1745Property);
    set => SetProperty(Value1745Property, value);
  }


  public static readonly PropertyInfo<string> Value1746Property = RegisterProperty<string>(nameof(Value1746));
  public string Value1746
  {
    get => GetProperty(Value1746Property);
    set => SetProperty(Value1746Property, value);
  }


  public static readonly PropertyInfo<string> Value1747Property = RegisterProperty<string>(nameof(Value1747));
  public string Value1747
  {
    get => GetProperty(Value1747Property);
    set => SetProperty(Value1747Property, value);
  }


  public static readonly PropertyInfo<string> Value1748Property = RegisterProperty<string>(nameof(Value1748));
  public string Value1748
  {
    get => GetProperty(Value1748Property);
    set => SetProperty(Value1748Property, value);
  }


  public static readonly PropertyInfo<string> Value1749Property = RegisterProperty<string>(nameof(Value1749));
  public string Value1749
  {
    get => GetProperty(Value1749Property);
    set => SetProperty(Value1749Property, value);
  }


  public static readonly PropertyInfo<string> Value1750Property = RegisterProperty<string>(nameof(Value1750));
  public string Value1750
  {
    get => GetProperty(Value1750Property);
    set => SetProperty(Value1750Property, value);
  }


  public static readonly PropertyInfo<string> Value1751Property = RegisterProperty<string>(nameof(Value1751));
  public string Value1751
  {
    get => GetProperty(Value1751Property);
    set => SetProperty(Value1751Property, value);
  }


  public static readonly PropertyInfo<string> Value1752Property = RegisterProperty<string>(nameof(Value1752));
  public string Value1752
  {
    get => GetProperty(Value1752Property);
    set => SetProperty(Value1752Property, value);
  }


  public static readonly PropertyInfo<string> Value1753Property = RegisterProperty<string>(nameof(Value1753));
  public string Value1753
  {
    get => GetProperty(Value1753Property);
    set => SetProperty(Value1753Property, value);
  }


  public static readonly PropertyInfo<string> Value1754Property = RegisterProperty<string>(nameof(Value1754));
  public string Value1754
  {
    get => GetProperty(Value1754Property);
    set => SetProperty(Value1754Property, value);
  }


  public static readonly PropertyInfo<string> Value1755Property = RegisterProperty<string>(nameof(Value1755));
  public string Value1755
  {
    get => GetProperty(Value1755Property);
    set => SetProperty(Value1755Property, value);
  }


  public static readonly PropertyInfo<string> Value1756Property = RegisterProperty<string>(nameof(Value1756));
  public string Value1756
  {
    get => GetProperty(Value1756Property);
    set => SetProperty(Value1756Property, value);
  }


  public static readonly PropertyInfo<string> Value1757Property = RegisterProperty<string>(nameof(Value1757));
  public string Value1757
  {
    get => GetProperty(Value1757Property);
    set => SetProperty(Value1757Property, value);
  }


  public static readonly PropertyInfo<string> Value1758Property = RegisterProperty<string>(nameof(Value1758));
  public string Value1758
  {
    get => GetProperty(Value1758Property);
    set => SetProperty(Value1758Property, value);
  }


  public static readonly PropertyInfo<string> Value1759Property = RegisterProperty<string>(nameof(Value1759));
  public string Value1759
  {
    get => GetProperty(Value1759Property);
    set => SetProperty(Value1759Property, value);
  }


  public static readonly PropertyInfo<string> Value1760Property = RegisterProperty<string>(nameof(Value1760));
  public string Value1760
  {
    get => GetProperty(Value1760Property);
    set => SetProperty(Value1760Property, value);
  }


  public static readonly PropertyInfo<string> Value1761Property = RegisterProperty<string>(nameof(Value1761));
  public string Value1761
  {
    get => GetProperty(Value1761Property);
    set => SetProperty(Value1761Property, value);
  }


  public static readonly PropertyInfo<string> Value1762Property = RegisterProperty<string>(nameof(Value1762));
  public string Value1762
  {
    get => GetProperty(Value1762Property);
    set => SetProperty(Value1762Property, value);
  }


  public static readonly PropertyInfo<string> Value1763Property = RegisterProperty<string>(nameof(Value1763));
  public string Value1763
  {
    get => GetProperty(Value1763Property);
    set => SetProperty(Value1763Property, value);
  }


  public static readonly PropertyInfo<string> Value1764Property = RegisterProperty<string>(nameof(Value1764));
  public string Value1764
  {
    get => GetProperty(Value1764Property);
    set => SetProperty(Value1764Property, value);
  }


  public static readonly PropertyInfo<string> Value1765Property = RegisterProperty<string>(nameof(Value1765));
  public string Value1765
  {
    get => GetProperty(Value1765Property);
    set => SetProperty(Value1765Property, value);
  }


  public static readonly PropertyInfo<string> Value1766Property = RegisterProperty<string>(nameof(Value1766));
  public string Value1766
  {
    get => GetProperty(Value1766Property);
    set => SetProperty(Value1766Property, value);
  }


  public static readonly PropertyInfo<string> Value1767Property = RegisterProperty<string>(nameof(Value1767));
  public string Value1767
  {
    get => GetProperty(Value1767Property);
    set => SetProperty(Value1767Property, value);
  }


  public static readonly PropertyInfo<string> Value1768Property = RegisterProperty<string>(nameof(Value1768));
  public string Value1768
  {
    get => GetProperty(Value1768Property);
    set => SetProperty(Value1768Property, value);
  }


  public static readonly PropertyInfo<string> Value1769Property = RegisterProperty<string>(nameof(Value1769));
  public string Value1769
  {
    get => GetProperty(Value1769Property);
    set => SetProperty(Value1769Property, value);
  }


  public static readonly PropertyInfo<string> Value1770Property = RegisterProperty<string>(nameof(Value1770));
  public string Value1770
  {
    get => GetProperty(Value1770Property);
    set => SetProperty(Value1770Property, value);
  }


  public static readonly PropertyInfo<string> Value1771Property = RegisterProperty<string>(nameof(Value1771));
  public string Value1771
  {
    get => GetProperty(Value1771Property);
    set => SetProperty(Value1771Property, value);
  }


  public static readonly PropertyInfo<string> Value1772Property = RegisterProperty<string>(nameof(Value1772));
  public string Value1772
  {
    get => GetProperty(Value1772Property);
    set => SetProperty(Value1772Property, value);
  }


  public static readonly PropertyInfo<string> Value1773Property = RegisterProperty<string>(nameof(Value1773));
  public string Value1773
  {
    get => GetProperty(Value1773Property);
    set => SetProperty(Value1773Property, value);
  }


  public static readonly PropertyInfo<string> Value1774Property = RegisterProperty<string>(nameof(Value1774));
  public string Value1774
  {
    get => GetProperty(Value1774Property);
    set => SetProperty(Value1774Property, value);
  }


  public static readonly PropertyInfo<string> Value1775Property = RegisterProperty<string>(nameof(Value1775));
  public string Value1775
  {
    get => GetProperty(Value1775Property);
    set => SetProperty(Value1775Property, value);
  }


  public static readonly PropertyInfo<string> Value1776Property = RegisterProperty<string>(nameof(Value1776));
  public string Value1776
  {
    get => GetProperty(Value1776Property);
    set => SetProperty(Value1776Property, value);
  }


  public static readonly PropertyInfo<string> Value1777Property = RegisterProperty<string>(nameof(Value1777));
  public string Value1777
  {
    get => GetProperty(Value1777Property);
    set => SetProperty(Value1777Property, value);
  }


  public static readonly PropertyInfo<string> Value1778Property = RegisterProperty<string>(nameof(Value1778));
  public string Value1778
  {
    get => GetProperty(Value1778Property);
    set => SetProperty(Value1778Property, value);
  }


  public static readonly PropertyInfo<string> Value1779Property = RegisterProperty<string>(nameof(Value1779));
  public string Value1779
  {
    get => GetProperty(Value1779Property);
    set => SetProperty(Value1779Property, value);
  }


  public static readonly PropertyInfo<string> Value1780Property = RegisterProperty<string>(nameof(Value1780));
  public string Value1780
  {
    get => GetProperty(Value1780Property);
    set => SetProperty(Value1780Property, value);
  }


  public static readonly PropertyInfo<string> Value1781Property = RegisterProperty<string>(nameof(Value1781));
  public string Value1781
  {
    get => GetProperty(Value1781Property);
    set => SetProperty(Value1781Property, value);
  }


  public static readonly PropertyInfo<string> Value1782Property = RegisterProperty<string>(nameof(Value1782));
  public string Value1782
  {
    get => GetProperty(Value1782Property);
    set => SetProperty(Value1782Property, value);
  }


  public static readonly PropertyInfo<string> Value1783Property = RegisterProperty<string>(nameof(Value1783));
  public string Value1783
  {
    get => GetProperty(Value1783Property);
    set => SetProperty(Value1783Property, value);
  }


  public static readonly PropertyInfo<string> Value1784Property = RegisterProperty<string>(nameof(Value1784));
  public string Value1784
  {
    get => GetProperty(Value1784Property);
    set => SetProperty(Value1784Property, value);
  }


  public static readonly PropertyInfo<string> Value1785Property = RegisterProperty<string>(nameof(Value1785));
  public string Value1785
  {
    get => GetProperty(Value1785Property);
    set => SetProperty(Value1785Property, value);
  }


  public static readonly PropertyInfo<string> Value1786Property = RegisterProperty<string>(nameof(Value1786));
  public string Value1786
  {
    get => GetProperty(Value1786Property);
    set => SetProperty(Value1786Property, value);
  }


  public static readonly PropertyInfo<string> Value1787Property = RegisterProperty<string>(nameof(Value1787));
  public string Value1787
  {
    get => GetProperty(Value1787Property);
    set => SetProperty(Value1787Property, value);
  }


  public static readonly PropertyInfo<string> Value1788Property = RegisterProperty<string>(nameof(Value1788));
  public string Value1788
  {
    get => GetProperty(Value1788Property);
    set => SetProperty(Value1788Property, value);
  }


  public static readonly PropertyInfo<string> Value1789Property = RegisterProperty<string>(nameof(Value1789));
  public string Value1789
  {
    get => GetProperty(Value1789Property);
    set => SetProperty(Value1789Property, value);
  }


  public static readonly PropertyInfo<string> Value1790Property = RegisterProperty<string>(nameof(Value1790));
  public string Value1790
  {
    get => GetProperty(Value1790Property);
    set => SetProperty(Value1790Property, value);
  }


  public static readonly PropertyInfo<string> Value1791Property = RegisterProperty<string>(nameof(Value1791));
  public string Value1791
  {
    get => GetProperty(Value1791Property);
    set => SetProperty(Value1791Property, value);
  }


  public static readonly PropertyInfo<string> Value1792Property = RegisterProperty<string>(nameof(Value1792));
  public string Value1792
  {
    get => GetProperty(Value1792Property);
    set => SetProperty(Value1792Property, value);
  }


  public static readonly PropertyInfo<string> Value1793Property = RegisterProperty<string>(nameof(Value1793));
  public string Value1793
  {
    get => GetProperty(Value1793Property);
    set => SetProperty(Value1793Property, value);
  }


  public static readonly PropertyInfo<string> Value1794Property = RegisterProperty<string>(nameof(Value1794));
  public string Value1794
  {
    get => GetProperty(Value1794Property);
    set => SetProperty(Value1794Property, value);
  }


  public static readonly PropertyInfo<string> Value1795Property = RegisterProperty<string>(nameof(Value1795));
  public string Value1795
  {
    get => GetProperty(Value1795Property);
    set => SetProperty(Value1795Property, value);
  }


  public static readonly PropertyInfo<string> Value1796Property = RegisterProperty<string>(nameof(Value1796));
  public string Value1796
  {
    get => GetProperty(Value1796Property);
    set => SetProperty(Value1796Property, value);
  }


  public static readonly PropertyInfo<string> Value1797Property = RegisterProperty<string>(nameof(Value1797));
  public string Value1797
  {
    get => GetProperty(Value1797Property);
    set => SetProperty(Value1797Property, value);
  }


  public static readonly PropertyInfo<string> Value1798Property = RegisterProperty<string>(nameof(Value1798));
  public string Value1798
  {
    get => GetProperty(Value1798Property);
    set => SetProperty(Value1798Property, value);
  }


  public static readonly PropertyInfo<string> Value1799Property = RegisterProperty<string>(nameof(Value1799));
  public string Value1799
  {
    get => GetProperty(Value1799Property);
    set => SetProperty(Value1799Property, value);
  }


  public static readonly PropertyInfo<string> Value1800Property = RegisterProperty<string>(nameof(Value1800));
  public string Value1800
  {
    get => GetProperty(Value1800Property);
    set => SetProperty(Value1800Property, value);
  }
}