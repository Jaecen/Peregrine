angular
.module('peregrineUi.controllers')
.controller('tournamentController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'playerResource', 'activeRoundResource',
	function($scope, $routeParams, $location, tournamentResource, playerResource, activeRoundResource) {
		$scope.players = []; //necessary?
		$scope.error = '';
		$scope.newPlayer = {};

		$scope.$watch('players', function(newValue) {
			if(newValue) {
				newValue.sort(comparePlayer);
			}
		}, true);

		if($routeParams.tournamentKey) {
			//get existing tournament
			tournamentResource.get(
				{ tournamentKey: $routeParams.tournamentKey },
				function success(tournament) {
					$scope.error = '';
					//handle redirects
					if(tournament.finished) {
						$location.path('/tournament/' + tournament.key + '/standings');
					}
					else if(tournament.started) {
						$location.path('/tournament/' + tournament.key + '/round/1');
					}
					//add the tournament to the scope
					$scope.tournament = tournament;
					//get players
					playerResource.query({ tournamentKey: tournament.key },
						function success(players) {
							$scope.error = '';
							$scope.players = players;
						},
						function error() {
							$scope.error = 'Sorry =( We failed to load your players.';
						});
				},
				function error() {
					$scope.error = 'Sorry =( We were unable to find your tournament.';
				});
		}
		else {
			//create a tournament
			$scope.tournament = tournamentResource.create(
				{},
				function() {
					$scope.error = '';
				},
				function() {
					$scope.error = 'We were unable to create a tournament.';
				});
		}

		$scope.updateTournament = function() {
			$scope.tournament.$save({}, function() {
				$scope.tournament.$get();
			});
		}

		$scope.addPlayer = function(newplayer) {
			if(!newplayer) {
				newplayer = $scope.newPlayer;
			}
			if(newplayer.name && newplayer.name.length > 0 && $scope.tournament.key) {
				playerResource.save(
					{ tournamentKey: $scope.tournament.key },
					newplayer,
					function success() {
						$scope.error = '';
						//get players
						playerResource.query({ tournamentKey: $scope.tournament.key },
							function success(players) {
								$scope.error = '';
								$scope.players = players;
							},
							function error(players) {
								$scope.error = 'Sorry =( We failed to load your players.';
							});
						$scope.newPlayer = {};
					},
					function error(response) {
						if(response && response.status === 409) {
							$scope.error = 'Sorry a player with the same name already exists.'
						}
						else {
							$scope.error = 'We were unable to add the player!';
						}
					});
			}
		};

		$scope.deletePlayer = function(player) {
			playerResource.delete(
				{ tournamentKey: $scope.tournament.key, playerName: player.name },
				function() {
					$scope.error = '';
					//get players
					playerResource.query({ tournamentKey: $scope.tournament.key },
						function success(players) {
							$scope.error = '';
							$scope.players = players;
						},
						function error(players) {
							$scope.error = 'Sorry =( We failed to load your players.';
						});
				},
				function() {
					$scope.error = 'We were unable to delete the player!';
				});
		};

		$scope.setActiveRound = function(tournamentKey, roundNumber) {
			activeRoundResource.set(
				{ tournamentKey: tournamentKey }, 
				{ roundNumber: roundNumber },
				function() {
					var path = '/tournament/' + tournamentKey + '/roundedit/' + roundNumber;
					$location.path(path);
				}
			);
		}

		function comparePlayer(playerOne, playerTwo) {
			if(playerOne.name < playerTwo.name)
				return -1;
			if(playerOne.name > playerTwo.name)
				return 1;
			return 0;
		}
	}
]);

