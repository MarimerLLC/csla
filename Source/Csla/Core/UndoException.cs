//-----------------------------------------------------------------------
// <copyright file="UndoException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exception indicating a problem with the</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  /// <summary>
  /// Exception indicating a problem with the
  /// use of the n-level undo feature in
  /// CSLA .NET.
  /// </summary>
  /// <remarks></remarks>
  [Serializable]
  public class UndoException : Exception
  {
    /// <summary>
    /// The type name of the object that caused this issue
    /// </summary>
    public string TypeName { get; } = "";
    /// <summary>
    /// The parent's type name of the object that caused this issue or null
    /// </summary>
    public string? ParentTypeName { get; }
    /// <summary>
    /// Object EditLevel
    /// </summary>
    public int CurrentEditLevel { get; }
    /// <summary>
    /// Expected object EditLevel
    /// </summary>
    public int ExpectedEditLevel { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="typeName">The type name of the object that caused this issue</param>
    /// <param name="parentTypeName">The parent's type name of the object that caused this issue or null</param>
    /// <param name="currentEditLevel">Object EditLevel</param>
    /// <param name="expectedEditLevel">Expected object EditLevel</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="typeName"/> is <see langword="null"/>.</exception>
    public UndoException(string message, string typeName, string? parentTypeName, int currentEditLevel, int expectedEditLevel)
      : base(message)
    {
      if (message is null)
        throw new ArgumentNullException(nameof(message));
      TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
      ParentTypeName = parentTypeName;
      CurrentEditLevel = currentEditLevel;
      ExpectedEditLevel = expectedEditLevel;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Text describing the exception.</param>
    /// <param name="ex">Inner exception.</param>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="ex"/> is <see langword="null"/>.</exception>
    public UndoException(string message, Exception ex)
      : base(message, ex)
    {
      if (message is null)
        throw new ArgumentNullException(nameof(message));
      if (ex is null)
        throw new ArgumentNullException(nameof(ex));
    }
  }
}