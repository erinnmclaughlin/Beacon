# Beacon

[![.NET](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml/badge.svg)](https://github.com/erinnmclaughlin/Beacon/actions/workflows/dotnet.yml)

## Prerequisites

To run the project locally, you need to ensure the following prerequisites are met:

* Install .NET SDK 9.0
* Install a compatible IDE such as Visual Studio, Rider, or VS Code.
* Ensure you have a local instance of either Microsoft SQL Server or PostgreSQL.
  * Update the `StorageProvider` in `src/Server/Beacon.WebHost/appsettings.Development.json` to either `MsSqlServer` or `Postgres`
  * Update the relevant connection string(s) to point to your local database server

## Build and Run Instructions

1. Open the solution file `Beacon.sln` in your chosen IDE.
2. Restore NuGet packages.
3. Build the solution.
4. Run the project using the appropriate launch profile from `src/Server/Beacon.WebHost/Properties/launchSettings.json`.

## Project Structure Overview

The project is structured into several main directories, each serving a specific purpose:

* `src/Client`: Contains client-side code.
* `src/Server`: Contains server-side code.
* `src/Shared`: Contains shared code used across different parts of the application.

### Key Projects

* `Beacon.API`: This project contains the API for the Beacon application. It includes the implementation of various features and endpoints for handling authentication, laboratory management, project management, and more.
* `Beacon.WebHost`: This project serves as the web host for the Beacon application. It configures and runs the web application, including setting up the database context and configuring the pipeline.
* `Beacon.Common`: This project contains shared code and models used across different parts of the Beacon application. It includes common services, request models, and validation rules.
* `BeaconUI.Core`: This project contains the core client-side code for the Beacon UI. It includes components, services, and other client-side logic for the application.
* `BeaconUI.WebApp`: This project contains the web application for the Beacon UI. It references `BeaconUI.Core` and sets up the web assembly host for the client-side application.
* `BeaconUI.DesktopApp`: This project contains the desktop application for the Beacon UI. It references `BeaconUI.Core` and sets up the desktop application using .NET MAUI.
* `Beacon.StorageProviders.MsSqlServer`: This project contains the storage provider implementation for Microsoft SQL Server. It includes the Entity Framework Core configuration and migrations for the SQL Server database.
* `Beacon.StorageProviders.Postgres`: This project contains the storage provider implementation for PostgreSQL. It includes the Entity Framework Core configuration and migrations for the PostgreSQL database.

### Important Configuration Files

* `src/Server/Beacon.WebHost/appsettings.Development.json`: Contains development-specific settings, including connection strings for databases.
* `src/Server/Beacon.WebHost/appsettings.json`: Contains general application settings.
