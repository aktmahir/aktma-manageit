# Calendar Management System

This project is a simple ASP.NET Core MVC calendar application designed for local demonstration and portfolio review. It focuses on user management, authenticated access, event scheduling, and a lightweight admin workflow without introducing unnecessary infrastructure.

## What this project demonstrates

- ASP.NET Core MVC patterns in a single application
- Entity Framework Core with SQL Server
- Cookie-based authentication and role-based authorization
- A realistic but compact local data model for users and events
- Docker-based local startup for a quick repository clone-and-run workflow

## Architecture

```text
Browser
  |
  v
ASP.NET MVC app
  |
  v
SQL Server (Docker container)
```

This project intentionally keeps the local demo simple: there is one web application and one database. No production-only platform dependencies are required for local execution.

## Tech stack

### Frontend
- ASP.NET Core MVC Razor views
- Bootstrap-based UI

### Backend
- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server

### Local infrastructure
- Docker Compose
- SQL Server container

## Quick start

### Requirements

- Git
- Docker Desktop or Docker Engine
- Docker Compose

### Run locally

```bash
git clone <repository-url>
cd aktma-manageit
cp .env.example .env
docker compose up --build
```

Then open:

- Frontend: http://localhost:8081
- Health endpoint: http://localhost:8081/health

### Demo accounts

The app seeds a small set of demo users automatically when the database is initialized.

- Admin: admin@example.com / AdminPass123!
- Company owner: john@example.com / JohnPass123!
- Member: jane@example.com / JanePass123!

## Main workflow

The application is designed to demonstrate the core calendar workflow quickly:

1. Open the app at http://localhost:8080.
2. Sign in with one of the seeded users.
3. Review upcoming events on the home page.
4. Navigate to event management and admin features as allowed by role.
5. Use the seeded data to understand the calendar and authorization structure without manual setup.

## Project structure

```text
.
├── Controllers/             # MVC controllers
├── Data/                   # EF Core context
├── Migrations/             # Database migration files
├── Models/                 # Domain models and hashing logic
├── Properties/             # ASP.NET launch settings
├── Views/                  # Razor views
├── .env.example            # Local Docker environment template
├── .dockerignore           # Docker build exclusions
├── CalendarApp.csproj      # .NET project definition
├── Dockerfile              # Application container build
├── Program.cs              # App startup and database bootstrapping
├── README.md               # Developer documentation
├── docker-compose.yml      # Local demo infrastructure definition
├── smoke-test.sh           # Simple smoke test for the app
└── appsettings.json        # Default configuration
```

## Configuration

The local demo uses environment variables from `.env`. The values in `.env.example` are safe demo values and are meant for local use only.

Key configuration values:

- ASPNETCORE_ENVIRONMENT=Development
- ASPNETCORE_URLS=http://+:8080
- ConnectionStrings__DefaultConnection=Server=database,1433;Database=CalendarDb;...
- MSSQL_SA_PASSWORD=YourStrong!Passw0rd

## Database behavior

The app uses SQL Server in Docker and automatically applies pending migrations on startup. This keeps the local demo simple and removes the need for manual database setup.

## Smoke test

A simple smoke test is included:

```bash
chmod +x smoke-test.sh
./smoke-test.sh
```

The script checks the health endpoint and verifies that the app is serving the expected pages.

## Troubleshooting

### Ports already in use

If port 8080 or 1433 is already in use on your machine, edit the values in `.env` before starting the stack:

```bash
APP_PORT=8082
DB_PORT=1434
```

### Containers are not starting

View the container logs:

```bash
docker compose logs -f
```

### Reset local database state

If you want to reset the demo data completely:

```bash
docker compose down -v
docker compose up --build
```

This removes the local SQL Server volume and recreates the demo database.

## Notes

- This repository is focused on a local portfolio/demo experience, not production deployment.
- The setup intentionally avoids Kubernetes, cloud dependencies, and manual database configuration.
- The main goal is a clean, readable app that can be started within minutes using Docker.

## License

This project is distributed under the MIT License. See [LICENSE](LICENSE) for details.
