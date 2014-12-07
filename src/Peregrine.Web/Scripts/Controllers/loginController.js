angular
.module('peregrineUi.controllers')
.controller('loginController', [
	'$scope', '$http', '$location',
	function ($scope, $http, $location) {
		$scope.error = '';

		$scope.login = function () {
			if ($scope.loginEmail === undefined)
				$scope.error = 'Whoops looks like you missed the email box there!';
			if ($scope.loginPassword === undefined)
				$scope.error = 'Yeah sorry we\'re gonna need your password on that.';

			$http({
				method: 'POST',
				url: '/Token',
				headers: {'Content-Type': 'application/x-www-form-urlencoded'},
				transformRequest: function(obj) {
					var str = [];
					for(var p in obj)
						str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
					return str.join("&");
				},
				data: { grant_type: 'password', username: $scope.loginEmail, password: $scope.loginPassword }
			})
			.success(function (data, status, headers, config) {
				console.log(data.access_token);
				sessionStorage.setItem("access_token", data.access_token);
			}).error(function (data, status, headers, config) {
				$scope.error = status;
			})
		}

		$scope.register = function (email, password) {
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
				$location.path('/');
			}).error(function (data, status, headers, config) {
				$scope.error = data.ModelState[""][0];
			})
		}
	}
]);
