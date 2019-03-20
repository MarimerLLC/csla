## What data access models are available in CSLA?
There are actually four data access models you can use with CSLA:

1. Encapsulated invoke: DataPortal_XYZ to abstract DAL
2. Factory implementation: ObjectFactory direct to data source
3. Factory invoke: ObjectFactory to abstract DAL
4. Encapsulated implementation: DataPortal_XYZ direct to data source

These are listed in my general order of preference.

I discuss all four models in the [Using CSLA 4: Data Access](http://store.lhotka.net/Default.aspx?tabid=1560&ProductID=22) ebook, as well as in the [Core 3.8 video series](http://store.lhotka.net/Default.aspx?tabid=1560&ProductID=18).

## Why doesn't my object have the right state after a Save()?
Variations on this include:

* The object remains dirty after save
* The database-assigned primary key value isn't in the object after save

The reason this happens is that you are not calling Save() properly. You **must** replace your original reference with the result of Save(). For example:

```_cust = _cust.Save();```


## I'm using Transactional.TransactionScope and getting DTC errors. What is wrong?
TS has a limitation that only one connection can be opened within the TransactionScope's lifetime.  Opening a second connection, even if using the same connection string, will promote the transaction from lightweight to a distributed transaction.

If you're going to use TransactionScope, be sure that the root object opens and closes the connection, and that child objects use this same connection.  LocalContext may be used to share the connection object.

CSLA .NET 3.5 introduced types in Csla.Data, such as ConnectionManager, that help manage connections, transactions, context objects and so forth to avoid the DTC issue.

## How do I save multiple root objects in one transaction?
When you save an editable root object the data portal may save that object, and all its child objects, in a transaction (if you apply the Transactional attribute). Sometimes the need exists to save more than one root object as part of a single transaction (a single data portal Save() call).

<!---[read more...](SaveMultipleRootObjects)--->

## What is the recommended concurrency model?
This [forum thread](https://cslanet.com/old-forum/5290.html) has good info, and links to good info.

## Can I use the repository pattern with CSLA?
Yes, [read about it here](https://cslanet.com/old-forum/9085.html).

## Can I use EF to directly load data into CSLA objects?
Maybe, see [this thread](https://cslanet.com/old-forum/9586.html) for information.
