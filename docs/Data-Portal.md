# Data Portal

## Should I use an n-tier or service-oriented architecture?

[This thread](https://cslanet.com/old-forum/10198.html) has some good information.

## CSLA 4 ApplicationContext in ASP.NET host

The ApplicationContext object must be managed differently in ASP.NET than other environments. In CSLA 4, Csla.dll doesn't (can't) reference System.Web.dll, so correct management of ApplicationContext for ASP.NET is in Csla.Web.dll. **All ASP.NET hosted code must reference Csla.Web.dll** to function correctly in CSLA 4. [More info...](https://cslanet.com/old-forum/9583.html)

## .NET: How do I configure WCF?

> ⚠️ Microsoft doesn't support WCF in .NET 6, and so CSLA only supports WCF for .NET Frameworks apps. You should consider using the HTTP data portal channel to replace any use of WCF.

WCF configuration can be very complex. There are many options, and they interact with each other in complex ways.

The Microsoft Patterns and Practices group wrote a [WCF configuration guide](http://wcfsecurity.codeplex.com), which is perhaps the best resource available.

## .NET: How do I set up WCF to use binary XML?

[This forum post](https://cslanet.com/old-forum/7494.html) has details on the configuration.

## SL: How do I implement compression in the Silverlight data portal?

The CSLA .NET for Silverlight data portal has hooks specifically designed to allow you to plug in a compression engine for data sent to/from the server. This is almost required, because the XML generated by the DataContractSerializer when serializing business objects gets very large fast!

[read more...](Silverlight.md)

## .NET: How do I use compression for the WCF data portal channel?

There are third party WCF bindings that implement compression. I recommend using one of those bindings.

[This thread](https://cslanet.com/old-forum/10805.html) discusses one option.

## .NET: How do I use compression for the Remoting data portal channel?

[This thread](https://cslanet.com/old-forum/8067.html) has information about one solution.

## SL: Can I (should I) use binary XML with the Silverlight data portal?

CSLA .NET 3.8 uses binary XML on your behalf. You should probably still use compression on the data (the previous topic in this FAQ), and you should probably configure your WCF endpoint and client to use binary XML as well.

[This thread](https://cslanet.com/old-forum/7258.html) has more information, along with performance data around the use of binary XML, compression and both combined.


## Data Size Limit in IIS 7

IIS 7 includes a new data size limit setting that may block transfer of large object graphs through the data portal. To overcome this issue, raise the limit:

```&lt;system.webServer&gt;
   &lt;security&gt;
     &lt;requestFiltering&gt;
       &lt;requestLimits maxAllowedContentLength="209715200" &gt;&lt;/requestLimits&gt;
     &lt;/requestFiltering&gt;
   &lt;/security&gt;
 &lt;/system.webServer&gt;
```

## SL: I get Maximum request length exceeded on data portal calls

The default size limits for WCF messages between Silverlight and ASP.NET are too low for any practical application scenarios when using the data portal. You need to up the limits by changing the WCF configuration in the client and server config files. For example:

Server

```&lt;system.serviceModel&gt;
     ......     
 &lt;basicHttpBinding&gt;
  &lt;binding name="basicHttpBinding_IWcfPortal"
           maxReceivedMessageSize="2147483647"
    &lt;readerQuotas maxBytesPerRead="2147483647"
                  maxArrayLength="2147483647"
                  maxStringContentLength="2147483647"
                  maxNameTableCharCount="2147483647"
                  maxDepth="2147483647"/&gt;
  &lt;/binding&gt;
 &lt;/basicHttpBinding&gt;
     ......     
&lt;/system.serviceModel&gt;

&lt;system.web&gt;
     ......     
     &lt;httpRuntime maxRequestLength="2147483647"/&gt;
     ......
&lt;/system.web&gt;
```

Client

```&lt;basicHttpBinding&gt;
  &lt;binding name="basicHttpBinding_IWcfPortal" 
           maxBufferSize="2147483647"
           maxReceivedMessageSize="2147483647”&gt;
  &lt;/binding&gt;
&lt;/basicHttpBinding&gt;
```

## How do I get server-side exception information on the client?

On the Windows/.NET platform the data portal returns a DataPortalException and the original exception is exposed as a BusinessException property.

On the Silverlight side things are a bit different, because it is not possible to serialize the actual exception object(s) from .NET back to Silverlight. Remember that SL is a different runtime and platform from .NET and the types don't always line up.

What we do instead, to provide as much information as we can, is we run through the exception chain on the server side and rip out most of the public property values. Those values are put into a WcfErrorInfo object chain, which looks somewhat similar to the original exception chain.

This object chain is available as an ErrorInfo property on the Silverlight DataPortalException object.

## Does the object graph get serialized when using the local data portal?

In version 3.5 and higher, by default the object graph is serialized when you invoke the data portal on a root object. 

Even in local mode the object graph is serialized once, to handle the case where the database throws an error in the middle of updating a set of objects. In that case, your object graph would be broken because some objects would have new (but now invalid) primary key values and timestamp values.

That scenario has never been a problem with a remote data portal, because the original object graph was still on the client, and the broken graph on the server is essentially discarded.

But for a long time (everything up to 3.5 basically) the local data portal would leave you with a broken object graph. In some of my older books I recommended writing UI code to clone the object graph and to have a try..catch block to solve this issue - but that's really not good, because it isn't a UI concern, and because most people didn't do this extra work.

So in 3.0 I added the option for the data portal to make the problem go away, but you had to turn it on (with the AutoCloneOnUpdate setting). And in 3.5 I changed the AutoCloneOnUpdate default to true, so the data portal does the right thing (cloning the object graph) by default, and you need to turn it off if you want the older (arguably broken) behavior.

## How can I have multiple configurations or servers for the data portal?

The data portal is designed for simplicity and extensibility. The simplicity part means there's only one configuration for the default data portal channels. The extensibility part means you can create your own proxy (and/or channel) to be as complex as you need.

In some cases you can subclass an existing proxy class (like WcfProxy) to achieve a level of flexiblity/complexity that isn't there by default. In other cases you'll need to create your own proxy from scratch.

This means you can create your own proxy class on the client side that reads different data portal configurations for different settings/scenarios or objects.

The easiest way to create your own proxy class is to use an existing proxy class as a template. For example, the WcfProxy class.
