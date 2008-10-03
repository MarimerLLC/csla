using System;

namespace Csla.Silverlight
{
  /// <summary>
  /// This class is used to keep track of bookmarks within Navigator.
  /// </summary>
  public class BoomarkInformation
  {
    private string _controlTypeName;
    private string _parameters;
    private string _title;

    /// <summary>
    /// New instance of bookmark.
    /// </summary>
    /// <param name="controlTypeName">
    /// Assembly qualified name of the control class to display.
    /// </param>
    /// <param name="parameters">
    /// Parameters that will be passed to control.  Single string is passed
    /// in, and controls assumes responcibility for parsing it.
    /// </param>
    /// <param name="title">
    /// Title of the bookmark.
    /// </param>
    public BoomarkInformation(string controlTypeName, string parameters, string title)
    {
      _controlTypeName = controlTypeName;
      _parameters = parameters;
      _title = title;
    }

    /// <summary>
    /// Assembly qualified name of the control class to display.
    /// </summary>
    public string ControlTypeName
    {
      get { return _controlTypeName; }
    }

    /// <summary>
    /// Parameters that will be passed to control.  Single string is passed
    /// in, and controls assumes responcibility for parsing it.
    /// </summary>
    public string Parameters
    {
      get { return _parameters; }
    }

    /// <summary>
    /// Title of the bookmark.
    /// </summary>
    public string Title
    {
      get { return _title; }
    }

    /// <summary>
    /// Determines if two bookmark information objects are equal
    /// based on the inforamtion they contain
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
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
