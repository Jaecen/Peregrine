angular
.module('peregrineUi.controllers')
.controller('mainController', [
	'$scope', '$location', 'tournamentResource', 'authService',
	function ($scope, $location, tournamentResource, authService) {
		$scope.error = '';
		$scope.loggedIn = authService.isLoggedIn()
		$scope.loading = true;
		tournamentResource.query(
			{},
			function success(tournaments) {
				$scope.tournaments = tournaments;
				$scope.error = '';
				$scope.loading = false;
			},
			function error() {
				$scope.error = 'We were unable to retrieve the tournament list';
				$scope.loading = false;
			});
	}
]);
