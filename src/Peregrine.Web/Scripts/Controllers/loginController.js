angular
.module('peregrineUi.controllers')
.controller('loginController', [
	'$scope', '$http', '$location', '$rootScope',
	function ($scope, $http, $location, $rootScope) {
		$scope.error = '';

		$scope.loginClick = function () {
			if ($scope.loginEmail === undefined)
				$scope.error = 'Whoops looks like you missed the email box there!';
			if ($scope.loginPassword === undefined)
				$scope.error = 'Yeah sorry we\'re gonna need your password on that.';

			login($scope.loginEmail, $scope.loginPassword);
		}

		$scope.registerClick = function (email, password) {
			if ($scope.registerEmail === undefined)
				$scope.error = 'Whoops looks like you missed the email box there!';
			if ($scope.registerPassword === undefined)
				$scope.error = 'Yeah sorry we\'re gonna need your password on that.';

			$http({
				method: 'POST',
				url: '/api/account/register',
				headers: { 'Content-Type': 'application/json' },
				data: { email: $scope.registerEmail, password: $scope.registerPassword }
			})
			.success(function (data, status, headers, config) {
				$scope.login($scope.registerEmail, $scope.registerPassword)
			}).error(function (data, status, headers, config) {
				$scope.error = data.ModelState[""][0];
			})
		}

		function login(email, password){
			$http({
				method: 'POST',
				url: '/Token',
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
				transformRequest: function (obj) {
					var str = [];
					for (var p in obj)
						str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
					return str.join("&");
				},
				data: { grant_type: 'password', username: email, password: password }
			})
			.success(function (data, status, headers, config) {
				sessionStorage.setItem("accessToken", data.access_token);
				sessionStorage.setItem("userName", data.userName);
				//tell the userlinks in the header to check for a login
				$rootScope.$emit('login')
				$location.path('/');
			}).error(function (data, status, headers, config) {
				$scope.error = status;
			})
		}
	}
]);
