using System;

namespace NavigationApp
{
  public class BoomarkInformation
  {
    private string _controlTypeName;
    private string _parameters;
    private string _title;
    public BoomarkInformation(string controlTypeName, string parameters, string title)
    {
      _controlTypeName = controlTypeName;
      _parameters = parameters;
      _title = title;
    }

    public string ControlTypeName
    {
      get { return _controlTypeName; }
    }

    public string Parameters
    {
      get { return _parameters; }
    }

    public string Title
    {
      get { return _title; }
    }

    public override bool Equals(object obj)
    {
      if (obj is BoomarkInformation)
      {
        BoomarkInformation compareTo = obj as BoomarkInformation;
        if (compareTo.ControlTypeName == this.ControlTypeName &&
            compareTo.Parameters == this.Parameters)
          return true;
        else
          return false;
      }
      else
        return false;
    }
  }
}
