## Does CSLA .NET support report generation?
Business objects created using CSLA .NET are bindable, and therefore can sometimes be used directly from report generation tools.

However, I generally divide "reports" into two categories:

1. "Lists", which are generated using small amounts of input, and may result in small or large amounts of output 
2. "Reports", which are generated using large amounts of input, and may result in small or large amounts of output 

Lists can be generated from business objects or other in-memory data, or from data in a database. Since the amount of data required to generate the list is small it doesn't matter a lot where the data comes from.

Reports can only be generated directly from the database by using a report generation tool. The overhead of retrieving large amounts of data into objects and then generating the report is simply unworkable.

Most people, in my observation, treat everything like a report. If they have lists, they'll often generate the output using Word or Excel exports.

[This thread](https://cslanet.com/old-forum/9933.html) has more information.
