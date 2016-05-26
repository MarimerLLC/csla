namespace Csla.Analyzers
{
  public static class DotNetMemberConstants
  {
    public static class Namespaces
    {
      public const string ISerializable = "System.Runtime.Serialization";
    }

    public static class Types
    {
      public const string ISerializable = 
        nameof(DotNetMemberConstants.Types.ISerializable);
    }
  }
}
