//define the main module
angular
.module('peregrineUi', [
	'ngRoute',
	'ngResource',
	'peregrineUi.controllers',
	'peregrineUi.directives',
	'peregrineUi.factories',
	'ui.bootstrap'
]).config(function ($routeProvider, $locationProvider) {
	$routeProvider
		.when('/', {
			controller: 'mainController',
			templateUrl: 'Partials/Home.html'
		})
		.when('/dash/tournament/:tournamentKey', {
			controller: 'tournamentDashboardController',
			templateUrl: 'Partials/TournamentDashboard.html'
		})
		.when('/tournamentedit', {
			controller: 'tournamentController',
			templateUrl: 'Partials/TournamentEdit.html'
		})
		.when('/tournamentedit/:tournamentKey', {
			controller: 'tournamentController',
			templateUrl: 'Partials/TournamentEdit.html'
		})
		.when('/tournament/:tournamentKey/round/:roundNumber', {
			controller: 'roundController',
			templateUrl: 'Partials/RoundDetail.html'
		})
		.when('/tournament/:tournamentKey/roundedit/:roundNumber', {
			controller: 'roundController',
			templateUrl: 'Partials/RoundEdit.html'
		})
		.when('/tournament/:tournamentKey/standings', {
			controller: 'standingsController',
			templateUrl: 'Partials/Standings.html'
		})
		.when('/secretadmin', {
			controller: 'adminController',
			templateUrl: 'Partials/Admin.html'
		})
		.when('/login', {
			controller: 'loginController',
			templateUrl: 'Partials/Login.html'
		})
		.otherwise({ redirectTo: '/' });
});

angular.module('peregrineUi.controllers', [
	'peregrineUi.resources',
	'peregrineUi.filters',
	'peregrineUi.factories'
]);

angular.module('peregrineUi.resources', [
]);

angular.module('peregrineUi.filters', [
]);

angular.module('peregrineUi.directives', [
]);

angular.module('peregrineUi.factories', [
]);
