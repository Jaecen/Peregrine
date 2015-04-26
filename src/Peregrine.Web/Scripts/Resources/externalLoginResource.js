angular
.module('peregrineUi.resources')
.factory('externalLoginResource', [
	'$resource',
	function($resource) {
		return $resource('/api/Account/ExternalLogins?returnurl=:returnUrl&generateState=true', { returnUrl: '/' }, {
			get: {
				isArray: true //this is necessary because the default account controller returns and array rather than an object. 
			},
		});
	}
])
