angular
.module('peregrineUi.controllers')
.controller('tournamentDashboardController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'roundResource', 'standingsResource', 'playerResource',
	function($scope, $routeParams, $location, tournamentResource, roundResource, standingsResource, playerResource) {

		$scope.location = $location;

		tournamentResource.get(
			{ tournamentKey: $routeParams.tournamentKey },
			function success(tournament) {
				$scope.error = '';
				//handle redirect if the tournament is complete already
				if(tournament.finished) {
					$location.path('/tournament/' + tournament.key + '/standings');
				}

				$scope.tournament = tournament;

				roundResource.get({
						tournamentKey: $routeParams.tournamentKey,
						roundNumber: tournament.activeRoundNumber 
					},
					function success(round) {
						$scope.round = round;
					},
					function error() {
						$scope.error = 'Could not load the current round.';
					});

				standingsResource.get(
					{ tournamentKey: $routeParams.tournamentKey },
					function success(standings) {
						$scope.standings = standings;
					},
					function error() {
						$scope.error = 'Could not load the current standings.';
					});

			},
			function error() {
				$scope.error = 'Sunova gunnysack! We lost track of your tournament.';
			});
	}
]);