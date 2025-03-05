# Beacon

[![.NET](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml/badge.svg)](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml)

## Getting Started

### Prerequisites

* [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
* Your favorite C# (e.g., Visual Studio, Rider, or VS Code).
* Your favorite db server (current options are Postgres or Microsoft SQL Server)

## Build and Run Instructions

1. Open the solution file `Beacon.sln` in your chosen IDE.
2. Ensure the `src/Server/Beacon.WebHost/appSettings.Development.json` file is configured correctly:
   * Update the `StorageProvider` to either `Postgres` or `MsSqlServer`
   * Update the relevant connection string(s) to point to your local database server
2. Restore NuGet packages.
3. Build the solution.
4. Run the project using the appropriate launch profile from `src/Server/Beacon.WebHost/Properties/launchSettings.json`.

