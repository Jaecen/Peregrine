angular
.module('peregrineUi.resources')
.factory('standingsResource', [
	'$resource',
	function($resource) {
		return $resource('/api/tournaments/:tournamentKey/standings');
	}
])
