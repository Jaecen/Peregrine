angular
.module('peregrineUi.controllers')
.controller('loginController', [
	'$scope', '$location', '$rootScope', '$timeout', 'authService', 'externalLoginResource', '$routeParams',
	function ($scope, $location, $rootScope, $timeout, authService, externalLoginResource, $routeParams) {
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
			$location.url(returnUrl);
		}

		var myParams = $routeParams;

		//handle external login signins
		if($routeParams.externalAccessToken) {
			var hasLocalAccount = $routeParams.hasLocalAccount;
			var provider = $routeParams.provider;
			var userName = $routeParams.externalUserName;
			var externalAccessToken = $routeParams.externalAccessToken;

			if(hasLocalAccount == 'False') {
				//This user has authed with Google but has no local account yet
				authService.logOut();

				authService.externalAuthData = {
					provider: provider,
					userName: userName,
					externalAccessToken: externalAccessToken
				};

				$location.url('/associate');
			}
			else {
				//Obtain a local access token and redirect back to where they came frome
				var externalData = { provider: provider, externalAccessToken: externalAccessToken };
				authService.obtainAccessToken(externalData)
					.then(function (response) {
						$scope.message = 'Okay if you\'re cool with ' + provider + ' you\'re cool with us.';
						afterLogin();
					},
					function (error) {
						$scope.message = error.error_description;
					});
			}
		}

	}
]);
