angular
.module('peregrineUi.controllers')
.controller('adminController', [
	'$scope', 'tournamentResource',
	function($scope, tournamentResource) {
		$scope.error = '';

		$scope.updateTournaments = function () {
			tournamentResource.query(
			{},
			function success(tournaments) {
				$scope.tournaments = tournaments;
				$scope.error = '';
			},
			function error() {
				$scope.error = 'We were unable to retrieve the tournament list';
			});
		};

		$scope.deleteTournament = function (tournamentKey) {
			tournamentResource.delete(
				{
					tournamentKey: tournamentKey
				},
				function success() {
					$scope.error = '';
					$scope.updateTournaments();
				},
				function error() {
					$scope.error = 'Sorry that tournament is indestrutible. Try a different one.';
				});
		}

		$scope.updateTournaments();
	}
]);
