<form name="mainForm" method="post" action="/weblog/Permalink.aspx?title=CSLA4BusinessRulesSubsystem" id="mainForm"><input type="hidden" name="__VIEWSTATE" id="__VIEWSTATE" value="/wEPDwUJMjcyNzcyNjQ3D2QWAmYPZBYCAgUPZBYEAgkPZBYEZg8VAR1odHRwOi8vd3d3Lmxob3RrYS5uZXQvd2VibG9nL2QCAQ8WAh4FdmFsdWUFBlNlYXJjaGQCHQ8PFgIeB1Zpc2libGVoZBYEZg8PFgIeBFRleHQFDVBpY2sgYSB0aGVtZTpkZAICDxBkZBYAZGSTi2Nc7QO55bI3HoVCk+NQOpurgM6N73B41LdnDVNNjg=="> <script type="text/javascript" language="JavaScript"> function showReferral() { var elems = document.getElementsByTagName('*'); var count = 0; for (var i=0;i<elems.length;i++) { if ( elems[i].id.indexOf('referralSpanHidden') != -1 ) { elems[i].style.display='inline'; count++; } else if ( elems[i].id.indexOf('referralMore') != -1 ) { elems[i].style.display='none'; count++; } if (count == 2) {break;} } } </script><script type="text/javascript"> var ct_img_expanded = 'http://www.lhotka.net/weblog/images/outlinedown.gif'; var ct_img_collapsed = 'http://www.lhotka.net/weblog/images/outlinearrow.gif'; (new Image(15,15)).src = ct_img_expanded; // caching (new Image(15,15)).src = ct_img_collapsed; // caching function ct_Expand(htmlNode,imgNode) { if (document.getElementById && document.getElementById(htmlNode) != null) { document.getElementById(imgNode).src=ct_img_expanded; document.getElementById(htmlNode).className='categoryListExpanded'; } } function ct_Collapse(htmlNode,imgNode) { if (document.getElementById && document.getElementById(htmlNode) != null) { document.getElementById(imgNode).src=ct_img_collapsed; document.getElementById(htmlNode).className='categoryListCollapsed'; } } function ct_toggleExpansionStatus(htmlNode,imgNode) { if (document.getElementById && document.getElementById(htmlNode) != null) { nodeState = document.getElementById(htmlNode).className; } if (nodeState == 'categoryListCollapsed') { ct_Expand(htmlNode,imgNode); } else { ct_Collapse(htmlNode,imgNode); } } </script><style type="text/css"> /* Class for expanded nested outlines */ .categoryListExpanded { width:100%; visibility: visible; display: block; } /* Class for collapsed nested outlines */ .categoryListCollapsed { width:100%; visibility: hidden; display: none; } </style><script type="text/javascript"> /* http://www.kryogenix.org/code/browser/searchhi/ */ /* Modified 20021006 to fix query string parsing and add case insensitivity */ function highlightWord(node,word) { // Iterate into this nodes childNodes if (node.hasChildNodes) { var hi_cn; for (hi_cn=0;hi_cn<node.childNodes.length;hi_cn++) { highlightWord(node.childNodes[hi_cn],word); } } // And do this node itself if (node.nodeType == 3) { // text node tempNodeVal = node.nodeValue.toLowerCase(); tempWordVal = word.toLowerCase(); if (tempNodeVal.indexOf(tempWordVal) != -1) { pn = node.parentNode; if (pn.className != "searchword") { // word has not already been highlighted! nv = node.nodeValue; ni = tempNodeVal.indexOf(tempWordVal); // Create a load of replacement nodes before = document.createTextNode(nv.substr(0,ni)); docWordVal = nv.substr(ni,word.length); after = document.createTextNode(nv.substr(ni+word.length)); hiwordtext = document.createTextNode(docWordVal); hiword = document.createElement("span"); hiword.className = "searchword"; hiword.appendChild(hiwordtext); pn.insertBefore(before,node); pn.insertBefore(hiword,node); pn.insertBefore(after,node); pn.removeChild(node); } } } } function googleSearchHighlight() { if (!document.createElement) return; ref = document.referrer; if (ref.indexOf('?') == -1 || ref.indexOf('http://www.lhotka.net/weblog/') != -1) { if (document.location.href.indexOf('PermaLink') != -1) { if (ref.indexOf('SearchView.aspx') == -1) return; } else { //Added by Scott Hanselman ref = document.location.href; if (ref.indexOf('?') == -1) return; } } qs = ref.substr(ref.indexOf('?')+1); qsa = qs.split('&'); for (i=0;i<qsa.length;i++) { qsip = qsa[i].split('='); if (qsip.length == 1) continue; if (qsip[0] == 'q' || qsip[0] == 'p') { // q= for Google, p= for Yahoo words = decodeURIComponent(qsip[1].replace(/\+/g,' ')).split(/\s+/); for (w=0;w<words.length;w++) { highlightWord(document.getElementsByTagName("body")[0],words[w]); } } } } // // addLoadEvent() // Adds event to window.onload without overwriting currently assigned onload functions. // Function found at Simon Willison's weblog - http://simon.incutio.com/ // function addLoadEvent(func) { var oldonload = window.onload; if (typeof window.onload != 'function') { window.onload = func; } else { window.onload = function() { oldonload(); func(); } } } addLoadEvent(googleSearchHighlight);</script><style type="text/css">.searchword { background-color: yellow; }</style> <input type="hidden" name="__VIEWSTATEGENERATOR" id="__VIEWSTATEGENERATOR" value="259B92DF"> <input type="hidden" name="__EVENTVALIDATION" id="__EVENTVALIDATION" value="/wEdAALFu3tYvzucsth4WitSKSrCSqyAu7A9VlBB96Ygyqx1bRi+FvnMcGU6mlqkx2qLo+I9pXmgQ3FMXCFdfPCiLzqx"> 

<div id="content">

<div id="left">

<div class="column">

<div style="float:right;margin:10px;margin-top:-20px">

<div style="padding-bottom:10px">

<div align="center" style="padding-bottom:10px"><script type="text/javascript"><!-- google_ad_client = "pub-4908425424216659"; /* 125x125, created 3/26/09 */ google_ad_slot = "4175676438"; google_ad_width = 125; google_ad_height = 125; //--></script></div>

</div>

</div>

