//-----------------------------------------------------------------------
// <copyright file="RangeIndexPolyfill.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Polyfill implementation of Range and Index for target frameworks where they are unavailable</summary>
//-----------------------------------------------------------------------
#if NET462
namespace System;

/// <summary>
/// Polyfill implementation of <see cref="Index"/> for target frameworks where it is unavailable.
/// </summary>
public readonly struct Index : IEquatable<Index>
{
  private readonly int _value;

  /// <summary>
  /// Initializes a new <see cref="Index"/> from the specified start-based position.
  /// </summary>
  /// <param name="value">Zero-based position counted from the start of the collection.</param>
  public Index(int value)
    : this(value, fromEnd: false)
  { }

  /// <summary>
  /// Initializes a new <see cref="Index"/> using the supplied value and origin.
  /// </summary>
  /// <param name="value">The position relative to the chosen origin.</param>
  /// <param name="fromEnd">True to interpret <paramref name="value"/> from the end of the collection.</param>
  public Index(int value, bool fromEnd)
  {
    if (value < 0)
      throw new ArgumentOutOfRangeException(nameof(value));

    _value = fromEnd ? ~value : value;
  }

  /// <summary>
  /// Gets a value indicating whether this index counts from the end of the collection.
  /// </summary>
  public bool IsFromEnd => _value < 0;

  /// <summary>
  /// Gets the zero-based index relative to the start of the collection.
  /// </summary>
  public int Value => IsFromEnd ? ~_value : _value;

  /// <summary>
  /// Gets an index that identifies the first element of the collection.
  /// </summary>
  public static Index Start => new(0, fromEnd: false);

  /// <summary>
  /// Gets an index that represents the position beyond the last element of the collection.
  /// </summary>
  public static Index End => new(0, fromEnd: true);

  /// <summary>
  /// Converts a start-based position to an <see cref="Index"/>.
  /// </summary>
  /// <param name="value">Zero-based position counted from the start.</param>
  public static implicit operator Index(int value) => FromStart(value);

  /// <summary>
  /// Creates an index counted from the start of the collection.
  /// </summary>
  /// <param name="value">Zero-based position counted from the start.</param>
  /// <returns>A new <see cref="Index"/>.</returns>
  public static Index FromStart(int value)
  {
    if (value < 0)
      throw new ArgumentOutOfRangeException(nameof(value));

    return new Index(value, fromEnd: false);
  }

  /// <summary>
  /// Creates an index counted from the end of the collection.
  /// </summary>
  /// <param name="value">Zero-based position counted from the end.</param>
  /// <returns>A new <see cref="Index"/>.</returns>
  public static Index FromEnd(int value)
  {
    if (value < 0)
      throw new ArgumentOutOfRangeException(nameof(value));

    return new Index(value, fromEnd: true);
  }

  /// <summary>
  /// Calculates the absolute offset represented by this index for the supplied collection length.
  /// </summary>
  /// <param name="length">Length of the target collection.</param>
  /// <returns>The calculated offset.</returns>
  public int GetOffset(int length)
  {
    var offset = IsFromEnd ? length - Value : Value;
    if ((uint)offset > (uint)length)
      throw new ArgumentOutOfRangeException(nameof(length));

    return offset;
  }

  /// <inheritdoc />
  public override bool Equals(object? value) => value is Index index && Equals(index);

  /// <inheritdoc />
  public bool Equals(Index other) => _value == other._value;

  /// <inheritdoc />
  public override int GetHashCode() => _value;

  /// <inheritdoc />
  public override string ToString() => IsFromEnd
    ? "^" + Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture)
    : Value.ToString(global::System.Globalization.CultureInfo.InvariantCulture);
}

/// <summary>
/// Polyfill implementation of <see cref="Range"/> for target frameworks where it is unavailable.
/// </summary>
public readonly struct Range : IEquatable<Range>
{
  /// <summary>
  /// Initializes a new <see cref="Range"/> with the provided start and end indexes.
  /// </summary>
  /// <param name="start">Inclusive start index.</param>
  /// <param name="end">Exclusive end index.</param>
  public Range(Index start, Index end)
  {
    Start = start;
    End = end;
  }

  /// <summary>
  /// Gets the inclusive start index of the range.
  /// </summary>
  public Index Start { get; }

  /// <summary>
  /// Gets the exclusive end index of the range.
  /// </summary>
  public Index End { get; }

  /// <summary>
  /// Creates a range that begins at the specified index and continues to the end of the sequence.
  /// </summary>
  /// <param name="start">Inclusive start index.</param>
  /// <returns>A <see cref="Range"/> defined from <paramref name="start"/> to the end.</returns>
  public static Range StartAt(Index start) => new(start, Index.End);

  /// <summary>
  /// Creates a range that starts at the beginning of the sequence and ends before the specified index.
  /// </summary>
  /// <param name="end">Exclusive end index.</param>
  /// <returns>A <see cref="Range"/> defined from the start to <paramref name="end"/>.</returns>
  public static Range EndAt(Index end) => new(Index.Start, end);

  /// <summary>
  /// Gets a range that covers an entire sequence.
  /// </summary>
  public static Range All => new(Index.Start, Index.End);

  /// <inheritdoc />
  public override bool Equals(object? value) => value is Range range && Equals(range);

  /// <inheritdoc />
  public bool Equals(Range other) => Start.Equals(other.Start) && End.Equals(other.End);

  /// <inheritdoc />
  public override int GetHashCode() => (Start.GetHashCode() * 31) + End.GetHashCode();

  /// <summary>
  /// Resolves the offset and length represented by this range for a sequence of the specified length.
  /// </summary>
  /// <param name="length">Length of the target collection.</param>
  /// <returns>A tuple containing the resolved offset and range length.</returns>
  public (int Offset, int Length) GetOffsetAndLength(int length)
  {
    var start = Start.GetOffset(length);
    var end = End.GetOffset(length);

    if (start > end)
      throw new ArgumentOutOfRangeException(nameof(length));

    return (start, end - start);
  }

  /// <summary>
  /// Deconstructs the range into its start and end indexes.
  /// </summary>
  /// <param name="start">Receives the inclusive start index.</param>
  /// <param name="end">Receives the exclusive end index.</param>
  public void Deconstruct(out Index start, out Index end)
  {
    start = Start;
    end = End;
  }

  /// <inheritdoc />
  public override string ToString() => Start + ".." + End;
}
#endif
