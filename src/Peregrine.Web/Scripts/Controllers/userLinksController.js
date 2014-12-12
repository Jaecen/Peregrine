angular
.module('peregrineUi.controllers')
.controller('userLinksController', [
	'$scope', '$http', '$rootScope', '$location', 'authService',
	function ($scope, $http, $rootScope, $location, authService) {

		function updateLoginStatus() {
			$scope.loggedIn = (sessionStorage.getItem('accessToken') != null && sessionStorage.getItem('accessToken').length > 0);
			$scope.userName = sessionStorage.getItem('userName');
		}

		//check the login status on page load
		updateLoginStatus();
		
		//listen for the login event from other controllers
		$rootScope.$on('login', function () {
			updateLoginStatus();
		});

		//logout
		$scope.logOut = function () {
			authService.logOut()
			.then(
				function success(data, status, headers, config) {
					updateLoginStatus();
					$location.path('/');
				},
				function error(data, status, headers, config) {
					$scope.error = "We were unable to log you out";
				});
		}
	}
]);
