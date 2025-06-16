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

## API Documentation
This API is documented using OpenAPI. You can explore and test the endpoints interactively via the Scalar UI.

Scalar UI URL: https://localhost:7126/api/scalar/ or http://localhost:5244/api/scalar/

Scalar provides a modern and user-friendly interface to navigate the OpenAPI specification and execute requests directly from the browser.

---

1. ## Clone the project 
```bash

git clone https://github.com/Ezefeola/FarmacityTestApi.git
cd FarmacityTestApi

```

2. ## Running the Project
Make sure you're in the root of the solution and that the SDK for .NET 8 is installed.
### Run the API locally

```bash
cd src/Adapter.Api
dotnet run --launch-profile "https"
```

3. ### Run Api Tests
Make sure you're in the root of the solution and that the SDK for .NET 8 is installed.

```bash
cd src/Core.Tests
dotnet test
```