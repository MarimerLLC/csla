# Security

Links to blogs or other resources regarding various aspects of securing the CSLA data portal and other features.

## CSLA 8+

In CSLA 8 the data portal was enhanced to always rerun business rules on the server for each inbound request from a client. This is a significant security enhancement, as it ensures that business rules are always enforced, regardless of the client's behavior.

Also in CSLA 8, the default behavior was changed so the user identity never automatically flows from the client to the server. You can still enable that behavior if you want, but it is disabled by default. This enhances security by default.

## CSLA 5 and CSLA 6 Remote Data Portal Security

* [CSLA 5 and CSLA 6 Remote Data Portal Security](https://blog.dotnotstandard.com/blog/csla-data-portal-security)

## Using Windows Authentication

* https://cslanet.com/old-forum/2331.html
* https://cslanet.com/old-forum/844.html

## Implementing Permission-based authorization

The .NET framework supports a role-based authorization model, and CSLA .NET is built on top of that model. However, it is possible to implement a more granular level of permission-based authorization on this model as well.

* Here's a [forum thread](https://cslanet.com/old-forum/8432.html) with good information.
* Here's one of Rocky's [blog posts](http://www.lhotka.net/weblog/PermissionbasedAuthorizationVsRolebasedAuthorization.aspx) on the topic.
