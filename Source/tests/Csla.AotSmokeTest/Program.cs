//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Runs CSLA scenarios in a NativeAOT (or trimmed) published app</summary>
//-----------------------------------------------------------------------

using System.Runtime.CompilerServices;
using Csla;
using Csla.AotSmokeTest;
using Csla.Configuration;
using Csla.Core;
using Csla.Rules;
using Csla.Serialization;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCsla();
using var provider = services.BuildServiceProvider();
var applicationContext = provider.GetRequiredService<ApplicationContext>();
var personPortal = provider.GetRequiredService<IDataPortal<PersonEdit>>();

Console.WriteLine($"Dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");

var failures = 0;

async Task Check(string name, Func<Task> scenario)
{
  try
  {
    await scenario();
    Console.WriteLine($"PASS {name}");
  }
  catch (Exception ex)
  {
    failures++;
    Console.WriteLine($"FAIL {name}");
    Console.WriteLine(ex);
  }
}

void Assert(bool condition, string message)
{
  if (!condition)
    throw new InvalidOperationException(message);
}

string Broken(BusinessBase obj) =>
  string.Join("; ", obj.BrokenRulesCollection.Select(r => $"{r.Property}: {r.Description}"));

await Check("Create root with child list", async () =>
{
  var person = await personPortal.CreateAsync();
  Assert(person.IsNew, "IsNew");
  Assert(person.Friends.Count == 0, "Friends empty");
  person.Friends.AddNew().Name = "Bob";
  Assert(person.Friends.Count == 1 && person.Friends[0].Name == "Bob", "AddNew child");
});

await Check("Fetch root with child list", async () =>
{
  var person = await personPortal.FetchAsync(42);
  Assert(person.Id == 42 && person.Age == 42 && person.Name == "Fetched", "fetched values");
  Assert(!person.IsNew, "IsNew false after fetch");
  Assert(person.Friends.Count == 1 && person.Friends[0].Name == "Friend of 42", "fetched children");
});

await Check("LoadProperty(IPropertyInfo, object)", async () =>
{
  var person = await personPortal.CreateAsync();
  person.LoadUntyped(PersonEdit.NameProperty, "Grace");
  person.LoadUntyped(PersonEdit.AgeProperty, 7);
  person.LoadUntyped(PersonEdit.ScoreProperty, 3);
  person.LoadUntyped(PersonEdit.KindProperty, PersonKind.Vendor);
  Assert(person.Name == "Grace" && person.Age == 7 && person.Score == 3 && person.Kind == PersonKind.Vendor, "loaded values");
});

await Check("LoadProperty widening fallback", async () =>
{
  var person = await personPortal.CreateAsync();
  if (RuntimeFeature.IsDynamicCodeSupported)
  {
    person.LoadUntyped(PersonEdit.TotalProperty, 5);
    Assert(person.Total == 5L, "widened value");
  }
  else
  {
    try
    {
      person.LoadUntyped(PersonEdit.TotalProperty, 5);
      throw new InvalidOperationException("expected NotSupportedException without dynamic code");
    }
    catch (NotSupportedException)
    { }
  }
});

await Check("Validation rules (data annotations, lambda, async)", async () =>
{
  var person = await personPortal.CreateAsync();
  Assert(!person.IsValid, "Required name broken on create");
  person.Name = "Ada";
  Assert(person.IsValid, $"valid after setting name: {Broken(person)}");
  person.Name = new string('x', 30);
  Assert(!person.IsValid, "StringLength broken");
  person.Name = "Ada";
  person.Age = 500;
  Assert(!person.IsValid, "Range broken");
  person.Age = 30;

  person.Nickname = "bad";
  await person.WaitForIdle();
  Assert(!person.IsValid, "lambda rule broken");

  person.Nickname = "reserved";
  await person.WaitForIdle();
  Assert(!person.IsValid, "async rule broken");

  person.Nickname = "ok";
  await person.WaitForIdle();
  Assert(person.IsValid, $"valid again: {Broken(person)}");
});

