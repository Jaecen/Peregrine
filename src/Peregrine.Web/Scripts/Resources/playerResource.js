angular
.module('peregrineUi.resources')
.factory('playerResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey/players/:playerName', { playerName: '@name' }, {
			save: {
				method: 'put'
			}
		});
	}
])