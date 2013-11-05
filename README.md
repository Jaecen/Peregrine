#Peregrine

Swiss-pairing and ranking management.

##API
###/tournament
*	**POST**

	Creates a new tournament. Returns a summary of the tournament with a "key" property. Use this value to access subresources of the tournament. This method will always succeed.

###/tournament/*{key}*
*	**GET**

	Returns a summary of the tournament.

	Returns a 404 if no tournament with the provided key exists.

###/tournament/*{key}*/player/*{name}*
*	**GET** 

	Returns a summary of the player.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no player with the provided name exists.

*	**PUT**
	
	Adds a player to the tournament. Players can not be added after the first game result is entered.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 409 if a player with the provided name already exists.

*	**DELETE**

	Drops a player from the tournament. If no game results have been recorded, this simply deletes the player from the tournament.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no player with the provided name exists.

###/tournament/*{key}*/round/*{number}*
*	**GET**

	Returns a summary of the round.

###/tournament/*{key}*/round/*{number}*/game/*{number}*
*	**GET**

	Returns a summary of the game.

*	**PUT**

	Sets the results of the game. Results for a previous round can not be changed once a new round has started.

###/tournament/*{key}*/round/*{number}*/pairings
*	**GET**

	Returns the pairings for the round.

###/tournament/*{key}*/standings
*	**GET**

	Returns the current standings for the tournament.


