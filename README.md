# chess-backend
This is a chess backend microservice built using C#, ASP\.NET Core, PostgreSQL,
and Docker. A simple frontend (`frontend/`) has been created which consumes the
API. The frontend is served using an Apache HTTP server.

## API
The API consists of HTTP endpoints for creating and retrieving chess games.
WebSocket endpoints allow a user to connect to an existing game (max 2
connections) and play chess in real-time.

### HTTP Endpoints
|Method|Path                |Description                 |
|------|--------------------|----------------------------|
|GET   |/api/chessgames     |Get all existing chess games|
|GET   |/api/chessgames/{id}|Get chess game with id      |
|POST  |/api/chessgames     |Create new chess game       |

### WebSocket Endpoints
|Path               |Description                                                                                |
|-------------------|-------------------------------------------------------------------------------------------|
|/ws/chessgames/{id}|Connect to chess game with id.<br>First connection is white player, second is black player.|

### WebSocket Message Requests
|Request Type|Description|
|------------|-----------|
|0           |Make move  |
|1           |Get moves  |
|2           |Get board  |

#### Get board
Send:
```
{"RequestType": 2}
```

Receive:
```
{ "Tiles": [ { "Index": 0, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "R" } }, { "Index": 1, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "N" } }, { "Index": 2, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "B" } }, { "Index": 3, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "Q" } }, { "Index": 4, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "K" } }, { "Index": 5, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "B" } }, { "Index": 6, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "N" } }, { "Index": 7, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "R" } }, { "Index": 8, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 9, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 10, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 11, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 12, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 13, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 14, "Color": 1, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 15, "Color": 0, "Piece": { "Color": 1, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "P" } }, { "Index": 16, "Color": 0, "Piece": null }, { "Index": 17, "Color": 1, "Piece": null }, { "Index": 18, "Color": 0, "Piece": null }, { "Index": 19, "Color": 1, "Piece": null }, { "Index": 20, "Color": 0, "Piece": null }, { "Index": 21, "Color": 1, "Piece": null }, { "Index": 22, "Color": 0, "Piece": null }, { "Index": 23, "Color": 1, "Piece": null }, { "Index": 24, "Color": 1, "Piece": null }, { "Index": 25, "Color": 0, "Piece": null }, { "Index": 26, "Color": 1, "Piece": null }, { "Index": 27, "Color": 0, "Piece": null }, { "Index": 28, "Color": 1, "Piece": null }, { "Index": 29, "Color": 0, "Piece": null }, { "Index": 30, "Color": 1, "Piece": null }, { "Index": 31, "Color": 0, "Piece": null }, { "Index": 32, "Color": 0, "Piece": null }, { "Index": 33, "Color": 1, "Piece": null }, { "Index": 34, "Color": 0, "Piece": null }, { "Index": 35, "Color": 1, "Piece": null }, { "Index": 36, "Color": 0, "Piece": null }, { "Index": 37, "Color": 1, "Piece": null }, { "Index": 38, "Color": 0, "Piece": null }, { "Index": 39, "Color": 1, "Piece": null }, { "Index": 40, "Color": 1, "Piece": null }, { "Index": 41, "Color": 0, "Piece": null }, { "Index": 42, "Color": 1, "Piece": null }, { "Index": 43, "Color": 0, "Piece": null }, { "Index": 44, "Color": 1, "Piece": null }, { "Index": 45, "Color": 0, "Piece": null }, { "Index": 46, "Color": 1, "Piece": null }, { "Index": 47, "Color": 0, "Piece": null }, { "Index": 48, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 49, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 50, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 51, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 52, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 53, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 54, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 55, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "p" } }, { "Index": 56, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "r" } }, { "Index": 57, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "n" } }, { "Index": 58, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "b" } }, { "Index": 59, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "q" } }, { "Index": 60, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "k" } }, { "Index": 61, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "b" } }, { "Index": 62, "Color": 1, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "n" } }, { "Index": 63, "Color": 0, "Piece": { "Color": 0, "HasMoved": false, "IsEnpassantable": false, "CharRepresentation": "r" } } ], "IsCheckmate": false, "Turn": 1 }
```

#### Get moves
Send:
```
{"RequestType": 1, "Tile": "a2"}
```

Receive:
```
[ { "Src": 8, "Dst": 16 }, { "Src": 8, "Dst": 24 } ]
```

#### Make move
Send:
```
{ "RequestType": 0, "Move": { "Src": 8, "Dst": 24 } }
```

Receive (broadcast): Same response format as getting the board (See above).


## Docker services
To run the application as a whole:

`docker compose up frontend`

Alternatively, to run individual services:
|Service     |Description                                                 |
|------------|------------------------------------------------------------|
|backend     |Run backend, database, and applies migrations               |
|build       |Build the project                                           |
|chess-db    |Runs the postgreSQL database                                |
|format-check|Checks the format of `.cs` files                            |
|frontend    |Run the frontend, backend, database, and applies migrations |
|migrations  |Applies migrations                                          |
|test        |Run tests                                                   |

These services are also used for CI with Github Actions. (See,
`.github/workflows/chess-backend.yaml`)

## Important URLs
* http://localhost:8000 - frontend
* http://localhost:8080 - backend
