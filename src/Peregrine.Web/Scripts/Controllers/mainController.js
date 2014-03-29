angular
.module('peregrineUi.controllers')
.controller('mainController', [
	'$scope', 'tournamentResource',
	function($scope, tournamentResource) {
		$scope.error = '';
		tournamentResource.query(
			{},
			function success(tournaments) {
				$scope.tournaments = tournaments;
				$scope.error = '';
			},
			function error() {
				$scope.error = 'We were unable to retrieve the tournament list';
			});
	}
]);
