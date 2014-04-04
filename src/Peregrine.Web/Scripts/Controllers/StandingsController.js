angular
.module('peregrineUi.controllers')
.controller('standingsController', [
	'$scope', '$routeParams', 'standingsResource',
	function($scope, $routeParams, standingsResource) {
		$scope.standings = standingsResource.get({ tournamentKey: $routeParams.tournamentKey });
	}
]);
