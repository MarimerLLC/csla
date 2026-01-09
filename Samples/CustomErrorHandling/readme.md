## Custom Error Handling sample

This sample shows how to use a custom data portal exception inspector with CSLA.

### Prerequisites
- .NET 10 SDK
- Windows (for the WinForms client)

### Build
- From the repo root: `dotnet build Samples/CustomErrorHandling/CustomErrorHandling.sln`

### Run the data portal host (AppServer)
- Start Kestrel with the project profile: `dotnet run --project Samples/CustomErrorHandling/AppServer/AppServer.csproj`
- The data portal controller listens at: https://localhost:5001/api/dataportal

### Run the WinForms client (WindowsUI)
- In a new terminal: `dotnet run --project Samples/CustomErrorHandling/WindowsUI/WindowsUI.csproj`
- The client is configured to call the AppServer endpoint above.

### What to try
- Click the buttons in the client to trigger different exception scenarios (non-serializable, server-only, not implemented) and observe how the custom inspector shapes the returned exceptions.
