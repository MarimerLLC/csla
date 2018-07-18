## Can I serialize a CSLA .NET business object with the XmlSerializer?
No, you cannot directly use the Xml Serializer to serialize Csla objects.  There are too many limitations which make this impractical.

Besides, this is a [fundamentally flawed architectural/design choice](ObjectsAsServiceContract.md).

If you need to serialize your objects using the Xml Serializer, you should create Xml message classes and use the DataMapper to transfer data into and out of your Csla business object.

See Chapter 11 (2005) or Chapter 21 (2008) for more details on this solution.

## Can I serialize a CSLA .NET business object with the DataContractSerializer?
No, you cannot directly use the DCS to serialize Csla objects.  There are too many limitations which make this impractical.

Besides, this is a [fundamentally flawed architectural/design choice](ObjectsAsServiceContract.md).

If you need to serialize your objects using the DCS, you should create Xml message classes and use the DataMapper to transfer data into and out of your Csla business object.

See Chapter 11 (2005) or Chapter 21 (2008) for more details on this solution.

## Can I serialize a CSLA .NET business object with the DataContractJsonSerializer?
No, you cannot directly use the DCJS to serialize Csla objects.  There are too many limitations which make this impractical.

Besides, this is a [fundamentally flawed architectural/design choice](ObjectsAsServiceContract.md).

If you need to serialize your objects using the DCJS, you should create message classes and use the DataMapper to transfer data into and out of your Csla business object. Then serialize the message objects into JSON with the DCJS.

See Chapter 11 (2005) or Chapter 21 (2008) for more details on this solution. The chapters don't explicitly cover JSON, but the basic principles of building an XML interface are the same as building a JSON interface.
