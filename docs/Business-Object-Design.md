This is a list of frequently asked questions about designing business objects for CSLA .NET.

## Object-oriented design and CSLA .NET
This is a list of links to useful articles/threads on the topic:

* [Forum thread](https://cslanet.com/old-forum/3465.html)
* [Forum thread on reuse and coupling](https://cslanet.com/old-forum/9146.html)
* [Domain, entity, DTO objects and design](https://cslanet.com/old-forum/10243.html)

## Moving items from one read-only list or treeview to another
It is often the case that a tree is best represented as a set of read-only objects, not editable objects. In other words, ROLB instead of BLB.

You can then have a set of Command objects that are used to perform operations on the read-only objects. The Command object can change the server-side data (in the database) and if that succeeds it can alter the client-side object graph (or at least coordinate the updates to the object graph - I'm not advocating breaking encapsulation).

<!---[read more...](MovingItemsBetweenLists)--->

## Moving items from one editable list to another
[This thread](https://cslanet.com/old-forum/9214.html) has information on how to move a child object from one editable list (BLB) to another editable list.

## Creating a Name/Value list object from an enum
You can load an NVLB object from an enum.

<!---[read more...](CreateNVLBFromEnum)--->

## How is an empty date different from a null date?
There are rules for null values. Anything compared to null is null. 

But in many applications an empty date isn't null. In fact, an empty date is 
either infinitely far in the past or the future. You can compare an empty 
date to a specific date and find out if it is greater than or less than. 

This is particularly apparent in systems that allow the user to type in a 
date value on a TextBox - like Quicken, Money and every point of sale or 
sales order system I've ever created over the past couple decades. Users 
hate masked edit boxes and calendar controls - at least in any heads-down 
app where productivity is valuable. 

And in those cases, what does it mean when the Ship Date field is empty? It 
means the order hasn't shipped yet - so the date is effectively infinitely 
far in the future. Reports or other comparisons should consider that empty 
value as bigger than any other value. 

That isn't null - because if it were null you couldn't get a meaningful 
comparison result. 

So empty is not a specific date, but it isn't a null date either.

## Can I use code obfuscation with CSLA .NET?
To some degree, yes. You can't necessarily obfuscate CSLA .NET itself, but you can obfuscate some of your code.

[This thread](https://cslanet.com/old-forum/3257.html) has some information.

## Can I use external rules engines with CSLA .NET business objects?
[This thread](https://cslanet.com/old-forum/7528.html) has some information.

Please note that in CSLA 4 and higher you'll be using a more powerful and flexible business rule system. The [CSLA 4 rule system](http://www.lhotka.net/weblog/CSLA4BusinessRulesSubsystem.aspx) makes it much easier to invoke external rules engines and to interpret their results back into your business object. This doesn't mean you can always use external rules engines, because they often do have different expectations about application architecture, but the CSLA 4 rules system makes this a much more approachable problem.

## Can I change how IsDirty is managed?
There are various ways to change the definition and behavior of IsDirty. One of the most common requests is for an object to track its original field values, so if all properties are changed back to their original values IsDirty returns to false. Jason Bock describes how to [implement this behavior](http://www.jasonbock.net/JB/Default.aspx?blog=entry.9cc70d85bef34e2b9a683ba82615f8a3).

## What's this about a forceInit trick with PropertyInfo<T> fields?
[This thread](https://cslanet.com/old-forum/7986.html) has an explanation.

Please note that in CSLA 4 and higher it is recommended that you make your PropertyInfo<T> fields public. If you do this, then there is no need to worry about _forceInit, because CSLA includes code to ensure that the static fields are properly initialized. 

NOTE: It appears that the _forceInit trick just doesn't work in .NET 4 and SL4, because in Release mode the compiler is smart enough to optimize the field away completely. The only real solution is to make the PropertyInfo<T> fields public.

## What is the PrivateField relationship type used for?
[This thread](https://cslanet.com/old-forum/9005.html) has an explanation.

## How do I get DynamicRootList(DRL) or EditableRootListBase (ERLB) to do X, Y or Z?
ERLB exists to address one very specific set of requirements: a list of root objects, bound to a datagrid, where changes to a row (object) are committed as soon as the user leaves that row. [This forum post](https://cslanet.com/old-forum/9150.html) provides more information.

## How do I deal with lookup lists?
[This post](https://cslanet.com/old-forum/9337.html) has good information.

## How do I implement a Unit of Work object?
[This post](https://cslanet.com/old-forum/8535.html) has an example. You can also see an example and get more information from the [Using CSLA 4 ebook series](http://store.lhotka.net), and the [Core 3.8 video series](http://store.lhotka.net/).

[This post](https://cslanet.com/old-forum/10293.html) has more information.

## Keeping private fields in a list object?
You should not try to maintain private fields (or public properties) in a BusinessListBase or ReadOnlyListBase object. The serializer used for Silverlight and WP7 won't serialize your property values, and there is no field manager to help you in any list base classes.

The recommended solution is to have a parent class that contains the list, and put your fields/properties in that class. [This thread](https://cslanet.com/old-forum/9828.html) has more information.

## Is CSLA .NET threadsafe?
No. [This thread](https://cslanet.com/old-forum/4205.html) has good information. 
