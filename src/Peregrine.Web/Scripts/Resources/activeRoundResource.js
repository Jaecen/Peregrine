angular
.module('peregrineUi.resources')
.factory('activeRoundResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey/rounds/active', { tournamentKey: '@tournamentKey' }, {
			set: {
				method: 'put'
			},
		});
	}
])
