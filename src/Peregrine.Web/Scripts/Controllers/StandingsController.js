angular
.module('peregrineUi.controllers')
.controller('standingsController', [
	'$scope', '$routeParams', 'standingsResource',
	function($scope, $routeParams, standingsResource) {
		console.log('Loading standings', $routeParams.tournamentKey);

		$scope.maxMatchPoints = 0;
		$scope.maxOpponentsMatchWinPercentage = 0;
		$scope.maxGameWinPercentage = 0;
		$scope.maxOpponentsGameWinPercentage = 0;

		$scope.standings = standingsResource.query({ tournamentKey: $routeParams.tournamentKey }, function(value) {
			value.forEach(function(standing) {
				$scope.maxMatchPoints = Math.max($scope.maxMatchPoints, standing.matchPoints);
				$scope.maxOpponentsMatchWinPercentage = Math.max($scope.maxOpponentsMatchWinPercentage, standing.opponentsMatchWinPercentage);
				$scope.maxGameWinPercentage = Math.max($scope.maxGameWinPercentage, standing.gameWinPercentage);
				$scope.maxOpponentsGameWinPercentage = Math.max($scope.maxOpponentsGameWinPercentage, standing.opponentsGameWinPercentage);
			})
		});
	}
]);
