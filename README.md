#Peregrine

Swiss-pairing and ranking management.

##API
###/event
POST: Creates a new event.

###/event/{key}
GET: Gets a summary of the event.

###/event/{id}/player/{name}
GET: Gets a summary of the player.

PUT: Adds a player to the event.

DELETE: Drops a player from the event.

###/event/{id}/round/{number}
GET: Gets a summary of the round.

###/event/{id}/round/{number}/game/{number}
GET: Gets a summary of the game.

PUT: Sets the results of the game.

###/event/{id}/round/{number}/pairings
GET: Gets the pairings for the round.

###/event/{id}/standings
GET: Gets the standings for the event.


