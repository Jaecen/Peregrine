/// <reference path="authService.js" />
angular
.module('peregrineUi.factories')
.factory('authService', [
	'$http', '$q',
	function($http, $q) {
		var serviceBase = '/';
		var authServiceFactory = {};

		var _authentication = {
			isAuth: false,
			userName: ""
		};

		var _externalAuthData = {
			provider: "",
			userName: "",
			externalAccessToken: ""
		};

		var _saveRegistration = function(registration) {
			var deferred = $q.defer();
			$http({
				method: 'POST',
				url: serviceBase + 'api/account/register',
				headers: { 'Content-Type': 'application/json' },
				data: registration
			})
			.success(function(response) {
				deferred.resolve(response);
			})
			.error(function(error, status) {
				deferred.reject(error);
			});
			return deferred.promise;
		};

		var _login = function(loginData) {
			var deferred = $q.defer();
			$http({
				method: 'POST',
				url: '/token',
				headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
				transformRequest: function(obj) {
					var str = [];
					for(var p in obj)
						str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
					return str.join("&");
				},
				data: { grant_type: 'password', username: loginData.userName, password: loginData.password }
			})
			.success(function(response) {
				sessionStorage.setItem('accessToken', response.access_token);
				sessionStorage.setItem('userName', loginData.userName);
				_authentication.isAuth = true;
				_authentication.userName = loginData.userName;
				deferred.resolve(response);
			})
			.error(function(error, status) {
				_logOut();
				deferred.reject(error);
			});
			return deferred.promise;
		};

		var _logOut = function() {
			var deferred = $q.defer();
			$http({
				method: 'POST',
				url: '/api/account/logout',
				headers: { 'Content-Type': 'application/json' }
			})
			.success(function(response, status) {
				sessionStorage.removeItem('accessToken');
				sessionStorage.removeItem('userName');
				_authentication.isAuth = false;
				_authentication.userName = "";
				deferred.resolve(response);
			})
			.error(function(error, status) {
				deferred.reject(error);
			})
			return deferred.promise;
		};

		var _fillAuthData = function() {
			var accessToken = sessionStorage.getItem('accessToken');
			var userName = sessionStorage.getItem('userName');
			if(accessToken && userName) {
				_authentication.isAuth = true;
				_authentication.userName = userName;
			}
		}

		var _obtainAccessToken = function(externalData) {

			var deferred = $q.defer();

			$http
				.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } })
				.success(function(response) {
					sessionStorage.setItem('accessToken', response.access_token);

					_authentication.isAuth = true;
					_authentication.userName = response.userName;
					_authentication.useRefreshTokens = false;

					deferred.resolve(response);
				}).error(function(err, status) {
					_logOut();
					deferred.reject(err);
				});

			return deferred.promise;

		};

		var _registerExternal = function(registerExternalData) {

			var deferred = $q.defer();

			$http
				.post(serviceBase + 'api/account/registerexternal', registerExternalData)
				.success(function(response) {

					sessionStorage.setItem('accessToken', response.access_token);
					sessionStorage.setItem('userName', response.userName);
					_authentication.isAuth = true;
					_authentication.userName = response.userName;
					deferred.resolve(response);
				})
				.error(function(err, status) {
					_logOut();
					deferred.reject(err);
				});

			return deferred.promise;
		};

		authServiceFactory.saveRegistration = _saveRegistration;
		authServiceFactory.login = _login;
		authServiceFactory.logOut = _logOut;
		authServiceFactory.fillAuthData = _fillAuthData;
		authServiceFactory.authentication = _authentication;

		authServiceFactory.obtainAccessToken = _obtainAccessToken;
		authServiceFactory.externalAuthData = _externalAuthData;
		authServiceFactory.registerExternal = _registerExternal;

		return authServiceFactory;
	}]);
