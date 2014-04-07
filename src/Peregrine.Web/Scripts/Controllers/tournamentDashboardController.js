angular
.module('peregrineUi.controllers')
.controller('tournamentDashboardController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'roundResource', 'standingsResource', 'playerResource',
	function($scope, $routeParams, $location, tournamentResource, roundResource, standingsResource, playerResource) {

		$scope.location = $location;
		var roundEventSource;
		var roundUpdatedHandler = function(event) {
			var round = JSON.parse(event.data);

			$scope.$apply(function() {
				$scope.round = round;
			})
		};

		var tournamentUrl = '/api/tournaments/' + $routeParams.tournamentKey + '/updates';
		var tournamentEventSource = new EventSource(tournamentUrl);
		var tournamentUpdatedHandler = function(event) {
			var tournament = JSON.parse(event.data);

			if(roundEventSource) {
				roundEventSource.removeEventListener('updated', roundUpdatedHandler);
			}

			if(tournament.activeRoundNumber) {
				var roundUrl = '/api/tournaments/' + tournament.key + '/rounds/' + tournament.activeRoundNumber + '/updates';
				roundEventSource = new EventSource(roundUrl);
				roundEventSource.addEventListener('updated', roundUpdatedHandler, false);
			}

			$scope.$apply(function() {
				$scope.tournament = tournament;
			})
		};
		tournamentEventSource.addEventListener('updated', tournamentUpdatedHandler, false);

		var standingsUrl = '/api/tournaments/' + $routeParams.tournamentKey + '/standings/updates';
		var standingsEventSource = new EventSource(standingsUrl);
		var standingsUpdatedHandler = function(event) {
			$scope.$apply(function() {
				$scope.standings = JSON.parse(event.data);
			})
		};
		standingsEventSource.addEventListener('updated', standingsUpdatedHandler, false);
	}
]);