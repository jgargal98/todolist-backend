# TodoList API

A **Clean Architecture** backend for a personal task management application built with **.NET 10**, **Entity Framework Core**, **ASP.NET Core Identity**, and **JWT authentication**. The API supports full CRUD for tasks, categories, tags, and user management with role-based access.

## Project Status

The project is fully functional with the following capabilities:

- [x] User registration and login with JWT (access + refresh tokens)
- [x] Task CRUD with subtasks, status, due dates, categories, and tags
- [x] Category CRUD (per-user scoped)
- [x] Tag CRUD (many-to-many relationship with tasks)
- [x] User listing
- [x] Input validation via FluentValidation
- [x] Global exception handling middleware
- [x] Swagger/OpenAPI documentation with JWT bearer auth
- [x] AutoMapper for entity-to-DTO mapping
- [x] EF Core auto-migrations at startup via `InitializeDatabaseAsync`
- [x] CORS policy configured for Angular frontend
- [x] Asymmetric RSA key pair (RS256) for JWT signing
- [x] Unit tests (xUnit + Moq + FluentValidation)
- [x] Integration tests (xUnit + WebApplicationFactory + InMemory DB)

## Tech Stack

| Technology            | Version |
| --------------------- | ------- |
| .NET                  | 10.0    |
| Entity Framework Core | 10.0.7  |
| ASP.NET Core Identity | 10.0.7  |
| JWT Bearer Auth       | 10.0.7  |
| AutoMapper            | 16.1.1  |
| FluentValidation      | 12.1.1  |
| Swashbuckle (Swagger) | 10.1.7  |
| SQL Server (LocalDB)  | LocalDB |
| xUnit                 | 2.9.3   |
| Moq                   | 4.20.72 |
| FluentAssertions      | 7.2.0   |

## Architecture

The solution follows **Clean Architecture** principles with four projects:

```
TodoList.Domain         → Entities (no dependencies)
TodoList.Application    → DTOs, services, mappings, interfaces
TodoList.Infrastructure → EF Core, repositories, JWT provider, DB seeding
TodoList.API            → Controllers, middleware, validation, startup
```

Dependency flow: `API → Application → Domain` and `API → Infrastructure → Application + Domain`.

### File Structure

```
todolist/
├── backend/
│   ├── TodoList.API/               # Entry point, Controllers, Program.cs
│   │   ├── Controllers/            # Auth, Tasks, Categories, Tags, Users
│   │   ├── Middlewares/            # GlobalExceptionMiddleware
│   │   └── Validation/            # FluentValidation validators
│   ├── TodoList.Application/       # DTOs, Interfaces, Services, Mappings
│   ├── TodoList.Domain/            # Entities (User, TaskItem, Category, Tag)
│   ├── TodoList.Infrastructure/    # Data context, Repositories, JWT, seeding
│   ├── TodoList.UnitTests/         # Unit tests (validators, services, mappings)
│   ├── TodoList.IntegrationTests/  # Integration tests (full HTTP pipeline)
│   └── TodoList.API.postman_collection.json  # Postman collection for demo
│
└── frontend/                       # Angular front end
```

## Data Model

### Entities

| Entity       | Description                                                                                    |
| ------------ | ---------------------------------------------------------------------------------------------- |
| **User**     | Custom Identity user with `RefreshToken` and `RefreshTokenExpiryTime`                          |
| **TaskItem** | Core task with title, description, due date, status (1-5), subtasks (JSON), category, and tags |
| **SubTask**  | Owned value object stored as JSON array within TaskItem                                        |
| **Category** | User-scoped task categories                                                                    |
| **Tag**      | User-scoped labels with many-to-many relationship to tasks (join table `TaskTags`)             |

### Entity Relationships

```
User (1) ──< TaskItem (N)
User (1) ──< Category (N)
User (1) ──< Tag (N)
Category (1) ──< TaskItem (N)
TaskItem (M) >──< Tag (N)  [via TaskTags]
```

### Data Model (Entity Relationship Diagram)

![Entity Relationship Diagram](ToDo-Schema.png)

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (comes with Visual Studio) or full SQL Server

### Setup

1. **Clone the repository** and navigate to `backend/`
2. **Run the API**:

    ```bash
    dotnet run --project TodoList.API
    ```

    The API starts at `http://localhost:5124`. Swagger UI is available at `http://localhost:5124/swagger`.

### Default Admin Account

On first run, the application seeds an admin user:

| Field    | Value                |
| -------- | -------------------- |
| Email    | `admin@todolist.com` |
| Username | `admin`              |
| Password | `Admin123!`          |

## API Endpoints

### Authentication (`api/Auth`)

| Method | Path                 | Description                        |
| ------ | -------------------- | ---------------------------------- |
| POST   | `/api/Auth/register` | Register a new user                |
| POST   | `/api/Auth/login`    | Login, returns JWT + refresh token |
| POST   | `/api/Auth/refresh`  | Refresh expired access token       |

### Tasks (`api/Tasks`)

All task endpoints require a valid JWT (`Authorization: Bearer <token>`).

| Method | Path                   | Description                               |
| ------ | ---------------------- | ----------------------------------------- |
| GET    | `/api/Tasks`           | List all tasks for the authenticated user |
| POST   | `/api/Tasks`           | Create a new task                         |
| PUT    | `/api/Tasks/{id}`      | Update a task                             |
| DELETE | `/api/Tasks/{id:guid}` | Delete a task                             |

### Categories (`api/Categories`)

| Method | Path                        | Description          |
| ------ | --------------------------- | -------------------- |
| GET    | `/api/Categories`           | List user categories |
| POST   | `/api/Categories`           | Create a category    |
| PUT    | `/api/Categories/{id:guid}` | Update a category    |
| DELETE | `/api/Categories/{id:guid}` | Delete a category    |

