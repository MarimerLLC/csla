At the risk of being overly blunt - exposing your business objects directly as the external interface (contract) for your service is a terrible idea. It is a fundamentally flawed architectural choice.

Services are constructed to expose an immutable contractual interface. Changing that interface should take a conscious effort or act, because such changes will break consumers.

Architecturally, who would want to tie the internal implementation (your objects) to this immutable contract? You can't ever change your object model without breaking the contract - which means you can't ever change your object model. Which means you can't evolve or maintain your application.

A service is a type of interface - no different from a web or Windows interface. No one ever exposes their objects directly through those interfaces - they always have UI code (or data binding) between the interface and the object model.

The exact same thing applies to a service interface. You should never expose your objects directly through a service interface either. You should always have some UI code (or data binding equivalent) between the interface (contract) and the object model.

<!---Useful/related threads/posts:

[37548](http://forums.lhotka.net/forums/permalink/37549/37548/ShowThread.aspx#37548)--->
