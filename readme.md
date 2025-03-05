# Beacon

[![.NET](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml/badge.svg)](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml)
[![codecov](https://codecov.io/gh/erinnmclaughlin/Beacon/graph/badge.svg?token=L23K6YGUER)](https://codecov.io/gh/erinnmclaughlin/Beacon)

Beacon is a **lightweight**, **modern**, and **intuitive** [laboratory information management system (LIMS)](https://en.wikipedia.org/wiki/Laboratory_information_management_system) focued on managing common laboratory operations. Beacon aims to meet the following goals:

- Improve process efficiency and quality of common laboratory operations.
- Simplify the user experience by offering a lightweight and intuitive core solution.
- Avoid feature bloat by offering optional extensions to the core system.

This project was initially developed at Penn State University as my Master's capstone project.

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

