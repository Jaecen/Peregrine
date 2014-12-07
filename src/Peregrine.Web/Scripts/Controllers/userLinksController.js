angular
.module('peregrineUi.controllers')
.controller('userLinksController', [
	'$scope', '$http', '$rootScope',
	function ($scope, $http, $rootScope) {

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
		$scope.logout = function () {
			$http({
				method: 'POST',
				url: '/api/account/logout',
				headers: { 'Content-Type': 'application/json' }
			})
			.success(function (data, status, headers, config) {
				sessionStorage.removeItem('accessToken');
				sessionStorage.removeItem('userName');
				updateLoginStatus();
			}).error(function (data, status, headers, config) {
				$scope.error = "We were unable to log you out";
			})
		}
	}
]);
