angular
.module('peregrineUi.controllers')
.controller('mainController', [
	'$scope', '$location', 'tournamentResource',
	function ($scope, $location, tournamentResource) {
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

		$scope.goToTournament = function () {
			if ($scope.searchTournamentCode) {
				tournamentResource.get({ tournamentKey: $scope.searchTournamentCode },
				function success(tournament) {
					//redirect to the tournament if it exists
					$location.path('/tournamentedit/' + tournament.key);
				},
				function error() {
					$scope.error = 'So sorry, but that is not a valid tournament code.';
				});
			}
			else {
				$scope.error = 'Please enter a tournament code';
			}
		};
	}
]);
