angular
.module('peregrineUi.factories')
.factory('authInterceptorService', [
	'$q', '$location',
	function ($q, $location) {
		var authInterceptorServiceFactory = {};

		var _request = function (config) {

			config.headers = config.headers || {};

			var accessToken = sessionStorage.getItem('accessToken');
			if (accessToken) {
				config.headers.Authorization = 'Bearer ' + accessToken;
			}

			return config;
		}

		var _responseError = function (rejection) {
			if (rejection.status === 401) {
				sessionStorage.setItem('returnUrl', $location.path())
				$location.path('/login');
			}
			return $q.reject(rejection);
		}

		authInterceptorServiceFactory.request = _request;
		authInterceptorServiceFactory.responseError = _responseError;

		return authInterceptorServiceFactory;
	}]);
