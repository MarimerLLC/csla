using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Csla.Test.RollBack
{
#warning "makes calls to the BeginEdit(), CancelEdit(), and ApplyEdit() methods, which seem to be causing problems in the C# library"
    [TestFixture]
    public class RollBackTests
    {
        [Test]
        public void NoFail()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot root = Csla.Test.RollBack.RollbackRoot.NewRoot();

            root.BeginEdit();
            root.Data = "saved";
            Assert.AreEqual("saved", root.Data, "data is 'saved'");
            Assert.AreEqual(false, root.Fail, "fail is false");
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
            Assert.AreEqual(true, root.IsValid, "isvalid is true");
            Assert.AreEqual(true, root.IsNew, "isnew is true");

            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot tmp = (RollbackRoot)(root.Clone());
            try
            {
                root.ApplyEdit();
                root = (RollbackRoot)(root.Save());
            }
            catch
            {
                root = tmp;
                Assert.Fail("exception occurred");
            }

            Assert.IsNotNull(root, "obj is not null");
            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"], "obj was inserted");
            Assert.AreEqual("saved", root.Data, "data is 'saved'");
            Assert.AreEqual(false, root.IsNew, "isnew is false");
            Assert.AreEqual(false, root.IsDeleted, "isdeleted is false");
            Assert.AreEqual(false, root.IsDirty, "isdirty is false");
        }

        [Test]
        public void YesFail()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot root = Csla.Test.RollBack.RollbackRoot.NewRoot();

            root.BeginEdit();
            root.Data = "saved";
            root.Fail = true;
            Assert.AreEqual("saved", root.Data, "data is 'saved'");
            Assert.AreEqual(true, root.Fail, "fail is true");
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
            Assert.AreEqual(true, root.IsValid, "isvalid is true");
            Assert.AreEqual(true, root.IsNew, "isnew is true");

            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot tmp = (RollbackRoot)(root.Clone());
            try
            {
                root.ApplyEdit();
                root = (RollbackRoot)(root.Save());
                Assert.Fail("exception didn't occur");
            }
            catch
            {
                root = tmp;
            }

            Assert.IsNotNull(root, "obj is not null");
            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"], "obj was inserted");
            Assert.AreEqual("saved", root.Data, "data is 'saved'");
            Assert.AreEqual(true, root.IsNew, "isnew is true");
            Assert.AreEqual(false, root.IsDeleted, "isdeleted is false");
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
        }

        [Test]
        public void YesFailCancel()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot root = Csla.Test.RollBack.RollbackRoot.NewRoot();
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
            Assert.AreEqual("<new>", root.Data, "data is '<new>'");

            root.BeginEdit();
            root.Data = "saved";
            root.Fail = true;
            Assert.AreEqual("saved", root.Data, "data is 'saved'");
            Assert.AreEqual(true, root.Fail, "fail is true");
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
            Assert.AreEqual(true, root.IsValid, "isvalid is true");
            Assert.AreEqual(true, root.IsNew, "isnew is true");

            Csla.ApplicationContext.GlobalContext.Clear();
            RollbackRoot tmp = (RollbackRoot)(root.Clone());
            try
            {
                root.ApplyEdit();
                root = (RollbackRoot)(root.Save());
                Assert.Fail("exception didn't occur");
            }
            catch
            {
                root = tmp;
                root.CancelEdit();
            }

            Assert.IsNotNull(root, "obj is not null");
            Assert.AreEqual("Inserted", Csla.ApplicationContext.GlobalContext["Root"], "obj was inserted");
            Assert.AreEqual("<new>", root.Data, "data is '<new>'");
            Assert.AreEqual(true, root.IsNew, "isnew is true");
            Assert.AreEqual(false, root.IsDeleted, "isdeleted is false");
            Assert.AreEqual(true, root.IsDirty, "isdirty is true");
        }
    }
}
