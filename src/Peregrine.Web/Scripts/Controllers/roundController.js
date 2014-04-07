angular
.module('peregrineUi.controllers')
.controller('roundController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'roundResource', 'outcomeResource', 'playerResource', 'activeRoundResource',
	function($scope, $routeParams, $location, tournamentResource, roundResource, outcomeResource, playerResource, activeRoundResource) {
		$scope.error = '';

		tournamentResource.get(
			{ tournamentKey: $routeParams.tournamentKey },
			function success(tournament) {
				$scope.error = '';
				//handle redirect if the tournament is complete already
				if (tournament.finished) {
					$location.path('/tournament/' + tournament.key + '/standings');
				}
				$scope.tournament = tournament;
			},
			function error() {
				$scope.error = 'Sunova gunnysack! We lost track of your tournament.';
			});

		$scope.updateRound = function() {
			roundResource.get({ tournamentKey: $routeParams.tournamentKey, roundNumber: $routeParams.roundNumber },
			function success(round) {
				$scope.error = '';
				//handle redirect if this round is complete already
				if (round.final) {
					$location.path('/tournament/' + $routeParams.tournamentKey + '/round/' + (round.number + 1));
				}
				$scope.round = round;
			},
			function error() {
				$scope.error = 'This round is off to a bad start. It got stuck in the tubes.';
			});
		};

		$scope.updatePlayerOutcome = function(player, outcome) {
			var apiOutcome = ((outcome === 'wins') ? 'wins' : 'draws');
			if(isNaN(Number(player[outcome])) || player[outcome] === "") {
				return;
			}
			outcomeResource.save(
				{
					tournamentKey: $routeParams.tournamentKey,
					roundNumber: $routeParams.roundNumber,
					playerName: player.name,
					outcome: apiOutcome,
					number: player[outcome]
				},
				{},
				function success() {
					$scope.error = '';
					$scope.updateRound();
				},
				function error() {
					$scope.error = 'Ahh jeez. You clicked so hard you broke it!';
				});
		}

		$scope.dropPlayer = function(player) {
			if(confirm('Whoa there! Seriously?')) {
				playerResource.delete(
					{
						tournamentKey: $routeParams.tournamentKey,
						playerName: player.name
					},
					function success() {
						$scope.error = '';
						$scope.updateRound();
					},
					function error() {
						$scope.error = 'We were unable to save your match data.';
					});
			}
		}

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

		$scope.go = function(path) {
			$location.path(path);
		};

		$scope.updateRound();
	}
]);
