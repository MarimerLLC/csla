//-----------------------------------------------------------------------
// <copyright file="SafeDataReaderAsyncTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the async members of SafeDataReader</summary>
//-----------------------------------------------------------------------

using System.Data;
using System.Data.SQLite;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.SafeDataReader
{
  [TestClass]
  public class SafeDataReaderAsyncTests
  {
    private const string DataTypesQuery =
      "SELECT BITFIELD, CHARFIELD, DATETIMEFIELD, UNIQUEIDENTIFIERFIELD, SMALLINTFIELD, INTFIELD, BIGINTFIELD, TEXT, IMAGEFIELD FROM MultiDataTypes";

    private SQLiteConnection _connection;

    [TestInitialize]
    public void Initialize()
    {
      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
      _connection = new SQLiteConnection(WellKnownValues.DataPortalTestDatabase);
      _connection.Open();
    }

    [TestCleanup]
    public void Cleanup()
    {
      _connection.Dispose();
    }

    private IDataReader ExecuteReader(string commandText)
    {
      using var cm = _connection.CreateCommand();
      cm.CommandText = commandText;
      return cm.ExecuteReader();
    }

    [TestMethod]
    public async Task ReadAsyncReadsAllRows()
    {
      var names = new List<string>();
      await using (var dr = new Csla.Data.SafeDataReader(ExecuteReader("SELECT Name, Date, Age FROM Table1")))
      {
        while (await dr.ReadAsync())
          names.Add($"{await dr.GetStringAsync("Name")}, age {await dr.GetInt32Async("Age")}, added on {await dr.GetSmartDateAsync("Date")}");
      }

      names.Should().Equal("Bill, age 56, added on 12/23/2004", "Jim, age 33, added on 1/14/2003");
    }

    [TestMethod]
    public async Task NextResultAsyncMovesToNextResultSet()
    {
      await using var dr = new Csla.Data.SafeDataReader(ExecuteReader("SELECT Name FROM Table1; SELECT TEXT FROM MultiDataTypes"));

      (await dr.NextResultAsync()).Should().BeTrue();
      (await dr.ReadAsync()).Should().BeTrue();
      (await dr.GetStringAsync(0)).Should().StartWith("a bunch of text");
      (await dr.NextResultAsync()).Should().BeFalse();
    }

    [TestMethod]
    public async Task AsyncGettersReturnValues()
    {
      await using var dr = new Csla.Data.SafeDataReader(ExecuteReader(DataTypesQuery));
      (await dr.ReadAsync()).Should().BeTrue();

      (await dr.GetBooleanAsync("BITFIELD")).Should().BeFalse();
      (await dr.GetSmartDateAsync("DATETIMEFIELD")).ToString().Should().Be("12/13/2005");
      (await dr.GetDateTimeAsync("DATETIMEFIELD")).Should().Be(new DateTime(2005, 12, 13));
      (await dr.GetGuidAsync("UNIQUEIDENTIFIERFIELD")).Should().Be(new Guid("c0f92820-61b5-11da-8cd6-0800200c9a66"));
      (await dr.GetInt16Async("SMALLINTFIELD")).Should().Be(32767);
      (await dr.GetInt32Async("INTFIELD")).Should().Be(2147483647);
      (await dr.GetInt64Async("BIGINTFIELD")).Should().Be(92233720368547111);
      (await dr.GetStringAsync(7)).Should().Be("a bunch of text...a bunch of text...a bunch of text...a bunch of text...");
      (await dr.GetValueAsync("CHARFIELD")).Should().Be("z");
      (await dr.GetFieldValueAsync<string>("CHARFIELD")).Should().Be("z");
      (await dr.GetFieldValueAsync<long>(6)).Should().Be(92233720368547111);
    }

    [TestMethod]
    public async Task AsyncGettersFixNullValues()
    {
      await using var dr = new Csla.Data.SafeDataReader(ExecuteReader(DataTypesQuery));
      (await dr.ReadAsync()).Should().BeTrue();
      const string nullColumn = "IMAGEFIELD";

      (await dr.IsDBNullAsync(nullColumn)).Should().BeTrue();
      (await dr.IsDBNullAsync(0)).Should().BeFalse();
      (await dr.GetStringAsync(nullColumn)).Should().BeEmpty();
      (await dr.GetValueAsync(nullColumn)).Should().BeNull();
      (await dr.GetInt16Async(nullColumn)).Should().Be(0);
      (await dr.GetInt32Async(nullColumn)).Should().Be(0);
      (await dr.GetInt64Async(nullColumn)).Should().Be(0);
      (await dr.GetDoubleAsync(nullColumn)).Should().Be(0);
      (await dr.GetFloatAsync(nullColumn)).Should().Be(0);
      (await dr.GetDecimalAsync(nullColumn)).Should().Be(0);
      (await dr.GetByteAsync(nullColumn)).Should().Be(0);
      (await dr.GetBooleanAsync(nullColumn)).Should().BeFalse();
      (await dr.GetGuidAsync(nullColumn)).Should().Be(Guid.Empty);
      (await dr.GetDateTimeAsync(nullColumn)).Should().Be(DateTime.MinValue);
      (await dr.GetDateTimeOffsetAsync(nullColumn)).Should().Be(DateTimeOffset.MinValue);
      (await dr.GetSmartDateAsync(nullColumn)).IsEmpty.Should().BeTrue();
      (await dr.GetSmartDateAsync(nullColumn, false)).Date.Should().Be(DateTime.MaxValue);
    }

    [TestMethod]
    public async Task CanceledTokenCancelsReadAsync()
    {
      await using var dr = new Csla.Data.SafeDataReader(ExecuteReader("SELECT Name FROM Table1"));
      using var cts = new CancellationTokenSource();
      cts.Cancel();

      await FluentActions.Awaiting(() => dr.ReadAsync(cts.Token)).Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task CloseAsyncClosesReader()
    {
      await using var dr = new Csla.Data.SafeDataReader(ExecuteReader("SELECT Name FROM Table1"));
      dr.IsClosed.Should().BeFalse();

      await dr.CloseAsync();

      dr.IsClosed.Should().BeTrue();
    }

    [TestMethod]
    public async Task DisposeAsyncDisposesReader()
    {
      var inner = ExecuteReader("SELECT Name FROM Table1");
      var dr = new Csla.Data.SafeDataReader(inner);

      await dr.DisposeAsync();

      inner.IsClosed.Should().BeTrue();
    }

    [TestMethod]
    public async Task AsyncMembersFallBackWhenReaderIsNotDbDataReader()
    {
      // SafeDataReader is an IDataReader but not a DbDataReader, so
      // wrapping one exercises the synchronous fallback path
      var inner = new Csla.Data.SafeDataReader(ExecuteReader(DataTypesQuery));
      await using var dr = new Csla.Data.SafeDataReader(inner);

      (await dr.ReadAsync()).Should().BeTrue();
      (await dr.IsDBNullAsync("IMAGEFIELD")).Should().BeTrue();
      (await dr.GetStringAsync("IMAGEFIELD")).Should().BeEmpty();
      (await dr.GetInt32Async("INTFIELD")).Should().Be(2147483647);
      (await dr.GetFieldValueAsync<string>("CHARFIELD")).Should().Be("z");
      (await dr.NextResultAsync()).Should().BeFalse();

      using var cts = new CancellationTokenSource();
      cts.Cancel();
      await FluentActions.Awaiting(() => dr.ReadAsync(cts.Token)).Should().ThrowAsync<OperationCanceledException>();

      await dr.CloseAsync();
      inner.IsClosed.Should().BeTrue();
    }
  }
}
