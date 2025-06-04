# Tanks-Online Backend

Backend service for Tanks-Online game, providing API endpoints and real-time communication features for multiplayer tank battles.

## Technology Stack

- **Java 24**: Core programming language
- **Spring Boot 3.5.0**: Application framework
- **Spring Security**: Authentication and authorization
- **Spring Data JPA**: Database access and ORM
- **WebSockets**: Real-time communication
- **PostgreSQL**: Database
- **Flyway**: Database migrations
- **JWT**: Token-based authentication
- **Caffeine**: In-memory caching
- **Swagger/OpenAPI**: API documentation
- 
## Getting Started

### Prerequisites

- JDK 24+
- PostgreSQL 
- Docker (optional, for containerized deployment)

### Configuration

The application can be configured through environment variables or the `application.yml` file:

```yaml
spring:
  datasource:
    url: ${DATABASE_URL:jdbc:postgresql://localhost:5432/game_db}
    username: ${DATABASE_USERNAME:admin}
    password: ${DATABASE_PASSWORD:password}
```

Key environment variables:
- `DATABASE_URL`: PostgreSQL connection URL (default: `jdbc:postgresql://localhost:5432/game_db`)
- `DATABASE_USERNAME`: Database user (default: `admin`)
- `DATABASE_PASSWORD`: Database password (default: `password`)
- `JWT_SECRET`: Secret key for JWT token generation

### Building the Application

```bash
./gradlew build
```

### Running the Application

```bash
./gradlew bootRun
```

### Using Docker

```bash
docker-compose up
```

## API Documentation

API documentation is available through Swagger UI after starting the application:

[Swagger UI](http://localhost:8080/swagger-ui.html)

## Main Features

The backend provides the following main features:

### Authentication and User Management

- User registration and login
- JWT-based authentication
- User profile management

### Game Management

- Create and join game sessions
- Real-time game state updates via WebSockets
- Player statistics tracking

### Chat System

- In-game chat functionality
- Message history

### Administration

- User management
- Game session monitoring

## Database Schema

Below is the Entity-Relationship Diagram of the database:

![ER Diagram](/src/main/resources/documentation/ERDiagram.png)

## Project Structure

```
src/
├── main/
│   ├── java/hr/antitalent/tanks_backend/
│   │   ├── configurations/ - Application configurations
│   │   ├── controllers/    - REST API endpoints
│   │   ├── domain/         - Entity models
│   │   ├── dto/            - Data Transfer Objects
│   │   ├── enums/          - Enumeration classes
│   │   ├── exceptions/     - Custom exceptions
│   │   ├── filters/        - Request filters
│   │   ├── repositories/   - Data access layer
│   │   ├── services/       - Business logic
│   │   ├── websocket/      - WebSocket handlers
│   │   └── TanksBackendApplication.java - Application entry point
│   └── resources/
│       ├── application.yml - Application configuration
│       ├── db/migration/   - Flyway database migrations
│       └── documentation/  - Project documentation
```

## API Controllers

- **AuthController**: Authentication endpoints (login, register)
- **UserController**: User management endpoints
- **GameController**: Game-related operations
- **GameSessionController**: Game session management
- **GameSessionPlayerController**: Player-specific operations within game sessions
- **ChatController**: Chat functionality
- **AdminController**: Administrative operations

## Future Development

> **Note:** This project is still under development. The next phases will include:

- Refactoring to Domain-Driven Design (DDD) architecture using Spring Modulith
- Implementing an event-driven system for better component decoupling
- Enhancing concurrency handling for improved performance in multiplayer scenarios
- Might finish if I find time, but it is not a priority at the moment

## License

Apache License 2.0

## Contributors

Antonio Omazić

