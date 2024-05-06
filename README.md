# League of Legends Lobby Bot

## Overview
This Discord bot is designed to facilitate 5v5 custom games in League of Legends by providing a lobby system. It automates the process of shuffling teams based on players' preferred roles and ranking.

## Features
- Lobby creation: Start a new lobby for a 5v5 custom game.
- Role selection: Players can indicate their preferred roles for the game.
- Team shuffling: Automatically divides players into two teams based on their primary and secondary roles and rating.

## Commands
- `/start-lobby`: Starts a new lobby.
- `/join-lobby <rank> <role> <role>`: Allows a player to join the lobby with their preferred role and their rank.
- `/remove-player <name>`: Removes a player from the lobby based of his name.
- `/roll-teams`: Initiates team shuffling and starts the game.
- `/restart-lobby`: Restarts the lobby. Removes all players from the lobby.

## Setup
1. Clone this repository to your local machine.
2. Install the required dependencies
3. Create bot in the Discord development portal
4. Link bot via config.js file with your bot-key
5. Invite bot to your Discord server

## Contributing
Contributions are welcome! If you have any suggestions, feature requests, or bug reports, please open an issue on GitHub or submit a pull request.

## License
This project is licensed under the MIT License

## Author
Julius Schäfer
