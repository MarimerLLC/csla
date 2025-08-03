//-----------------------------------------------------------------------
// <copyright file="ContextBOVerifier.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;


namespace Csla.Testing.Business.ApplicationContext
{
  [Serializable]
  public abstract class ContextBOVerifier<T> : BusinessBase<T> where T : ContextBOVerifier<T> 
  {
    static int _dummy = 0;
    public ContextBOVerifier() :this(true){}
    public ContextBOVerifier(bool isNew)
    {

      if (isNew)
        MarkNew();
      else
        MarkOld();

      _dummy = _dummy + 0;
    }

    protected override void OnDeserialized(StreamingContext context)
    {
      _dummy = _dummy + 0;
      base.OnDeserialized(context);
    }

    private static PropertyInfo<string> NameProperty =
      RegisterProperty(new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected static PropertyInfo<string> ReceivedContextValueProperty = 
      RegisterProperty(new PropertyInfo<string>("ReceivedContextValue", "ReceivedContextValue")); 
    public string ReceivedContextValue
    {
      get { return GetProperty(ReceivedContextValueProperty); }
      set { SetProperty(ReceivedContextValueProperty, value); }
    }

    #region Data Access

    //Defined in inherited classes - .server for .Net and .client for Silverlight implementations
    protected void SetReceivedContextValuePropertyFrom(Contexts selectedContext)
    {
      if(selectedContext == Contexts.Client)
        LoadProperty(ReceivedContextValueProperty, (string)Csla.ApplicationContext.ClientContext["MSG"]);
      else
#pragma warning disable CS0618 // Type or member is obsolete
        LoadProperty(ReceivedContextValueProperty, (string)Csla.ApplicationContext.GlobalContext["MSG"]);
#pragma warning restore CS0618 // Type or member is obsolete

    }

    protected void SetContextValueModified(Contexts selectedContext)
    {
      if (selectedContext == Contexts.Client)
        Csla.ApplicationContext.ClientContext["MSG"] = ContextMessageValues.MODIFIED_VALUE;
      else
#pragma warning disable CS0618 // Type or member is obsolete
        Csla.ApplicationContext.GlobalContext["MSG"] = ContextMessageValues.MODIFIED_VALUE;
#pragma warning restore CS0618 // Type or member is obsolete
    }
    #endregion
  }

  public class ContextMessageValues
  {
    public const string INITIAL_VALUE = "Hello World!";
    public const string MODIFIED_VALUE = "Changed";
  }

  public enum Contexts
  {
    Client,
    Global
  }
}