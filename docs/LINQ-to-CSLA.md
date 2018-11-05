## When is it appropriate or inappropriate to use LINQ to CSLA indexing?
The indexing feature of LINQ to CSLA allows for amazingly fast queries against a collection. However, it does require that an index be constructed and maintained, and that is not free. This is discussed in the _Expert 2008 Business Objects_ book, but is a topic worth reinforcing.

If you are going to do numerous queries over the same instance of a collection, then it is worth using indexing. If you are going to do just a few queries over an instance of a collection, the cost of building the index might be higher than the performance savings on the queries.

Here are some forum posts with some thoughts about indexing in a web setting.

https://cslanet.com/old-forum/11353.html

https://cslanet.com/old-forum/7425.html

https://cslanet.com/old-forum/6507.html
