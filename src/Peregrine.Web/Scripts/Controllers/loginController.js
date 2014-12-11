angular
.module('peregrineUi.controllers')
.controller('loginController', [
	'$scope', '$location', '$rootScope', 'authService',
	function ($scope, $location, $rootScope, authService) {
		$scope.error = '';

		$scope.registration = {
			userName: "",
			password: ""
		};

		$scope.loginData = {
			userName: "",
			password: ""
		};

		$scope.loginClick = function () {
			authService.login($scope.loginData)
				.then(function (response) {
					//tell the userlinks in the header to check for a login
					$rootScope.$emit('login')
					//redirect
					var returnUrl = sessionStorage.getItem('returnUrl') != null ? sessionStorage.getItem('returnUrl') : '/';
					sessionStorage.removeItem('returnUrl');
					$location.path(returnUrl);
				},
				function (error) {
					$scope.message = error.error_description;
				});
		}

		$scope.registerClick = function () {
			authService.saveRegistration($scope.registration)
				.then(function (response) {
					$scope.savedSuccessfully = true;
					$scope.message = "User has been registered successfully, you will be redicted to login page in 2 seconds.";
					startTimer();
				},
				function (response) {
					var errors = [];
					for (var key in response.data.modelState) {
						for (var i = 0; i < response.data.modelState[key].length; i++) {
							errors.push(response.data.modelState[key][i]);
						}
					}
					$scope.error = "Failed to register user due to:" + errors.join(' ');
				});
		}

		var startTimer = function () {
			var timer = $timeout(function () {
				$timeout.cancel(timer);
				$location.path('/login');
			}, 2000);
		}
	}
]);
