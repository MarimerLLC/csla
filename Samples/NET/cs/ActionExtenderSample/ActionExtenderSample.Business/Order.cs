//-----------------------------------------------------------------------
// <copyright file="Order.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
// <remarks>Generated file.</remarks>
//-----------------------------------------------------------------------
using ActionExtenderSample.Business;

namespace ActionExtenderSample.Business
{
  public partial class Order
  {

    #region OnDeserialized actions

    /*/// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
        base.OnDeserialized(context);
        // add your custom OnDeserialized actions here.
    }*/

    #endregion

    #region Custom Business Rules and Property Authorization

    //partial void AddBusinessRulesExtend()
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    #endregion

    #region Pseudo Event Handlers

    //partial void OnCreate(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnDeletePre(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnDeletePost(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnFetchPre(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnFetchPost(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnFetchRead(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnUpdatePre(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnUpdatePost(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnInsertPre(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    //partial void OnInsertPost(DataPortalHookArgs args)
    //{
    //    throw new System.Exception("The method or operation is not implemented.");
    //}

    #endregion

  }
}
