# Unit Testing

## Unit Testing Business Domain Classes

1. Unit testing with CSLA was difficult until CSLA 4
1. Integration testing (with mock data) was easy and recommended prior to CSLA 4 (and really isn’t a bad solution even today, though unit testing is very practical now)
1. A number of the enhancements in CSLA 5 will simplify unit testing scenarios even further
1. CSLA 6 requires and fully embraces the use of dependency injection. As a result, unit testing is now a lot easier.

@jasonbock has written a nice blog post on how to use some of the features of CSLA 4 that support unit testing and mocking:

* [Abstractions in CSLA 4](Abstractions-in-CSLA.md)

Please note that CSLA 5 has support for dependency injection, and CSLA 6 and later _require_ the use of DI. This means that abstractions and unit testing in CSLA 5 and later is easier than what is described in the CSLA 4 blog post.

## Unit Testing User Serialization Exception

When running your CSLA-based code in a unit test environment such as mstest or nunit you may encounter an unexpected serialization error in tests that set or use a custom security principal (e.g. you set `ApplicationContext.User` before or in the test).

This is a known issue that flows from the way these unit test frameworks use threads and .NET AppDomains. A single thread is used and runs in both the unit test host (like mstest) and in your test code. But the host and your tests are in different AppDomains, so when the thread flows between one and the other any context on the thread (such as the CurrentPrincipal) is serialized. This fails though, because the host doesn't have the same runtime context as your tests.

The solution is to make sure to implement a "cleanup" method in each test class. In that cleanup method set `ApplicationContext.User` to null. This way, after each test and before the thread returns to the mstest/nunit host the user value is cleared so .NET doesn't attempt to serialize your custom principal object between AppDomains.
