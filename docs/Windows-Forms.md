## Is Windows Forms supported in CSLA .NET 4.0?
Yes, but your business collections need to inherit from base classes in Csla.Windows, not the ones in the main Csla namespace. Starting with CSLA .NET 4.0, the base collection types fully support WPF binding. 
<!---[See this page](WpfCollectionBinding) for more information.--->

## Windows Forms isn't in Expert 2008 Business Objects - where do I get info?
It was a difficult choice to leave Windows Forms out of the 2008 book, but space/cost constraints meant I had to make hard choices. 

The important aspects of Windows Forms are covered in two places. 

1. The 2008 book does include discussion of the CslaActionExtender class, which is a new Windows Forms control that is very helpful – it should be listed in the index.
2. The Using CSLA .NET 3.0 ebook (available from [http://store.lhotka.net](http://store.lhotka.net)) has a chapter on Windows Forms which covers data binding and some important issues you need to understand to effectively use data binding against objects.

If you need more basic information about Windows Forms, I’m afraid you’ll have to go back to the ''Expert 2005 Business Objects'' book. That book doesn’t contain the in-depth data binding information that’s in the ebook, but does provide a more basic walk-through of the creation of the PTWin project.

## Loading data into a DataGridView seems slow
Read this thread for suggestions: https://cslanet.com/old-forum/11736.html
