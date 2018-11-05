## Does CSLA .NET support WPF?
Yes. CSLA .NET has extensive support for WPF, both through its support for data binding, and with productivity controls and components in the Csla.Wpf namespace.

The [Expert C# 2008 Business Objects](https://www.apress.com/us/book/9781430210191#otherversion=9781430210207) and [Expert VB 2008 Business Objects](http://www.apress.com/9781430216384) books have a lot of information about WPF and how CSLA .NET supports this technology.

<!---Starting with CSLA .NET 4.0, the base collection types will fully support WPF binding. [See this page](WpfCollectionBinding) for more information.--->

## Should I use Silverlight or WPF?
This is a [good forum thread](https://cslanet.com/old-forum/10245.html) on the topic.
<!---and you can read [Rocky's thoughts](SilverlightOrWpf).--->

<!---##Does CSLA .NET work with the MVVM pattern?
Yes. [Click here](Mvvm) for more information.--->

## Why doesn't sorting work automatically in a DataGrid?
Prior to CSLA 4 all the CSLA collection/list classes inherit from BindingList<T>, which means they work with all UI technologies. They work great with Windows Forms and Web Forms, and work for the most part with WPF.

However, WPF doesn't fully honor the IBindingList interface, and in particular doesn't do automatic sorting of IBindingList collection types. This is a well-known limitation of WPF. WPF only fully supports ObservableCollection<T> types.

The problem is that Windows Forms doesn't work at all with ObservableCollection<T> lists. Well, it sort of works, but most common datagrid behaviors are unavailable, so it doesn't work in any real app scenario.

CSLA 4 changes the CSLA collection/list base classes to inherit from ObservableCollection<T>. The older BindingList<T> base classes are still there, but renamed, so there's still legacy support for Windows Forms, with primary support for WPF.

## Why doesn't the standard WPF datagrid seem to work right?
The WPF datagrid doesn't correctly implement some of the standard data binding behaviors. The CSLA .NET base classes are designed to support standard data binding, just like the DataTable or other bindable objects. Because the WPF datagrid doesn't follow Microsoft's own rules, it doesn't work correctly.

Unfortunately, the WPF datagrid doesn't implement events or methods that could be used to create workaround solutions either. I have been unable to find any way to "fix" the WPF datagrid to make it work correctly.

I (and others) have made Microsoft aware of this issue numerous times. You can go [vote on a connect issue](https://connect.microsoft.com/WPF/feedback/details/675473/wpf-datagrid-add-new-behaviour-and-ieditableobject-invocation) to encourage Microsoft to fix the control.

[This thread](https://cslanet.com/old-forum/10233.html) has more information about the issue.
