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
			var email = $routeParams.email;

			if(hasLocalAccount == 'False') {
				//This user has authed with Google but has no local account yet
				authService.logOut();

				authService.externalAuthData = {
					provider: provider,
					userName: userName,
					externalAccessToken: externalAccessToken,
					email: email
				};

				$location.url('/associate');
			}
			else {
				//Obtain a local access token and redirect back to where they came frome
				var externalData = { provider: provider, externalAccessToken: externalAccessToken };
				authService.obtainAccessToken(externalData)
					.then(function(response) {
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