await Check("Authorization rules (property and object)", async () =>
{
  var person = await personPortal.CreateAsync();
  Assert(!person.CanWriteSecret, "IsInRole denies property write without the role");
  Assert(BusinessRules.HasPermission(applicationContext, AuthorizationActions.CreateObject, typeof(PersonEdit)), "create allowed");
  Assert(!BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PersonEdit)), "delete denied without the role");
});

await Check("Clone and n-level undo", async () =>
{
  var person = await personPortal.CreateAsync();
  person.Name = "Ada";
  person.Friends.AddNew().Name = "Bob";
  var clone = person.Clone();
  Assert(clone.Name == "Ada" && clone.Friends.Count == 1 && clone.Friends[0].Name == "Bob", "clone values");

  person.BeginEdit();
  person.Name = "Changed";
  person.Friends.AddNew().Name = "Carol";
  person.CancelEdit();
  Assert(person.Name == "Ada" && person.Friends.Count == 1, "undo restored state");
});

await Check("Serialize and deserialize bytes", async () =>
{
  var person = await personPortal.FetchAsync(7);
  person.Kind = PersonKind.Vendor;
  var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
  var copy = (PersonEdit)formatter.Deserialize(formatter.Serialize(person))!;
  Assert(copy.Name == "Fetched" && copy.Kind == PersonKind.Vendor && copy.Friends.Count == 1, "round-tripped values");
});

await Check("Save (insert, update) and SaveAndMerge", async () =>
{
  var person = await personPortal.CreateAsync();
  person.Name = "Ada";
  person.Friends.AddNew().Name = "Bob";
  person = await person.SaveAsync();
  Assert(!person.IsNew && !person.IsDirty && person.Id == 100, "inserted");

  person.Name = "Ada L";
  person.Friends[0].Name = "Robert";
  await person.SaveAndMergeAsync();
  Assert(!person.IsDirty && person.Name == "Ada L" && person.Friends[0].Name == "Robert", "updated and merged");
});

await Check("GraphMerger", async () =>
{
  var target = await personPortal.FetchAsync(1);
  var source = target.Clone();
  source.Name = "Merged";
  source.Friends[0].Name = "Robert";
  new GraphMerger(applicationContext).MergeGraph(target, source);
  Assert(target.Name == "Merged" && target.Friends[0].Name == "Robert", "merged values");
});

await Check("Delete denied by object authorization", async () =>
{
  try
  {
    await personPortal.DeleteAsync(1);
    throw new InvalidOperationException("expected the delete to be denied");
  }
  catch (Exception ex) when (ex is Csla.Security.SecurityException || ex.InnerException is Csla.Security.SecurityException)
  { }
});

await Check("Command execute with criteria", async () =>
{
  var command = await provider.GetRequiredService<IDataPortal<PersonExists>>().ExecuteAsync("Ada");
  Assert(command.Exists, "command result");
});

await Check("Read-only list fetch", async () =>
{
  var list = await provider.GetRequiredService<IDataPortal<PersonInfoList>>().FetchAsync();
  Assert(list.Count == 2 && list[1].Name == "Grace", "list contents");
});

await Check("ObjectFactory fetch", async () =>
{
  var item = await provider.GetRequiredService<IDataPortal<InventoryItem>>().FetchAsync("ABC-1");
  Assert(item.Sku == "ABC-1" && !item.IsNew, "factory-fetched item");
});

await Check("ObjectFactory fetch by type name", async () =>
{
  var item = await provider.GetRequiredService<IDataPortal<CatalogItem>>().FetchAsync("CAT-7");
  Assert(item.Code == "CAT-7", "factory-fetched item");
});

Console.WriteLine(failures == 0 ? "All scenarios passed" : $"{failures} scenario(s) failed");
return failures == 0 ? 0 : 1;
