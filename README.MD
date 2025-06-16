# FarmacityTest Backend API

This project is a backend API built with **.NET 8**, using **Entity Framework Core**, following **Hexagonal Architecture** principles. It is structured with **Minimal APIs**, supports testing with **xUnit** and **Moq**, and manages persistence with SQL Server.

---

## Architecture Overview

The solution is organized around Hexagonal Architecture, with clearly separated concerns:

- **Core**  
  Contains the application and domain logic, interfaces, and models.

- **Adapter.Api**  
  The entry point for the application using Minimal APIs.

- **Adapter.SqlServer**  
  Infrastructure layer for SQL Server database access via Entity Framework Core.

- **Adapter.CompositionRoot**  
  Centralized configuration and dependency injection wiring.

- **Core.Tests**  
  Unit test project using [xUnit](https://xunit.net/) and [Moq](https://github.com/moq/moq).

---

1. ## Clone the project 
```bash

git clone https://github.com/Ezefeola/FarmacityTestApi.git
cd farmacity-test-backend

```

2. ## Running the Project
Make sure you're in the root of the solution and that the SDK for .NET 8 is installed.

### Run the API locally

```bash
cd Adapter.Api
dotnet run --launch-profile "https"