### Tags (`api/Tags`)

| Method | Path                  | Description    |
| ------ | --------------------- | -------------- |
| GET    | `/api/Tags`           | List user tags |
| POST   | `/api/Tags`           | Create a tag   |
| DELETE | `/api/Tags/{id:guid}` | Delete a tag   |

### Users (`api/Users`)

| Method | Path         | Description               |
| ------ | ------------ | ------------------------- |
| GET    | `/api/Users` | List all registered users |

### Health Check

| Method | Path              | Description                    |
| ------ | ----------------- | ------------------------------ |
| GET    | `/api/HelloWorld` | Returns `"hello from the api"` |

### Task Status Values

| Value | Name        |
| ----- | ----------- |
| 1     | Pending     |
| 2     | In Progress |
| 3     | On Hold     |
| 4     | Completed   |
| 5     | Canceled    |

## Testing

The solution contains two test projects: **Unit Tests** and **Integration Tests**.

### Running Tests

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test TodoList.UnitTests

# Run only integration tests
dotnet test TodoList.IntegrationTests
```

### Unit Tests (`TodoList.UnitTests`)

Uses **xUnit** + **Moq** + **FluentValidation TestHelper**. Located at `TodoList.UnitTests/`.

| Category       | File                                                          | What it tests                                   |
| -------------- | ------------------------------------------------------------- | ----------------------------------------------- |
| Validators     | `Validators/Auth/LoginRequestValidatorTests.cs`               | Login email/password validation rules           |
| Validators     | `Validators/Auth/RegisterRequestValidatorTests.cs`            | Register email, password, confirm rules         |
| Validators     | `Validators/Auth/RefreshRequestValidatorTests.cs`             | Refresh token presence validation               |
| Validators     | `Validators/Category/CreateCategoryRequestValidatorTests.cs`  | Category name max length and required rules     |
| Validators     | `Validators/Category/UpdateCategoryRequestValidatorTests.cs`  | Category name max length and required rules     |
| Validators     | `Validators/Tag/CreateTagRequestValidatorTests.cs`            | Tag name max length and required rules          |
| Validators     | `Validators/Task/CreateTaskRequestValidatorTests.cs`          | Task title, description, status, due date, subs |
| Validators     | `Validators/Task/UpdateTaskRequestValidatorTests.cs`          | Task title, description, status, due date, subs |
| Services       | `Services/AuthServiceTests.cs`                                | Login, register, refresh token logic            |
| Services       | `Services/CategoryServiceTests.cs`                            | Category CRUD with user-scoping                 |
| Services       | `Services/TagServiceTests.cs`                                 | Tag CRUD with user-scoping                      |
| Services       | `Services/TaskServiceTests.cs`                                | Task CRUD with subtasks, tags, ownership        |
| Services       | `Services/UserServiceTests.cs`                                | User listing                                    |
| Mappings       | `Mappings/MappingProfileTests.cs`                             | AutoMapper configuration validity and mapping   |

### Integration Tests (`TodoList.IntegrationTests`)

Uses **xUnit** + **WebApplicationFactory** + **InMemory Database** + **TestAuthHandler**. Located at `TodoList.IntegrationTests/`.

The `IntegrationTestWebApplicationFactory`:
- Replaces SQL Server with an **EF Core InMemory** database
- Replaces JWT Bearer auth with `TestAuthHandler` (auto-authenticated requests)
- Generates ephemeral RSA keys for the real JwtProvider
- Seeds a test user via `UserManager`

| File                                | What it tests                                      |
| ----------------------------------- | -------------------------------------------------- |
| `AuthIntegrationTests.cs`           | Register, login, refresh, duplicate email, bad auth|
| `TasksIntegrationTests.cs`          | Create, get all, update, delete tasks              |
| `CategoriesIntegrationTests.cs`     | Create, get all, update, delete categories         |
| `TagsIntegrationTests.cs`           | Create, get all, delete tags                       |
| `UsersIntegrationTests.cs`          | List all users                                     |
| `HelloWorldIntegrationTests.cs`     | Public health check endpoint                       |
| `ExceptionIntegrationTests.cs`      | Global exception handling middleware               |

### Postman Collection

For manual API testing and demos, a **Postman collection** (v2.1.0) is available at:

```
TodoList.API.postman_collection.json
```

Import it into Postman (`File → Import`) and follow this flow:

1. Open the **Auth > Login** request body and set valid credentials
2. **Send Login** — the test script auto-saves the JWT to `{{jwt_token}}`
3. All protected endpoints (Tasks, Categories, Tags, Users) inherit the token automatically via folder-level Bearer auth
4. Use **Create Task / Category / Tag** to generate resources; their IDs are captured automatically for subsequent update/delete requests

Collection variables:

| Variable         | Default                  | Description                     |
| ---------------- | ------------------------ | ------------------------------- |
| `base_url`       | `http://localhost:5124`   | API base URL                    |
| `jwt_token`      | *(empty)*                | Auto-populated after login      |
| `refresh_token`  | *(empty)*                | Auto-populated after login      |
| `task_id`        | *(empty)*                | Auto-populated after creation   |
| `category_id`    | *(empty)*                | Auto-populated after creation   |
| `tag_id`         | *(empty)*                | Auto-populated after creation   |

## Configuration

Key settings in `appsettings.json`:

- **ConnectionStrings:DefaultConnection** — SQL Server connection string (LocalDB by default)
- **Jwt** — RSA private/public keys (PEM), issuer, and audience
- **IdentityOptions** — Password policy and user settings
- **CORS** — Frontend URL allowed (default `http://localhost:3000`, overridable via `FrontendUrl` env var)
