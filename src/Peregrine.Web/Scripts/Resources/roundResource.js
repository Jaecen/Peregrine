angular
.module('peregrineUi.resources')
.factory('roundResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey/rounds/:roundNumber', { roundNumber: '@number' }, {
			save: {
				method: 'put'
			}
		});
	}
])
