angular
.module('peregrineUi.resources')
.factory('outcomeResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey/rounds/:roundNumber/:playerName/:outcome/:number', null, {
			save: {
				method: 'put'
			}
		});
	}
])