[

# Rockford Lhotka's Blog

](http://www.lhotka.net/weblog)

[Home](http://www.lhotka.net/weblog) | [Lhotka.net](http://www.lhotka.net) | [CSLA .NET](http://www.cslanet.com)

<div class="date">[![](http://www.lhotka.net/weblog/themes/dasBlog/dayLink.gif)](http://www.lhotka.net/weblog/default,date,2010-04-05.aspx) Monday, April 5, 2010</div>

<div class="newsItems"><a name="ad607a960-8540-45d9-8bcb-5eb0b3681866"></a>[« CSLA 4 data portal changes](http://www.lhotka.net/weblog/CSLA4DataPortalChanges.aspx) | [Main](http://www.lhotka.net/weblog/default.aspx) | [CSLA 4 business rule chaining »](http://www.lhotka.net/weblog/CSLA4BusinessRuleChaining.aspx)

<div class="itemBoxStyle">

<div class="itemTitleStyle">[CSLA 4 business rules subsystem](http://www.lhotka.net/weblog/CSLA4BusinessRulesSubsystem.aspx)</div>

<div class="itemBodyStyle">

The first preview of the new CSLA 4 business rules subsystem will be available soon from [http://www.lhotka.net/cslanet/download.aspx](http://www.lhotka.net/weblog/ct.ashx?id=d607a960-8540-45d9-8bcb-5eb0b3681866&url=http%3a%2f%2fwww.lhotka.net%2fcslanet%2fdownload.aspx).

Of course there are a lot of other changes to [CSLA .NET](http://www.lhotka.net/weblog/ct.ashx?id=d607a960-8540-45d9-8bcb-5eb0b3681866&url=http%3a%2f%2fwww.cslanet.com%2f) in this preview, so make sure to carefully read the change log. Although there are a lot of breaking changes, most of them have pretty minimal impact on people who are already using the 3.8 coding style for classes. Except for the business rules – that impacts everyone.

This is a major change to the way business and validation rules work, with some pretty amazing new capabilities as a result.

The next preview will roll authorization rules into this as well, and that’ll be the last major change for CSLA 4\.

The business rule changes apply to both Silverlight and .NET – as always the idea is that the vast majority of your business code should be the same regardless of platform.

I’d like to summarize the primary changes from 3.8 to 4 in regards to business and validation rules.

## Simple changes

The simplest change is that the ValidationRules property in BusinessBase is now named BusinessRules. Also, the Csla.Validation namespace has been replaced by the Csla.Rules namespace.

If you are using DataAnnotations validation attributes in 3.8, they continue to work in 4 without change.

That was the easy part. Now for the interesting changes.

## Rule classes

Rule methods are replaced by rule classes. This means a 3.8 rule like this:

> private static bool MyRule<T>(T target, RuleArgs e) where T : Customer  
> {  
>   if (target.Sales < 10)  
>   {  
>     e.Description = "Customer has low sales";  
>     e.Severity = RuleSeverities.Information;  
>     return false;  
>   }  
>   else  
>     return true;  
> }

becomes a class like this:

> private class MyRule : Csla.Rules.BusinessRule  
> {  
>   protected override void Execute(RuleContext context)  
>   {  
>     var target = (Customer)context.Target;  
>     if (target.Sales < 10)  
>       context.AddInformationResult("Customer has low sales");  
>   }  
> }

It is the same basic rule, but packaged just a little differently. Rule types must implement IBusinessRule, but it is easier to inherit from BusinessRule, which provides a set of basic functionality you’ll typically need when implementing a rule.

The most important thing to understand is that the RuleContext parameter provides input and output for the Execute() method. The context parameter includes a bunch of information necessary to implement the rule, and has methods like AddErrorResult() that you use to indicate the results of the rule.

There’s one coding convention that you must follow: the protected properties from BusinessRule _must never be changed in Execute()_. I wish .NET had the ability to create an immutable type – a class where you could initialize properties as the object is created, and then ensure they are never changed after that point. But such a concept doesn’t exist, at least in C# or VB. But that is what you must do with rule objects. You can set the properties of the rule object as it is created. After that point, if you change properties of the rule object you are going to cause bugs. So do not change the properties of a rule in Execute() and you’ll be happy.

## AddRule Method

In the Customer business class you still override AddBusinessRules(), and that code looks like this:

> protected override void AddBusinessRules()  
> {  
>   base.AddBusinessRules();  
>   BusinessRules.AddRule(new MyRule { PrimaryProperty = SalesProperty });  
> }

The new AddRule() method accepts an instance of an IBusinessRule instead of a delegate like in 3.8\. This one instance of the rule is used for all instances of the business object type. In other words, exactly one instance of MyRule is created, and it is used by all instances of Customer. You must be aware of this when creating a rule class, because it means you can _never alter instance-level fields or properties after the rule is initialized_. If you do alter an instance-level field or property, you’ll affect the rule’s behavior for all Customer objects in the entire application – and that’d probably be a very bad thing.

Most rule methods that require a primary property will actually require it on the constructor. For example, look at the Required and MaxLength rules:

> protected override void AddBusinessRules()  
> {  
>   base.AddBusinessRules();  
>   BusinessRules.AddRule(new MyRule { PrimaryProperty = SalesProperty });  
> **  BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));  
>   BusinessRules.AddRule(new Csla.Rules.CommonRules.MaxLength(NameProperty, 20));  
> **}

The result is the same either way, but when you are writing your rules it is typically best to explicitly require a primary property on the constructor if you plan to use the primary property value. Later I’ll talk about per-object rules that have no primary property.

You should also notice that the properties are always Csla.Core.IPropertyInfo, not string property names. The new business rule system _requires_ that you use PropertyInfo fields to provide metadata about your properties. This syntax has been available since CSLA 3.5, and for my part I haven’t created a business class without them for years. This is one part of a larger effort to eliminate the use of string property names throughout CSLA and CSLA-based business code.

## Invoking Rules

In the previous AddRule() call, the PrimaryProperty property of the rule is set to SalesProperty. This is what links the rule to a specific property. So when the Sales property changes, this rule will be automatically invoked. As in 3.8, all rules are invoked (by default) when an object is created through the data portal, and can be invoked by calling BusinessRules.CheckRules(). And you can invoke rules for a property with BusinessRules.CheckRules(SalesProperty) – though you should notice that CheckRules() now requires an IPropertyInfo, not a string property name.

If you don’t provide a PrimaryProperty value as shown in this example, the rule method is associated with the business type, not a specific property. This means that the rule will run when CheckRules() is called with no parameter, or when the new CheckObjectRules() method is called to invoke rules attached to the business type.

In summary, there are now three CheckRules() overloads:

1.  CheckRules() – checks all rules for an object
2.  CheckObjectRules() – checks all rules not associated with a specific property (object rules)
3.  CheckRules(property) – checks all rules for a specific property

The same concepts of priority and short-circuiting from 3.8 apply in 4.

## InputProperties and InputPropertyValues

You can write that same rule just a little differently, making it unaware of the Target object reference:

> public class MyRule : Csla.Rules.BusinessRule  
> {  
>   public MyRule(Csla.Core.IPropertyInfo primaryProperty)  
>     : base(primaryProperty)  
>   {  
>     InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };  
>   }
> 
>   protected override void Execute(RuleContext context)  
>   {  
>     var sales = (double)context.InputPropertyValues[PrimaryProperty];  
>     if (sales < 10)  
>       context.AddInformationResult("Sales are low");  
>   }  
> }

This is a little more complex and a lot more flexible. Notice that the rule now requires a primary property, which is actually managed by the BusinessRule base class. Also notice that it adds the primaryProperty value to a list of InputProperties. This tells CSLA that any time this rule is invoked, it must be provided with that property value.

In the Execute() method notice how the sales value is retrieved from the context by using the InputPropertyValues dictionary. This dictionary contains the values from the business object based on the properties in the InputProperties list. A rule can require many property values be provided. Of course the rule will only work with objects that actually have those properties!

Because this rule will work with any object that can provide a double property value, the class is now public and could be placed in a library of rules.

The AddRule() method call in Customer must change a little:

> protected override void AddBusinessRules()  
> {  
>   base.AddBusinessRules();  
>   BusinessRules.AddRule(new MyRule(SalesProperty));  
> }

As before, the PrimaryProperty property is being set, but this time it is through the constructor instead of setting the property directly. This is because the rule now requires the primary property value, and can only work if it is supplied.

## Business Rules: Rules that modify data

The CSLA 4 rules system formally supports business rules. Really 3.8 did too, but it was a little confusing with the terminology, which is why ValidationRules is now called BusinessRules – to avoid that confusion.

The RuleContext object now has an AddOutValue() method you can use to provide an output value from your rule:

context.AddOutValue(NameProperty, “Rocky”);

When the rule completes, any out values are updated into the business object using the LoadProperty() method (so there’s no circular loop triggered).

This AddOutValue() technique is safe for synchronous and asynchronous rules, because the business object isn’t updated until the rule completes and control has returned to the UI thread.

If you are creating a synchronous rule, the RuleContext does have a Target property that provides you with a reference to the business object, so you could set the property yourself – just remember that setting a property typically triggers validation, which could cause an endless loop. Overall, it is best to use AddOutValue() to alter values.

## Validation Rules: Errors, Warnings and Information

As in 3.8, CSLA 4 allows validation rules to provide an error, a warning or an informational message. The RuleContext object has three methods:

1.  AddErrorResult()
2.  AddWarningResult()
3.  AddInformationalResult()

Each of these methods takes a string parameter which is the human-readable text to be displayed. A rule should only call one of these methods. If the rule calls them more than once, only the last one will have effect.

If AddErrorResult() is called, the rule is returning a failure result, and the business object will become invalid (IsSelfValid will be false).

If no method is called, or AddWarningResult() or AddInformationalResult() is called the rule is returning a success result and the business object will not become invalid. This is basically the same as the 3.8 behavior.

## Dependent Properties

The concept of dependent properties is quite different in the new system. Instead of an AddDependentProperty() method, a rule now indicates the properties it affects. This can be coded into the rule itself or specified when AddRule() is called. Either way, what’s happening is that every rule object maintains a list of AffectedProperties for that rule.

When a rule completes a PropertyChanged event is raised for every property in AffectedProperties, as well as any properties altered through AddOutValue().

It is also important to realize that, with an async rule, all properties in AffectedProperties will be marked busy when the rule starts, which typically means the user will see a busy animation (if you are using the PropertyStatus control) to show that the property has some outstanding operation running.

## Rule Sets

This is a major new feature of the rules system that is designed to support web applications where multiple customers/clients/organizations are sharing the same virtual root and application. In that case it might be necessary to have different business rules for each customer/client/organization. Rule sets are a way to accomplish this goal.

When you call BusinessRules.AddRule(), the rules are added to a rule set. Normally they are added to the default rule set, and if you only need one set of rules per type of object you can just be happy – it works. But if you need multiple sets of rules per type of object (such as one set of rules for customer A and a different set for customer B), then you’ll want to load each set of rules and attach those rules to your business object type.

This is done by changing the BusinessRules.RuleSet property. For example:

> protected override void AddBusinessRules()  
> {  
>   base.AddBusinessRules();
> 
>   // add rules to default rule set  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);
> 
>   // add rules to CustA rule set  
>   BusinessRules.RuleSet = "CustA";  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);
> 
>   // add rules to CustB rule set  
>   BusinessRules.RuleSet = "CustB";  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);  
>   BusinessRules.AddRule(...);  
>   
>   // use default rule set  
>   BusinessRules.RuleSet = “default”;  
> }

This code loads rules into three rule sets: default, CustA and CustB. It is up to you to set the RuleSet property to the correct value for each business object instance.

To set the rule set, set the BusinessRules.RuleSet property, then make sure to call BusinessRules.CheckRules() to apply the new set of rules.

The RuleSet value is serialized along with the state of the business object, so you can (and typically will) set the RuleSet property in your DataPortal_Create() and DataPortal_Fetch() methods, and that rule set will be used through the life of the object. I expect the most common scenario will be to set the RuleSet based on the identity of the current user, or some context value in Csla.ApplicationContext.ClientContext.

## Per-Object Rules

Finally, it is possible to have rules that are not attached to a specific primary property. These rules are attached to the business object. For example:

> protected override void AddBusinessRules()  
> {  
>   base.AddBusinessRules();  
>   BusinessRules.AddRule(new MyObjectRule());  
> }
> 
> private class MyObjectRule  
> {  
>   protected override void Execute(RuleContext context)  
>   {  
>     var target = (Customer)context.Target;  
>     // check rules here  
>   }  
> }

Notice that the AddRule() method has no primary property specified. Because there’s no primary property, the rule is attached to the business object itself. Normally this type of rule is a private rule in the business class, and uses the business object’s properties directly. But you can specify input and affected properties, as well as provide output values as discussed earlier.

Per-object rules are run in two cases:

*   You call BusinessRules.CheckRules()
*   You call BusinessRules.CheckObjectRules()

Per-object rules are not run automatically when properties change. So if you don’t invoke them, they won’t run.

## Async Rules

One of my primary goals in designing the new rule system is to provide a lot of consistency between sync and async rules. To this end, both sync and async rules are constructed the same way: by implementing IBusinessRule or subclassing BusinessRule.

But there are some extra restrictions on async rules. Most notably, async rules must get their input values through the RuleContext, and must provide any output values through RuleContext. To help enforce this, the context.Target property is always null in an async rule. This should help prevent the rule from trying to interact directly with the business object.

The reason this is so important, is that I assume an async rule will run some of its code on a background thread. Most of CSLA (and .NET) is not threadsafe, so having multiple threads interact with a business object will cause problems. If the async rule uses the RuleContext as a message to get and return values, CSLA can help ensure processing occurs on the correct thread.

Here’s a simple async rule. It is a little silly, in that all it does is ToUpper() a string value, but it should give you the idea:

> public class AsyncToUpper : Csla.Rules.BusinessRule  
> {  
>   public AsyncToUpper(Csla.Core.IPropertyInfo primaryProperty)  
>     : base(primaryProperty)  
>   {  
>     IsAsync = true;  
>     InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };  
>   }
> 
>   protected override void Execute(Csla.Rules.RuleContext context)  
>   {  
>     var bw = new System.ComponentModel.BackgroundWorker();  
>     bw.DoWork += (o, e) =>  
>     {  
>       var value = (string)context.InputPropertyValues[PrimaryProperty];  
>       context.AddOutValue(PrimaryProperty, value.ToUpper());  
>     };  
>     bw.RunWorkerCompleted += (o, e) =>  
>     {  
>       if (e.Error != null)  
>         context.AddErrorResult(e.Error.Message);  
>       context.Complete();  
>     };  
>     bw.RunWorkerAsync();  
>   }  
> }

The rule indicates that it is async by setting IsAsync to true in its constructor. This tells CSLA that the rule expects to run some or all of its logic on a background thread, so CSLA does some extra work for you. Specifically, CSLA marks the primary property and any AffectedProperties as busy before the rule starts and not busy when it ends. It also sets up the rule invocation so its results are handled through an async callback within CSLA. And it makes sure the RuleContext.Target property is null.

Next, notice that the InputProperties list is initialized in the rule’s constructor. This is the list of property values the rule requires to operate, and these property values will be provided through the RuleContext to the Execute() method in the InputPropertyValues dictionary.

The Execute() method itself is using a BackgroundWorker to run some code on a background thread. You can use BackgroundWorker or the async DataPortal and they’ll work fine. The one big requirement here is that whatever you use _must ensure that the competed callback is on the UI thread_. It is _your responsibility_ to make sure the completed callback is on the UI thread (if any). The BackgroundWorker and data portal do this for you automatically. If you use some other async technology you must take steps to make sure this is done correctly.

The AddOutValue() method is used to provide the output value. Remember that the actual business object property isn’t affected until after the rule completes, which is when CSLA is handling the results of your rule on the UI thread (where it can safely update the business object).

The RunWorkerCompleted event is used to handle any async errors. You’d do the same thing with the data portal, handling the FetchCompleted event (or similar). It is important to remember that exceptions occurring on a background thread are lost unless you carry them through. The code shown here is following what I typically do, which is to add an error result from the rule in the case of an async exception.

One last thing to keep in mind: there is exactly one instance of your rule object being used by all instances of a business type. Because of this, the Execute() method must be atomic and stateless. To put it another way, you should never, ever, ever alter any instance-level fields or properties of the rule object from the Execute() method. If you do alter an instance-level field or property of the rule object, that change will affect _all business objects_, not just the one you are running against right now. And with async rules you’ll run into race conditions and other nasty multi-threading issues. This is really the same restriction I mentioned earlier with sync rules – don’t change rule properties in Execute() – but it is so important that I wanted to reiterate the point here too.

While there are some restrictions on how you construct an async rule, I am pretty happy with how similar sync and async rules are to implement. In fact, all the async concepts (input values, AddOutValue()) work just fine with sync rules too.

## Moving from 3.8 to 4

While the new business rules system is somewhat different from the 3.8 implementation, the process of moving from 3.8 to 4 isn’t terribly painful.

*   Every rule method must become a rule class. This is a pretty mechanical process, but obviously does require some work
*   Every use of ValidationRules must be changed to BusinessRules
*   Every AddRule() call will be affected, which is another mechanical change
*   Dependent properties become AffectedProperties on each AddRule() method call

I was able to get the entire unit test codebase changed over in less than 8 hours – and that included changes not only for the business rules, but also for the data portal and several other breaking changes. I don’t mean to trivialize the effort required, but the changes are mostly mechanical in nature, so it is really a matter of plowing through the code to make the changes, which is mostly repetitive and boring, not really hard.

## Summary

I’m using the major version change from 3 to 4 as an opportunity to make some fairly large changes to CSLA. Changes that enable some key scenarios needed by [Magenic](http://www.lhotka.net/weblog/ct.ashx?id=d607a960-8540-45d9-8bcb-5eb0b3681866&url=http%3a%2f%2fwww.magenic.com%2f) customers, and requested by people on the [forum](http://www.lhotka.net/weblog/ct.ashx?id=d607a960-8540-45d9-8bcb-5eb0b3681866&url=http%3a%2f%2fforums.lhotka.net%2f) explicitly or implicitly. Some effort will be required to upgrade, but I suspect most people will find it well worth the time.

The big changes are:

*   Rule sets
*   Per-object rules
*   Rule objects instead of rule methods
*   Common model for sync and async rules

I’m pretty excited about these changes, and I hope you find them useful!

</div>

<div class="itemCategoryLinksStyle">[CSLA .NET](http://www.lhotka.net/weblog/CategoryView,category,CSLA%2B.NET.aspx)</div>

<div class="itemFooterStyle">Monday, April 5, 2010 12:40:05 PM (Central Standard Time, UTC-06:00)  [![#](http://www.lhotka.net/weblog/images/itemLink.gif "Use the link of this item to make permanent references to this entry.")](http://www.lhotka.net/weblog/CSLA4BusinessRulesSubsystem.aspx)    [Disclaimer](FormatPage.aspx?path=siteConfig/disclaimer.format.html "Click here to view the Disclaimer for posts") <span>Related posts:  
[In place list editing in Razor Pages](http://www.lhotka.net/weblog/InPlaceListEditingInRazorPages.aspx)  
[Microsoft's AI for Accessibility Program](http://www.lhotka.net/weblog/MicrosoftsAIForAccessibilityProgram.aspx)  
[WebAssembly talk at TCCC](http://www.lhotka.net/weblog/WebAssemblyTalkAtTCCC.aspx)  
[CSLA running on Blazor](http://www.lhotka.net/weblog/CSLARunningOnBlazor.aspx)  
[CSLA .NET 20 Year Anniversary](http://www.lhotka.net/weblog/CSLANET20YearAnniversary.aspx)  
[Frameworks have value](http://www.lhotka.net/weblog/FrameworksHaveValue.aspx)  
</span>  
</div>

</div>

<script type="text/javascript">window.addEventListener('load', function() { var mainpage = ""; if(mainpage.length == 0) { CommentsReset('d607a960-8540-45d9-8bcb-5eb0b3681866','http://www.lhotka.net/weblog/CSLA4BusinessRulesSubsystem.aspx','CSLA 4 business rules subsystem', 'en'); } }, false);</script>

<noscript>Please enable JavaScript to view the [comments powered by Disqus.](https://disqus.com/?ref_noscript)</noscript>

</div>

<div style="width:270px">

<div class="sidetitle">On this page....</div>

<div class="side">

<table class="titleListStyle" border="0">

<tbody>

<tr>

<td>[CSLA 4 business rules subsystem](/weblog/CSLA4BusinessRulesSubsystem.aspx#ad607a960-8540-45d9-8bcb-5eb0b3681866)</td>

</tr>

</tbody>

</table>

</div>

<div class="sidetitle">Search</div>

<div class="side"><script type="text/javascript"><!-- function doSearch(searchString) { // Trim string. searchString = searchString.replace(/^\s+|\s+$/g, ""); if (searchString.length > 0) { location.href = "http://www.lhotka.net/weblog/SearchView.aspx?q=" + encodeURIComponent(searchString); } return false; } --></script>

<div class="searchContainerStyle"><input id="searchString" onkeypress="if (event.keyCode == 13) { doSearch(searchString.value); return false; }" type="text" class="searchTextBoxStyle"> <input name="_ctl26:searchButton" type="button" id="_ctl26_searchButton" onclick="doSearch(searchString.value);" class="searchButtonStyle" value="Search"></div>

</div>

<div class="sidetitle">Archives</div>

<div class="side" align="center">[![Feed your aggregator (RSS 2.0)](http://www.lhotka.net/weblog/images/feed-icon-16x16.gif "Feed your aggregator (RSS 2.0)")](http://feeds.feedburner.com/RockfordLhotka)

<div class="archiveLinksContainerStyle">

<table class="archiveLinksTableStyle" border="0">

<tbody>

<tr>

<td>[June, 2018 (4)](http://www.lhotka.net/weblog/default,month,2018-06.aspx)</td>

</tr>

<tr>

<td>[May, 2018 (1)](http://www.lhotka.net/weblog/default,month,2018-05.aspx)</td>

</tr>

<tr>

<td>[April, 2018 (3)](http://www.lhotka.net/weblog/default,month,2018-04.aspx)</td>

</tr>

<tr>

<td>[March, 2018 (4)](http://www.lhotka.net/weblog/default,month,2018-03.aspx)</td>

</tr>

<tr>

<td>[December, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-12.aspx)</td>

</tr>

<tr>

<td>[November, 2017 (2)](http://www.lhotka.net/weblog/default,month,2017-11.aspx)</td>

</tr>

<tr>

<td>[October, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-10.aspx)</td>

</tr>

<tr>

<td>[September, 2017 (3)](http://www.lhotka.net/weblog/default,month,2017-09.aspx)</td>

</tr>

<tr>

<td>[August, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-08.aspx)</td>

</tr>

<tr>

<td>[July, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-07.aspx)</td>

</tr>

<tr>

<td>[June, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-06.aspx)</td>

</tr>

<tr>

<td>[May, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-05.aspx)</td>

</tr>

<tr>

<td>[April, 2017 (2)](http://www.lhotka.net/weblog/default,month,2017-04.aspx)</td>

</tr>

<tr>

<td>[March, 2017 (1)](http://www.lhotka.net/weblog/default,month,2017-03.aspx)</td>

</tr>

<tr>

<td>[February, 2017 (2)](http://www.lhotka.net/weblog/default,month,2017-02.aspx)</td>

</tr>

<tr>

<td>[January, 2017 (2)](http://www.lhotka.net/weblog/default,month,2017-01.aspx)</td>

</tr>

<tr>

<td>[December, 2016 (5)](http://www.lhotka.net/weblog/default,month,2016-12.aspx)</td>

</tr>

<tr>

<td>[November, 2016 (2)](http://www.lhotka.net/weblog/default,month,2016-11.aspx)</td>

</tr>

<tr>

<td>[August, 2016 (4)](http://www.lhotka.net/weblog/default,month,2016-08.aspx)</td>

</tr>

<tr>

<td>[July, 2016 (2)](http://www.lhotka.net/weblog/default,month,2016-07.aspx)</td>

</tr>

<tr>

<td>[June, 2016 (4)](http://www.lhotka.net/weblog/default,month,2016-06.aspx)</td>

</tr>

<tr>

<td>[May, 2016 (3)](http://www.lhotka.net/weblog/default,month,2016-05.aspx)</td>

</tr>

<tr>

<td>[April, 2016 (4)](http://www.lhotka.net/weblog/default,month,2016-04.aspx)</td>

</tr>

<tr>

<td>[March, 2016 (1)](http://www.lhotka.net/weblog/default,month,2016-03.aspx)</td>

</tr>

<tr>

<td>[February, 2016 (7)](http://www.lhotka.net/weblog/default,month,2016-02.aspx)</td>

</tr>

<tr>

<td>[January, 2016 (4)](http://www.lhotka.net/weblog/default,month,2016-01.aspx)</td>

</tr>

<tr>

<td>[December, 2015 (4)](http://www.lhotka.net/weblog/default,month,2015-12.aspx)</td>

</tr>

<tr>

<td>[November, 2015 (2)](http://www.lhotka.net/weblog/default,month,2015-11.aspx)</td>

</tr>

<tr>

<td>[October, 2015 (2)](http://www.lhotka.net/weblog/default,month,2015-10.aspx)</td>

</tr>

<tr>

<td>[September, 2015 (3)](http://www.lhotka.net/weblog/default,month,2015-09.aspx)</td>

</tr>

<tr>

<td>[August, 2015 (3)](http://www.lhotka.net/weblog/default,month,2015-08.aspx)</td>

</tr>

<tr>

<td>[July, 2015 (2)](http://www.lhotka.net/weblog/default,month,2015-07.aspx)</td>

</tr>

<tr>

<td>[June, 2015 (2)](http://www.lhotka.net/weblog/default,month,2015-06.aspx)</td>

</tr>

<tr>

<td>[May, 2015 (1)](http://www.lhotka.net/weblog/default,month,2015-05.aspx)</td>

</tr>

<tr>

<td>[February, 2015 (1)](http://www.lhotka.net/weblog/default,month,2015-02.aspx)</td>

</tr>

<tr>

<td>[January, 2015 (1)](http://www.lhotka.net/weblog/default,month,2015-01.aspx)</td>

</tr>

<tr>

<td>[October, 2014 (1)](http://www.lhotka.net/weblog/default,month,2014-10.aspx)</td>

</tr>

<tr>

<td>[August, 2014 (2)](http://www.lhotka.net/weblog/default,month,2014-08.aspx)</td>

</tr>

<tr>

<td>[July, 2014 (3)](http://www.lhotka.net/weblog/default,month,2014-07.aspx)</td>

</tr>

<tr>

<td>[June, 2014 (4)](http://www.lhotka.net/weblog/default,month,2014-06.aspx)</td>

</tr>

<tr>

<td>[May, 2014 (2)](http://www.lhotka.net/weblog/default,month,2014-05.aspx)</td>

</tr>

<tr>

<td>[April, 2014 (6)](http://www.lhotka.net/weblog/default,month,2014-04.aspx)</td>

</tr>

<tr>

<td>[March, 2014 (4)](http://www.lhotka.net/weblog/default,month,2014-03.aspx)</td>

</tr>

<tr>

<td>[February, 2014 (4)](http://www.lhotka.net/weblog/default,month,2014-02.aspx)</td>

</tr>

<tr>

<td>[January, 2014 (2)](http://www.lhotka.net/weblog/default,month,2014-01.aspx)</td>

</tr>

<tr>

<td>[December, 2013 (3)](http://www.lhotka.net/weblog/default,month,2013-12.aspx)</td>

</tr>

<tr>

<td>[October, 2013 (3)](http://www.lhotka.net/weblog/default,month,2013-10.aspx)</td>

</tr>

<tr>

<td>[August, 2013 (5)](http://www.lhotka.net/weblog/default,month,2013-08.aspx)</td>

</tr>

<tr>

<td>[July, 2013 (2)](http://www.lhotka.net/weblog/default,month,2013-07.aspx)</td>

</tr>

<tr>

<td>[May, 2013 (3)](http://www.lhotka.net/weblog/default,month,2013-05.aspx)</td>

</tr>

<tr>

<td>[April, 2013 (2)](http://www.lhotka.net/weblog/default,month,2013-04.aspx)</td>

</tr>

<tr>

<td>[March, 2013 (3)](http://www.lhotka.net/weblog/default,month,2013-03.aspx)</td>

</tr>

<tr>

<td>[February, 2013 (7)](http://www.lhotka.net/weblog/default,month,2013-02.aspx)</td>

</tr>

<tr>

<td>[January, 2013 (4)](http://www.lhotka.net/weblog/default,month,2013-01.aspx)</td>

</tr>

<tr>

<td>[December, 2012 (3)](http://www.lhotka.net/weblog/default,month,2012-12.aspx)</td>

</tr>

<tr>

<td>[November, 2012 (3)](http://www.lhotka.net/weblog/default,month,2012-11.aspx)</td>

</tr>

<tr>

<td>[October, 2012 (7)](http://www.lhotka.net/weblog/default,month,2012-10.aspx)</td>

</tr>

<tr>

<td>[September, 2012 (1)](http://www.lhotka.net/weblog/default,month,2012-09.aspx)</td>

</tr>

<tr>

<td>[August, 2012 (4)](http://www.lhotka.net/weblog/default,month,2012-08.aspx)</td>

</tr>

<tr>

<td>[July, 2012 (3)](http://www.lhotka.net/weblog/default,month,2012-07.aspx)</td>

</tr>

<tr>

<td>[June, 2012 (5)](http://www.lhotka.net/weblog/default,month,2012-06.aspx)</td>

</tr>

<tr>

<td>[May, 2012 (4)](http://www.lhotka.net/weblog/default,month,2012-05.aspx)</td>

</tr>

<tr>

<td>[April, 2012 (6)](http://www.lhotka.net/weblog/default,month,2012-04.aspx)</td>

</tr>

<tr>

<td>[March, 2012 (10)](http://www.lhotka.net/weblog/default,month,2012-03.aspx)</td>

</tr>

<tr>

<td>[February, 2012 (2)](http://www.lhotka.net/weblog/default,month,2012-02.aspx)</td>

</tr>

<tr>

<td>[January, 2012 (2)](http://www.lhotka.net/weblog/default,month,2012-01.aspx)</td>

</tr>

<tr>

<td>[December, 2011 (4)](http://www.lhotka.net/weblog/default,month,2011-12.aspx)</td>

</tr>

<tr>

<td>[November, 2011 (6)](http://www.lhotka.net/weblog/default,month,2011-11.aspx)</td>

</tr>

<tr>

<td>[October, 2011 (14)](http://www.lhotka.net/weblog/default,month,2011-10.aspx)</td>

</tr>

<tr>

<td>[September, 2011 (5)](http://www.lhotka.net/weblog/default,month,2011-09.aspx)</td>

</tr>

<tr>

<td>[August, 2011 (3)](http://www.lhotka.net/weblog/default,month,2011-08.aspx)</td>

</tr>

<tr>

<td>[June, 2011 (2)](http://www.lhotka.net/weblog/default,month,2011-06.aspx)</td>

</tr>

<tr>

<td>[May, 2011 (1)](http://www.lhotka.net/weblog/default,month,2011-05.aspx)</td>

</tr>

<tr>

<td>[April, 2011 (3)](http://www.lhotka.net/weblog/default,month,2011-04.aspx)</td>

</tr>

<tr>

<td>[March, 2011 (6)](http://www.lhotka.net/weblog/default,month,2011-03.aspx)</td>

</tr>

<tr>

<td>[February, 2011 (3)](http://www.lhotka.net/weblog/default,month,2011-02.aspx)</td>

</tr>

<tr>

<td>[January, 2011 (6)](http://www.lhotka.net/weblog/default,month,2011-01.aspx)</td>

</tr>

<tr>

<td>[December, 2010 (3)](http://www.lhotka.net/weblog/default,month,2010-12.aspx)</td>

</tr>

<tr>

<td>[November, 2010 (8)](http://www.lhotka.net/weblog/default,month,2010-11.aspx)</td>

</tr>

<tr>

<td>[October, 2010 (6)](http://www.lhotka.net/weblog/default,month,2010-10.aspx)</td>

</tr>

<tr>

<td>[September, 2010 (6)](http://www.lhotka.net/weblog/default,month,2010-09.aspx)</td>

</tr>

<tr>

<td>[August, 2010 (7)](http://www.lhotka.net/weblog/default,month,2010-08.aspx)</td>

</tr>

<tr>

<td>[July, 2010 (8)](http://www.lhotka.net/weblog/default,month,2010-07.aspx)</td>

</tr>

<tr>

<td>[June, 2010 (6)](http://www.lhotka.net/weblog/default,month,2010-06.aspx)</td>

</tr>

<tr>

<td>[May, 2010 (8)](http://www.lhotka.net/weblog/default,month,2010-05.aspx)</td>

</tr>

<tr>

<td>[April, 2010 (13)](http://www.lhotka.net/weblog/default,month,2010-04.aspx)</td>

</tr>

<tr>

<td>[March, 2010 (7)](http://www.lhotka.net/weblog/default,month,2010-03.aspx)</td>

</tr>

<tr>

<td>[February, 2010 (5)](http://www.lhotka.net/weblog/default,month,2010-02.aspx)</td>

</tr>

<tr>

<td>[January, 2010 (9)](http://www.lhotka.net/weblog/default,month,2010-01.aspx)</td>

</tr>

<tr>

<td>[December, 2009 (6)](http://www.lhotka.net/weblog/default,month,2009-12.aspx)</td>

</tr>

<tr>

<td>[November, 2009 (8)](http://www.lhotka.net/weblog/default,month,2009-11.aspx)</td>

</tr>

<tr>

<td>[October, 2009 (11)](http://www.lhotka.net/weblog/default,month,2009-10.aspx)</td>

</tr>

<tr>

<td>[September, 2009 (5)](http://www.lhotka.net/weblog/default,month,2009-09.aspx)</td>

</tr>

<tr>

<td>[August, 2009 (5)](http://www.lhotka.net/weblog/default,month,2009-08.aspx)</td>

</tr>

<tr>

<td>[July, 2009 (10)](http://www.lhotka.net/weblog/default,month,2009-07.aspx)</td>

</tr>

<tr>

<td>[June, 2009 (5)](http://www.lhotka.net/weblog/default,month,2009-06.aspx)</td>

</tr>

<tr>

<td>[May, 2009 (7)](http://www.lhotka.net/weblog/default,month,2009-05.aspx)</td>

</tr>

<tr>

<td>[April, 2009 (7)](http://www.lhotka.net/weblog/default,month,2009-04.aspx)</td>

</tr>

<tr>

<td>[March, 2009 (11)](http://www.lhotka.net/weblog/default,month,2009-03.aspx)</td>

</tr>

<tr>

<td>[February, 2009 (6)](http://www.lhotka.net/weblog/default,month,2009-02.aspx)</td>

</tr>

<tr>

<td>[January, 2009 (9)](http://www.lhotka.net/weblog/default,month,2009-01.aspx)</td>

</tr>

<tr>

<td>[December, 2008 (5)](http://www.lhotka.net/weblog/default,month,2008-12.aspx)</td>

</tr>

<tr>

<td>[November, 2008 (4)](http://www.lhotka.net/weblog/default,month,2008-11.aspx)</td>

</tr>

<tr>

<td>[October, 2008 (7)](http://www.lhotka.net/weblog/default,month,2008-10.aspx)</td>

</tr>

<tr>

<td>[September, 2008 (8)](http://www.lhotka.net/weblog/default,month,2008-09.aspx)</td>

</tr>

<tr>

<td>[August, 2008 (11)](http://www.lhotka.net/weblog/default,month,2008-08.aspx)</td>

</tr>

<tr>

<td>[July, 2008 (11)](http://www.lhotka.net/weblog/default,month,2008-07.aspx)</td>

</tr>

<tr>

<td>[June, 2008 (10)](http://www.lhotka.net/weblog/default,month,2008-06.aspx)</td>

</tr>

<tr>

<td>[May, 2008 (6)](http://www.lhotka.net/weblog/default,month,2008-05.aspx)</td>

</tr>

<tr>

<td>[April, 2008 (8)](http://www.lhotka.net/weblog/default,month,2008-04.aspx)</td>

</tr>

<tr>

<td>[March, 2008 (9)](http://www.lhotka.net/weblog/default,month,2008-03.aspx)</td>

</tr>

<tr>

<td>[February, 2008 (6)](http://www.lhotka.net/weblog/default,month,2008-02.aspx)</td>

</tr>

<tr>

<td>[January, 2008 (6)](http://www.lhotka.net/weblog/default,month,2008-01.aspx)</td>

</tr>

<tr>

<td>[December, 2007 (6)](http://www.lhotka.net/weblog/default,month,2007-12.aspx)</td>

</tr>

<tr>

<td>[November, 2007 (9)](http://www.lhotka.net/weblog/default,month,2007-11.aspx)</td>

</tr>

<tr>

<td>[October, 2007 (7)](http://www.lhotka.net/weblog/default,month,2007-10.aspx)</td>

</tr>

<tr>

<td>[September, 2007 (5)](http://www.lhotka.net/weblog/default,month,2007-09.aspx)</td>

</tr>

<tr>

<td>[August, 2007 (8)](http://www.lhotka.net/weblog/default,month,2007-08.aspx)</td>

</tr>

<tr>

<td>[July, 2007 (6)](http://www.lhotka.net/weblog/default,month,2007-07.aspx)</td>

</tr>

<tr>

<td>[June, 2007 (8)](http://www.lhotka.net/weblog/default,month,2007-06.aspx)</td>

</tr>

<tr>

<td>[May, 2007 (7)](http://www.lhotka.net/weblog/default,month,2007-05.aspx)</td>

</tr>

<tr>

<td>[April, 2007 (9)](http://www.lhotka.net/weblog/default,month,2007-04.aspx)</td>

</tr>

<tr>

<td>[March, 2007 (8)](http://www.lhotka.net/weblog/default,month,2007-03.aspx)</td>

</tr>

<tr>

<td>[February, 2007 (5)](http://www.lhotka.net/weblog/default,month,2007-02.aspx)</td>

</tr>

<tr>

<td>[January, 2007 (9)](http://www.lhotka.net/weblog/default,month,2007-01.aspx)</td>

</tr>

<tr>

<td>[December, 2006 (4)](http://www.lhotka.net/weblog/default,month,2006-12.aspx)</td>

</tr>

<tr>

<td>[November, 2006 (3)](http://www.lhotka.net/weblog/default,month,2006-11.aspx)</td>

</tr>

<tr>

<td>[October, 2006 (4)](http://www.lhotka.net/weblog/default,month,2006-10.aspx)</td>

</tr>

<tr>

<td>[September, 2006 (9)](http://www.lhotka.net/weblog/default,month,2006-09.aspx)</td>

</tr>

<tr>

<td>[August, 2006 (4)](http://www.lhotka.net/weblog/default,month,2006-08.aspx)</td>

</tr>

<tr>

<td>[July, 2006 (9)](http://www.lhotka.net/weblog/default,month,2006-07.aspx)</td>

</tr>

<tr>

<td>[June, 2006 (4)](http://www.lhotka.net/weblog/default,month,2006-06.aspx)</td>

</tr>

<tr>

<td>[May, 2006 (10)](http://www.lhotka.net/weblog/default,month,2006-05.aspx)</td>

</tr>

<tr>

<td>[April, 2006 (4)](http://www.lhotka.net/weblog/default,month,2006-04.aspx)</td>

</tr>

<tr>

<td>[March, 2006 (11)](http://www.lhotka.net/weblog/default,month,2006-03.aspx)</td>

</tr>

<tr>

<td>[February, 2006 (3)](http://www.lhotka.net/weblog/default,month,2006-02.aspx)</td>

</tr>

<tr>

<td>[January, 2006 (13)](http://www.lhotka.net/weblog/default,month,2006-01.aspx)</td>

</tr>

<tr>

<td>[December, 2005 (6)](http://www.lhotka.net/weblog/default,month,2005-12.aspx)</td>

</tr>

<tr>

<td>[November, 2005 (7)](http://www.lhotka.net/weblog/default,month,2005-11.aspx)</td>

</tr>

<tr>

<td>[October, 2005 (4)](http://www.lhotka.net/weblog/default,month,2005-10.aspx)</td>

</tr>

<tr>

<td>[September, 2005 (9)](http://www.lhotka.net/weblog/default,month,2005-09.aspx)</td>

</tr>

<tr>

<td>[August, 2005 (6)](http://www.lhotka.net/weblog/default,month,2005-08.aspx)</td>

</tr>

<tr>

<td>[July, 2005 (7)](http://www.lhotka.net/weblog/default,month,2005-07.aspx)</td>

</tr>

<tr>

<td>[June, 2005 (5)](http://www.lhotka.net/weblog/default,month,2005-06.aspx)</td>

</tr>

<tr>

<td>[May, 2005 (4)](http://www.lhotka.net/weblog/default,month,2005-05.aspx)</td>

</tr>

<tr>

<td>[April, 2005 (7)](http://www.lhotka.net/weblog/default,month,2005-04.aspx)</td>

</tr>

<tr>

<td>[March, 2005 (16)](http://www.lhotka.net/weblog/default,month,2005-03.aspx)</td>

</tr>

<tr>

<td>[February, 2005 (17)](http://www.lhotka.net/weblog/default,month,2005-02.aspx)</td>

</tr>

<tr>

<td>[January, 2005 (17)](http://www.lhotka.net/weblog/default,month,2005-01.aspx)</td>

</tr>

<tr>

<td>[December, 2004 (13)](http://www.lhotka.net/weblog/default,month,2004-12.aspx)</td>

</tr>

<tr>

<td>[November, 2004 (7)](http://www.lhotka.net/weblog/default,month,2004-11.aspx)</td>

</tr>

<tr>

<td>[October, 2004 (14)](http://www.lhotka.net/weblog/default,month,2004-10.aspx)</td>

</tr>

<tr>

<td>[September, 2004 (11)](http://www.lhotka.net/weblog/default,month,2004-09.aspx)</td>

</tr>

<tr>

<td>[August, 2004 (7)](http://www.lhotka.net/weblog/default,month,2004-08.aspx)</td>

</tr>

<tr>

<td>[July, 2004 (3)](http://www.lhotka.net/weblog/default,month,2004-07.aspx)</td>

</tr>

<tr>

<td>[June, 2004 (6)](http://www.lhotka.net/weblog/default,month,2004-06.aspx)</td>

</tr>

<tr>

<td>[May, 2004 (3)](http://www.lhotka.net/weblog/default,month,2004-05.aspx)</td>

</tr>

<tr>

<td>[April, 2004 (2)](http://www.lhotka.net/weblog/default,month,2004-04.aspx)</td>

</tr>

<tr>

<td>[March, 2004 (1)](http://www.lhotka.net/weblog/default,month,2004-03.aspx)</td>

</tr>

<tr>

<td>[February, 2004 (5)](http://www.lhotka.net/weblog/default,month,2004-02.aspx)</td>

</tr>

</tbody>

</table>

</div>

</div>

<div class="sidetitle">Categories</div>

<div class="side">

<div class="categoryListContainerStyle">

<table class="categoryListTableStyle" border="0">

<tbody>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=%20AspNetCore)  [AspNetCore](http://www.lhotka.net/weblog/CategoryView,category,%2BAspNetCore.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=.NET%20Core) [.NET Core](http://www.lhotka.net/weblog/CategoryView,category,.NET%2BCore.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Android) [Android](http://www.lhotka.net/weblog/CategoryView,category,Android.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Architecture) [Architecture](http://www.lhotka.net/weblog/CategoryView,category,Architecture.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=ASP.NET%20MVC) [ASP.NET MVC](http://www.lhotka.net/weblog/CategoryView,category,ASP.NET%2BMVC.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Blazor) [Blazor](http://www.lhotka.net/weblog/CategoryView,category,Blazor.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Books) [Books](http://www.lhotka.net/weblog/CategoryView,category,Books.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Bxf) [Bxf](http://www.lhotka.net/weblog/CategoryView,category,Bxf.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=CSLA%20.NET) [CSLA .NET](http://www.lhotka.net/weblog/CategoryView,category,CSLA%2B.NET.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=dasBlog) [dasBlog](http://www.lhotka.net/weblog/CategoryView,category,dasBlog.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Distributed%20OO) [Distributed OO](http://www.lhotka.net/weblog/CategoryView,category,Distributed%2BOO.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=git) [git](http://www.lhotka.net/weblog/CategoryView,category,git.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=h5js) [h5js](http://www.lhotka.net/weblog/CategoryView,category,h5js.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Hololens) [Hololens](http://www.lhotka.net/weblog/CategoryView,category,Hololens.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=iOS) [iOS](http://www.lhotka.net/weblog/CategoryView,category,iOS.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=JavaScript) [JavaScript](http://www.lhotka.net/weblog/CategoryView,category,JavaScript.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Magenic) [Magenic](http://www.lhotka.net/weblog/CategoryView,category,Magenic.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Microsoft%20.NET) [Microsoft .NET](http://www.lhotka.net/weblog/CategoryView,category,Microsoft%2B.NET.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=mono) [mono](http://www.lhotka.net/weblog/CategoryView,category,mono.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=MonoDroid) [MonoDroid](http://www.lhotka.net/weblog/CategoryView,category,MonoDroid.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=MonoTouch) [MonoTouch](http://www.lhotka.net/weblog/CategoryView,category,MonoTouch.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=MOslo) [MOslo](http://www.lhotka.net/weblog/CategoryView,category,MOslo.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=News) [News](http://www.lhotka.net/weblog/CategoryView,category,News.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Ooui) [Ooui](http://www.lhotka.net/weblog/CategoryView,category,Ooui.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Programming) [Programming](http://www.lhotka.net/weblog/CategoryView,category,Programming.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Service-Oriented) [Service-Oriented](http://www.lhotka.net/weblog/CategoryView,category,Service-Oriented.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Silverlight) [Silverlight](http://www.lhotka.net/weblog/CategoryView,category,Silverlight.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Surface) [Surface](http://www.lhotka.net/weblog/CategoryView,category,Surface.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=UWP) [UWP](http://www.lhotka.net/weblog/CategoryView,category,UWP.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=VSTS) [VSTS](http://www.lhotka.net/weblog/CategoryView,category,VSTS.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WCF) [WCF](http://www.lhotka.net/weblog/CategoryView,category,WCF.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Web) [Web](http://www.lhotka.net/weblog/CategoryView,category,Web.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WebAssembly) [WebAssembly](http://www.lhotka.net/weblog/CategoryView,category,WebAssembly.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Windows%2010) [Windows 10](http://www.lhotka.net/weblog/CategoryView,category,Windows%2B10.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Windows%208) [Windows 8](http://www.lhotka.net/weblog/CategoryView,category,Windows%2B8.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Windows%20Azure) [Windows Azure](http://www.lhotka.net/weblog/CategoryView,category,Windows%2BAzure.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Windows%20Forms) [Windows Forms](http://www.lhotka.net/weblog/CategoryView,category,Windows%2BForms.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Windows%20Phone) [Windows Phone](http://www.lhotka.net/weblog/CategoryView,category,Windows%2BPhone.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WinRT) [WinRT](http://www.lhotka.net/weblog/CategoryView,category,WinRT.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WomenInTech) [WomenInTech](http://www.lhotka.net/weblog/CategoryView,category,WomenInTech.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Workflow) [Workflow](http://www.lhotka.net/weblog/CategoryView,category,Workflow.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WP7) [WP7](http://www.lhotka.net/weblog/CategoryView,category,WP7.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WP8) [WP8](http://www.lhotka.net/weblog/CategoryView,category,WP8.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=WPF) [WPF](http://www.lhotka.net/weblog/CategoryView,category,WPF.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Xamarin) [Xamarin](http://www.lhotka.net/weblog/CategoryView,category,Xamarin.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Xbox) [Xbox](http://www.lhotka.net/weblog/CategoryView,category,Xbox.aspx)</td>

</tr>

<tr>

<td class="categoryListCellStyle">[![](http://www.lhotka.net/weblog/images/feedButton.gif)](http://www.lhotka.net/weblog/SyndicationService.asmx/GetRssCategory?categoryName=Zune) [Zune](http://www.lhotka.net/weblog/CategoryView,category,Zune.aspx)</td>

</tr>

</tbody>

</table>

</div>

</div>

<div class="sidetitle">About</div>

<div class="side">

Powered by: newtelligence dasBlog 2.0.7226.0

**Disclaimer**  
The opinions expressed herein are my own personal opinions and do not represent my employer's view in any way.

© Copyright 2018, Marimer LLC

[![Send mail to the author(s)](http://www.lhotka.net/weblog/images/mailTo.gif "Send mail to the author(s)")](javascript:var e1='%6c%68%6f%74%6b%61%2e%6e%65%74',e2='mailto: ', e3='%72%6f%63%6b%79';var e0=e2+e3+'%40'+e1+'?Subject=Comments on: Rockford Lhotka - CSLA 4 business rules subsystem';(window.location?window.location.replace(e0):document.write(e0)); ) E-mail

</div>

[Sign In](Login.aspx)</div>

</div>

</div>

</div>

<script type="text/javascript">/* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */ var disqus_shortname = 'rockfordlhotka'; var disqus_identifier = ''; var disqus_url = 'http://www.lhotka.net/weblog/'; var disqus_config = function () { this.language = "en"; }; /* * * Disqus Reset Function * * */ var CommentsReset = function (newIdentifier, newUrl, newTitle, newLanguage) { var remdiv = document.getElementById('disqus_thread') if(remdiv !== null) { remdiv.parentNode.removeChild(remdiv); } var objTo = document.getElementById('comment_' + newIdentifier) var dsqdiv = document.createElement('div'); dsqdiv.id = "disqus_thread"; objTo.appendChild(dsqdiv) DISQUS.reset({ reload: true, config: function () { this.page.identifier = newIdentifier; this.page.url = newUrl; this.page.title = newTitle; this.language = newLanguage; } }); }; /* * * DON'T EDIT BELOW THIS LINE * * */ (function() { var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true; dsq.src = 'http://' + disqus_shortname + '.disqus.com/embed.js'; (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq); })();</script></form>
