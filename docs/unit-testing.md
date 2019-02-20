## Unit Testing User Serialization Exception

When running your CSLA-based code in a unit test environment such as mstest or nunit you may encounter an unexpected serialization error in tests that set or use a custom security principal (e.g. you set `ApplicationContext.User` before or in the test).

This is a known issue that flows from the way these unit test frameworks use threads and .NET AppDomains. A single thread is used and runs in both the unit test host (like mstest) and in your test code. But the host and your tests are in different AppDomains, so when the thread flows between one and the other any context on the thread (such as the CurrentPrincipal) is serialized. This fails though, because the host doesn't have the same runtime context as your tests.

The solution is to make sure to implement a "cleanup" method in each test class. In that cleanup method set `ApplicationContext.User` to null. This way, after each test and before the thread returns to the mstest/nunit host the user value is cleared so .NET doesn't attempt to serialize your custom principal object between AppDomains.
