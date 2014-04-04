angular
.module('peregrineUi.controllers')
.controller('tournamentDashboardController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'roundResource', 'standingsResource', 'playerResource',
	function($scope, $routeParams, $location, tournamentResource, roundResource, standingsResource, playerResource) {

		$scope.location = $location;

		var tournamentEventSource;
		var roundEventSource;

		$scope.tournament = tournamentResource.get({
			tournamentKey: $routeParams.tournamentKey
		});

		$scope.tournament.$promise
			.then(function(tournament) {
				var url = '/api/tournaments/' + $routeParams.tournamentKey + '/updates';
				console.log('listening for tournament updates', url);

				tournamentEventSource = new EventSource(url);
				tournamentEventSource.addEventListener(
					'updated',
					function(event) {
						console.log('tournament update', event);
						$scope.$apply(function() {
							$scope.tournament = JSON.parse(event.data);
						})
					},
					false);
			});

		$scope.round = roundResource.get({
			tournamentKey: $routeParams.tournamentKey,
			roundNumber: 'current'
		});

		$scope.round.$promise
			.then(function(round) {
				var url = '/api/tournaments/' + $routeParams.tournamentKey + '/rounds/' + round.number + '/updates';
				console.log('listening for round updates', url);

				roundEventSource = new EventSource(url);
				roundEventSource.addEventListener(
					'updated',
					function(event) {
						console.log('round update', event);
						$scope.$apply(function() {
							$scope.round = JSON.parse(event.data);
						})
					},
					false);

			});

		standingsResource.query({
				tournamentKey: $routeParams.tournamentKey
			}).$promise
			.then(function(standings) {
				// Track the max value for each metric so we can graph each player's stats against it.
				var maxMatchPoints = 0;
				var maxOpponentsMatchWinPercentage = 0;
				var maxGameWinPercentage = 0;
				var maxOpponentsGameWinPercentage = 0;

				//for(var index = 0; index < standings.length; index++)
				standings.forEach(function(standing) {
					maxMatchPoints = Math.max(maxMatchPoints, standing.matchPoints);
					maxOpponentsMatchWinPercentage = Math.max(maxOpponentsMatchWinPercentage, standing.opponentsMatchWinPercentage);
					maxGameWinPercentage = Math.max(maxGameWinPercentage, standing.gameWinPercentage);
					maxOpponentsGameWinPercentage = Math.max(maxOpponentsGameWinPercentage, standing.opponentsGameWinPercentage);
				});

				standings.forEach(function(standing) {
					standing.percents = {
						matchPoints: standing.matchPoints / maxMatchPoints,
						opponentsMatchWinPercentage: standing.opponentsMatchWinPercentage / maxOpponentsMatchWinPercentage,
						gameWinPercentage: standing.gameWinPercentage / maxGameWinPercentage,
						opponentsGameWinPercentage: standing.opponentsGameWinPercentage / maxOpponentsGameWinPercentage
					};
				});

				$scope.standings = standings;
			});
	}
]);