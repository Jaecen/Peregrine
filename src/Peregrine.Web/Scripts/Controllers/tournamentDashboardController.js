angular
.module('peregrineUi.controllers')
.controller('tournamentDashboardController', [
	'$scope', '$routeParams', '$location', 'tournamentResource', 'roundResource', 'standingsResource', 'playerResource',
	function($scope, $routeParams, $location, tournamentResource, roundResource, standingsResource, playerResource) {

		$scope.location = $location;

		var tournamentEventSource;
		var roundEventSource;

		var tournamentUrl = '/api/tournaments/' + $routeParams.tournamentKey + '/updates';
		tournamentEventSource = new EventSource(tournamentUrl);
		tournamentEventSource.addEventListener(
			'updated',
			function(event) {
				$scope.$apply(function() {
					$scope.tournament = JSON.parse(event.data);
				})
			},
			false);

		var roundUrl = '/api/tournaments/' + $routeParams.tournamentKey + '/rounds/1/updates';
		roundEventSource = new EventSource(roundUrl);
		roundEventSource.addEventListener(
			'updated',
			function(event) {
				$scope.$apply(function() {
					$scope.round = JSON.parse(event.data);
				})
			},
			false);

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