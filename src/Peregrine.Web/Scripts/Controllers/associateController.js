angular
.module('peregrineUi.controllers')
.controller('associateController', ['$scope', '$rootScope', '$location', '$timeout', 'authService', function ($scope, $rootScope, $location, $timeout, authService) {

	$scope.savedSuccessfully = false;
	$scope.message = "";

	$scope.registerData = {
		userName: "",
		provider: authService.externalAuthData.provider,
		externalAccessToken: authService.externalAuthData.externalAccessToken
	};

	$scope.registerExternal = function () {

		authService.registerExternal($scope.registerData)
			.then(function (response) {
				$scope.savedSuccessfully = true;
				$scope.message = "Thanks! Okay where were you...";
				afterLogin();
			},
			function (response) {
				var errors = [];
				for(var key in response.ModelState) {
					errors.push(response.ModelState[key]);
				}
				if(errors.length > 0) {
					$scope.message = "Failed to register user due to: " + errors.join(' ');
				}
				else {
					$scope.message = response.Message;
				}
			});
	};

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

}]);