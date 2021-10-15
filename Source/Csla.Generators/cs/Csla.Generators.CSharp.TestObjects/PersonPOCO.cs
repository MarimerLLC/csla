//-----------------------------------------------------------------------
// <copyright file="PersonPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Complex class that can be used for testing serialization behaviour</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;

namespace Csla.Generators.CSharp.TestObjects
{

  /// <summary>
  /// A class for which automatic serialization code is to be generated
  /// Includes children of various types to support multiple testing scenarios
  /// </summary>
  /// <remarks>The class is decorated with the AutoSerializable attribute so that it is picked up by our source generator</remarks>
  [AutoSerializable]
  public partial class PersonPOCO
  {

#pragma warning disable CS0414 // Remove unused private members
    private string _fieldTest = "foo";
#pragma warning restore CS0414 // Remove unused private members
    private string _lastName;

    [AutoSerialized]
    private string _middleName;

    public int PersonId { get; set; }

    public string FirstName { get; set; }

    public string MiddleName => _middleName;

    public void SetMiddleName(string middleName)
    {
      _middleName = middleName;
    }

    public string LastName
    {
      get { return _lastName; }
      set { _lastName = value; }
    }

    [AutoNonSerialized]
    public string NonSerializedText { get; set; } = string.Empty;

    [AutoSerialized]
    private string PrivateSerializedText { get; set; } = string.Empty;

    public string GetPrivateSerializedText()
    {
      return PrivateSerializedText;
    }

    public void SetPrivateSerializedText(string newValue)
    {
      PrivateSerializedText = newValue;
    }

    private string PrivateText { get; set; } = string.Empty;

    public string GetUnderlyingPrivateText()
    {
      return PrivateText;
    }

    public void SetUnderlyingPrivateText(string value)
    {
      PrivateText = value;
    }

    internal DateTime DateOfBirth { get; set; }

    public void SetDateOfBirth(DateTime newDateOfBirth)
    {
      DateOfBirth = newDateOfBirth;
    }

    public DateTime GetDateOfBirth()
    {
      return DateOfBirth;
    }

    public AddressPOCO Address { get; set; }

    public EmailAddress EmailAddress { get; set; }

  }

}
