angular
.module('peregrineUi.controllers')
.controller('loginController', [
	'$scope', '$location', '$rootScope', '$timeout', 'authService', 'externalLoginResource',
	function ($scope, $location, $rootScope, $timeout, authService, externalLoginResource) {
		$scope.error = '';

		$scope.registration = {
			email: "",
			password: ""
		};

		$scope.loginData = {
			userName: "",
			password: ""
		};

		$scope.externalLogins = [];

		externalLoginResource.get(
				{},
				function success(externalLogins) {
					$scope.error = '';
					for (var i = 0; i < externalLogins.length; i++) {
						$scope.externalLogins.push(externalLogins[i]);
					}
				},
				function error() {
					$scope.error = 'So apparently you can\'t login right now. Uh... try again?';
				});

		$scope.loginClick = function () {
			authService.login($scope.loginData)
				.then(function (response) {
					$scope.message = 'Hey nice work you remembered! Bubbye.';
					afterLogin();
				},
				function (error) {
					$scope.message = error.error_description;
				});
		}

		$scope.registerClick = function () {
			authService.saveRegistration($scope.registration)
				.then(
					function (response) {
						$scope.error = '';
						var loginData = {
								userName: $scope.registration.email,
								password: $scope.registration.password
							};
					
						authService.login(loginData)
						.then(
							function success(response, status) {
								$scope.message = 'Nice! you\'re registered and logged in. Now get off my login screen!';
								afterLogin();
							},
							function error(error, status) {
								$scope.error('We were able to create your account, but we failed to log you in')
							});
					},
					function (response) {
						var errors = [];
						for (var key in response.data.ModelState) {
							for (var i = 0; i < response.data.ModelState[key].length; i++) {
								errors.push(response.data.ModelState[key][i]);
							}
						}
						$scope.error = 'Sorry that didn\'t work out. : ' + errors.join(' ');
					});
		}

		var afterLogin = function () {
			//tell the userlinks in the header to check for a login
			$rootScope.$emit('login');
			$timeout(function () {
				redirect();
			}, 2500);
		}

		var redirect = function () {
			var returnUrl = sessionStorage.getItem('returnUrl') != null ? sessionStorage.getItem('returnUrl') : '/';
			sessionStorage.removeItem('returnUrl');
			$location.path(returnUrl);
		}
	}
]);
