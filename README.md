#Peregrine

Swiss-pairing and ranking management.

##API
Part of the goal of this API is to explore HATEOAS in a JSON context. For the purposes of this API, discoverability boils down to two facts: what resources are related to the current resource and what HTTP methods are available on the current resource.

To this end, each resource must meet the following requirements:

*	Each response must contain a Link header containing all resources related to the requested resource.
*	Each response must contain an Allow header containing all supported HTTP methods on the requested resource.
*	Each resources must support an OPTIONS request.
*	Each resource must provide a 406 Not Acceptable if the requested HTTP method is not supported.

###/tournament
*	**POST**

	Creates a new tournament. Returns a summary of the tournament with a "key" property. Use this value to access subresources of the tournament. This method will always succeed.

###/tournament/*{key}*
*	**GET**

	Returns a summary of the tournament.

	Returns a 404 if no tournament with the provided key exists.

###/tournament/*{key}*/players
*	**GET** 

	Returns a list of players in the tournament.

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

###/tournament/*{key}*/rounds
*	**GET**

	Returns a list of rounds in the tournament.

	Returns a 404 if no tournament with the provided key exists.

###/tournament/*{key}*/round/*{number}*
*	**GET**

	Returns a summary of the round. The first round will continuously regenerated until results are entered.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

###/tournament/*{key}*/round/*{number}*/*{player}*/win
*	**POST**

	Adds a win result in the round for the provided player. Results for a previous round can not be changed once a new round has started. Returns a summary of the match the player participated in.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

	Returns a 404 if no player with the provided name exists.

###/tournament/*{key}*/round/*{number}*/*{player}*/draw
*	**POST**

	Adds a draw result in the round for the provided player. Only one draw result needs to be submitted for a game. Results for a previous round can not be changed once a new round has started. Returns a summary of the match the player participated in.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

	Returns a 404 if no player with the provided name exists.

###/tournament/*{key}*/round/*{number}*/matches
*	**GET**

	Returns a list of the matches for the given round.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

###/tournament/*{key}*/round/*{number}*/match/*{number}*
*	**GET**

	Returns a summary of the match.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

	Returns a 404 if no match with the provided number exists.

###/tournament/*{key}*/round/*{number}*/pairings
*	**GET**

	Returns the pairings for the round.

###/tournament/*{key}*/standings
*	**GET**

	Returns the current standings for the tournament.


