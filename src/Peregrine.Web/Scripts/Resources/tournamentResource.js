angular
.module('peregrineUi.resources')
.factory('tournamentResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey', { tournamentKey: '@key' }, {
			create: {
				method: 'post'
			},
			save: {
				method: 'put'
			}
		});
	}
])
