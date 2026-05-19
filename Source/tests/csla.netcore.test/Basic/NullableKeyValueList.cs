namespace Csla.Test.Basic;

/// <summary>
/// Example that a nullable key is now possible for <see cref="NameValueListBase{K,V}"/>.
/// </summary>
public class NullableKeyValueList : NameValueListBase<int?, string>
{
  [Fetch]
  private void Fetch()
  {
    // This class must be compilable.
  }
}