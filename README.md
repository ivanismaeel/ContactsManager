# Contacts Manager

A comprehensive contact management application built with clean architecture principles and ASP.NET Core MVC.

![ContactsManager](https://github.com/user-attachments/assets/17942249-a18d-4664-8ea8-2449e8d274f6)


## Architecture Overview

```
ContactsManager/
├── src/
│   ├── ContactsManager.Core/           # Domain models and business logic
│   ├── ContactsManager.Infrastructure/ # Data access and external services
│   └── ContactsManager.UI/             # ASP.NET Core MVC presentation layer
└── test/
    ├── ContactsManager.ControllerTests/
    ├── ContactsManager.IntegrationTests/
    └── ContactsManager.ServiceTests/
```

The project follows both Clean Architecture and MVC patterns:

- **Clean Architecture**: Separates the application into concentric layers with dependencies pointing inward
- **MVC Pattern**: Organizes the UI layer using the Model-View-Controller pattern

### Core Layer

```
ContactsManager.Core/
├── Domain/
│   ├── Entities/           # Business models
│   ├── IdentityEntities/   # Authentication models
│   └── RepositoryContracts/ # Data access interfaces
├── DTO/                    # Data Transfer Objects
├── Enums/                  # Type-safe enumerations
├── Exceptions/             # Custom exceptions
├── Helpers/                # Utility classes
├── ServiceContracts/       # Business operation interfaces
└── Services/               # Business logic implementations
```

### Infrastructure Layer

```
ContactsManager.Infrastructure/
├── DbContext/              # EF Core database context
├── Migrations/             # Database schema evolution
└── Repositories/           # Data access implementations
```

### UI Layer (MVC)

```
ContactsManager.UI/
├── Areas/                  # Feature-specific sections
├── Controllers/            # MVC Controllers handling HTTP requests
│   ├── AccountController.cs
│   ├── CountriesController.cs
│   ├── HomeController.cs
│   └── PersonsController.cs
├── Filters/                # Request pipeline components
├── Middleware/             # HTTP request pipeline
├── Views/                  # MVC Views (UI templates)
│   ├── Account/            # Authentication views
│   ├── Countries/          # Country management views
│   ├── Home/               # Dashboard/home views
│   ├── Persons/            # Contact management views
│   └── Shared/             # Shared layouts and partials
├── wwwroot/                # Static assets (CSS, JS, images)
└── Program.cs              # Application entry point
```

## Key Features

- Contact management (add, edit, delete, search, sort, filter)
- Country management for contact addresses
- Data export/import functionality
- User authentication & role-based authorization
- Comprehensive error handling
- Responsive UI design

## Technology Stack

- ASP.NET Core 9.0 MVC
- Entity Framework Core
- SQL Server (configurable)
- ASP.NET Core Identity
- HTML, CSS, JavaScript, jQuery
- xUnit, Moq for testing

## Quick Start

### Prerequisites

```bash
# Check .NET version (requires 9.0+)
dotnet --version
```

### Clone & Setup

```bash
# Clone repository
git clone https://github.com/ivanismaeel/ContactsManager.git

# Navigate to project
cd ContactsManager

# Restore dependencies
dotnet restore
```

### Database Setup

#### Option 1: Local SQL Server

```bash
# Update database connection in src/ContactsManager.UI/appsettings.json first, then:

# Apply migrations
dotnet ef database update --project src/ContactsManager.Infrastructure --startup-project src/ContactsManager.UI
```

#### Option 2: Docker SQL Server

```bash
# Start SQL Server using Docker Compose
docker-compose up -d sql

# Update connection string in appsettings.json to:
# "ConnectionStrings": {
#   "DefaultConnection": "Server=localhost,1433;Database=ContactsManager;User Id=sa;Password=Password@2;TrustServerCertificate=True"
# }

# Apply migrations
dotnet ef database update --project src/ContactsManager.Infrastructure --startup-project src/ContactsManager.UI
```

### Build & Run

```bash
# Build solution
dotnet build

# Run application
dotnet run --project src/ContactsManager.UI

# Access the application at https://localhost:5001 or http://localhost:5000
```

### Docker Deployment

```bash
# Start SQL Server container only
docker-compose up -d sql

# Access SQL Server at localhost:1433
# User: sa
# Password: Password@2
```

When you're ready to containerize the web application as well, uncomment the web service section in docker-compose.yml and run:

```bash
# Start both SQL Server and web application
docker-compose up -d
```

## Docker Compose Configuration

The project includes a `docker-compose.yml` file for easy deployment of the SQL Server database:

```yaml
services:
  sql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sql
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "Password@2"
    platform: "linux/amd64"
    ports:
      - "1433:1433"
      
  # Uncomment when ready to containerize the web application
  # web:
  #   build: .
  #   container_name: contacts-manager
  #   ports:
  #     - "8080:80"
  #   depends_on:
  #     - sql
  #   environment:
  #     - ConnectionStrings__DefaultConnection=Server=sql,1433;Database=ContactsManager;User Id=sa;Password=Password@2;TrustServerCertificate=True
```

## MVC Implementation

The application follows the standard MVC pattern:

- **Models**: Defined in the Core layer as domain entities and DTOs
- **Views**: Located in the UI layer's Views directory, organized by controller
- **Controllers**: Handle HTTP requests and coordinate between models and views

Key controllers include:
- `PersonsController`: Manages contact CRUD operations
- `CountriesController`: Handles country data management
- `AccountController`: Manages user authentication
- `HomeController`: Handles the dashboard and navigation

## Development Commands

### Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test test/ContactsManager.ServiceTests
dotnet test test/ContactsManager.ControllerTests
dotnet test test/ContactsManager.IntegrationTests

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

### EF Core Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/ContactsManager.Infrastructure --startup-project src/ContactsManager.UI

# Remove last migration
dotnet ef migrations remove --project src/ContactsManager.Infrastructure --startup-project src/ContactsManager.UI
```

### Publishing

```bash
# Publish for production
dotnet publish src/ContactsManager.UI -c Release -o ./publish

# Run published app
cd publish
dotnet ContactsManager.UI.dll
```

## Best Practices Implemented

- Clean Architecture with separation of concerns
- MVC pattern for organized UI development
- SOLID principles (Single responsibility, Interface segregation)
- Repository pattern for data access abstraction
- Dependency injection for loose coupling
- Comprehensive test coverage

## License

This project is licensed under the MIT License - see the LICENSE file for details.
