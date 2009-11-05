using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

//using MethodCallervb;
using MethodCallercs;

namespace MethodCallerTest
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void PrintResult()
    {
      this.resultTextBox.AppendText(Environment.NewLine);
    }

    private void PrintResult(string text)
    {
      this.resultTextBox.AppendText(text);
      this.resultTextBox.AppendText(Environment.NewLine);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      MethodCaller.CallMethod(this, "NoParams");
      PrintResult();
      PrintResult(string.Format(
        "  NoParamsIntResult() = {0}", MethodCaller.CallMethod(this, "NoParamsIntResult").ToString()));
      PrintResult("  (should be 123)");
      PrintResult();
      PrintResult(string.Format(
        "  IntParamIntResult() = {0}", MethodCaller.CallMethod(this, "IntParamIntResult", 42).ToString()));
      PrintResult("  (should be 42)");
      PrintResult();
      PrintResult(string.Format(
        "  StringParamStringResult() = {0}", MethodCaller.CallMethod(this, "StringParamStringResult", "abc").ToString()));
      PrintResult("  (should be abc)");
      PrintResult();
      MethodCaller.CallMethod(this, "ParamArrayParam", "a", "b", "c", "d");
      PrintResult("  (should be 4)");
      PrintResult();
      MethodCaller.CallMethod(this, "ParamArrayParam", null);
      PrintResult("  (should be 1)");
      PrintResult();
      MethodCaller.CallMethod(this, "FourStringParam", "a", "b", "c", "d");
      PrintResult();

      try
      {
        MethodCaller.CallMethod(this, "NoSuchMethod");
      }
      catch (Exception ex)
      {
        PrintResult("NoSuchMethod() failed as expected");
        PrintResult(string.Format("  ({0}) {1}", ex.GetType().Name, ex.Message));
        PrintResult();
      }

      try
      {
        MethodCaller.CallMethod(this, "ThrowsException");
      }
      catch (Exception ex)
      {
        PrintResult("ThrowsException() failed as expected");
        PrintResult(string.Format("  ({0}) {1}", ex.GetType().Name, ex.Message));
        PrintResult(string.Format("  ({0}) {1}", ex.InnerException.GetType().Name, ex.InnerException.Message));
        PrintResult();
      }

      MethodCaller.CallMethod(this, "SingleArrayParam", new object[] { 1, "a", 42, "abc" });
      MethodCaller.CallMethod(this, "SingleArrayParam", new object[] { 1, "a", 42, "abc" });
      PrintResult();

      MethodCaller.CallMethod(this, "SingleStringArrayParam", new string[] { "1", "a", "42", "abc" });
      MethodCaller.CallMethod(this, "SingleStringArrayParam", new string[] { "1", "a", "42", "abc" });
      PrintResult();

      int iterations = 100000;
      PrintResult("Beginning performance testing...");
      DateTime start = DateTime.Now;
      for (int count = 0; count < iterations; count++)
        MethodCaller.CallMethod(this, "PerfTestNoParam");
      DateTime end = DateTime.Now;
      PrintResult(string.Format("NoParam elapsed: {0}", (end - start).TotalMilliseconds));
      start = DateTime.Now;
      for (int count = 0; count < iterations; count++)
        MethodCaller.CallMethod(this, "PerfTestIntParam", 123);
      end = DateTime.Now;
      PrintResult(string.Format("IntParam elapsed: {0}", (end - start).TotalMilliseconds));
      start = DateTime.Now;
      for (int count = 0; count < iterations; count++)
        MethodCaller.CallMethod(this, "PerfTestStringParam", "abc123");
      end = DateTime.Now;
      PrintResult(string.Format("StringParam elapsed: {0}", (end - start).TotalMilliseconds));
      PrintResult();
    }

    private void NoParams()
    {
      PrintResult("NoParams() ok");
    }

    private int NoParamsIntResult()
    {
      PrintResult("NoParamsIntResult() ok");
      return 123;
    }

    private int IntParamIntResult(int test)
    {
      PrintResult("IntParamIntResult() ok");
      return test;
    }

    private string StringParamStringResult(string test)
    {
      PrintResult("StringParamStringResult() ok");
      return test;
    }

    private void ParamArrayParam(params object[] test)
    {
      PrintResult("ParamArrayParam() ok");
      if (test != null)
        PrintResult(string.Format("  Array length {0}", test.Length.ToString()));
      else
        PrintResult(string.Format("  Array length <null>"));
    }

    private void FourStringParam(string a, string b, string c, string d)
    {
      PrintResult("FourStringParam() ok");
    }

    private void ThrowsException()
    {
      throw new NotImplementedException("Exception from ThrowsException");
    }

    private void SingleArrayParam(object[] test)
    {
      PrintResult("SingleArrayParam() ok");
      if (test != null)
        PrintResult(string.Format("  Array length {0}", test.Length.ToString()));
      else
        PrintResult(string.Format("  Array length <null>"));
    }

    private void SingleStringArrayParam(string[] test)
    {
      PrintResult("SingleStringArrayParam() ok");
      if (test != null)
        PrintResult(string.Format("  Array length {0}", test.Length.ToString()));
      else
        PrintResult(string.Format("  Array length <null>"));
    }

    private void PerfTestNoParam()
    {
    }

    private void PerfTestIntParam(int test)
    {
    }

    private void PerfTestStringParam(string test)
    {
    }
  }
}
