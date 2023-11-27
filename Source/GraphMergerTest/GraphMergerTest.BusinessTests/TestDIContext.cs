using System;

namespace GraphMergerTest.BusinessTests
{
    public class TestDIContext
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public TestDIContext(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }
    }
}
