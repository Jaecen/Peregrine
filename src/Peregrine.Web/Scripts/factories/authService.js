angular
.module('peregrineUi.factories')
.factory('authService', [
	'$http', '$q',
	function ($http, $q) {
		var serviceBase = '/';
		var authServiceFactory = {};

		var _authentication = {
			isAuth: false,
			userName: ""
		};

		var _saveRegistration = function (registration) {
			_logOut();
			return $http.post(serviceBase + 'api/account/register', registration)
				.then(function (response) {
				return response;
			});

		};

		var _login = function (loginData) {
			var deferred = $q.defer();
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
				data: { grant_type: 'password', username: loginData.userName, password: loginData.password }
			})
			.success(function (response) {
				sessionStorage.setItem('accessToken', response.access_token);
				sessionStorage.setItem('userName', loginData.userName);
				_authentication.isAuth = true;
				_authentication.userName = loginData.userName;
				deferred.resolve(response);
			})
			.error(function (error, status) {
				_logOut();
				deferred.reject(error);
			});
			return deferred.promise;
		};

		var _logOut = function () {
			var deferred = $q.defer();
			$http({
				method: 'POST',
				url: '/api/account/logout',
				headers: { 'Content-Type': 'application/json' }
			})
			.success(function (response, status) {
				sessionStorage.removeItem('accessToken');
				sessionStorage.removeItem('userName');
				_authentication.isAuth = false;
				_authentication.userName = "";
				deferred.resolve(response);
			})
			.error(function (error, status) {
				deferred.reject(error);
			})
			return deferred.promise;
		};

		var _fillAuthData = function () {
			var accessToken = sessionStorage.getItem('accessToken');
			var userName = sessionStorage.getItem('userName');
			if (accessToken && userName) {
				_authentication.isAuth = true;
				_authentication.userName = userName;
			}
		}

		authServiceFactory.saveRegistration = _saveRegistration;
		authServiceFactory.login = _login;
		authServiceFactory.logOut = _logOut;
		authServiceFactory.fillAuthData = _fillAuthData;
		authServiceFactory.authentication = _authentication;

		return authServiceFactory;
	}]);
