angular
.module('peregrineUi.controllers')
.controller('roundController', [
	'$scope', '$routeParams', '$location', 'roundResource', 'outcomeResource', 'playerResource',
	function($scope, $routeParams, $location, roundResource, outcomeResource, playerResource) {
		$scope.error = '';
		$scope.tournamentKey = $routeParams.tournamentKey;

		$scope.updateRound = function() {
			roundResource.get({ tournamentKey: $routeParams.tournamentKey, roundNumber: $routeParams.roundNumber },
			function success(round) {
				$scope.error = '';
				$scope.round = round;
				roundResource.query(
				{
					tournamentKey: $routeParams.tournamentKey
				},
				function success(rounds) {
					$scope.error = '';
					$scope.isAnotherRound = rounds.length > $scope.round.number;
				},
				function error() {
					$scope.error = 'We couldn\'t tell if there is another round or not.';
				});
			},
			function error() {
				$scope.error = 'We couldn\'t get your round';
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
					$scope.error = 'We were unable to save your match data.';
				});
		}

		$scope.dropPlayer = function(player) {
			if(confirm('Seriously?')) {
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

		$scope.go = function(path) {
			$location.path(path);
		};

		$scope.updateRound();
	}
]);
