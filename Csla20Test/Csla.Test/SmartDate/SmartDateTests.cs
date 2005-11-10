using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.SmartDate
{
    [TestFixture]
    public class SmartDateTests
    {
        [Test]
        public void TestSmartDate()
        {
            DateTime now = DateTime.Now;
            Csla.SmartDate d = new Csla.SmartDate(now, true);
            Assert.AreEqual(now, d.Date);
        }

        //implement remaining

    }
}
