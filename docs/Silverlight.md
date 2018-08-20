## Does CSLA .NET support Silverlight 2.0?
Yes. [CSLA .NET for Silverlight](http://www.lhotka.net/cslalight) version 3.6 was the first major development framework released for Silverlight, and it has been available since November 2008.

## Does CSLA .NET support Silverlight 3.0?
Yes. CSLA .NET for Silverlight versions 3.7 and 3.8 support Silverlight 3.

## Does CSLA .NET support Silverlight 4?
Yes. CSLA .NET for Silverlight version 3.8 works fine on Silverlight 4. 

CSLA .NET 4.0 will specifically target .NET 4.0 and Silverlight 4.

## Should I use Silverlight or WPF?
This is a [good forum thread](https://cslanet.com/old-forum/10245.html) on the topic. 
<!---and you can read [Rocky's thoughts](SilverlightOrWpf).--->

<!---##What are the major differences from CSLA .NET for Windows?
[Click here for information](WindowsVsSilverlight)--->

## How does the MobileFormatter serializer work?
The following blog posts show the thinking and implementation of MobileFormatter:

* [Silverlight serialization](http://www.lhotka.net/weblog/SilverlightSerializer.aspx)
* [Prototype serializer](http://www.lhotka.net/weblog/CSLALightObjectSerialization.aspx)
* [Serialization implementation](http://www.lhotka.net/weblog/CSLALightSerializationImplementation.aspx)

MobileFormatter only works with primitive values, values we special-cased (like Decimal, Guid, etc) and types that implement Csla.Core.IMobileObject.

Because it can be hard to implement IMobileObject correctly, there are several base types in Csla.Core that already do the work for you, including MobileObject, MobileList and MobileDictionary.

## How do I implement a lazy-loaded property in Silverlight?
Because all server access in Silverlight is async, implementing a lazy loaded property is a little more complex than in .NET.

<!---[FAQ info](LazyLoadedPropertyInSilverlight) and also --->
read this [blog post](http://www.lhotka.net/weblog/LazyLoadingChildrenInCSLA4AndSilverlight.aspx) .

<!---##Does CSLA .NET work with the MVVM pattern?
Yes. [Click here](Mvvm) for more information.--->

## How do I get a key value from a ComboBox?
The Silverlight 2 (and I think Silverlight 3) ComboBox control is missing some important features. This [blog post](http://www.lhotka.net/weblog/SilverlightComboBoxControlAndDataBinding.aspx) shows how to create a more complete control.
