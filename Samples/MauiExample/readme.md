# CSLA MAUI Example

This sample is a small .NET MAUI app that uses the `Csla.Maui` package with CSLA business objects. It's set up to publish with full trimming on Android and with NativeAOT on iOS and Mac Catalyst, with no trim or AOT warnings.

## Projects

- `DataAccess`: an in-memory `IPersonDal` implementation.
- `BusinessLibrary`: the `PersonEdit`, `PersonInfo` and `PersonList` business classes and their rules.
- `MauiExample`: the MAUI app, with a person list page and a person edit page.

The **List** tab fetches a `PersonList` through the data portal. **Edit** opens a person in `PersonEditPage`, where `Name` has a required rule, a capitalization warning, an informational message and a "no letter Z" rule. `csla:PropertyInfo` shows the error, warning and information text next to the editor.

## Run the sample

Open `MauiExample.sln` in Visual Studio and run the `MauiExample` project, or from `Samples/MauiExample`:

```bash
dotnet build MauiExample/MauiExample.csproj -t:Run -f net10.0-android
```

## Trimming and NativeAOT

### Project settings

`MauiExample.csproj`:

| Setting | Platforms | Why |
|---|---|---|
| `TrimmerSingleWarn=false` | All | Report each trim warning at its location instead of one warning per assembly |
| `TrimMode=full` | Android | Trim all assemblies, not only the framework. Android Release builds trim by default |
| `PublishAot=true` | iOS, Mac Catalyst | Compile with NativeAOT on publish. These are the platforms MAUI supports NativeAOT on. NativeAOT always trims every assembly, so `TrimMode` isn't set for them |

`BusinessLibrary.csproj` and `DataAccess.csproj` set `IsAotCompatible=true`, so the trim and AOT analyzers run when the libraries build, not only when the app is published.

### What the code does to stay trim safe

- **Business classes are `partial`** and their data portal methods have operation attributes (`[Create]`, `[Fetch]`, `[Insert]`, `[Update]`, `[DeleteSelf]`, `[Delete]`, `[FetchChild]`). The CSLA source generator then calls the operation methods directly, without reflection. Analyzer CSLA0024 reports classes that aren't `partial`.
- **Properties are copied explicitly.** `PersonEdit.Fetch` assigns `Id` and `Name` from the entity instead of calling `DataMapper.Map`, which reflects over both objects and is marked `[RequiresUnreferencedCode]`.
- **Data portals come from DI with concrete types** (`IDataPortal<PersonEdit>`, `IChildDataPortal<PersonInfo>`). These annotate their type parameter, so the trimmer keeps the business types.
- **Bindings are compiled.** Every binding has an `x:DataType`, so MAUI doesn't resolve binding paths by reflection.
- **`Styles.xaml` uses the `<OnPlatform>` element** instead of the `{OnPlatform}` markup extension, whose `Default` property isn't trim safe.

### `csla:PropertyInfo`

`csla:PropertyInfo` walks its `Path` over the binding source by reflection, so CSLA marks it `[RequiresUnreferencedCode]`. Using it in XAML produces an IL2026 warning in the page's generated `InitializeComponent`. `PersonEditPage.xaml.cs` suppresses that warning for that one method. The suppression is justified because the binding source is the `PersonEdit` business object, whose members CSLA preserves. If you bind `PropertyInfo` to an object that isn't a CSLA business object, preserve that object's type yourself.

`Csla.Maui` 11.0.0-alpha.1 also reports an IL2112 warning on `Csla.Xaml.PropertyInfo.BrokenRulesProperty`. That warning is fixed in CSLA and goes away with the next `Csla.Maui` prerelease.

## Publish

From `Samples/MauiExample`:

```bash
# Android, fully trimmed
dotnet publish MauiExample/MauiExample.csproj -f net10.0-android -c Release

# iOS device, NativeAOT (on a Mac, with a signing identity and provisioning profile)
dotnet publish MauiExample/MauiExample.csproj -f net10.0-ios -c Release

# Mac Catalyst, NativeAOT (on a Mac)
dotnet publish MauiExample/MauiExample.csproj -f net10.0-maccatalyst -c Release -r maccatalyst-arm64
```

`dotnet publish` for iOS only accepts device runtime identifiers. To check the NativeAOT build for the iOS simulator without signing, build with the property `dotnet publish` sets:

```bash
dotnet build MauiExample/MauiExample.csproj -f net10.0-ios -c Release -r iossimulator-arm64 -p:_IsPublishing=true
```

Treat any `warning IL` line in the publish output as a bug to fix.

For the full set of rules, see "Trimming and NativeAOT annotations" in [Upgrading to CSLA 11](../../docs/Upgrading%20to%20CSLA%2011.md#trimming-and-nativeaot-annotations).
