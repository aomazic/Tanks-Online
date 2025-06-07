# ATA Systems(Learning Project)

A learning project to build a multiplayer game system with a Spring Boot backend and Unity frontend.

![Project Status: In Development](https://img.shields.io/badge/status-in%20development-yellow)

## Learning Goals & Purpose

This project serves as a personal learning platform to explore:

- Spring Boot backend development
- Unity client-server integration patterns
- Real-time communication with WebSockets
- Authentication and session management for games
- Multiplayer game architecture principles

The choice of Spring Boot was deliberate to enhance my Spring skills as working spring professionally can be stale and repetitive.

## Project Structure

This repository consists of two main components:

1. **[TanksGame](/TanksGame/README.md)** - Unity-based front end game client
2. **[tanks_backend](/tanks_backend/README.md)** - Spring Boot backend service

See the respective README files for specific details about each component.

## Current Development Status

This project is in early development as a learning exercise. Key components implemented so far:
- User authentication system
- Real-time WebSocket communication
- Game session management
- Basic client-server interaction

## Technical Focus Areas

- **Backend**: Spring Boot, WebSockets, JPA, Security
- **Frontend**: Unity, C#, .NET
- **Communication**: REST APIs and WebSocket protocols
- **Architecture**: Service-oriented design with event-based communication

## Getting Started

Please refer to the individual README files for the [game client](/TanksGame/README.md) and [backend service](/tanks_backend/README.md) for specific setup instructions.

## About the Game

- Tanks-Online is a cooperative PVE tower defense bullet hell multiplayer game where players control tanks to defend against waves of enemies. For detailed game information, see the [TanksGame README](/TanksGame/README.md).
- Game builds as available for download in the [TanksGame builds](/TanksGame/builds) directory (needs backend running locally on 8080).

## Learning Outcomes (So Far)

- Implementation of WebSocket handlers for real-time game state synchronization
- Implementing Game session chat functionality using WebSockets
- Creation of a JWT-based authentication system for game clients
- Development of session management for multiplayer coordination
- Unity client integration with Spring Boot backend services

## Future Learning Directions
- Actually implementing the game mechanics so its somewhat playable :)

## License

This project is licensed under the Apache License 2.0 - see the LICENSE file for details.

## Contributors

Antonio Omazić
