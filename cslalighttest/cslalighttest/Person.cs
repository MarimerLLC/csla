using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;

namespace Example.Business
{
  [Serializable]
  public partial class Person : BusinessBase<Person>
  {
    #region Serialization

    //protected override object GetValue(System.Reflection.FieldInfo field)
    //{
    //  if (field.DeclaringType == typeof(Person))
    //    return field.GetValue(this);
    //  else
    //    return base.GetValue(field);
    //}

    //protected override void SetValue(System.Reflection.FieldInfo field, object value)
    //{
    //  if (field.DeclaringType == typeof(Person))
    //    field.SetValue(this, value);
    //  else
    //    base.SetValue(field, value);
    //}

    #endregion

    public string Name { get; set; }

    private DateTime _birthdate;

    public int Age
    {
      get { return (DateTime.Now - _birthdate).Days / 365; }
      set { _birthdate = DateTime.Now - new TimeSpan(value * 365, 0, 0, 0); }
    }

    [NonSerialized]
    private string mUnserialized = "";
    public string Unserialized
    {
      get { return mUnserialized; }
      set { mUnserialized = value; }
    }

    private AddressList _addresses = new AddressList();

    public AddressList Addresses 
    {
      get { return _addresses; }
      set { _addresses = value; }
    }

    public DateTime GetBDate()
    {
      Type t = this.GetType();
      var f = t.GetField("_birthdate", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
      return (DateTime)f.GetValue(this);
    }

    public Address PrimaryAddress { get; set; }

    public override bool Equals(object theOtherPerson)
    {
      Person myOtherPerson = theOtherPerson as Person;
      if (myOtherPerson == null)
        return false;
      if (myOtherPerson.Name != this.Name)
        return false;
      if (myOtherPerson.Age != this.Age)
        return false;
      if (!this.Addresses.Equals(myOtherPerson.Addresses))
        return false;
      if ((myOtherPerson.PrimaryAddress == null) != (this.PrimaryAddress == null))
        return false;  // not same state of nullity
      if (this.PrimaryAddress != null && !this.PrimaryAddress.Equals(myOtherPerson.PrimaryAddress))
        return false;
      return this.Addresses.Equals(myOtherPerson.Addresses);
    }

    public override int GetHashCode()
    {
      return (this.Name + this.Age.ToString()).GetHashCode();
    }

    //[Serializable]
    //public class Criteria : CriteriaBase
    //{
    //  #region Serialization

    //  protected override object GetValue(System.Reflection.FieldInfo field)
    //  {
    //    if (field.DeclaringType == typeof(Criteria))
    //      return field.GetValue(this);
    //    else
    //      return base.GetValue(field);
    //  }

    //  protected override void SetValue(System.Reflection.FieldInfo field, object value)
    //  {
    //    if (field.DeclaringType == typeof(Criteria))
    //      field.SetValue(this, value);
    //    else
    //      base.SetValue(field, value);
    //  }

    //  #endregion

    //  public string Name { get; set; }

    //  private Criteria()
    //  { }

    //  public Criteria(string name)
    //    : base(typeof(Person))
    //  {
    //    this.Name = name;
    //  }
    //}

  }
}
