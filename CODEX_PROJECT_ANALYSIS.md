I want you to thoroughly analyze and understand this existing project before making any changes.

Project technology:
- Backend: ASP.NET Core
- Database: Oracle Database
- Language: C#
- This is an existing/legacy project, so do not assume the architecture or coding conventions.

Your first task is ONLY to read and analyze the project. Do NOT modify, create, delete, rename, or refactor any files.

Please inspect the entire repository, including:
- Solution and project files (.sln, .csproj)
- Controllers / API endpoints
- Services and business logic
- Models, DTOs, ViewModels
- Entity Framework / ORM configuration
- Oracle database configuration and connection handling
- DbContext and database mappings
- Repositories and data-access code
- SQL queries, stored procedures, functions, packages, triggers, and views if they are included in the repository
- Dependency injection configuration
- Middleware
- Authentication and authorization
- Configuration files (appsettings*.json, environment configuration, etc.)
- Logging and exception handling
- Background jobs/services
- External API/service integrations
- Frontend code, if present
- Tests and test configuration
- Docker/container/deployment configuration, if present
- CI/CD configuration, if present
- NuGet packages and important dependencies

Pay particular attention to how the application communicates with Oracle:
1. How the Oracle connection is configured.
2. Which Oracle provider/ORM is being used.
3. How connections, transactions, and DbContexts are managed.
4. How queries are generated or executed.
5. Whether stored procedures/packages are used.
6. How parameters are passed.
7. How Oracle errors/exceptions are handled.
8. Any potential database-performance concerns you can identify.

Build a mental model of the application architecture and explain it to me.

After reading the project, provide a structured analysis with these sections:

1. Executive Summary
   - What this application appears to do.
   - Main business purpose.
   - Major components.

2. Technology Stack
   - .NET / ASP.NET Core version
   - C# version if identifiable
   - Oracle version/provider if identifiable
   - ORM/data-access technology
   - Important NuGet packages
   - Frontend technology if present

3. Project Structure
   - Explain the important folders/projects.
   - Explain the responsibility of each major component.

4. Architecture
   - Explain the architectural pattern being used.
   - Describe the request flow from API/controller to database.
   - Explain dependencies between layers/components.

5. Database Architecture
   - Explain how the application communicates with Oracle.
   - Identify DbContexts, repositories, SQL, stored procedures, packages, etc.
   - Identify important tables/entities if they can be determined from the code.
   - Explain transaction handling.

6. Authentication & Authorization
   - Explain how users authenticate.
   - Explain roles/claims/permissions.
   - Identify relevant middleware and configuration.

7. Important Business Logic
   - Identify the most important services/classes/methods.
   - Explain what they appear to do.
   - Trace important business workflows through the code.

8. API Endpoints
   - List important controllers/endpoints.
   - Explain their purpose.
   - Show the flow from endpoint -> service -> data access -> Oracle where possible.

9. Configuration
   - Explain important configuration settings.
   - Identify environment-specific configuration.
   - Do NOT expose or reproduce passwords, secrets, API keys, connection strings, or other sensitive values. If found, only identify the configuration key/name.

10. Error Handling & Logging
    - Explain how exceptions are handled.
    - Explain logging strategy.
    - Identify potential weaknesses.

11. Potential Problems
    - Identify bugs or suspicious code.
    - Identify security concerns.
    - Identify Oracle/database performance concerns.
    - Identify maintainability problems.
    - Clearly distinguish confirmed problems from things that require further investigation.

12. Testing
    - Explain existing tests.
    - Identify what is and isn't covered.

13. Important Files
    - Give me a prioritized list of files I should understand first.
    - Explain why each file is important.

14. Application Flow
    - Give me 2-3 concrete examples of important user/API operations.
    - Trace each operation through the code from request to Oracle and back.

15. Questions / Unknowns
    - List anything you could not determine from the repository.
    - Do not guess. Clearly mark unknowns.

Finally, create a concise "Project Knowledge Map" that I can use in future prompts.

IMPORTANT RULES:
- Do not modify any files during this analysis.
- Do not generate code yet.
- Do not refactor anything.
- Do not make assumptions when the source code can answer the question.
- When making an architectural claim, reference the relevant file/class/method.
- If something is unclear, say that it is unclear and explain what would need to be inspected.
- Never expose secrets, passwords, tokens, or private credentials.
- Take enough time to inspect the repository thoroughly rather than only looking at the README or a few files.

At the end, tell me:
"Project analysis complete. I am ready for your next task."
