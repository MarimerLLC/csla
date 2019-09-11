# A home for your business logic 
![CSLA .NET logo](https://raw.githubusercontent.com/MarimerLLC/csla/master/Support/Logos/csla%20win8_compact_s.png)

If you stop and think about it, we have all these great frameworks to create a UI and presentation layer: Windows Forms up through Xamarin Forms, and patterns like MVC, MVP, and MVVM. And we have myriad ways to interact with databases: ADO.NET, Entity Framework, Dapper, nHibernate, and many more. **But what about business logic?** 

Yes, the user experience is critical. And your data is invaluable. But neither are useful without business logic, and yet there's no official home for business logic.

MVC, MVP, and MVVM _explicitly_ tell you not to put business logic in your view, or presenter, or controller, or viewmodel. So what's left? The model.

But most people view the model as a set of anemic data container objects, typically shaped based on database tables or service endpoints, not on the actual scenario where the model is used.

CSLA is all about a different way. What if your model was designed using authentic object-oriented design concepts, so the model's shape is appropriate for the business scenario being addressed? And what if your objects were about _behavior_ instead of data? If the objects were actual actors in the user scenario, not just dumb data containers?

And then, suppose you had a framework that helped you create those behavior-based business domain objects so your code is:

1. Cross-platform, running unchanged anywhere you can run .NET code
1. Able to access an understandable, but extremely flexible and powerful, rules engine where you can implement all your validation, business, algorithmic, and authorization rules in a totally reusable manner
1. Standardized, so devs can use code snippets or code generators to become highly productive, and training costs are radically reduced
1. Fully data binding enabled, with the differences in data binding between Blazor, ASP.NET, Xamarin, Windows Forms, WPF, UWP, and other platforms abstracted away so devs don't have to worry about it - data binding just works, everywhere
1. Persistence-enabled, whether the data access layer is local on the client device, or remote on an app server via http, WCF, or any other network technology - again, differences are abstracted so devs can focus on business issues, not low-level "plumbing code"
1. Totally neutral in terms of data access technology: devs can use ADO.NET, Entity Framework, Dapper, or any other technology that's appropriate to access SQL Server, Oracle, your legacy AS/400, Excel files, web services, or combinations of all these and more

This is CSLA .NET: the framework that brings business logic to the same level as the UI and data access.

CSLA .NET: A home for your business logic.
