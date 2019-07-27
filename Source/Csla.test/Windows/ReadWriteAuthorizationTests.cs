//-----------------------------------------------------------------------
// <copyright file="ReadWriteAuthorizationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Test.Security;
using UnitDriven;
using System.Diagnostics;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Windows
{
#if TESTING
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
#endif
  [TestClass()]
  public class ReadWriteAuthorizationTests
  {

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestReadWrite()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson();
      using (var personForm = new PersonForm())
      {


        Assert.IsFalse(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.firstNameTextBox));
        Assert.IsFalse(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.middleNameTextBox));
        Assert.IsFalse(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.lastNameTextBox));

        personForm.readWriteAuthorization1.SetApplyAuthorization(personForm.firstNameTextBox, true);
        personForm.readWriteAuthorization1.SetApplyAuthorization(personForm.middleNameTextBox, true);
        personForm.readWriteAuthorization1.SetApplyAuthorization(personForm.lastNameTextBox, true);

        Assert.IsTrue(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.firstNameTextBox),
                      "ApplyAuth on");
        Assert.IsTrue(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.middleNameTextBox),
                      "ApplyAuth on");
        Assert.IsTrue(personForm.readWriteAuthorization1.GetApplyAuthorization(personForm.lastNameTextBox),
                      "ApplyAuth on");

        // assert that controls are available
        Assert.IsFalse(personForm.firstNameTextBox.ReadOnly, "firstname initial readonly false");

        // read and write of FirstName is not allowed
        personRoot.AuthLevel = 0;
        personForm.BindUI(personRoot);
        personForm.Show();  // must show form to get databinding to work properly

        // RWA shoul now set ReadOnly to true and no textvalue
        Assert.IsFalse(personRoot.CanReadProperty(EditablePerson.FirstNameProperty.Name));
        Assert.IsFalse(personRoot.CanWriteProperty(EditablePerson.FirstNameProperty.Name));

        Assert.IsTrue(personForm.firstNameTextBox.ReadOnly, "Control set to readonly");
        Assert.IsTrue(String.IsNullOrEmpty(personForm.firstNameTextBox.Text), "Not allowed to get value");

        // now allowed to get value but not set value
        personRoot.AuthLevel = 1;
        Assert.IsTrue(personRoot.CanReadProperty(EditablePerson.FirstNameProperty.Name));
        Assert.IsFalse(personRoot.CanWriteProperty(EditablePerson.FirstNameProperty.Name));
        Assert.IsTrue(personForm.firstNameTextBox.ReadOnly, "Control set to readonly");
        Assert.AreEqual(personRoot.FirstName, personForm.firstNameTextBox.Text, "display text value from BO");

        personRoot.AuthLevel = 2;
        Assert.IsTrue(personRoot.CanReadProperty(EditablePerson.FirstNameProperty.Name));
        Assert.IsTrue(personRoot.CanWriteProperty(EditablePerson.FirstNameProperty.Name));
        Assert.IsFalse(personForm.firstNameTextBox.ReadOnly, "Control no longer readonly");
        Assert.AreEqual(personRoot.FirstName, personForm.firstNameTextBox.Text, "display text value from BO");
      }
    }
  }
}