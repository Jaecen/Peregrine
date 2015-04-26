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

		var _response = function (response) {

			//var queryString = $location.search();
			//if(queryString.external_access_token && $location.path() != '/login') {
			//	//remove the querystring
			//	$location.search('');
			//	//store these in the session
			//	sessionStorage.setItem('externalAccessToken', queryString.external_access_token);
			//	sessionStorage.setItem('externalProvider', queryString.provider);
			//	sessionStorage.setItem('externalUserName', queryString.external_user_name);
			//	sessionStorage.setItem('hasLocalAccount', queryString.haslocalaccount);
			//	//let the login controller handle the rest
			//	$location.path('/login')
			//}

			return response;
		}

		var _responseError = function (rejection) {
			if (rejection.status === 401) {
				sessionStorage.setItem('returnUrl', $location.path())
				$location.path('/login');
			}
			return $q.reject(rejection);
		}

		authInterceptorServiceFactory.request = _request;
		authInterceptorServiceFactory.response = _response;
		authInterceptorServiceFactory.responseError = _responseError;

		return authInterceptorServiceFactory;
	}]);
