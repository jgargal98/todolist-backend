# Project Status: Notes Application (Full Stack)

Technical summary of the current infrastructure, system configuration, and development progress for the Notes application.

## System Architecture

The system utilizes a decoupled cloud-based architecture:

- Framework: .NET 10 Web API (LTS).
- Architecture: Clean Architecture (Domain, Application, Infrastructure, API).
- Database: Azure SQL Database (Relational).
- Hosting: Azure App Service (F1 - Free Plan).
- Frontend: Azure Static Web App.
- Deployment: Integrated CI/CD via GitHub Actions.

## Implemented Configurations

### 1. Data Layer (Azure SQL)

- Instance: Logical SQL Server provisioned in a region with available quota.
- Network Security:
    - Firewall enabled for internal Azure service communication.
    - Local IP whitelisting configured for development environment access.
- Access: Configured via SQL Server Authentication.

### 2. Application Layer (App Service)

- Platform: App Service running on a .NET-optimized environment.
- Secret Management: Connection string injection implemented via Azure Environment Variables (ConnectionStrings\_\_DefaultConnection).
- Code Security: The appsettings.json file in the repository contains no sensitive credentials, delegating real authentication to the Azure Portal configuration.

### 3. Deployment Workflow (CI/CD)

- Repository: GitHub ([jgargal/TodoList](https://github.com/jgargal98/todolist)).
- Automation: A production-ready workflow is established where every push to the main branch triggers a GitHub Action to build, test, and deploy the code directly to the Azure production environment.

## System Design and Visualization

To ensure scalability and maintainability, the project follows strict architectural and data modeling standards.

### File Structure and Clean Architecture

The following structure demonstrates the separation of concerns. The Domain layer remains independent, while Infrastructure handles data persistence and Application manages business logic and DTO mapping.

```text
todolist/
├── .github/workflows/          # CI/CD Pipelines (Backend & Frontend)
├── backend/
│   ├── TodoList.API/           # Entry point, Controllers, and Program.cs
│   ├── TodoList.Application/   # DTOs, Interfaces, Mappings, and Services
│   ├── TodoList.Domain/        # Entities and Repository Interfaces
│   └── TodoList.Infrastructure/# Data Context, Repositories, and Migrations
│
│
└── frontend/                   # Angular front end
```

### Data Model (Entity Relationship Diagram)

The database schema is designed to handle user authentication and relational note management efficiently. This diagram illustrates the core entities and their relationships within the Azure SQL instance.

![Entity Relationship Diagram](ToDo-Schema.png)

## Recent Development Progress

The following critical backend milestones have been achieved:

- Clean Architecture Implementation: Structured the solution into Domain, Application, Infrastructure, and API projects.
- DTO Mapping: Integrated AutoMapper to decouple domain entities from API responses, ensuring secure data transfer and abstraction.
- Repository Pattern: Established the foundation for data access through interfaces in the Domain layer and implementations in the Infrastructure layer.

## Current Status

- Backend (API): Fully operational. Deployed on Azure App Service using .NET 10 LTS. The API handles database migrations automatically via the DbInitializer on startup.
- Frontend: Fully deployed. Hosted as an Azure Static Web App, communicating with the production API endpoint.
- Connectivity: Link between App Service and SQL Database verified through environment variables.
- Codebase: Repository synchronized and hardened against credential leaks.
- Database: Schema is up to date and provisioned on Azure SQL.
