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
  public class ActionExtenderTests
  {

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestActionExtenderDefaultButtonState()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.NewEditablePerson();
      using (var personForm = new PersonForm())
      {
        //allow read and write
        personRoot.AuthLevel = 2;
        personForm.BindUI(personRoot,true);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.SaveButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.CancelButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.CloseButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.ValidateButton.Enabled, true, "Default button state on new not correct");
      }

      personRoot = EditablePerson.GetEditablePerson(2);
      using (var personForm = new PersonForm())
      {
        personForm.BindUI(personRoot,true);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.SaveButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.CancelButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.CloseButton.Enabled, true, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.ValidateButton.Enabled, true, "Default button state on fetch not correct");
      }
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestActionExtenderChangeState()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson(2);
      using (var personForm = new PersonForm())
      {
        personForm.BindUI(personRoot,true);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.SaveButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.CancelButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.CloseButton.Enabled, true, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.ValidateButton.Enabled, true, "Default button state on fetch not correct");

        personForm.firstNameTextBox.Text = "";

        Assert.AreEqual(personForm.SaveButton.Enabled, false, "Save button did not become disabled");
        Assert.AreEqual(personForm.CancelButton.Enabled, true, "Cancel button is not enabled");
        Assert.AreEqual(personForm.CloseButton.Enabled, true, "Button state is not correct");
        Assert.AreEqual(personForm.ValidateButton.Enabled, true, "Button state is not correct");

        personForm.firstNameTextBox.Text = "ABC";

        Assert.AreEqual(personForm.SaveButton.Enabled, true, "Save button did not become enabled");
        Assert.AreEqual(personForm.CancelButton.Enabled, true, "Cancel button is not enabled");
        Assert.AreEqual(personForm.CloseButton.Enabled, true, "Button state is not correct");
        Assert.AreEqual(personForm.ValidateButton.Enabled, true, "Button state is not correct");
      }
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestActionExtenderToolStripDefaultButtonState()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.NewEditablePerson();
      using (var personForm = new PersonForm())
      {
        //allow read and write
        personRoot.AuthLevel = 2;
        personForm.BindUI(personRoot,false);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.saveToolStripButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.cancelToolStripButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.closeToolStripButton.Enabled, true, "Default button state on new not correct");
        Assert.AreEqual(personForm.validateToolStripButton.Enabled, true, "Default button state on new not correct");
      }

      personRoot = EditablePerson.GetEditablePerson(2);
      using (var personForm = new PersonForm())
      {
        personForm.BindUI(personRoot,false);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.saveToolStripButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.cancelToolStripButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.closeToolStripButton.Enabled, true, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.validateToolStripButton.Enabled, true, "Default button state on fetch not correct");
      }
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestActionExtenderToolStripChangeState()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson(2);
      using (var personForm = new PersonForm())
      {
        personForm.BindUI(personRoot,false);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure button state matches logical status
        Assert.AreEqual(personForm.saveToolStripButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.cancelToolStripButton.Enabled, false, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.closeToolStripButton.Enabled, true, "Default button state on fetch not correct");
        Assert.AreEqual(personForm.validateToolStripButton.Enabled, true, "Default button state on fetch not correct");

        personForm.firstNameTextBox.Text = "";

        Assert.AreEqual(personForm.saveToolStripButton.Enabled, false, "Save button did not become disabled");
        Assert.AreEqual(personForm.cancelToolStripButton.Enabled, true, "Cancel button is not enabled");
        Assert.AreEqual(personForm.closeToolStripButton.Enabled, true, "Button state is not correct");
        Assert.AreEqual(personForm.validateToolStripButton.Enabled, true, "Button state is not correct");

        personForm.firstNameTextBox.Text = "ABC";

        Assert.AreEqual(personForm.saveToolStripButton.Enabled, true, "Save button did not become enabled");
        Assert.AreEqual(personForm.cancelToolStripButton.Enabled, true, "Cancel button is not enabled");
        Assert.AreEqual(personForm.closeToolStripButton.Enabled, true, "Button state is not correct");
        Assert.AreEqual(personForm.validateToolStripButton.Enabled, true, "Button state is not correct");
      }
    }

  }
}