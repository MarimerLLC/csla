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
  public class BindingRefreshTests
  {

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestDoNotRefreshOnException()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson();
      using (var personForm = new PersonForm())
      {
        //allow read and but not write
        personRoot.AuthLevel = 1;
        personForm.BindUI(personRoot);
        personForm.bindingSourceRefresh1.RefreshOnException = false;
        personForm.Show();  // must show form to get databinding to work properly

        //ensure all text boxes equal business object
        Assert.AreEqual(personForm.firstNameTextBox.Text, personRoot.FirstName);

        try
        {
          personForm.firstNameTextBox.Text = "Dummy First Name Value";
        }
        catch (System.Security.SecurityException) { };
        
        Assert.AreNotEqual(personForm.firstNameTextBox.Text, personRoot.FirstName);
      }
    }


    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void TestRefreshOnException()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson();
      using (var personForm = new PersonForm())
      {
        //allow read and but not write
        personRoot.AuthLevel = 1;
        personForm.BindUI(personRoot);
        personForm.bindingSourceRefresh1.RefreshOnException = true;
        personForm.Show();  // must show form to get databinding to work properly
     
        //ensure text boxes equal business object
        Assert.AreEqual(personForm.firstNameTextBox.Text, personRoot.FirstName);

        try
        {
          personForm.firstNameTextBox.Text = "Dummy First Name Value";
        }
        catch (System.Security.SecurityException) { };

        Assert.AreEqual(personForm.firstNameTextBox.Text, personRoot.FirstName, "Values did not refresh");
      }
    }

    [TestMethod()]
    public void TestRefresh()
    {
      ApplicationContext.GlobalContext.Clear();

      var personRoot = EditablePerson.GetEditablePerson();
      using (var personForm = new PersonForm())
      {
        //allow read and write of all fields
        personRoot.AuthLevel = 2;
        personForm.BindUI(personRoot);
        personForm.Show();  // must show form to get databinding to work properly

        //ensure all text boxes equal business object
        Assert.AreEqual(personForm.firstNameTextBox.Text, personRoot.FirstName);
        Assert.AreEqual(personForm.lastNameTextBox.Text, personRoot.LastName);
        Assert.AreEqual(personForm.middleNameTextBox.Text, personRoot.MiddleName);
        Assert.AreEqual(personForm.placeOfBirthTextBox.Text, personRoot.PlaceOfBirth);

        personRoot.AuthLevel = 3;
        Assert.AreEqual(personForm.firstNameTextBox.Text, personRoot.FirstName, "Values did not refresh");
        Assert.AreEqual(personForm.lastNameTextBox.Text, personRoot.LastName, "Values did not refresh");
        Assert.AreEqual(personForm.middleNameTextBox.Text, personRoot.MiddleName, "Values did not refresh");
        Assert.AreEqual(personForm.placeOfBirthTextBox.Text, personRoot.PlaceOfBirth, "Values did not refresh");

      }
    }
  }
}