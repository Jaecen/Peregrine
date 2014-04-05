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

		var standingsUrl = '/api/tournaments/' + $routeParams.tournamentKey + '/standings/updates';
		roundEventSource = new EventSource(standingsUrl);
		roundEventSource.addEventListener(
			'updated',
			function(event) {
				$scope.$apply(function() {
					$scope.standings = JSON.parse(event.data);
				})
			},
			false);
	}
]);