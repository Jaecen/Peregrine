#Peregrine

Swiss-pairing and ranking management.

##API

###/tournaments
*	**GET**

	Returns a list of tournament keys. This is intended as an administrative function. 

*	**POST**

	Creates a new tournament. Returns a summary of the tournament with a "key" property. Use this value to access subresources of the tournament.

###/tournaments/*{key}*
*	**GET**

	Returns a summary of the tournament.

	Returns a 404 if no tournament with the provided key exists.

*	**DELETE**

	Deletes the indicated tournament. There is no way to undo this operation.

	Returns a 204 (No Content) on success.

	Returns a 404 if no tournament with the provided key exists.

###/tournaments/*{key}*/players
*	**GET** 

	Returns a list of the names of players in the tournament.

	Returns a 404 if no tournament with the provided key exists.

###/tournaments/*{key}*/players/*{name}*
*	**GET** 

	Returns a summary of the player.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no player with the provided name exists.

*	**PUT**
	
	Adds a player to the tournament.

	Returns a 405 (Method Not Allowed) if attempting to add a player after the first game result is entered.

	Returns a 409 (Conflict) if a player with the provided name already exists.

	Returns a 404 if no tournament with the provided key exists.

*	**DELETE**

	Drops a player from the tournament. If no game results have been recorded, this simply deletes the player from the tournament.

	Returns a 204 (No Content) if the player is deleted.

	Returns a 200 and the details of the player if the player is dropped.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no player with the provided name exists.

###/tournaments/*{key}*/rounds
*	**GET**

	Returns a list of rounds in the tournament.

	Returns a 404 if no tournament with the provided key exists.

###/tournaments/*{key}*/rounds/*{number}*
*	**GET**

	Returns a summary of the round. The first round will continuously regenerated until results are entered.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

###/tournaments/*{key}*/rounds/*{number}*/*{player}*/win/{count}
*	**PUT**

	Sets the win results in the round for the provided player. Results for a previous round can not be changed once a new round has started. Returns a summary of the match the player participated in.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

	Returns a 404 if no player with the provided name exists.

	Returns a 404 if no there were no matches for the given player in the given round.

###/tournaments/*{key}*/rounds/*{number}*/*{player}*/draw/{count}
*	**PUT**

	Set the draw results in the round for the match the provided player was in. A draw result only needs to submitted for one player in the match. Draw results are recorded against both players in the match. Results for a previous round can not be changed once a new round has started. Returns a summary of the match the player participated in.

	Returns a 404 if no tournament with the provided key exists.

	Returns a 404 if no round with the provided number exists.

	Returns a 404 if no player with the provided name exists.

	Returns a 404 if no there were no matches for the given player in the given round.

###/tournaments/*{key}*/standings
*	**GET**

	Returns the current standings for the tournament.

###/tournaments/*{key}*/rounds/*{number}*/standings
*	**GET**

	Returns the standings at the specified round.
	
##ToDo/Questions
*	Change draw and win to their plural form in the API on the MatchResultController
*	Add a dropped attribute to the player object on a match so we can indicate it and disable the player
*	When we drop a player and the round as already got results recorded should we automatically add two wins to the opponent?