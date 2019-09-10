using System;
using System.Threading;
using Csla.Core;
using Csla.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class IntegrationRuleTest<T> : ObjectFactory where T : Csla.BusinessBase<T>
{
  private T _businessObject;
  public EventWaitHandle RulesCompletedWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

  public T BusinessObject
  {
    get { return _businessObject; }
    set
    {
      if (_businessObject != null)
        _businessObject.ValidationComplete -= OnBusinessObjectOnValidationComplete;
      _businessObject = value;
      if (_businessObject != null)
        _businessObject.ValidationComplete += OnBusinessObjectOnValidationComplete;
    }
  }

  [TestInitialize]
  public void Initialize()
  {
    BusinessObject = Activator.CreateInstance<T>();
  }

  [ClassCleanup]
  public void Cleanup()
  {
    BusinessObject = null;
  }

  private void OnBusinessObjectOnValidationComplete(object sender, EventArgs args)
  {
    if (RulesCompletedWaitHandle != null)
      RulesCompletedWaitHandle.Set();
  }

  public void CheckRules()
  {
    CheckRules(1000);
  }

  public void CheckRules(int maxMillisecondsToWait)
  {
    RulesCompletedWaitHandle.Reset();
    ((ICheckRules)BusinessObject).CheckRules();
    RulesCompletedWaitHandle.WaitOne(maxMillisecondsToWait);
  }
}
