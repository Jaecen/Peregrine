#Peregrine

Swiss-pairing and ranking management.

##API
###/tournament
POST: Creates a new tournament.

###/tournament/{key}
GET: Gets a summary of the tournament.

###/tournament/{id}/player/{name}
GET: Gets a summary of the player.

PUT: Adds a player to the tournament.

DELETE: Drops a player from the tournament.

###/tournament/{id}/round/{number}
GET: Gets a summary of the round.

###/tournament/{id}/round/{number}/game/{number}
GET: Gets a summary of the game.

PUT: Sets the results of the game.

###/tournament/{id}/round/{number}/pairings
GET: Gets the pairings for the round.

###/tournament/{id}/standings
GET: Gets the standings for the tournament.


